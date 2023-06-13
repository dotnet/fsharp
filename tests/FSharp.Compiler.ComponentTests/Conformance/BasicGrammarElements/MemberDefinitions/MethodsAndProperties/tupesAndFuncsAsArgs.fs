// #Conformance #DeclarationElements #MemberDefinitions #MethodsAndProperties 
#light

type Foo() =
    static member ExecuteFunction (func : (int * string) -> bool) (args : (int * string)) =
            func args
    
    // Type infered tuples
    member this.AddTupes (a : (int*int*int)) (b : (int*int*int*int)) c =
        let a1, a2, a3 = a
        let b1, b2, b3, b4 = b
        let c1, c2 = c
        (a1 + b1 + System.Int32.Parse(c1), a2 + b2 + System.Int32.Parse(c2), a3 + b3 + 0, 0 + b4 + 0)

let result = Foo.ExecuteFunction (fun (x,y) -> x = System.Int32.Parse(y)) (123, "123")
if result <> true then failwith "Failed: 1"

let foo = new Foo()
let a,b,c,d = foo.AddTupes ((1,2,3)) (1,2,3,4) ("1", "2")
if a <> 3 || b <> 6 || c <> 6 || d <> 4 then failwith "Failed: 2"
