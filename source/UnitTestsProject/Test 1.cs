// Copyright (c) Damir Dobric. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NeoCortexApi;
using NeoCortexApi.Entities;
using NeoCortexApi.Types;
using NeoCortexEntities.NeuroVisualizer;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace UnitTestsProject
{
    [TestClass]
    public class TemporalPoolerTest1
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
        private Parameters getDefaultParameters1()
        {
            Parameters retVal = Parameters.getTemporalDefaultParameters();
            retVal.Set(KEY.COLUMN_DIMENSIONS, new int[] { 36 });
            retVal.Set(KEY.CELLS_PER_COLUMN, 5);
            retVal.Set(KEY.ACTIVATION_THRESHOLD, 2);
            retVal.Set(KEY.INITIAL_PERMANENCE, 0.15);
            retVal.Set(KEY.CONNECTED_PERMANENCE, 0.15);
            retVal.Set(KEY.MIN_THRESHOLD, 2);
            retVal.Set(KEY.MAX_NEW_SYNAPSE_COUNT, 2);
            retVal.Set(KEY.PERMANENCE_INCREMENT, 0.15);
            retVal.Set(KEY.PERMANENCE_DECREMENT, 0.15);
            retVal.Set(KEY.PREDICTED_SEGMENT_DECREMENT, 0.0);
            retVal.Set(KEY.RANDOM, new ThreadSafeRandom(42));
            retVal.Set(KEY.SEED, 42);

            return retVal;
        }

        private HtmConfig GetDefaultTMParameters1()
        {
            HtmConfig htmConfig = new HtmConfig(new int[] { 32 }, new int[] { 32 })
            {
                CellsPerColumn = 5,
                ActivationThreshold = 2,
                InitialPermanence = 0.15,
                ConnectedPermanence = 0.15,
                MinThreshold = 2,
                MaxNewSynapseCount = 3,
                PermanenceIncrement = 0.15,
                PermanenceDecrement = 0.15,
                PredictedSegmentDecrement = 0,
                Random = new ThreadSafeRandom(42),
                RandomGenSeed = 42
            };

            return htmConfig;
        }

        private Parameters getDefaultParameters1(Parameters p, string key, Object value)
        {
            Parameters retVal = p == null ? getDefaultParameters1() : p;
            retVal.Set(key, value);

            return retVal;
        }


      

        
        [TestMethod]
        public void TestActivateDendrites()
        {
            // Arrange

            // Create a new Connections object
            var conn = new Connections();

            // Create a new ComputeCycle object
            var cycle = new ComputeCycle();

            // Set the learn variable to true
            bool learn = true;

            // Define arrays for the external predictive inputs that will be used in the ActivateDendrites method
            int[] externalPredictiveInputsActive = new int[] { 1, 2, 3 };
            int[] externalPredictiveInputsWinners = new int[] { 4, 5, 6 };

            // Create a new instance of the TemporalMemory class using reflection
            var myClass = (TemporalMemory)Activator.CreateInstance(typeof(TemporalMemory), nonPublic: true);

            // Act

            // Get the ActivateDendrites method of the TemporalMemory class using reflection
            var method = typeof(TemporalMemory).GetMethod("ActivateDendrites", BindingFlags.NonPublic | BindingFlags.Instance);

            // Invoke the ActivateDendrites method with the specified arguments
            method.Invoke(myClass, new object[] { conn, cycle, learn, externalPredictiveInputsActive, externalPredictiveInputsWinners });

            
            // Check that the ComputeCycle object is not null
            Assert.IsNotNull(cycle);
        }



        
        [TestMethod]
        public void TestBurstUnpredictedColumnsforFiveCells2()
        {
            // Arrange

            // Create a new TemporalMemory object
            var tm = new TemporalMemory();

            // Create a new Connections object
            var cn = new Connections();

            // Get the default parameters for the Connections object
            var p = getDefaultParameters1();

            // Apply the default parameters to the Connections object
            p.apply(cn);

            // Initialize the TemporalMemory object with the Connections object
            tm.Init(cn);

            // Define the active columns and bursting cells
            var activeColumns = new int[] { 0 };
            var burstingCells = cn.GetCells(new int[] { 0, 1, 2, 3, 4 });

            // Act

            // Compute the result using the specified active columns and set the learn flag to true
            var result = tm.Compute(activeColumns, true) as ComputeCycle;

            // Assert

            // Check that the resulting active cells are equivalent to the specified bursting cells
            CollectionAssert.AreEquivalent(burstingCells, result.ActiveCells);
        }



       
        [TestMethod]
        [TestCategory("Prod")]
        public void TestSegmentCreationIfNotEnoughWinnerCells2()
        {
            // Create a new TemporalMemory object
            TemporalMemory tm = new TemporalMemory();

            // Create a new Connections object
            Connections cn = new Connections();

            // Get the default parameters for the Connections object and set the MAX_NEW_SYNAPSE_COUNT to 5
            Parameters p = getDefaultParameters1(null, KEY.MAX_NEW_SYNAPSE_COUNT, 5);

            // Apply the default parameters to the Connections object
            p.apply(cn);

            // Initialize the TemporalMemory object with the Connections object
            tm.Init(cn);

            // Define the zero columns and active columns
            int[] zeroColumns = { 0, 1, 2, 3 };
            int[] activeColumns = { 3, 4 };

            // Compute the result with the zero columns and set the learn flag to true
            tm.Compute(zeroColumns, true);

            // Compute the result with the active columns and set the learn flag to true
            tm.Compute(activeColumns, true);

            // Assert

            // Check that the number of segments in the Connections object is equal to 2
            Assert.AreEqual(2, cn.NumSegments(), 0);
        }


        
        [TestMethod]
        public void TestMatchingSegmentAddSynapsesToSubsetOfWinnerCells()
        {
            // Create a new instance of TemporalMemory and Connections, and set default parameters.
            TemporalMemory tm = new TemporalMemory();
            Connections cn = new Connections();
            Parameters p = getDefaultParameters1(null, KEY.CELLS_PER_COLUMN, 1);
            p = getDefaultParameters1(p, KEY.MIN_THRESHOLD, 1);
            p.apply(cn);
            tm.Init(cn);

            // Set up some initial state for the memory, with some active columns and previous winner cells.
            int[] previousActiveColumns = { 0, 1, 2, 3, 4 };
            IList<Cell> prevWinnerCells = cn.GetCells(new int[] { 0, 1, 2, 3, 4 });
            int[] activeColumns = { 5 };

            // Create a new distal dendrite segment and add a synapse to it.
            DistalDendrite matchingSegment = cn.CreateDistalSegment(cn.GetCell(5));
            cn.CreateSynapse(matchingSegment, cn.GetCell(0), 0.15);

            // Compute the memory for one cycle with the previous active columns, and then with the new active column.
            ComputeCycle cc = tm.Compute(previousActiveColumns, true) as ComputeCycle;
            Assert.IsTrue(cc.WinnerCells.SequenceEqual(prevWinnerCells));
            cc = tm.Compute(activeColumns, true) as ComputeCycle;

            // Get the synapses from the matching segment and check their properties.
            List<Synapse> synapses = matchingSegment.Synapses;
            Assert.AreEqual(2, synapses.Count);
            synapses.Sort();
            foreach (Synapse synapse in synapses)
            {
                if (synapse.GetPresynapticCell().Index == 0) continue;

                Assert.AreEqual(0.15, synapse.Permanence, 0.01);
                Assert.IsTrue(synapse.GetPresynapticCell().Index == 1 ||
                           synapse.GetPresynapticCell().Index == 2 ||
                           synapse.GetPresynapticCell().Index == 3 ||
                           synapse.GetPresynapticCell().Index == 4);
            }
        }



        [TestMethod]
        [TestCategory("Prod")]
        public void TestActivateCorrectlyPredictiveCells1()
        {
            // Arrange
            //The method creates a new instance of the TemporalMemory class and a Connections object, and sets some default parameters for the memory using the getDefaultParameters1() method.
            int implementation = 0;
            TemporalMemory tm = implementation == 0 ? new TemporalMemory() : new TemporalMemoryMT();
            Connections cn = new Connections();
            Parameters p = getDefaultParameters1();
            p.apply(cn);
            tm.Init(cn);

            //The method creates an input pattern by setting the previousActiveColumns and activeColumns arrays, which represent the indices of the active columns in the previous and current time steps, respectively.
            int[] previousActiveColumns = { 0 };
            int[] activeColumns = { 1 };

            //The method obtains a specific cell from the Connections object using the GetCell() method and sets up some synapses to it using the CreateSynapses() helper method.
            Cell cell5 = cn.GetCell(5);
            ISet<Cell> expectedPredictiveCells = new HashSet<Cell>(new Cell[] { cell5 });

            CreateSynapses(cn, cell5, new int[] { 0, 1, 2, 3, 4 }, 0.15);

            // Act
            //The method then calls the Compute() method of the TemporalMemory instance twice with the input pattern to activate the memory and obtain the resulting active and predictive cells.
            ComputeCycle cc1 = tm.Compute(previousActiveColumns, true) as ComputeCycle;
            ComputeCycle cc2 = tm.Compute(activeColumns, true) as ComputeCycle;

            // Assert
            //Finally, the method performs some assertions to check that the expected predictive and active cells are indeed activated by the input pattern.
            Assert.IsTrue(cc1.PredictiveCells.SequenceEqual(expectedPredictiveCells));
            Assert.IsTrue(cc2.ActiveCells.SequenceEqual(expectedPredictiveCells));
            Assert.AreEqual(expectedPredictiveCells.Count, cc1.PredictiveCells.Count);
            Assert.AreEqual(expectedPredictiveCells.Count, cc2.ActiveCells.Count);
            
        }

        // Helper method to create synapses between a distal dendrite segment and a set of cells
        private void CreateSynapses(Connections cn, Cell cell, int[] targetCells, double permanence)
        {
            DistalDendrite segment = cn.CreateDistalSegment(cell);
            foreach (int i in targetCells)
            {
                cn.CreateSynapse(segment, cn.GetCell(i), permanence);
            }
        }






        
        [TestMethod]
        public void TestBurstUnpredictedColumnsforFiveCells1()
        {
            // Get default configuration parameters for the HtmConfig class and create a new Connections object.
            HtmConfig htmConfig = GetDefaultTMParameters1();
            Connections cn = new Connections(htmConfig);

            // Create a new instance of TemporalMemory and initialize it with the Connections object.
            TemporalMemory tm = new TemporalMemory();
            tm.Init(cn);

            // Set up some initial state for the memory, with one active column and five bursting cells.
            int[] activeColumns = { 0 };
            var burstingCells = cn.GetCells(new int[] { 0, 1, 2, 3, 4 });

            // Compute the memory for one cycle with the active column.
            ComputeCycle cc = tm.Compute(activeColumns, true) as ComputeCycle;

            // Check that the active cells after the cycle are equal to the bursting cells.
            Assert.IsTrue(cc.ActiveCells.SequenceEqual(burstingCells));
        }


        
        [TestMethod]
        [TestCategory("Prod")]
        public void TestNoneActiveColumns()
        {
            // Create a new TemporalMemory and Connections object, and initialize them with default parameters.
            TemporalMemory tm = new TemporalMemory();
            Connections cn = new Connections();
            Parameters p = getDefaultParameters1();
            p.apply(cn);
            tm.Init(cn);

            // Set up some initial state for the memory, with one active segment and five synapses to other cells.
            int[] previousActiveColumns = { 0 };
            Cell cell5 = cn.GetCell(5);
            DistalDendrite activeSegment = cn.CreateDistalSegment(cell5);
            cn.CreateSynapse(activeSegment, cn.GetCell(0), 0.15);
            cn.CreateSynapse(activeSegment, cn.GetCell(1), 0.15);
            cn.CreateSynapse(activeSegment, cn.GetCell(2), 0.15);
            cn.CreateSynapse(activeSegment, cn.GetCell(3), 0.15);
            cn.CreateSynapse(activeSegment, cn.GetCell(4), 0.15);

            // Compute the memory for one cycle with the active segment, and check that there are active, winner, and predictive cells.
            ComputeCycle cc = tm.Compute(previousActiveColumns, true) as ComputeCycle;
            Assert.IsFalse(cc.ActiveCells.Count == 0);
            Assert.IsFalse(cc.WinnerCells.Count == 0);
            Assert.IsFalse(cc.PredictiveCells.Count == 0);

            // Compute the memory for one cycle with no active columns, and check that there are no active, winner, or predictive cells.
            int[] zeroColumns = new int[0];
            ComputeCycle cc2 = tm.Compute(zeroColumns, true) as ComputeCycle;
            Assert.IsTrue(cc2.ActiveCells.Count == 0);
            Assert.IsTrue(cc2.WinnerCells.Count == 0);
            Assert.IsTrue(cc2.PredictiveCells.Count == 0);
        }



        [TestMethod]
        [TestCategory("Prod")]
        public void TestPredictedActiveCellsAreCorrect()
        {
            //The test initializes a TemporalMemory object and a Connections object with default parameters and sets up some active columns and cells.
            TemporalMemory tm = new TemporalMemory();
            Connections cn = new Connections();
            Parameters p = getDefaultParameters1();
            p.apply(cn);
            tm.Init(cn);


            //The test creates two DistalDendrite objects and creates synapses between them and the previous active cells.
            int[] previousActiveColumns = { 0 };
            int[] activeColumns = { 1 };
            Cell[] previousActiveCells = { cn.GetCell(0), cn.GetCell(1), cn.GetCell(2), cn.GetCell(3), cn.GetCell(4) };
            List<Cell> expectedWinnerCells = new List<Cell>(cn.GetCells(new int[] { 6, 8 }));

            //The test then runs two ComputeCycle cycles on the temporal memory with the previous active columns and the current active columns, both with learning turned off.
            DistalDendrite activeSegment1 = cn.CreateDistalSegment(expectedWinnerCells[0]);
            cn.CreateSynapse(activeSegment1, previousActiveCells[0], 0.15);
            cn.CreateSynapse(activeSegment1, previousActiveCells[1], 0.15);
            cn.CreateSynapse(activeSegment1, previousActiveCells[2], 0.15);
            cn.CreateSynapse(activeSegment1, previousActiveCells[3], 0.15);

            DistalDendrite activeSegment2 = cn.CreateDistalSegment(expectedWinnerCells[1]);
            
            cn.CreateSynapse(activeSegment2, previousActiveCells[1], 0.15);
            cn.CreateSynapse(activeSegment2, previousActiveCells[2], 0.15);
            cn.CreateSynapse(activeSegment2, previousActiveCells[3], 0.15);

            //The test finally checks if the expected winner cells match the actual winner cells.
            ComputeCycle cc = tm.Compute(previousActiveColumns, false) as ComputeCycle; // learn=false
            cc = tm.Compute(activeColumns, false) as ComputeCycle; // learn=false
            //Assert
            Assert.IsTrue(cc.WinnerCells.SequenceEqual(new LinkedHashSet<Cell>(expectedWinnerCells)));
        }

        [TestMethod]
        [TestCategory("Prod")]
        public void TestReinforcedSelectedMatchingSegmentInBurstingColumn1()
        {
            // The code instantiates a TemporalMemory object and a Connections object and initializes them with default parameters.
            TemporalMemory tm = new TemporalMemory();
            Connections cn = new Connections();
            Parameters p = getDefaultParameters1(null, KEY.PERMANENCE_DECREMENT, 0.8);
            p.apply(cn);
            tm.Init(cn);
            // It creates arrays of previousActiveColumns, activeColumns, previousActiveCells, and burstingCells.
            int[] previousActiveColumns = { 0 };
            int[] activeColumns = { 1 };
            Cell[] previousActiveCells = { cn.GetCell(0), cn.GetCell(1), cn.GetCell(2), cn.GetCell(3) , cn.GetCell(4) };
            Cell[] burstingCells = { cn.GetCell(4), cn.GetCell(6) , cn.GetCell(8) };

            // It creates an activeSegment and several Synapses using the Connections object.
            DistalDendrite activeSegment = cn.CreateDistalSegment(burstingCells[0]);
            Synapse as1 = cn.CreateSynapse(activeSegment, previousActiveCells[0], 0.15);
            Synapse as2 = cn.CreateSynapse(activeSegment, previousActiveCells[0], 0.15);
            Synapse as3 = cn.CreateSynapse(activeSegment, previousActiveCells[0], 0.15);
            Synapse as4 = cn.CreateSynapse(activeSegment, previousActiveCells[0], 0.15);
            Synapse as5 = cn.CreateSynapse(activeSegment, previousActiveCells[0], 0.15);
            Synapse is1 = cn.CreateSynapse(activeSegment, cn.GetCell(81), 0.15);

            DistalDendrite otherMatchingSegment = cn.CreateDistalSegment(burstingCells[1]);
            cn.CreateSynapse(otherMatchingSegment, previousActiveCells[0], 0.15);
            cn.CreateSynapse(otherMatchingSegment, previousActiveCells[1], 0.15);
            cn.CreateSynapse(otherMatchingSegment, previousActiveCells[2], 0.15);
            cn.CreateSynapse(otherMatchingSegment, cn.GetCell(81), 0.15);

            // It then calls the Compute method of the TemporalMemory object twice, with learn set to true, to reinforce the activeSegment.
            tm.Compute(previousActiveColumns, true);
            tm.Compute(activeColumns, true);

            // Finally, it uses several Assert statements to check that the permanence values of the Synapses are set correctly.
            Assert.AreEqual(0.15, as1.Permanence, 0.01);
            Assert.AreEqual(0.15, as2.Permanence, 0.01);
            Assert.AreEqual(0.15, as3.Permanence, 0.01);
            Assert.AreEqual(0.15, as4.Permanence, 0.01);
            Assert.AreEqual(0.15, as5.Permanence, 0.01);
            Assert.AreEqual(0.15, is1.Permanence, 0.001);
        }

        [TestMethod]
        [TestCategory("Prod")]
        public void TestReinforcedSelectedMatchingSegmentInBurstingColumn()
        {
            TemporalMemory tm = new TemporalMemory();
            Connections cn = new Connections();
            Parameters p = getDefaultParameters1(null, KEY.PERMANENCE_DECREMENT, 0.06);
            p.apply(cn);
            tm.Init(cn);
            int[] previousActiveColumns = { 0 };
            int[] activeColumns = { 1 };
            Cell[] previousActiveCells = { cn.GetCell(0), cn.GetCell(1), cn.GetCell(2), cn.GetCell(3) , cn.GetCell(4) };
            Cell[] burstingCells = { cn.GetCell(4), cn.GetCell(6) };

            DistalDendrite activeSegment = cn.CreateDistalSegment(burstingCells[0]);
            Synapse as1 = cn.CreateSynapse(activeSegment, previousActiveCells[0], 0.3);
            Synapse as2 = cn.CreateSynapse(activeSegment, previousActiveCells[0], 0.3);
            Synapse as3 = cn.CreateSynapse(activeSegment, previousActiveCells[0], 0.3);
            Synapse as4 = cn.CreateSynapse(activeSegment, previousActiveCells[0], 0.3);
            Synapse as5 = cn.CreateSynapse(activeSegment, previousActiveCells[0], 0.3);
            Synapse is1 = cn.CreateSynapse(activeSegment, cn.GetCell(81), 0.3);

            DistalDendrite otherMatchingSegment = cn.CreateDistalSegment(burstingCells[1]);
            cn.CreateSynapse(otherMatchingSegment, previousActiveCells[0], 0.3);
            cn.CreateSynapse(otherMatchingSegment, previousActiveCells[1], 0.3);
            cn.CreateSynapse(otherMatchingSegment, previousActiveCells[2], 0.3);
            cn.CreateSynapse(otherMatchingSegment, cn.GetCell(81), 0.3);

            tm.Compute(previousActiveColumns, true);
            tm.Compute(activeColumns, true);

            Assert.AreEqual(0.3, as1.Permanence, 0.01);
            Assert.AreEqual(0.3, as2.Permanence, 0.01);
            Assert.AreEqual(0.3, as3.Permanence, 0.01);
            Assert.AreEqual(0.3, as4.Permanence, 0.01);
            Assert.AreEqual(0.3, as5.Permanence, 0.01);
            Assert.AreEqual(0.3, is1.Permanence, 0.001);
        }

        [TestMethod]
        [TestCategory("Prod")]
        public void TestNoNewSegmentIfNotEnoughWinnerCells1()
        {
            TemporalMemory tm = new TemporalMemory();
            Connections cn = new Connections();
            Parameters p = getDefaultParameters1(null, KEY.MAX_NEW_SYNAPSE_COUNT, 5);
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
            Parameters p = getDefaultParameters1(null, KEY.MAX_NEW_SYNAPSE_COUNT, 2);
            p.apply(cn);
            tm.Init(cn);

            int[] previousActiveColumns = { 0, 1, 2 ,3 , 4 };
            int[] activeColumns = { 8 };

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
                Assert.AreEqual(0.14, synapse.Permanence, 0.01);
                Assert.IsTrue(prevWinnerCells.Contains(synapse.GetPresynapticCell()));
            }
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void TestNewSegmentAddSynapsesToSubsetOfWinnerCells1()
        {
            // Arrange
            var tm = new TemporalMemory();
            var cn = new Connections();
            var p = getDefaultParameters1(null, KEY.MAX_NEW_SYNAPSE_COUNT, 2);
            p.apply(cn);
            tm.Init(cn);

            var previousActiveColumns = new[] { 0, 1, 2, 3, 4 };
            var activeColumns = new[] { 6 };

            // Act
            var cc = tm.Compute(previousActiveColumns, true) as ComputeCycle;
            var prevWinnerCells = cc.WinnerCells;

            cc = tm.Compute(activeColumns, true) as ComputeCycle;
            var winnerCells = new List<Cell>(cc.WinnerCells);

            // Assert
            Assert.AreEqual(5, prevWinnerCells.Count);
            Assert.AreEqual(1, winnerCells.Count);

            var segments = winnerCells[0].DistalDendrites;
            Assert.AreEqual(1, segments.Count);

            var synapses = segments[0].Synapses;
            Assert.AreEqual(2, synapses.Count);

            foreach (var synapse in synapses)
            {
                Assert.AreEqual(0.14, synapse.Permanence, 0.01);
                Assert.IsTrue(prevWinnerCells.Contains(synapse.GetPresynapticCell()));
            }
        }


        
        [TestMethod]
        public void TestMatchingSegmentAddSynapsesToSubsetOfWinnerCells1()
        {
            // Arrange
            var tm = new TemporalMemory();
            var cn = new Connections();
            var p = getDefaultParameters1(null, KEY.CELLS_PER_COLUMN, 1);
            p = getDefaultParameters1(p, KEY.MIN_THRESHOLD, 1);
            p.apply(cn);
            tm.Init(cn);

            var previousActiveColumns = new[] { 0, 1, 2, 3, 4 };
            var prevWinnerCells = cn.GetCells(previousActiveColumns);
            var activeColumns = new[] { 5 };

            var matchingSegment = cn.CreateDistalSegment(cn.GetCell(5));
            cn.CreateSynapse(matchingSegment, cn.GetCell(0), 0.15);

            // Act
            var cc = tm.Compute(previousActiveColumns, true) as ComputeCycle;
            cc = tm.Compute(activeColumns, true) as ComputeCycle;

            // Assert
            var synapses = matchingSegment.Synapses;
            Assert.AreEqual(2, synapses.Count);

            synapses.Sort();
            foreach (var synapse in synapses)
            {
                if (synapse.GetPresynapticCell().Index == 0) continue;

                Assert.AreEqual(0.15, synapse.Permanence, 0.01);
                Assert.IsTrue(new[] { 1, 2, 3, 4 }.Contains(synapse.GetPresynapticCell().Index));
            }
        }



        


       


    }
}
