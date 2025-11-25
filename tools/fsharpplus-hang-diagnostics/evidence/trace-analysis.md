# Trace Analysis Report

**Generated:** 2025-11-25 13:11:30 UTC
**Trace File:** hang-trace.nettrace
**Total Events:** 345946

## Executive Summary

### ⚠️ Hang Detected

Found **15 significant time gap(s)** (> 1 second) in event stream.

**Largest gap:** 36.13 seconds
- Gap started at: 13:10:02.458
- Gap ended at: 13:10:38.591

**Last event before gap:**
- Provider: `Microsoft-Windows-DotNETRuntime`
- Event: `EventID(30)`
- Thread: 0
- Payload: ``

**First event after gap:**
- Provider: `Microsoft-Windows-DotNETRuntime`
- Event: `EventID(30)`
- Thread: 30128
- Payload: ``

## Timeline Analysis

**First event:** 13:08:38.790
**Last event:** 13:10:38.785
**Duration:** 120.00 seconds
**Events per second:** 2883.00

### Event Density Over Time

| Time Range | Event Count | Events/sec |
|------------|-------------|------------|
| 13:08:38 - 13:08:48 | 323140 | 32314.0 |
| 13:08:48 - 13:08:58 | 280 | 28.0 |
| 13:08:58 - 13:09:08 | 35 | 3.5 |
| 13:09:08 - 13:09:18 | 175 | 17.5 |
| 13:09:18 - 13:09:28 | 0 | 0.0 |
| 13:09:28 - 13:09:38 | 12 | 1.2 |
| 13:09:38 - 13:09:48 | 229 | 22.9 |
| 13:09:48 - 13:09:58 | 0 | 0.0 |
| 13:09:58 - 13:10:08 | 2 | 0.2 |
| 13:10:08 - 13:10:18 | 0 | 0.0 |
| 13:10:18 - 13:10:28 | 0 | 0.0 |
| 13:10:28 - 13:10:38 | 22073 | 2207.3 |

### All Significant Time Gaps (> 1 second)

| # | Start | End | Duration (s) | Last Event Before |
|---|-------|-----|--------------|-------------------|
| 1 | 13:08:43.774 | 13:08:44.955 | 1.18 | Microsoft-Windows-DotNETRuntime/EventID(284) |
| 2 | 13:08:45.955 | 13:08:49.156 | 3.20 | Microsoft-Windows-DotNETRuntime/EventID(301) |
| 3 | 13:08:49.156 | 13:08:54.701 | 5.55 | Microsoft-Windows-DotNETRuntime/EventID(30) |
| 4 | 13:08:55.703 | 13:08:59.703 | 4.00 | Microsoft-Windows-DotNETRuntime/EventID(284) |
| 5 | 13:09:00.513 | 13:09:02.146 | 1.63 | Microsoft-Windows-DotNETRuntime/EventID(30) |
| 6 | 13:09:02.467 | 13:09:06.347 | 3.88 | Microsoft-Windows-DotNETRuntime/EventID(30) |
| 7 | 13:09:06.347 | 13:09:09.704 | 3.36 | Microsoft-Windows-DotNETRuntime/EventID(31) |
| 8 | 13:09:10.705 | 13:09:13.907 | 3.20 | Microsoft-Windows-DotNETRuntime/EventID(301) |
| 9 | 13:09:13.907 | 13:09:29.706 | 15.80 | Microsoft-Windows-DotNETRuntime/EventID(31) |
| 10 | 13:09:29.807 | 13:09:33.808 | 4.00 | Microsoft-Windows-DotNETRuntime/EventID(282) |
| 11 | 13:09:33.808 | 13:09:40.341 | 6.53 | Microsoft-Windows-DotNETRuntime/EventID(31) |
| 12 | 13:09:41.342 | 13:09:42.454 | 1.11 | Microsoft-Windows-DotNETRuntime/EventID(301) |
| 13 | 13:09:43.455 | 13:09:46.459 | 3.00 | Microsoft-Windows-DotNETRuntime/EventID(301) |
| 14 | 13:09:46.459 | 13:10:02.458 | 16.00 | Microsoft-Windows-DotNETRuntime/EventID(31) |
| 15 | 13:10:02.458 | 13:10:38.591 | 36.13 | Microsoft-Windows-DotNETRuntime/EventID(30) |

## Hot Methods (Most JIT'd)

| Method | JIT Count |
|--------|-----------|
| `System.Runtime.CompilerServices.AsyncMethodBuilderCore.Start` | 129 |
| `System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1[System.__Canon].AwaitUnsafeOnCompleted` | 92 |
| `System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1[System.__Canon].Start` | 60 |
| `System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1[System.__Canon].GetStateMachineBox` | 41 |
| `System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1[System.Boolean].AwaitUnsafeOnCompleted` | 24 |
| `System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1[System.Threading.Tasks.VoidTaskResult].AwaitUnsafeOnCompleted` | 23 |
| `System.Linq.Enumerable.Select` | 20 |
| `System.Runtime.CompilerServices.AsyncTaskMethodBuilder.Start` | 18 |
| `System.Runtime.CompilerServices.AsyncValueTaskMethodBuilder`1[System.__Canon].Start` | 16 |
| `System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1[System.Threading.Tasks.VoidTaskResult].GetStateMachineBox` | 15 |
| `System.Buffers.IndexOfAnyAsciiSearcher.IndexOfAnyCore` | 14 |
| `System.Runtime.CompilerServices.AsyncTaskMethodBuilder.AwaitUnsafeOnCompleted` | 13 |
| `<PrivateImplementationDetails>.InlineArrayAsReadOnlySpan` | 12 |
| `System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1[System.Boolean].Start` | 12 |
| `System.HashCode.Add` | 11 |
| `System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1[System.ValueTuple`2[System.__Canon,System.__Canon]].AwaitUnsafeOnCompleted` | 11 |
| `System.Runtime.CompilerServices.AsyncValueTaskMethodBuilder`1[System.__Canon].AwaitUnsafeOnCompleted` | 10 |
| `System.Buffers.IndexOfAnyAsciiSearcher.IndexOfAny` | 9 |
| `System.Runtime.InteropServices.MemoryMarshal.GetArrayDataReference` | 9 |
| `System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1[System.Boolean].GetStateMachineBox` | 9 |

## Provider Statistics

| Provider | Event Count |
|----------|-------------|
| `Microsoft-Windows-DotNETRuntime` | 324568 |
| `Microsoft-Windows-DotNETRuntimeRundown` | 21377 |
| `Microsoft-DotNETCore-EventPipe` | 1 |

## Thread Activity

| Thread ID | Event Count | First Event | Last Event | Active Duration (s) |
|-----------|-------------|-------------|------------|---------------------|
| 30114 | 68738 | 13:08:38.790 | 13:08:39.742 | 0.95 |
| 30136 | 42805 | 13:08:39.462 | 13:09:02.467 | 23.01 |
| 30130 | 38363 | 13:08:39.201 | 13:09:02.467 | 23.27 |
| 30126 | 37900 | 13:08:38.806 | 13:08:45.156 | 6.35 |
| 30147 | 30068 | 13:08:40.417 | 13:09:29.707 | 49.29 |
| 30137 | 29077 | 13:08:39.466 | 13:08:44.957 | 5.49 |
| 30140 | 26655 | 13:08:39.738 | 13:09:02.147 | 22.41 |
| 30134 | 26068 | 13:08:39.445 | 13:09:00.513 | 21.07 |
| 30116 | 21381 | 13:10:38.595 | 13:10:38.785 | 0.19 |
| 30148 | 20537 | 13:08:40.460 | 13:09:02.146 | 21.69 |
| 30133 | 1931 | 13:08:39.397 | 13:08:44.957 | 5.56 |
| 30242 | 522 | 13:10:38.592 | 13:10:38.595 | 0.00 |
| 30135 | 496 | 13:08:39.456 | 13:08:39.743 | 0.29 |
| 30225 | 187 | 13:09:40.342 | 13:09:46.459 | 6.12 |
| 30131 | 152 | 13:08:39.201 | 13:10:38.592 | 119.39 |
| 30139 | 126 | 13:08:39.724 | 13:10:38.591 | 118.87 |
| 30128 | 80 | 13:10:38.591 | 13:10:38.593 | 0.00 |
| 30132 | 78 | 13:08:39.291 | 13:08:39.292 | 0.00 |
| 30216 | 59 | 13:09:09.705 | 13:09:13.907 | 4.20 |
| 30151 | 48 | 13:08:54.702 | 13:08:59.703 | 5.00 |

## F# Compiler Activity

*No F# compiler specific events recorded*

## MSBuild Activity

*No MSBuild specific events recorded*

## Lock Contention Events

**Total contention events:** 465

### Contention Events (last 20)

| Time | Thread | Provider |
|------|--------|----------|
| 13:08:41.882 | 30147 | `Microsoft-Windows-DotNETRuntime` |
| 13:08:41.872 | 30140 | `Microsoft-Windows-DotNETRuntime` |
| 13:08:41.862 | 30148 | `Microsoft-Windows-DotNETRuntime` |
| 13:08:41.711 | 30140 | `Microsoft-Windows-DotNETRuntime` |
| 13:08:41.711 | 30148 | `Microsoft-Windows-DotNETRuntime` |
| 13:08:41.711 | 30136 | `Microsoft-Windows-DotNETRuntime` |
| 13:08:41.707 | 30140 | `Microsoft-Windows-DotNETRuntime` |
| 13:08:41.706 | 30140 | `Microsoft-Windows-DotNETRuntime` |
| 13:08:41.704 | 30136 | `Microsoft-Windows-DotNETRuntime` |
| 13:08:41.703 | 30140 | `Microsoft-Windows-DotNETRuntime` |
| 13:08:41.702 | 30140 | `Microsoft-Windows-DotNETRuntime` |
| 13:08:41.700 | 30136 | `Microsoft-Windows-DotNETRuntime` |
| 13:08:41.699 | 30136 | `Microsoft-Windows-DotNETRuntime` |
| 13:08:41.699 | 30136 | `Microsoft-Windows-DotNETRuntime` |
| 13:08:41.697 | 30140 | `Microsoft-Windows-DotNETRuntime` |
| 13:08:41.697 | 30140 | `Microsoft-Windows-DotNETRuntime` |
| 13:08:41.697 | 30140 | `Microsoft-Windows-DotNETRuntime` |
| 13:08:41.696 | 30140 | `Microsoft-Windows-DotNETRuntime` |
| 13:08:41.683 | 30148 | `Microsoft-Windows-DotNETRuntime` |
| 13:08:41.683 | 30136 | `Microsoft-Windows-DotNETRuntime` |

## GC Activity

*No GC events recorded*

## Recommendations

Based on the trace analysis:

1. **Investigate the hang point:** The trace shows significant time gaps indicating where the process became unresponsive.
2. **Check the last active method:** Review the method(s) active before the hang occurred.
3. **Analyze dump file:** If a memory dump was captured, use `analyze-dump.fsx` for detailed thread and lock analysis.
4. **Look for deadlocks:** Multiple threads waiting on locks may indicate a deadlock condition.
5. **Check F# compiler activity:** Review F# compiler events near the hang point for type checking or optimization issues.

---
*Report generated by analyze-trace.fsx*
