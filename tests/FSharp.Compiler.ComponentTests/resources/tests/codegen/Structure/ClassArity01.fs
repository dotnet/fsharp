// #NoMT #CodeGen #Interop 
#light

// Verify the generation of class function arity

namespace Test

type AClassType() =

    // Zero parameters (unit gets dropped)
    member this.Params0 () = ()

    // One parameter (of type unit)
    member this.Params1 (u : unit) = ()

    // One Parameter (of tuple type)
    member this.Params1a (tupe : int * float * string) = ()

    // One Parameter (takes one param, returns a fast func: after fix for #3665, the IL is just the uncurried form!)
    member this.Params1b (x : int) y = x + y

    // One Parameter (takes one param, returns a fast func: after fix for #3665, the IL is just the uncurried form!)
    member this.Params1c (x : float) y z = x + y + z

    // Two parameters
    member this.Params2 (x : decimal, y : decimal) = x + y

    // Three parameters
    member this.Params3 (x : float, y : int, z : char) = true

// --------------------------------


module Tester =

    open CodeGenHelper

    printfn "Testing..."

    try

        System.Reflection.Assembly.GetExecutingAssembly()
        |> getType "Test.AClassType"
        |> getMethod "Params0"
        |> should takeParams []

        System.Reflection.Assembly.GetExecutingAssembly()
        |> getType "Test.AClassType"
        |> getMethod "Params1"
        |> should takeParams [ typeof<unit> ]

        System.Reflection.Assembly.GetExecutingAssembly()
        |> getType "Test.AClassType"
        |> getMethod "Params1a"
        |> should takeParams [ typeof<int * float * string> ]

        System.Reflection.Assembly.GetExecutingAssembly()
        |> getType "Test.AClassType"
        |> getMethod "Params1b"
        |> should takeParams [ typeof<int>; typeof<int> ]
        
        System.Reflection.Assembly.GetExecutingAssembly()
        |> getType "Test.AClassType"
        |> getMethod "Params1c"
        |> should takeParams [ typeof<float>; typeof<float>; typeof<float>]

        System.Reflection.Assembly.GetExecutingAssembly()
        |> getType "Test.AClassType"
        |> getMethod "Params2"
        |> should takeParams [ typeof<decimal>; typeof<decimal> ]

        System.Reflection.Assembly.GetExecutingAssembly()
        |> getType "Test.AClassType"
        |> getMethod "Params3"
        |> should takeParams [ typeof<float>; typeof<int>; typeof<char> ]


    with
    | e -> printfn "Unhandled Exception: %s" e.Message
           exit 1

    exit 0
