``` ini

BenchmarkDotNet=v0.12.1, OS=Windows 10.0.19042
Intel Xeon CPU E5-1620 0 3.60GHz, 1 CPU, 8 logical and 4 physical cores
.NET Core SDK=5.0.104
  [Host]     : .NET Core 5.0.6 (CoreCLR 5.0.621.22011, CoreFX 5.0.621.22011), X64 RyuJIT DEBUG
  DefaultJob : .NET Core 5.0.6 (CoreCLR 5.0.621.22011, CoreFX 5.0.621.22011), X64 RyuJIT


```
|                                      Method |             Categories |         Mean |        Error |       StdDev |       Median | Ratio | RatioSD |       Gen 0 | Gen 1 | Gen 2 |    Allocated |
|-------------------------------------------- |----------------------- |-------------:|-------------:|-------------:|-------------:|------:|--------:|------------:|------:|------:|-------------:|
|                   ManyWriteFile_CSharpTasks |          ManyWriteFile |  25,930.8 μs |    516.57 μs |    614.94 μs |  25,742.6 μs |  1.00 |    0.00 |    218.7500 |     - |     - |    1125280 B |
|                          ManyWriteFile_Task |          ManyWriteFile |  27,671.4 μs |    528.78 μs |    543.02 μs |  27,691.3 μs |  1.07 |    0.04 |    218.7500 |     - |     - |    1124995 B |
|          ManyWriteFile_TaskUsingTaskBuilder |          ManyWriteFile |  32,466.9 μs |    644.71 μs |  1,441.98 μs |  31,875.5 μs |  1.29 |    0.07 |    800.0000 |     - |     - |    4403297 B |
|                                             |                        |              |              |              |              |       |         |             |       |       |              |
|                   NonAsyncBinds_CSharpTasks |          NonAsyncBinds | 144,056.7 μs |  1,352.17 μs |  1,198.66 μs | 144,336.3 μs |  1.00 |    0.00 | 151500.0000 |     - |     - |  792000334 B |
|                          NonAsyncBinds_Task |          NonAsyncBinds | 152,799.5 μs |  1,919.11 μs |  1,795.13 μs | 152,834.7 μs |  1.06 |    0.02 | 151500.0000 |     - |     - |  792000334 B |
|          NonAsyncBinds_TaskUsingTaskBuilder |          NonAsyncBinds | 229,904.8 μs |  2,725.81 μs |  2,416.36 μs | 229,533.4 μs |  1.60 |    0.02 | 221666.6667 |     - |     - | 1160000445 B |
|                                             |                        |              |              |              |              |       |         |             |       |       |              |
|                      AsyncBinds_CSharpTasks |             AsyncBinds |  87,424.4 μs |  1,711.91 μs |  4,449.48 μs |  86,005.9 μs |  1.00 |    0.00 |    166.6667 |     - |     - |    1120771 B |
|                             AsyncBinds_Task |             AsyncBinds |  89,335.9 μs |  1,842.70 μs |  5,375.25 μs |  88,469.4 μs |  1.04 |    0.08 |    333.3333 |     - |     - |    2081001 B |
|             AsyncBinds_TaskUsingTaskBuilder |             AsyncBinds | 126,024.8 μs |  2,497.27 μs |  7,124.84 μs | 123,109.3 μs |  1.45 |    0.12 |   3000.0000 |     - |     - |   15633947 B |
|                                             |                        |              |              |              |              |       |         |             |       |       |              |
|                  SingleSyncTask_CSharpTasks |         SingleSyncTask |  87,943.1 μs |  1,751.66 μs |  3,021.52 μs |  86,573.0 μs |  1.00 |    0.00 |           - |     - |     - |            - |
|                         SingleSyncTask_Task |         SingleSyncTask |  94,410.3 μs |  1,792.08 μs |  4,721.06 μs |  92,178.2 μs |  1.09 |    0.06 |           - |     - |     - |            - |
|         SingleSyncTask_TaskUsingTaskBuilder |         SingleSyncTask | 129,338.5 μs |  2,836.14 μs |  7,999.38 μs | 128,253.2 μs |  1.47 |    0.11 |  91800.0000 |     - |     - |  480000000 B |
|                                             |                        |              |              |              |              |       |         |             |       |       |              |
|                  SyncBuilderLoop_NormalCode |                   sync | 803,207.8 μs | 14,094.53 μs | 17,309.35 μs | 804,164.9 μs |  1.00 |    0.00 | 367000.0000 |     - |     - | 1921760000 B |
|                SyncBuilderLoop_WorkflowCode |                   sync | 787,211.0 μs | 11,184.81 μs |  9,339.82 μs | 784,381.3 μs |  0.98 |    0.02 | 367000.0000 |     - |     - | 1921760000 B |
|                                             |                        |              |              |              |              |       |         |             |       |       |              |
|               TinyVariableSizedList_Builtin |  TinyVariableSizedList |  55,382.9 μs |    637.04 μs |    595.89 μs |  55,337.2 μs |  1.00 |    0.00 |  20333.3333 |     - |     - |  106668141 B |
|            TinyVariableSizedList_NewBuilder |  TinyVariableSizedList |  15,871.0 μs |    307.74 μs |    441.35 μs |  15,769.2 μs |  0.29 |    0.01 |   2031.2500 |     - |     - |   10666656 B |
|                                             |                        |              |              |              |              |       |         |             |       |       |              |
|                   VariableSizedList_Builtin |      VariableSizedList | 307,225.4 μs |  4,958.21 μs |  5,511.03 μs | 306,346.6 μs |  1.00 |    0.00 |  63000.0000 |     - |     - |  330679664 B |
|                VariableSizedList_NewBuilder |      VariableSizedList | 154,245.4 μs |  2,676.51 μs |  3,663.63 μs | 152,891.7 μs |  0.50 |    0.01 |  44750.0000 |     - |     - |  234666624 B |
|                                             |                        |              |              |              |              |       |         |             |       |       |              |
|                       FixedSizeList_Builtin |         FixedSizedList |  91,856.7 μs |  1,809.06 μs |  1,857.77 μs |  91,312.2 μs |  1.00 |    0.00 |  61166.6667 |     - |     - |  320000000 B |
|                    FixedSizeList_NewBuilder |         FixedSizedList | 204,361.3 μs |  3,321.46 μs |  3,106.90 μs | 203,793.2 μs |  2.22 |    0.05 |  61000.0000 |     - |     - |  320000445 B |
|                                             |                        |              |              |              |              |       |         |             |       |       |              |
|              TinyVariableSizedArray_Builtin | TinyVariableSizedArray |  90,656.4 μs |    701.78 μs |    586.01 μs |  90,624.8 μs |  1.00 |    0.00 |  30000.0000 |     - |     - |  157335448 B |
|           TinyVariableSizedArray_NewBuilder | TinyVariableSizedArray |  27,055.8 μs |    523.23 μs |  1,008.08 μs |  26,991.9 μs |  0.30 |    0.01 |  10687.5000 |     - |     - |   55999968 B |
|                                             |                        |              |              |              |              |       |         |             |       |       |              |
|                  VariableSizedArray_Builtin |     VariableSizedArray | 384,421.0 μs |  9,254.21 μs | 26,848.15 μs | 377,376.3 μs |  1.00 |    0.00 |  77000.0000 |     - |     - |  405333272 B |
|               VariableSizedArray_NewBuilder |     VariableSizedArray | 160,481.4 μs |  2,526.36 μs |  1,972.42 μs | 159,714.1 μs |  0.40 |    0.03 |  59000.0000 |     - |     - |  309333606 B |
|                                             |                        |              |              |              |              |       |         |             |       |       |              |
|                      FixedSizeArray_Builtin |        FixedSizedArray |  28,638.1 μs |    292.65 μs |    259.43 μs |  28,637.4 μs |  1.00 |    0.00 |  19875.0000 |     - |     - |  104000000 B |
|                   FixedSizeArray_NewBuilder |        FixedSizedArray | 197,035.1 μs |  1,699.56 μs |  1,589.77 μs | 196,753.9 μs |  6.88 |    0.08 |  82333.3333 |     - |     - |  432000445 B |
|                                             |                        |              |              |              |              |       |         |             |       |       |              |
|                  MultiStepOption_OldBuilder |        MultiStepOption |  59,013.6 μs |    695.08 μs |    616.17 μs |  59,045.5 μs |  1.00 |    0.00 |  38666.6667 |     - |     - |  202668102 B |
|                  MultiStepOption_NewBuilder |        MultiStepOption |  17,343.1 μs |    140.28 μs |    117.14 μs |  17,297.2 μs |  0.29 |    0.00 |  13437.5000 |     - |     - |   70399968 B |
|                   MultiStepOption_NoBuilder |        MultiStepOption |  16,538.0 μs |    235.65 μs |    220.43 μs |  16,527.8 μs |  0.28 |    0.00 |  13437.5000 |     - |     - |   70399968 B |
|                                             |                        |              |              |              |              |       |         |             |       |       |              |
|             MultiStepValueOption_OldBuilder |   MultiStepValueOption |  45,371.5 μs |    321.14 μs |    268.17 μs |  45,354.0 μs |  1.00 |    0.00 |  19166.6667 |     - |     - |  100266664 B |
|             MultiStepValueOption_NewBuilder |   MultiStepValueOption |   3,860.5 μs |     76.92 μs |    107.83 μs |   3,841.5 μs |  0.08 |    0.00 |           - |     - |     - |            - |
|              MultiStepValueOption_NoBuilder |   MultiStepValueOption |   3,754.6 μs |     29.81 μs |     26.43 μs |   3,754.7 μs |  0.08 |    0.00 |           - |     - |     - |            - |
|                                             |                        |              |              |              |              |       |         |             |       |       |              |
| NestedForLoops_TaskSeqUsingRawResumableCode |                taskSeq |     927.6 μs |     10.16 μs |      9.51 μs |     926.2 μs |  1.53 |    0.02 |     58.5938 |     - |     - |     307609 B |
|        NestedForLoops_CSharpAsyncEnumerable |                taskSeq |     605.4 μs |      8.16 μs |      6.81 μs |     604.9 μs |  1.00 |    0.00 |     24.4141 |     - |     - |     131280 B |
