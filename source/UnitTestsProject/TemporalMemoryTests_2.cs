// Copyright (c) Damir Dobric. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NeoCortexApi;
using NeoCortexApi.Entities;
using NeoCortexApi.Types;
using System;
using System.Collections.Generic;
using System.Linq;

namespace UnitTestsProject
{
    [TestClass]
    public class TemporalPoolerTest2
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
        private Parameters getDefaultParameters2()
        {
            Parameters retVal = Parameters.getTemporalDefaultParameters();
            retVal.Set(KEY.COLUMN_DIMENSIONS, new int[] { 32 });
            retVal.Set(KEY.CELLS_PER_COLUMN, 6);
            retVal.Set(KEY.ACTIVATION_THRESHOLD, 3);
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

        private HtmConfig GetDefaultTMParameters2()
        {
            HtmConfig htmConfig = new HtmConfig(new int[] { 32 }, new int[] { 32 })
            {
                CellsPerColumn = 6,
                ActivationThreshold = 3,
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


        private Parameters GetDefaultParameters2(Parameters p, string key, Object value)
        {
            Parameters retVal = p == null ? getDefaultParameters2() : p;
            retVal.Set(key, value);

            return retVal;
        }


        [TestMethod]
        [TestCategory("Prod")]
        [DataRow(0)]
        [DataRow(1)]
        public void TestActivateCorrectlyPredictiveCells(int tmImplementation)
        {
            TemporalMemory tm = tmImplementation == 0 ? new TemporalMemory() : new TemporalMemoryMT();
            Connections cn = new Connections();
            Parameters p = getDefaultParameters2();
            p.apply(cn);
            tm.Init(cn);

            int[] previousActiveColumns = { 0 };
            int[] activeColumns = { 1 };

            // Cell6 belongs to column with index 1.
            Cell cell6 = cn.GetCell(6);

            // ISet<Cell> expectedActiveCells = Stream.of(cell6).collect(Collectors.toSet());
            ISet<Cell> expectedActiveCells = new HashSet<Cell>(new Cell[] { cell6 });

            // We add distal dentrite at column1.cell6
            DistalDendrite activeSegment = cn.CreateDistalSegment(cell6);

            //
            // We add here synapses between column0.cells[0-5] and segment.
            cn.CreateSynapse(activeSegment, cn.GetCell(0), 0.20);
            cn.CreateSynapse(activeSegment, cn.GetCell(1), 0.20);
            cn.CreateSynapse(activeSegment, cn.GetCell(2), 0.20);
            cn.CreateSynapse(activeSegment, cn.GetCell(3), 0.20);
            cn.CreateSynapse(activeSegment, cn.GetCell(4), 0.20);
            cn.CreateSynapse(activeSegment, cn.GetCell(5), 0.20);

            ComputeCycle cc = tm.Compute(previousActiveColumns, true) as ComputeCycle;
            Assert.IsTrue(cc.PredictiveCells.SequenceEqual(expectedActiveCells));

            ComputeCycle cc2 = tm.Compute(activeColumns, true) as ComputeCycle;
            Assert.IsTrue(cc2.ActiveCells.SequenceEqual(expectedActiveCells));
        }



        [TestMethod]
        [TestCategory("Prod")]
        public void TestNumberOfColumns()
        {
           
            var tm = new TemporalMemory();
            var cn = new Connections();
            var p = Parameters.getAllDefaultParameters();
            p.Set(KEY.COLUMN_DIMENSIONS, new int[] { 62, 62 });
            p.Set(KEY.CELLS_PER_COLUMN, 30);
            p.apply(cn);
            tm.Init(cn);

            
            var actualNumColumns = cn.HtmConfig.NumColumns;
            var expectedNumColumns = 62 * 62;

            Assert.AreEqual(expectedNumColumns, actualNumColumns);
        }

        [TestMethod]
        [TestCategory("Prod")]
        public void TestWithTwoActiveColumns()
        {
            TemporalMemory tm = new TemporalMemory();
            Connections cn = new Connections();
            Parameters p = getDefaultParameters2();
            p.apply(cn);
            tm.Init(cn);

            int[] previousActiveColumns = { 4, 5 }; 
            Cell cell6 = cn.GetCell(7); 
            Cell cell7 = cn.GetCell(8);

            DistalDendrite activeSegment = cn.CreateDistalSegment(cell6);
           
            cn.CreateSynapse(activeSegment, cn.GetCell(0), 0.20);
            cn.CreateSynapse(activeSegment, cn.GetCell(1), 0.20);
            cn.CreateSynapse(activeSegment, cn.GetCell(2), 0.20);
            cn.CreateSynapse(activeSegment, cn.GetCell(3), 0.20);
            cn.CreateSynapse(activeSegment, cn.GetCell(4), 0.20);
            cn.CreateSynapse(activeSegment, cn.GetCell(5), 0.20);



            ComputeCycle cc = tm.Compute(previousActiveColumns, true) as ComputeCycle;
            Assert.IsFalse(cc.ActiveCells.Count == 0);
            Assert.IsFalse(cc.WinnerCells.Count == 0);
            Assert.IsTrue(cc.PredictiveCells.Count == 0);

            int[] zeroColumns = new int[0];
            ComputeCycle cc2 = tm.Compute(zeroColumns, true) as ComputeCycle; 
            Assert.IsTrue(cc2.ActiveCells.Count == 0); 
            Assert.IsTrue(cc2.WinnerCells.Count == 0);  
            Assert.IsTrue(cc2.PredictiveCells.Count == 0); 
        }

        [TestMethod]
        public void TestBurstUnpredictedColumnsforSixCells()
        {
            TemporalMemory tm = new TemporalMemory();
            Connections cn = new Connections();
            Parameters p = getDefaultParameters2();
            p.apply(cn);
            tm.Init(cn);

            int[] activeColumns = { 0 };
            var burstingCells = cn.GetCells(new int[] { 0, 1, 2, 3, 4, 5 });

            ComputeCycle cc = tm.Compute(activeColumns, true) as ComputeCycle;

            Assert.IsTrue(cc.ActiveCells.SequenceEqual(burstingCells));
        }

        [TestMethod]
        [TestCategory("Prod")]
        public void TestNoNewSegmentIfNotEnoughWinnerCells()
        {
            TemporalMemory tm = new TemporalMemory();
            Connections cn = new Connections();
            Parameters p = GetDefaultParameters2(null, KEY.MAX_NEW_SYNAPSE_COUNT, 5);
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
        public void TestNewSegmentAddSynapsesToSubsetOfWinnerCells()
        {
            TemporalMemory tm = new TemporalMemory();
            Connections cn = new Connections();
            Parameters p = GetDefaultParameters2(null, KEY.MAX_NEW_SYNAPSE_COUNT, 2);
            p.apply(cn);
            tm.Init(cn);

            int[] previousActiveColumns = { 0, 1, 2, 3, 4 };
            int[] activeColumns = { 4 };

            ComputeCycle cc = tm.Compute(previousActiveColumns, true) as ComputeCycle;

            IList<Cell> prevWinnerCells = cc.WinnerCells;
            Assert.AreEqual(5, prevWinnerCells.Count);

            cc = tm.Compute(activeColumns, true) as ComputeCycle;

            List<Cell> winnerCells = new List<Cell>(cc.WinnerCells);
            Assert.AreEqual(1, winnerCells.Count);

            //DD
            //List<DistalDendrite> segments = winnerCells[0].GetSegments(cn);
            List<DistalDendrite> segments = winnerCells[0].DistalDendrites;
            //List<DistalDendrite> segments = winnerCells[0].Segments;
            Assert.AreEqual(1, segments.Count);

            //DD List<Synapse> synapses = cn.GetSynapses(segments[0]);
            List<Synapse> synapses = segments[0].Synapses;
            Assert.AreEqual(2, synapses.Count);

            foreach (Synapse synapse in synapses)
            {
                Assert.AreEqual(0.23, synapse.Permanence, 0.05);
                Assert.IsTrue(prevWinnerCells.Contains(synapse.GetPresynapticCell()));
            }
        }


        [TestMethod]
        [TestCategory("Prod")]
        public void TestNewSegmentAddSynapsesToAllWinnerCells()
        {
            TemporalMemory tm = new TemporalMemory();
            Connections cn = new Connections();
            Parameters p = GetDefaultParameters2(null, KEY.MAX_NEW_SYNAPSE_COUNT, 5);
            p.apply(cn);
            tm.Init(cn);

            int[] previousActiveColumns = { 0, 1, 2, 3, 4 };
            int[] activeColumns = { 5 };

            ComputeCycle cc = tm.Compute(previousActiveColumns, true) as ComputeCycle;
            List<Cell> prevWinnerCells = new List<Cell>(cc.WinnerCells);
            Assert.AreEqual(5, prevWinnerCells.Count);

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
                Assert.AreEqual(0.23, synapse.Permanence, 0.05);
                presynapticCells.Add(synapse.GetPresynapticCell());
            }

            presynapticCells.Sort();

            Assert.IsTrue(prevWinnerCells.SequenceEqual(presynapticCells));
        }

        [TestMethod]
        [TestCategory("Prod")]
        public void TestMatchingSegmentAddSynapsesToAllWinnerCells()
        {
            TemporalMemory tm = new TemporalMemory();
            Connections cn = new Connections();
            Parameters p = GetDefaultParameters2(null, KEY.CELLS_PER_COLUMN, 1);
            p = GetDefaultParameters2(p, KEY.MIN_THRESHOLD, 1);
            p.apply(cn);
            tm.Init(cn);

            int[] previousActiveColumns = { 0, 1 };
            IList<Cell> prevWinnerCells = cn.GetCells(new int[] { 0, 1 });
            int[] activeColumns = { 4 };

            DistalDendrite matchingSegment = cn.CreateDistalSegment(cn.GetCell(4));
            cn.CreateSynapse(matchingSegment, cn.GetCell(0), 0.5);

            ComputeCycle cc = tm.Compute(previousActiveColumns, true) as ComputeCycle;
            Assert.IsTrue(cc.WinnerCells.SequenceEqual(prevWinnerCells));

            cc = tm.Compute(activeColumns, true) as ComputeCycle;

            //DD List<Synapse> synapses = cn.GetSynapses(matchingSegment);
            List<Synapse> synapses = matchingSegment.Synapses;
            Assert.AreEqual(2, synapses.Count);

            synapses.Sort();

            foreach (Synapse synapse in synapses)
            {
                if (synapse.GetPresynapticCell().Index == 0) continue;

                Assert.AreEqual(0.25, synapse.Permanence, 0.05);
                Assert.AreEqual(1, synapse.GetPresynapticCell().Index);
            }
        }


        [TestMethod]
        public void TestActiveSegmentGrowSynapsesAccordingToPotentialOverlap()
        {
            TemporalMemory tm = new TemporalMemory();
            Connections cn = new Connections();
            Parameters p = GetDefaultParameters2(null, KEY.CELLS_PER_COLUMN, 1);
            p = GetDefaultParameters2(p, KEY.MIN_THRESHOLD, 1);
            p = GetDefaultParameters2(p, KEY.ACTIVATION_THRESHOLD, 2);
            p = GetDefaultParameters2(p, KEY.MAX_NEW_SYNAPSE_COUNT, 4);
            p.apply(cn);
            tm.Init(cn);

            // Use 1 cell per column so that we have easy control over the winner cells.
            int[] previousActiveColumns = { 0, 1, 2, 3, 4 };
            List<Cell> prevWinnerCells = new List<Cell>(new Cell[] { cn.GetCell(0), cn.GetCell(1), cn.GetCell(2), cn.GetCell(3), cn.GetCell(4) });

            int[] activeColumns = { 5 };

            DistalDendrite activeSegment = cn.CreateDistalSegment(cn.GetCell(5));
            cn.CreateSynapse(activeSegment, cn.GetCell(0), 0.5);
            cn.CreateSynapse(activeSegment, cn.GetCell(1), 0.5);
            cn.CreateSynapse(activeSegment, cn.GetCell(2), 0.2);

            ComputeCycle cc = tm.Compute(previousActiveColumns, true) as ComputeCycle;
            Assert.IsTrue(prevWinnerCells.SequenceEqual(cc.WinnerCells));
            cc = tm.Compute(activeColumns, true) as ComputeCycle;

            List<Cell> presynapticCells = new List<Cell>();
            foreach (var syn in activeSegment.Synapses)
            {
                presynapticCells.Add(syn.GetPresynapticCell());
            }

            Assert.IsTrue(
                presynapticCells.Count == 4 && (
                (presynapticCells.Contains(cn.GetCell(0)) && presynapticCells.Contains(cn.GetCell(1)) && presynapticCells.Contains(cn.GetCell(2)) && presynapticCells.Contains(cn.GetCell(3))) ||
                (presynapticCells.Contains(cn.GetCell(0)) && presynapticCells.Contains(cn.GetCell(1)) && presynapticCells.Contains(cn.GetCell(2)) && presynapticCells.Contains(cn.GetCell(4)))));
        }


        [TestMethod]
        [TestCategory("Prod")]
        public void TestDestroyWeakSynapseOnWrongPrediction()
        {
            TemporalMemory tm = new TemporalMemory();
            Connections cn = new Connections();
            Parameters p = GetDefaultParameters2(null, KEY.INITIAL_PERMANENCE, 0.3);
            p = GetDefaultParameters2(p, KEY.MAX_NEW_SYNAPSE_COUNT, 4);
            p = GetDefaultParameters2(p, KEY.PREDICTED_SEGMENT_DECREMENT, 0.02);
            p.apply(cn);
            tm.Init(cn);

            int[] previousActiveColumns = { 0 };
            Cell[] previousActiveCells = { cn.GetCell(0), cn.GetCell(1), cn.GetCell(2), cn.GetCell(3) };
            int[] activeColumns = { 2 };
            Cell expectedActiveCell = cn.GetCell(5);

            DistalDendrite activeSegment = cn.CreateDistalSegment(expectedActiveCell);
            cn.CreateSynapse(activeSegment, previousActiveCells[0], 0.5);
            cn.CreateSynapse(activeSegment, previousActiveCells[1], 0.5);
            cn.CreateSynapse(activeSegment, previousActiveCells[2], 0.5);
            // Weak Synapse
            cn.CreateSynapse(activeSegment, previousActiveCells[3], 0.017);

            tm.Compute(previousActiveColumns, true);
            tm.Compute(activeColumns, true);

            Assert.AreEqual(3, activeSegment.Synapses.Count);
        }

        [TestMethod]
        [TestCategory("Prod")]
        public void TestDestroyWeakSynapseOnActiveReinforce()
        {
            TemporalMemory tm = new TemporalMemory();
            Connections cn = new Connections();
            Parameters p = GetDefaultParameters2(null, KEY.INITIAL_PERMANENCE, 0.3);
            p = GetDefaultParameters2(p, KEY.MAX_NEW_SYNAPSE_COUNT, 4);
            p = GetDefaultParameters2(p, KEY.PREDICTED_SEGMENT_DECREMENT, 0.02);
            p.apply(cn);
            tm.Init(cn);

            int[] previousActiveColumns = { 0 };
            Cell[] previousActiveCells = { cn.GetCell(0), cn.GetCell(1), cn.GetCell(2), cn.GetCell(3), cn.GetCell(4) };
            int[] activeColumns = { 2 };
            Cell expectedActiveCell = cn.GetCell(5);

            DistalDendrite activeSegment = cn.CreateDistalSegment(expectedActiveCell);
            cn.CreateSynapse(activeSegment, previousActiveCells[0], 0.5);
            cn.CreateSynapse(activeSegment, previousActiveCells[1], 0.5);
            cn.CreateSynapse(activeSegment, previousActiveCells[2], 0.5);
            cn.CreateSynapse(activeSegment, previousActiveCells[3], 0.5);
            // Weak Synapse
            Synapse weakSynapse = cn.CreateSynapse(activeSegment, previousActiveCells[4], 0.006);

            tm.Compute(previousActiveColumns, true);
            tm.Compute(activeColumns, true);

            Assert.IsFalse(activeSegment.Synapses.Contains(weakSynapse));
        }

    }

}

