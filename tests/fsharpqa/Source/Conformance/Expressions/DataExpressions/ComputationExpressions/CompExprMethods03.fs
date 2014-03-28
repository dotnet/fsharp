// #Conformance #DataExpressions #ComputationExpressions 
// Verify the ability to define computation expression methods and
// that they get called as part of a custom workflow builder.

// Verify TryFinally and TryWith

open System.Collections.Generic

type NoExnWorkflow() =

    member this.TryFinally<'a> (body : unit -> 'a, finallyBlock : unit -> unit) =
        // Don't execute finally block - this is how we verify it executes
        // _our_ try/finally. 
        try
            body()
        with
            e -> Unchecked.defaultof<'a>
        
    member this.TryWith<'a> (body : unit -> 'a, catchBlock : exn -> 'a) =
    
        // Don't execute with block - this is how we verify it executes
        // _our_ try/with
        try
            body()
        with
            e -> Unchecked.defaultof<'a>
        
    member this.Delay<'a> (f : unit -> 'a) = f
    member this.Combine(firstPart, secondPart) = secondPart
    member this.Zero () = ()
    member this.Return x = x

let noExnWorkflow = new NoExnWorkflow()

//////////////////////////////////////////////////

let testFinally =
    noExnWorkflow {
        try
            return 5
        finally
            failwith "TEST FAILURE: Finally block shouldn't be executed!"
    }
    
if testFinally() <> 5 then exit 1

//////////////////////////////////////////////////

let testWith =
    noExnWorkflow {
        try
            failwith "Starting the try/with test..."
        with
            e -> failwith "TEST FAILURE: With block shouldn't be executed!"
    }

if testWith() <> () then exit 1

//////////////////////////////////////////////////

exit 0
