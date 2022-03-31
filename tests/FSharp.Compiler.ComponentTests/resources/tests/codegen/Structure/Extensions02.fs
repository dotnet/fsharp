// #NoMT #CodeGen #Interop 
// Verify extension methods do NOT become intrinsic if in different modules

module Test

module Parts =
    type Foo() =
        member this.PropA = 1
        member this.MethA(a) = a + this.PropA

module Extensions = 

    type Parts.Foo with
        member this.PropB = this.PropA + 1
        member this.MethB(a) = a + this.PropB + this.MethA(a)

// --------------------------------

module Tester =

    open System
    open CodeGenHelper

    printfn "Testing..."

    try
        //System.Reflection.Assembly.GetExecutingAssembly().GetTypes() 
        //|> Array.iter(fun ty -> printfn "%s" ty.FullName)

        // Since they are nested modules, the name is somewhat mangled. As of Beta1 there
        // is no way to add nested namespaces in F#.
        System.Reflection.Assembly.GetExecutingAssembly()
        |> should containType "Test+Parts+Foo"

        System.Reflection.Assembly.GetExecutingAssembly()
        |> getType "Test+Parts+Foo"
        |> should containMember "MethA"

        System.Reflection.Assembly.GetExecutingAssembly()
        |> getType "Test+Parts+Foo"
        |> should notContainMember "MethB"

        System.Reflection.Assembly.GetExecutingAssembly()
        |> getType "Test+Parts+Foo"
        |> should containProp "PropA"

        System.Reflection.Assembly.GetExecutingAssembly()
        |> getType "Test+Parts+Foo"
        |> should notContainProp "PropB"
       
    with
    | e -> printfn "Unhandled Exception: %s" e.Message
           exit 1

    exit 0
