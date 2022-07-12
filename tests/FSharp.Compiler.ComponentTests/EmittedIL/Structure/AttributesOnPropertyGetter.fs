// #Regression #NoMT #CodeGen #Interop
// Regression test for FSHARP1.0:1539
// Attributes on properties, getters and setters
module bug1539
open System

type AttrOnGetter() = inherit System.Attribute()

type C(x:int) =
   let mutable m_value = x

   member this.ReadWrite 
        with [<AttrOnGetter>] get() = m_value 
        and set x = m_value <- x

open CodeGenHelper

try  
    System.Reflection.Assembly.GetExecutingAssembly()
    |> getType "bug1539+C"
    |> getMethod "get_ReadWrite"
    |> should haveAttribute "AttrOnGetter"

with
| e -> printfn "Unhandled Exception: %s" e.Message 
       raise (Exception($"Oops: {e}"))

