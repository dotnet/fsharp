// #Conformance #DataExpressions #ComputationExpressions 
// Verify the ability to define computation expression methods and
// that they get called as part of a custom workflow builder.

// Verify yield, yield!

open System.Collections.Generic

type WorkflowBuilder() =

    let yieldedItems = new List<string>()
    member this.Items = yieldedItems |> Array.ofSeq

    member this.Yield(item) = yieldedItems.Add(item)
    member this.YieldFrom(items : seq<string>) = 
        items |> Seq.iter (fun item -> yieldedItems.Add(item.ToUpper()))
        ()

    member this.Combine(f, g) = g

    member this.Delay (f : unit -> 'a) =
        f()

    member this.Zero() = ()
    member this.Return _ = this.Items

let workflow = new WorkflowBuilder()

let result =
    workflow {
        yield "foo"
        yield "bar"
        yield! [| "a"; "b"; "c" |]
        
        return ()
    }


if result <> [| "foo"; "bar"; "A"; "B"; "C" |] then exit 1

exit 0
