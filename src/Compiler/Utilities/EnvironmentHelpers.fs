module internal FSharp.Compiler.EnvironmentHelpers

open System

let isEnvVarTruthy (name: string) =
    match Environment.GetEnvironmentVariable(name) with
    | null
    | "" -> false
    | value when String.Equals(value, "1", StringComparison.OrdinalIgnoreCase) -> true
    | value when String.Equals(value, "true", StringComparison.OrdinalIgnoreCase) -> true
    | _ -> false

/// Tests switches whose contract accepts only the value "1".
let isEnvVarSetToOne (name: string) =
    String.Equals(Environment.GetEnvironmentVariable name, "1", StringComparison.Ordinal)

/// Shared names keep compiler configuration and diagnostic switches consistent.
[<RequireQualifiedAccess>]
module EnvironmentVariableNames =

    /// Environment variable for class-based state machine emission.
    [<Literal>]
    let HotReloadClassStateMachines = "FSHARP_HOTRELOAD_CLASS_STATEMACHINES"

    /// Environment variable for the synthesized-name debug-record override.
    [<Literal>]
    let HotReloadDisableSynthesizedNameSnapshotCdi =
        "FSHARP_HOTRELOAD_DISABLE_SYNTHESIZED_NAME_SNAPSHOT_CDI"

    /// Environment variable for the directory for diagnostic delta artifacts.
    [<Literal>]
    let HotReloadDumpDeltas = "FSHARP_HOTRELOAD_DUMP_DELTAS"

    /// Environment variable for runtime delta application.
    [<Literal>]
    let HotReloadEnableRuntimeApply = "FSHARP_HOTRELOAD_ENABLE_RUNTIME_APPLY"

    /// Environment variable for incremental optimizer emission.
    [<Literal>]
    let HotReloadIncrementalEmit = "FSHARP_HOTRELOAD_INCREMENTAL_EMIT"

    /// Environment variable for compilation inside the compiler service.
    [<Literal>]
    let HotReloadInProcessCompile = "FSHARP_HOTRELOAD_INPROCESS_COMPILE"

    /// Environment variable for closure-name diagnostics.
    [<Literal>]
    let HotReloadTraceClosureNames = "FSHARP_HOTRELOAD_TRACE_CLOSURENAMES"

    /// Environment variable for metadata heap diagnostics.
    [<Literal>]
    let HotReloadTraceHeaps = "FSHARP_HOTRELOAD_TRACE_HEAPS"

    /// Environment variable for metadata heap-offset diagnostics.
    [<Literal>]
    let HotReloadTraceHeapOffsets = "FSHARP_HOTRELOAD_TRACE_HEAP_OFFSETS"

    /// Environment variable for metadata diagnostics.
    [<Literal>]
    let HotReloadTraceMetadata = "FSHARP_HOTRELOAD_TRACE_METADATA"

    /// Environment variable for method diagnostics.
    [<Literal>]
    let HotReloadTraceMethods = "FSHARP_HOTRELOAD_TRACE_METHODS"

    /// Environment variable for compiler output diagnostics.
    [<Literal>]
    let HotReloadTraceOutput = "FSHARP_HOTRELOAD_TRACE_OUTPUT"

    /// Environment variable for portable PDB diagnostics.
    [<Literal>]
    let HotReloadTracePdb = "FSHARP_HOTRELOAD_TRACE_PDB"

    /// Environment variable for session transition diagnostics.
    [<Literal>]
    let HotReloadTraceSessions = "FSHARP_HOTRELOAD_TRACE_SESSIONS"

    /// Environment variable for user-string diagnostics.
    [<Literal>]
    let HotReloadTraceStrings = "FSHARP_HOTRELOAD_TRACE_STRINGS"

    /// Environment variable for synthesized type diagnostics.
    [<Literal>]
    let HotReloadTraceSynthesized = "FSHARP_HOTRELOAD_TRACE_SYNTHESIZED"

    /// Environment variable for compiler timing diagnostics.
    [<Literal>]
    let HotReloadTraceTiming = "FSHARP_HOTRELOAD_TRACE_TIMING"
