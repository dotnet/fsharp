// #Regression #Conformance #DeclarationElements #Attributes 
#light

// FSB 3565, verify typeof<_> and typedefof<_> can be used in
// attributes. Also, verify that their behaivor is the same in
// attributes as in normal usage.

open System

   
type CustomAttribute(typeofResult : Type, typedefofResult : Type) =
    inherit System.Attribute()
    member this.TypeofResult    = typeofResult
    member this.TypedefofResult = typedefofResult
    
[<CustomAttribute(typeof<list<int>>, typedefof<list<int>>)>]
type SomeClass() = 
    class
    end

let runTest() = 
    let testObj = new SomeClass()
    let itsAttributes = testObj.GetType().GetCustomAttributes(false)

    let attrib = itsAttributes |> Array.find (fun attrib -> match attrib with :? CustomAttribute -> true | _ -> false)

    let ca = attrib :?> CustomAttribute
    if ca.TypeofResult    <> typeof<list<int>>    then exit 1
    if ca.TypedefofResult <> typedefof<list<int>> then exit 1

    // And verify typeof<_> <> typedefof<_>
    if typeof<list<int>> = typedefof<list<int>>   then exit 1

    true

// Actually run our test
if runTest() <> true then exit 1

exit 0
