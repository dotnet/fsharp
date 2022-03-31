// #NoMT #CodeGen #Interop 
// Verify extension methods become intrinsic if in same namespace

namespace Test

type Foo() =
    member this.PropA = 1
    member this.MethA(a) = a + this.PropA

type Bar() =
    static member Value = 1

type Foo with
    // These look like extension methods, but since
    // they are in the same module/namespace as the
    // class they will simply be added as if they
    // were in the same type declaration
    member this.PropB = this.PropA + Bar.Value

    // From the spec, extension methods are defined at the same time
    // (like class members) so they can be mutually recursive.
    member this.MethB(a) = a + this.PropB + this.MethA(a)

// --------------------------------

module Tester =

    open CodeGenHelper
    open System

    printfn "Testing..."

    try

        System.Reflection.Assembly.GetExecutingAssembly()
        |> should containType "Test.Foo"

        System.Reflection.Assembly.GetExecutingAssembly()
        |> getType "Test.Foo"
        |> should containProp "PropA"

        System.Reflection.Assembly.GetExecutingAssembly()
        |> getType "Test.Foo"
        |> should containProp "PropB"

        System.Reflection.Assembly.GetExecutingAssembly()
        |> getType "Test.Foo"
        |> should containMember "MethA"

        System.Reflection.Assembly.GetExecutingAssembly()
        |> getType "Test.Foo"
        |> should containMember "MethB"

    with
    | e -> printfn "Unhandled Exception: %s" e.Message
           raise (Exception($"Oops: {e}"))
