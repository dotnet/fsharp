// #Regression #Conformance #DeclarationElements #Attributes 
#light

// FSB 1437, Assembly attribute w/array parameter fails to build

open System

[<AttributeUsage(AttributeTargets.All)>]
type TestAttribute(param : int[]) =
    inherit Attribute()
    member this.Value = param

// Apply the attribute
[<TestAttribute([|0|])>]
type Foo() =
    [<TestAttribute([|1;2|])>]
    override this.ToString() = "Stuff"

[<assembly:TestAttribute ([|0|])>]
do
    let testPassed =
        let getTestAttribute (t : Type) =
            let tyAttributes = t.GetCustomAttributes(false)
            let attrib = tyAttributes |> Array.find (fun attrib -> match attrib with :? TestAttribute -> true | _ -> false)
            (attrib :?> TestAttribute)
        
        let tyFoo = typeof<Foo>
        let testAtt = getTestAttribute tyFoo
        if testAtt.Value <> [|0|] then 
            false
        else
            true

    if not testPassed then failwith "Failed: 1"
