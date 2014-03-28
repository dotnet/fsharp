// #Regression #Conformance #ObjectOrientedTypes #Classes #MethodsAndProperties #MemberDefinitions 
// Regression test for FSHARP1.0:4666
// Classes should be allowed to implement interfaces with methods same name as field

type IFoo =    
    abstract Bar: int8              // <- Bar is method in interface
        
type Foo (value: int8) =                    // For a class
    member this.Bar = value    
    interface IFoo with
        member this.Bar = this.Bar + 10y     // <- no problem here!

let o = Foo(1y)
(if (o :> IFoo).Bar = 11y then 0 else 1) |> exit
