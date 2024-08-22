namespace Provider
#load "TypeMagic.fs"

open System
open System.Linq.Expressions
open System.Reflection
open Microsoft.FSharp.Core.CompilerServices
open Microsoft.FSharp.Linq.NullableOperators
open System.Collections.Generic
open System.IO
open FSharp.TypeMagic

[<assembly: TypeProviderAssembly>]
do ()

[<AutoOpen>]
module Utils = 
    let (|StartsWith|_|) p (s:string) = if s.StartsWith(p) then Some() else None
    let mkNamespace (name,typ:System.Type) = 
        { new IProvidedNamespace with
          member this.NamespaceName = name
          member this.GetTypes() = [|typ|]
          member this.GetNestedNamespaces() = [| |]
          member this.ResolveTypeName typeName = if typ.Name = typeName then typ else null }

    let mkParamArrayAttributeData() = 
        { new CustomAttributeData() with 
                member __.Constructor =  typeof<ParamArrayAttribute>.GetConstructors().[0]
                member __.ConstructorArguments = upcast [| |]
                member __.NamedArguments = upcast [| |] }

    let mkObsoleteAttributeData(msg:string) = 
        { new CustomAttributeData() with 
                member __.Constructor =  typeof<ObsoleteAttribute>.GetConstructors().[0]
                member __.ConstructorArguments = upcast [| CustomAttributeTypedArgument(typeof<string>, msg)  |]
                member __.NamedArguments = upcast [| |] }

    let mkConditionalAttributeData(msg:string) = 
        { new CustomAttributeData() with 
                member __.Constructor =  typeof<System.Diagnostics.ConditionalAttribute>.GetConstructors().[0]
                member __.ConstructorArguments = upcast [| CustomAttributeTypedArgument(typeof<string>, msg)  |]
                member __.NamedArguments = upcast [| |] }

    let mkAllowNullLiteralValueAttributeData(value: bool) = 
        { new CustomAttributeData() with 
                member __.Constructor =  typeof<Microsoft.FSharp.Core.AllowNullLiteralAttribute>.GetConstructors().[0]
                member __.ConstructorArguments = upcast [| CustomAttributeTypedArgument(typeof<bool>, value)  |]
                member __.NamedArguments = upcast [| |] }

type public Runtime() =
    static member Id x = x

type MemoizationTable<'T,'U when 'T : equality>(createType) =
    let createdDB = new Dictionary<'T,'U>()
    member x.Contents = (createdDB :> IDictionary<_,_>)
    member x.Apply typeName =
        let found,result = createdDB.TryGetValue typeName
        if found then
            result 
        else
            let ty = createType typeName
            createdDB.[typeName] <- ty
            ty 

[<TypeProvider>]
type public GlobalNamespaceProvider() =
    static let thisAssembly = typeof<GlobalNamespaceProvider>.Assembly
    static let modul = thisAssembly.GetModules().[0]
    let rootNamespace = null
    [<Literal>]
    static let typeName = "ProvidedTypeFromGlobalNamespace"
    let invalidation = new Event<System.EventHandler,_>()

    let typ =
        TypeBuilder.CreateSimpleType(TypeContainer.Namespace(modul,null), typeName)
    let types = [|typ|]

    interface IProvidedNamespace with
        member this.GetNestedNamespaces() = [||]
        member this.ResolveTypeName(name) =
            match name with
            |   typeName -> typ
            |   _ -> null
        member this.NamespaceName = null
        member this.GetTypes() = types


    interface IDisposable with
        member __.Dispose() = ()
    interface ITypeProvider with
        member this.GetStaticParameters _ = [||]
        member this.ApplyStaticArguments(_,_,_) = raise <| System.InvalidOperationException()
        member this.GetNamespaces() = [| this |]
        member this.GetInvokerExpression(_,_) = failwith "GetInvokerExpression"
        [<CLIEvent>]
        member this.Invalidate = invalidation.Publish
        member this.GetGeneratedAssemblyContents(assembly) = failwith "GetGeneratedAssemblyContents"

#if USE_IMPLICIT_ITypeProvider2
    member this.GetStaticParametersForMethod _ = [||]
    member this.ApplyStaticArgumentsForMethod(_,_,_) = raise <| System.InvalidOperationException()
#else
    interface ITypeProvider2 with
        member this.GetStaticParametersForMethod _ = [||]
        member this.ApplyStaticArgumentsForMethod(_,_,_) = raise <| System.InvalidOperationException()
#endif



[<TypeProvider>]
type public Provider(config: TypeProviderConfig) =
    let thisAssembly = typeof<Provider>.Assembly
    let modul = thisAssembly.GetModules().[0]
    let rootNamespace = "FSharp.HelloWorld"
    let nestedNamespaceName1 = "FSharp.HelloWorld.NestedNamespace1"
    let nestedNamespaceName2 = "FSharp.HelloWorld.Nested.Nested.Nested.Namespace2"

    do if not (config.ReferencedAssemblies |> Seq.exists (fun s -> s.Contains("FSharp.Core")) ) then 
          failwith "expected FSharp.Core in type provider config references"

    // Test provision of erase methods with static parameters
    let helloWorldMethodWithStaticParameters =
        MemoizationTable(fun (isStatic, enclType, ty, nm, n:int) ->
           TypeBuilder.CreateMethod(enclType, nm, ty, isStatic=isStatic, parameters=[| for i in 1..n -> TypeBuilder.CreateParameter("x" +  string i,ty) |]) :> MethodBase)

    let helloWorldMethodsWithStaticParametersUninstantiated enclType =
         [| yield helloWorldMethodWithStaticParameters.Apply(false,enclType,typeof<obj>,"HelloWorldInstanceMethodWithStaticCharParameter",0) :> MemberInfo 
            yield helloWorldMethodWithStaticParameters.Apply(false,enclType,typeof<obj>,"HelloWorldInstanceMethodWithStaticBoolParameter",0) :> MemberInfo 
            yield helloWorldMethodWithStaticParameters.Apply(false,enclType,typeof<obj>,"HelloWorldInstanceMethodWithStaticSByteParameter",0) :> MemberInfo 
            yield helloWorldMethodWithStaticParameters.Apply(false,enclType,typeof<obj>,"HelloWorldInstanceMethodWithStaticInt16Parameter",0) :> MemberInfo 
            yield helloWorldMethodWithStaticParameters.Apply(false,enclType,typeof<obj>,"HelloWorldInstanceMethodWithStaticInt32Parameter",0) :> MemberInfo 
            yield helloWorldMethodWithStaticParameters.Apply(false,enclType,typeof<obj>,"HelloWorldInstanceMethodWithStaticOptionalInt32Parameter",0) :> MemberInfo 
            yield helloWorldMethodWithStaticParameters.Apply(false,enclType,typeof<obj>,"HelloWorldInstanceMethodWithStaticInt64Parameter",0) :> MemberInfo 
            yield helloWorldMethodWithStaticParameters.Apply(false,enclType,typeof<obj>,"HelloWorldInstanceMethodWithStaticByteParameter",0) :> MemberInfo 
            yield helloWorldMethodWithStaticParameters.Apply(false,enclType,typeof<obj>,"HelloWorldInstanceMethodWithStaticUInt16Parameter",0) :> MemberInfo 
            yield helloWorldMethodWithStaticParameters.Apply(true,enclType,typeof<obj>,"HelloWorldStaticMethodWithStaticUInt32Parameter",0) :> MemberInfo 
            yield helloWorldMethodWithStaticParameters.Apply(true,enclType,typeof<obj>,"HelloWorldStaticMethodWithStaticUInt64Parameter",0) :> MemberInfo 
            yield helloWorldMethodWithStaticParameters.Apply(false,enclType,typeof<obj>,"HelloWorldInstanceMethodWithStaticDayOfWeekParameter",0) :> MemberInfo 
            yield helloWorldMethodWithStaticParameters.Apply(false,enclType,typeof<obj>,"HelloWorldInstanceMethodWithStaticStringParameter",0) :> MemberInfo 
            yield helloWorldMethodWithStaticParameters.Apply(false,enclType,typeof<obj>,"HelloWorldInstanceMethodWithStaticSingleParameter",0) :> MemberInfo 
            yield helloWorldMethodWithStaticParameters.Apply(false,enclType,typeof<obj>,"HelloWorldInstanceMethodWithStaticDoubleParameter",0) :> MemberInfo 
            yield helloWorldMethodWithStaticParameters.Apply(false,enclType,typeof<obj>,"HelloWorldInstanceMethodWithStaticDecimalParameter",0) :> MemberInfo 
         |]

    // Test provision of an erased class 
    let mkHelloWorldType namespaceName className baseType =
        let rec allMembers = 
            lazy 
                [| for (propertyName, propertyType) in  [for i in 1 .. 2 -> ("StaticProperty"+string i, (if i = 1 then typeof<string> else typeof<int>)) ] do 
                       let prop = TypeBuilder.CreateSyntheticProperty(theType,propertyName,propertyType,isStatic=true) 
                       yield! TypeBuilder.JoinPropertiesIntoMemberInfos [prop]
                   yield TypeBuilder.CreateMethod(theType ,"OneOptionalParameter",typeof<int>, isStatic=true, parameters=[| TypeBuilder.CreateParameter("arg1",typeof<int>,optionalValue=3375);  |]) :> MemberInfo 
                   yield TypeBuilder.CreateMethod(theType ,"TwoOptionalParameters",typeof<int>, isStatic=true, parameters=[| TypeBuilder.CreateParameter("arg1",typeof<int>,optionalValue=3375); TypeBuilder.CreateParameter("arg2",typeof<int>,optionalValue=3390) |]) :> MemberInfo 
                   yield TypeBuilder.CreateMethod(theType ,"UsesByRef",typeof<int>, isStatic=true, parameters=[| TypeBuilder.CreateParameter("arg1",typeof<int>.MakeByRefType())|]) :> MemberInfo 
                   yield TypeBuilder.CreateMethod(theType ,"UsesByRefAsOutParameter",typeof<int>, isStatic=true, parameters=[| TypeBuilder.CreateParameter("arg1",typeof<int>); TypeBuilder.CreateParameter("arg2",typeof<int>.MakeByRefType(),isOut=true)|]) :> MemberInfo 
                   yield TypeBuilder.CreateMethod(theType ,"op_Addition",theType, isStatic=true, parameters=[| TypeBuilder.CreateParameter("arg1",theType);TypeBuilder.CreateParameter("arg2",theType) |]) :> MemberInfo 
                   yield TypeBuilder.CreateMethod(theType ,"op_Subtraction",theType, isStatic=true, parameters=[| TypeBuilder.CreateParameter("arg1",theType);TypeBuilder.CreateParameter("arg2",theType) |]) :> MemberInfo 
                   yield TypeBuilder.CreateMethod(theType ,"ReturnsNullSeqString",typeof<seq<string>>, isStatic=true, parameters=[| |]) :> MemberInfo 
                   yield TypeBuilder.CreateMethod(theType ,"ReturnsNullString",typeof<string>, isStatic=true, parameters=[| |]) :> MemberInfo 
                   yield TypeBuilder.CreateMethod(theType ,"ReturnsMondayDayOfWeek",typeof<System.DayOfWeek>, isStatic=true, parameters=[| |]) :> MemberInfo 
                   yield TypeBuilder.CreateMethod(theType ,"ReturnsDefaultDayOfWeek",typeof<System.DayOfWeek>, isStatic=true, parameters=[| |]) :> MemberInfo 
                   yield TypeBuilder.CreateMethod(theType ,"ReturnsDefaultDateTime",typeof<System.DateTime>, isStatic=true, parameters=[| |]) :> MemberInfo 
                   yield TypeBuilder.CreateMethod(theType ,"ReturnsUnit",typeof<unit>, isStatic=true, parameters=[| |]) :> MemberInfo 
                   yield TypeBuilder.CreateMethod(theType ,"ReturnsTuple2OfInt",typeof<int * int>, isStatic=true, parameters=[| |]) :> MemberInfo 
                   yield TypeBuilder.CreateMethod(theType ,"ReturnsTuple3OfInt",typeof<int * int * int>, isStatic=true, parameters=[| |]) :> MemberInfo 
                   yield TypeBuilder.CreateMethod(theType ,"ReturnsTuple4OfInt",typeof<int * int * int * int>, isStatic=true, parameters=[| |]) :> MemberInfo 
                   yield TypeBuilder.CreateMethod(theType ,"ReturnsTuple5OfInt",typeof<int * int * int * int * int>, isStatic=true, parameters=[| |]) :> MemberInfo 
                   yield TypeBuilder.CreateMethod(theType ,"ReturnsTuple6OfInt",typeof<int * int * int * int * int * int>, isStatic=true, parameters=[| |]) :> MemberInfo 
                   yield TypeBuilder.CreateMethod(theType ,"ReturnsTuple7OfInt",typeof<int * int * int * int * int * int * int>, isStatic=true, parameters=[| |]) :> MemberInfo 
                   yield TypeBuilder.CreateMethod(theType ,"ReturnsTuple8OfInt",typeof<int * int * int * int * int * int * int * int>, isStatic=true, parameters=[| |]) :> MemberInfo 
                   yield TypeBuilder.CreateMethod(theType ,"ReturnsTuple9OfInt",typeof<int * int * int * int * int * int * int * int * int>, isStatic=true, parameters=[| |]) :> MemberInfo 
                   yield TypeBuilder.CreateMethod(theType ,"GetItem1OfTuple2OfInt",typeof<int>, isStatic=true, parameters=[| TypeBuilder.CreateParameter("arg",typeof<int*int>) |]) :> MemberInfo 
                   yield TypeBuilder.CreateMethod(theType ,"GetItem2OfTuple2OfInt",typeof<int>, isStatic=true, parameters=[| TypeBuilder.CreateParameter("arg",typeof<int*int>) |]) :> MemberInfo 

                   yield TypeBuilder.CreateMethod(theType ,"GetItem1OfTuple7OfInt",typeof<int>, isStatic=true, parameters=[| TypeBuilder.CreateParameter("arg",typeof<int*int*int*int*int*int*int>) |]) :> MemberInfo 
                   yield TypeBuilder.CreateMethod(theType ,"GetItem7OfTuple7OfInt",typeof<int>, isStatic=true, parameters=[| TypeBuilder.CreateParameter("arg",typeof<int*int*int*int*int*int*int>) |]) :> MemberInfo 

                   yield TypeBuilder.CreateMethod(theType ,"GetItem1OfTuple8OfInt",typeof<int>, isStatic=true, parameters=[| TypeBuilder.CreateParameter("arg",typeof<int*int*int*int*int*int*int*int>) |]) :> MemberInfo 
                   yield TypeBuilder.CreateMethod(theType ,"GetItem7OfTuple8OfInt",typeof<int>, isStatic=true, parameters=[| TypeBuilder.CreateParameter("arg",typeof<int*int*int*int*int*int*int*int>) |]) :> MemberInfo 
                   yield TypeBuilder.CreateMethod(theType ,"GetItem8OfTuple8OfInt",typeof<int>, isStatic=true, parameters=[| TypeBuilder.CreateParameter("arg",typeof<int*int*int*int*int*int*int*int>) |]) :> MemberInfo 
                   
                   yield TypeBuilder.CreateMethod(theType ,"GetItem1OfTuple9OfInt",typeof<int>, isStatic=true, parameters=[| TypeBuilder.CreateParameter("arg",typeof<int*int*int*int*int*int*int*int*int>) |]) :> MemberInfo 
                   yield TypeBuilder.CreateMethod(theType ,"GetItem7OfTuple9OfInt",typeof<int>, isStatic=true, parameters=[| TypeBuilder.CreateParameter("arg",typeof<int*int*int*int*int*int*int*int*int>) |]) :> MemberInfo 
                   yield TypeBuilder.CreateMethod(theType ,"GetItem8OfTuple9OfInt",typeof<int>, isStatic=true, parameters=[| TypeBuilder.CreateParameter("arg",typeof<int*int*int*int*int*int*int*int*int>) |]) :> MemberInfo 
                   yield TypeBuilder.CreateMethod(theType ,"GetItem9OfTuple9OfInt",typeof<int>, isStatic=true, parameters=[| TypeBuilder.CreateParameter("arg",typeof<int*int*int*int*int*int*int*int*int>) |]) :> MemberInfo 
                   yield TypeBuilder.CreateMethod(theType ,"ArrayGetSmokeTest",typeof<int>, isStatic=true, parameters=[| TypeBuilder.CreateParameter("arg",typeof<int[]>) |]) :> MemberInfo 
                   yield TypeBuilder.CreateMethod(theType ,"ArraySetSmokeTest",typeof<unit>, isStatic=true, parameters=[| TypeBuilder.CreateParameter("arg",typeof<int[]>) |]) :> MemberInfo 
                   yield TypeBuilder.CreateMethod(theType ,"VarSetSmokeTest",typeof<int>, isStatic=true, parameters=[|  |]) :> MemberInfo 
                   yield TypeBuilder.CreateMethod(theType ,"Array2DGetSmokeTest",typeof<int>, isStatic=true, parameters=[| TypeBuilder.CreateParameter("arg",typeof<int[,]>) |]) :> MemberInfo 
                   yield TypeBuilder.CreateMethod(theType ,"Array2DSetSmokeTest",typeof<unit>, isStatic=true, parameters=[| TypeBuilder.CreateParameter("arg",typeof<int[,]>) |]) :> MemberInfo 
                   yield TypeBuilder.CreateMethod(theType ,"TakesParamArray",typeof<int>, isStatic=true, parameters=[| TypeBuilder.CreateParameter("arg1",typeof<int>); TypeBuilder.CreateParameter("arg2",typeof<obj[]>,getCustomAttributes=(fun () -> [| mkParamArrayAttributeData() |])); |]) :> MemberInfo 
                   yield TypeBuilder.CreateMethod(theType ,"IsMarkedConditional1",typeof<unit>, isStatic=true, parameters=[|  |], getCustomAttributes=(fun () -> [| mkConditionalAttributeData("INTERACTIVE") |])) :> MemberInfo 
                   yield TypeBuilder.CreateMethod(theType ,"IsMarkedObsolete1",typeof<int>, isStatic=true, parameters=[|  |], getCustomAttributes=(fun () -> [| mkObsoleteAttributeData(null) |])) :> MemberInfo 
                   yield TypeBuilder.CreateMethod(theType ,"IsMarkedObsolete2",typeof<int>, isStatic=true, parameters=[|  |], getCustomAttributes=(fun () -> [| mkObsoleteAttributeData("this is obsolete, ok? don't use it. we warned you.") |])) :> MemberInfo 
                   yield TypeBuilder.CreateMethod(theType ,"ReturnsNewArray",typeof<int[]>, isStatic=true, parameters=[| |]) :> MemberInfo 
                   yield TypeBuilder.CreateMethod(theType ,"ReturnsEmptyNewArray",typeof<int[]>, isStatic=true, parameters=[| |]) :> MemberInfo 
                   yield TypeBuilder.CreateMethod(theType ,"IfThenElseUnitUnit",typeof<System.Void>, isStatic=true, parameters=[| TypeBuilder.CreateParameter("arg",typeof<bool>) |]) :> MemberInfo 
                   yield TypeBuilder.CreateMethod(theType ,"IfThenElseVoidUnit",typeof<unit>, isStatic=true, parameters=[| TypeBuilder.CreateParameter("arg",typeof<bool>) |]) :> MemberInfo 
                   yield TypeBuilder.CreateMethod(theType ,"SequentialSmokeTest",typeof<int>, isStatic=true, parameters=[|  |]) :> MemberInfo 
                   yield TypeBuilder.CreateMethod(theType ,"SequentialSmokeTest2",typeof<unit>, isStatic=true, parameters=[|  |]) :> MemberInfo 
                   yield TypeBuilder.CreateMethod(theType ,"LambdaSmokeTest",typeof<int -> int >, isStatic=true, parameters=[|  |]) :> MemberInfo 
                   yield TypeBuilder.CreateMethod(theType ,"LambdaSmokeTest2",typeof<unit -> unit >, isStatic=true, parameters=[|  |]) :> MemberInfo 
                   yield TypeBuilder.CreateMethod(theType ,"LambdaSmokeTest3",typeof<int -> int -> int>, isStatic=true, parameters=[|  |]) :> MemberInfo 
                   yield TypeBuilder.CreateMethod(theType ,"CallIntrinsics",typeof<obj list>, isStatic=true, parameters=[|  |]) :> MemberInfo 
                   yield TypeBuilder.CreateMethod(theType ,"NewUnionCaseSmokeTest1",typeof<list<int>>, isStatic=true, parameters=[|  |]) :> MemberInfo 
                   yield TypeBuilder.CreateMethod(theType ,"NewUnionCaseSmokeTest2",typeof<list<int>>, isStatic=true, parameters=[|  |]) :> MemberInfo 
                   yield TypeBuilder.CreateMethod(theType ,"NewUnionCaseSmokeTest3",typeof<option<int>>, isStatic=true, parameters=[|  |]) :> MemberInfo 
                   yield TypeBuilder.CreateMethod(theType ,"NewUnionCaseSmokeTest4",typeof<option<int>>, isStatic=true, parameters=[|  |]) :> MemberInfo 
                   yield TypeBuilder.CreateMethod(theType ,"NewUnionCaseSmokeTest5",typeof<Choice<int,string>>, isStatic=true, parameters=[|  |]) :> MemberInfo 
                   yield TypeBuilder.CreateMethod(theType ,"NewObjectGeneric1",typeof<System.Collections.Generic.Dictionary<int,int>>, isStatic=true, parameters=[|  |]) :> MemberInfo 
                   yield TypeBuilder.CreateMethod(theType ,"NewObjectGeneric2",typeof<System.Collections.Generic.Dictionary<int,int>>, isStatic=true, parameters=[|  |]) :> MemberInfo 
                   yield TypeBuilder.CreateMethod(theType ,"NewObjectGeneric3",typeof<System.Collections.Generic.Dictionary<int,int>>, isStatic=true, parameters=[|  |]) :> MemberInfo 
                   yield TypeBuilder.CreateMethod(theType ,"NewRecordSmokeTest1",typeof<int ref>, isStatic=true, parameters=[|  |]) :> MemberInfo 
                   yield TypeBuilder.CreateMethod(theType ,"NewDelegateSmokeTest1",typeof<System.Func<int,int>>, isStatic=true, parameters=[|  |]) :> MemberInfo 
                   yield TypeBuilder.CreateMethod(theType ,"NewDelegateSmokeTest2",typeof<System.Func<int,int,int>>, isStatic=true, parameters=[|  |]) :> MemberInfo 
                   yield TypeBuilder.CreateMethod(theType ,"WhileLoopSmokeTest1",typeof<int>, isStatic=true, parameters=[|  |]) :> MemberInfo 
                   yield TypeBuilder.CreateMethod(theType ,"ForIntegerRangeLoopSmokeTest1",typeof<int>, isStatic=true, parameters=[|  |]) :> MemberInfo 
                   yield TypeBuilder.CreateMethod(theType ,"StructProperty1",typeof<float>, isStatic=true, parameters=[|  |]) :> MemberInfo 
                   yield TypeBuilder.CreateMethod(theType ,"StructProperty2",typeof<float>, isStatic=true, parameters=[|  |]) :> MemberInfo 
                   yield TypeBuilder.CreateMethod(theType ,"StructProperty3",typeof<float>, isStatic=true, parameters=[|  |]) :> MemberInfo 
                   yield TypeBuilder.CreateMethod(theType ,"StructMethod1",typeof<float>, isStatic=true, parameters=[|  |]) :> MemberInfo 
                   yield TypeBuilder.CreateMethod(theType ,"StructMethod2",typeof<float>, isStatic=true, parameters=[|  |]) :> MemberInfo 
                   yield TypeBuilder.CreateMethod(theType ,"UnionCaseTest1",typeof<bool>, isStatic=true, parameters=[|   TypeBuilder.CreateParameter("arg",typeof<list<int>>) |]) :> MemberInfo 
                   yield TypeBuilder.CreateMethod(theType ,"UnionCaseTest2",typeof<bool>, isStatic=true, parameters=[|   TypeBuilder.CreateParameter("arg",typeof<option<int>>) |]) :> MemberInfo 
                   yield TypeBuilder.CreateMethod(theType ,"TryFinallySmokeTest",typeof<int>, isStatic=true, parameters=[|  |]) :> MemberInfo 
                   yield TypeBuilder.CreateMethod(theType ,"TryWithSmokeTest",typeof<int>, isStatic=true, parameters=[|  |]) :> MemberInfo 
                   yield TypeBuilder.CreateMethod(theType ,"TryWithSmokeTest2",typeof<bool>, isStatic=true, parameters=[|  |]) :> MemberInfo 
                   yield TypeBuilder.CreateMethod(theType ,"TryWithSmokeTest3",typeof<bool>, isStatic=true, parameters=[|  |]) :> MemberInfo 
                   
                   yield! helloWorldMethodsWithStaticParametersUninstantiated theType
                   yield (theNestedType :> _)
                   yield TypeBuilder.CreateConstructor(theType,(fun _ -> [| |])) :> MemberInfo |]  

        and allMembersOfNestedType = 
            lazy 
                [| for (propertyName, propertyType) in  [for i in 1 .. 2 -> ("StaticProperty"+string i, (if i = 1 then typeof<string> else typeof<int>)) ] do 
                       let prop = TypeBuilder.CreateSyntheticProperty(theNestedType,propertyName,propertyType,isStatic=true) 
                       yield! TypeBuilder.JoinPropertiesIntoMemberInfos [prop]
                   yield TypeBuilder.CreateConstructor(theNestedType,(fun _ -> [| |])) :> MemberInfo
                    |]  

        and theType = TypeBuilder.CreateSimpleType(TypeContainer.Namespace(modul, namespaceName),className,members=allMembers,baseType=baseType, 
                                                   getCustomAttributes=(fun () -> [| mkAllowNullLiteralValueAttributeData(false) |]))

        and theNestedType = TypeBuilder.CreateSimpleType(TypeContainer.Type(theType),"NestedType",members=allMembersOfNestedType, 
                                                         getCustomAttributes=(fun () -> [| mkAllowNullLiteralValueAttributeData(true) |]))

        theType
    let helloWorldType = mkHelloWorldType rootNamespace "HelloWorldType" (typeof<obj>)
    let helloWorldException = mkHelloWorldType rootNamespace "HelloWorldException" (typeof<Exception>)
    let helloWorldTypeNested1 = mkHelloWorldType nestedNamespaceName1 "HelloWorldType" (typeof<obj>)
    let helloWorldTypeNested2 = mkHelloWorldType nestedNamespaceName2 "HelloWorldType" (typeof<obj>)

    // Test provision of an erased class that is a subclass of another erased class
    let mkHelloWorldSubType className baseType =
        let rec allMembers = 
            lazy 
                [| for (propertyName, propertyType) in  [for i in 1 .. 2 -> ("StaticProperty"+string i+"InSubClass", (if i = 1 then typeof<string> else typeof<int>)) ] do 
                       let prop = TypeBuilder.CreateSyntheticProperty(theType,propertyName,propertyType,isStatic=true) 
                       yield! TypeBuilder.JoinPropertiesIntoMemberInfos [prop]
                   yield TypeBuilder.CreateConstructor(theType,(fun _ -> [| |])) :> MemberInfo |]  

        and theType : Type = 
            let container = TypeContainer.Namespace(modul, rootNamespace)
            TypeBuilder.CreateSimpleType(container,className,baseType=baseType,members=allMembers)

        theType

    let helloWorldSubType = mkHelloWorldSubType "HelloWorldSubType" helloWorldType
    let helloWorldSubException = mkHelloWorldSubType "HelloWorldSubException" helloWorldException

    // Test provision of an erased class with static parameters
    let helloWorldTypeWithStaticParameters =
        MemoizationTable(fun (fullName,n) ->
          let rec allMembers = 
              lazy 
                  [| for (propertyName, propertyType) in  [for i in -10 .. n -> ("StaticProperty"+(if i >= 0 then string i else "Minus" + string (-i)), (if i = 1 then typeof<string> else typeof<int>)) ] do 
                         let prop = TypeBuilder.CreateSyntheticProperty(theType,propertyName,propertyType,isStatic=true) 
                         yield! TypeBuilder.JoinPropertiesIntoMemberInfos [prop]
                     yield (theNestedType :> _)
                     // Note these are methods taking static parameters inside a type taking static parameters
                     yield! helloWorldMethodsWithStaticParametersUninstantiated theType
                     yield TypeBuilder.CreateConstructor(theType,(fun _ -> [| |])) :> MemberInfo  |]

          and allMembersOfNestedType = 
              lazy 
                  [| for (propertyName, propertyType) in  [for i in 1 .. 2 -> ("StaticProperty"+string i, (if i = 1 then typeof<string> else typeof<int>)) ] do 
                         let prop = TypeBuilder.CreateSyntheticProperty(theNestedType,propertyName,propertyType,isStatic=true) 
                         yield! TypeBuilder.JoinPropertiesIntoMemberInfos [prop]
                     yield TypeBuilder.CreateConstructor(theNestedType,(fun _ -> [| |])) :> MemberInfo |]  

          and theType = TypeBuilder.CreateSimpleType(TypeContainer.Namespace(modul, rootNamespace),fullName,members=allMembers)
          and theNestedType = TypeBuilder.CreateSimpleType(TypeContainer.Type(theType),"NestedType",members=allMembersOfNestedType)


          theType)

    let helloWorldTypeWithStaticCharParameterUninstantiated = helloWorldTypeWithStaticParameters.Apply("HelloWorldTypeWithStaticCharParameter",1)
    let helloWorldTypeWithStaticBoolParameterUninstantiated = helloWorldTypeWithStaticParameters.Apply("HelloWorldTypeWithStaticBoolParameter",1)
    let helloWorldTypeWithStaticSByteParameterUninstantiated = helloWorldTypeWithStaticParameters.Apply("HelloWorldTypeWithStaticSByteParameter",1)
    let helloWorldTypeWithStaticInt16ParameterUninstantiated = helloWorldTypeWithStaticParameters.Apply("HelloWorldTypeWithStaticInt16Parameter",1)
    let helloWorldTypeWithStaticInt32ParameterUninstantiated = helloWorldTypeWithStaticParameters.Apply("HelloWorldTypeWithStaticInt32Parameter",1)
    let helloWorldTypeWithStaticOptionalInt32ParameterUninstantiated = helloWorldTypeWithStaticParameters.Apply("HelloWorldTypeWithStaticOptionalInt32Parameter",1)
    let helloWorldTypeWithStaticInt64ParameterUninstantiated = helloWorldTypeWithStaticParameters.Apply("HelloWorldTypeWithStaticInt64Parameter",1)
    let helloWorldTypeWithStaticByteParameterUninstantiated = helloWorldTypeWithStaticParameters.Apply("HelloWorldTypeWithStaticByteParameter",1)
    let helloWorldTypeWithStaticUInt16ParameterUninstantiated = helloWorldTypeWithStaticParameters.Apply("HelloWorldTypeWithStaticUInt16Parameter",1)
    let helloWorldTypeWithStaticUInt32ParameterUninstantiated = helloWorldTypeWithStaticParameters.Apply("HelloWorldTypeWithStaticUInt32Parameter",1)
    let helloWorldTypeWithStaticUInt64ParameterUninstantiated = helloWorldTypeWithStaticParameters.Apply("HelloWorldTypeWithStaticUInt64Parameter",1)
    let helloWorldTypeWithStaticDayOfWeekParameterUninstantiated = helloWorldTypeWithStaticParameters.Apply("HelloWorldTypeWithStaticDayOfWeekParameter",1)
    let helloWorldTypeWithStaticStringParameterUninstantiated = helloWorldTypeWithStaticParameters.Apply("HelloWorldTypeWithStaticStringParameter",1)
    let helloWorldTypeWithStaticSingleParameterUninstantiated = helloWorldTypeWithStaticParameters.Apply("HelloWorldTypeWithStaticSingleParameter",1)
    let helloWorldTypeWithStaticDoubleParameterUninstantiated = helloWorldTypeWithStaticParameters.Apply("HelloWorldTypeWithStaticDoubleParameter",1)
    let helloWorldTypeWithStaticDecimalParameterUninstantiated = helloWorldTypeWithStaticParameters.Apply("HelloWorldTypeWithStaticDecimalParameter",1)


    let mkNestedNamespace (name,typ) = 
        { new IProvidedNamespace with
             member this.NamespaceName = name
             member this.GetTypes() = [| typ |]
             member this.GetNestedNamespaces() = [| |]
             member this.ResolveTypeName typeName = if typeName = typ.Name then typ else null }

    let nestedNamespace1 = mkNestedNamespace (nestedNamespaceName1,helloWorldTypeNested1)
    let nestedNamespace2 = mkNestedNamespace (nestedNamespaceName2,helloWorldTypeNested2)

    let types = 
        [| helloWorldType; 
           helloWorldSubType; helloWorldException; helloWorldSubException;
           helloWorldTypeWithStaticStringParameterUninstantiated
           helloWorldTypeWithStaticSByteParameterUninstantiated
           helloWorldTypeWithStaticInt16ParameterUninstantiated
           helloWorldTypeWithStaticOptionalInt32ParameterUninstantiated
           helloWorldTypeWithStaticInt32ParameterUninstantiated
           helloWorldTypeWithStaticInt64ParameterUninstantiated
           helloWorldTypeWithStaticByteParameterUninstantiated
           helloWorldTypeWithStaticUInt16ParameterUninstantiated
           helloWorldTypeWithStaticUInt32ParameterUninstantiated
           helloWorldTypeWithStaticUInt64ParameterUninstantiated
           helloWorldTypeWithStaticDayOfWeekParameterUninstantiated
           helloWorldTypeWithStaticDecimalParameterUninstantiated
           helloWorldTypeWithStaticSingleParameterUninstantiated
           helloWorldTypeWithStaticDoubleParameterUninstantiated
           helloWorldTypeWithStaticBoolParameterUninstantiated
           helloWorldTypeWithStaticCharParameterUninstantiated |]

    let invalidation = new Event<System.EventHandler,_>()

    // This implements both get_StaticProperty1 and get_StaticProperty2
    static member GetPropertyByName(propertyName:string) : 'T = 
        match propertyName with 
        | StartsWith "StaticProperty1" -> "You got a static property" |> box |> unbox
        | StartsWith "StaticProperty2" -> 42 |> box |> unbox
        | StartsWith "StaticPropertyMinus" as nm -> 40 - (int (nm.Replace("StaticPropertyMinus",""))) |> box |> unbox
        | StartsWith "StaticProperty" as nm -> int (nm.Replace("StaticProperty","")) + 40 |> box |> unbox
        | _ -> failwith "unexpected property"

    // This implements OneOptionalParameter and TwoOptionalParameters
    static member OneOptionalParameter(arg1:int) : int = arg1 + 1
    static member TwoOptionalParameters(arg1:int,arg2:int) : int = arg1 + arg2 + 1
    
    // This implements UsesByRef
    static member UsesByRefImpl(arg1:byref<int>) : int = arg1 + 1
    static member UsesByRefAsOutParameterImpl(arg1:int, arg2:byref<int>) : int = arg2 <- arg1 + 1; 17

    // This implements op_Addition
    static member Add(arg1:obj, arg2:obj) : obj = arg1
    // This implements op_Subtraction
    static member Sub(arg1:obj, arg2:obj) : obj = arg2
    static member ReturnsVoid() : unit = ()
    
    interface IProvidedNamespace with
        member this.NamespaceName = rootNamespace
        member this.GetTypes() = types
        member __.ResolveTypeName typeName : System.Type = 
            match typeName with
            | "HelloWorldType" -> helloWorldType
            | "HelloWorldException" -> helloWorldException
            | "HelloWorldSubException" -> helloWorldSubException
            | "HelloWorldSubType" -> helloWorldSubType
            | "HelloWorldTypeWithStaticStringParameter" -> helloWorldTypeWithStaticStringParameterUninstantiated
            | "HelloWorldTypeWithStaticSByteParameter" -> helloWorldTypeWithStaticSByteParameterUninstantiated
            | "HelloWorldTypeWithStaticInt16Parameter" -> helloWorldTypeWithStaticInt16ParameterUninstantiated
            | "HelloWorldTypeWithStaticInt32Parameter" -> helloWorldTypeWithStaticInt32ParameterUninstantiated
            | "HelloWorldTypeWithStaticOptionalInt32Parameter" -> helloWorldTypeWithStaticOptionalInt32ParameterUninstantiated
            | "HelloWorldTypeWithStaticInt64Parameter" -> helloWorldTypeWithStaticInt64ParameterUninstantiated
            | "HelloWorldTypeWithStaticByteParameter" -> helloWorldTypeWithStaticByteParameterUninstantiated
            | "HelloWorldTypeWithStaticUInt16Parameter" -> helloWorldTypeWithStaticUInt16ParameterUninstantiated
            | "HelloWorldTypeWithStaticUInt32Parameter" -> helloWorldTypeWithStaticUInt32ParameterUninstantiated
            | "HelloWorldTypeWithStaticUInt64Parameter" -> helloWorldTypeWithStaticUInt64ParameterUninstantiated
            | "HelloWorldTypeWithStaticDayOfWeekParameter" -> helloWorldTypeWithStaticDayOfWeekParameterUninstantiated
            | "HelloWorldTypeWithStaticDecimalParameter" -> helloWorldTypeWithStaticDecimalParameterUninstantiated
            | "HelloWorldTypeWithStaticSingleParameter" -> helloWorldTypeWithStaticSingleParameterUninstantiated
            | "HelloWorldTypeWithStaticDoubleParameter" -> helloWorldTypeWithStaticDoubleParameterUninstantiated
            | "HelloWorldTypeWithStaticBoolParameter" -> helloWorldTypeWithStaticBoolParameterUninstantiated
            | "HelloWorldTypeWithStaticCharParameter" -> helloWorldTypeWithStaticCharParameterUninstantiated
            | _ -> null
       
        member this.GetNestedNamespaces() = [| nestedNamespace1; nestedNamespace2 |]

    interface IDisposable with
        member __.Dispose() = ()
    interface ITypeProvider with
        member this.GetNamespaces() = [| this |]

        member this.GetStaticParameters(typeWithoutArguments) = 
         [| match typeWithoutArguments.Name with
            |   "HelloWorldTypeWithStaticSByteParameter" -> yield TypeBuilder.CreateStaticParameter("Count",typeof<sbyte>, 0) 
            |   "HelloWorldTypeWithStaticInt16Parameter" -> yield TypeBuilder.CreateStaticParameter("Count",typeof<int16>, 0) 
            |   "HelloWorldTypeWithStaticInt32Parameter" -> yield TypeBuilder.CreateStaticParameter("Count",typeof<int32>, 0) 
            |   "HelloWorldTypeWithStaticOptionalInt32Parameter" -> 
                   yield TypeBuilder.CreateStaticParameter("Count",typeof<int32>, 0, defaultValue=42) 
#if ADD_AN_OPTIONAL_STATIC_PARAMETER
                   yield TypeBuilder.CreateStaticParameter("ExtraParameter",typeof<int32>, 1, defaultValue=43)             
#endif
            |   "HelloWorldTypeWithStaticInt64Parameter" -> yield TypeBuilder.CreateStaticParameter("Count",typeof<int64>, 0) 
            |   "HelloWorldTypeWithStaticByteParameter" -> yield TypeBuilder.CreateStaticParameter("Count",typeof<byte>, 0) 
            |   "HelloWorldTypeWithStaticUInt16Parameter" -> yield TypeBuilder.CreateStaticParameter("Count",typeof<uint16>, 0) 
            |   "HelloWorldTypeWithStaticUInt32Parameter" -> yield TypeBuilder.CreateStaticParameter("Count",typeof<uint32>, 0) 
            |   "HelloWorldTypeWithStaticUInt64Parameter" -> yield TypeBuilder.CreateStaticParameter("Count",typeof<uint64>, 0) 
            |   "HelloWorldTypeWithStaticDayOfWeekParameter" -> yield TypeBuilder.CreateStaticParameter("Count",typeof<System.DayOfWeek>, 0) 
            |   "HelloWorldTypeWithStaticStringParameter" -> yield TypeBuilder.CreateStaticParameter("Count",typeof<string>, 0) 
            |   "HelloWorldTypeWithStaticDecimalParameter" -> yield TypeBuilder.CreateStaticParameter("Count",typeof<decimal>, 0) 
            |   "HelloWorldTypeWithStaticSingleParameter" -> yield TypeBuilder.CreateStaticParameter("Count",typeof<single>, 0) 
            |   "HelloWorldTypeWithStaticDoubleParameter" -> yield TypeBuilder.CreateStaticParameter("Count",typeof<double>, 0) 
            |   "HelloWorldTypeWithStaticBoolParameter" -> yield TypeBuilder.CreateStaticParameter("Flag",typeof<bool>, 0) 
            |   "HelloWorldTypeWithStaticCharParameter" -> yield TypeBuilder.CreateStaticParameter("Count",typeof<char>, 0) 
            |   _ -> ()
         |]
        member this.ApplyStaticArguments(typeWithoutArguments, typeNameWithArguments, staticArguments) =
            let typeNameWithArguments = typeNameWithArguments.[typeNameWithArguments.Length-1]
            match typeWithoutArguments.Name with
            | "HelloWorldType" -> 
                if staticArguments.Length <> 0 then failwith "this provided type does not accept static parameters" 
                helloWorldType
            | "HelloWorldTypeWithStaticSByteParameter" -> 
                if staticArguments.Length <> 1 then failwith "this provided type accepts one static parameter" 
                helloWorldTypeWithStaticParameters.Apply (typeNameWithArguments, staticArguments.[0] :?> sbyte |> int)
            | "HelloWorldTypeWithStaticInt16Parameter" -> 
                if staticArguments.Length <> 1 then failwith "this provided type accepts one static parameter" 
                helloWorldTypeWithStaticParameters.Apply (typeNameWithArguments, staticArguments.[0] :?> int16 |> int)
            | "HelloWorldTypeWithStaticInt32Parameter" -> 
                if staticArguments.Length <> 1 then failwith "this provided type accepts one static parameter" 
                helloWorldTypeWithStaticParameters.Apply (typeNameWithArguments, staticArguments.[0] :?> int)
            | "HelloWorldTypeWithStaticInt64Parameter" -> 
                if staticArguments.Length <> 1 then failwith "this provided type accepts one static parameter" 
                helloWorldTypeWithStaticParameters.Apply (typeNameWithArguments, staticArguments.[0] :?> int64 |> int)
            | "HelloWorldTypeWithStaticByteParameter" -> 
                if staticArguments.Length <> 1 then failwith "this provided type accepts one static parameter" 
                helloWorldTypeWithStaticParameters.Apply (typeNameWithArguments, staticArguments.[0] :?> byte |> int)
            | "HelloWorldTypeWithStaticUInt16Parameter" -> 
                if staticArguments.Length <> 1 then failwith "this provided type accepts one static parameter" 
                helloWorldTypeWithStaticParameters.Apply (typeNameWithArguments, staticArguments.[0] :?> uint16 |> int)
            | "HelloWorldTypeWithStaticUInt32Parameter" -> 
                if staticArguments.Length <> 1 then failwith "this provided type accepts one static parameter" 
                helloWorldTypeWithStaticParameters.Apply (typeNameWithArguments, staticArguments.[0] :?> uint32 |> int)
            | "HelloWorldTypeWithStaticUInt64Parameter" -> 
                if staticArguments.Length <> 1 then failwith "this provided type accepts one static parameter" 
                helloWorldTypeWithStaticParameters.Apply (typeNameWithArguments, staticArguments.[0] :?> uint64 |> int)
            | "HelloWorldTypeWithStaticDayOfWeekParameter" -> 
                if staticArguments.Length <> 1 then failwith "this provided type accepts one static parameter" 
                helloWorldTypeWithStaticParameters.Apply (typeNameWithArguments, staticArguments.[0] :?> System.DayOfWeek |> int)
            | "HelloWorldTypeWithStaticBoolParameter" -> 
                if staticArguments.Length <> 1 then failwith "this provided type accepts one static parameter" 
                helloWorldTypeWithStaticParameters.Apply (typeNameWithArguments, if (staticArguments.[0] :?> bool) then 1 else 0)
            | "HelloWorldTypeWithStaticStringParameter" -> 
                if staticArguments.Length <> 1 then failwith "this provided type accepts one static parameter" 
                helloWorldTypeWithStaticParameters.Apply (typeNameWithArguments, staticArguments.[0] :?> string |> String.length)
            | "HelloWorldTypeWithStaticDecimalParameter" -> 
                if staticArguments.Length <> 1 then failwith "this provided type accepts one static parameter" 
                helloWorldTypeWithStaticParameters.Apply (typeNameWithArguments, staticArguments.[0] :?> decimal |> int)
            | "HelloWorldTypeWithStaticCharParameter" -> 
                if staticArguments.Length <> 1 then failwith "this provided type accepts one static parameter" 
                helloWorldTypeWithStaticParameters.Apply (typeNameWithArguments, staticArguments.[0] :?> char |> int)
            | "HelloWorldTypeWithStaticSingleParameter" -> 
                if staticArguments.Length <> 1 then failwith "this provided type accepts one static parameter" 
                helloWorldTypeWithStaticParameters.Apply (typeNameWithArguments, staticArguments.[0] :?> single |> int)
            | "HelloWorldTypeWithStaticDoubleParameter" -> 
                if staticArguments.Length <> 1 then failwith "this provided type accepts one static parameter" 
                helloWorldTypeWithStaticParameters.Apply (typeNameWithArguments, staticArguments.[0] :?> double |> int)

            | "HelloWorldTypeWithStaticOptionalInt32Parameter" -> 
#if ADD_AN_OPTIONAL_STATIC_PARAMETER
                if staticArguments.Length <> 2 then failwith "this provided type accepts two static parameters" 
#else
                if staticArguments.Length <> 1 then failwith "this provided type accepts zero, one static parameter" 
#endif
                helloWorldTypeWithStaticParameters.Apply (typeNameWithArguments, staticArguments.[0] :?> int)

            | nm -> failwith (sprintf "ApplyStaticArguments %s" nm)

        member __.GetInvokerExpression(syntheticMethodBase:MethodBase, parameterExpressions:Quotations.Expr[]) = 
            let expr = 
                if syntheticMethodBase.IsConstructor then 
                     Quotations.Expr.NewObject(typeof<obj>.GetConstructor [| |], [])
                elif syntheticMethodBase.Name = "ReturnsNullSeqString" then 
                     Quotations.Expr.Value(null,typeof<seq<string>>)
                elif syntheticMethodBase.Name = "ReturnsNullString" then 
                     Quotations.Expr.Value(null,typeof<string>)
                elif syntheticMethodBase.Name = "ReturnsMondayDayOfWeek" then 
                     Quotations.Expr.Value(System.DayOfWeek.Monday,typeof<System.DayOfWeek>)
                elif syntheticMethodBase.Name = "ReturnsDefaultDayOfWeek" then 
                     Quotations.Expr.DefaultValue(typeof<System.DayOfWeek>)
                elif syntheticMethodBase.Name = "ReturnsDefaultDateTime" then 
                     Quotations.Expr.DefaultValue(typeof<System.DateTime>)
                elif syntheticMethodBase.Name = "ReturnsUnit" then 
                     Quotations.Expr.Value(null, typeof<unit>)
                elif syntheticMethodBase.Name = "GetItem1OfTuple2OfInt" then 
                     Quotations.Expr.TupleGet(parameterExpressions.[0],0)
                elif syntheticMethodBase.Name = "GetItem2OfTuple2OfInt" then 
                     Quotations.Expr.TupleGet(parameterExpressions.[0],1)
                elif syntheticMethodBase.Name = "GetItem1OfTuple7OfInt" then 
                     Quotations.Expr.TupleGet(parameterExpressions.[0],0)
                elif syntheticMethodBase.Name = "GetItem7OfTuple7OfInt" then 
                     Quotations.Expr.TupleGet(parameterExpressions.[0],6)

                elif syntheticMethodBase.Name = "GetItem1OfTuple8OfInt" then 
                     Quotations.Expr.TupleGet(parameterExpressions.[0],0)
                elif syntheticMethodBase.Name = "GetItem7OfTuple8OfInt" then 
                     Quotations.Expr.TupleGet(parameterExpressions.[0],6)
                elif syntheticMethodBase.Name = "GetItem8OfTuple8OfInt" then 
                     Quotations.Expr.TupleGet(parameterExpressions.[0],7)

                elif syntheticMethodBase.Name = "GetItem1OfTuple9OfInt" then 
                     Quotations.Expr.TupleGet(parameterExpressions.[0],0)
                elif syntheticMethodBase.Name = "GetItem7OfTuple9OfInt" then 
                     Quotations.Expr.TupleGet(parameterExpressions.[0],6)
                elif syntheticMethodBase.Name = "GetItem8OfTuple9OfInt" then 
                     Quotations.Expr.TupleGet(parameterExpressions.[0],7)
                elif syntheticMethodBase.Name = "GetItem9OfTuple9OfInt" then 
                     Quotations.Expr.TupleGet(parameterExpressions.[0],8)

                elif syntheticMethodBase.Name = "ReturnsNewArray" then 
                     Quotations.Expr.NewArray (typeof<int>, [ Quotations.Expr.Value(3,typeof<int>); Quotations.Expr.Value(6,typeof<int>) ])
                elif syntheticMethodBase.Name = "ReturnsEmptyNewArray" then 
                     Quotations.Expr.NewArray (typeof<int>, [  ])
                elif syntheticMethodBase.Name = "IfThenElseUnitUnit" then 
                     <@@ if (%%(parameterExpressions.[0]) : bool) then Provider.ReturnsVoid() else failwith "" @@>
                elif syntheticMethodBase.Name = "IfThenElseVoidUnit" then 
                     <@@ if (%%(parameterExpressions.[0]) : bool) then System.Console.WriteLine "hello" else failwith "" @@>
                elif syntheticMethodBase.Name = "SequentialSmokeTest" then 
                     <@@ System.Console.WriteLine "hello"; 1 @@>
                elif syntheticMethodBase.Name = "SequentialSmokeTest2" then 
                     <@@ System.Console.WriteLine "hello"; System.Console.WriteLine "world"; @@>
                elif syntheticMethodBase.Name = "LambdaSmokeTest" then 
                     <@@ (fun x -> x + 1) @@>
                elif syntheticMethodBase.Name = "ArrayGetSmokeTest" then 
                     <@@ (%%(parameterExpressions.[0]) : int[]).[0] @@>
                elif syntheticMethodBase.Name = "ArraySetSmokeTest" then 
                     <@@ (%%(parameterExpressions.[0]) : int[]).[0] <- 3 @@>
                elif syntheticMethodBase.Name = "VarSetSmokeTest" then 
                     <@@ let mutable x = 3 in x <- x + 1; x @@>
                elif syntheticMethodBase.Name = "Array2DGetSmokeTest" then 
                     <@@ (%%(parameterExpressions.[0]) : int[,]).[0,0] @@>
                elif syntheticMethodBase.Name = "TakesParamArray" then 
                     <@@ (%%(parameterExpressions.[1]) : obj[]).Length @@>
                elif syntheticMethodBase.Name = "IsMarkedConditional1" then 
                     <@@ () @@>
                elif syntheticMethodBase.Name = "IsMarkedObsolete1" then 
                     <@@ 1 @@>
                elif syntheticMethodBase.Name = "IsMarkedObsolete2" then 
                     <@@ 2 @@>
                elif syntheticMethodBase.Name = "Array2DSetSmokeTest" then 
                     <@@ (%%(parameterExpressions.[0]) : int[,]).[0,0] <- 3 @@>
                elif syntheticMethodBase.Name = "LambdaSmokeTest2" then 
                     <@@ (fun () -> System.Console.WriteLine "hello") @@>
                elif syntheticMethodBase.Name = "LambdaSmokeTest3" then 
                     <@@ (fun x y -> x + y) @@>
                elif syntheticMethodBase.Name.StartsWith("HelloWorldInstanceMethodWithStaticCharParameter") then 
                    <@@ (%%(parameterExpressions.[1]) : char) @@>
                elif syntheticMethodBase.Name.StartsWith("HelloWorldInstanceMethodWithStaticBoolParameter") then 
                    <@@ (%%(parameterExpressions.[1]) : bool) @@>
                elif syntheticMethodBase.Name.StartsWith("HelloWorldInstanceMethodWithStaticSByteParameter") then 
                    <@@ (%%(parameterExpressions.[1]) : sbyte) @@>
                elif syntheticMethodBase.Name.StartsWith("HelloWorldInstanceMethodWithStaticInt16Parameter") then 
                    <@@ (%%(parameterExpressions.[1]) : int16) @@>
                elif syntheticMethodBase.Name.StartsWith("HelloWorldInstanceMethodWithStaticInt32Parameter") then 
                    <@@ (%%(parameterExpressions.[1]) : int32) @@>
                elif syntheticMethodBase.Name.StartsWith("HelloWorldInstanceMethodWithStaticUInt16Parameter") then 
                    <@@ (%%(parameterExpressions.[1]) : int32) @@>
                elif syntheticMethodBase.Name.StartsWith("HelloWorldInstanceMethodWithStaticOptionalInt32Parameter") then 
                    <@@ (%%(parameterExpressions.[1]) : int32) @@>
                elif syntheticMethodBase.Name.StartsWith("HelloWorldInstanceMethodWithStaticInt64Parameter") then 
                    <@@ (%%(parameterExpressions.[1]) : int64) @@>
                elif syntheticMethodBase.Name.StartsWith("HelloWorldInstanceMethodWithStaticByteParameter") then 
                    <@@ (%%(parameterExpressions.[1]) : byte) @@>
                elif syntheticMethodBase.Name.StartsWith("HelloWorldStaticMethodWithStaticUInt32Parameter") then 
                    <@@ (%%(parameterExpressions.[0]) : uint32) @@>
                elif syntheticMethodBase.Name.StartsWith("HelloWorldStaticMethodWithStaticUInt64Parameter") then 
                    <@@ (%%(parameterExpressions.[0]) : uint64) @@>
                elif syntheticMethodBase.Name.StartsWith("HelloWorldInstanceMethodWithStaticDayOfWeekParameter") then 
                    <@@ (%%(parameterExpressions.[1]) : System.DayOfWeek) @@>
                elif syntheticMethodBase.Name.StartsWith("HelloWorldInstanceMethodWithStaticStringParameter") then 
                    <@@ (%%(parameterExpressions.[1]) : string) @@>
                elif syntheticMethodBase.Name.StartsWith("HelloWorldInstanceMethodWithStaticSingleParameter") then 
                    <@@ (%%(parameterExpressions.[1]) : single) @@>
                elif syntheticMethodBase.Name.StartsWith("HelloWorldInstanceMethodWithStaticDoubleParameter") then 
                    <@@ (%%(parameterExpressions.[1]) : double) @@>
                elif syntheticMethodBase.Name.StartsWith("HelloWorldInstanceMethodWithStaticDecimalParameter") then 
                    <@@ (%%(parameterExpressions.[1]) : decimal) @@>                     
                elif syntheticMethodBase.Name = "CallIntrinsics" then 
                     <@@ [ ((true & false) |> box);
                           ((true && false) |> box);
                           ((true || false) |> box);
                           ((true or false) |> box);
                           ((compare true false) |> box);
                           ((true = false) |> box);
                           ((3 +? System.Nullable 4) |> box);  // should be inlined by the compiler
                           ((System.Nullable 3 ?+ 4) |> box); // should be inlined by the compiler
                           ((3 *? System.Nullable 4) |> box); // should be inlined by the compiler
                           ((System.Nullable 3 ?* 4) |> box); // should be inlined by the compiler
                           ((true =? System.Nullable false) |> box);
                           ((System.Nullable true ?= false) |> box);
                           ((System.Nullable true ?=? System.Nullable false) |> box);
                           ((true <> false) |> box);
                           ((1 < 2) |> box);
                           ((1 <= 2) |> box);
                           (1 > 2) |> box;
                           (1 >= 2) |> box;
                           LanguagePrimitives.GenericComparisonWithComparer LanguagePrimitives.GenericComparer 3 4 |> box;
                           LanguagePrimitives.HashCompare.FastHashTuple2 LanguagePrimitives.GenericEqualityComparer (1,2) |> box;
                           LanguagePrimitives.HashCompare.FastHashTuple3 LanguagePrimitives.GenericEqualityComparer (1,2,3) |> box;
                           LanguagePrimitives.HashCompare.FastHashTuple4 LanguagePrimitives.GenericEqualityComparer (1,2,3,4) |> box;
                           LanguagePrimitives.HashCompare.FastHashTuple5 LanguagePrimitives.GenericEqualityComparer (1,2,3,4,5) |> box;

                           LanguagePrimitives.HashCompare.FastEqualsTuple2 LanguagePrimitives.GenericEqualityComparer (1,2)  (1,2) |> box;
                           LanguagePrimitives.HashCompare.FastEqualsTuple3 LanguagePrimitives.GenericEqualityComparer (1,2,3)  (1,2,3) |> box;
                           LanguagePrimitives.HashCompare.FastEqualsTuple4 LanguagePrimitives.GenericEqualityComparer (1,2,3,4)  (1,2,3,4) |> box;
                           LanguagePrimitives.HashCompare.FastEqualsTuple5 LanguagePrimitives.GenericEqualityComparer (1,2,3,4,5)  (1,2,3,4,5) |> box;

                           LanguagePrimitives.HashCompare.FastCompareTuple2 LanguagePrimitives.GenericComparer (1,2)  (1,2) |> box;
                           LanguagePrimitives.HashCompare.FastCompareTuple3 LanguagePrimitives.GenericComparer (1,2,3)  (1,2,3) |> box;
                           LanguagePrimitives.HashCompare.FastCompareTuple4 LanguagePrimitives.GenericComparer (1,2,3,4)  (1,2,3,4) |> box;
                           LanguagePrimitives.HashCompare.FastCompareTuple5 LanguagePrimitives.GenericComparer (1,2,3,4,5)  (1,2,3,4,5) |> box;
                           (3 ||| 4 |> box);
                           (3 &&& 4 |> box);
                           (3 ^^^ 4 |> box); 
                           (~~~3 |> box); 
                           ((3 <<< 1) |> box); 
                           ((3 >>> 1) |> box); 
                           ((3 + 1) |> box); 
                           ((3 - 1) |> box); 
                           ((3 * 1) |> box); 
                           ((+(3)) |> box); 
                           (List.map id [1;2;3] |> box); 
                           (List.sum [1;2;3] |> box);  // should be inlined by the compiler
                           (List.sumBy id [1;2;3] |> box);  // should be inlined by the compiler
                           (List.average [1.0;2.0;3.0] |> box);  // should be inlined by the compiler
                           (List.averageBy id [1.0;2.0;3.0] |> box);  // should be inlined by the compiler
                           (Array.map id [|1;2;3|] |> box); 
                           (Array.sum [|1;2;3|] |> box);      // should be inlined by the compiler
                           (Array.sumBy id [|1;2;3|] |> box);   // should be inlined by the compiler
                           (Array.average [|1.0;2.0;3.0|] |> box);  // should be inlined by the compiler
                           (Array.averageBy id [|1.0;2.0;3.0|] |> box);  // should be inlined by the compiler
                           (Seq.map id [1;2;3] |> box); 
                           (Seq.sum [1;2;3] |> box); // should be inlined by the compiler
                           (Seq.sumBy id [1;2;3] |> box); // should be inlined by the compiler
                           (Seq.average [1.0;2.0;3.0] |> box); // should be inlined by the compiler
                           (Seq.averageBy id [1.0;2.0;3.0] |> box); // should be inlined by the compiler
                           ((-(3)) |> box); 
                           ((3.0 + 1.0) |> box); 
                           ((3.0 - 1.0) |> box); 
                           ((3.0 * 1.0) |> box); 
                           ((+(3.0)) |> box); 
                           ((-(3.0)) |> box); 
                           ((3.0f + 1.0f) |> box); 
                           ((3.0f - 1.0f) |> box); 
                           ((3.0f * 1.0f) |> box); 
                           ((+(3.0f)) |> box); 
                           ((-(3.0f)) |> box); 
                           (Operators.Checked.(+) 3 4  |> box); 
                           Operators.Checked.(-) 3 4  |> box; 
                           Operators.Checked.(*) 3 4  |> box; 
                           (Operators.Checked.(~-) 3  |> box); 
                           (not true  |> box); 
                           (typeof<int> |> box); 
                           (sizeof<int> |> box); 
                           (Unchecked.defaultof<int> |> box); 
                           (typedefof<int> |> box); 
                           (enum<System.DayOfWeek>(3)  |> box); 
                           ((..) 3 4   |> box); 
                           (OperatorIntrinsics.RangeInt32 3 4   |> box); 
                           (LanguagePrimitives.IntrinsicFunctions.GetArray [|1;2|] 0   |> box); 
                           (LanguagePrimitives.IntrinsicFunctions.GetArray2D (array2D [[1;2]]) 0 0   |> box); 
                           (LanguagePrimitives.IntrinsicFunctions.GetArray3D (Array3D.init 1 1 1 (fun _ _ _ -> 0) ) 0 0 0  |> box); 
                           (LanguagePrimitives.IntrinsicFunctions.GetArray4D (Array4D.init 1 1 1 1 (fun _ _ _ _ -> 0) )  0 0 0 0 |> box); 
                           (Seq.collect (fun x -> [x]) [1;2]   |> box); 
                           (Seq.delay (fun _ -> Seq.empty) |> box); 
                           (Seq.append [1] [2] |> box); 
                           (seq { for i in 0 .. 10 -> (i,i*i) } |> box); 
                           ([| for i in 0 .. 10 -> (i,i*i) |] |> box); 
                           ([ for i in 0 .. 10 -> (i,i*i) ] |> box); 
                           (sprintf "%d" 1 |> box); 
                           ((lazy (3 + 4)) |> box);
                           (1.0M |> box);
                           // These are not supported as provided expressions
                           //(<@ 1+2 @> |> box);
                           //(<@@ 1+2 @@> |> box);
                           ]
                         @@>

                elif syntheticMethodBase.Name = "NewUnionCaseSmokeTest1" then 
                     <@@ [] : list<int> @@>
                elif syntheticMethodBase.Name = "NewUnionCaseSmokeTest2" then 
                     <@@ [1] : list<int> @@>
                elif syntheticMethodBase.Name = "NewUnionCaseSmokeTest3" then 
                     <@@ None : option<int> @@>
                elif syntheticMethodBase.Name = "NewUnionCaseSmokeTest4" then 
                     <@@ Some 3 : option<int> @@>
                elif syntheticMethodBase.Name = "NewUnionCaseSmokeTest5" then 
                     <@@ Choice1Of2 3 : Choice<int,string> @@>
                elif syntheticMethodBase.Name = "NewRecordSmokeTest1" then 
                     <@@ { contents = 3 } @@>
                elif syntheticMethodBase.Name = "WhileLoopSmokeTest1" then 
                     <@@ let x = ref 0 in while !x < 2 do System.Console.WriteLine "hello"; incr x done; !x @@>
                elif syntheticMethodBase.Name = "ForIntegerRangeLoopSmokeTest1" then 
                     <@@ let x = ref 0 in for i = 0 to 10 do incr x done; !x @@>
                elif syntheticMethodBase.Name = "NewDelegateSmokeTest1" then 
                     <@@ new System.Func<int,int>(fun x -> x + 1) @@>
                elif syntheticMethodBase.Name = "NewDelegateSmokeTest2" then 
                     <@@ new System.Func<int,int,int>(fun x y -> x + y) @@>
                elif syntheticMethodBase.Name = "StructProperty1" then 
                     <@@ let s = System.TimeSpan(1,0,0,0) in s.TotalSeconds @@>
                elif syntheticMethodBase.Name = "StructProperty2" then 
                     <@@ let mutable s = System.TimeSpan(1,0,0,0) in s.TotalSeconds @@>
                elif syntheticMethodBase.Name = "StructProperty3" then 
                     <@@ System.TimeSpan(1,0,0,0).TotalSeconds @@>
                elif syntheticMethodBase.Name = "StructMethod1" then 
                     <@@ System.TimeSpan(1,0,0,0).Add(System.TimeSpan(1,0,0,0)).TotalSeconds @@>
                elif syntheticMethodBase.Name = "StructMethod2" then 
                     <@@ let s = System.TimeSpan(1,0,0,0) in let s2 = s.Add(System.TimeSpan(1,0,0,0)) in s2.TotalSeconds @@>
                elif syntheticMethodBase.Name = "NewObjectGeneric1" then 
                     <@@ new System.Collections.Generic.Dictionary<int,int>() @@>
                elif syntheticMethodBase.Name = "NewObjectGeneric2" then 
                     <@@ new System.Collections.Generic.Dictionary<int,int>(null: IEqualityComparer<int>) @@>
                elif syntheticMethodBase.Name = "NewObjectGeneric3" then 
                     <@@ new System.Collections.Generic.Dictionary<int,int>(HashIdentity.Structural<int>) @@>
                elif syntheticMethodBase.Name = "UnionCaseTest1" then 
                     <@@ match (%%(parameterExpressions.[0]) : list<int>) with _ :: _ -> true | [] -> false @@>
                elif syntheticMethodBase.Name = "UnionCaseTest2" then 
                     <@@ match (%%(parameterExpressions.[0]) : option<int>) with Some x -> true | None -> false @@>

                // Note, in F# 3.0 the compiled form of pattern matching over union cases can't be included
                // in a provided expression. These should go into a runtime library.
                // 
                // You get: test.fsx(231,10): error FS1109: A reference to the type 'Microsoft.FSharp.Core.FSharpChoice`2.Choice2Of2' in assembly 'FSharp.Core' was found, but the type could not be found in that assembly
                //
                // elif syntheticMethodBase.Name = "UnionCaseTest3" then 
                //     <@@ match (%%(parameterExpressions.[0]) : Choice<int,string>) with Choice1Of2 x -> true | Choice2Of2 x -> false @@>

                elif syntheticMethodBase.Name = "TryFinallySmokeTest" then 
                     <@@ try 3 finally System.Console.WriteLine "hello" @@>
                elif syntheticMethodBase.Name = "TryWithSmokeTest" then 
                     <@@ try failwith "fail" with _ -> 3 @@>
                elif syntheticMethodBase.Name = "TryWithSmokeTest2" then 
                     <@@ try invalidArg "fail" "fail2" with :? System.ArgumentException as e -> e.Message.StartsWith "fail2" @@>
                elif syntheticMethodBase.Name = "TryWithSmokeTest3" then 
                     <@@ try invalidArg "fail" "fail2" with :? System.ArithmeticException | :? System.ArgumentException as e   -> e.Message.StartsWith "fail2" @@>
                elif syntheticMethodBase.Name = "ReturnsTuple2OfInt" then 
                     Quotations.Expr.NewTuple [ Quotations.Expr.Value(3,typeof<int>); 
                                                Quotations.Expr.Value(6,typeof<int>) ]
                elif syntheticMethodBase.Name = "ReturnsTuple3OfInt" then 
                     Quotations.Expr.NewTuple [ Quotations.Expr.Value(3,typeof<int>); 
                                                Quotations.Expr.Value(6,typeof<int>); 
                                                Quotations.Expr.Value(9,typeof<int>); ]
                elif syntheticMethodBase.Name = "ReturnsTuple4OfInt" then 
                     Quotations.Expr.NewTuple [ Quotations.Expr.Value(3,typeof<int>); 
                                                Quotations.Expr.Value(6,typeof<int>); 
                                                Quotations.Expr.Value(9,typeof<int>); 
                                                Quotations.Expr.Value(12,typeof<int>); ]
                elif syntheticMethodBase.Name = "ReturnsTuple5OfInt" then 
                     Quotations.Expr.NewTuple [ Quotations.Expr.Value(3,typeof<int>); 
                                                Quotations.Expr.Value(6,typeof<int>); 
                                                Quotations.Expr.Value(9,typeof<int>); 
                                                Quotations.Expr.Value(12,typeof<int>); 
                                                Quotations.Expr.Value(15,typeof<int>); ]
                elif syntheticMethodBase.Name = "ReturnsTuple6OfInt" then 
                     Quotations.Expr.NewTuple [ Quotations.Expr.Value(3,typeof<int>); 
                                                Quotations.Expr.Value(6,typeof<int>); 
                                                Quotations.Expr.Value(9,typeof<int>); 
                                                Quotations.Expr.Value(12,typeof<int>); 
                                                Quotations.Expr.Value(15,typeof<int>); 
                                                Quotations.Expr.Value(18,typeof<int>); ]
                elif syntheticMethodBase.Name = "ReturnsTuple7OfInt" then 
                     Quotations.Expr.NewTuple [ Quotations.Expr.Value(3,typeof<int>); 
                                                Quotations.Expr.Value(6,typeof<int>); 
                                                Quotations.Expr.Value(9,typeof<int>); 
                                                Quotations.Expr.Value(12,typeof<int>); 
                                                Quotations.Expr.Value(15,typeof<int>); 
                                                Quotations.Expr.Value(18,typeof<int>); 
                                                Quotations.Expr.Value(21,typeof<int>); ]
                elif syntheticMethodBase.Name = "ReturnsTuple8OfInt" then 
                     Quotations.Expr.NewTuple [ Quotations.Expr.Value(3,typeof<int>); 
                                                Quotations.Expr.Value(6,typeof<int>); 
                                                Quotations.Expr.Value(9,typeof<int>); 
                                                Quotations.Expr.Value(12,typeof<int>); 
                                                Quotations.Expr.Value(15,typeof<int>); 
                                                Quotations.Expr.Value(18,typeof<int>); 
                                                Quotations.Expr.Value(21,typeof<int>); 
                                                Quotations.Expr.Value(24,typeof<int>); ]
                elif syntheticMethodBase.Name = "ReturnsTuple9OfInt" then 
                     Quotations.Expr.NewTuple [ Quotations.Expr.Value(3,typeof<int>); 
                                                Quotations.Expr.Value(6,typeof<int>); 
                                                Quotations.Expr.Value(9,typeof<int>); 
                                                Quotations.Expr.Value(12,typeof<int>); 
                                                Quotations.Expr.Value(15,typeof<int>); 
                                                Quotations.Expr.Value(18,typeof<int>); 
                                                Quotations.Expr.Value(21,typeof<int>); 
                                                Quotations.Expr.Value(24,typeof<int>); 
                                                Quotations.Expr.Value(27,typeof<int>); ]
                elif syntheticMethodBase.Name = "OneOptionalParameter" then 
                     Quotations.Expr.Call (typeof<Provider>.GetMethod("OneOptionalParameter", BindingFlags.Static ||| BindingFlags.Public), [ for p in parameterExpressions -> p ])
                elif syntheticMethodBase.Name = "TwoOptionalParameters" then 
                     Quotations.Expr.Call (typeof<Provider>.GetMethod("TwoOptionalParameters", BindingFlags.Static ||| BindingFlags.Public), [ for p in parameterExpressions -> p ])
                elif syntheticMethodBase.Name = "UsesByRef" then 
                     Quotations.Expr.Call (typeof<Provider>.GetMethod("UsesByRefImpl", BindingFlags.Static ||| BindingFlags.Public), [ for p in parameterExpressions -> p ])
                elif syntheticMethodBase.Name = "UsesByRefAsOutParameter" then 
                     Quotations.Expr.Call (typeof<Provider>.GetMethod("UsesByRefAsOutParameterImpl", BindingFlags.Static ||| BindingFlags.Public), [ for p in parameterExpressions -> p ])
                elif syntheticMethodBase.Name = "op_Addition" then 
                     Quotations.Expr.Call (typeof<Provider>.GetMethod("Add", BindingFlags.Static ||| BindingFlags.Public), [ for p in parameterExpressions -> p ])
                elif syntheticMethodBase.Name = "op_Subtraction" then 
                     // This deliberately swaps argument order so it is not a "simple" expression
                     Quotations.Expr.Call (typeof<Provider>.GetMethod("Sub", BindingFlags.Static ||| BindingFlags.Public), List.rev [ for p in parameterExpressions -> p ])
                else
                    // trim off the "get_"
                    if not (syntheticMethodBase.Name.StartsWith "get_") then failwith "expected syntheticMethodBase to be a property getter, with name starting with \"get_\""
                    let propertyName = syntheticMethodBase.Name.Substring(4)
                    let syntheticMethodBase = syntheticMethodBase :?> MethodInfo
                    let getClassInstancesByName = 
                        typeof<Provider>
                            .GetMethod("GetPropertyByName", BindingFlags.Static ||| BindingFlags.Public)
                            .MakeGenericMethod([|syntheticMethodBase.ReturnType|])
                    Quotations.Expr.Call(getClassInstancesByName, [ Quotations.Expr.Value(propertyName) ])
            
            let rec trans q = 
                match q with 
                // Eliminate F# property gets to method calls
                | Quotations.Patterns.PropertyGet(obj,propInfo,args) -> 
                    match obj with 
                    | None -> trans (Quotations.Expr.Call(propInfo.GetGetMethod(),args))
                    | Some o -> 
                       // In F# quotations involving pattern matching over union cases, the 
                       // static type of 'o' is not the target type when we fetch the items from the union type.
                       // For example, see the quotation form of this:
                       //  <@@ match (Choice1Of2 3) with Choice1Of2 x -> true | x -> false @@>
                       //
                       // Normally this doesn't matter, e.g. for an evaluator. However, for provided expressions we can't cope 
                       // with this case as the F# compiler is not aware of the case-types for the union type.
                       //
                       // let o = 
                       //     if propInfo.DeclaringType.IsAssignableFrom o.Type then 
                       //         o
                       //     else 
                       //         Quotations.Expr.Coerce(o,propInfo.DeclaringType)

                       trans (Quotations.Expr.Call(trans o,propInfo.GetGetMethod(),args))
                // Eliminate F# property sets to method calls
                | Quotations.Patterns.PropertySet(obj,propInfo,args,v) -> 
                     match obj with 
                     | None -> trans (Quotations.Expr.Call(propInfo.GetSetMethod(),args@[v]))
                     | Some o -> trans (Quotations.Expr.Call(trans o,propInfo.GetSetMethod(),args@[v]))
                // Eliminate F# function applications to FSharpFunc<_,_>.Invoke calls
                | Quotations.Patterns.Application(f,e) -> 
                    Quotations.Expr.Call(trans f, f.Type.GetMethod "Invoke", [ trans e ]) 

                | Quotations.Patterns.NewUnionCase(ci, es) ->
                    trans (Quotations.Expr.Call(Reflection.FSharpValue.PreComputeUnionConstructorInfo ci, es) )
                | Quotations.Patterns.NewRecord(ci, es) ->
                    trans (Quotations.Expr.NewObject(Reflection.FSharpValue.PreComputeRecordConstructorInfo ci, es) )
                | Quotations.Patterns.UnionCaseTest(e,uc) ->
                    let tagInfo = Reflection.FSharpValue.PreComputeUnionTagMemberInfo uc.DeclaringType
                    let tagExpr = 
                        match tagInfo with 
                        | :? PropertyInfo as tagProp ->
                             Quotations.Expr.PropertyGet(e,tagProp) 
                        | :? MethodInfo as tagMeth -> 
                             if tagMeth.IsStatic then Quotations.Expr.Call(tagMeth, [e])
                             else Quotations.Expr.Call(e,tagMeth,[])
                        | _ -> failwith "unreachable: unexpected result from PreComputeUnionTagMemberInfo"
                    let tagNumber = uc.Tag
                    // Translate to a call the F# library equality routine
                    trans <@@ (%%(tagExpr) : int) = tagNumber @@>

                // Handle the generic cases
                | Quotations.ExprShape.ShapeLambda(v,body) -> 
                    Quotations.Expr.Lambda(v, trans body)
                | Quotations.ExprShape.ShapeCombination(comb,args) -> 
                    Quotations.ExprShape.RebuildShapeCombination(comb,List.map trans args)
                | Quotations.ExprShape.ShapeVar _ -> q
            trans expr

        // This event is never triggered in this sample, because the schema never changes
        [<CLIEvent>]
        member this.Invalidate = invalidation.Publish
       

        member this.GetGeneratedAssemblyContents(assembly) = failwith "GetGeneratedAssemblyContents"

    member this.GetStaticParametersForMethodImpl(methodWithoutArguments: MethodBase) =
     [| match methodWithoutArguments.Name with
        | StartsWith "HelloWorldInstanceMethodWithStaticSByteParameter" -> yield TypeBuilder.CreateStaticParameter("Count",typeof<sbyte>, 0) 
        | StartsWith "HelloWorldInstanceMethodWithStaticInt16Parameter" -> yield TypeBuilder.CreateStaticParameter("Count",typeof<int16>, 0) 
        | StartsWith "HelloWorldInstanceMethodWithStaticInt32Parameter" -> yield TypeBuilder.CreateStaticParameter("Count",typeof<int32>, 0) 
        | StartsWith "HelloWorldInstanceMethodWithStaticOptionalInt32Parameter" -> 
                yield TypeBuilder.CreateStaticParameter("Count",typeof<int32>, 0, defaultValue=42) 
#if ADD_AN_OPTIONAL_STATIC_PARAMETER
                yield TypeBuilder.CreateStaticParameter("ExtraParameter",typeof<int32>, 1, defaultValue=43)             
#endif
        | StartsWith "HelloWorldInstanceMethodWithStaticInt64Parameter" -> yield TypeBuilder.CreateStaticParameter("Count",typeof<int64>, 0) 
        | StartsWith "HelloWorldInstanceMethodWithStaticByteParameter" -> yield TypeBuilder.CreateStaticParameter("Count",typeof<byte>, 0) 
        | StartsWith "HelloWorldInstanceMethodWithStaticUInt16Parameter" -> yield TypeBuilder.CreateStaticParameter("Count",typeof<uint16>, 0) 
        | StartsWith "HelloWorldStaticMethodWithStaticUInt32Parameter" -> yield TypeBuilder.CreateStaticParameter("Count",typeof<uint32>, 0) 
        | StartsWith "HelloWorldStaticMethodWithStaticUInt64Parameter" -> yield TypeBuilder.CreateStaticParameter("Count",typeof<uint64>, 0) 
        | StartsWith "HelloWorldInstanceMethodWithStaticDayOfWeekParameter" -> yield TypeBuilder.CreateStaticParameter("Count",typeof<System.DayOfWeek>, 0) 
        | StartsWith "HelloWorldInstanceMethodWithStaticDecimalParameter" -> yield TypeBuilder.CreateStaticParameter("Count",typeof<System.Decimal>, 0) 
        | StartsWith "HelloWorldInstanceMethodWithStaticSingleParameter" -> yield TypeBuilder.CreateStaticParameter("Count",typeof<single>, 0) 
        | StartsWith "HelloWorldInstanceMethodWithStaticDoubleParameter" -> yield TypeBuilder.CreateStaticParameter("Count",typeof<double>, 0) 
        | StartsWith "HelloWorldInstanceMethodWithStaticBoolParameter" -> yield TypeBuilder.CreateStaticParameter("Count",typeof<bool>, 0) 
        | StartsWith "HelloWorldInstanceMethodWithStaticCharParameter" -> yield TypeBuilder.CreateStaticParameter("Count",typeof<char>, 0) 
        | _ -> () |]

    member this.ApplyStaticArgumentsForMethodImpl(methodWithoutArguments: MethodBase, mangledName:string, staticArguments:obj[]) = 
        let methodNameWithArguments = mangledName
        match methodWithoutArguments.Name with
        | "HelloWorldMethod" -> 
            if staticArguments.Length <> 0 then failwith "this provided method does not accept static parameters" 
            methodWithoutArguments
        | StartsWith "HelloWorldInstanceMethodWithStaticSByteParameter" -> 
            if staticArguments.Length <> 1 then failwith "this provided method accepts one static parameter" 
            helloWorldMethodWithStaticParameters.Apply(false,methodWithoutArguments.DeclaringType,typeof<sbyte>,methodNameWithArguments, staticArguments.[0] :?> sbyte |> int)
        | StartsWith "HelloWorldInstanceMethodWithStaticInt16Parameter" -> 
            if staticArguments.Length <> 1 then failwith "this provided method accepts one static parameter" 
            helloWorldMethodWithStaticParameters.Apply(false,methodWithoutArguments.DeclaringType,typeof<int16>,methodNameWithArguments, staticArguments.[0] :?> int16 |> int)
        | StartsWith "HelloWorldInstanceMethodWithStaticInt32Parameter" -> 
            if staticArguments.Length <> 1 then failwith "this provided method accepts one static parameter" 
            helloWorldMethodWithStaticParameters.Apply(false,methodWithoutArguments.DeclaringType,typeof<int>,methodNameWithArguments, staticArguments.[0] :?> int)
        | StartsWith "HelloWorldInstanceMethodWithStaticInt64Parameter" -> 
            if staticArguments.Length <> 1 then failwith "this provided method accepts one static parameter" 
            helloWorldMethodWithStaticParameters.Apply(false,methodWithoutArguments.DeclaringType,typeof<int64>,methodNameWithArguments, staticArguments.[0] :?> int64 |> int)
        | StartsWith "HelloWorldInstanceMethodWithStaticByteParameter" -> 
            if staticArguments.Length <> 1 then failwith "this provided method accepts one static parameter" 
            helloWorldMethodWithStaticParameters.Apply(false,methodWithoutArguments.DeclaringType,typeof<byte>,methodNameWithArguments, staticArguments.[0] :?> byte |> int)
        | StartsWith "HelloWorldInstanceMethodWithStaticUInt16Parameter" -> 
            if staticArguments.Length <> 1 then failwith "this provided method accepts one static parameter" 
            helloWorldMethodWithStaticParameters.Apply(false,methodWithoutArguments.DeclaringType,typeof<uint16>,methodNameWithArguments, staticArguments.[0] :?> uint16 |> int)
        | StartsWith "HelloWorldStaticMethodWithStaticUInt32Parameter" -> 
            if staticArguments.Length <> 1 then failwith "this provided method accepts one static parameter" 
            helloWorldMethodWithStaticParameters.Apply(true,methodWithoutArguments.DeclaringType,typeof<uint32>,methodNameWithArguments, staticArguments.[0] :?> uint32 |> int)
        | StartsWith "HelloWorldStaticMethodWithStaticUInt64Parameter" -> 
            if staticArguments.Length <> 1 then failwith "this provided method accepts one static parameter" 
            helloWorldMethodWithStaticParameters.Apply(true,methodWithoutArguments.DeclaringType,typeof<uint64>,methodNameWithArguments, staticArguments.[0] :?> uint64 |> int)
        | StartsWith "HelloWorldInstanceMethodWithStaticDayOfWeekParameter" -> 
            if staticArguments.Length <> 1 then failwith "this provided method accepts one static parameter" 
            helloWorldMethodWithStaticParameters.Apply(false,methodWithoutArguments.DeclaringType,typeof<System.DayOfWeek>,methodNameWithArguments, staticArguments.[0] :?> System.DayOfWeek |> int)
        | StartsWith "HelloWorldInstanceMethodWithStaticBoolParameter" -> 
            if staticArguments.Length <> 1 then failwith "this provided method accepts one static parameter" 
            helloWorldMethodWithStaticParameters.Apply(false,methodWithoutArguments.DeclaringType,typeof<bool>,methodNameWithArguments, if (staticArguments.[0] :?> bool) then 1 else 0)
        | StartsWith "HelloWorldInstanceMethodWithStaticStringParameter" -> 
            if staticArguments.Length <> 1 then failwith "this provided method accepts one static parameter" 
            helloWorldMethodWithStaticParameters.Apply(false,methodWithoutArguments.DeclaringType,typeof<string>,methodNameWithArguments, staticArguments.[0] :?> string |> String.length)
        | StartsWith "HelloWorldInstanceMethodWithStaticDecimalParameter" -> 
            if staticArguments.Length <> 1 then failwith "this provided method accepts one static parameter" 
            helloWorldMethodWithStaticParameters.Apply(false,methodWithoutArguments.DeclaringType,typeof<decimal>,methodNameWithArguments, staticArguments.[0] :?> decimal |> int)
        | StartsWith "HelloWorldInstanceMethodWithStaticCharParameter" -> 
            if staticArguments.Length <> 1 then failwith "this provided method accepts one static parameter" 
            helloWorldMethodWithStaticParameters.Apply(false,methodWithoutArguments.DeclaringType,typeof<char>,methodNameWithArguments, staticArguments.[0] :?> char |> int)
        | StartsWith "HelloWorldInstanceMethodWithStaticSingleParameter" -> 
            if staticArguments.Length <> 1 then failwith "this provided method accepts one static parameter" 
            helloWorldMethodWithStaticParameters.Apply(false,methodWithoutArguments.DeclaringType,typeof<single>,methodNameWithArguments, staticArguments.[0] :?> single |> int)
        | StartsWith "HelloWorldInstanceMethodWithStaticDoubleParameter" -> 
            if staticArguments.Length <> 1 then failwith "this provided method accepts one static parameter" 
            helloWorldMethodWithStaticParameters.Apply(false,methodWithoutArguments.DeclaringType,typeof<double>,methodNameWithArguments, staticArguments.[0] :?> double |> int)

        | StartsWith "HelloWorldInstanceMethodWithStaticOptionalInt32Parameter" -> 
#if ADD_AN_OPTIONAL_STATIC_PARAMETER
            if staticArguments.Length <> 2 then failwith "this provided method accepts two static parameters" 
#else
            if staticArguments.Length <> 1 then failwith "this provided method accepts zero, one static parameter" 
#endif
            helloWorldMethodWithStaticParameters.Apply(false,methodWithoutArguments.DeclaringType,typeof<int>,methodNameWithArguments, staticArguments.[0] :?> int)

        | nm -> failwith (sprintf "ApplyStaticArgumentsForMethod %s" nm)

#if USE_IMPLICIT_ITypeProvider2
    member this.GetStaticParametersForMethod(methodWithoutArguments) = this.GetStaticParametersForMethodImpl(methodWithoutArguments)
    member this.ApplyStaticArgumentsForMethod(methodWithoutArguments, mangledName, staticArguments) = this.ApplyStaticArgumentsForMethodImpl(methodWithoutArguments, mangledName, staticArguments) 
#else
    interface ITypeProvider2 with
        member this.GetStaticParametersForMethod(methodWithoutArguments) = this.GetStaticParametersForMethodImpl(methodWithoutArguments)
        member this.ApplyStaticArgumentsForMethod(methodWithoutArguments, mangledName, staticArguments) = 
            this.ApplyStaticArgumentsForMethodImpl(methodWithoutArguments, mangledName, staticArguments) 
#endif


[<TypeProvider>]
type public GenerativeProvider(config: TypeProviderConfig) =
    let modul = typeof<Provider>.Assembly.GetModules().[0]
    let rootNamespace = "FSharp.HelloWorldGenerative"
    let theAssembly = System.Reflection.Assembly.LoadFrom(Path.Combine(Path.GetDirectoryName config.RuntimeAssembly,"provided.dll"))

    let exclude = set [ "TheGeneratedType1"; 
                        "TheGeneratedType1WithIndexer" 
                        "TheGeneratedType2"; 
                        "TheGeneratedType2WithIndexer"; 
                        "TheGeneratedType3InContainerType1"; 
                        "TheGeneratedType3InContainerType2"; 
                        "TheGeneratedType5"; 
                        "TheGeneratedType5WithIndexer" ]


    let typeTable = 
        MemoizationTable(fun (actualName,typeName) -> 
             let theType = System.Reflection.Assembly.LoadFrom(Path.Combine(Path.GetDirectoryName config.RuntimeAssembly,"provided.dll")).GetType(typeName)
             theType)

    let theContainerTypeUninstantiated = TypeBuilder.CreateSimpleType(TypeContainer.Namespace(modul,rootNamespace), "TheContainerType", members=lazy [| |], isErased=false)

    let theTypes() = [| for x in theAssembly.GetTypes() do if not (exclude.Contains x.Name) && not x.IsNested then yield  x :> MemberInfo |]

    let invalidation = new Event<System.EventHandler,_>()

    interface IProvidedNamespace with
       member this.NamespaceName = rootNamespace
       member this.GetTypes() = [|theContainerTypeUninstantiated|]
       member this.GetNestedNamespaces() = [| |]
       member this.ResolveTypeName typeName =
         match typeName with
         |   "TheContainerType" -> theContainerTypeUninstantiated
         |   _ -> null

    interface IDisposable with
        member __.Dispose() = ()
    interface ITypeProvider with
       member this.GetNamespaces() = [| this |]
       member this.GetStaticParameters(typeWithoutArguments) = 
            match typeWithoutArguments.Name with
            |   "TheContainerType" -> [| TypeBuilder.CreateStaticParameter("TypeName",typeof<string>, 0) |]
            | _ -> [| |]
       member this.ApplyStaticArguments(typeWithoutArguments, typeNameWithArguments, staticArguments) = 
            let typeNameWithArguments = typeNameWithArguments.[typeNameWithArguments.Length-1]
            match typeWithoutArguments.Name with
            |   "TheContainerType" -> typeTable.Apply (typeNameWithArguments, (staticArguments.[0] :?> string))
            | _ -> null

       member __.GetInvokerExpression(mbase, parameters) = TypeBuilder.StandardGenerativeGetInvokerExpression(mbase, parameters)
       [<CLIEvent>]
       member this.Invalidate = invalidation.Publish
       
       member this.GetGeneratedAssemblyContents(assembly) = System.IO.File.ReadAllBytes "provided.dll"


[<TypeProvider>]
type public TwoNamespaceGenerativeProvider(config: TypeProviderConfig) =
    let modul = typeof<Provider>.Assembly.GetModules().[0]
    let rootNamespace1 = "FSharp.HelloWorldGenerativeNamespace1"
    let rootNamespace2 = "FSharp.HelloWorldGenerativeNamespace2"

    let theType1 = System.Reflection.Assembly.LoadFrom(Path.Combine(Path.GetDirectoryName config.RuntimeAssembly,"provided.dll")).GetType("TheGeneratedType1")
    let theContainerType1 = TypeBuilder.CreateSimpleType(TypeContainer.Namespace(modul,rootNamespace1), "TheContainerType", members=lazy [| |], isErased=false)
    let theType2 = System.Reflection.Assembly.LoadFrom(Path.Combine(Path.GetDirectoryName config.RuntimeAssembly,"provided.dll")).GetType("TheGeneratedType1WithIndexer")
    let theContainerType2 = TypeBuilder.CreateSimpleType(TypeContainer.Namespace(modul,rootNamespace2), "TheContainerType", members=lazy [| |], isErased=false)

    let invalidation = new Event<System.EventHandler,_>()

    let n1 = mkNamespace (rootNamespace1, theContainerType1)
    let n2 = mkNamespace (rootNamespace2, theContainerType2)
    interface IDisposable with
        member __.Dispose() = ()
    interface ITypeProvider with
       member this.GetNamespaces() = [| n1; n2 |]
       member this.GetStaticParameters(typeWithoutArguments) = 
            if typeWithoutArguments.FullName = theContainerType1.FullName then [| TypeBuilder.CreateStaticParameter("Unused",typeof<string>, 0) |]
            elif typeWithoutArguments.FullName = theContainerType2.FullName then [| TypeBuilder.CreateStaticParameter("Unused",typeof<string>, 0) |]
            else [| |]
       member this.ApplyStaticArguments(typeWithoutArguments, typeNameWithArguments, staticArguments) = 
            let typeNameWithArguments = typeNameWithArguments.[typeNameWithArguments.Length-1]
            if typeWithoutArguments.FullName = theContainerType1.FullName then theType1
            elif typeWithoutArguments.FullName = theContainerType2.FullName then theType2
            else typeWithoutArguments
       member __.GetInvokerExpression(mbase, parameters) = TypeBuilder.StandardGenerativeGetInvokerExpression(mbase, parameters)
       [<CLIEvent>]
       member this.Invalidate = invalidation.Publish
       
       member this.GetGeneratedAssemblyContents(assembly) = System.IO.File.ReadAllBytes "provided.dll"


[<TypeProvider>]
type public TwoInternalNamespaceGenerativeProvider(config: TypeProviderConfig) =
    let modul = typeof<Provider>.Assembly.GetModules().[0]
    let rootNamespace1 = "FSharp.HelloWorldGenerativeInternalNamespace1"
    let rootNamespace2 = "FSharp.HelloWorldGenerativeInternalNamespace2"
    let theType1 = System.Reflection.Assembly.LoadFrom(Path.Combine(Path.GetDirectoryName config.RuntimeAssembly,"provided.dll")).GetType("TheGeneratedType5")
    let theContainerType1 = TypeBuilder.CreateSimpleType(TypeContainer.Namespace(modul,rootNamespace1), "TheContainerType", members=lazy [| |], isErased=false)
    let theType2 = System.Reflection.Assembly.LoadFrom(Path.Combine(Path.GetDirectoryName config.RuntimeAssembly,"provided.dll")).GetType("TheGeneratedType5WithIndexer")
    let theContainerType2 = TypeBuilder.CreateSimpleType(TypeContainer.Namespace(modul,rootNamespace2), "TheContainerType", members=lazy [| |], isErased=false)

    let invalidation = new Event<System.EventHandler,_>()

    let n1 = mkNamespace (rootNamespace1, theContainerType1)
    let n2 = mkNamespace (rootNamespace2, theContainerType2)
    interface IDisposable with
        member __.Dispose() = ()
    interface ITypeProvider with
       member this.GetNamespaces() = [| n1; n2 |]
       member this.GetStaticParameters(typeWithoutArguments) = 
            if typeWithoutArguments.Equals(theContainerType1) then [| TypeBuilder.CreateStaticParameter("Unused",typeof<string>, 0) |]
            elif typeWithoutArguments.Equals(theContainerType2) then [| TypeBuilder.CreateStaticParameter("Unused",typeof<string>, 0) |]
            else [| |]
       member this.ApplyStaticArguments(typeWithoutArguments, typeNameWithArguments, staticArguments) = 
            let typeNameWithArguments = typeNameWithArguments.[typeNameWithArguments.Length-1]
            if typeWithoutArguments.FullName = theContainerType1.FullName then theType1
            elif typeWithoutArguments.FullName = theContainerType2.FullName then theType2
            else typeWithoutArguments
       member __.GetInvokerExpression(mbase, parameters) = TypeBuilder.StandardGenerativeGetInvokerExpression(mbase, parameters)
       [<CLIEvent>]
       member this.Invalidate = invalidation.Publish
       member this.GetGeneratedAssemblyContents(assembly) = System.IO.File.ReadAllBytes "provided.dll"
       


[<TypeProvider>]
// Two namespaces in the same provider contribute to the same namespace
type public OneProviderContributesTwoFragmentsToSameNamespace(config: TypeProviderConfig) =
    let modul = typeof<Provider>.Assembly.GetModules().[0]
    let rootNamespace = "FSharp.OneProviderContributesTwoFragmentsToSameNamespace"
    let theType1 = System.Reflection.Assembly.LoadFrom(Path.Combine(Path.GetDirectoryName config.RuntimeAssembly,"provided.dll")).GetType("TheGeneratedType2")
    let theContainerType1Uninstantiated = TypeBuilder.CreateSimpleType(TypeContainer.Namespace(modul,rootNamespace), "TheContainerType1", members=lazy [| |], isErased=false)
    let theType2 = System.Reflection.Assembly.LoadFrom(Path.Combine(Path.GetDirectoryName config.RuntimeAssembly,"provided.dll")).GetType("TheGeneratedType2WithIndexer")
    let theContainerType2Uninstantiated = TypeBuilder.CreateSimpleType(TypeContainer.Namespace(modul,rootNamespace), "TheContainerType2", members=lazy [| |], isErased=false)

    let invalidation = new Event<System.EventHandler,_>()

    let n1 = mkNamespace (rootNamespace, theContainerType1Uninstantiated)
    let n2 = mkNamespace (rootNamespace, theContainerType2Uninstantiated)
    interface IDisposable with
        member __.Dispose() = ()
    interface ITypeProvider with
       member this.GetNamespaces() = [| n1; n2 |]
       member this.GetStaticParameters(typeWithoutArguments) = 
            match typeWithoutArguments.Name with
            |   n when n = "TheContainerType1" || n = "TheContainerType2" -> [| TypeBuilder.CreateStaticParameter("Unused",typeof<string>, 0) |]
            | _ -> [| |]
       member this.ApplyStaticArguments(typeWithoutArguments, typeNameWithArguments, staticArguments) = 
            match typeWithoutArguments.Name with
            |   n when n = "TheContainerType1" -> theType1
            |   n when n = "TheContainerType2" -> theType2
            | _ -> typeWithoutArguments
       member __.GetInvokerExpression(mbase, parameters) = TypeBuilder.StandardGenerativeGetInvokerExpression(mbase, parameters)
       [<CLIEvent>]
       member this.Invalidate = invalidation.Publish
       
       member this.GetGeneratedAssemblyContents(assembly) = System.IO.File.ReadAllBytes "provided.dll"


// Two providers contribute to same namespace
type public TwoProvidersContributeToSameNamespaceBase(config: TypeProviderConfig, typeName) =
    let modul = typeof<Provider>.Assembly.GetModules().[0]
    let rootNamespace = "FSharp.TwoProvidersContributeToSameNamespace"
    let theType1 = System.Reflection.Assembly.LoadFrom(Path.Combine(Path.GetDirectoryName config.RuntimeAssembly,"provided.dll")).GetType("TheGeneratedType3In" + typeName)

    let invalidation = new Event<System.EventHandler,_>()
    let theContainerTypeUninstantiated = TypeBuilder.CreateSimpleType(TypeContainer.Namespace(modul,rootNamespace), typeName, members=lazy [| |], isErased=false)
    let n1 = mkNamespace (rootNamespace, theContainerTypeUninstantiated)
    interface IDisposable with
        member __.Dispose() = ()
    interface ITypeProvider with
       member this.GetNamespaces() = [| n1 |]
       member this.GetStaticParameters(typeWithoutArguments) = 
            match typeWithoutArguments.Name with
            |   n when n = typeName -> [| TypeBuilder.CreateStaticParameter("Tag",typeof<string>, 0) |]
            | _ -> [| |]
       member this.ApplyStaticArguments(typeWithoutArguments, typeNameWithArguments, staticArguments) = 
            match typeWithoutArguments.Name with
            |   n when n = typeName-> theType1
            | _ -> typeWithoutArguments
       member __.GetInvokerExpression(mbase, parameters) = TypeBuilder.StandardGenerativeGetInvokerExpression(mbase, parameters)
       [<CLIEvent>]
       member this.Invalidate = invalidation.Publish
       member this.GetGeneratedAssemblyContents(assembly) = System.IO.File.ReadAllBytes "provided.dll"
       
// Two providers contribute to same namespace
[<TypeProvider>]
type public TwoProvidersContributeToSameNamespaceA(config: TypeProviderConfig) =
    inherit TwoProvidersContributeToSameNamespaceBase(config,"ContainerType1")


[<TypeProvider>]
type public TwoProvidersContributeToSameNamespaceB(config: TypeProviderConfig) =
    inherit TwoProvidersContributeToSameNamespaceBase(config,"ContainerType2")



[<TypeProvider>]
type public GenerativeProviderWithStaticParameter(config: TypeProviderConfig) =
    let modul = typeof<Provider>.Assembly.GetModules().[0]
    let rootNamespace = "FSharp.HelloWorldGenerativeWithStaticParameter"

    let invalidation = new Event<System.EventHandler,_>()

    let typeTable = 
        MemoizationTable(fun (actualName,c) -> 
             let theType = System.Reflection.Assembly.LoadFrom(Path.Combine(Path.GetDirectoryName config.RuntimeAssembly,"provided"+c+".dll")).GetType("TheGeneratedType"+c)
             theType)
    let theContainerTypeUninstantiated = TypeBuilder.CreateSimpleType(TypeContainer.Namespace(modul,rootNamespace), "TheContainerType", members=lazy [| |], isErased=false)
    interface IProvidedNamespace with
       member this.NamespaceName = rootNamespace
       member this.GetTypes() = [|theContainerTypeUninstantiated|]
       member this.GetNestedNamespaces() = [| |]
       member this.ResolveTypeName typeName =
         match typeName with
         |   "TheContainerType" -> theContainerTypeUninstantiated
         |   _ -> null

    interface IDisposable with
        member __.Dispose() = ()
    interface ITypeProvider with
       member this.GetNamespaces() = [| this |]
       member this.GetStaticParameters(typeWithoutArguments) = 
            match typeWithoutArguments.Name with
            |   "TheContainerType" -> [| TypeBuilder.CreateStaticParameter("Tag",typeof<string>, 0) |]
            | _ -> [| |]
       member this.ApplyStaticArguments(typeWithoutArguments, typeNameWithArguments, staticArguments) = 
            let typeNameWithArguments = typeNameWithArguments.[typeNameWithArguments.Length-1]
            match typeWithoutArguments.Name with
            |   "TheContainerType" -> 
                match (staticArguments.[0] :?> string) with 
                | "J" -> typeTable.Apply (typeNameWithArguments, "J")
                | "K" -> typeTable.Apply (typeNameWithArguments, "K")
                | _ -> failwith "invalid static parameter to TheContainerType: expected 'J' or 'K'"
            | _ -> null

       member __.GetInvokerExpression(mbase, parameters) = TypeBuilder.StandardGenerativeGetInvokerExpression(mbase, parameters)
       [<CLIEvent>]
       member this.Invalidate = invalidation.Publish
       
       member this.GetGeneratedAssemblyContents(assembly) = 
           if assembly.GetName().Name = "providedJ" then 
                System.IO.File.ReadAllBytes "providedJ.dll"
           elif assembly.GetName().Name = "providedK" then 
                System.IO.File.ReadAllBytes "providedK.dll"
           else failwith "expected providedJ or providedK"
