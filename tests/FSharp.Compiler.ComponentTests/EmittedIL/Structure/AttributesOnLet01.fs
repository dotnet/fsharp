// #NoMT #CodeGen #Interop 
#light

// Verify the ability to put attributes on let-bindings
module AttributesOnLet01

open System

[<Obsolete>]
let x = 1

[<ObsoleteAttribute>]
let square a = a * a

// Use the CodeGenHelper library to test this
open CodeGenHelper

try

    System.Reflection.Assembly.GetExecutingAssembly()
    |> getType "AttributesOnLet01"
    |> getMember "x"
    |> should haveAttribute "ObsoleteAttribute"

    System.Reflection.Assembly.GetExecutingAssembly()
    |> getType "AttributesOnLet01"
    |> getMember "square"
    |> should haveAttribute "ObsoleteAttribute"

with
| e -> printfn "Unhandled Exception: %s" e.Message 
       raise (Exception($"Oops: {e}"))
