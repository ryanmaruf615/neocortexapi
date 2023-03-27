// Copyright (c) Damir Dobric. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NeoCortexApi;
using NeoCortexApi.Entities;
using NeoCortexApi.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace UnitTestsProject
{
    [TestClass]
    public class TemporalPoolerTest3
    {
        private static bool areDisjoined<T>(ICollection<T> arr1, ICollection<T> arr2)
        {
            foreach (var item in arr1)
            {
                if (arr2.Contains(item))
                    return false;
            }

            return true;
        }
        private Parameters getDefaultParameters3()
        {
            Parameters retVal = Parameters.getTemporalDefaultParameters();
            retVal.Set(KEY.COLUMN_DIMENSIONS, new int[] { 32 });
            retVal.Set(KEY.CELLS_PER_COLUMN, 6);
            retVal.Set(KEY.ACTIVATION_THRESHOLD, 2);
            retVal.Set(KEY.INITIAL_PERMANENCE, 0.20);
            retVal.Set(KEY.CONNECTED_PERMANENCE, 0.20);
            retVal.Set(KEY.MIN_THRESHOLD, 2);
            retVal.Set(KEY.MAX_NEW_SYNAPSE_COUNT, 2);
            retVal.Set(KEY.PERMANENCE_INCREMENT, 0.20);
            retVal.Set(KEY.PERMANENCE_DECREMENT, 0.20);
            retVal.Set(KEY.PREDICTED_SEGMENT_DECREMENT, 0.0);
            retVal.Set(KEY.RANDOM, new ThreadSafeRandom(42));
            retVal.Set(KEY.SEED, 42);

            return retVal;
        }

        private HtmConfig GetDefaultTMParameters3()
        {
            HtmConfig htmConfig = new HtmConfig(new int[] { 32 }, new int[] { 32 })
            {
                CellsPerColumn = 6,
                ActivationThreshold = 2,
                InitialPermanence = 0.20,
                ConnectedPermanence = 0.20,
                MinThreshold = 2,
                MaxNewSynapseCount = 2,
                PermanenceIncrement = 0.20,
                PermanenceDecrement = 0.20,
                PredictedSegmentDecrement = 0,
                Random = new ThreadSafeRandom(42),
                RandomGenSeed = 42
            };

            return htmConfig;
        }

        [TestMethod]
        [Category("Prod")]
        public void TestNoneActiveColumns()
        {
            // Arrange
            var tm = new TemporalMemory();
            var cn = new Connections();
            getDefaultParameters3().apply(cn);
            tm.Init(cn);
            var cell6 = cn.GetCell(6);
            var activeSegment = cn.CreateDistalSegment(cell6);
            var synapses = new[] { 0, 1, 2, 3, 4, 5 }
                .Select(i => cn.GetCell(i))
                .Select(cell => cn.CreateSynapse(activeSegment, cell, 0.20))
                .ToList();

            // Act
            var cc1 = tm.Compute(new[] { 0 }, true) as ComputeCycle;
            var cc2 = tm.Compute(new int[0], true) as ComputeCycle;

            // Assert
            Assert.IsFalse(cc1.ActiveCells.Count == 0);
            Assert.IsFalse(cc1.WinnerCells.Count == 0);
            Assert.IsFalse(cc1.PredictiveCells.Count == 0);
            Assert.IsTrue(cc2.ActiveCells.Count == 0);
            Assert.IsTrue(cc2.WinnerCells.Count == 0);
            Assert.IsTrue(cc2.PredictiveCells.Count == 0);
        }


        [TestMethod]
        [TestCategory("Prod")]
        public void TestAdaptSegmentToCentre()
        {
            TemporalMemory tm = new TemporalMemory();
            Connections cn = new Connections();
            Parameters p = getDefaultParameters3();
            p.apply(cn);
            tm.Init(cn);

            DistalDendrite dd = cn.CreateDistalSegment(cn.GetCell(0));
            Synapse s1 = cn.CreateSynapse(dd, cn.GetCell(6), 0.8); // set initial permanence to 0.8

            TemporalMemory.AdaptSegment(cn, dd, cn.GetCells(new int[] { 6 }), 0.1, 0.1); // adjust permanence by 0.1 increment and decrement
            Assert.AreEqual(0.9, s1.Permanence, 0.1);

            // Now permanence should be at mean
            TemporalMemory.AdaptSegment(cn, dd, cn.GetCells(new int[] { 6 }), 0.1, 0.1); // adjust permanence by 0.1 increment and decrement
            Assert.AreEqual(1.0, s1.Permanence, 0.1);
        }


        [TestMethod]
        public void TestArrayNotContainingCells()
        {
            // Arrange
            HtmConfig htmConfig = GetDefaultTMParameters3();
            Connections cn = new Connections(htmConfig);
            TemporalMemory tm = new TemporalMemory();
            tm.Init(cn);

            int[] activeColumns = { 4, 5 };
            Cell[] burstingCells = cn.GetCells(new int[] { 0, 1, 2, 3, 4, 5 });

            // Act
            ComputeCycle cc = tm.Compute(activeColumns, true) as ComputeCycle;

            // Assert
            // Verify that ComputeCycle's ActiveCells array does not contain any cells from burstingCells array
            foreach (var cell in cc.ActiveCells)
            {
                Assert.IsFalse(cc.ActiveCells.SequenceEqual(burstingCells));
            }
        }


        [TestMethod]
        [TestCategory("Prod")]
        public void TestDestroySegmentsWithTooFewSynapsesToBeMatching()
        {
            TemporalMemory tm = new TemporalMemory();
            Connections cn = new Connections();
            Parameters p = GetDefaultParameters3(null, KEY.INITIAL_PERMANENCE, .2);
            p = GetDefaultParameters3(p, KEY.MAX_NEW_SYNAPSE_COUNT, 4);
            p = GetDefaultParameters3(p, KEY.PREDICTED_SEGMENT_DECREMENT, 0.02);
            p.apply(cn);
            tm.Init(cn);

            int[] prevActiveColumns = { 0 };
            Cell[] prevActiveCells = { cn.GetCell(0), cn.GetCell(1), cn.GetCell(2), cn.GetCell(3), cn.GetCell(4) };
            int[] activeColumns = { 2 };
            Cell expectedActiveCell = cn.GetCell(6);

            DistalDendrite matchingSegment = cn.CreateDistalSegment(cn.GetCell(6));
            cn.CreateSynapse(matchingSegment, prevActiveCells[0], .015);
            cn.CreateSynapse(matchingSegment, prevActiveCells[1], .015);
            cn.CreateSynapse(matchingSegment, prevActiveCells[2], .015);
            cn.CreateSynapse(matchingSegment, prevActiveCells[3], .015);
            cn.CreateSynapse(matchingSegment, prevActiveCells[4], .015);
            tm.Compute(prevActiveColumns, true);
            tm.Compute(activeColumns, true);

            Assert.AreEqual(0, cn.NumSegments(expectedActiveCell));
        }

        [TestMethod]
        [TestCategory("Prod")]
        public void TestNewSegmentAddSynapsesToAllWinnerCells()
        {
            TemporalMemory tm = new TemporalMemory();
            Connections cn = new Connections();
            Parameters p = GetDefaultParameters3(null, KEY.MAX_NEW_SYNAPSE_COUNT, 6);
            p.apply(cn);
            tm.Init(cn);

            int[] previousActiveColumns = { 0, 1, 2, 3, 4, 5 };
            int[] activeColumns = { 6 };

            ComputeCycle cc = tm.Compute(previousActiveColumns, true) as ComputeCycle;
            List<Cell> prevWinnerCells = new List<Cell>(cc.WinnerCells);
            Assert.AreEqual(6, prevWinnerCells.Count);

            cc = tm.Compute(activeColumns, true) as ComputeCycle;

            List<Cell> winnerCells = new List<Cell>(cc.WinnerCells);
            Assert.AreEqual(1, winnerCells.Count);

            //DD
            //List<DistalDendrite> segments = winnerCells[0].GetSegments(cn);
            List<DistalDendrite> segments = winnerCells[0].DistalDendrites;

            //List<DistalDendrite> segments = winnerCells[0].Segments;
            Assert.AreEqual(1, segments.Count);
            //List<Synapse> synapses = segments[0].GetAllSynapses(cn);
            List<Synapse> synapses = segments[0].Synapses;

            List<Cell> presynapticCells = new List<Cell>();
            foreach (Synapse synapse in synapses)
            {
                Assert.AreEqual(0.25, synapse.Permanence, 0.05);
                presynapticCells.Add(synapse.GetPresynapticCell());
            }

            presynapticCells.Sort();

            Assert.IsTrue(prevWinnerCells.SequenceEqual(presynapticCells));
        }

        [TestMethod]
        [TestCategory("Prod")]
        [DataRow(0)]
        [DataRow(1)]
        public void TestActivateCorrectlyPredictiveCells(int tmImplementation)
        {
            // Arrange
            TemporalMemory tm = tmImplementation == 0 ? new TemporalMemory() : new TemporalMemoryMT();
            Connections cn = new Connections();
            Parameters p = getDefaultParameters3();
            p.apply(cn);
            tm.Init(cn);

            int[] previousActiveColumns = { 0 };
            int[] activeColumns = { 1 };

            Cell cell6 = cn.GetCell(6);
            DistalDendrite activeSegment = cn.CreateDistalSegment(cell6);

            cn.CreateSynapse(activeSegment, cn.GetCell(0), 0.20);
            cn.CreateSynapse(activeSegment, cn.GetCell(1), 0.20);
            cn.CreateSynapse(activeSegment, cn.GetCell(2), 0.20);
            cn.CreateSynapse(activeSegment, cn.GetCell(3), 0.20);
            cn.CreateSynapse(activeSegment, cn.GetCell(4), 0.20);
            cn.CreateSynapse(activeSegment, cn.GetCell(5), 0.20);

            ISet<Cell> expectedActiveCells = new HashSet<Cell>(new Cell[] { cell6 });

            // Act
            ComputeCycle cc1 = tm.Compute(previousActiveColumns, true) as ComputeCycle;
            ComputeCycle cc2 = tm.Compute(activeColumns, true) as ComputeCycle;

            // Assert
            Assert.IsTrue(cc1.PredictiveCells.SequenceEqual(expectedActiveCells));
            Assert.IsTrue(cc2.ActiveCells.SequenceEqual(expectedActiveCells));
        }


        



        


        

        
        
        
        [TestMethod]
        [TestCategory("Prod")]
        public void TestNoNewSegmentIfNotEnoughWinnerCells()
        {
            TemporalMemory tm = new TemporalMemory();
            Connections cn = new Connections();
            Parameters p = GetDefaultParameters3(null, KEY.MAX_NEW_SYNAPSE_COUNT, 6);
            p.apply(cn);
            tm.Init(cn);

            int[] zeroColumns = { };
            int[] activeColumns = { 0 };

            tm.Compute(zeroColumns, true);
            tm.Compute(activeColumns, true);

            Assert.AreEqual(0, cn.NumSegments(), 0);
        }
        
        [TestMethod]
        [TestCategory("Prod")]
        public void TestDestroyWeakSynapseOnActiveReinforce()
        {
            TemporalMemory tm = new TemporalMemory();
            Connections cn = new Connections();
            Parameters p = GetDefaultParameters3(null, KEY.INITIAL_PERMANENCE, 0.3);
            p = GetDefaultParameters3(p, KEY.MAX_NEW_SYNAPSE_COUNT, 6);
            p = GetDefaultParameters3(p, KEY.PREDICTED_SEGMENT_DECREMENT, 0.04);
            p.apply(cn);
            tm.Init(cn);

            int[] previousActiveColumns = { 0 };
            Cell[] previousActiveCells = { cn.GetCell(0), cn.GetCell(1), cn.GetCell(2), cn.GetCell(3), cn.GetCell(4), cn.GetCell(5) };
            int[] activeColumns = { 5 };
            Cell expectedActiveCell = cn.GetCell(6);

            DistalDendrite activeSegment = cn.CreateDistalSegment(expectedActiveCell);
            cn.CreateSynapse(activeSegment, previousActiveCells[0], 0.5);
            cn.CreateSynapse(activeSegment, previousActiveCells[1], 0.5);
            cn.CreateSynapse(activeSegment, previousActiveCells[2], 0.5);
            cn.CreateSynapse(activeSegment, previousActiveCells[3], 0.5);
            cn.CreateSynapse(activeSegment, previousActiveCells[4], 0.5);
            // Weak Synapse
            cn.CreateSynapse(activeSegment, previousActiveCells[5], 0.009);

            tm.Compute(previousActiveColumns, true);
            tm.Compute(activeColumns, true);

            Assert.AreEqual(5, activeSegment.Synapses.Count);
        }
        [TestMethod]
        [TestCategory("Prod")]
        public void TestBurstNotpredictedColumns()
        {
            TemporalMemory tm = new TemporalMemory();
            Connections cn = new Connections();
            Parameters p = getDefaultParameters3();
            p.apply(cn);
            tm.Init(cn);

            int[] activeColumns = { 1, 2, 3 }; //Cureently Active column
            IList<Cell> burstingCells = cn.GetCells(new int[] { 0, 1, 2, 3, 4, 5 }); //Number of Cell Indexs

            ComputeCycle cc = tm.Compute(activeColumns, true) as ComputeCycle; //COmpute class object 

            Assert.IsFalse(cc.ActiveCells.SequenceEqual(burstingCells));
        }
        // This is a unit test for the TemporalMemory class in C#


        private Parameters GetDefaultParameters3(Parameters p, string key, Object value)
        {
            Parameters retVal = p == null ? getDefaultParameters3() : p;
            retVal.Set(key, value);

            return retVal;
        }



    }
}
