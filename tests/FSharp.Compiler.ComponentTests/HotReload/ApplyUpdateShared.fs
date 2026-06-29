module FSharp.Compiler.ComponentTests.HotReload.ApplyUpdateShared

let baselineSourceText = """
using System;
using System.IO;
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using System.Runtime.CompilerServices;

[assembly: System.Diagnostics.Debuggable(System.Diagnostics.DebuggableAttribute.DebuggingModes.Default |
                                         System.Diagnostics.DebuggableAttribute.DebuggingModes.DisableOptimizations |
                                         System.Diagnostics.DebuggableAttribute.DebuggingModes.EnableEditAndContinue)]

namespace Sample
{
    public static class MethodDemo
    {
        public static string GetMessage() => "Hello baseline";
    }

    public static class ModuleInfo
    {
        static partial class Accessors
        {
            [UnsafeAccessor(UnsafeAccessorKind.Method, Name = "GetDebuggerInfoBits")]
            public static extern int CallGetDebuggerInfoBits(Module module);

            [UnsafeAccessor(UnsafeAccessorKind.Method, Name = "IsEditAndContinueCapable")]
            public static extern bool CallIsEnCCapable(Module module);

            [UnsafeAccessor(UnsafeAccessorKind.Method, Name = "IsEditAndContinueEnabled")]
            public static extern bool CallIsEncEnabled(Module module);
        }

        public static int? TryGetDebuggerInfoBits()
        {
            var mod = typeof(ModuleInfo).Assembly.ManifestModule;
            try { return Accessors.CallGetDebuggerInfoBits(mod); } catch { }
            var t = mod.GetType();
            var m = t.GetMethod("GetDebuggerInfoBits", BindingFlags.Instance | BindingFlags.NonPublic);
            if (m != null)
                return (int)m.Invoke(mod, null);
            var f = t.GetField("m_debuggerBits", BindingFlags.Instance | BindingFlags.NonPublic)
                 ?? t.GetField("m_debuggerInfoBits", BindingFlags.Instance | BindingFlags.NonPublic);
            if (f != null)
                return (int)f.GetValue(mod);
            return null;
        }

        public static bool? TryIsEditAndContinueCapable()
        {
            var mod = typeof(ModuleInfo).Assembly.ManifestModule;
            try { return Accessors.CallIsEnCCapable(mod); } catch { }
            var t = mod.GetType();
            var m = t.GetMethod("IsEditAndContinueCapable", BindingFlags.Instance | BindingFlags.NonPublic);
            return m != null ? (bool)m.Invoke(mod, null) : (bool?)null;
        }

        public static bool? TryIsEditAndContinueEnabled()
        {
            var mod = typeof(ModuleInfo).Assembly.ManifestModule;
            try { return Accessors.CallIsEncEnabled(mod); } catch { }
            var t = mod.GetType();
            var m = t.GetMethod("IsEditAndContinueEnabled", BindingFlags.Instance | BindingFlags.NonPublic);
            return m != null ? (bool)m.Invoke(mod, null) : (bool?)null;
        }

        public static (bool isSystem, bool isReflectionEmit, bool isReadyToRun)? TryPeFlags()
        {
            try
            {
                var path = typeof(ModuleInfo).Assembly.Location;
                using var fs = File.OpenRead(path);
                using var pe = new System.Reflection.PortableExecutable.PEReader(fs);
                var md = pe.GetMetadataReader();
                var asm = md.GetAssemblyDefinition();
                bool isSystem = string.Equals(md.GetString(asm.Name), "System.Private.CoreLib", StringComparison.Ordinal);
                bool isRefEmit = false;
                bool isR2R = pe.PEHeaders.CorHeader.Flags.HasFlag(System.Reflection.PortableExecutable.CorFlags.ILOnly) == false;
                return (isSystem, isRefEmit, isR2R);
            }
            catch { return null; }
        }
    }
}
"""

open System
open System.Reflection
open System.Reflection.Metadata
open FSharp.Compiler.ComponentTests.HotReload.TestHelpers
open FSharp.Compiler.IlxDeltaEmitter

// Shared artifacts for runtime ApplyUpdate tests so child/runner/console flows
// exercise identical baseline and delta construction logic.
type internal ApplyUpdateDeltaArtifacts =
    { BaselineArtifacts: BaselineArtifacts
      TypeName: string
      UpdatedMessage: string
      Delta: IlxDelta }

// Compile a baseline with real compiler settings and produce a single-generation
// method-body delta that all ApplyUpdate hosts can reuse.
let internal createApplyUpdateDeltaArtifacts (updatedMessage: string) : ApplyUpdateDeltaArtifacts =
    let baselineArtifacts = createBaselineFromRealCompiler baselineSourceText
    let typeName = "Sample.MethodDemo"
    let methodKey = methodKeyByName baselineArtifacts.Baseline typeName "GetMessage"
    let updatedModule = createMethodModule updatedMessage |> withDebuggableAttribute

    let request : IlxDeltaRequest =
        { Baseline = baselineArtifacts.Baseline
          UpdatedTypes = [ typeName ]
          UpdatedMethods = [ methodKey ]
          UpdatedAccessors = []
          Module = updatedModule
          SymbolChanges = None
          CurrentGeneration = 1
          PreviousGenerationId = None
          SynthesizedNames = None }

    let delta = emitDelta request

    { BaselineArtifacts = baselineArtifacts
      TypeName = typeName
      UpdatedMessage = updatedMessage
      Delta = delta }

// Probe runtime debugger/EnC flags using multiple reflection fallbacks so test logs stay
// actionable across runtime variations where individual private APIs may be absent.
let internal probeApplyUpdateAssembly (tracePrefix: string) (assemblyPath: string) (assembly: Assembly) =
    let moduleType = assembly.ManifestModule.GetType()

    moduleType.GetMethod("SetDebuggerInfoBits", BindingFlags.Instance ||| BindingFlags.NonPublic)
    |> Option.ofObj
    |> Option.iter (fun m ->
        let paramType = m.GetParameters().[0].ParameterType
        let bitsObj = System.Enum.ToObject(paramType, 0x0C)
        m.Invoke(assembly.ManifestModule, [| bitsObj |]) |> ignore
        printfn "[%s] SetDebuggerInfoBits invoked with 0x0C" tracePrefix)

    match DebuggerFlagProbe.tryComputeFlags assemblyPath with
    | Some flags -> printfn "[%s] Debugger flags (computed)=%A" tracePrefix flags
    | None -> printfn "[%s] Debugger flags (computed)=<unavailable>" tracePrefix

    let dbgBits =
        moduleType.GetMethod("GetDebuggerInfoBits", BindingFlags.Instance ||| BindingFlags.NonPublic)
        |> Option.ofObj
        |> Option.map (fun m -> m.Invoke(assembly.ManifestModule, [||]) :?> int)
        |> Option.orElseWith (fun () ->
            [ "m_debuggerInfoBits"; "m_debuggerBits" ]
            |> Seq.tryPick (fun name ->
                moduleType.GetField(name, BindingFlags.Instance ||| BindingFlags.NonPublic)
                |> Option.ofObj
                |> Option.map (fun f -> f.GetValue(assembly.ManifestModule) :?> int)))

    match dbgBits with
    | Some bits -> printfn "[%s] DebuggerInfoBits=0x%X" tracePrefix bits
    | None -> printfn "[%s] DebuggerInfoBits=<unavailable>" tracePrefix

    try
        let moduleInfo = assembly.GetType("Sample.ModuleInfo", throwOnError = true)
        let bitsFromHelper = moduleInfo.GetMethod("TryGetDebuggerInfoBits", BindingFlags.Public ||| BindingFlags.Static).Invoke(null, [||])
        let encCapableHelper = moduleInfo.GetMethod("TryIsEditAndContinueCapable", BindingFlags.Public ||| BindingFlags.Static).Invoke(null, [||])
        let encEnabledHelper = moduleInfo.GetMethod("TryIsEditAndContinueEnabled", BindingFlags.Public ||| BindingFlags.Static).Invoke(null, [||])
        let peFlags = moduleInfo.GetMethod("TryPeFlags", BindingFlags.Public ||| BindingFlags.Static).Invoke(null, [||])
        printfn "[%s] DebuggerInfoBits(ModuleInfo)=%A" tracePrefix bitsFromHelper
        printfn "[%s] ModuleInfo.TryIsEditAndContinueCapable=%A" tracePrefix encCapableHelper
        printfn "[%s] ModuleInfo.TryIsEditAndContinueEnabled=%A" tracePrefix encEnabledHelper
        printfn "[%s] ModuleInfo.TryPeFlags=%A" tracePrefix peFlags
    with ex ->
        printfn "[%s] ModuleInfo helpers unavailable: %s" tracePrefix (ex.ToString())

    let encMethod = moduleType.GetMethod("IsEditAndContinueCapable", BindingFlags.Instance ||| BindingFlags.NonPublic)
    let encCapable =
        match encMethod with
        | null ->
            printfn "[%s] IsEditAndContinueCapable not found on %s" tracePrefix moduleType.FullName
            false
        | m ->
            let result = m.Invoke(assembly.ManifestModule, [||]) :?> bool
            printfn "[%s] IsEditAndContinueCapable=%b" tracePrefix result
            result

    moduleType, encCapable

// MetadataUpdater.ApplyUpdate expects a span even when no PDB delta was emitted.
let internal pdbBytesOrEmpty (pdbOpt: byte[] option) =
    defaultArg pdbOpt Array.empty
