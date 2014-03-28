// #Regression #Conformance #TypeInference #TypeConstraints 
// Regression test for FSharp1.0:4189
// Title: Type checking oddity

// This suggestion was resolved as by design,
// so the test makes sure, we're emitting error message about 'not being avalid object construction expression'

//<Expects status="error" span="(16,30-16,60)" id="FS0696">This is not a valid object construction expression\. Explicit object constructors must either call an alternate constructor or initialize all fields of the object and specify a call to a super class constructor\.$</Expects>

type ImmutableStack<'a> private(items: 'a list) = 
   
    member this.Push item = ImmutableStack(item::items)
    member this.Pop = match items with | [] -> failwith "No elements in stack" | x::xs -> x,ImmutableStack(xs)
    
    // Notice type annotation is commented out, which results in an error
    new(col (*: seq<'a>*)) = ImmutableStack(List.ofSeq col)
