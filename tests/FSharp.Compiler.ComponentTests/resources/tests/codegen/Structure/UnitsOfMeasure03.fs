// #NoMT #CodeGen #Interop 
#light

// Verify types which are generic WRT a unit of measure are exposed as 
// standard .NET types (no funny buisness!)

namespace Test

[<Measure>]
type widget

[<Measure>]
type sprocket

// Generic with regard to a unit of measure
type PointG0<[<Measure>] 'a>(x : float<'a>, y : float<'a>) =
    member this.X = x
    member this.Y = y
    
type PointWidget(x, y) =
    inherit PointG0<widget>(x, y)
    
// Generic with regard to a unit of measure AND a typearg
type PointG2<[<Measure>] 'm, 'a, 'b>() =
    inherit PointG0<'m>(0.0<_>, 0.0<_>)
    member this.DefaultA = Unchecked.defaultof<'a>
    member this.DefaultB = Unchecked.defaultof<'b>

// --------------------------------

module Tester =

    open System
    open CodeGenHelper

    printfn "Testing..."

    // Regression for 3263 (we shouldn't mangle name if it isn't actually generic)
    System.Reflection.Assembly.GetExecutingAssembly()
    |> should containType "Test.PointG0"

    System.Reflection.Assembly.GetExecutingAssembly()
    |> getType "Test.PointG0"
    |> (fun (ty : Type) -> if ty.IsGenericType then failwith "Error: Shouldn't be generic!")

    System.Reflection.Assembly.GetExecutingAssembly()
    |> getType "Test.PointWidget"
    |> (fun (ty : Type) -> if ty.BaseType.Name <> "PointG0" then failwith "Error: Not inherited properly?")

    // Truely generic type
    System.Reflection.Assembly.GetExecutingAssembly()
    |> getType "Test.PointG2`2"
    |> (fun (ty : Type) -> if not ty.IsGenericType then failwith "Error: Should be generic!")

    printfn "Finished"
