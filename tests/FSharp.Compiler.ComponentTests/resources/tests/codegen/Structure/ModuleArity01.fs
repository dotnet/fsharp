// #Regression #NoMT #CodeGen #Interop 

// Verify the generation of module value arity
// Regression test for FSHARP1.0:4762
namespace Test

module AModuleType =

    // Zero parameters (unit gets dropped)
    let Params0 () = ()

    // One parameter (of type unit)
    let Params1 (u : unit) = ()

    // One Parameter (of tuple type)
    let Params1a (tupe : int * float * string) = ()

    // One Parameter (takes one param, returns a fast func: after fix for #3665, the IL is just the uncurried form!)
    let Params1b (x : int) y = x + y

    // One Parameter (takes one param, returns a fast func: after fix for #3665, the IL is just the uncurried form!)
    let Params1c (x : float) y z = x + y + z

    // Two parameters
    let Params2 (x : decimal, y : decimal) = x + y

    // Three parameters
    let Params3 (x : float, y : int, z : char) = true

// --------------------------------


module Tester =

    open CodeGenHelper

    printfn "Testing..."

    try

        System.Reflection.Assembly.GetExecutingAssembly()
        |> getType "Test.AModuleType"
        |> getMethod "Params0"
        |> should takeParams []

        System.Reflection.Assembly.GetExecutingAssembly()
        |> getType "Test.AModuleType"
        |> getMethod "Params1"
        |> should takeParams [ typeof<unit> ]

        System.Reflection.Assembly.GetExecutingAssembly()
        |> getType "Test.AModuleType"
        |> getMethod "Params1a"
        |> should takeParams [ typeof<int>; typeof<float>; typeof<string> ]      // See FSHARP1.0:4762 (it is a bit unusual, but according to the spec

        System.Reflection.Assembly.GetExecutingAssembly()
        |> getType "Test.AModuleType"
        |> getMethod "Params1b"
        |> should takeParams [ typeof<int>; typeof<int> ]
        
        System.Reflection.Assembly.GetExecutingAssembly()
        |> getType "Test.AModuleType"
        |> getMethod "Params1c"
        |> should takeParams [ typeof<float>; typeof<float>; typeof<float>]

        System.Reflection.Assembly.GetExecutingAssembly()
        |> getType "Test.AModuleType"
        |> getMethod "Params2"
        |> should takeParams [ typeof<decimal>; typeof<decimal> ]

        System.Reflection.Assembly.GetExecutingAssembly()
        |> getType "Test.AModuleType"
        |> getMethod "Params3"
        |> should takeParams [ typeof<float>; typeof<int>; typeof<char> ]


    with
    | e -> printfn "Unhandled Exception: %s" e.Message
           exit 1

    exit 0
