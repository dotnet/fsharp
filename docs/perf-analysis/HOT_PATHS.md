# Hot Paths Analysis - Issue #19132

## Overview

This document contains hot path analysis from performance traces collected during large F# project builds.

Related Issue: https://github.com/dotnet/fsharp/issues/19132

## Trace Collection Methodology

Traces are collected using:
```bash
dotnet-trace collect --output <trace-file>.nettrace -- dotnet build -c <config> -p:ParallelCompilation=<value>
```

## Trace Analysis

### 5000 Module Build (Release, ParallelCompilation=true)

**Status**: Trace collection pending

When trace is collected, this section will contain:
- Top functions by inclusive time
- Top functions by exclusive time
- Call tree analysis from actual trace data
