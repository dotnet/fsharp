``` ini

BenchmarkDotNet=v0.11.5, OS=Windows 10.0.19042
Intel Xeon CPU E5-1620 0 3.60GHz, 1 CPU, 8 logical and 4 physical cores
.NET Core SDK=5.0.104
  [Host]     : .NET Core 2.1.27 (CoreCLR 4.6.29916.01, CoreFX 4.6.29916.03), 64bit RyuJIT DEBUG
  DefaultJob : .NET Core 2.1.27 (CoreCLR 4.6.29916.01, CoreFX 4.6.29916.03), 64bit RyuJIT


```
|                              Method |                                  Categories |      Mean |     Error |     StdDev |    Median | Ratio | RatioSD |      Gen 0 | Gen 1 | Gen 2 | Allocated |
|------------------------------------ |-------------------------------------------- |----------:|----------:|-----------:|----------:|------:|--------:|-----------:|------:|------:|----------:|
|  TinyVariableSizedList_StateMachine | TinyVariableSizedResizeArrayBuilderCodeList |  48.66 ms | 0.6821 ms |  0.6381 ms |  48.33 ms |  1.00 |    0.00 | 20300.0000 |     - |     - | 101.73 MB |
|                                     |                                             |           |           |            |           |       |         |            |       |       |           |
|       TinyVariableSizedList_Builder |                       TinyVariableSizedList |  17.95 ms | 0.2052 ms |  0.1819 ms |  17.92 ms |     ? |       ? |  2031.2500 |     - |     - |  10.17 MB |
|                                     |                                             |           |           |            |           |       |         |            |       |       |           |
|      VariableSizedList_StateMachine |                           VariableSizedList | 296.67 ms | 4.6649 ms |  4.3636 ms | 297.19 ms |  1.00 |    0.00 | 63000.0000 |     - |     - | 315.35 MB |
|           VariableSizedList_Builder |                           VariableSizedList | 179.32 ms | 4.2550 ms | 12.1397 ms | 175.47 ms |  0.63 |    0.05 | 44666.6667 |     - |     - |  223.8 MB |
|                                     |                                             |           |           |            |           |       |         |            |       |       |           |
|          FixedSizeList_StateMachine |                              FixedSizedList |  95.81 ms | 1.9153 ms |  3.5974 ms |  95.01 ms |  1.00 |    0.00 | 61000.0000 |     - |     - | 305.18 MB |
|               FixedSizeList_Builder |                              FixedSizedList | 198.75 ms | 1.7705 ms |  1.5695 ms | 198.23 ms |  2.03 |    0.08 | 61000.0000 |     - |     - | 305.18 MB |
|                                     |                                             |           |           |            |           |       |         |            |       |       |           |
| TinyVariableSizedArray_StateMachine |                      TinyVariableSizedArray | 103.82 ms | 1.9652 ms |  2.3395 ms | 103.77 ms |  1.00 |    0.00 | 31500.0000 |     - |     - | 157.67 MB |
|      TinyVariableSizedArray_Builder |                      TinyVariableSizedArray |  31.62 ms | 0.5074 ms |  0.4498 ms |  31.56 ms |  0.30 |    0.01 | 11187.5000 |     - |     - |  55.95 MB |
|                                     |                                             |           |           |            |           |       |         |            |       |       |           |
|     VariableSizedArray_StateMachine |                          VariableSizedArray | 407.25 ms | 8.7037 ms |  9.6741 ms | 405.43 ms |  1.00 |    0.00 | 78000.0000 |     - |     - | 394.19 MB |
|          VariableSizedArray_Builder |                          VariableSizedArray | 185.80 ms | 2.3786 ms |  2.2250 ms | 185.13 ms |  0.46 |    0.01 | 60333.3333 |     - |     - | 302.63 MB |
|                                     |                                             |           |           |            |           |       |         |            |       |       |           |
|         FixedSizeArray_StateMachine |                             FixedSizedArray |  43.21 ms | 0.8454 ms |  1.1572 ms |  43.27 ms |  1.00 |    0.00 | 19833.3333 |     - |     - |  99.18 MB |
|              FixedSizeArray_Builder |                             FixedSizedArray | 233.24 ms | 4.8559 ms |  5.9635 ms | 232.74 ms |  5.40 |    0.22 | 83666.6667 |     - |     - | 419.62 MB |
