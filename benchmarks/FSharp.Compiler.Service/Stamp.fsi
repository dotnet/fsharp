namespace FSharp.Compiler.Service

open System

[<Struct>]
type internal TimeStamp =

    member DateTime: DateTime

    static member Create: unit -> TimeStamp
