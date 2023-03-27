# Project Title : ML22/23-11 Improve UnitTests for Temporal Memory Algorithm
--------------------------------------------------------------------------------------------
# Introduction:

Temporal memory algorithm is a powerful machine learning algorithm that is commonly used in a variety of applications, such as anomaly detection and prediction. However, ensuring the accuracy and reliability of the algorithm can be challenging, especially when it comes to unit testing. Unit testing is an essential part of the software development process that involves testing individual components or modules of a system to ensure they meet the expected behavior and functionality. In the case of temporal memory algorithm, unit testing can be improved by adopting effective testing strategies and techniques that address the specific challenges of testing this complex algorithm. This can include approaches such as input perturbation, boundary testing, and stress testing, among others, that can help identify and address potential issues and improve the overall quality and reliability of the algorithm.

# Getting Started : 

For development and testing reasons, follow these procedures to get a copy of the project up and running on your own system. Look at the notes on
how to deploy the project and experiment it out on a live system. Here is the necessary links:

- Project Solution File [NeoCortexApi]([[https://github.com/UniversityOfAppliedSciencesFrankfurt/se-cloud-2022-2023/blob/Team_UnitTestBD/Source/MyProject/UnitTestProject/NeoCortexApi.All.sln)](https://github.com/ryanmaruf615/neocortexapi/blob/dev/source/NeoCortexApi.sln)](https://github.com/ryanmaruf615/neocortexapi/blob/dev/source/NeoCortexApi.sln)

- Project Documentation [Documentation](https://github.com/ryanmaruf615/neocortexapi/tree/dev/source/MySEProject/Documentation)
- 
- Project Commits  [Commits](https://github.com/ryanmaruf615/neocortexapi/commits/dev)

- Final Project [UnitTestProject](https://github.com/ryanmaruf615/neocortexapi/blob/dev/source/NeoCortexApi/TemporalMemory.cs)

- Temporal Memory Test Class [TemporalMemory.cs](https://github.com/ryanmaruf615/neocortexapi/blob/dev/source/UnitTestsProject/TemporalMemoryTests%20-1.cs
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


2.Test case for Temporal Memory Algorithm by Group members :
--------------------------------------------------------------
#### Md Maruf Hossain contribution:

        -TestActivateDendrites  
        
        -TestBurstUnpredictedColumnsforFiveCells2
        
        -TestSegmentCreationIfNotEnoughWinnerCells2
        
        -TestMatchingSegmentAddSynapsesToSubsetOfWinnerCells
        
        -TestActivateCorrectlyPredictiveCells
        
** To View the Full Code ** 
[Click Here](https://github.com/ryanmaruf615/neocortexapi/blob/dev/source/UnitTestsProject/TemporalMemoryTests%20-1.cs)    


#### Mousumi Parvin tonni contribution:

        -TestDestroyWeakSynapseOnActiveReinforce  
        
        -TestBurstUnpredictedColumnsforSixCells
        
        -TestActivateCorrectlyPredictiveCells
        
        -TestNumberOfColumns   
        
** To View the Full Code ** 
[Click Here](https://github.com/ryanmaruf615/neocortexapi/blob/dev/source/UnitTestsProject/TemporalMemoryTests_2.cs)    

#### Aktari Sadia Sabah contribution:

        -TestNoneActiveColumns  
        
        -TestAdaptSegmentToCentre
        
        -TestArrayNotContainingCells
        
        -TestDestroySegmentsWithTooFewSynapsesToBeMatching
        
        -TestNewSegmentAddSynapsesToAllWinnerCells
        
** To View the Full Code ** 
[Click Here](https://github.com/ryanmaruf615/neocortexapi/blob/dev/source/UnitTestsProject/TemporalMemoryTests%20-3.cs)    




**Similar Studies/Research used as References**
===============================================

[1]. NeoCortexAPI, C# Implementation of HTM : https://github.com/ddobric/neocortexapi

[2]. NeoCortexAPI, SpatialPooler and TemporalMemory documents : https://github.com/ddobric/neocortexapi/blob/master/source/Documentation/SpatialPooler.md#spatial-pooler

[3].Numenta. (n.d.). Temporal Memory Algorithm. Retrieved March 25, 2023, from https://numenta.com/resources/temporal-memory-algorithm/

