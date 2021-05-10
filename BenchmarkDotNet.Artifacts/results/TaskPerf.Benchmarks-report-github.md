``` ini

BenchmarkDotNet=v0.12.1, OS=Windows 10.0.19042
Intel Xeon CPU E5-1620 0 3.60GHz, 1 CPU, 8 logical and 4 physical cores
.NET Core SDK=5.0.104
  [Host]     : .NET Core 5.0.5 (CoreCLR 5.0.521.16609, CoreFX 5.0.521.16609), X64 RyuJIT DEBUG
  DefaultJob : .NET Core 5.0.5 (CoreCLR 5.0.521.16609, CoreFX 5.0.521.16609), X64 RyuJIT


```
|                                 Method |             Categories |         Mean |        Error |       StdDev |       Median | Ratio | RatioSD |       Gen 0 | Gen 1 | Gen 2 |    Allocated |
|--------------------------------------- |----------------------- |-------------:|-------------:|-------------:|-------------:|------:|--------:|------------:|------:|------:|-------------:|
|              ManyWriteFile_CSharpAsync |          ManyWriteFile |  25,010.1 μs |    493.63 μs |    568.47 μs |  24,761.8 μs |  1.00 |    0.00 |    214.2857 |     - |     - |    1125280 B |
|                     ManyWriteFile_Task |          ManyWriteFile |  26,031.3 μs |    364.68 μs |    323.28 μs |  25,955.6 μs |  1.04 |    0.03 |    218.7500 |     - |     - |    1124993 B |
|      ManyWriteFile_TaskUsingCoroutines |          ManyWriteFile |  25,705.4 μs |    327.49 μs |    290.31 μs |  25,774.4 μs |  1.03 |    0.03 |    218.7500 |     - |     - |    1125002 B |
|              ManyWriteFile_TaskBuilder |          ManyWriteFile |  28,951.4 μs |    542.39 μs |    452.92 μs |  28,922.7 μs |  1.15 |    0.03 |    843.7500 |     - |     - |    4400693 B |
|                                        |                        |              |              |              |              |       |         |             |       |       |              |
|              NonAsyncBinds_CSharpAsync |          NonAsyncBinds | 142,621.8 μs |  2,843.36 μs |  4,077.87 μs | 141,452.2 μs |  1.00 |    0.00 | 151500.0000 |     - |     - |  792000000 B |
|                     NonAsyncBinds_Task |          NonAsyncBinds | 142,776.9 μs |  1,878.12 μs |  1,756.79 μs | 142,143.0 μs |  1.00 |    0.04 | 151500.0000 |     - |     - |  792000000 B |
|      NonAsyncBinds_TaskUsingCoroutines |          NonAsyncBinds | 147,714.9 μs |  2,194.87 μs |  2,053.09 μs | 147,352.0 μs |  1.03 |    0.04 | 151500.0000 |     - |     - |  792000150 B |
|              NonAsyncBinds_TaskBuilder |          NonAsyncBinds | 226,418.9 μs |    903.84 μs |    801.23 μs | 226,485.0 μs |  1.58 |    0.05 | 221666.6667 |     - |     - | 1160000000 B |
|                                        |                        |              |              |              |              |       |         |             |       |       |              |
|                 AsyncBinds_CSharpAsync |             AsyncBinds |  86,695.4 μs |  1,731.63 μs |  5,051.24 μs |  85,067.2 μs |  1.00 |    0.00 |    166.6667 |     - |     - |    1121911 B |
|                        AsyncBinds_Task |             AsyncBinds |  86,681.7 μs |  1,720.64 μs |  3,776.84 μs |  86,118.2 μs |  1.00 |    0.07 |    333.3333 |     - |     - |    2080973 B |
|         AsyncBinds_TaskUsingCoroutines |             AsyncBinds |  90,617.2 μs |  1,807.37 μs |  5,097.72 μs |  89,582.6 μs |  1.05 |    0.08 |    200.0000 |     - |     - |    2082318 B |
|                 AsyncBinds_TaskBuilder |             AsyncBinds | 109,783.8 μs |  1,860.15 μs |  2,214.38 μs | 109,184.0 μs |  1.28 |    0.05 |   3000.0000 |     - |     - |   15577037 B |
|                                        |                        |              |              |              |              |       |         |             |       |       |              |
|             SingleSyncTask_CSharpAsync |         SingleSyncTask |  87,274.0 μs |  1,130.34 μs |    943.89 μs |  87,132.8 μs |  1.00 |    0.00 |           - |     - |     - |         95 B |
|                    SingleSyncTask_Task |         SingleSyncTask |  94,221.7 μs |  1,877.36 μs |  1,756.08 μs |  94,713.9 μs |  1.08 |    0.02 |           - |     - |     - |         95 B |
|     SingleSyncTask_TaskUsingCoroutines |         SingleSyncTask |  91,791.1 μs |  1,527.98 μs |  1,275.93 μs |  91,345.6 μs |  1.05 |    0.01 |           - |     - |     - |         95 B |
|             SingleSyncTask_TaskBuilder |         SingleSyncTask | 121,353.2 μs |  1,997.82 μs |  1,668.27 μs | 121,475.6 μs |  1.39 |    0.03 |  91800.0000 |     - |     - |  480000267 B |
|                                        |                        |              |              |              |              |       |         |             |       |       |              |
|             SyncBuilderLoop_NormalCode |                   sync | 789,976.4 μs | 15,732.88 μs | 32,138.10 μs | 780,184.3 μs |  1.00 |    0.00 | 367000.0000 |     - |     - | 1921760000 B |
|           SyncBuilderLoop_WorkflowCode |                   sync | 791,390.0 μs | 12,009.21 μs | 10,645.86 μs | 789,855.7 μs |  0.98 |    0.04 | 367000.0000 |     - |     - | 1921760000 B |
|                                        |                        |              |              |              |              |       |         |             |       |       |              |
|          TinyVariableSizedList_Builder |  TinyVariableSizedList |  49,375.8 μs |    958.57 μs |    984.38 μs |  49,056.2 μs |  1.00 |    0.00 |  20363.6364 |     - |     - |  106666656 B |
|     TinyVariableSizedList_StateMachine |  TinyVariableSizedList |  10,697.0 μs |     83.32 μs |     73.86 μs |  10,667.2 μs |  0.22 |    0.00 |   2031.2500 |     - |     - |   10666656 B |
|      TinyVariableSizedList_InlinedCode |  TinyVariableSizedList |  16,414.7 μs |    171.89 μs |    134.20 μs |  16,364.7 μs |  0.33 |    0.01 |   2031.2500 |     - |     - |   10666656 B |
|                                        |                        |              |              |              |              |       |         |             |       |       |              |
|              VariableSizedList_Builder |      VariableSizedList | 311,639.3 μs |  6,095.12 μs |  6,774.71 μs | 309,479.1 μs |  1.00 |    0.00 |  63000.0000 |     - |     - |  330679664 B |
|         VariableSizedList_StateMachine |      VariableSizedList | 100,735.9 μs |  1,971.03 μs |  2,108.98 μs | 100,053.8 μs |  0.32 |    0.01 |  44833.3333 |     - |     - |  234666624 B |
|          VariableSizedList_InlinedCode |      VariableSizedList | 163,725.9 μs |  3,246.99 μs |  5,600.90 μs | 163,610.6 μs |  0.53 |    0.02 |  44750.0000 |     - |     - |  234666624 B |
|                                        |                        |              |              |              |              |       |         |             |       |       |              |
|                  FixedSizeList_Builder |         FixedSizedList |  90,219.2 μs |    831.02 μs |    736.68 μs |  90,010.9 μs |  1.00 |    0.00 |  61166.6667 |     - |     - |  320000000 B |
|             FixedSizeList_StateMachine |         FixedSizedList | 127,779.8 μs |  2,451.88 μs |  2,517.90 μs | 126,725.7 μs |  1.42 |    0.03 |  61000.0000 |     - |     - |  320000000 B |
|              FixedSizeList_InlinedCode |         FixedSizedList | 217,002.4 μs |  4,331.04 μs |  8,134.74 μs | 215,841.2 μs |  2.43 |    0.11 |  61000.0000 |     - |     - |  320000000 B |
|                                        |                        |              |              |              |              |       |         |             |       |       |              |
|         TinyVariableSizedArray_Builder | TinyVariableSizedArray |  98,035.0 μs |  1,870.32 μs |  2,496.82 μs |  98,162.1 μs |  1.00 |    0.00 |  30000.0000 |     - |     - |  157333304 B |
|    TinyVariableSizedArray_StateMachine | TinyVariableSizedArray |  23,658.0 μs |    376.29 μs |    314.22 μs |  23,672.6 μs |  0.24 |    0.01 |  10687.5000 |     - |     - |   55999968 B |
|     TinyVariableSizedArray_InlinedCode | TinyVariableSizedArray |  26,102.3 μs |    513.85 μs |    480.66 μs |  26,172.3 μs |  0.27 |    0.01 |  10687.5000 |     - |     - |   55999968 B |
|                                        |                        |              |              |              |              |       |         |             |       |       |              |
|             VariableSizedArray_Builder |     VariableSizedArray | 370,393.4 μs |  6,617.72 μs |  7,080.89 μs | 372,370.0 μs |  1.00 |    0.00 |  77000.0000 |     - |     - |  405333840 B |
|        VariableSizedArray_StateMachine |     VariableSizedArray | 134,315.4 μs |  1,455.20 μs |  1,289.99 μs | 134,298.7 μs |  0.36 |    0.01 |  59000.0000 |     - |     - |  309333606 B |
|         VariableSizedArray_InlinedCode |     VariableSizedArray | 161,472.8 μs |  3,160.17 μs |  3,639.26 μs | 160,472.9 μs |  0.44 |    0.01 |  59000.0000 |     - |     - |  309333272 B |
|                                        |                        |              |              |              |              |       |         |             |       |       |              |
|                 FixedSizeArray_Builder |        FixedSizedArray |  31,165.4 μs |    214.60 μs |    167.55 μs |  31,206.9 μs |  1.00 |    0.00 |  19875.0000 |     - |     - |  104000000 B |
|            FixedSizeArray_StateMachine |        FixedSizedArray | 165,592.2 μs |  2,608.64 μs |  2,312.49 μs | 164,845.7 μs |  5.31 |    0.08 |  82500.0000 |     - |     - |  432000334 B |
|             FixedSizeArray_InlinedCode |        FixedSizedArray | 196,220.4 μs |  2,102.96 μs |  1,864.22 μs | 196,386.1 μs |  6.30 |    0.08 |  82333.3333 |     - |     - |  432000445 B |
|                                        |                        |              |              |              |              |       |         |             |       |       |              |
|             MultiStepOption_OldBuilder |        MultiStepOption |  57,025.6 μs |    581.98 μs |    515.91 μs |  57,082.2 μs |  1.00 |    0.00 |  38666.6667 |     - |     - |  202666632 B |
|           MultiStepOption_StateMachine |        MultiStepOption |  23,880.4 μs |    384.76 μs |    341.08 μs |  23,848.8 μs |  0.42 |    0.01 |  13437.5000 |     - |     - |   70399992 B |
|         MultiStepOption_InlineIfLambda |        MultiStepOption |  17,655.3 μs |    203.43 μs |    180.34 μs |  17,599.7 μs |  0.31 |    0.00 |  13437.5000 |     - |     - |   70399968 B |
|              MultiStepOption_NoBuilder |        MultiStepOption |  17,136.8 μs |    339.77 μs |    465.08 μs |  16,905.6 μs |  0.30 |    0.01 |  13437.5000 |     - |     - |   70399968 B |
|                                        |                        |              |              |              |              |       |         |             |       |       |              |
|        MultiStepValueOption_OldBuilder |   MultiStepValueOption |  47,394.0 μs |    793.69 μs |  1,212.04 μs |  47,055.2 μs |  1.00 |    0.00 |  19090.9091 |     - |     - |  100266664 B |
|      MultiStepValueOption_StateMachine |   MultiStepValueOption |   8,403.9 μs |     40.29 μs |     35.71 μs |   8,406.1 μs |  0.18 |    0.00 |           - |     - |     - |         24 B |
|    MultiStepValueOption_InlineIfLambda |   MultiStepValueOption |   4,059.1 μs |     16.19 μs |     13.52 μs |   4,060.8 μs |  0.09 |    0.00 |           - |     - |     - |            - |
|         MultiStepValueOption_NoBuilder |   MultiStepValueOption |   4,069.0 μs |     17.04 μs |     15.11 μs |   4,065.7 μs |  0.09 |    0.00 |           - |     - |     - |            - |
|                                        |                        |              |              |              |              |       |         |             |       |       |              |
|                 TaskSeq_NestedForLoops |                taskSeq |           NA |           NA |           NA |           NA |     ? |       ? |           - |     - |     - |            - |
| CSharp_IAsyncEnumerable_NestedForLoops |                taskSeq |     584.3 μs |      3.78 μs |      3.35 μs |     584.1 μs |  1.00 |    0.00 |     24.4141 |     - |     - |     131280 B |

Benchmarks with issues:
  Benchmarks.TaskSeq_NestedForLoops: DefaultJob
