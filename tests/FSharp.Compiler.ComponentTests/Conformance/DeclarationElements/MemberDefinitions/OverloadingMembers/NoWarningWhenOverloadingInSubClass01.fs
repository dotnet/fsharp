// #Regression #Conformance #DeclarationElements #MemberDefinitions #Overloading 
// FSharp1.0:4748 - override outscope warning does not seem to be overload sensitive
// This code should compile without warnings!

#light

module Test

type Root() =
    abstract Foo : int -> int
    default x.Foo y = y - 1

type Deriva() =
    inherit Root()

    member x.Foo (s:string) = s.Length

let test = Deriva()
test.Foo 1 |> ignore
test.Foo "test" |> ignore

