// #Conformance #DataExpressions #ComputationExpressions 
// Verify the ability to define computation expression methods and
// that they get called as part of a custom workflow builder.

// Verify For

open System.Collections.Generic

type WorkflowBuilder() =

    let m_loopedElements = new List<string>()
    let m_combineCalledWith = new List<string>()

    member this.LoopedElements    = Seq.toArray m_loopedElements
    member this.CombineCalledWith = Seq.toArray m_combineCalledWith

    member this.For(elements : seq<'item>, loopBody : 'item -> unit) =
        
        elements
        |> Seq.iter (fun item -> printfn "%d: %A" m_loopedElements.Count item
                                 m_loopedElements.Add(sprintf "%A" item)
                                 let _ = loopBody item
                                 ())
                                 
    member this.Combine(firstPart : unit, secondPart : 'a) =
       m_combineCalledWith.Add(sprintf "%A" secondPart)
       secondPart
        
    member this.Delay (f : unit -> 'a) =
        f()
        
    member this.Zero() = ()

let workflow = new WorkflowBuilder()

type Primitive = Int of int | Char of char

let result =
    workflow {
        for i in 1 .. 2 do
            ()
            for (:? string as s) in [ box "world"; box "universe" ] do
                ()
            for Int(i) in [| Int(1); Int(2) |] do
                ()
                for Char(c) in [| Char('a'); Char('b') |] do
                    ()
        }

if result <> () then printfn "failed"; exit 1

if workflow.CombineCalledWith <> [| "()"; "()" |] then printfn "failed"; exit 1

if workflow.LoopedElements <> 
    [|
        "1"; 
            "\"world\""; "\"universe\""; 
            "Int 1"; 
                "Char 'a'"; "Char 'b'"; 
            "Int 2";
                "Char 'a'"; "Char 'b'"; 
        "2"; 
            "\"world\""; "\"universe\""; 
                "Int 1"; 
                    "Char 'a'"; "Char 'b'"; 
                "Int 2"; 
                    "Char 'a'"; "Char 'b'" |] then printfn "failed"; exit 1

printfn "Succeeded"
exit 0