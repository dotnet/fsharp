// #NoMT #CodeGen #Interop 
#light

// Verify delegates are generated correctly

namespace Test

type UnitDelegate            = delegate of unit -> unit

type VoidReturnType          = delegate of obj * System.EventArgs -> unit

type TupledDelegate          = delegate of int * float * unit -> decimal * char * obj

type GenericDelegate<'a, 'b> = delegate of 'a -> 'b

// --------------------------------

module Tester =

    open CodeGenHelper

    printfn "Testing..."

    try

        // Take no parameters, return void
        System.Reflection.Assembly.GetExecutingAssembly()
        |> getType "Test.UnitDelegate"
        |> should beDelegate [] typeof<System.Void>

        System.Reflection.Assembly.GetExecutingAssembly()
        |> getType "Test.VoidReturnType"
        |> should beDelegate [typeof<obj>; typeof<System.EventArgs>] typeof<System.Void>

        System.Reflection.Assembly.GetExecutingAssembly()
        |> getType "Test.TupledDelegate"
        |> should beDelegate [typeof<int>; typeof<float>; typeof<unit>] typeof<decimal * char * obj>

        typeof< GenericDelegate<string Set, int list> >
        |> should beDelegate [typeof<string Set>] typeof<int list>


    with
    | e -> printfn "Unhandled Exception: %s" e.Message
           exit 1

    exit 0
