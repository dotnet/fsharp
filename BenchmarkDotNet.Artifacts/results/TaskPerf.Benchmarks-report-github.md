``` ini

BenchmarkDotNet=v0.11.5, OS=Windows 10.0.19042
Intel Xeon CPU E5-1620 0 3.60GHz, 1 CPU, 8 logical and 4 physical cores
.NET Core SDK=5.0.104
  [Host]     : .NET Core 2.1.27 (CoreCLR 4.6.29916.01, CoreFX 4.6.29916.03), 64bit RyuJIT DEBUG
  DefaultJob : .NET Core 2.1.27 (CoreCLR 4.6.29916.01, CoreFX 4.6.29916.03), 64bit RyuJIT


```
|                              Method |             Categories |      Mean |     Error |    StdDev | Ratio | RatioSD |      Gen 0 | Gen 1 | Gen 2 | Allocated |
|------------------------------------ |----------------------- |----------:|----------:|----------:|------:|--------:|-----------:|------:|------:|----------:|
| TinyVariableSizedArray_StateMachine | TinyVariableSizedArray | 101.11 ms | 1.6023 ms | 1.4988 ms |  1.00 |    0.00 | 31400.0000 |     - |     - | 157.67 MB |
|      TinyVariableSizedArray_Builder | TinyVariableSizedArray |  30.99 ms | 0.4923 ms | 0.4605 ms |  0.31 |    0.01 | 11187.5000 |     - |     - |  55.95 MB |
|                                     |                        |           |           |           |       |         |            |       |       |           |
|     VariableSizedArray_StateMachine |     VariableSizedArray | 388.66 ms | 4.3207 ms | 4.0416 ms |  1.00 |    0.00 | 78000.0000 |     - |     - | 394.19 MB |
|          VariableSizedArray_Builder |     VariableSizedArray | 178.17 ms | 0.6523 ms | 0.5092 ms |  0.46 |    0.00 | 60333.3333 |     - |     - | 302.63 MB |
|                                     |                        |           |           |           |       |         |            |       |       |           |
|         FixedSizeArray_StateMachine |        FixedSizedArray |  40.40 ms | 0.5243 ms | 0.4905 ms |  1.00 |    0.00 | 19769.2308 |     - |     - |  99.18 MB |
|              FixedSizeArray_Builder |        FixedSizedArray | 220.69 ms | 2.5385 ms | 2.3745 ms |  5.46 |    0.09 | 83666.6667 |     - |     - | 419.62 MB |
