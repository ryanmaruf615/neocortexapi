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
            retVal.Set(KEY.CELLS_PER_COLUMN, 5);
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
                CellsPerColumn = 5,
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
        [TestCategory("Prod")]
        [DataRow(0)]
        [DataRow(1)]
        public void TestActivateCorrectlyPredictiveCells(int tmImplementation)
        {
            TemporalMemory tm = tmImplementation == 0 ? new TemporalMemory() : new TemporalMemoryMT();
            Connections cn = new Connections();
            Parameters p = getDefaultParameters3();
            p.apply(cn);
            tm.Init(cn);

            int[] previousActiveColumns = { 0 };
            int[] activeColumns = { 1 };

            // Cell4 belongs to column with index 1.
            Cell cell5 = cn.GetCell(5);

            // ISet<Cell> expectedActiveCells = Stream.of(cell4).collect(Collectors.toSet());
            ISet<Cell> expectedActiveCells = new HashSet<Cell>(new Cell[] { cell5 });

            // We add distal dentrite at column1.cell4
            DistalDendrite activeSegment = cn.CreateDistalSegment(cell5);

            //
            // We add here synapses between column0.cells[0-3] and segment.
            cn.CreateSynapse(activeSegment, cn.GetCell(0), 0.20);
            cn.CreateSynapse(activeSegment, cn.GetCell(1), 0.20);
            cn.CreateSynapse(activeSegment, cn.GetCell(2), 0.20);
            cn.CreateSynapse(activeSegment, cn.GetCell(3), 0.20);
            cn.CreateSynapse(activeSegment, cn.GetCell(4), 0.20);


            ComputeCycle cc = tm.Compute(previousActiveColumns, true) as ComputeCycle;
            Assert.IsTrue(cc.PredictiveCells.SequenceEqual(expectedActiveCells));

            ComputeCycle cc2 = tm.Compute(activeColumns, true) as ComputeCycle;
            Assert.IsTrue(cc2.ActiveCells.SequenceEqual(expectedActiveCells));
        }

        [TestMethod]
        [TestCategory("Prod")]
        public void TestAdaptSegmentToCentre()
        {
            TemporalMemory tm = new TemporalMemory();
            Connections cn = new Connections();
            Parameters p = Parameters.getAllDefaultParameters();
            p.apply(cn);
            tm.Init(cn);

            DistalDendrite dd = cn.CreateDistalSegment(cn.GetCell(0));
            Synapse s1 = cn.CreateSynapse(dd, cn.GetCell(5), 0.5); // central 

            TemporalMemory.AdaptSegment(cn, dd, cn.GetCells(new int[] { 5 }), cn.HtmConfig.PermanenceIncrement, cn.HtmConfig.PermanenceDecrement);
            Assert.AreEqual(0.6, s1.Permanence, 0.1);

            // Now permanence should be at mean
            TemporalMemory.AdaptSegment(cn, dd, cn.GetCells(new int[] { 5 }), cn.HtmConfig.PermanenceIncrement, cn.HtmConfig.PermanenceDecrement);
            Assert.AreEqual(0.7, s1.Permanence, 0.1);
        }



        private Parameters GetDefaultParameters3(Parameters p, string key, Object value)
        {
            Parameters retVal = p == null ? getDefaultParameters3() : p;
            retVal.Set(key, value);

            return retVal;
        }



    }
}
