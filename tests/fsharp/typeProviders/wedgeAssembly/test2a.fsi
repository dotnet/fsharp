module Test2a

module ErasedTypes = 
    val f : unit -> FSharp.HelloWorld.HelloWorldType
    val testTypeEquiv : FSharp.HelloWorld.HelloWorldType * FSharp.HelloWorld.HelloWorldType -> bool

module InternalGenerativeTypes = 
    
    type internal TheGeneratedType1 = FSharp.HelloWorldGenerative.TheContainerType<"TheGeneratedType1">
    val internal f2 : unit -> TheGeneratedType1
    val internal f3 : unit -> TheGeneratedType1
    val internal testTypeEquiv2 : TheGeneratedType1 * TheGeneratedType1 -> bool

module PublicGenerativeTypes = 
    
    type TheGeneratedType2 = FSharp.HelloWorldGenerative.TheContainerType<"TheGeneratedType2">
    val f2 : unit -> TheGeneratedType2
    val f3 : unit -> TheGeneratedType2
    val testTypeEquiv2 : TheGeneratedType2 * TheGeneratedType2 -> bool

module PublicErasedQuotations = 
    val q1 : Quotations.Expr<FSharp.HelloWorld.HelloWorldSubType -> FSharp.HelloWorld.HelloWorldSubType>
    val q2 : Quotations.Expr<FSharp.HelloWorld.HelloWorldType.NestedType -> FSharp.HelloWorld.HelloWorldType.NestedType>

module PublicErasedTypesWithStaticParameters = 
    val f1 : unit -> FSharp.HelloWorld.HelloWorldTypeWithStaticInt32Parameter<1> 
    val f2 : unit -> FSharp.HelloWorld.HelloWorldTypeWithStaticStringParameter<"1"> 
    type AbbrevString1 = FSharp.HelloWorld.HelloWorldTypeWithStaticStringParameter<"1">
    type AbbrevString2 = FSharp.HelloWorld.HelloWorldTypeWithStaticStringParameter<"2">
    val testTypeEquivString1 : AbbrevString1 * AbbrevString1 -> bool
    val testTypeEquivString2 : AbbrevString2 * AbbrevString2 -> bool

module GenerativeTypesWithStaticParameters = 
    
    type internal TheGeneratedTypeJ = FSharp.HelloWorldGenerativeWithStaticParameter.TheContainerType<"J">
    val internal v1 : TheGeneratedTypeJ 
    val v2 : int

    
    type TheGeneratedTypeK = FSharp.HelloWorldGenerativeWithStaticParameter.TheContainerType<"K">
    val v3 : TheGeneratedTypeK 
    val v4 : int


