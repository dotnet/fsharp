namespace FSharp.Compiler.Service

open System

/// We could use Roslyn's version stamp, but we need access to DateTime in order for IProjectReference TryGetLogicalTimeStamp to function correctly.
[<Struct>]
type internal VersionStamp =

    member DateTime: DateTime

    member NewVersionStamp: unit -> VersionStamp

    static member Create: unit -> VersionStamp
