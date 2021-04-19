``` ini

BenchmarkDotNet=v0.11.5, OS=Windows 10.0.19042
Intel Xeon CPU E5-1620 0 3.60GHz, 1 CPU, 8 logical and 4 physical cores
.NET Core SDK=5.0.104
  [Host]     : .NET Core 2.1.27 (CoreCLR 4.6.29916.01, CoreFX 4.6.29916.03), 64bit RyuJIT DEBUG
  DefaultJob : .NET Core 2.1.27 (CoreCLR 4.6.29916.01, CoreFX 4.6.29916.03), 64bit RyuJIT


```
|                              Method |             Categories |      Mean |     Error |     StdDev |    Median | Ratio | RatioSD |      Gen 0 | Gen 1 | Gen 2 | Allocated |
|------------------------------------ |----------------------- |----------:|----------:|-----------:|----------:|------:|--------:|-----------:|------:|------:|----------:|
|  TinyVariableSizedList_StateMachine |  TinyVariableSizedList |  48.88 ms | 0.9511 ms |  0.9341 ms |  48.56 ms |  1.00 |    0.00 | 20272.7273 |     - |     - | 101.73 MB |
|       TinyVariableSizedList_Builder |  TinyVariableSizedList |  17.94 ms | 0.0328 ms |  0.0291 ms |  17.94 ms |  0.37 |    0.01 |  2031.2500 |     - |     - |  10.17 MB |
|                                     |                        |           |           |            |           |       |         |            |       |       |           |
|      VariableSizedList_StateMachine |      VariableSizedList | 292.23 ms | 1.4533 ms |  1.3595 ms | 291.71 ms |  1.00 |    0.00 | 63000.0000 |     - |     - | 315.35 MB |
|           VariableSizedList_Builder |      VariableSizedList | 160.07 ms | 0.6359 ms |  0.5948 ms | 160.12 ms |  0.55 |    0.00 | 44750.0000 |     - |     - |  223.8 MB |
|                                     |                        |           |           |            |           |       |         |            |       |       |           |
|          FixedSizeList_StateMachine |         FixedSizedList |  92.71 ms | 0.5075 ms |  0.4499 ms |  92.55 ms |  1.00 |    0.00 | 61000.0000 |     - |     - | 305.18 MB |
|               FixedSizeList_Builder |         FixedSizedList | 196.73 ms | 0.8574 ms |  0.7601 ms | 196.69 ms |  2.12 |    0.01 | 61000.0000 |     - |     - | 305.18 MB |
|                                     |                        |           |           |            |           |       |         |            |       |       |           |
| TinyVariableSizedArray_StateMachine | TinyVariableSizedArray |  98.47 ms | 0.5679 ms |  0.5034 ms |  98.46 ms |  1.00 |    0.00 | 31500.0000 |     - |     - | 157.67 MB |
|      TinyVariableSizedArray_Builder | TinyVariableSizedArray |  30.49 ms | 0.2252 ms |  0.1996 ms |  30.47 ms |  0.31 |    0.00 | 11187.5000 |     - |     - |  55.95 MB |
|                                     |                        |           |           |            |           |       |         |            |       |       |           |
|     VariableSizedArray_StateMachine |     VariableSizedArray | 386.57 ms | 1.1513 ms |  1.0206 ms | 386.11 ms |  1.00 |    0.00 | 78000.0000 |     - |     - | 394.19 MB |
|          VariableSizedArray_Builder |     VariableSizedArray | 187.86 ms | 3.7548 ms | 10.5289 ms | 183.83 ms |  0.49 |    0.03 | 60333.3333 |     - |     - | 302.63 MB |
|                                     |                        |           |           |            |           |       |         |            |       |       |           |
|         FixedSizeArray_StateMachine |        FixedSizedArray |  40.24 ms | 0.2550 ms |  0.2261 ms |  40.16 ms |  1.00 |    0.00 | 19769.2308 |     - |     - |  99.18 MB |
|              FixedSizeArray_Builder |        FixedSizedArray | 218.93 ms | 1.8969 ms |  1.5840 ms | 218.56 ms |  5.44 |    0.05 | 83666.6667 |     - |     - | 419.62 MB |
