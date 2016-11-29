// #Conformance #DeclarationElements #MemberDefinitions #OptionalDefaultParameterValueArguments 

open System.Runtime.InteropServices
open System

// test that only default parameters at interfaces have effect.
type MyFace = 
    abstract Test : [<Optional;DefaultParameterValue(4)>] i:int-> int

type Impl() =
    interface MyFace with
        // the default will be ignored here.
        // C# compiler has a warning for this.
        override __.Test([<Optional;DefaultParameterValue(5)>] i) = i

let impl = new Impl() :> MyFace
if impl.Test() <> 4 then exit 1

exit 0
