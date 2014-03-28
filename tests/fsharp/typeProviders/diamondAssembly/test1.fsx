#r "provider.dll"

module ErasedTypes = 
    let f() : FSharp.HelloWorld.HelloWorldType = Unchecked.defaultof<_>
    let testTypeEquiv(a:FSharp.HelloWorld.HelloWorldType,b:FSharp.HelloWorld.HelloWorldType) = (a = b)

    type TheType = FSharp.HelloWorld.HelloWorldType

module InternalGenerativeTypes = 
    
    type internal TheGeneratedType4 = FSharp.HelloWorldGenerative.TheContainerType<"TheGeneratedType4">

    let internal f2() : TheGeneratedType4 = Unchecked.defaultof<_>
    let internal f3() : TheGeneratedType4 = Unchecked.defaultof<_>
    let internal testTypeEquiv2(a:TheGeneratedType4,b:TheGeneratedType4) = (a = b)

module PublicGenerativeTypes = 
    
    type TheGeneratedType5 = FSharp.HelloWorldGenerativeInternalNamespace1.TheContainerType<"unused">

    let f2() : TheGeneratedType5 = Unchecked.defaultof<_>
    let f3() : TheGeneratedType5 = Unchecked.defaultof<_>
    let testTypeEquiv2(a:TheGeneratedType5,b:TheGeneratedType5) = (a = b)



//#endif
