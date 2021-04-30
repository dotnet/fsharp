``` ini

BenchmarkDotNet=v0.12.1, OS=Windows 10.0.19042
Intel Xeon CPU E5-1620 0 3.60GHz, 1 CPU, 8 logical and 4 physical cores
.NET Core SDK=5.0.104
  [Host]     : .NET Core 3.1.14 (CoreCLR 4.700.21.16201, CoreFX 4.700.21.16208), X64 RyuJIT DEBUG
  DefaultJob : .NET Core 3.1.14 (CoreCLR 4.700.21.16201, CoreFX 4.700.21.16208), X64 RyuJIT


```
|                                 Method |             Categories |         Mean |        Error |       StdDev |       Median | Ratio | RatioSD |       Gen 0 |    Gen 1 | Gen 2 |    Allocated |
|--------------------------------------- |----------------------- |-------------:|-------------:|-------------:|-------------:|------:|--------:|------------:|---------:|------:|-------------:|
|              ManyWriteFile_CSharpAsync |          ManyWriteFile |  27,564.6 μs |    495.44 μs |    439.19 μs |  27,448.9 μs |  1.00 |    0.00 |    375.0000 |        - |     - |    2005320 B |
|                     ManyWriteFile_Task |          ManyWriteFile |  25,616.9 μs |    488.74 μs |    600.22 μs |  25,511.1 μs |  0.93 |    0.02 |    375.0000 |        - |     - |    2004992 B |
|              ManyWriteFile_TaskBuilder |          ManyWriteFile |  37,177.4 μs |    579.24 μs |    513.48 μs |  37,079.8 μs |  1.35 |    0.03 |   1000.0000 |        - |     - |    5290459 B |
|                                        |                        |              |              |              |              |       |         |             |          |       |              |
|              NonAsyncBinds_CSharpAsync |          NonAsyncBinds | 165,954.1 μs |  2,450.59 μs |  2,046.36 μs | 165,915.2 μs |  1.00 |    0.00 | 151333.3333 |        - |     - |  792000000 B |
|                     NonAsyncBinds_Task |          NonAsyncBinds | 185,824.3 μs |  3,583.97 μs |  5,024.23 μs | 183,469.0 μs |  1.13 |    0.04 | 151333.3333 |        - |     - |  792000000 B |
|              NonAsyncBinds_TaskBuilder |          NonAsyncBinds | 260,215.5 μs |  4,994.49 μs |  5,128.97 μs | 260,403.2 μs |  1.57 |    0.04 | 221500.0000 |        - |     - | 1160000668 B |
|                                        |                        |              |              |              |              |       |         |             |          |       |              |
|                 AsyncBinds_CSharpAsync |             AsyncBinds |  89,273.9 μs |  1,784.75 μs |  4,575.00 μs |  88,201.0 μs |  1.00 |    0.00 |    166.6667 |        - |     - |    1121275 B |
|                        AsyncBinds_Task |             AsyncBinds |  91,374.6 μs |  1,823.00 μs |  2,099.37 μs |  91,201.1 μs |  1.01 |    0.06 |    333.3333 |        - |     - |    2082468 B |
|                 AsyncBinds_TaskBuilder |             AsyncBinds | 159,356.4 μs |  4,116.82 μs | 12,008.94 μs | 158,436.6 μs |  1.80 |    0.17 |   3000.0000 |        - |     - |   15861808 B |
|                                        |                        |              |              |              |              |       |         |             |          |       |              |
|             SingleSyncTask_CSharpAsync |         SingleSyncTask |  87,033.7 μs |    725.82 μs |    643.42 μs |  86,892.5 μs |  1.00 |    0.00 |           - |        - |     - |        223 B |
|                    SingleSyncTask_Task |         SingleSyncTask |  94,205.9 μs |    892.21 μs |    834.57 μs |  94,210.1 μs |  1.08 |    0.01 |           - |        - |     - |        111 B |
|             SingleSyncTask_TaskBuilder |         SingleSyncTask | 129,428.4 μs |  1,819.75 μs |  1,702.20 μs | 129,537.8 μs |  1.49 |    0.02 |  91750.0000 |        - |     - |  480000310 B |
|                                        |                        |              |              |              |              |       |         |             |          |       |              |
|             SyncBuilderLoop_NormalCode |                   sync | 826,107.7 μs | 10,326.67 μs |  8,623.24 μs | 825,063.5 μs |  1.00 |    0.00 | 367000.0000 |        - |     - | 1921760000 B |
|           SyncBuilderLoop_WorkflowCode |                   sync | 813,272.6 μs | 10,672.98 μs |  9,461.32 μs | 813,590.3 μs |  0.98 |    0.02 | 367000.0000 |        - |     - | 1921760000 B |
|                                        |                        |              |              |              |              |       |         |             |          |       |              |
|          TinyVariableSizedList_Builder |  TinyVariableSizedList |  47,897.0 μs |    602.09 μs |    533.74 μs |  47,905.8 μs |  1.00 |    0.00 |  20363.6364 |        - |     - |  106666656 B |
|     TinyVariableSizedList_StateMachine |  TinyVariableSizedList |  10,743.3 μs |     67.20 μs |     62.86 μs |  10,755.8 μs |  0.22 |    0.00 |   2031.2500 |        - |     - |   10666665 B |
|      TinyVariableSizedList_InlinedCode |  TinyVariableSizedList |  16,912.1 μs |    147.83 μs |    138.28 μs |  16,899.8 μs |  0.35 |    0.01 |   2031.2500 |        - |     - |   10666676 B |
|                                        |                        |              |              |              |              |       |         |             |          |       |              |
|              VariableSizedList_Builder |      VariableSizedList | 286,776.2 μs |  3,299.21 μs |  2,924.66 μs | 286,845.9 μs |  1.00 |    0.00 |  63000.0000 |        - |     - |  330666624 B |
|         VariableSizedList_StateMachine |      VariableSizedList | 104,452.2 μs |    907.64 μs |    804.60 μs | 104,295.3 μs |  0.36 |    0.00 |  44800.0000 |        - |     - |  234666891 B |
|          VariableSizedList_InlinedCode |      VariableSizedList | 162,624.5 μs |  3,240.43 μs |  6,165.25 μs | 159,685.9 μs |  0.58 |    0.03 |  44750.0000 |        - |     - |  234666624 B |
|                                        |                        |              |              |              |              |       |         |             |          |       |              |
|                  FixedSizeList_Builder |         FixedSizedList |  95,820.3 μs |    991.52 μs |    878.96 μs |  95,709.9 μs |  1.00 |    0.00 |  61166.6667 |        - |     - |  320000000 B |
|             FixedSizeList_StateMachine |         FixedSizedList | 132,487.0 μs |  2,003.12 μs |  1,775.72 μs | 132,172.6 μs |  1.38 |    0.02 |  61000.0000 |        - |     - |  320000334 B |
|              FixedSizeList_InlinedCode |         FixedSizedList | 205,548.8 μs |  2,390.78 μs |  2,236.34 μs | 205,384.5 μs |  2.14 |    0.03 |  61000.0000 |        - |     - |  320000000 B |
|                                        |                        |              |              |              |              |       |         |             |          |       |              |
|         TinyVariableSizedArray_Builder | TinyVariableSizedArray |  87,111.2 μs |  1,226.40 μs |  1,024.10 μs |  86,678.3 μs |  1.00 |    0.00 |  30000.0000 |        - |     - |  157333608 B |
|    TinyVariableSizedArray_StateMachine | TinyVariableSizedArray |  26,585.1 μs |    271.29 μs |    253.76 μs |  26,560.9 μs |  0.31 |    0.00 |  10687.5000 |        - |     - |   55999968 B |
|     TinyVariableSizedArray_InlinedCode | TinyVariableSizedArray |  27,605.7 μs |    337.14 μs |    315.36 μs |  27,578.0 μs |  0.32 |    0.00 |  10687.5000 |        - |     - |   55999968 B |
|                                        |                        |              |              |              |              |       |         |             |          |       |              |
|             VariableSizedArray_Builder |     VariableSizedArray | 363,921.8 μs |  4,287.66 μs |  4,010.68 μs | 362,824.8 μs |  1.00 |    0.00 |  77000.0000 |        - |     - |  405333272 B |
|        VariableSizedArray_StateMachine |     VariableSizedArray | 148,678.5 μs |  2,090.88 μs |  2,237.22 μs | 148,492.1 μs |  0.41 |    0.01 |  59000.0000 |        - |     - |  309333272 B |
|         VariableSizedArray_InlinedCode |     VariableSizedArray | 171,921.1 μs |  2,647.75 μs |  2,210.99 μs | 171,483.6 μs |  0.47 |    0.01 |  59000.0000 |        - |     - |  309333717 B |
|                                        |                        |              |              |              |              |       |         |             |          |       |              |
|                 FixedSizeArray_Builder |        FixedSizedArray |  41,984.8 μs |    537.08 μs |    502.39 μs |  42,199.8 μs |  1.00 |    0.00 |  19833.3333 |        - |     - |  104000000 B |
|            FixedSizeArray_StateMachine |        FixedSizedArray | 189,331.6 μs |  3,766.35 μs |  3,867.76 μs | 189,209.7 μs |  4.51 |    0.12 |  82333.3333 |        - |     - |  432000608 B |
|             FixedSizeArray_InlinedCode |        FixedSizedArray | 213,787.3 μs |  3,551.50 μs |  3,322.07 μs | 213,249.0 μs |  5.09 |    0.10 |  82333.3333 |        - |     - |  432000000 B |
|                                        |                        |              |              |              |              |       |         |             |          |       |              |
|           MultiStepOption_StateMachine |        MultiStepOption |  26,161.4 μs |    482.60 μs |    451.42 μs |  26,011.4 μs |  1.00 |    0.00 |  13437.5000 |        - |     - |   70399992 B |
|         MultiStepOption_InlineIfLambda |        MultiStepOption |  22,772.1 μs |    454.77 μs |    808.35 μs |  22,874.6 μs |  0.87 |    0.03 |  13437.5000 |        - |     - |   70399968 B |
|             MultiStepOption_OldBuilder |        MultiStepOption |  62,819.0 μs |  1,250.39 μs |  2,317.68 μs |  61,890.3 μs |  2.48 |    0.10 |  38750.0000 |        - |     - |  202666632 B |
|              MultiStepOption_NoBuilder |        MultiStepOption |  18,267.2 μs |    236.38 μs |    209.55 μs |  18,230.6 μs |  0.70 |    0.02 |  13437.5000 |        - |     - |   70400008 B |
|                                        |                        |              |              |              |              |       |         |             |          |       |              |
|      MultiStepValueOption_StateMachine |   MultiStepValueOption |  30,136.0 μs |    299.50 μs |    233.83 μs |  30,242.5 μs |  1.00 |    0.00 |           - |        - |     - |         24 B |
|    MultiStepValueOption_InlineIfLambda |   MultiStepValueOption |  27,720.7 μs |    170.67 μs |    151.30 μs |  27,661.1 μs |  0.92 |    0.01 |           - |        - |     - |         20 B |
|        MultiStepValueOption_OldBuilder |   MultiStepValueOption |  65,206.8 μs |    560.50 μs |    496.87 μs |  65,150.9 μs |  2.16 |    0.02 |  19125.0000 |        - |     - |  100266664 B |
|         MultiStepValueOption_NoBuilder |   MultiStepValueOption |  27,648.2 μs |     59.85 μs |     53.05 μs |  27,636.3 μs |  0.92 |    0.01 |           - |        - |     - |         18 B |
|                                        |                        |              |              |              |              |       |         |             |          |       |              |
|                 TaskSeq_NestedForLoops |                taskSeq |           NA |           NA |           NA |           NA |     ? |       ? |           - |        - |     - |            - |
|                AsyncSeq_NestedForLoops |                taskSeq |  16,360.5 μs |    124.66 μs |    110.50 μs |  16,367.6 μs | 24.35 |    0.24 |   4500.0000 | 125.0000 |     - |   23582240 B |
| CSharp_IAsyncEnumerable_NestedForLoops |                taskSeq |     671.5 μs |      4.71 μs |      4.41 μs |     672.6 μs |  1.00 |    0.00 |     24.4141 |        - |     - |     131290 B |

Benchmarks with issues:
  Benchmarks.TaskSeq_NestedForLoops: DefaultJob
