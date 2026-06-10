module internal FSharp.Compiler.CompilerGeneratedNameMapState

open System.Runtime.CompilerServices
open FSharp.Compiler.GeneratedNames

// Keep optional name-map state external to CompilerGlobalState so core signatures can remain stable.
type private NameMapHolder() =
    let syncRoot = obj ()
    let mutable current: ICompilerGeneratedNameMap option = None

    member _.TryGet() = lock syncRoot (fun () -> current)
    member _.Set(value: ICompilerGeneratedNameMap option) = lock syncRoot (fun () -> current <- value)

let private holders = ConditionalWeakTable<obj, NameMapHolder>()

let private getHolder (owner: obj) =
    holders.GetValue(owner, fun _ -> NameMapHolder())

let tryGetCompilerGeneratedNameMap (owner: obj) =
    getHolder owner |> fun holder -> holder.TryGet()

let setCompilerGeneratedNameMap (owner: obj) (map: ICompilerGeneratedNameMap) =
    getHolder owner |> fun holder -> holder.Set(Some map)

let setCompilerGeneratedNameMapOpt (owner: obj) (map: ICompilerGeneratedNameMap option) =
    getHolder owner |> fun holder -> holder.Set(map)

let clearCompilerGeneratedNameMap (owner: obj) =
    getHolder owner |> fun holder -> holder.Set(None)
