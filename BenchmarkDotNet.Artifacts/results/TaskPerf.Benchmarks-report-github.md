``` ini

BenchmarkDotNet=v0.11.5, OS=Windows 10.0.18363
Intel Core i7-6820HQ CPU 2.70GHz (Skylake), 1 CPU, 8 logical and 4 physical cores
.NET Core SDK=3.1.200
  [Host]     : .NET Core 2.1.16 (CoreCLR 4.6.28516.03, CoreFX 4.6.28516.10), 64bit RyuJIT DEBUG
  DefaultJob : .NET Core 2.1.16 (CoreCLR 4.6.28516.03, CoreFX 4.6.28516.10), 64bit RyuJIT


```
|                          Method |     Categories |           Mean |         Error |        StdDev |         Median | Ratio | RatioSD |        Gen 0 | Gen 1 | Gen 2 |    Allocated |
|-------------------------------- |--------------- |---------------:|--------------:|--------------:|---------------:|------:|--------:|-------------:|------:|------:|-------------:|
|       ManyWriteFile_CSharpAsync |  ManyWriteFile |    22,842.4 us |    437.069 us |    598.264 us |    22,898.2 us |  1.00 |    0.00 |     468.7500 |     - |     - |       1104 B |
|              ManyWriteFile_Task |  ManyWriteFile |    24,157.2 us |    380.028 us |    355.479 us |    24,041.5 us |  1.06 |    0.03 |     468.7500 |     - |     - |       1136 B |
|       ManyWriteFile_TaskBuilder |  ManyWriteFile |    27,138.5 us |    530.346 us |    825.685 us |    26,977.0 us |  1.19 |    0.04 |    1250.0000 |     - |     - |       1792 B |
|       ManyWriteFile_FSharpAsync |  ManyWriteFile |    41,970.7 us |    383.088 us |    339.597 us |    41,952.3 us |  1.84 |    0.04 |    2000.0000 |     - |     - |       3648 B |
|                                 |                |                |               |               |                |       |         |              |       |       |              |
|       NonAsyncBinds_CSharpAsync |  NonAsyncBinds |   134,061.4 us |  1,305.710 us |  1,090.327 us |   134,135.5 us |  1.00 |    0.00 |  188750.0000 |     - |     - |  792000000 B |
|              NonAsyncBinds_Task |  NonAsyncBinds |   155,811.3 us |  3,062.787 us |  5,032.246 us |   154,195.7 us |  1.19 |    0.04 |  188750.0000 |     - |     - |  792000000 B |
|       NonAsyncBinds_TaskBuilder |  NonAsyncBinds |   223,803.6 us |  2,552.347 us |  2,262.589 us |   223,413.3 us |  1.67 |    0.03 |  288000.0000 |     - |     - | 1208000000 B |
|       NonAsyncBinds_FSharpAsync |  NonAsyncBinds | 1,253,221.7 us | 20,811.200 us | 18,448.586 us | 1,247,145.3 us |  9.35 |    0.17 |  694000.0000 |     - |     - | 2912000000 B |
|                                 |                |                |               |               |                |       |         |              |       |       |              |
|          AsyncBinds_CSharpAsync |     AsyncBinds |   105,363.2 us |  1,516.911 us |  1,418.920 us |   105,053.1 us |  1.00 |    0.00 |    1200.0000 |     - |     - |    1529206 B |
|                 AsyncBinds_Task |     AsyncBinds |   105,252.8 us |    724.876 us |    678.050 us |   105,289.2 us |  1.00 |    0.02 |    1400.0000 |     - |     - |    2490766 B |
|          AsyncBinds_TaskBuilder |     AsyncBinds |   133,925.3 us |  2,493.420 us |  2,332.346 us |   133,445.0 us |  1.27 |    0.02 |    4000.0000 |     - |     - |    4972798 B |
|                                 |                |                |               |               |                |       |         |              |       |       |              |
|      SingleSyncTask_CSharpAsync | SingleSyncTask |    77,543.5 us |    822.760 us |    769.610 us |    77,225.8 us |  1.00 |    0.00 |            - |     - |     - |            - |
|             SingleSyncTask_Task | SingleSyncTask |   130,568.1 us |    962.270 us |    853.027 us |   130,681.5 us |  1.68 |    0.02 |            - |     - |     - |            - |
|      SingleSyncTask_TaskBuilder | SingleSyncTask |   139,269.3 us |  1,671.650 us |  1,395.904 us |   139,271.1 us |  1.80 |    0.02 |  143000.0000 |     - |     - |  600000000 B |
|      SingleSyncTask_FSharpAsync | SingleSyncTask | 1,725,565.2 us | 22,510.555 us | 19,955.021 us | 1,720,283.7 us | 22.25 |    0.36 | 1029000.0000 |     - |     - | 4320000000 B |
|                                 |                |                |               |               |                |       |         |              |       |       |              |
|      SyncBuilderLoop_NormalCode |           sync |   856,111.2 us |  5,416.215 us |  5,066.331 us |   855,589.7 us |  1.00 |    0.00 |  458000.0000 |     - |     - | 1922000000 B |
|    SyncBuilderLoop_WorkflowCode |           sync |   844,729.7 us |  6,792.670 us |  6,353.868 us |   846,315.2 us |  0.99 |    0.01 |  458000.0000 |     - |     - | 1922480000 B |
|                                 |                |                |               |               |                |       |         |              |       |       |              |
|      ListBuilder_ListExpression |           list |   449,594.3 us |  8,855.140 us |  8,283.103 us |   448,293.9 us |  1.00 |    0.00 |   99000.0000 |     - |     - |  416000000 B |
|         ListBuilder_ListBuilder |           list |   908,550.8 us |  8,034.259 us |  7,515.251 us |   909,338.1 us |  2.02 |    0.04 |  663000.0000 |     - |     - | 2781332944 B |
|                                 |                |                |               |               |                |       |         |              |       |       |              |
|    ArrayBuilder_ArrayExpression |          array |   344,607.2 us |  6,882.650 us | 13,903.293 us |   338,209.7 us |  1.00 |    0.00 |   98000.0000 |     - |     - |  413333272 B |
|       ArrayBuilder_ArrayBuilder |          array |   698,442.9 us | 13,619.569 us | 19,092.734 us |   691,483.0 us |  2.00 |    0.11 |  617000.0000 |     - |     - | 2589332976 B |
|                                 |                |                |               |               |                |       |         |              |       |       |              |
|                 TaskSeq_Example |        taskSeq |     1,886.0 us |     19.958 us |     15.582 us |     1,884.5 us |  2.92 |    0.04 |     824.2188 |     - |     - |    3464136 B |
|                AsyncSeq_Example |        taskSeq |    14,105.1 us |    282.094 us |    366.802 us |    14,056.9 us | 21.96 |    0.67 |    5625.0000 |     - |     - |   23616952 B |
| CSharp_IAsyncEnumerable_Example |        taskSeq |       646.5 us |      6.898 us |      6.453 us |       646.7 us |  1.00 |    0.00 |      33.2031 |     - |     - |     142232 B |
