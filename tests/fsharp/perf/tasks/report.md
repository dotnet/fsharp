
|                    Method |     Mean |     Error |    StdDev | Ratio | RatioSD |     Gen 0 | Gen 1 | Gen 2 | Allocated |
|-------------------------- |---------:|----------:|----------:|------:|--------:|----------:|------:|------:|----------:|
| ManyWriteFile_CSharpAsync | 22.97 ms | 0.4425 ms | 0.5096 ms |  1.00 |    0.00 |  468.7500 |     - |     - |   1.08 KB |
|        ManyWriteFile_Task | 25.46 ms | 0.4946 ms | 0.7249 ms |  1.12 |    0.03 | 1250.0000 |     - |     - |   1.75 KB |
| ManyWriteFile_TaskBuilder | 25.70 ms | 0.5099 ms | 1.2978 ms |  1.12 |    0.06 | 1250.0000 |     - |     - |   1.75 KB |
| ManyWriteFile_FSharpAsync | 38.54 ms | 0.5229 ms | 0.4891 ms |  1.67 |    0.05 | 2000.0000 |     - |     - |   3.56 KB |


|                    Method |       Mean |     Error |    StdDev | Ratio | RatioSD |       Gen 0 | Gen 1 | Gen 2 |  Allocated |
|-------------------------- |-----------:|----------:|----------:|------:|--------:|------------:|------:|------:|-----------:|
| NonAsyncBinds_CSharpAsync |   124.5 ms |  1.452 ms |  1.358 ms |  1.00 |    0.00 | 188800.0000 |     - |     - |  755.31 MB |
|        NonAsyncBinds_Task |   202.6 ms |  2.773 ms |  2.315 ms |  1.63 |    0.04 | 288000.0000 |     - |     - | 1152.04 MB |
| NonAsyncBinds_TaskBuilder |   205.2 ms |  2.541 ms |  2.252 ms |  1.65 |    0.03 | 288000.0000 |     - |     - | 1152.04 MB |
| NonAsyncBinds_FSharpAsync | 1,168.4 ms | 13.681 ms | 12.128 ms |  9.39 |    0.13 | 694000.0000 |     - |     - |  2777.1 MB |


|                 Method |      Mean |     Error |    StdDev | Ratio | RatioSD |     Gen 0 | Gen 1 | Gen 2 | Allocated |
|----------------------- |----------:|----------:|----------:|------:|--------:|----------:|------:|------:|----------:|
| AsyncBinds_CSharpAsync |  99.94 ms | 0.9770 ms | 0.9139 ms |  1.00 |    0.00 | 1200.0000 |     - |     - |   1.46 MB |
|        AsyncBinds_Task | 134.89 ms | 1.8871 ms | 1.7652 ms |  1.35 |    0.02 | 4000.0000 |     - |     - |   4.82 MB |
| AsyncBinds_TaskBuilder | 135.21 ms | 2.6237 ms | 2.6943 ms |  1.35 |    0.03 | 4000.0000 |     - |     - |   4.78 MB |


|                     Method |        Mean |      Error |     StdDev | Ratio | RatioSD |        Gen 0 | Gen 1 | Gen 2 |    Allocated |
|--------------------------- |------------:|-----------:|-----------:|------:|--------:|-------------:|------:|------:|-------------:|
| SingleSyncTask_CSharpAsync |    75.89 ms |  0.7138 ms |  0.6327 ms |  1.00 |    0.00 |            - |     - |     - |            - |
|        SingleSyncTask_Task |   123.78 ms |  2.0786 ms |  1.9444 ms |  1.63 |    0.03 |  143000.0000 |     - |     - |  600000000 B |
| SingleSyncTask_TaskBuilder |   124.57 ms |  2.3291 ms |  2.3918 ms |  1.64 |    0.04 |  143000.0000 |     - |     - |  600000000 B |
| SingleSyncTask_FSharpAsync | 1,785.20 ms | 34.8378 ms | 38.7222 ms | 23.50 |    0.62 | 1029000.0000 |     - |     - | 4320000000 B |


|                     Method |       Mean |    Error |   StdDev | Ratio |        Gen 0 | Gen 1 | Gen 2 | Allocated |
|--------------------------- |-----------:|---------:|---------:|------:|-------------:|------:|------:|----------:|
| ListBuilder_ListExpression | 3,701.0 ms | 67.22 ms | 56.13 ms |  1.00 | 1422000.0000 |     - |     - |   5.56 GB |
|    ListBuilder_ListBuilder |   865.3 ms | 12.83 ms | 12.00 ms |  0.23 |  663000.0000 |     - |     - |   2.59 GB |

|                       Method |       Mean |     Error |    StdDev | Ratio |        Gen 0 | Gen 1 | Gen 2 | Allocated |
|----------------------------- |-----------:|----------:|----------:|------:|-------------:|------:|------:|----------:|
| ArrayBuilder_ArrayExpression | 3,134.1 ms | 22.932 ms | 20.328 ms |  1.00 | 1131000.0000 |     - |     - |   4.42 GB |
|    ArrayBuilder_ArrayBuilder |   652.4 ms |  8.879 ms |  7.414 ms |  0.21 |  617000.0000 |     - |     - |   2.41 GB |


|                       Method |     Mean |     Error |    StdDev | Ratio | RatioSD |       Gen 0 | Gen 1 | Gen 2 | Allocated |
|----------------------------- |---------:|----------:|----------:|------:|--------:|------------:|------:|------:|----------:|
|   SyncBuilderLoop_NormalCode | 789.4 ms |  7.602 ms |  6.739 ms |  1.00 |    0.00 | 458000.0000 |     - |     - |   1.79 GB |
| SyncBuilderLoop_WorkflowCode | 794.6 ms | 12.181 ms | 10.799 ms |  1.01 |    0.02 | 458000.0000 |     - |     - |   1.79 GB |

