// #Conformance #ObjectOrientedTypes #Enums 
#light

// FSB 531, Cannot use "A | B" in enum flags when specifying a custom attribute (not a recognised constant)

open System

type AnEnum =
    | A = 1
    | B = 2
    | C = 4
    | D = 8

type CustomAttribute(x : AnEnum) =
    inherit Attribute()
    member this.Value = x
    
[<CustomAttribute(AnEnum.A ||| AnEnum.B ||| AnEnum.C ||| AnEnum.D)>]
type SomeClass() =
    override this.ToString() = "SomeClass"

// Now use reflection to make sure we get the right value
let scAttributes = typeof<SomeClass>.GetCustomAttributes(false)
let custAttrib = scAttributes |> Array.find (fun attrib -> match attrib with :? CustomAttribute -> true | _ -> false)

let testResult =
    match custAttrib with
    | null -> false
    | _    -> let typedCustAttrib = custAttrib :?> CustomAttribute
              let value = typedCustAttrib.Value
              if (int value) <> 15 then 
                  false
              else
                  true

if testResult <> true then exit 1
exit 0
