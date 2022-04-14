// #Regression #Conformance #TypesAndModules #Records 
// Regression test for FSHARP1.0:4666
// Records should be allowed to implement interfaces with member same name as field

type IFoo =    
    abstract Bar: int8                      // <- Bar is member in interface

type Foo =
    { Bar: int8 }    
    interface IFoo with        
        member this.Bar = this.Bar + 10y     // <- no problem here!

let o = { Bar = 1y }
if (o :> IFoo).Bar <> 11y then failwith "Failed"
