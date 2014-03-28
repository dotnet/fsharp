#r "helloWorldProvider.dll"


type internal TheGeneratedType1 = FSharp.HelloWorldGenerative.TheContainerType<TypeName="TheGeneratedType1">

// references must go through the alias GeneratedType, not FSharp.HelloWorldGenerative.TheGeneratedType
let internal f2() = Unchecked.defaultof<FSharp.HelloWorldGenerative.TheContainerType<TypeName="TheGeneratedType1">>

// references must go through the alias GeneratedType, not FSharp.HelloWorldGenerative.TheGeneratedType
let internal v1 : FSharp.HelloWorldGenerative.TheContainerType<TypeName="TheGeneratedType1"> = FSharp.HelloWorldGenerative.TheContainerType<TypeName="TheGeneratedType1">()


