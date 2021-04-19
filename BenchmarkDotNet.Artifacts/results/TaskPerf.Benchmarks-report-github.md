``` ini

BenchmarkDotNet=v0.11.5, OS=Windows 10.0.19042
Intel Xeon CPU E5-1620 0 3.60GHz, 1 CPU, 8 logical and 4 physical cores
.NET Core SDK=5.0.104
  [Host]     : .NET Core 2.1.27 (CoreCLR 4.6.29916.01, CoreFX 4.6.29916.03), 64bit RyuJIT DEBUG
  DefaultJob : .NET Core 2.1.27 (CoreCLR 4.6.29916.01, CoreFX 4.6.29916.03), 64bit RyuJIT

Categories=taskSeq  

```
|                          Method |        Mean |      Error |     StdDev | Ratio | RatioSD |     Gen 0 |    Gen 1 | Gen 2 | Allocated |
|-------------------------------- |------------:|-----------:|-----------:|------:|--------:|----------:|---------:|------:|----------:|
|                 TaskSeq_Example |  1,000.2 us |   4.344 us |   3.851 us |  1.60 |    0.01 |  136.7188 |        - |     - |  722752 B |
|                AsyncSeq_Example | 15,845.7 us | 307.510 us | 469.601 us | 25.14 |    0.64 | 4468.7500 | 125.0000 |     - |     880 B |
| CSharp_IAsyncEnumerable_Example |    626.6 us |   3.110 us |   2.757 us |  1.00 |    0.00 |   26.3672 |        - |     - |  142208 B |
