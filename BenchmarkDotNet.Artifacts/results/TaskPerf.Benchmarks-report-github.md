``` ini

BenchmarkDotNet=v0.11.5, OS=Windows 10.0.19042
Intel Xeon CPU E5-1620 0 3.60GHz, 1 CPU, 8 logical and 4 physical cores
.NET Core SDK=5.0.104
  [Host]     : .NET Core 2.1.27 (CoreCLR 4.6.29916.01, CoreFX 4.6.29916.03), 64bit RyuJIT DEBUG
  DefaultJob : .NET Core 2.1.27 (CoreCLR 4.6.29916.01, CoreFX 4.6.29916.03), 64bit RyuJIT


```
|                       Method | Categories |     Mean |    Error |   StdDev | Ratio |      Gen 0 | Gen 1 | Gen 2 | Allocated |
|----------------------------- |----------- |---------:|---------:|---------:|------:|-----------:|------:|------:|----------:|
|   ListBuilder_ListExpression |       list | 307.7 ms | 2.656 ms | 2.485 ms |  1.00 | 63000.0000 |     - |     - | 315.35 MB |
|      ListBuilder_ListBuilder |       list | 154.3 ms | 2.900 ms | 2.713 ms |  0.50 | 44750.0000 |     - |     - |  223.8 MB |
|                              |            |          |          |          |       |            |       |       |           |
| ArrayBuilder_ArrayExpression |      array | 387.4 ms | 4.493 ms | 4.203 ms |  1.00 | 78000.0000 |     - |     - | 394.19 MB |
|    ArrayBuilder_ArrayBuilder |      array | 178.2 ms | 1.893 ms | 1.770 ms |  0.46 | 60333.3333 |     - |     - | 302.63 MB |
