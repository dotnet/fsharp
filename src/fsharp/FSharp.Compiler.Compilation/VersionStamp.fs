namespace FSharp.Compiler.Compilation

open System

[<Struct>]
type VersionStamp (dateTime: DateTime) =

    member __.DateTime = dateTime

    member __.NewVersionStamp () = VersionStamp DateTime.UtcNow

    static member Create () = VersionStamp DateTime.UtcNow

