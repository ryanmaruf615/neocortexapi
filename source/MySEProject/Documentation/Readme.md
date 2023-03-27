# Project Title : ML22/23-11 Improve UnitTests for Temporal Memory Algorithm
--------------------------------------------------------------------------------------------
# Introduction:

Temporal memory algorithm is a powerful machine learning algorithm that is commonly used in a variety of applications, such as anomaly detection and prediction. However, ensuring the accuracy and reliability of the algorithm can be challenging, especially when it comes to unit testing. Unit testing is an essential part of the software development process that involves testing individual components or modules of a system to ensure they meet the expected behavior and functionality. In the case of temporal memory algorithm, unit testing can be improved by adopting effective testing strategies and techniques that address the specific challenges of testing this complex algorithm. This can include approaches such as input perturbation, boundary testing, and stress testing, among others, that can help identify and address potential issues and improve the overall quality and reliability of the algorithm.

# Getting Started : 

For development and testing reasons, follow these procedures to get a copy of the project up and running on your own system. Look at the notes on
how to deploy the project and experiment it out on a live system. Here is the necessary links:

- Project Solution File [NeoCortexApi]([[https://github.com/UniversityOfAppliedSciencesFrankfurt/se-cloud-2022-2023/blob/Team_UnitTestBD/Source/MyProject/UnitTestProject/NeoCortexApi.All.sln)](https://github.com/ryanmaruf615/neocortexapi/blob/dev/source/NeoCortexApi.sln)](https://github.com/ryanmaruf615/neocortexapi/blob/dev/source/NeoCortexApi.sln)

- Project Documentation [Documentation](https://github.com/ryanmaruf615/neocortexapi/tree/dev/source/MySEProject/Documentation)

- Final Project [UnitTestProject](https://github.com/ryanmaruf615/neocortexapi/blob/dev/source/NeoCortexApi/TemporalMemory.cs)

- Spatial Pooler Test Class [SpatialPoolerTests.cs](https://github.com/ryanmaruf615/neocortexapi/blob/dev/source/UnitTestsProject/TemporalMemoryTests%20-1.cs
- https://github.com/ryanmaruf615/neocortexapi/blob/dev/source/UnitTestsProject/TemporalMemoryTests_2.cs
- https://github.com/ryanmaruf615/neocortexapi/blob/dev/source/UnitTestsProject/TemporalMemoryTests%20-3.cs
- )

**Project Description**
========================

## 1.Temporal memory Algorithm in HTM:
Temporal Memory Algorithm (TMA) is a core component of the Hierarchical Temporal Memory (HTM) framework, which is a machine learning algorithm based on the principles of the human brain's neocortex. The neocortex is responsible for processing sensory information and making predictions based on temporal patterns.

TMA in HTM is designed to learn and recognize temporal patterns in data streams, such as time-series data or sequences of events. It does this by creating a sparse distributed representation of the input data and continuously adapting its connections based on the temporal patterns it observes. The algorithm uses a set of heuristics to determine which connections to strengthen or weaken, based on the frequency and recency of the input patterns.

TMA is a powerful algorithm that can learn and recognize complex temporal patterns, even in noisy or incomplete data streams. It is a key component of the HTM framework, which has been used in a variety of applications, such as anomaly detection, prediction, and classification, among others.

2.Objective:
------------

Improved the Unit test methods for the Temporal memory algorithm we have added few new test methods and improved some exsisting.

For this project the following objectives are accomplished:

-       To Analyze existing code for Learning and understing Temporal memory algorithm in HTM.
-       write new test methods and run them successfully.
-       improve the existing Unit test methods in the temporal memory algorithm.
-       rewrite test methods for same methods from Temporalmemory.cs .

**Implementation**
==================

1.Temporal memory Algorithm Initialization:
--------------------------------
The initialization of the Temporal Memory Algorithm (TMA) in Hierarchical Temporal Memory (HTM) involves setting up the necessary data structures and parameters required for the algorithm to learn and recognize temporal patterns in data streams.During initialization, the TMA creates a set of initial synapses that connect input bits to the neurons in the temporal pool. These synapses are randomly distributed, but the number of synapses per input bit is determined by a connectivity parameter that is set during initialization. This parameter controls the sparsity of the TMA's representation of the input data and is typically set to a low value to encourage sparsity.The TMA also sets up a number of parameters that control its behavior, such as the learning rate, which determines the rate at which the TMA adapts its synapses based on the input data. Other parameters include the threshold for activating neurons and the number of active neurons required to recognize a pattern.


2.Test case for Temporal Memory Algorithm :
--------------------------------------------------------------
#### 
we have used  ActivateDendrites method and  We have written a test method.The purpose of this unit test is to ensure that the private method "ActivateDendrites" performs as expected and produces the correct output for the given input parameters. By passing various inputs to the method, this test method helps ensure that the method is robust and handles different scenarios correctly. The test also verifies that the private method can be accessed using reflection, which is useful for debugging and testing private methods that are not directly exposed to the external code.

        [TestMethod]

        public void TestActivateDendrites()

        {

            // Arrange

            var conn = new Connections();

            var cycle = new ComputeCycle();

            bool learn = true;

            int[] externalPredictiveInputsActive = new int[] { 1, 2, 3 };

            int[] externalPredictiveInputsWinners = new int[] { 4, 5, 6 };

            var myClass = (TemporalMemory)Activator.CreateInstance(typeof(TemporalMemory), nonPublic: true);

            // Act
            var method = typeof(TemporalMemory).GetMethod("ActivateDendrites", BindingFlags.NonPublic | BindingFlags.Instance);
            method.Invoke(myClass, new object[] { conn, cycle, learn, externalPredictiveInputsActive, externalPredictiveInputsWinners });

            //Assert
            Assert.IsNotNull(cycle);
        }

#### 
we have worked on  ComputeCycle Compute method and create a test method .The purpose of this particular test is to verify that the HTM algorithm can identify and activate the set of "bursting cells" when given a particular input. The test creates an instance of the TemporalMemory and Connections classes, and initializes them with default parameters.If the test passes, it means that the Temporal Memory algorithm correctly predicts which cells should burst based on the input. If it fails, it indicates a problem with the implementation of the algorithm.

        [TestMethod]

        public void TestBurstUnpredictedColumnsforFiveCells2()

        {

            // Arrange

            var tm = new TemporalMemory();

            var cn = new Connections();

            var p = getDefaultParameters1();

            p.apply(cn);

            tm.Init(cn);

            var activeColumns = new int[] { 0 };

            var burstingCells = cn.GetCells(new int[] { 0, 1, 2, 3, 4 });

            // Act

            var result = tm.Compute(activeColumns, true) as ComputeCycle;

            // Assert

            CollectionAssert.AreEquivalent(burstingCells, result.ActiveCells);
        }
####
we have used PunishPredictedColumn method and write a test method .This test verifies that when the input to the TemporalMemory class contains only a few active columns, which do not meet the threshold to create a new segment, the expected number of segments in the Connections object is correctly  updated. Here, we pass an array of three zero columns and two active columns to the Compute method of the TemporalMemory object. We then verify that the expected number of segments is one, indicating that a new segment was created when the active columns were processed

    [TestMethod]

    [TestCategory("Prod")]

    public void TestSegmentCreationIfNotEnoughWinnerCells()

    {

    TemporalMemory tm = new TemporalMemory();
    
    Connections cn = new Connections();
    
    Parameters p = getDefaultParameters1(null, KEY.MAX_NEW_SYNAPSE_COUNT, 5);
    
    p.apply(cn);
    
    tm.Init(cn);

    int[] zeroColumns = { 0, 1, 2 };
    
    int[] activeColumns = { 3, 4 };

    tm.Compute(zeroColumns, true);
    
    tm.Compute(activeColumns, true);

    Assert.AreEqual(2, cn.NumSegments(), 0);
}
#### 
Matching Segmen Add Synapses To Subset Of Winner CellsThis test method appears to be testing that when a new active column is processed by a TemporalMemory, and its distal dendrite segment matches an existing one, synapses are created between the new column's cells and a subset of the cells in the existing segment.The purpose of this unit test method is to verify that when a matching segment is created for a new active column in the Temporal Memory, the correct synapses are added to the segment connecting to a subset of winner cells from the previous active columns. Specifically, it checks that the synapses have the correct permanence value and are connected to the correct presynaptic cells. The test ensures that the implementation of the matching segment creation and synapse addition logic is correct.

    [TestMethod]

    public void TestMatchingSegmentAddSynapsesToSubsetOfWinnerCells()

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
    
    Assert.AreEqual(2, synapses.Count);//
    
    synapses.Sort();
    
    foreach (var synapse in synapses)
    {
        if (synapse.GetPresynapticCell().Index == 0) continue;

        Assert.AreEqual(0.15, synapse.Permanence, 0.01);
        
        Assert.IsTrue(new[] { 1, 2, 3, 4 }.Contains(synapse.GetPresynapticCell().Index));
    }
}

#### 
This unit test verifies the functionality of the Temporal Memory object in predicting the next set of active cells based on the previous set of active cells and the current input. The test creates a Temporal Memory object with a default or multi-threaded implementation, initializes it with a Connections object, and defines the previous and current sets of active columns. It then creates synapses between the current active column and the previous active columns and defines the expected predictive cells. Finally, the test computes the next cycle with the previous and current active columns as input and verifies that the predictive cells are as expected. The test also verifies that the active cells are as expected and that the count of predictive cells and active cells is the same as the expected count

    [TestMethod]

    [TestCategory("Prod")]

      public void TestActivateCorrectlyPredictiveCells()
      {
      // Arrange
    
      int implementation = 0; // 0 = default implementation, 1 = multi-threaded implementation
    
      TemporalMemory tm = implementation == 0 ? new TemporalMemory() : new TemporalMemoryMT();
    
      Connections cn = new Connections();
    
      Parameters p = getDefaultParameters1();
    
      p.apply(cn);
    
       tm.Init(cn);

      int[] previousActiveColumns = { 0 };
    
     int[] activeColumns = { 1 };
    
      Cell cell5 = cn.GetCell(5);
    
      ISet<Cell> expectedPredictiveCells = new HashSet<Cell>(new Cell[] { cell5 });

      CreateSynapses(cn, cell5, new int[] { 0, 1, 2, 3, 4 }, 0.15);

      // Act
    
      ComputeCycle cc1 = tm.Compute(previousActiveColumns, true) as ComputeCycle;
    
      ComputeCycle cc2 = tm.Compute(activeColumns, true) as ComputeCycle;

      // Assert
    
      Assert.IsTrue(cc1.PredictiveCells.SequenceEqual(expectedPredictiveCells));
    
      Assert.IsTrue(cc2.ActiveCells.SequenceEqual(expectedPredictiveCells));
    
      Assert.AreEqual(expectedPredictiveCells.Count, cc1.PredictiveCells.Count);
    
     Assert.AreEqual(expectedPredictiveCells.Count, cc2.ActiveCells.Count);
    
      // Add more assertions as needed
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

####
Here we used DistalDendrite GetSegmentWithHighestPotential method and written a test case where the purpose of this unit test is to verify that a weak synapse is destroyed when an active reinforcement occurs in a temporal memory model. The test initializes a temporal memory object and sets up connections and parameters, as well as defining previous active columns and cells, active columns, and an expected active cell. The test creates a distal dendrite segment and several synapses, including a weak synapse. The temporal memory object is then used to compute the previous and active columns, and the test asserts that the weak synapse is not present in the segment after the active reinforcement has occurred. The unit test ensures that the implementation correctly handles the destruction of weak synapses during active reinforcement.

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

####
This ComputeCycle Compute method of the TM implementation returns a set of active cells that matches the expected set of bursting cells, using the Assert.IsTrue() method. If the test passes, it indicates that the implementation correctly identified the predicted cells in response to the given input pattern.The test aims to verify whether the implementation correctly identifies and activates the predicted set of cells in response to the input. In this case, the input pattern is a single active column (column 0), and the predicted set of cells that should become active in response to this input are specified as cells 0 to 5.

        [TestMethod]
        public void TestBurstUnpredictedColumnsforSixCells()
        {
           
            var tm = new TemporalMemory();
            var cn = new Connections();
            var p = getDefaultParameters2();
            p.apply(cn);
            tm.Init(cn);
            var activeColumns = new[] { 0 };
            var burstingCells = cn.GetCells(new[] { 0, 1, 2, 3, 4, 5 });
           var result = tm.Compute(activeColumns, true);
            var cc = (ComputeCycle)result;
            Assert.IsTrue(cc.ActiveCells.SequenceEqual(burstingCells));
        }	
####
This unit test method tests the TestActivateCorrectlyPredictiveCells method of a Temporal Memory object. It uses data rows to test the method with different implementations of the Temporal Memory object.It creates a Connections object and sets default parameters, then initializes the Temporal Memorywith the Connections. It defines previous and active columns for the current and previous time steps, respectively. It gets the cell object for a specific cell in a specific column, and defines the expected set of active cells after prediction. It creates a new distal segment at the specified cell, and connects the segment to active synapses from specific cells in a different column. It then computes the prediction at the previous time step and verifies the expected result,and computes the active cells at the current time step and verifies the expected result.
        [TestMethod]
        [TestCategory("Prod")]
        [DataRow(0)]
        [DataRow(1)]
        public void TestActivateCorrectlyPredictiveCells(int tmImplementation)
        {
            TemporalMemory tm = tmImplementation == 0 ? new TemporalMemory() : new             TemporalMemoryMT();
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

####
This unit test ColumnData method tests the number of columns that are created when configuring a new Temporal Memory instance with a specific set of parameters. It sets the column dimensions to 62x62 and the number of cells per column to 30, applies these parameters to a Connections object, and initializes the Temporal Memory instance. It then retrieves the actual numberof columns from the Connections object and compares it to the expected number of columns,which is the product of the column dimensions. The unit test passes if these two values are equal.
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

####
This code tests the behavior of a Temporal Memory model using "ComputeCycle" method, presented with an input of no active columns. The TemporalMemory class and Connections class are initialized with default parameters. A distal segment is created for one of the cells in the Connections object, and synapses are created between this segment and a set of other cells.Then, the Compute method of the TemporalMemory object is called twice - once with an input of a single active column (new[] { 0 }) and once with an empty input (new int[0]). The output of these computations is stored in cc1 and cc2, respectively.Finally, the test asserts that cc1 contains non-zero counts of active cells, winner cells, and predictive cells, while cc2 contains zero counts of these cell types. This ensures that the Temporal Memory object behaves as expected when presented with no active columns.

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

####
The purpose of this code is to test the "AdaptSegment" method of the "TemporalMemory" class. The method adjusts the permanence values of synapses in a distal dendrite segment towards a target activation center using increment and decrement parameters. The code creates a new temporal memory instance, initializes its connections and sets its parameters to default values. It then creates a new distal dendrite segment, adds a synapse to it with an initial permanence of 0.8, and adjusts its permanence value twice towards the center, which is represented by cell 6. The code then checks that the final permanence value of the synapse is within a range of 0.1 of the expected value.

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


####
The purpose of this code is to test that the "ComputeCycle" method of the TemporalMemory class correctly calculates the active cells and that the ActiveCells array returned by the method does not contain any cells that are in a given array of burstingCells. The code first creates an instance of Connections and TemporalMemory classes, and then creates an array of active columns and an array of bursting cells. It then calls the Compute method of the TemporalMemory instance with the active columns and sets the learn parameter to true. Finally, it checks that the ActiveCells array returned by the Compute method does not contain any cells that are in the burstingCells array.

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







####
This is a unit test for the "DestroySegmentsWithTooFewSynapsesToBeMatching" method in the "TemporalMemory" class. The purpose of the method is to destroy any segments in the given cell that have too few synapses to be considered a matching segment. In this test, a temporal memory is initialized with a set of parameters and a set of connections. Then, a previous set of active columns and cells, and a current set of active columns are defined. A matching segment is created with a set of synapses and the temporal memory is computed with the previous and current active columns. Finally, the test verifies that the expected active cell has no segments left in the connections object, as the segment it had previously has too few synapses to be considered a matching segment. The purpose of the test is to verify that the DestroySegmentsWithTooFewSynapsesToBeMatching method works as intended.

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
####
This code tests that a new segment added to a winner cell in a Temporal Memory has synapses added to all the previous winner cells from the previous time step. It initializes a Temporal Memory and creates a Connections object. It then computes the previous active columns and the current active columns in the Temporal Memory. It asserts that the number of winner cells for the current active columns is 1, and that a new segment has been added to the winner cell. It then verifies that synapses have been added to all the previous winner cells from the previous time step.

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








**Similar Studies/Research used as References**
===============================================

[1]. NeoCortexAPI, C# Implementation of HTM : https://github.com/ddobric/neocortexapi

[2]. NeoCortexAPI, SpatialPooler and TemporalMemory documents : https://github.com/ddobric/neocortexapi/blob/master/source/Documentation/SpatialPooler.md#spatial-pooler

[3].Numenta. (n.d.). Temporal Memory Algorithm. Retrieved March 25, 2023, from https://numenta.com/resources/temporal-memory-algorithm/

