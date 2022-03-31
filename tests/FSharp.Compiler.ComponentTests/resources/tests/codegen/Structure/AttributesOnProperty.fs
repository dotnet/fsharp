// #Regression #NoMT #CodeGen #Interop
// Regression test for FSHARP1.0:1539
// Attributes on properties, getters and setters
module bug1539

type AttrOnProperty() = inherit System.Attribute()

type C(x:int) =
   let mutable m_value = x

   [<AttrOnProperty>]
   member this.ReadWrite 
        with get() = m_value 
        and  set x = m_value <- x

open CodeGenHelper

try  
    System.Reflection.Assembly.GetExecutingAssembly()
    |> getType "bug1539+C"
    |> getProperty "ReadWrite"
    |> should haveAttribute "AttrOnProperty"

with
| e -> printfn "Unhandled Exception: %s" e.Message 
       exit 1
