
#r "provider.dll"
#r "test1.dll"
#load "./test1.fsx"
#load "./test2a.fsx"

module ErasedTypes = 
    let f() : FSharp.HelloWorld.HelloWorldType = Unchecked.defaultof<_>
    let testTypeEquiv(a:FSharp.HelloWorld.HelloWorldType,b:FSharp.HelloWorld.HelloWorldType) = (a = b)
    let testTypeEquiv2(a:Test1.ErasedTypes.TheType,b:FSharp.HelloWorld.HelloWorldType) = (a = b)


module InternalGenerativeTypes = 

    let internal f2() : Test1.PublicGenerativeTypes.TheGeneratedType5 = Unchecked.defaultof<_>
    let internal f3() : Test1.PublicGenerativeTypes.TheGeneratedType5 = Unchecked.defaultof<_>
    let internal testTypeEquiv2(a:Test1.PublicGenerativeTypes.TheGeneratedType5,b:Test1.PublicGenerativeTypes.TheGeneratedType5) = (a = b)

module PublicGenerativeTypes = 
    type TheGeneratedType5 = Test1.PublicGenerativeTypes.TheGeneratedType5

    let f2() : TheGeneratedType5 = Unchecked.defaultof<_>
    let f3() : TheGeneratedType5 = Unchecked.defaultof<_>
    let testTypeEquiv2(a:TheGeneratedType5,b:TheGeneratedType5) = (a = b)



