#r "../../../../../../Debug/net40/bin/serialiser_tp.dll"

open System
open FSharp.Reflection
open Serialiser

type MyRecord = { 
    Id : string 
    Value : int
}

let instance = { Id = "Foo"; Value = 1 }

type MyRecordSerialiser = Serialiser.SourceType<MyRecord> 

printfn "Headers: %A" MyRecordSerialiser.Headers

printfn "Instance as JSON %s" (MyRecordSerialiser.Serialise(instance))