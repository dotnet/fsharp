namespace FSharp.Compiler.HotReload

open System
open FSharp.Compiler.AbstractIL.IL
open FSharp.Compiler.CodeGen
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
    }

/// <summary>Payload returned to tooling after a delta has been produced.</summary>
type internal DeltaEmissionResult =
    {
        Delta: IlxDelta
    }
