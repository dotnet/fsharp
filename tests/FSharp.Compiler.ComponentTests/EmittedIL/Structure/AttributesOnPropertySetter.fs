// #Regression #NoMT #CodeGen #Interop
// Regression test for FSHARP1.0:1539
// Attributes on properties, getters and setters
module bug1539
open System

type AttrOnSetter() = inherit System.Attribute()

type C(x:int) =
   let mutable m_value = x

   member this.ReadWrite 
        with get() = m_value 
        and  [<AttrOnSetter>] set x = m_value <- x

open CodeGenHelper

try  
    System.Reflection.Assembly.GetExecutingAssembly()
    |> getType "bug1539+C"
    |> getMethod "set_ReadWrite"
    |> should haveAttribute "AttrOnSetter"

with
| e -> printfn "Unhandled Exception: %s" e.Message 
       raise (Exception($"Oops: {e}"))
