// #Regression #Conformance #DeclarationElements #Attributes 
// Regression test for FSharp1.0:5172
// Title: "method" attribute target is not recognized
// Descr: Verify that attribute target 'method' is recognized by F# compiler correctly.

open System
open System.Reflection

type Foo() = inherit Attribute()

type C() = 
    class
        [<method: Foo>]
        let bar z = z * 1
    
        [<method: Foo>]
        member this.f x = x + 1
        
        [<method: Foo>]
        static member g x = x - 1
    end
    
[<method: Foo>]
let foo x = x * x

[<method: Foo>]
let goo = fun x -> 100

type Interface1 = 
    [<method: Foo>]
    abstract Foo : int -> int


// test that count of methods with 'Foo' attribute in the assembly is exactly 6
let test1 = 
    System.Reflection.Assembly.GetExecutingAssembly().GetTypes()
    |> Seq.map ( fun tp ->
                    tp.GetMethods(
                        BindingFlags.Instance  |||
                        BindingFlags.Static    |||
                        BindingFlags.Public    |||
                        BindingFlags.NonPublic |||
                        BindingFlags.DeclaredOnly )
               )
    |> Seq.concat
    |> Seq.filter ( fun m -> m.IsDefined(typeof<Foo>, false))
    |> Seq.length
    |> (=) 6
    
if not test1 then failwith "Failed: 1"
