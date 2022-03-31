// #NoMT #CodeGen #Interop 
#light

// Verify the ability to put attributes on let-bindings
// - When declaring multiple values at once, attr is applied to all
module AttributesOnLet02

open System

[<System.Obsolete()>]
let (a, b) = (1,2)

(*
// This syntax is no longer allowed, attributes can only be applied to whole pattern rather than its elements separately.
// Related test is placed under Conformance\DeclarationElements\LetBindings\Basic

let ([<System.Obsolete()>] venus, earth, [<System.Obsolete()>] mars) = 
        ("too hot","just right", "too cold")
*)

// Validation
open CodeGenHelper

try

    // The one attribute applies to both values
    System.Reflection.Assembly.GetExecutingAssembly()
    |> getType "AttributesOnLet02"
    |> getProperty "a"
    |> should haveAttribute "ObsoleteAttribute"

    System.Reflection.Assembly.GetExecutingAssembly()
    |> getType "AttributesOnLet02"
    |> getProperty "b"
    |> should haveAttribute "ObsoleteAttribute"

(*
    // Verify ability to apply attribute to individual values
    System.Reflection.Assembly.GetExecutingAssembly()
    |> getType "AttributesOnLet02"
    |> getProperty "venus"
    |> should haveAttribute "ObsoleteAttribute"

    System.Reflection.Assembly.GetExecutingAssembly()
    |> getType "AttributesOnLet02"
    |> getProperty "earth"
    |> verify doesn'tHaveAttribute "ObsoleteAttribute"

    System.Reflection.Assembly.GetExecutingAssembly()
    |> getType "AttributesOnLet02"
    |> getProperty "mars"
    |> should haveAttribute "ObsoleteAttribute"
*)

with
| e -> printfn "Unhandled Exception: %s" e.Message 
       exit 1

exit 0
