namespace FSharp.Compiler.Service

open System

[<Struct>]
type internal VersionStamp =

    member DateTime: DateTime

    member NewVersionStamp: unit -> VersionStamp

    static member Create: unit -> VersionStamp
