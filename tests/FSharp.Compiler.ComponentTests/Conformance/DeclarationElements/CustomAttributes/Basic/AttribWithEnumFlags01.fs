// #Conformance #DeclarationElements #Attributes 
#light

// FSB 950, Custom attributes with flags enumeration arguments

type EnumType =
    | A = 1
    | B = 2
    | C = 4

type EnumTypeInt64 = | E = 1L
    
type CustomAttribute(x : EnumType) =
    inherit System.Attribute()
    member this.Value = x
    
type CustomAttributeInt64(x : EnumTypeInt64) = 
    inherit System.Attribute()
    member this.Value = x
    
[<CustomAttribute(EnumType.A ||| EnumType.C)>]
type SomeClass() = 
    override this.ToString() = "foo"

[<CustomAttributeInt64(LanguagePrimitives.EnumOfValue 10L)>] 
type SomeClassInt64 = 
    interface end
    
let runTest() = 
    let testObj = new SomeClass()
    let itsAttributes = testObj.GetType().GetCustomAttributes(false)

    let attrib = itsAttributes |> Array.find (fun attrib -> match attrib with :? CustomAttribute -> true | _ -> false)
    if (attrib :?> CustomAttribute).Value <> (EnumType.A ||| EnumType.C) then failwith "Failed: 1"
    
    do
        let value = 
            typeof<SomeClassInt64>.GetCustomAttributes(false)
            |> Seq.tryPick(function
                | :? CustomAttributeInt64 as ca -> Some(int64 ca.Value)
                | _ -> None
            )
        match value with
        | Some v when v = 10L -> ()
        | _ -> failwith "Failed: 2"
    
    true

// Actually run our test
if runTest() <> true then failwith "Failed: 3"
