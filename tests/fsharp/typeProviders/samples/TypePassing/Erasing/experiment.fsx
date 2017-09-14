#r "../../../../../../Debug/net40/bin/type_passing_tp.dll"

open System
open FSharp.Reflection
open Test

[<AttributeUsage(AttributeTargets.Class ||| AttributeTargets.Field)>]
type MyTpAttribute(name:string) = 
     inherit Attribute() 

     member x.Name with get() = name


type [<MyTpAttribute("Foo")>] AttributedRecord = 
    { Id : string }
    member x.TestInstanceProperty = 1
    member x.TestInstanceMethod(y:string) = y
    static member TestStaticMethod(y:string) = y
    static member TestStaticProperty = 2

module AttributedRecord = 

    let T = typeof<AttributedRecord>
    type S = TypePassing.Summarize<AttributedRecord>

    T.CustomAttributes |> Seq.iter (fun x -> printfn "%A" x.AttributeType.Name)
    S.CustomAttribute_Names |> Seq.iter (printfn "%A")

if failures.Length > 0 then 
    printfn "FAILURES: %A" failures
    exit 1
else   
    printfn "TEST PASSED (with some inaccuracies)"
    exit 0
