// #Conformance #DeclarationElements #LetBindings 
#light

type Foo() =
    let mutable m_x = 0
    member this.DoStuff x =
        let nestedDoStuff x = 
            let doubleNestedDoStuff x y = x * y
            let curriedDoStuff = doubleNestedDoStuff x
            (curriedDoStuff x)
        nestedDoStuff x
        
let rec recursiveFunctionA x =
    if x < 0 then 
        0
    else 
        recursiveFunctionB (x - 2)
and recursiveFunctionB x =
    // This doesn't do anything meaningful...
    let zero =
        let someArbitraryFunction (x,y:int) = 
            let listMaker (x:'t) : 't list = []
            (listMaker x) @ (listMaker y)
        List.length (someArbitraryFunction (1, 2))
    if x < zero then
        zero
    else
        recursiveFunctionA (x + 1)
