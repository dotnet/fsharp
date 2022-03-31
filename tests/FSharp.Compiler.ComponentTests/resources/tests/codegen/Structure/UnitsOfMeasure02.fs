// #NoMT #CodeGen #Interop 
#light

// Verify units of measure are dropped and types are just their
// underlying primitive values when used in functions.

namespace Test

[<Measure>]
type widget

[<Measure>]
type sprocket

type AClass() =
    member this.DoStuff (x : float<widget>, y : float<sprocket>) = 
        let a = decimal x
        let b = decimal y
        a * b * 0.0M<sprocket>

// --------------------------------

module Tester =

    open CodeGenHelper

    printfn "Testing..."

    System.Reflection.Assembly.GetExecutingAssembly()
    |> getType "Test.AClass"
    |> getMethod "DoStuff"
    |> should takeParams [typeof<float>; typeof<float>]

    printfn "Finished"
