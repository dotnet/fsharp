namespace FSharp.Compiler.HotReload

open System
open FSharp.Compiler.AbstractIL.IL
open FSharp.Compiler.CodeGen
open FSharp.Compiler.EncMethodDebugInformation
open FSharp.Compiler.HotReloadBaseline
open FSharp.Compiler.HotReload.SymbolChanges
open FSharp.Compiler.IlxDeltaEmitter

/// <summary>Errors surfaced when emitting hot reload deltas.</summary>
type internal HotReloadError =
    | NoActiveSession
    | NoChanges
    | UnsupportedEdit of string
    | DeltaEmissionException of exn

/// <summary>Input describing the members that changed during the current hot reload cycle.</summary>
type internal DeltaEmissionRequest =
    {
        IlModule: ILModuleDef
        UpdatedTypes: string list
        UpdatedMethods: MethodDefinitionKey list
        UpdatedAccessors: AccessorUpdate list
        SymbolChanges: FSharpSymbolChanges option
        /// <summary>
        /// Per-method EnC debug information recomputed from the fresh typed tree of the edited
        /// compilation, keyed by baseline MethodDef token (see
        /// HotReloadBaseline.computeRefreshedEncMethodDebugInfos). Entries of methods updated by
        /// the delta replace their counterparts in the chained baseline; updated methods without
        /// an entry have theirs dropped (fail closed). Callers without typed-tree access pass the
        /// empty map.
        /// </summary>
        RefreshedEncDebugInfos: Map<int, EncMethodDebugInformation>
        /// <summary>
        /// Per-method closure-name tables (occurrence-chain -> closure class name)
        /// recomputed by the occurrence-keyed allocator from the fresh typed tree, keyed
        /// by baseline MethodDef token (see
        /// HotReloadBaseline.computeOccurrenceKeyedClosureNames). Chained into the
        /// next-generation baseline with the same replace-or-drop semantics as
        /// RefreshedEncDebugInfos. Callers without typed-tree access pass the empty map.
        /// </summary>
        RefreshedClosureNameRows: Map<int, Map<int list, string>>
    }

/// <summary>Payload returned to tooling after a delta has been produced.</summary>
type internal DeltaEmissionResult =
    {
        Delta: IlxDelta
    }
