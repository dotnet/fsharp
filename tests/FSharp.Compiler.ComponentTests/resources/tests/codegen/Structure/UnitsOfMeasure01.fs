// #NoMT #CodeGen #Interop 
#light

// Verify units of measure are dropped and types are just their
// underlying primitive values.

namespace Test

[<Measure>]
type widget

[<Measure>]
type sprocket =
    static member DoStuff() = 5

type AClass() =
    member this.WidgetSprocketRatio = 4.25<widget / sprocket>
    member this.WidgetFactor   = 0.09345M<widget ^ 2>
    member this.SprocketFactor = 0.125f<1 / sprocket>



// --------------------------------

module Tester =

    open CodeGenHelper

    printfn "Testing..."

    // Verify the opaque types for Units of Measure are not generated.
    System.Reflection.Assembly.GetExecutingAssembly()
    |> should containType "Test.widget"             // see FSHARP1.0:4134
    
    // Sprocket should be found since it has static methods on it
    System.Reflection.Assembly.GetExecutingAssembly()
    |> should containType "Test.sprocket"

    System.Reflection.Assembly.GetExecutingAssembly()
    |> getType "Test.AClass"
    |> getProperty "WidgetFactor"
    |> should haveType typeof<decimal>

    System.Reflection.Assembly.GetExecutingAssembly()
    |> getType "Test.AClass"
    |> getProperty "WidgetSprocketRatio"
    |> should haveType typeof<float>

    System.Reflection.Assembly.GetExecutingAssembly()
    |> getType "Test.AClass"
    |> getProperty "SprocketFactor"
    |> should haveType typeof<float32>

    printfn "Finished"
