namespace FSharp.Compiler.Service

open System

[<Struct>]
type TimeStamp (dateTime: DateTime) =

    member __.DateTime = dateTime

    static member Create () = TimeStamp DateTime.UtcNow

