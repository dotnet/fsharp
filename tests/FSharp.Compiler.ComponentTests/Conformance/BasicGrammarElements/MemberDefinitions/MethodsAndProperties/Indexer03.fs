// #Regression #Conformance #DeclarationElements #MemberDefinitions #MethodsAndProperties 
// Regression test for FSHARP1.0:5062
// Make sure we don't ICE on this code!
//<Expects status="success"></Expects>
module TestModule

type TestType1 ( x : int , y : int ) =  
    let mutable x = x
    let mutable y = y
    
    [<DefaultValue>]
    // Static field
    static val mutable private instArray : int array
    
    do TestType1.instArray <- [|1 .. 10|]

    /// Static indexer with tupled getter and setter 
    static member Item with get (i : int * string) = TestType1.instArray.[0]
                       and  set (i : string) (j : int)  = ()

    /// Indexer with single index getter
    member this.Item with get (i : int) = match i with | 0 -> x | 1 -> y | _ -> failwith "Incorrect index"
