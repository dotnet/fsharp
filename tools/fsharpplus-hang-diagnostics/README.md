# FSharpPlus Build Hang Diagnostic Pipeline

This diagnostic pipeline investigates why `dotnet test build.proj -v n` hangs when building [FSharpPlus PR #614](https://github.com/fsprojects/FSharpPlus/pull/614) with .NET SDK 10.0.100.

**Related Issue:** https://github.com/dotnet/fsharp/issues/19116

## Quick Start

```bash
cd tools/fsharpplus-hang-diagnostics
chmod +x *.sh
./run-all.sh
```

## Prerequisites

- .NET SDK 10.0.100 or later
- Git
- Linux/macOS (for timeout command)

The scripts will automatically install:
- `dotnet-trace` - for collecting ETW traces
- `dotnet-dump` - for capturing memory dumps

## Scripts

| Script | Description |
|--------|-------------|
| `run-all.sh` | Master script that runs the complete pipeline |
| `collect-diagnostics.sh` | Clones FSharpPlus, runs the build with tracing, and captures diagnostics |
| `analyze-trace.fsx` | F# script that analyzes `.nettrace` files for hang patterns |
| `analyze-dump.fsx` | F# script that analyzes `.dmp` files for thread states |
| `combined-analysis.fsx` | F# script that combines trace and dump analysis |
| `generate-diagnostic-run.fsx` | F# script that generates run metadata report |

## Output Files

All output is written to the `output/` directory:

| File | Description |
|------|-------------|
| `hang-trace.nettrace` | ETW trace captured during the build |
| `hang-dump-*.dmp` | Memory dumps (if captured) |
| `DIAGNOSTIC-RUN.md` | Run metadata and configuration |
| `trace-analysis.md` | Detailed trace analysis with event timeline |
| `dump-analysis.md` | Thread and lock analysis from memory dumps |
| `FINAL-REPORT.md` | Combined report with root cause analysis |
| `console-output.txt` | Full console output from the build |
| `run-metadata.json` | Machine-readable run metadata |

## What the Pipeline Does

1. **Collection Phase** (`collect-diagnostics.sh`)
   - Installs diagnostic tools (`dotnet-trace`, `dotnet-dump`)
   - Clones FSharpPlus repository (branch `gus/fsharp9`)
   - Runs `dotnet test build.proj -v n` with ETW tracing
   - Uses a 120-second timeout (the build is expected to hang)
   - Attempts to capture memory dumps of hanging processes

2. **Trace Analysis** (`analyze-trace.fsx`)
   - Opens the `.nettrace` file using `Microsoft.Diagnostics.Tracing.TraceEvent`
   - Identifies time gaps in event stream (indicating hangs)
   - Tracks JIT activity, GC events, and lock contention
   - Identifies F# compiler and MSBuild related events
   - Generates `trace-analysis.md`

3. **Dump Analysis** (`analyze-dump.fsx`)
   - Opens `.dmp` files using `Microsoft.Diagnostics.Runtime` (ClrMD)
   - Enumerates all threads and their stack traces
   - Identifies common hang points (multiple threads at same location)
   - Analyzes F# compiler and MSBuild thread activity
   - Tracks lock ownership and waiters
   - Generates `dump-analysis.md`

4. **Combined Analysis** (`combined-analysis.fsx`)
   - Reads both trace and dump analysis reports
   - Correlates findings from both sources
   - Provides root cause hypothesis
   - Generates `FINAL-REPORT.md` with recommendations

## Expected Results

When the hang reproduces:

- **Exit Code 124**: Timeout (hang confirmed)
- **Trace Analysis**: Shows time gaps and last active methods
- **Dump Analysis**: Shows thread states at hang point

When the hang does NOT reproduce:

- **Exit Code 0**: Success (no hang)
- The reports will note that the issue was not reproduced

## Manual Running

To run individual components:

```bash
# Just collect diagnostics
./collect-diagnostics.sh

# Just analyze existing trace
dotnet fsi analyze-trace.fsx

# Just analyze existing dumps
dotnet fsi analyze-dump.fsx

# Just generate final report
dotnet fsi combined-analysis.fsx
```

## Troubleshooting

### "dotnet-trace not found"

Install manually:
```bash
dotnet tool install --global dotnet-trace
export PATH="$PATH:$HOME/.dotnet/tools"
```

### "Trace file not found"

The trace may not have been generated if:
- `dotnet-trace` was killed before flushing
- Insufficient disk space
- Permission issues

Try running with a longer timeout or manual trace collection.

### "No dump files captured"

This is expected! The timeout mechanism kills the process before we can manually capture a dump. For dump analysis:

1. Start the build manually: `dotnet test build.proj -v n`
2. Wait for it to hang
3. In another terminal: `dotnet-dump collect -p $(pgrep dotnet | head -1)`
4. Kill the build: `Ctrl+C`
5. Run the analysis scripts

## NuGet Packages Used

- `Microsoft.Diagnostics.Tracing.TraceEvent` (3.1.8+) - For trace analysis
- `Microsoft.Diagnostics.Runtime` (3.1.512+) - For dump analysis

## Contributing

This diagnostic pipeline is part of the investigation for F# issue #19116. If you have suggestions for improving the analysis, please open an issue or PR.
