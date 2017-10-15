#r "../../../../../../Debug/net40/bin/serialiser_tp.dll"

open System
open FSharp.Reflection
open Serialiser

type MyRecord = { 
    Id : string 
    Value : int
}


let instance = { Id = "Foo"; Value = 1 }

<@@ instance.Id @@>

type MyRecordSerialiser = Serialiser.SourceType<MyRecord> 

printfn "%A" MyRecordSerialiser.Headers

printfn "%A" (MyRecordSerialiser.Serialise(instance))