// #Regression #Conformance #ObjectOrientedTypes #Structs 
// Regression test for FSHARP1.0:4666
// Structs should be allowed to implement interfaces with methods same name as field

type IFoo =    
    abstract Bar: int8                      // <- Bar is member in interface
 
[<Struct>]        
type Foo (value: int8) =
    member this.Bar = value
    interface IFoo with        
        member this.Bar = this.Bar + 10y     // <- no problem here!

let o = Foo(1y)
(if (o :> IFoo).Bar = 11y then 0 else 1) |> exit
