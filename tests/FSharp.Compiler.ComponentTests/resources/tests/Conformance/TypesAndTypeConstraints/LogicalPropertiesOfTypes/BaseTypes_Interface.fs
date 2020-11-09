// #Conformance #TypeConstraints 
#light

// Test the base types of F# types: Interface

let baseTypeName<'a> = match typeof<'a> with
                       | null -> null
                       | x when x.BaseType <> null -> x.BaseType.Name
                       | x                         -> null

type InterfaceType =
    interface
    end

// As far as reflection is concerned, interfaces do have null as a basetype
if baseTypeName<InterfaceType> <> null then exit 1
// But for purposes of 'compiler goo' they do not, such as coersion between types.
type AClass() =
    interface InterfaceType
    
// Note how an interface has '.ToString()' despite not being derived from Syste.Object
let anInterface = (new AClass() :> InterfaceType)
if anInterface.ToString().EndsWith("AClass") = false then exit 1

exit 0
