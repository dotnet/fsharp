module Test2b

module ErasedTypes = 
    let f() : FSharp.HelloWorld.HelloWorldType = Unchecked.defaultof<_>
    let testTypeEquiv(a:FSharp.HelloWorld.HelloWorldType,b:FSharp.HelloWorld.HelloWorldType) = (a = b)

module PublicErasedQuotations = 
    let q1 = <@ (fun (x : FSharp.HelloWorld.HelloWorldSubType) -> x) @>
    let q2 = <@ (fun (x : FSharp.HelloWorld.HelloWorldType.NestedType) -> x) @>

module PublicErasedTypesWithStaticParameters = 
    let f1() : FSharp.HelloWorld.HelloWorldTypeWithStaticInt32Parameter<1> = Unchecked.defaultof<_>
    let f2() : FSharp.HelloWorld.HelloWorldTypeWithStaticStringParameter<"1"> = Unchecked.defaultof<_>
    type AbbrevString1 = FSharp.HelloWorld.HelloWorldTypeWithStaticStringParameter<"1">
    type AbbrevString2 = FSharp.HelloWorld.HelloWorldTypeWithStaticStringParameter<"2">
    let testTypeEquivString1(a:AbbrevString1,b:AbbrevString1) = (a = b)
    let testTypeEquivString2(a:AbbrevString2,b:AbbrevString2) = (a = b)


module InternalGenerativeTypes = 
    
    type internal TheGeneratedType1 = FSharp.HelloWorldGenerative.TheContainerType<"TheGeneratedType1">

    let internal f2() : TheGeneratedType1 = Unchecked.defaultof<_>
    let internal f3() : TheGeneratedType1 = Unchecked.defaultof<_>
    let internal testTypeEquiv2(a:TheGeneratedType1,b:TheGeneratedType1) = (a = b)


module PublicGenerativeTypes = 
    
    type TheGeneratedType2 = FSharp.HelloWorldGenerative.TheContainerType<"TheGeneratedType2">

    let f2() : TheGeneratedType2 = Unchecked.defaultof<_>
    let f3() : TheGeneratedType2 = Unchecked.defaultof<_>
    let testTypeEquiv2(a:TheGeneratedType2,b:TheGeneratedType2) = (a = b)


module GenerativeTypesWithStaticParameters = 
    
    type internal TheGeneratedTypeJ = FSharp.HelloWorldGenerativeWithStaticParameter.TheContainerType<"J">

    let internal v1 : TheGeneratedTypeJ = TheGeneratedTypeJ()

    let v2 = v1.Item1

    
    type TheGeneratedTypeK = FSharp.HelloWorldGenerativeWithStaticParameter.TheContainerType<"K">

    let v3 : TheGeneratedTypeK = TheGeneratedTypeK()

    let v4 = v3.Item1


//#endif
