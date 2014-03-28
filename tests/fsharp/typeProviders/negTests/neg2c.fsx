#r "helloWorldProvider.dll"


type internal TheGeneratedType1 = FSharp.HelloWorldGenerative.TheContainerType<TypeName="TheGeneratedType1">

let f2() = Unchecked.defaultof<TheGeneratedType1> // should give an error because TheGeneratedType1 is not accessible enough to be part of public API
