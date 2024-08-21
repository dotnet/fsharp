// #Regression #Conformance #DeclarationElements #MemberDefinitions #MethodsAndProperties 
#light

// Regression test for FSHARP1.0:4150 - Bug in List.partition (Was : Recursive functions declared in implicit class constructors taking exactly two parameters yield error "Invalid Value.")

open System

type Foo() = 
    let rec recFunc1 (x, y) = ()
    let rec recFunc2 (x, y, _) = (y, x) |> ignore
    override this.ToString() = "Stuff"
    
module Bar = 
    let rec recFunc1 (x, y) = ()
    let rec recFunc2 (x, y, _) = (y, x) |> ignore
    let rec recFunc3 (_) = (1) |> ignore
    let Func4 (y, x) = ()
    
let f = Foo()
let c = Bar.Func4(1,1)
