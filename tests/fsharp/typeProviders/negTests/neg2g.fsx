#r "helloWorldProvider.dll"

open FSharp.HelloWorldGenerative


// A 'Generate' reference should be to the container, not to the nested types
type internal GeneratedType = TheContainerType<TypeName="TheOuterType">.TheNestedGeneratedType

