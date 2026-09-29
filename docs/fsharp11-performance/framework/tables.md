# Portable-payload runtime comparison

Generated medians; all successful primary observations retained. See [methodology](README.md) for arm definitions and uncertainty.

## Compilation

| Workload | Arm | Allocated GiB | Peak RAM MiB | Peak private commit MiB | G0 | G1 | G2 | CPU s | Wall s |
| --- | --- | --- | --- | --- | --- | --- | --- | --- | --- |
| FSharp.Core | old-framework | 7.734 | 1712.1 | 1912.4 | 8 | 4 | 2 | 39.63 | 26.53 |
| FSharp.Core | new-framework | 5.737 | 2121.2 | 2315.3 | 5 | 2 | 1 | 39.94 | 25.02 |
| FSharp.Core | old-net10 | 7.598 | 665.6 | 692.4 | 76.5 | 28 | 8 | 36.16 | 27.91 |
| FSharp.Core | new-net10 | 5.612 | 765.3 | 797.9 | 52 | 17 | 6.5 | 35.10 | 21.85 |
| FSharp.Core | old-net11 | 7.598 | 657.2 | 686.5 | 76.5 | 28 | 8 | 36.00 | 27.85 |
| FSharp.Core | new-net11 | 5.608 | 785.5 | 806.0 | 51 | 17 | 7 | 36.27 | 22.58 |
| FSharp.Compiler.Service | old-framework | 37.364 | 4091.3 | 4301.8 | 20 | 11 | 5 | 155.09 | 111.58 |
| FSharp.Compiler.Service | new-framework | 28.169 | 4911.5 | 5143.5 | 14 | 7.5 | 4 | 184.01 | 56.26 |
| FSharp.Compiler.Service | old-net10 | 35.247 | 2500.6 | 2588.7 | 130.5 | 55 | 14 | 123.20 | 75.49 |
| FSharp.Compiler.Service | new-net10 | 27.141 | 3053.9 | 3146.0 | 94 | 43 | 13 | 213.74 | 37.42 |
| FSharp.Compiler.Service | old-net11 | 35.250 | 2500.0 | 2580.7 | 129 | 53.5 | 14 | 130.76 | 81.27 |
| FSharp.Compiler.Service | new-net11 | 27.175 | 3133.6 | 3256.8 | 92.5 | 42 | 13 | 220.36 | 38.19 |
| FsToolkit.ErrorHandling | old-framework | 2.248 | 1415.4 | 1615.2 | 3 | 2 | 1 | 20.56 | 13.03 |
| FsToolkit.ErrorHandling | new-framework | 1.759 | 986.1 | 1174.0 | 3 | 2 | 1 | 25.92 | 11.23 |
| FsToolkit.ErrorHandling | old-net10 | 2.215 | 533.1 | 524.2 | 36 | 14 | 9 | 15.05 | 11.30 |
| FsToolkit.ErrorHandling | new-net10 | 1.721 | 541.3 | 578.4 | 30 | 13 | 7 | 15.53 | 8.62 |
| FsToolkit.ErrorHandling | old-net11 | 2.216 | 517.3 | 578.9 | 38 | 14 | 8 | 15.41 | 11.24 |
| FsToolkit.ErrorHandling | new-net11 | 1.722 | 527.2 | 611.7 | 30 | 13 | 7 | 15.96 | 8.92 |
| Oxpecker | old-framework | 1.155 | 1111.3 | 1151.7 | 1 | 0 | 0 | 17.18 | 11.21 |
| Oxpecker | new-framework | 0.593 | 541.8 | 582.1 | 1 | 0 | 0 | 20.20 | 10.23 |
| Oxpecker | old-net10 | 1.152 | 395.2 | 381.2 | 25 | 12 | 5 | 10.32 | 9.15 |
| Oxpecker | new-net10 | 0.589 | 322.1 | 275.6 | 19 | 9 | 5 | 8.59 | 6.34 |
| Oxpecker | old-net11 | 1.150 | 399.0 | 384.2 | 26 | 13 | 6 | 11.19 | 9.93 |
| Oxpecker | new-net11 | 0.588 | 322.0 | 302.4 | 19 | 9 | 5 | 9.16 | 6.66 |
| Nu | old-framework | 15.518 | 3338.0 | 3533.9 | 9 | 5 | 2 | 72.32 | 51.66 |
| Nu | new-framework | 11.975 | 3371.1 | 3556.3 | 7 | 4 | 2 | 69.58 | 44.29 |
| Nu | old-net10 | 14.734 | 1257.4 | 1432.8 | 79 | 33.5 | 10 | 63.99 | 41.80 |
| Nu | new-net10 | 11.405 | 1409.9 | 1530.5 | 64 | 28 | 11 | 66.18 | 36.62 |
| Nu | old-net11 | 14.766 | 1306.4 | 1504.3 | 84 | 35 | 12 | 65.91 | 43.40 |
| Nu | new-net11 | 11.402 | 1414.9 | 1527.2 | 67 | 28 | 11 | 67.78 | 36.16 |
| FsAutoComplete | old-framework | 5.987 | 2272.6 | 2464.6 | 7 | 4 | 2 | 46.14 | 18.11 |
| FsAutoComplete | new-framework | 4.658 | 2157.5 | 2375.0 | 6 | 3 | 2 | 42.89 | 14.78 |
| FsAutoComplete | old-net10 | 5.806 | 1023.2 | 1166.1 | 50 | 23 | 11 | 44.59 | 18.39 |
| FsAutoComplete | new-net10 | 4.524 | 957.5 | 1079.7 | 43 | 20 | 10 | 42.77 | 15.69 |
| FsAutoComplete | old-net11 | 5.811 | 1021.8 | 1171.3 | 49.5 | 23 | 11 | 48.83 | 20.44 |
| FsAutoComplete | new-net11 | 4.524 | 951.8 | 1046.3 | 43.5 | 20 | 10 | 43.64 | 16.28 |

## IDE graphs

| Workload | Arm | Allocated GiB | Peak RAM MiB | Peak private commit MiB | G0 | G1 | G2 | CPU s | Wall s | RAM after 10 s idle MiB | Retained managed heap MiB |
| --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- |
| Oxpecker | old-framework | 1.492 | 1113.3 | 1144.2 | 2 | 1 | 0 | 12.21 | 10.60 | 1095.4 | 108.4 |
| Oxpecker | new-framework | 0.590 | 572.2 | 604.8 | 1 | 0 | 0 | 9.54 | 8.83 | 554.4 | 75.2 |
| Oxpecker | old-net10 | 1.471 | 383.9 | 342.5 | 34.5 | 13 | 6 | 11.05 | 9.35 | 392.8 | 108.0 |
| Oxpecker | new-net10 | 0.586 | 285.7 | 241.4 | 16 | 9 | 4 | 7.41 | 6.56 | 295.6 | 75.1 |
| Oxpecker | old-net11 | 1.467 | 385.6 | 339.8 | 34 | 14 | 7 | 12.20 | 10.09 | 396.1 | 107.9 |
| Oxpecker | new-net11 | 0.584 | 287.3 | 242.5 | 20 | 8 | 4 | 8.20 | 7.15 | 299.4 | 75.0 |
| FsAutoComplete | old-framework | 6.249 | 2497.3 | 2676.4 | 7 | 4 | 2 | 35.84 | 18.78 | 2497.1 | 545.0 |
| FsAutoComplete | new-framework | 3.964 | 2057.3 | 2238.2 | 4 | 2 | 1 | 27.32 | 16.10 | 2057.4 | 380.7 |
| FsAutoComplete | old-net10 | 6.080 | 1174.0 | 1305.5 | 48 | 23 | 12 | 47.98 | 23.62 | 1193.1 | 544.1 |
| FsAutoComplete | new-net10 | 3.857 | 841.6 | 904.4 | 40 | 18 | 10 | 37.34 | 18.48 | 852.4 | 380.2 |
| FsAutoComplete | old-net11 | 6.081 | 1173.3 | 1289.4 | 50.5 | 23.5 | 11 | 50.13 | 23.28 | 1191.4 | 543.8 |
| FsAutoComplete | new-net11 | 3.850 | 860.7 | 952.3 | 41 | 19.5 | 11 | 39.47 | 19.41 | 862.7 | 380.1 |

## Applications: length four, bytes allocated per operation

| Kernel | old-framework | new-framework | old-net10 | new-net10 | old-net11 | new-net11 |
| --- | --- | --- | --- | --- | --- | --- |
| Cart | 24 | 0 | 24 | 0 | 24 | 0 |
| Rules | 48 | 0 | 48 | 0 | 48 | 0 |
| Telemetry | 48 | 0 | 48 | 0 | 48 | 0 |
| Option | 48 | 24 | 0 | 0 | 0 | 0 |
| Nested | 48 | 0 | 48 | 0 | 48 | 0 |
| FilterMap | 146 | 146 | 146 | 146 | 146 | 146 |
| Escaping | 24 | 24 | 24 | 24 | 24 | 24 |
| NonCapturing | 0 | 0 | 0 | 0 | 0 | 0 |
