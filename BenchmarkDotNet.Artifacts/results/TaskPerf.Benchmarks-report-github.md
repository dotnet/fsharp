``` ini

BenchmarkDotNet=v0.11.5, OS=Windows 10.0.19042
Intel Xeon CPU E5-1620 0 3.60GHz, 1 CPU, 8 logical and 4 physical cores
.NET Core SDK=5.0.104
  [Host]     : .NET Core 2.1.27 (CoreCLR 4.6.29916.01, CoreFX 4.6.29916.03), 64bit RyuJIT DEBUG
  DefaultJob : .NET Core 2.1.27 (CoreCLR 4.6.29916.01, CoreFX 4.6.29916.03), 64bit RyuJIT


```
|                                 Method |             Categories |            Mean |           Error |          StdDev |          Median |  Ratio | RatioSD |       Gen 0 |     Gen 1 | Gen 2 |    Allocated |
|--------------------------------------- |----------------------- |----------------:|----------------:|----------------:|----------------:|-------:|--------:|------------:|----------:|------:|-------------:|
|              ManyWriteFile_CSharpAsync |          ManyWriteFile |     27,720.8 us |       549.95 us |     1,533.03 us |     27,307.7 us |   1.00 |    0.00 |    375.0000 |         - |     - |       1104 B |
|                     ManyWriteFile_Task |          ManyWriteFile |     27,833.1 us |       554.31 us |       846.50 us |     27,631.0 us |   1.01 |    0.05 |    375.0000 |         - |     - |       1152 B |
|              ManyWriteFile_TaskBuilder |          ManyWriteFile |     42,984.2 us |       842.84 us |     1,336.83 us |     42,636.6 us |   1.55 |    0.09 |   1000.0000 |         - |     - |       1808 B |
|              ManyWriteFile_FSharpAsync |          ManyWriteFile |     44,383.7 us |       689.95 us |       576.14 us |     44,628.2 us |   1.65 |    0.05 |   1454.5455 |         - |     - |        936 B |
|                                        |                        |                 |                 |                 |                 |        |         |             |           |       |              |
|              NonAsyncBinds_CSharpAsync |          NonAsyncBinds |    162,204.4 us |     1,665.45 us |     1,557.87 us |    161,822.3 us |   1.00 |    0.00 | 151000.0000 |         - |     - |  792000000 B |
|                     NonAsyncBinds_Task |          NonAsyncBinds |    173,433.4 us |       662.83 us |       553.49 us |    173,342.5 us |   1.07 |    0.01 | 151000.0000 |         - |     - |  792000000 B |
|              NonAsyncBinds_TaskBuilder |          NonAsyncBinds |    248,864.7 us |     1,025.26 us |       856.14 us |    248,668.0 us |   1.53 |    0.02 | 221000.0000 |         - |     - | 1160000000 B |
|              NonAsyncBinds_FSharpAsync |          NonAsyncBinds | 12,894,093.8 us |   265,229.61 us |   777,872.54 us | 12,598,945.9 us |  81.07 |    4.59 | 532000.0000 | 2000.0000 |     - |  775999136 B |
|                                        |                        |                 |                 |                 |                 |        |         |             |           |       |              |
|                 AsyncBinds_CSharpAsync |             AsyncBinds |    106,408.1 us |     2,000.06 us |     1,964.33 us |    105,633.7 us |   1.00 |    0.00 |    800.0000 |         - |     - |    1528294 B |
|                        AsyncBinds_Task |             AsyncBinds |    119,073.2 us |     7,718.14 us |     7,925.96 us |    115,067.4 us |   1.12 |    0.08 |   1000.0000 |         - |     - |    2528674 B |
|                 AsyncBinds_TaskBuilder |             AsyncBinds |    230,811.0 us |     4,185.62 us |     3,915.23 us |    230,812.4 us |   2.17 |    0.06 |   3000.0000 |         - |     - |    4986509 B |
|                                        |                        |                 |                 |                 |                 |        |         |             |           |       |              |
|             SingleSyncTask_CSharpAsync |         SingleSyncTask |     77,474.7 us |       264.50 us |       247.41 us |     77,457.0 us |   1.00 |    0.00 |           - |         - |     - |            - |
|                    SingleSyncTask_Task |         SingleSyncTask |    117,582.2 us |       574.64 us |       537.51 us |    117,470.9 us |   1.52 |    0.01 |           - |         - |     - |            - |
|             SingleSyncTask_TaskBuilder |         SingleSyncTask |    125,740.5 us |       889.11 us |       694.16 us |    125,876.2 us |   1.62 |    0.01 |  91500.0000 |         - |     - |  480000000 B |
|             SingleSyncTask_FSharpAsync |         SingleSyncTask | 39,369,928.6 us | 1,189,170.61 us | 3,468,866.48 us | 38,868,183.3 us | 509.95 |   33.20 | 926000.0000 | 3000.0000 |     - | 3879989488 B |
|                                        |                        |                 |                 |                 |                 |        |         |             |           |       |              |
|             SyncBuilderLoop_NormalCode |                   sync |    822,671.6 us |     2,060.14 us |     1,927.05 us |    822,652.5 us |   1.00 |    0.00 | 366000.0000 |         - |     - | 1921760000 B |
|           SyncBuilderLoop_WorkflowCode |                   sync |  1,015,999.3 us |    52,669.34 us |   155,296.75 us |    944,769.5 us |   1.42 |    0.23 | 366000.0000 |         - |     - | 1922240000 B |
|                                        |                        |                 |                 |                 |                 |        |         |             |           |       |              |
|     TinyVariableSizedList_StateMachine |  TinyVariableSizedList |     55,059.8 us |     1,229.26 us |     3,406.27 us |     53,935.8 us |   1.00 |    0.00 |  20333.3333 |         - |     - |  106666656 B |
|          TinyVariableSizedList_Builder |  TinyVariableSizedList |     20,289.4 us |       380.85 us |       356.25 us |     20,290.3 us |   0.34 |    0.02 |   2031.2500 |         - |     - |   10666656 B |
|                                        |                        |                 |                 |                 |                 |        |         |             |           |       |              |
|         VariableSizedList_StateMachine |      VariableSizedList |    318,779.3 us |     3,166.39 us |     2,644.08 us |    318,875.7 us |   1.00 |    0.00 |  63000.0000 |         - |     - |  330666624 B |
|              VariableSizedList_Builder |      VariableSizedList |    164,465.6 us |     2,171.21 us |     2,030.95 us |    163,474.6 us |   0.52 |    0.01 |  44750.0000 |         - |     - |  234666624 B |
|                                        |                        |                 |                 |                 |                 |        |         |             |           |       |              |
|             FixedSizeList_StateMachine |         FixedSizedList |    103,506.5 us |     1,997.36 us |     1,961.68 us |    103,209.3 us |   1.00 |    0.00 |  61000.0000 |         - |     - |  320000000 B |
|                  FixedSizeList_Builder |         FixedSizedList |    221,445.9 us |     3,756.06 us |     3,513.42 us |    220,492.4 us |   2.14 |    0.06 |  61000.0000 |         - |     - |  320000000 B |
|                                        |                        |                 |                 |                 |                 |        |         |             |           |       |              |
|    TinyVariableSizedArray_StateMachine | TinyVariableSizedArray |    107,476.4 us |     2,089.38 us |     2,235.61 us |    107,648.8 us |   1.00 |    0.00 |  31400.0000 |         - |     - |  165333304 B |
|         TinyVariableSizedArray_Builder | TinyVariableSizedArray |     45,025.8 us |     3,065.75 us |     9,039.43 us |     41,278.8 us |   0.33 |    0.01 |  11142.8571 |         - |     - |   58666632 B |
|                                        |                        |                 |                 |                 |                 |        |         |             |           |       |              |
|        VariableSizedArray_StateMachine |     VariableSizedArray |    430,924.2 us |     8,682.09 us |    25,049.82 us |    420,930.3 us |   1.00 |    0.00 |  78000.0000 |         - |     - |  413333272 B |
|             VariableSizedArray_Builder |     VariableSizedArray |    196,516.5 us |     3,734.74 us |     4,151.16 us |    196,858.4 us |   0.42 |    0.02 |  60333.3333 |         - |     - |  317333272 B |
|                                        |                        |                 |                 |                 |                 |        |         |             |           |       |              |
|            FixedSizeArray_StateMachine |        FixedSizedArray |     44,270.4 us |       719.12 us |       672.67 us |     44,195.5 us |   1.00 |    0.00 |  19833.3333 |         - |     - |  104000000 B |
|                 FixedSizeArray_Builder |        FixedSizedArray |    241,253.7 us |     4,215.19 us |     3,736.65 us |    240,190.6 us |   5.45 |    0.12 |  83666.6667 |         - |     - |  440000000 B |
|                                        |                        |                 |                 |                 |                 |        |         |             |           |       |              |
|           MultiStepOption_StateMachine |        MultiStepOption |     26,483.4 us |       286.85 us |       239.54 us |     26,508.8 us |   1.00 |    0.00 |  10968.7500 |         - |     - |   57600000 B |
|             MultiStepOption_OldBuilder |        MultiStepOption |     64,191.9 us |     1,040.66 us |       973.44 us |     64,221.4 us |   2.42 |    0.05 |  38625.0000 |         - |     - |  202666632 B |
|              MultiStepOption_NoBuilder |        MultiStepOption |     18,692.4 us |       332.48 us |       311.00 us |     18,656.1 us |   0.71 |    0.01 |  13406.2500 |         - |     - |   70399968 B |
|                                        |                        |                 |                 |                 |                 |        |         |             |           |       |              |
|      MultiStepValueOption_StateMachine |   MultiStepValueOption |     33,815.1 us |       378.74 us |       354.27 us |     33,742.6 us |   1.00 |    0.00 |           - |         - |     - |         24 B |
|        MultiStepValueOption_OldBuilder |   MultiStepValueOption |     66,349.2 us |       804.55 us |       713.21 us |     66,472.8 us |   1.96 |    0.03 |  19000.0000 |         - |     - |  100266664 B |
|         MultiStepValueOption_NoBuilder |   MultiStepValueOption |     27,996.3 us |       137.41 us |       121.81 us |     27,971.6 us |   0.83 |    0.01 |           - |         - |     - |            - |
|                                        |                        |                 |                 |                 |                 |        |         |             |           |       |              |
|                 TaskSeq_NestedForLoops |                taskSeq |      1,084.8 us |        21.19 us |        20.81 us |      1,079.1 us |   1.58 |    0.06 |    134.7656 |         - |     - |     711824 B |
|                AsyncSeq_NestedForLoops |                taskSeq |     16,239.9 us |       324.66 us |       398.71 us |     16,144.4 us |  23.66 |    0.97 |   4468.7500 |  125.0000 |     - |        880 B |
| CSharp_IAsyncEnumerable_NestedForLoops |                taskSeq |        693.7 us |        13.86 us |        28.93 us |        689.2 us |   1.00 |    0.00 |     26.3672 |         - |     - |     142208 B |
