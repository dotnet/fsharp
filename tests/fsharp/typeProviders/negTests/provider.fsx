namespace Provider
open System
open System.Linq.Expressions
open System.Reflection
open Microsoft.FSharp.Core.CompilerServices
open System.Collections.Generic
open System.IO

#load "TypeEvil.fs"
#load "..\helloWorld\TypeMagic.fs"

open FSharp.TypeEvil
open FSharp.TypeMagic

[<assembly: TypeProviderAssembly>]
do ()

type public Runtime() =
    static member Id x = x

[<AutoOpen>]
module Utils = 
    let doEvil() = failwith "deliberate error for testing purposes"

    let mkAllowNullLiteralValueAttributeData(value: bool) = 
        { new CustomAttributeData() with 
                member __.Constructor =  typeof<Microsoft.FSharp.Core.AllowNullLiteralAttribute>.GetConstructors().[0]
                member __.ConstructorArguments = upcast [| CustomAttributeTypedArgument(typeof<bool>, value)  |]
                member __.NamedArguments = upcast [| |] }

[<TypeProvider>]
type public GoodProviderForNegativeTypeTests1() =
    let modul = typeof<GoodProviderForNegativeTypeTests1>.Assembly.GetModules().[0]
    let namespaceName = "FSharp.GoodProviderForNegativeTypeTests1"
    let theType =
        let members (typ:Type) =
            let invoke _ = failwith "Kaboom"
            let fooP = TypeBuilder.CreateProperty(typ, "Foo", typeof<int>, getInvoke = invoke,isStatic = true)
            let invalidInvokerExpressionP = TypeBuilder.CreateProperty(typ, "InvalidInvokerExpression", typeof<int>, getInvoke = invoke,isStatic = true)
            let nullInvokerExpressionP = TypeBuilder.CreateProperty(typ, "NullInvokerExpression", typeof<int>, getInvoke = invoke,isStatic = true)
            fun (_bf:BindingFlags) (mt:MemberTypes) (s:string option) ->
            [|
                if mt &&& MemberTypes.Property = MemberTypes.Property then
                    match s with
                    |   Some "Foo" -> yield fooP :> MemberInfo
                    |   Some "InvalidInvokerExpression" -> yield invalidInvokerExpressionP :> MemberInfo
                    |   Some "NullInvokerExpression" -> yield nullInvokerExpressionP :> MemberInfo
                    |   None -> 
                          yield fooP :> MemberInfo 
                          yield invalidInvokerExpressionP :> MemberInfo
                          yield nullInvokerExpressionP :> MemberInfo
                    |   _ -> ()
            |]
        TypeBuilder.CreateType(TypeContainer.Namespace(modul, namespaceName), "TheType", members = members)       

    let types = [|theType|]

    let invalidation = new Event<System.EventHandler,_>()

    interface IProvidedNamespace with
        member this.GetNestedNamespaces() = [| |]
        member this.NamespaceName = namespaceName
        member this.GetTypes() = types
        member this.ResolveTypeName(s) =
            match s with
            |   "TheType" -> theType
            |   "TheHype" -> failwith "Kaboom"
            |   _ -> null

    interface IDisposable with
        member __.Dispose() = ()
    interface ITypeProvider with
        member this.GetNamespaces() = [| this |]
        member this.GetStaticParameters(typeWithoutArguments) = [| |]
        member this.ApplyStaticArguments(typeWithoutArguments, typeNameWithArguments, staticArguments) = typeWithoutArguments

        member this.GetInvokerExpression(mb:MethodBase,parameters) = 
            match mb,parameters with
            |   _, [||] when mb.Name = "get_Foo" ->
                    let mi = typeof<Runtime>.GetMethod("Id").MakeGenericMethod([|typeof<int>|])
                    Quotations.Expr.Call(mi, [ Quotations.Expr.Value(42) ])
            |   _, [||] when mb.Name = "get_InvalidInvokerExpression" ->
                    Quotations.Expr.WhileLoop(Quotations.Expr.Value(true), Quotations.Expr.Value((), typeof<unit>))
            |   _, [||] when mb.Name = "get_NullInvokerExpression" ->
                    Unchecked.defaultof<_>
            |   _ -> failwith "Unexpected method erasure"

        [<CLIEvent>]
        member this.Invalidate = invalidation.Publish
       
        member this.GetGeneratedAssemblyContents(assembly) = failwith "GetGeneratedAssemblyContents - only erased types were provided!!"


type public EvilProviderBase(namespaceName,?GetNestedNamespaces,?get_NamespaceName,?GetTypes,?ResolveTypeName,?GetNamespaces,?GetStaticParameters,?GetStaticParametersForMethod,?ApplyStaticArguments,?ApplyStaticArgumentsForMethod,?GetInvokerExpression) =
    let invalidation = new Event<System.EventHandler,_>()

    let modul = typeof<EvilProviderBase>.Assembly.GetModules().[0]
    let okType =
        let members (typ:Type) =
            let invoke _ = failwith "Kaboom"
            let booM = TypeBuilder.CreateMethod(typ, "Boo", typeof<int>, invoke = invoke, isStatic = true)
            let fooP = TypeBuilder.CreateProperty(typ, "Foo", typeof<int>, getInvoke = invoke,isStatic = true)
            fun (_bf:BindingFlags) (mt:MemberTypes) (s:string option) ->
            [|
                if mt &&& MemberTypes.Property = MemberTypes.Property then
                    match s with
                    |   Some "Foo" -> yield fooP :> MemberInfo
                    |   None -> yield fooP :> MemberInfo
                    |   _ -> ()
                if mt &&& MemberTypes.Method = MemberTypes.Method then
                    match s with
                    |   Some "Boo" -> yield booM :> MemberInfo
                    |   None -> 
                          yield booM :> MemberInfo 
                    |   _ -> ()
            |]
        TypeBuilder.CreateType(TypeContainer.Namespace(modul, namespaceName), "TheType", members = members)       

    interface IProvidedNamespace with
        member this.GetNestedNamespaces() = match GetNestedNamespaces with Some f -> f() | None -> [| |]
        member this.NamespaceName = match get_NamespaceName with Some f -> f() | None -> namespaceName
        // Return an array
        member this.GetTypes() = match GetTypes with Some f -> f() | None -> [| okType |]
        member this.ResolveTypeName(s) = 
           match ResolveTypeName with 
           | Some f -> f(s) 
           | None -> 
           match s with 
           | "TheType" -> okType
           | "IsArrayTypeRaisesException" -> TypeEvil.Intercept(okType, s, get_IsArrayImpl=(fun _ -> doEvil()))
           | "IsArrayTypeReturnsTrue" -> TypeEvil.Intercept(okType,  s, get_IsArrayImpl=(fun _ -> true))
           | "IsGenericTypeRaisesException" -> TypeEvil.Intercept(okType,  s, get_IsGenericType=(fun _ -> doEvil()))
           | "IsGenericTypeReturnsTrue" -> TypeEvil.Intercept(okType,  s, get_IsGenericType=(fun _ -> true))
           | "TypeWhereNameRaisesException" -> TypeEvil.Intercept(okType, s, get_Name=(fun _ -> doEvil()))
           | "TypeWhereNameReturnsNull" -> TypeEvil.Intercept(okType, s, get_Name=(fun _ -> null))
           | "TypeWhereFullNameRaisesException" -> TypeEvil.Intercept(okType, s, get_FullName=(fun _ -> doEvil()))
           | "TypeWhereFullNameReturnsNull" -> TypeEvil.Intercept(okType,s, get_FullName=(fun _ -> null))
           | "TypeWhereNamespaceRaisesException" -> TypeEvil.Intercept(okType, s, get_Namespace=(fun _ -> doEvil()))
           | "TypeWhereNamepsaceReturnsNull" -> TypeEvil.Intercept(okType, s, get_Namespace=(fun _ -> null))
           | "DeclaringTypeRaisesException" -> TypeEvil.Intercept(okType, s, get_DeclaringType=(fun _ -> doEvil()))

           | "TypeWhereGetMethodsRaisesException" -> TypeEvil.Intercept(okType, s, GetMethods=(fun _ -> doEvil()))
           | "TypeWhereGetEventsRaisesException" -> TypeEvil.Intercept(okType, s, GetEvents=(fun _ -> doEvil()))
           | "TypeWhereGetFieldsRaisesException" -> TypeEvil.Intercept(okType, s, GetFields=(fun _ -> doEvil()))
           | "TypeWhereGetPropertiesRaisesException" -> TypeEvil.Intercept(okType, s, GetProperties=(fun _ -> doEvil()))
           | "TypeWhereGetNestedTypesRaisesException" -> TypeEvil.Intercept(okType, s, GetNestedTypes=(fun _ -> doEvil()))
           | "TypeWhereGetConstructorsRaisesException" -> TypeEvil.Intercept(okType, s, GetConstructors=(fun _ -> doEvil()))
           | "TypeWhereGetInterfacesRaisesException" -> TypeEvil.Intercept(okType, s, GetInterfaces=(fun _ -> doEvil()))

           | "TypeWhereGetMethodsReturnsNull" -> TypeEvil.Intercept(okType, s, GetMethods=(fun _ -> null))
           | "TypeWhereGetEventsReturnsNull" -> TypeEvil.Intercept(okType, s, GetEvents=(fun _ -> null))
           | "TypeWhereGetFieldsReturnsNull" -> TypeEvil.Intercept(okType, s, GetFields=(fun _ -> null))
           | "TypeWhereGetPropertiesReturnsNull" -> TypeEvil.Intercept(okType, s, GetProperties=(fun _ -> null))
           | "TypeWhereGetNestedTypesReturnsNull" -> TypeEvil.Intercept(okType, s, GetNestedTypes=(fun _ -> null))
           | "TypeWhereGetConstructorsReturnsNull" -> TypeEvil.Intercept(okType, s, GetConstructors=(fun _ -> null))
           | "TypeWhereGetInterfacesReturnsNull" -> TypeEvil.Intercept(okType, s, GetInterfaces=(fun _ -> null))

           // positive tests - it should not matter that these raise exceptions
           | "TypeWhereGetGenericArgumentsRaisesException" -> TypeEvil.Intercept(okType, s, GetGenericArguments=(fun _ -> doEvil()))
           | "TypeWhereGetMembersRaisesException" -> TypeEvil.Intercept(okType, s, GetMembers=(fun _ -> doEvil()))

           | _ -> null

    interface IDisposable with
        member __.Dispose() = ()
    interface ITypeProvider with
        member this.GetNamespaces() = match GetNamespaces with Some f -> f() | None -> [| this |]
      
        member this.GetStaticParameters(typeWithoutArguments) = match GetStaticParameters with Some f -> f() | None -> [| |]
        member this.ApplyStaticArguments(typeWithoutArguments, typeNameWithArguments, staticArguments) = match ApplyStaticArguments with Some f -> f() | None -> typeWithoutArguments

        member this.GetInvokerExpression(mb:MethodBase,parameters) = 
            match GetInvokerExpression with 
            | Some f -> f() 
            | None -> 
            match mb,parameters with
            |   _, [||] when mb.Name = "get_Foo" ->
                    let mi = typeof<Runtime>.GetMethod("Id").MakeGenericMethod([|typeof<int>|])
                    Quotations.Expr.Call(mi, [Quotations.Expr.Value(42) ])
            |   _ -> failwith "Unexpected method erasure"


        [<CLIEvent>]
        member this.Invalidate = invalidation.Publish
        member this.GetGeneratedAssemblyContents(assembly) = failwith "GetGeneratedAssemblyContents - only erased types were provided!!"
      
    member this.GetStaticParametersForMethod(methWithoutArguments: MethodBase) : ParameterInfo[] = 
        match GetStaticParametersForMethod with Some f -> f() | None -> [| |]
    member this.ApplyStaticArgumentsForMethod(methWithoutArguments:MethodBase, methNameWithArguments: string, staticArguments: obj[]) = 
        match ApplyStaticArgumentsForMethod with Some f -> f() | None -> methWithoutArguments
       
[<TypeProvider>]
type public EvilProvider() = 
    inherit EvilProviderBase("FSharp.EvilProvider")


#if EVIL_PROVIDER_GetNestedNamespaces_Exception
[<TypeProvider>]
type public EvilProviderWhereGetNestedNamespacesRaisesException() = 
    inherit EvilProviderBase("FSharp.EvilProviderWhereGetNestedNamespacesRaisesException",GetNestedNamespaces=(fun _ -> doEvil()))
#endif

#if EVIL_PROVIDER_NamespaceName_Exception
[<TypeProvider>]
type public EvilProviderWhereNamespaceNameRaisesException() = 
    inherit EvilProviderBase("FSharp.EvilProviderWhereNamespaceNameRaisesException",get_NamespaceName=(fun _ -> doEvil()))
#endif

#if EVIL_PROVIDER_GetTypes_Exception
[<TypeProvider>]
type public EvilProviderWhereGetTypesRaisesException() = 
    inherit EvilProviderBase("FSharp.EvilProviderWhereGetTypesRaisesException",GetTypes=(fun _ -> doEvil()))
#endif

#if EVIL_PROVIDER_ResolveTypeName_Exception
[<TypeProvider>]
type public EvilProviderWhereResolveTypeNameRaisesException() = 
    inherit EvilProviderBase("FSharp.EvilProviderWhereResolveTypeNameRaisesException",ResolveTypeName=(fun _ -> doEvil()))
#endif

#if EVIL_PROVIDER_GetNamespaces_Exception
[<TypeProvider>]
type public EvilProviderWhereGetNamespacesRaisesException() = 
    inherit EvilProviderBase("FSharp.EvilProviderWhereGetNamespacesRaisesException",GetNamespaces=(fun _ -> doEvil()))
#endif

#if EVIL_PROVIDER_GetStaticParameters_Exception
[<TypeProvider>]
type public EvilProviderWhereGetStaticParametersRaisesException() = 
    inherit EvilProviderBase("FSharp.EvilProviderWhereGetStaticParametersRaisesException",GetStaticParameters=(fun _ -> doEvil()))
#endif

#if EVIL_PROVIDER_GetStaticParametersForMethod_Exception
[<TypeProvider>]
type public EvilProviderWhereGetStaticParametersForMethodRaisesException() = 
    inherit EvilProviderBase("FSharp.EvilProviderWhereGetStaticParametersForMethodRaisesException",GetStaticParametersForMethod=(fun _ -> doEvil()))
#endif

#if EVIL_PROVIDER_GetInvokerExpression_Exception
[<TypeProvider>]
type public EvilProviderWhereApplyGetInvokerExpressionRaisesException() = 
    inherit EvilProviderBase("FSharp.EvilProviderWhereGetInvokerExpressionRaisesException",GetInvokerExpression=(fun _ -> doEvil()))
#endif



#if EVIL_PROVIDER_GetNestedNamespaces_Null
[<TypeProvider>]
type public EvilProviderWhereGetNestedNamespacesReturnsNull() = 
    inherit EvilProviderBase("FSharp.EvilProviderWhereGetNestedNamespacesReturnsNull",GetNestedNamespaces=(fun _ -> null))
#endif

#if EVIL_PROVIDER_NamespaceName_Null
[<TypeProvider>]
type public EvilProviderWhereNamespaceNameReturnsNull() = 
    inherit EvilProviderBase("FSharp.EvilProviderWhereNamespaceNameReturnsNull",get_NamespaceName=(fun _ -> null))
#endif

#if EVIL_PROVIDER_NamespaceName_Empty
[<TypeProvider>]
type public EvilProviderWhereNamespaceNameReturnsEmpty() = 
    inherit EvilProviderBase("FSharp.EvilProviderWhereNamespaceNameReturnsEmpty",get_NamespaceName=(fun _ -> ""))
#endif

#if EVIL_PROVIDER_GetTypes_Null
[<TypeProvider>]
type public EvilProviderWhereGetTypesReturnsNull() = 
    inherit EvilProviderBase("FSharp.EvilProviderWhereGetTypesReturnsNull",GetTypes=(fun _ -> null))
#endif

#if EVIL_PROVIDER_ResolveTypeName_Null
[<TypeProvider>]
type public EvilProviderWhereResolveTypeNameReturnsNull() = 
    inherit EvilProviderBase("FSharp.EvilProviderWhereResolveTypeNameReturnsNull",ResolveTypeName=(fun _ -> null))
#endif

#if EVIL_PROVIDER_GetNamespaces_Null
[<TypeProvider>]
type public EvilProviderWhereGetNamespacesReturnsNull() = 
    inherit EvilProviderBase("FSharp.EvilProviderWhereGetNamespacesReturnsNull",GetNamespaces=(fun _ -> null))
#endif

#if EVIL_PROVIDER_GetStaticParameters_Null
[<TypeProvider>]
type public EvilProviderWhereGetStaticParametersReturnsNull() = 
    inherit EvilProviderBase("FSharp.EvilProviderWhereGetStaticParametersReturnsNull",GetStaticParameters=(fun _ -> null))
#endif

#if EVIL_PROVIDER_GetStaticParametersForMethod_Null
[<TypeProvider>]
type public EvilProviderWhereGetStaticParametersForMethodReturnsNull() = 
    inherit EvilProviderBase("FSharp.EvilProviderWhereGetStaticParametersForMethodReturnsNull",GetStaticParametersForMethod=(fun _ -> null))
#endif

#if EVIL_PROVIDER_GetInvokerExpression_Null
[<TypeProvider>]
type public EvilProviderWhereApplyGetInvokerExpressionReturnsNull() = 
    inherit EvilProviderBase("FSharp.EvilProviderWhereGetInvokerExpressionReturnsNull",GetInvokerExpression=(fun _ -> Unchecked.defaultof<_>))
#endif

#if EVIL_PROVIDER_DoesNotHaveConstructor
[<TypeProvider>]
type public EvilProviderDoesNotHaveConstructor(_o : obj, s : string) = 
    inherit EvilProviderBase("FSharp.EvilProviderDoesNotHaveConstructor")
#endif

#if EVIL_PROVIDER_ConstructorThrows
[<TypeProvider>]
type public EvilProviderConstructorThrows() = 
    inherit EvilProviderBase("FSharp.ConstructorThrows")
    do failwith "Kaboom"
#endif


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
type public GoodProviderForNegativeStaticParameterTypeTests() =
    let thisAssembly = typeof<GoodProviderForNegativeStaticParameterTypeTests>.Assembly
    let modul = thisAssembly.GetModules().[0]
    let rootNamespace = "FSharp.GoodProviderForNegativeStaticParameterTypeTests"
    let helloWorldType =
        let rec allMembers = 
            lazy 
                [| for (propertyName, propertyType) in  [for i in 1 .. 2 -> ("StaticProperty"+string i, (if i = 1 then typeof<string> else typeof<int>)) ] do 
                       yield TypeBuilder.CreateSyntheticProperty(theType,propertyName,propertyType,isStatic=true) :> MemberInfo |]  

        and theType = 
            let container = TypeContainer.Namespace(modul, rootNamespace)
            TypeBuilder.CreateSimpleType(container,"HelloWorldType",members=allMembers,
                                         getCustomAttributes=(fun () -> [| mkAllowNullLiteralValueAttributeData(false) |]))

        theType

    // This defines all the members in the one synthetic type. A lazy computation is used to ensure the
    // members are only created once.
    let helloWorldTypeWithStaticParameters =
      MemoizationTable(fun (fullName,n) ->
        let rec allMembers = 
            lazy 
                [| for (propertyName, propertyType) in  [for i in 1 .. n -> ("StaticProperty"+string i, (if i = 1 then typeof<string> else typeof<int>)) ] do 
                       yield TypeBuilder.CreateSyntheticProperty(theType,propertyName,propertyType,isStatic=true) :> MemberInfo |]  

        and theType = 
            let container = TypeContainer.Namespace(modul, rootNamespace)
            TypeBuilder.CreateSimpleType(container,fullName,members=allMembers)

        theType)

    let helloWorldTypeWithStaticParameterUninstantiated = helloWorldTypeWithStaticParameters.Apply("HelloWorldTypeWithStaticParameter",1)
    let helloWorldTypeWithStaticCharParameterUninstantiated = helloWorldTypeWithStaticParameters.Apply("HelloWorldTypeWithStaticCharParameter",1)
    let helloWorldTypeWithStaticBoolParameterUninstantiated = helloWorldTypeWithStaticParameters.Apply("HelloWorldTypeWithStaticBoolParameter",1)
    let helloWorldTypeWithStaticSByteParameterUninstantiated = helloWorldTypeWithStaticParameters.Apply("HelloWorldTypeWithStaticSByteParameter",1)
    let helloWorldTypeWithStaticInt16ParameterUninstantiated = helloWorldTypeWithStaticParameters.Apply("HelloWorldTypeWithStaticInt16Parameter",1)
    let helloWorldTypeWithStaticInt32ParameterUninstantiated = helloWorldTypeWithStaticParameters.Apply("HelloWorldTypeWithStaticInt32Parameter",1)
    let helloWorldTypeWithStaticInt64ParameterUninstantiated = helloWorldTypeWithStaticParameters.Apply("HelloWorldTypeWithStaticInt64Parameter",1)
    let helloWorldTypeWithStaticByteParameterUninstantiated = helloWorldTypeWithStaticParameters.Apply("HelloWorldTypeWithStaticByteParameter",1)
    let helloWorldTypeWithStaticUInt16ParameterUninstantiated = helloWorldTypeWithStaticParameters.Apply("HelloWorldTypeWithStaticUInt16Parameter",1)
    let helloWorldTypeWithStaticUInt32ParameterUninstantiated = helloWorldTypeWithStaticParameters.Apply("HelloWorldTypeWithStaticUInt32Parameter",1)
    let helloWorldTypeWithStaticUInt64ParameterUninstantiated = helloWorldTypeWithStaticParameters.Apply("HelloWorldTypeWithStaticUInt64Parameter",1)
    let helloWorldTypeWithStaticStringParameterUninstantiated = helloWorldTypeWithStaticParameters.Apply("HelloWorldTypeWithStaticStringParameter",1)
    let helloWorldTypeWithStaticSingleParameterUninstantiated = helloWorldTypeWithStaticParameters.Apply("HelloWorldTypeWithStaticSingleParameter",1)
    let helloWorldTypeWithStaticDoubleParameterUninstantiated = helloWorldTypeWithStaticParameters.Apply("HelloWorldTypeWithStaticDoubleParameter",1)
    let helloWorldTypeWithStaticDecimalParameterUninstantiated = helloWorldTypeWithStaticParameters.Apply("HelloWorldTypeWithStaticDecimalParameter",1)

    let types = [|helloWorldTypeWithStaticStringParameterUninstantiated; 
                  helloWorldType;
                  helloWorldTypeWithStaticCharParameterUninstantiated
                  helloWorldTypeWithStaticBoolParameterUninstantiated
                  helloWorldTypeWithStaticSByteParameterUninstantiated
                  helloWorldTypeWithStaticInt16ParameterUninstantiated
                  helloWorldTypeWithStaticInt32ParameterUninstantiated
                  helloWorldTypeWithStaticInt64ParameterUninstantiated
                  helloWorldTypeWithStaticByteParameterUninstantiated
                  helloWorldTypeWithStaticUInt16ParameterUninstantiated
                  helloWorldTypeWithStaticUInt32ParameterUninstantiated
                  helloWorldTypeWithStaticUInt64ParameterUninstantiated
                  helloWorldTypeWithStaticStringParameterUninstantiated
                  helloWorldTypeWithStaticSingleParameterUninstantiated
                  helloWorldTypeWithStaticDoubleParameterUninstantiated
                  helloWorldTypeWithStaticDecimalParameterUninstantiated|]

    let invalidation = new Event<System.EventHandler,_>()

    // This implements both get_StaticProperty1 and get_StaticProperty2
    static member GetPropertyByName(propertyName:string) : 'T = 
        match propertyName with 
        | "StaticProperty1" -> "You got a static property" |> box |> unbox
        | "StaticProperty2" -> 42 |> box |> unbox
        | nm when nm.StartsWith "StaticProperty" -> int (nm.Replace("StaticProperty","")) + 40 |> box |> unbox
        | _ -> failwith "unexpected property"

    interface IProvidedNamespace with
        member this.GetNestedNamespaces() = [| |]
        member this.NamespaceName = rootNamespace
        member this.GetTypes() = types
        member __.ResolveTypeName typeName : System.Type = 
            match typeName with
            | "HelloWorldType" -> helloWorldType
            | "HelloWorldTypeWithStaticStringParameter" -> helloWorldTypeWithStaticStringParameterUninstantiated
            | "HelloWorldTypeWithStaticSByteParameter" -> helloWorldTypeWithStaticSByteParameterUninstantiated
            | "HelloWorldTypeWithStaticInt16Parameter" -> helloWorldTypeWithStaticInt16ParameterUninstantiated
            | "HelloWorldTypeWithStaticInt32Parameter" -> helloWorldTypeWithStaticInt32ParameterUninstantiated
            | "HelloWorldTypeWithStaticInt64Parameter" -> helloWorldTypeWithStaticInt64ParameterUninstantiated
            | "HelloWorldTypeWithStaticByteParameter" -> helloWorldTypeWithStaticByteParameterUninstantiated
            | "HelloWorldTypeWithStaticUInt16Parameter" -> helloWorldTypeWithStaticUInt16ParameterUninstantiated
            | "HelloWorldTypeWithStaticUInt32Parameter" -> helloWorldTypeWithStaticUInt32ParameterUninstantiated
            | "HelloWorldTypeWithStaticUInt64Parameter" -> helloWorldTypeWithStaticUInt64ParameterUninstantiated
            | "HelloWorldTypeWithStaticDecimalParameter" -> helloWorldTypeWithStaticDecimalParameterUninstantiated
            | "HelloWorldTypeWithStaticSingleParameter" -> helloWorldTypeWithStaticSingleParameterUninstantiated
            | "HelloWorldTypeWithStaticDoubleParameter" -> helloWorldTypeWithStaticDoubleParameterUninstantiated
            | "HelloWorldTypeWithStaticBoolParameter" -> helloWorldTypeWithStaticBoolParameterUninstantiated
            | "HelloWorldTypeWithStaticCharParameter" -> helloWorldTypeWithStaticCharParameterUninstantiated
            | _ -> null
       
    interface IDisposable with
        member __.Dispose() = ()
    interface ITypeProvider with
        member this.GetNamespaces() = [| this |]
        member this.GetStaticParameters(typeWithoutArguments) = 
            match typeWithoutArguments.Name with
            |   "HelloWorldTypeWithStaticSByteParameter" -> [| TypeBuilder.CreateStaticParameter("Count",typeof<sbyte>, 0) |]
            |   "HelloWorldTypeWithStaticInt16Parameter" -> [| TypeBuilder.CreateStaticParameter("Count",typeof<int16>, 0) |]
            |   "HelloWorldTypeWithStaticInt32Parameter" -> [| TypeBuilder.CreateStaticParameter("Count",typeof<int32>, 0) |]
            |   "HelloWorldTypeWithStaticInt64Parameter" -> [| TypeBuilder.CreateStaticParameter("Count",typeof<int64>, 0) |]
            |   "HelloWorldTypeWithStaticByteParameter" -> [| TypeBuilder.CreateStaticParameter("Count",typeof<byte>, 0) |]
            |   "HelloWorldTypeWithStaticUInt16Parameter" -> [| TypeBuilder.CreateStaticParameter("Count",typeof<uint16>, 0) |]
            |   "HelloWorldTypeWithStaticUInt32Parameter" -> [| TypeBuilder.CreateStaticParameter("Count",typeof<uint32>, 0) |]
            |   "HelloWorldTypeWithStaticUInt64Parameter" -> [| TypeBuilder.CreateStaticParameter("Count",typeof<uint64>, 0) |]
            |   "HelloWorldTypeWithStaticStringParameter" -> [| TypeBuilder.CreateStaticParameter("Count",typeof<string>, 0) |]
            |   "HelloWorldTypeWithStaticDecimalParameter" -> [| TypeBuilder.CreateStaticParameter("Count",typeof<decimal>, 0) |]
            |   "HelloWorldTypeWithStaticSingleParameter" -> [| TypeBuilder.CreateStaticParameter("Count",typeof<single>, 0) |]
            |   "HelloWorldTypeWithStaticDoubleParameter" -> [| TypeBuilder.CreateStaticParameter("Count",typeof<double>, 0) |]
            |   "HelloWorldTypeWithStaticBoolParameter" -> [| TypeBuilder.CreateStaticParameter("Flag",typeof<bool>, 0) |]
            |   "HelloWorldTypeWithStaticCharParameter" -> [| TypeBuilder.CreateStaticParameter("Count",typeof<char>, 0) |]
            |   _ -> [| |]

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
            | nm -> failwith (sprintf "ApplyStaticArguments %s" nm)

        member __.GetInvokerExpression(syntheticMethodBase:MethodBase, parameterExpressions:Quotations.Expr []) = 
            // trim off the "get_"
            if not (syntheticMethodBase.Name.StartsWith "get_") then failwith "expected syntheticMethodBase to be a property getter, with name starting with \"get_\""
            let propertyName = syntheticMethodBase.Name.Substring(4)
            let syntheticMethodBase = syntheticMethodBase :?> MethodInfo
            let getClassInstancesByName = 
                typeof<GoodProviderForNegativeStaticParameterTypeTests>
                    .GetMethod("GetPropertyByName", BindingFlags.Static ||| BindingFlags.Public)
                    .MakeGenericMethod([|syntheticMethodBase.ReturnType|])
            Quotations.Expr.Call(getClassInstancesByName, [ Quotations.Expr.Value(propertyName) ])

        // This event is never triggered in this sample, because the schema never changes
        [<CLIEvent>]
        member this.Invalidate = invalidation.Publish
       
        member this.GetGeneratedAssemblyContents(assembly) = failwith "GetGeneratedAssemblyContents - only erased types were provided!!"

#if EVIL_PROVIDER_ReturnsAGeneratedTypeNotNestedInAnErasedType
[<TypeProvider>]
// Evil in the sense that it returns a generated type that is not nested in a container type
type public EvilGeneratedProviderThatReturnsAGeneratedTypeNotNestedInAnErasedType(config: TypeProviderConfig) =
    let modul = typeof<EvilGeneratedProviderThatReturnsAGeneratedTypeNotNestedInAnErasedType>.Assembly.GetModules().[0]
    let rootNamespace = "FSharp.EvilGeneratedProviderThatReturnsAGeneratedTypeNotNestedInAnErasedType"
    let theType = Assembly.LoadFrom(Path.Combine(Path.GetDirectoryName config.RuntimeAssembly,"provided.dll")).GetType("TheGeneratedType")

    let invalidation = new Event<System.EventHandler,_>()

    interface IProvidedNamespace with
       member this.NamespaceName = rootNamespace
       member this.GetTypes() = [|theType|]
       member this.GetNestedNamespaces() = [| |]
       member this.ResolveTypeName typeName =
         match typeName with
         |   "TheType" -> theType
         |   _ -> null

    interface IDisposable with
        member __.Dispose() = ()
    interface ITypeProvider with
       member this.GetNamespaces() = [| this |]
       member this.GetStaticParameters(typeWithoutArguments) =  [| |]
       member this.ApplyStaticArguments(typeWithoutArguments, typeNameWithArguments, staticArguments) = typeWithoutArguments
       member __.GetInvokerExpression(mbase, parameters) = TypeBuilder.StandardGenerativeGetInvokerExpression(mbase, parameters)
       [<CLIEvent>]
       member this.Invalidate = invalidation.Publish
       member this.GetGeneratedAssemblyContents(assembly) = failwith "GetGeneratedAssemblyContents - only erased types were provided!!"
       
#endif

#if EVIL_PROVIDER_GeneratesMscorlibTypes
[<TypeProvider>]
type public EvilProviderGeneratesMscorlibTypes() =
    let invalidation = new Event<System.EventHandler,_>()

    interface IProvidedNamespace with
       member this.NamespaceName = "FSharp.EvilProviderGeneratesMscorlibTypes"
       member this.GetTypes() = [|typeof<int>|]
       member this.GetNestedNamespaces() = [| |]
       member this.ResolveTypeName typeName =
         match typeName with
         |   "M" -> typeof<int>
         |   _ -> null

    interface IDisposable with
        member __.Dispose() = ()
    interface ITypeProvider with
       member this.GetNamespaces() = [| this |]
       member this.GetStaticParameters(typeWithoutArguments) =  [| |]
       member this.ApplyStaticArguments(typeWithoutArguments, typeNameWithArguments, staticArguments) = typeWithoutArguments
       member this.ResolveExtensionReference(name) = null

       member __.GetInvokerExpression(mbase, parameters) = TypeBuilder.StandardGenerativeGetInvokerExpression(mbase, parameters)
       [<CLIEvent>]
       member this.Invalidate = invalidation.Publish
       member this.GetGeneratedAssemblyContents(assembly) = failwith "GetGeneratedAssemblyContents - only erased types were provided!!"
#endif

#if EVIL_PROVIDER_ReturnsTypeWithIncorrectNameFromApplyStaticArguments
[<TypeProvider>]
// Evil in the sense that it returns a generated type that is not nested in a container type
type public EvilGeneratedProviderThatReturnsTypeWithIncorrectNameFromApplyStaticArguments(config: TypeProviderConfig) =
    let modul = typeof<EvilGeneratedProviderThatReturnsTypeWithIncorrectNameFromApplyStaticArguments>.Assembly.GetModules().[0]
    let rootNamespace = "FSharp.EvilGeneratedProviderThatReturnsTypeWithIncorrectNameFromApplyStaticArguments"

    let typeTable = 
        MemoizationTable(fun (_actualName,c) -> 
             let theType = System.Reflection.Assembly.LoadFrom(Path.Combine(Path.GetDirectoryName config.RuntimeAssembly,"provided"+c+".dll")).GetType("TheGeneratedType"+c)
             
             // The error here is that a type with name "TheContainerType" is returned instead of a type with name "actualName"
             let theContainerType = TypeBuilder.CreateSimpleType(TypeContainer.Namespace(modul,rootNamespace), "TheContainerType", members=lazy [| theType |])
             theContainerType)

    let invalidation = new Event<System.EventHandler,_>()

    let theContainerTypeUninstantiated = typeTable.Apply("TheContainerType","")
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
       member this.GetGeneratedAssemblyContents(assembly) = failwith "GetGeneratedAssemblyContents - only erased types were provided!!"
       
#endif
