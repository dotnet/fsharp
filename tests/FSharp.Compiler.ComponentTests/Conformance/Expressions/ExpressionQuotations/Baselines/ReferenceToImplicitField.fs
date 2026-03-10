// #Conformance #Quotations #Regression
// Bug 6423:Implicit field accesses in implicit method definitions are quoted incorrectly 
open Microsoft.FSharp.Quotations
open Microsoft.FSharp.Quotations.Patterns
type Foo() =
   let source = [1;2;3]
   [<ReflectedDefinition>]
   let foo() = source
   let bar() =
            let b = <@ source @>
            b
   member __.Bar = bar()
   [<ReflectedDefinition>]
   member x.Z() = source


let foo = Foo()
let success = 
    match foo.Bar with
    |   FieldGet(Some (Value (v,t)), _) -> 
                printfn "%A" v
                obj.ReferenceEquals(v, foo) && t = typeof<Foo>
    |   _ -> false

exit (if success then 0 else 1)