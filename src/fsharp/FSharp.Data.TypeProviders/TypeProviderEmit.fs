// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

#if TPEMIT_INTERNAL_AND_MINIMAL_FOR_TYPE_CONTAINERS
namespace Internal.Utilities.TypeProvider.Emit
#else
namespace Microsoft.FSharp.TypeProvider.Emit
#endif

open System
open System.Text
open System.IO
open System.Reflection
open System.Reflection.Emit
open System.Collections.Generic
open Microsoft.FSharp.Core.CompilerServices

[<AutoOpen>]
module Misc =
    let nonNull str x = if x=null then failwith ("Null in " + str) else x
    let notRequired opname item = 
        let msg = FSData.SR.invalidOperationOnProvidedType(opname,item)
        System.Diagnostics.Debug.Assert (false, msg)
        raise (System.NotSupportedException msg)

    let mkEditorHideMethodsCustomAttributeData() = 
        { new CustomAttributeData() with 
                member __.Constructor =  typeof<TypeProviderEditorHideMethodsAttribute>.GetConstructors().[0]
                member __.ConstructorArguments = upcast [| |]
                member __.NamedArguments = upcast [| |] }

    /// This makes an xml doc attribute w.r.t. an amortized computation of an xml doc string.
    /// It is important that the text of the xml doc only get forced when poking on the ConstructorArguments
    /// for the CustomAttributeData object.
    let mkXmlDocCustomAttributeDataLazy(lazyText: Lazy<string>) = 
        { new CustomAttributeData() with 
                member __.Constructor =  typeof<TypeProviderXmlDocAttribute>.GetConstructors().[0]
                member __.ConstructorArguments = upcast [| CustomAttributeTypedArgument(typeof<string>, lazyText.Force())  |]
                member __.NamedArguments = upcast [| |] }

    let mkXmlDocCustomAttributeData(s:string) =  mkXmlDocCustomAttributeDataLazy (lazy s)

    let mkDefinitionLocationAttributeCustomAttributeData(line:int,column:int,filePath:string) = 
        { new CustomAttributeData() with 
                member __.Constructor =  typeof<TypeProviderDefinitionLocationAttribute>.GetConstructors().[0]
                member __.ConstructorArguments = upcast [| |]
                member __.NamedArguments = 
                    upcast [| CustomAttributeNamedArgument(typeof<TypeProviderDefinitionLocationAttribute>.GetProperty("FilePath"), CustomAttributeTypedArgument(typeof<string>, filePath));
                              CustomAttributeNamedArgument(typeof<TypeProviderDefinitionLocationAttribute>.GetProperty("Line"), CustomAttributeTypedArgument(typeof<int>, line)) ;
                              CustomAttributeNamedArgument(typeof<TypeProviderDefinitionLocationAttribute>.GetProperty("Column"), CustomAttributeTypedArgument(typeof<int>, column)) 
                           |] }

(*
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
*)

    type CustomAttributesImpl() =
        let customAttributes = ResizeArray<CustomAttributeData>()
        let mutable hideObjectMethods = false
        let mutable xmlDocDelayed = None
        let mutable xmlDocAlwaysRecomputed = None

        // XML doc text that we only compute once, if any. This must _not_ be forced until the ConstructorArguments
        // property of the custom attribute is foced.
        let xmlDocDelayedText = 
            lazy 
                (match xmlDocDelayed with None -> assert false; "" | Some f -> f())

        // Custom atttributes that we only compute once
        let customAttributesOnce = 
            lazy 
               [| if hideObjectMethods then yield mkEditorHideMethodsCustomAttributeData() 
                  match xmlDocDelayed with None -> () | Some _ -> customAttributes.Add(mkXmlDocCustomAttributeDataLazy xmlDocDelayedText) 
                  yield! customAttributes |]

        member __.AddDefinitionLocation(line:int,column:int,filePath:string) = customAttributes.Add(mkDefinitionLocationAttributeCustomAttributeData(line, column, filePath))
        member __.AddXmlDocComputed(xmlDoc : unit -> string) = xmlDocAlwaysRecomputed <- Some xmlDoc
        member __.AddXmlDocDelayed(xmlDoc : unit -> string) = xmlDocDelayed <- Some xmlDoc
        member this.AddXmlDoc(text:string) =  this.AddXmlDocDelayed (fun () -> text)
        member __.HideObjectMethods with set v = hideObjectMethods <- v
        member __.GetCustomAttributesData() = 
            [| yield! customAttributesOnce.Force()
               match xmlDocAlwaysRecomputed with None -> () | Some f -> customAttributes.Add(mkXmlDocCustomAttributeData (f()))  |]
            :> IList<_>


    let transQuotationToCode qexprf (argExprs: Quotations.Expr[]) = 
        let expr = qexprf (Array.toList argExprs)
        let rec trans q = 
            match q with 
            // Eliminate F# property gets to method calls
            | Quotations.Patterns.PropertyGet(obj,propInfo,args) -> 
                match obj with 
                | None -> trans (Quotations.Expr.Call(propInfo.GetGetMethod(),args))
                | Some o -> trans (Quotations.Expr.Call(trans o,propInfo.GetGetMethod(),args))
            // Eliminate F# property sets to method calls
            | Quotations.Patterns.PropertySet(obj,propInfo,args,v) -> 
                 match obj with 
                 | None -> trans (Quotations.Expr.Call(propInfo.GetSetMethod(),args@[v]))
                 | Some o -> trans (Quotations.Expr.Call(trans o,propInfo.GetSetMethod(),args@[v]))
            // Eliminate F# function applications to FSharpFunc<_,_>.Invoke calls
            | Quotations.Patterns.Application(f,e) -> 
                trans (Quotations.Expr.Call(trans f, f.Type.GetMethod "Invoke", [ e ]) )
            | Quotations.Patterns.NewUnionCase(ci, es) ->
                trans (Quotations.Expr.Call(Reflection.FSharpValue.PreComputeUnionConstructorInfo ci, es) )
            | Quotations.Patterns.NewRecord(ci, es) ->
                trans (Quotations.Expr.NewObject(Reflection.FSharpValue.PreComputeRecordConstructorInfo ci, es) )
            | Quotations.Patterns.UnionCaseTest(e,uc) ->
                let tagInfo = Reflection.FSharpValue.PreComputeUnionTagMemberInfo uc.DeclaringType
                let tagExpr = 
                    match tagInfo with 
                    | :? PropertyInfo as tagProp ->
                         trans (Quotations.Expr.PropertyGet(e,tagProp) )
                    | :? MethodInfo as tagMeth -> 
                         if tagMeth.IsStatic then trans (Quotations.Expr.Call(tagMeth, [e]))
                         else trans (Quotations.Expr.Call(e,tagMeth,[]))
                    | _ -> failwith "unreachable: unexpected result from PreComputeUnionTagMemberInfo"
                let tagNumber = uc.Tag
                trans <@@ (%%(tagExpr) : int) = tagNumber @@>



            // Handle the generic cases
            | Quotations.ExprShape.ShapeLambda(v,body) -> 
                Quotations.Expr.Lambda(v, trans body)
            | Quotations.ExprShape.ShapeCombination(comb,args) -> 
                Quotations.ExprShape.RebuildShapeCombination(comb,List.map trans args)
            | Quotations.ExprShape.ShapeVar _ -> q
        trans expr

    let adjustTypeAttributes attributes isNested = 
        let visibilityAttributes = 
            match attributes &&& TypeAttributes.VisibilityMask with 
            | TypeAttributes.Public when isNested -> TypeAttributes.NestedPublic
            | TypeAttributes.NotPublic when isNested -> TypeAttributes.NestedAssembly
            | TypeAttributes.NestedPublic when not isNested -> TypeAttributes.Public
            | TypeAttributes.NestedAssembly 
            | TypeAttributes.NestedPrivate 
            | TypeAttributes.NestedFamORAssem
            | TypeAttributes.NestedFamily
            | TypeAttributes.NestedFamANDAssem when not isNested -> TypeAttributes.NotPublic
            | a -> a
        (attributes &&& ~~~TypeAttributes.VisibilityMask) ||| visibilityAttributes

type ProvidedStaticParameter(parameterName:string,parameterType:Type,?parameterDefaultValue:obj) = 
    inherit System.Reflection.ParameterInfo()

    override __.RawDefaultValue = defaultArg parameterDefaultValue null
    override __.Attributes = if parameterDefaultValue.IsNone then enum 0 else ParameterAttributes.Optional
    override __.Position = 0
    override __.ParameterType = parameterType
    override __.Name = parameterName 

    override __.GetCustomAttributes(_inherit) = ignore(_inherit); notRequired "GetCustomAttributes" parameterName
    override __.GetCustomAttributes(_attributeType, _inherit) = notRequired "GetCustomAttributes" parameterName

type ProvidedParameter(name:string,parameterType:Type,?isOut:bool,?optionalValue:obj) = 
    inherit System.Reflection.ParameterInfo()
    let isOut = defaultArg isOut false
    let customAttributesImpl = CustomAttributesImpl()
    override this.Name = name
    override this.ParameterType = parameterType
    override this.Attributes = (base.Attributes ||| (if isOut then ParameterAttributes.Out else enum 0)
                                                ||| (match optionalValue with None -> enum 0 | Some _ -> ParameterAttributes.Optional ||| ParameterAttributes.HasDefault))
    override this.RawDefaultValue = defaultArg optionalValue null
    override __.GetCustomAttributesData()                 = customAttributesImpl.GetCustomAttributesData()
    override __.GetCustomAttributes(_inherit) = ignore(_inherit); notRequired "GetCustomAttributes" name
    override __.GetCustomAttributes(_attributeType, _inherit) = notRequired "GetCustomAttributes" name


type ProvidedConstructor(parameters : ProvidedParameter list) = 
    inherit ConstructorInfo()
    let parameters  = parameters |> List.map (fun p -> p :> ParameterInfo) 

    let mutable declaringType = null : System.Type
    let mutable invokeCode    = None : option<Quotations.Expr[] -> Quotations.Expr>
    let nameText () = FSData.SR.constructorFor((if declaringType=null then FSData.SR.notYetKnownType() else declaringType.FullName))

    let customAttributesImpl = CustomAttributesImpl()
    member this.AddXmlDocComputed xmlDoc                    = customAttributesImpl.AddXmlDocComputed xmlDoc
    member this.AddXmlDocDelayed xmlDoc                     = customAttributesImpl.AddXmlDocDelayed xmlDoc
    member this.AddXmlDoc xmlDoc                            = customAttributesImpl.AddXmlDoc xmlDoc
    member this.AddDefinitionLocation(line,column,filePath) = customAttributesImpl.AddDefinitionLocation(line, column, filePath)
    member this.HideObjectMethods with set v                = customAttributesImpl.HideObjectMethods <- v
    override this.GetCustomAttributesData()                 = customAttributesImpl.GetCustomAttributesData()

    member this.DeclaringTypeImpl 
        with set x = 
            if declaringType<>null then failwith (FSData.SR.declaringTypeAlreadySet(nameText())); 
            declaringType <- x

    member this.InvokeCode 
        with set (q:Quotations.Expr list -> Quotations.Expr) = this.InvokeCodeInternal <- transQuotationToCode q

    member this.InvokeCodeInternal 
        with get() = 
            match invokeCode with
            | Some f -> f
            | None -> failwith (FSData.SR.pcNoInvoker(nameText()))
        and  set f = 
            match invokeCode with
            | None -> invokeCode <- Some f
            | Some _ -> failwith (FSData.SR.pcCodeAlreadyGiven(nameText()))

    // Implement overloads
    override this.GetParameters() = parameters |> List.toArray 
    override this.Attributes = MethodAttributes.Public ||| MethodAttributes.RTSpecialName
    override this.Name = if this.IsStatic then ".cctor" else ".ctor"
    override this.DeclaringType = declaringType |> nonNull "ProvidedConstructor.DeclaringType"                                   
    override this.IsDefined(_attributeType, _inherit) = true 

    override this.Invoke(_invokeAttr, _binder, _parameters, _culture)      = notRequired "Invoke" (nameText())
    override this.Invoke(_obj, _invokeAttr, _binder, _parameters, _culture) = notRequired "Invoke" (nameText())
    override this.ReflectedType                                        = notRequired "ReflectedType" (nameText())
    override this.GetMethodImplementationFlags()                       = notRequired "GetMethodImplementationFlags" (nameText())
    override this.MethodHandle                                         = notRequired "MethodHandle" (nameText())
    override this.GetCustomAttributes(_inherit)                     = notRequired "GetCustomAttributes" (nameText())
    override this.GetCustomAttributes(_attributeType, _inherit)      = notRequired "GetCustomAttributes" (nameText())

type ProvidedMethod(methodName: string, parameters: ProvidedParameter list, returnType: Type) =
    inherit System.Reflection.MethodInfo()
    let argParams = parameters |> List.map (fun p -> p :> ParameterInfo) 

    // State
    let mutable declaringType : Type = null
    let mutable methodAttrs   = MethodAttributes.Public
    let mutable invokeCode    = None : option<Quotations.Expr[] -> Quotations.Expr>

    let customAttributesImpl = CustomAttributesImpl()
    member this.AddXmlDocComputed xmlDoc                    = customAttributesImpl.AddXmlDocComputed xmlDoc
    member this.AddXmlDocDelayed xmlDoc                     = customAttributesImpl.AddXmlDocDelayed xmlDoc
    member this.AddXmlDoc xmlDoc                            = customAttributesImpl.AddXmlDoc xmlDoc
    member this.AddDefinitionLocation(line,column,filePath) = customAttributesImpl.AddDefinitionLocation(line, column, filePath)
    override this.GetCustomAttributesData()                 = customAttributesImpl.GetCustomAttributesData()

    member this.SetMethodAttrs m = methodAttrs <- m
    member this.DeclaringTypeImpl with set x = declaringType <- x // check: not set twice
    member this.IsStaticMethod 
        with get()  = methodAttrs.HasFlag(MethodAttributes.Static)
        and set x = if x then methodAttrs <- methodAttrs ||| MethodAttributes.Static
                    else methodAttrs <- methodAttrs &&& (~~~ MethodAttributes.Static)
    member this.InvokeCode 
        with set  (q:Quotations.Expr list -> Quotations.Expr) = this.InvokeCodeInternal <- transQuotationToCode q

    member this.InvokeCodeInternal 
        with get() = 
            match invokeCode with
            | Some f -> f
            | None -> failwith (FSData.SR.pmNoInvokerName(this.Name,(if declaringType=null then FSData.SR.notYetKnownType() else declaringType.FullName)))
        and  set f = 
            match invokeCode with
            | None -> invokeCode <- Some f
            | Some _ -> failwith (FSData.SR.pcNoInvokerName(this.Name,(if declaringType=null then FSData.SR.notYetKnownType() else declaringType.FullName)))

    // Implement overloads
    override this.GetParameters() = argParams |> Array.ofList
    override this.Attributes = methodAttrs
    override this.Name = methodName
    override this.DeclaringType = declaringType |> nonNull "ProvidedMethod.DeclaringType"                                   
    override this.IsDefined(_attributeType, _inherit) : bool = true
    override this.MemberType = MemberTypes.Method
    override this.CallingConvention = 
        let cc = CallingConventions.Standard
        let cc = if not (this.IsStatic) then cc ||| CallingConventions.HasThis else cc
        cc
    override this.ReturnType = returnType
    override this.ReturnParameter = null // REVIEW: Give it a name and type?
    override this.ToString() = "Method " + this.Name

    override this.ReturnTypeCustomAttributes                           = notRequired "ReturnTypeCustomAttributes" this.Name
    override this.GetBaseDefinition()                                  = notRequired "GetBaseDefinition" this.Name
    override this.GetMethodImplementationFlags()                       = notRequired "GetMethodImplementationFlags" this.Name
    override this.MethodHandle                                         = notRequired "MethodHandle" this.Name
    override this.Invoke(_obj, _invokeAttr, _binder, _parameters, _culture) = notRequired "Invoke" this.Name
    override this.ReflectedType                                        = notRequired "ReflectedType" this.Name
    override this.GetCustomAttributes(_inherit)                     = notRequired "GetCustomAttributes" this.Name
    override this.GetCustomAttributes(_attributeType, _inherit)      =  notRequired "GetCustomAttributes" this.Name


type ProvidedProperty(propertyName:string,propertyType:Type, ?parameters:ProvidedParameter list) = 
    inherit System.Reflection.PropertyInfo()
    // State

    let parameters = defaultArg parameters []
    let mutable declaringType = null
    let mutable isStatic = false
    let mutable getterCode = None : option<Quotations.Expr[] -> Quotations.Expr>
    let mutable setterCode = None : option<Quotations.Expr[] -> Quotations.Expr>

    let hasGetter() = getterCode.IsSome
    let hasSetter() = setterCode.IsSome

    // Delay construction - to pick up the latest isStatic
    let markSpecialName (m:ProvidedMethod) = m.SetMethodAttrs(m.Attributes ||| MethodAttributes.SpecialName); m
    let getter = lazy (ProvidedMethod("get_" + propertyName,parameters,propertyType,IsStaticMethod=isStatic,DeclaringTypeImpl=declaringType,InvokeCodeInternal=getterCode.Value) |> markSpecialName)  
    let setter = lazy (ProvidedMethod("set_" + propertyName,parameters @ [ProvidedParameter("value",propertyType)],typeof<System.Void>,IsStaticMethod=isStatic,DeclaringTypeImpl=declaringType,InvokeCodeInternal=setterCode.Value) |> markSpecialName) 

    let customAttributesImpl = CustomAttributesImpl()
    member this.AddXmlDocComputed xmlDoc                    = customAttributesImpl.AddXmlDocComputed xmlDoc
    member this.AddXmlDocDelayed xmlDoc                     = customAttributesImpl.AddXmlDocDelayed xmlDoc
    member this.AddXmlDoc xmlDoc                            = customAttributesImpl.AddXmlDoc xmlDoc
    member this.AddDefinitionLocation(line,column,filePath) = customAttributesImpl.AddDefinitionLocation(line, column, filePath)
    override this.GetCustomAttributesData()                 = customAttributesImpl.GetCustomAttributesData()

    member this.DeclaringTypeImpl with set x = declaringType <- x // check: not set twice
    member this.IsStatic 
        with get()  = isStatic
        and set x = isStatic <- x

    member this.GetterCode 
        with set  (q:Quotations.Expr list -> Quotations.Expr) = this.GetterCodeInternal <- transQuotationToCode q

    member this.GetterCodeInternal 
        with get() = getterCode.Value
        and  set f = 
            if not getter.IsValueCreated then getterCode <- Some f else failwith (FSData.SR.ppGetterAlreadyCreated())

    member this.SetterCode 
        with set (q:Quotations.Expr list -> Quotations.Expr) = this.SetterCodeInternal <- transQuotationToCode q

    member this.SetterCodeInternal 
        with get() = setterCode.Value
        and  set f = 
            if not (setter.IsValueCreated) then setterCode <- Some f else failwith (FSData.SR.ppSetterAlreadyCreated())

    // Implement overloads
    override this.PropertyType = propertyType
    override this.SetValue(_obj, _value, _invokeAttr, _binder, _index, _culture) = notRequired "SetValue" this.Name
    override this.GetAccessors _nonPublic  = notRequired "nonPublic" this.Name
    override this.GetGetMethod _nonPublic = if hasGetter() then getter.Force() :> MethodInfo else null
    override this.GetSetMethod _nonPublic = if hasSetter() then setter.Force() :> MethodInfo else null
    override this.GetIndexParameters() = [| for p in parameters -> upcast p |]
    override this.Attributes = PropertyAttributes.None
    override this.CanRead = hasGetter()
    override this.CanWrite = hasSetter()
    override this.GetValue(_obj, _invokeAttr, _binder, _index, _culture) : obj = notRequired "GetValue" this.Name
    override this.Name = propertyName
    override this.DeclaringType = declaringType |> nonNull "ProvidedProperty.DeclaringType"
    override this.MemberType : MemberTypes = MemberTypes.Property

    override this.ReflectedType                                     = notRequired "ReflectedType" this.Name
    override this.GetCustomAttributes(_inherit)                  = notRequired "GetCustomAttributes" this.Name
    override this.GetCustomAttributes(_attributeType, _inherit)   = notRequired "GetCustomAttributes" this.Name
    override this.IsDefined(_attributeType, _inherit)             = notRequired "IsDefined" this.Name



/// Represents the type constructor in a provided symbol type.
type SymbolKind = SDArray | Array of int | Pointer | ByRef | Generic of System.Type


/// Represents an array or other symbolic type involving a provided type as the argument.
/// See the type provider spec for the methods that must be implemented.
/// Note that the type provider specification does not require us to implement pointer-equality for provided types.
type ProvidedSymbolType(kind: SymbolKind, args: Type list) =
    inherit Type()

    override this.FullName =   
        match kind,args with 
        | SymbolKind.SDArray,[arg] -> arg.FullName + "[]" 
        | SymbolKind.Array _,[arg] -> arg.FullName + "[*]" 
        | SymbolKind.Pointer,[arg] -> arg.FullName + "*" 
        | SymbolKind.ByRef,[arg] -> arg.FullName + "&"
        | SymbolKind.Generic gty, args -> gty.FullName + args.ToString()
        | _ -> failwith (FSData.SR.unreachable())
   
    /// Although not strictly required by the type provider specification, this is required when doing basic operations like FullName on
    /// .NET symbolic types made from this type, e.g. when building Nullable<SomeProvidedType[]>.FullName
    override this.DeclaringType =                                                                 
        match kind,args with 
        | SymbolKind.SDArray,[arg] -> arg
        | SymbolKind.Array _,[arg] -> arg
        | SymbolKind.Pointer,[arg] -> arg
        | SymbolKind.ByRef,[arg] -> arg
        | SymbolKind.Generic gty,_ -> gty
        | _ -> failwith (FSData.SR.unreachable())

    override this.Name =
        match kind,args with 
        | SymbolKind.SDArray,[arg] -> arg.Name + "[]" 
        | SymbolKind.Array _,[arg] -> arg.Name + "[*]" 
        | SymbolKind.Pointer,[arg] -> arg.Name + "*" 
        | SymbolKind.ByRef,[arg] -> arg.Name + "&"
        | SymbolKind.Generic gty, args -> gty.FullName + args.ToString()
        | _ -> failwith (FSData.SR.unreachable())

    override this.BaseType =
        match kind with 
        | SymbolKind.SDArray -> typeof<System.Array>
        | SymbolKind.Array _ -> typeof<System.Array>
        | SymbolKind.Pointer -> typeof<System.ValueType>
        | SymbolKind.ByRef -> typeof<System.ValueType>
        | SymbolKind.Generic gty  -> gty.BaseType

    override this.GetArrayRank() = (match kind with SymbolKind.Array n -> n | SymbolKind.SDArray -> 1 | _ -> invalidOp (FSData.SR.nonArrayType()))
    override this.IsArrayImpl() = (match kind with SymbolKind.Array _ | SymbolKind.SDArray -> true | _ -> false)
    override this.IsByRefImpl() = (match kind with SymbolKind.ByRef _ -> true | _ -> false)
    override this.IsPointerImpl() = (match kind with SymbolKind.Pointer _ -> true | _ -> false)
    override this.IsPrimitiveImpl() = false
    override this.IsGenericType = (match kind with SymbolKind.Generic _ -> true | _ -> false)
    override this.GetGenericArguments() = (match kind with SymbolKind.Generic _ -> args |> List.toArray | _ -> invalidOp (FSData.SR.nonGenericType()))
    override this.GetGenericTypeDefinition() = (match kind with SymbolKind.Generic e -> e | _ -> invalidOp (FSData.SR.nonGenericType()))
    override this.IsCOMObjectImpl() = false
    override this.HasElementTypeImpl() = (match kind with SymbolKind.Generic _ -> false | _ -> true)
    override this.GetElementType() = (match kind,args with (SymbolKind.Array _  | SymbolKind.SDArray | SymbolKind.ByRef | SymbolKind.Pointer),[e] -> e | _ -> invalidOp (FSData.SR.notAnArrayPointerOrByref()))
    override this.ToString() = this.FullName

    override this.Module : Module                                                                  = notRequired "Module" this.Name
    override this.Assembly                                                                         = notRequired "Assembly" this.Name
    override this.Namespace                                                                        = notRequired "Namespace" this.Name
    override this.GetConstructors _bindingAttr                                                      = notRequired "GetConstructors" this.Name
    override this.GetMethodImpl(_name, _bindingAttr, _binderBinder, _callConvention, _types, _modifiers) = notRequired "GetMethodImpl" this.Name
    override this.GetMembers _bindingAttr                                                           = notRequired "GetMembers" this.Name
    override this.GetMethods _bindingAttr                                                           = notRequired "GetMethods" this.Name
    override this.GetField(_name, _bindingAttr)                                                      = notRequired "GetField" this.Name
    override this.GetFields _bindingAttr                                                            = notRequired "GetFields" this.Name
    override this.GetInterface(_name, _ignoreCase)                                                   = notRequired "GetInterface" this.Name
    override this.GetInterfaces()                                                                  = notRequired "GetInterfaces" this.Name
    override this.GetEvent(_name, _bindingAttr)                                                      = notRequired "GetEvent" this.Name
    override this.GetEvents _bindingAttr                                                            = notRequired "GetEvents" this.Name
    override this.GetProperties _bindingAttr                                                        = notRequired "GetProperties" this.Name
    override this.GetPropertyImpl(_name, _bindingAttr, _binder, _returnType, _types, _modifiers)         = notRequired "GetPropertyImpl" this.Name
    override this.GetNestedTypes _bindingAttr                                                       = notRequired "GetNestedTypes" this.Name
    override this.GetNestedType(_name, _bindingAttr)                                                 = notRequired "GetNestedType" this.Name
    override this.GetAttributeFlagsImpl()                                                          = notRequired "GetAttributeFlagsImpl" this.Name
    override this.UnderlyingSystemType                                                             = notRequired "UnderlyingSystemType" this.Name
    override this.GetCustomAttributesData()                                                        = notRequired "GetCustomAttributesData" this.Name
    override this.MemberType                                                                       = notRequired "MemberType" this.Name
    override this.GetHashCode()                                                                    = notRequired "GetHashCode" this.Name
    override this.Equals(_that:obj) : bool                                                          = notRequired "Equals" this.Name
    override this.GetMember(_name,_mt,_bindingAttr)                                                   = notRequired "GetMember" this.Name
    override this.GUID                                                                             = notRequired "GUID" this.Name
    override this.InvokeMember(_name, _invokeAttr, _binder, _target, _args, _modifiers, _culture, _namedParameters) = notRequired "InvokeMember" this.Name
    override this.AssemblyQualifiedName                                                            = notRequired "AssemblyQualifiedName" this.Name
    override this.GetConstructorImpl(_bindingAttr, _binder, _callConvention, _types, _modifiers)        = notRequired "GetConstructorImpl" this.Name
    override this.GetCustomAttributes(_inherit)                                                 = notRequired "GetCustomAttributes" this.Name
    override this.GetCustomAttributes(_attributeType, _inherit)                                  = notRequired "GetCustomAttributes" this.Name
    override this.IsDefined(_attributeType, _inherit)                                            = notRequired "IsDefined" this.Name



[<Class>]
type ProvidedMeasureBuilder() =

    static let theBuilder = ProvidedMeasureBuilder()
    static member Default = theBuilder
    member b.One = typeof<Core.CompilerServices.MeasureOne> 
    member b.Product (m1,m2) = typedefof<Core.CompilerServices.MeasureProduct<_,_>>.MakeGenericType [| m1;m2 |] 
    member b.Inverse m = typedefof<Core.CompilerServices.MeasureInverse<_>>.MakeGenericType [| m |] 
    member b.Ratio (m1, m2) = b.Product(m1, b.Inverse m2)
    member b.Square m = b.Product(m, m)
    member b.SI m = 
        match typedefof<list<int>>.Assembly.GetType("Microsoft.FSharp.Data.UnitSystems.SI.UnitNames."+m) with 
        | null -> failwith (FSData.SR.unitNotFound(m)) 
        | v -> v
    member b.AnnotateType (basicType, annotation) = ProvidedSymbolType(Generic basicType, annotation) :> Type


[<RequireQualifiedAccess>]
type TypeContainer =
  | Namespace of Assembly * string // namespace
  | Type of System.Type
  | TypeToBeDecided

module GlobalProvidedAssemblyElementsTable = 
    let theTable = Dictionary<Assembly, byte[]>()

type ProvidedTypeDefinition(container:TypeContainer,className : string, baseType  : Type option) as this =
    inherit Type()
    // state
    let mutable attributes   = 
        TypeAttributes.Public ||| 
        TypeAttributes.Class ||| 
        TypeAttributes.Sealed |||
        enum (int32 TypeProviderTypeAttributes.IsErased)

    
    let mutable baseType   = baseType
    let mutable membersKnown   = ResizeArray<MemberInfo>()
    let mutable membersQueue   = ResizeArray<(unit -> list<MemberInfo>)>()       
    let mutable staticParams = [ ] 
    let mutable staticParamsApply = None
    let mutable container = container
    let mutable interfaceImpls = ResizeArray<Type>()
    let mutable methodOverrides = ResizeArray<ProvidedMethod * MethodInfo>()

    // members API
    let getMembers() = 
        if membersQueue.Count > 0 then 
            let elems = membersQueue |> Seq.toArray // take a copy in case more elements get added
            membersQueue.Clear()
            for  f in elems do
                for i in f() do 
                    membersKnown.Add i       
                    match i with
                    | :? ProvidedProperty    as p -> 
                        if p.CanRead then membersKnown.Add (p.GetGetMethod true)
                        if p.CanWrite then membersKnown.Add (p.GetSetMethod true)
                    | _ -> ()
        
        membersKnown.ToArray()

    let mutable theAssembly = 
      lazy
        match container with
        | TypeContainer.Namespace (theAssembly, rootNamespace) ->
            if theAssembly = null then failwith "Null assemblies not allowed"
            if rootNamespace<>null && rootNamespace.Length=0 then failwith "Use 'null' for global namespace"
            theAssembly
        | TypeContainer.Type superTy -> superTy.Assembly
        | TypeContainer.TypeToBeDecided -> failwith (sprintf "type '%s' was not added as a member to a declaring type" this.Name)
    
    let rootNamespace =
      lazy 
        match container with
        | TypeContainer.Namespace (_,rootNamespace) -> rootNamespace
        | TypeContainer.Type enclosingTyp           -> enclosingTyp.Namespace
        | TypeContainer.TypeToBeDecided -> failwith (FSData.SR.typeNotAddedAsAMember(this.Name))

    let declaringType =
      lazy
        match container with
        | TypeContainer.Namespace _ -> null
        | TypeContainer.Type enclosingTyp           -> enclosingTyp
        | TypeContainer.TypeToBeDecided -> failwith (FSData.SR.typeNotAddedAsAMember(this.Name))

    let fullName = 
      lazy
        match container with
        | TypeContainer.Type declaringType -> declaringType.FullName + "+" + className
        | TypeContainer.Namespace (_,namespaceName) ->  
            if namespaceName="" then failwith (FSData.SR.useNullForGlobalNamespace())
            match namespaceName with
            | null -> className
            | _    -> namespaceName + "." + className
        | TypeContainer.TypeToBeDecided -> failwith (FSData.SR.typeNotAddedAsAMember(this.Name))
                                                            
    let patchUpAddedMemberInfo (this:Type) (m:MemberInfo) = 
        match m with
        | :? ProvidedConstructor as c -> c.DeclaringTypeImpl <- this // patch up "declaring type" on provided MethodInfo
        | :? ProvidedMethod      as m -> m.DeclaringTypeImpl <- this // patch up "declaring type" on provided MethodInfo
        | :? ProvidedProperty    as p -> p.DeclaringTypeImpl <- this // patch up "declaring type" on provided MethodInfo
        | :? ProvidedTypeDefinition  as t -> t.DeclaringTypeImpl <- this 
        | _ -> ()

    let customAttributesImpl = CustomAttributesImpl()
    member this.AddXmlDocComputed xmlDoc                    = customAttributesImpl.AddXmlDocComputed xmlDoc
    member this.AddXmlDocDelayed xmlDoc                     = customAttributesImpl.AddXmlDocDelayed xmlDoc
    member this.AddXmlDoc xmlDoc                            = customAttributesImpl.AddXmlDoc xmlDoc
    member this.AddDefinitionLocation(line,column,filePath) = customAttributesImpl.AddDefinitionLocation(line, column, filePath)
    member this.HideObjectMethods with set v                = customAttributesImpl.HideObjectMethods <- v
    override this.GetCustomAttributesData()                 = customAttributesImpl.GetCustomAttributesData()

    new (assembly:Assembly,namespaceName,className,baseType) = new ProvidedTypeDefinition(TypeContainer.Namespace (assembly,namespaceName), className, baseType)
    new (className,baseType) = new ProvidedTypeDefinition(TypeContainer.TypeToBeDecided, className, baseType)
    // state ops

    member this.SetBaseType t = baseType <- Some t
    member this.SetAttributes x = attributes <- x
    // Add MemberInfos
    member this.AddMembersDelayed(makeMS : unit -> list<#MemberInfo>) =
        membersQueue.Add (fun () -> makeMS() |> List.map (fun x -> patchUpAddedMemberInfo this x; x :> MemberInfo ))
    member this.AddMembers(ms:list<#MemberInfo>) = (* strict *)
        ms |> List.iter (patchUpAddedMemberInfo this) // strict: patch up now
        membersQueue.Add (fun () -> ms |> List.map (fun x -> x :> MemberInfo))
    member this.AddMember(m:MemberInfo) = this.AddMembers [m]    
    member this.AddMemberDelayed(m : unit -> #MemberInfo) = this.AddMembersDelayed(fun () -> [m()])

    member this.AddAssemblyTypesAsNestedTypesDelayed (assemblyf : unit -> System.Reflection.Assembly)  = 
            let bucketByPath nodef tipf (items: (string list * 'Value) list) = 
                // Find all the items with an empty key list and call 'tipf' 
                let tips = 
                    [ for (keylist,v) in items do 
                         match keylist with 
                         | [] -> yield tipf v
                         | _ -> () ]

                // Find all the items with a non-empty key list. Bucket them together by
                // the first key. For each bucket, call 'nodef' on that head key and the bucket.
                let nodes = 
                    let buckets = new Dictionary<_,_>(10)
                    for (keylist,v) in items do
                        match keylist with 
                        | [] -> ()
                        | key::rest -> 
                            buckets.[key] <- (rest,v) :: (if buckets.ContainsKey key then buckets.[key] else []);

                    [ for (KeyValue(key,items)) in buckets -> nodef key items ]

                tips @ nodes
            this.AddMembersDelayed (fun _ -> 
                let topTypes = [ for ty in assemblyf().GetTypes() do 
                                        if not ty.IsNested then
                                             let namespaceParts = match ty.Namespace with null -> [] | s -> s.Split '.' |> Array.toList
                                             yield namespaceParts,  ty ]
                let rec loop types = 
                    types 
                    |> bucketByPath
                        (fun namespaceComponent typesUnderNamespaceComponent -> 
                            let t = ProvidedTypeDefinition(namespaceComponent, baseType = Some typeof<obj>)
                            t.AddMembers (loop typesUnderNamespaceComponent)
                            (t :> Type))
                        (fun ty -> ty)
                loop topTypes)

    /// Abstract a type to a parametric-type. Requires "formal parameters" and "instantiation function".
    member this.DefineStaticParameters(staticParameters : list<ProvidedStaticParameter>, apply    : (string -> obj[] -> ProvidedTypeDefinition)) =
        staticParams      <- staticParameters 
        staticParamsApply <- Some apply

    /// Get ParameterInfo[] for the parametric type parameters (//s GetGenericParameters)
    member this.GetStaticParameters() = [| for p in staticParams -> p :> ParameterInfo |]

    /// Instantiate parametrics type
    member this.MakeParametricType(name:string,args:obj[]) =
        if staticParams.Length>0 then
            if staticParams.Length <> args.Length then
                failwith (FSData.SR.pdErrorExpectingStaticParameters(staticParams.Length,args.Length,(fullName.Force())))
            match staticParamsApply with
            | None -> failwith (FSData.SR.pdDefineStaticParametersNotCalled())
            | Some f -> f name args

        else
            failwith (FSData.SR.ptdStaticParametersSuppliedButNotExpected(fullName.Force()))

    member this.DeclaringTypeImpl
        with set x = 
            match container with TypeContainer.TypeToBeDecided -> () | _ -> failwith (FSData.SR.containerTypeAlreadySet(this.FullName,x.FullName));
            container <- TypeContainer.Type  x

    // Implement overloads
    override this.Assembly = theAssembly.Force()
    member this.SetAssembly assembly = theAssembly <- lazy assembly
    override this.FullName = fullName.Force()
    override this.Namespace = rootNamespace.Force()
    override this.BaseType = match baseType with Some ty -> ty | None -> null
    // Constructors
    override this.GetConstructors bindingAttr = 
        [| for m in this.GetMembers bindingAttr do                
                if m.MemberType = MemberTypes.Constructor then
                    yield (m :?> ConstructorInfo) |]
    // Methods
    override this.GetMethodImpl(name, _bindingAttr, _binderBinder, _callConvention, _types, _modifiers) : MethodInfo = 
        let membersWithName = 
            [ for m in getMembers() do                
                if m.MemberType.HasFlag(MemberTypes.Method) && m.Name = name then
                    yield  m ]
        match membersWithName with 
        | []        -> null
        | [meth]    -> meth :?> MethodInfo
        | _several   -> failwith (FSData.SR.getMethodImplDoesNotSupportOverloads())

    override this.GetMethods bindingAttr = 
        this.GetMembers bindingAttr 
        |> Array.filter (fun m -> m.MemberType.HasFlag(MemberTypes.Method)) 
        |> Array.map (fun m -> m :?> MethodInfo)

    // Fields
    override this.GetField(name, bindingAttr) = 
        let fields = [| for m in this.GetMembers bindingAttr do
                            if m.MemberType.HasFlag(MemberTypes.Field) && (name = null || m.Name = name) then // REVIEW: name = null. Is that a valid query?!
                                yield m |] 
        if fields.Length > 0 then fields.[0] :?> FieldInfo else null

    override this.GetFields bindingAttr = 
        [| for m in this.GetMembers bindingAttr do if m.MemberType.HasFlag(MemberTypes.Field) then yield m :?> FieldInfo |]


    override this.GetInterface(_name, _ignoreCase) = notRequired "GetInterface" this.Name

    override this.GetInterfaces() = 
        [| yield! interfaceImpls  |]

    member this.GetInterfaceImplementations() = 
        [| yield! interfaceImpls |]

    member this.AddInterfaceImplementation ityp = interfaceImpls.Add ityp
    member this.GetMethodOverrides() = 
        [| yield! methodOverrides |]

    member this.DefineMethodOverride (bodyMethInfo,declMethInfo) = methodOverrides.Add (bodyMethInfo, declMethInfo)

    // Events
    override this.GetEvent(name, bindingAttr) = 
        let events = this.GetMembers bindingAttr 
                     |> Array.filter(fun m -> m.MemberType.HasFlag(MemberTypes.Event) && (name = null || m.Name = name)) 
        if events.Length > 0 then events.[0] :?> EventInfo else null

    override this.GetEvents bindingAttr = 
        [| for m in this.GetMembers bindingAttr do if m.MemberType.HasFlag(MemberTypes.Event) then yield downcast m |]    

    // Properties
    override this.GetProperties bindingAttr = 
        [| for m in this.GetMembers bindingAttr do if m.MemberType.HasFlag(MemberTypes.Property) then yield downcast m |]

    override this.GetPropertyImpl(name, bindingAttr, binder, returnType, types, modifiers) = 
        if returnType <> null then failwith (FSData.SR.gpiNeedToHandleSpecifiedReturnType())
        if types      <> null then failwith (FSData.SR.gpiNeedToHandleSpecifiedParameterTypes())
        if modifiers  <> null then failwith (FSData.SR.gpiNeedToHandleSpecifiedModifiers())
        if binder  <> null then failwith (FSData.SR.gpiNeedToHandleBinder())
        let props = this.GetMembers bindingAttr |> Array.filter(fun m -> m.MemberType.HasFlag(MemberTypes.Property) && (name = null || m.Name = name))  // Review: nam = null, valid query!?
        if props.Length > 0 then
            props.[0] :?> PropertyInfo
        else
            null
    // Nested Types
    override this.MakeArrayType() = ProvidedSymbolType(SymbolKind.SDArray, [this]) :> Type
    override this.MakeArrayType arg = ProvidedSymbolType(SymbolKind.Array arg, [this]) :> Type
    override this.MakePointerType() = ProvidedSymbolType(SymbolKind.Pointer, [this]) :> Type
    override this.MakeByRefType() = ProvidedSymbolType(SymbolKind.ByRef, [this]) :> Type

    override this.GetMembers _bindingAttr = getMembers() 

    override this.GetNestedTypes bindingAttr = 
        this.GetMembers bindingAttr 
        |> Array.filter(fun m -> 
            m.MemberType.HasFlag(MemberTypes.NestedType) || 
            // Allow 'fake' nested types that are actually real .NET types
            m.MemberType.HasFlag(MemberTypes.TypeInfo)) |> Array.map(fun m -> m :?> Type)

    override this.GetMember(name,mt,_bindingAttr) = 
        let mt = 
            if mt &&& MemberTypes.NestedType = MemberTypes.NestedType then 
                mt ||| MemberTypes.TypeInfo 
            else
                mt
        getMembers() |> Array.filter(fun m->0<>(int(m.MemberType &&& mt)) && m.Name = name)
        
    override this.GetNestedType(name, bindingAttr) = 
        let nt = this.GetMember(name, MemberTypes.NestedType ||| MemberTypes.TypeInfo, bindingAttr)
        match nt.Length with
        | 0 -> null
        | 1 -> downcast nt.[0]
        | _ -> failwith (FSData.SR.moreThanOneNestedType(name,this.FullName))

    // Attributes, etc..
    override this.GetAttributeFlagsImpl() = adjustTypeAttributes attributes this.IsNested 
    override this.IsArrayImpl() = false
    override this.IsByRefImpl() = false
    override this.IsPointerImpl() = false
    override this.IsPrimitiveImpl() = false
    override this.IsCOMObjectImpl() = false
    override this.HasElementTypeImpl() = false
    override this.UnderlyingSystemType = typeof<System.Type>
    override this.Name = className
    override this.DeclaringType = declaringType.Force()
    override this.MemberType = if this.IsNested then MemberTypes.NestedType else MemberTypes.TypeInfo      
    override this.GetHashCode() = rootNamespace.GetHashCode() ^^^ className.GetHashCode()
    override this.Equals(that:obj) = 
        match that with
        | null              -> false
        | :? ProvidedTypeDefinition as ti -> System.Object.ReferenceEquals(this,ti)
        | _                 -> false

    override this.GetGenericArguments() = [||] 
    override this.ToString() = this.FullName
    

    override this.Module : Module                                                                        = notRequired "Module" this.Name
    override this.GUID                                                                                   = Guid.Empty
    override this.GetConstructorImpl(_bindingAttr, _binder, _callConvention, _types, _modifiers)         = null
    override this.GetCustomAttributes(_inherit)                                                          = [| |]
    override this.GetCustomAttributes(_attributeType, _inherit)                                          = [| |]
    override this.IsDefined(_attributeType: Type, _inherit)                                              = false

    override this.GetElementType()                                                                                  = notRequired "Module" this.Name
    override this.InvokeMember(_name, _invokeAttr, _binder, _target, _args, _modifiers, _culture, _namedParameters) = notRequired "Module" this.Name
    override this.AssemblyQualifiedName                                                                             = notRequired "Module" this.Name

    member this.IsErased 
        with get() = (attributes &&& enum (int32 TypeProviderTypeAttributes.IsErased)) <> enum 0
        and set v = 
           if v then attributes <- attributes ||| enum (int32 TypeProviderTypeAttributes.IsErased)
           else attributes <- attributes &&& ~~~(enum (int32 TypeProviderTypeAttributes.IsErased))

    member this.SuppressRelocation 
        with get() = (attributes &&& enum (int32 TypeProviderTypeAttributes.SuppressRelocate)) <> enum 0
        and set v = 
           if v then attributes <- attributes ||| enum (int32 TypeProviderTypeAttributes.SuppressRelocate)
           else attributes <- attributes &&& ~~~(enum (int32 TypeProviderTypeAttributes.SuppressRelocate))

    /// Emit the given provided type definitions into an assembly and adjust 'Assembly' property of all type definitions to return that
    /// assembly.
    member this.ConvertToGenerated (assemblyFileName: string, ?reportAssemblyElements) = 
        if this.IsErased then invalidOp ("The provided type "+this.Name+"is marked as erased and cannot be converted to a generated type. Set 'IsErased' to false on the ProvidedTypeDefinition")
        let typeDefinitions = [this]
        let theElementsLazy = 
           lazy 
              let assemblyShortName = Path.GetFileNameWithoutExtension assemblyFileName
              let assemblyName = AssemblyName assemblyShortName
              let assembly = 
                  System.AppDomain.CurrentDomain.DefineDynamicAssembly(name=assemblyName,access=AssemblyBuilderAccess.Save,dir=Path.GetDirectoryName assemblyFileName)
              let assemblyMainModule = 
                  assembly.DefineDynamicModule("MainModule", Path.GetFileName assemblyFileName)
              let typeMap = Dictionary(HashIdentity.Reference)

              // phase 1 - set assembly fields and emit type definitions
              begin 
                  let rec typeMembers (tb:TypeBuilder)  (td : ProvidedTypeDefinition) = 
                      for ntd in td.GetNestedTypes(BindingFlags.Public ||| BindingFlags.NonPublic) do
                          nestedType tb ntd

                  and nestedType (tb:TypeBuilder)  (ntd : Type) = 
                      match ntd with 
                      | :? ProvidedTypeDefinition as pntd -> 
                          if pntd.IsErased then invalidOp ("The nested provided type "+pntd.Name+"is marked as erased and cannot be converted to a generated type. Set 'IsErased' to false on the ProvidedTypeDefinition")
                          // Adjust the attributes - we're codegen'ing this type as nested
                          let attributes = adjustTypeAttributes ntd.Attributes true
                          let ntb = tb.DefineNestedType(pntd.Name,attr=attributes)
                          pntd.SetAssembly null
                          typeMap.[pntd] <- ntb
                          typeMembers ntb pntd
                      | _ -> ()
                     
                  for td in typeDefinitions do 
                      // Filter out the additional TypeProviderTypeAttributes flags
                      let attributes = td.Attributes &&& ~~~(enum (int32 TypeProviderTypeAttributes.SuppressRelocate))
                                                     &&& ~~~(enum (int32 TypeProviderTypeAttributes.IsErased))
                      // Adjust the attributes - we're codegen'ing as non-nested
                      let attributes = adjustTypeAttributes attributes false                       
                      let tb = assemblyMainModule.DefineType(name=td.FullName,attr= attributes) 
                      td.SetAssembly null
                      typeMap.[td] <- tb
                      typeMembers tb td 
              end
              let rec convType (ty:Type) = 
                  match ty with 
                  | :? ProvidedTypeDefinition as ptd ->   
                      if typeMap.ContainsKey ptd then typeMap.[ptd] :> Type else ty
                  | _ -> 
                      if ty.IsGenericType then ty.GetGenericTypeDefinition().MakeGenericType (Array.map convType (ty.GetGenericArguments()))
                      elif ty.HasElementType then 
                         let ety = convType (ty.GetElementType()) 
                         if ty.IsArray then 
                             let rank = ty.GetArrayRank()
                             if rank = 1 then ety.MakeArrayType() 
                             else ety.MakeArrayType rank 
                          elif ty.IsPointer then ety.MakePointerType() 
                          elif ty.IsByRef then ety.MakeByRefType()
                          else ty
                      else ty

              let ctorMap = Dictionary<ProvidedConstructor, ConstructorBuilder>(HashIdentity.Reference)
              let methMap = Dictionary<ProvidedMethod, MethodBuilder>(HashIdentity.Reference)

              let iterateTypes f = 
                  let rec typeMembers (ptd : ProvidedTypeDefinition) = 
                      let tb = typeMap.[ptd] 
                      f tb ptd
                      for ntd in ptd.GetNestedTypes(BindingFlags.Public ||| BindingFlags.NonPublic) do
                          nestedType ntd

                  and nestedType (ntd : Type) = 
                      match ntd with 
                      | :? ProvidedTypeDefinition as pntd -> typeMembers pntd
                      | _ -> ()
                     
                  for td in typeDefinitions do 
                      typeMembers td 

              // phase 2 - emit member definitions
              iterateTypes (fun tb ptd -> 
                  let defineMeth (minfo:MethodInfo) = 
                      match minfo with 
                      | :? ProvidedMethod as pminfo when not (methMap.ContainsKey pminfo)  -> 
                          let mb = tb.DefineMethod(minfo.Name, minfo.Attributes, convType minfo.ReturnType, [| for p in minfo.GetParameters() -> convType p.ParameterType |])
                          //, CallingConventions.Standard, [| for p in cinfo.GetParameters() -> convType p.ParameterType |])
                          methMap.[pminfo] <- mb
                      | _ -> () 
                  for cinfo in ptd.GetConstructors(BindingFlags.Public ||| BindingFlags.NonPublic) do
                      match cinfo with 
                      | :? ProvidedConstructor as pcinfo when not (ctorMap.ContainsKey pcinfo)  -> 
                          let cb = tb.DefineConstructor(cinfo.Attributes, CallingConventions.Standard, [| for p in cinfo.GetParameters() -> convType p.ParameterType |])
                          ctorMap.[pcinfo] <- cb
                      | _ -> () 
                    
                  for minfo in ptd.GetMethods(BindingFlags.Public ||| BindingFlags.NonPublic) do
                      defineMeth minfo

                  for ityp in ptd.GetInterfaceImplementations() do
                      tb.AddInterfaceImplementation ityp)

              // phase 3 - emit member code
              iterateTypes (fun  tb ptd -> 
                  // Allow at most one constructor, and use its arguments as the fields of the type
                  let ctorArgs, ctorInfoOpt =
                      match ptd.GetConstructors(BindingFlags.Public ||| BindingFlags.NonPublic) |> Seq.toList with 
                      | [] -> [], None
                      | [ :? ProvidedConstructor as pcinfo ] -> [ for p in pcinfo.GetParameters() -> p ], Some pcinfo
                      | _ -> failwith "internal error: at most one constructor allowed"
                  let ctorArgsAsFields = [ for ctorArg in ctorArgs -> tb.DefineField(ctorArg.Name, convType ctorArg.ParameterType, FieldAttributes.Private) ]

                  // Emit the constructor (if any)
                  match ctorInfoOpt with 
                  | None -> ()
                  | Some pcinfo -> 
                
                      assert ctorMap.ContainsKey pcinfo
                      let cb = ctorMap.[pcinfo]
                      let ilg = cb.GetILGenerator()
                      ilg.Emit(OpCodes.Ldarg_0)
                      let minfo = typeof<obj>.GetConstructor [| |]
                      ilg.Emit(OpCodes.Call,minfo)
                      for ctorArgsAsFieldIdx,ctorArgsAsField in List.mapi (fun i x -> (i,x)) ctorArgsAsFields do 
                          ilg.Emit(OpCodes.Ldarg_0)
                          ilg.Emit(OpCodes.Ldarg, ctorArgsAsFieldIdx+1)
                          ilg.Emit(OpCodes.Stfld, ctorArgsAsField)
                    
                      ilg.Emit(OpCodes.Ret)
                    
                  // Emit the methods
                  let emitMethod (minfo:MethodInfo) = 
                    match minfo with 
                    | :? ProvidedMethod as pminfo   -> 
                      let mb = methMap.[pminfo]
                      let ilg = mb.GetILGenerator()
                      let pop () = ilg.Emit(OpCodes.Pop)

                      let parameterVars = 
                          [| if not pminfo.IsStatic then 
                                 yield Quotations.Var("this", pminfo.DeclaringType)
                             for p in pminfo.GetParameters() do 
                                 yield Quotations.Var(p.Name, p.ParameterType) |]
                      let parameters = 
                          [| for v in parameterVars -> Quotations.Expr.Var v |]
                      let linqCode = pminfo.InvokeCodeInternal parameters
                      let locals = Dictionary<Quotations.Var,LocalBuilder>()
                      //printfn "Emitting linqCode for %s::%s, code = %s" pminfo.DeclaringType.FullName pminfo.Name (try linqCode.ToString() with _ -> "<error>")
                      
                      /// emits given expression to corresponding IL
                      /// callerDontNeedResult - if true then caller will not use result of this expression so it needs to be discarded
                      let rec emit (callerDontNeedResult : bool) (expr: Quotations.Expr) = 
                          match expr with 
                          | Quotations.Patterns.Var v -> 
                              if callerDontNeedResult then ()
                              else
                              let methIdx = parameterVars |> Array.tryFindIndex (fun p -> p = v) 
                              match methIdx with 
                              | Some idx -> ilg.Emit(OpCodes.Ldarg, idx)
                              | None -> 
                              let ctorArgFieldOpt = ctorArgsAsFields |> List.tryFind (fun f -> f.Name = v.Name) 
                              match ctorArgFieldOpt with 
                              | Some ctorArgField -> 
                                  ilg.Emit(OpCodes.Ldarg_0)
                                  ilg.Emit(OpCodes.Ldfld, ctorArgField)
                              | None -> 
                              match locals.TryGetValue v with 
                              | true, localBuilder -> 
                                  ilg.Emit(OpCodes.Ldloc, localBuilder.LocalIndex)
                              | false, _ -> 
                                  failwith "unknown parameter/field"
                          | Quotations.Patterns.Coerce (arg,ty) -> 
                              if callerDontNeedResult then ()
                              else
                              emit false arg
                              ilg.Emit(OpCodes.Castclass , convType ty)
                          | Quotations.Patterns.Call (objOpt,meth,args) -> 
                              match objOpt with None -> () | Some e -> emit false e
                              for pe in args do 
                                  emit false pe
                              let meth = match meth with :? ProvidedMethod as pm when methMap.ContainsKey pm -> methMap.[pm] :> MethodInfo | m -> m
                              ilg.Emit((if meth.IsAbstract || meth.IsVirtual then  OpCodes.Callvirt else OpCodes.Call), meth)
                              
                              
                              let returnTypeIsVoid = meth.ReturnType = typeof<System.Void>
                              match returnTypeIsVoid, callerDontNeedResult with
                              | false, true -> 
                                    // method produced something, but we don't need it
                                    pop()
                              | true, false when expr.Type = typeof<unit> -> 
                                    // if we need result and method produce void and result should be unit - push null as unit value on stack
                                    ilg.Emit(OpCodes.Ldnull)
                              | _ -> ()

                          | Quotations.Patterns.NewObject (ctor,args) -> 
                              for pe in args do 
                                  emit false pe
                              let meth = match ctor with :? ProvidedConstructor as pc when ctorMap.ContainsKey pc -> ctorMap.[pc] :> ConstructorInfo | c -> c
                              ilg.Emit(OpCodes.Newobj, meth)

                              if callerDontNeedResult then pop()

                          | Quotations.Patterns.Value (obj, _ty) -> 
                              let rec emitC (v:obj) = 
                                  match v with 
                                  | :? string as x -> ilg.Emit(OpCodes.Ldstr, x)
                                  | :? int8 as x -> ilg.Emit(OpCodes.Ldc_I4, int32 x)
                                  | :? uint8 as x -> ilg.Emit(OpCodes.Ldc_I4, int32 (int8 x))
                                  | :? int16 as x -> ilg.Emit(OpCodes.Ldc_I4, int32 x)
                                  | :? uint16 as x -> ilg.Emit(OpCodes.Ldc_I4, int32 (int16 x))
                                  | :? int32 as x -> ilg.Emit(OpCodes.Ldc_I4, x)
                                  | :? uint32 as x -> ilg.Emit(OpCodes.Ldc_I4, int32 x)
                                  | :? int64 as x -> ilg.Emit(OpCodes.Ldc_I8, x)
                                  | :? uint64 as x -> ilg.Emit(OpCodes.Ldc_I8, int64 x)
                                  | :? char as x -> ilg.Emit(OpCodes.Ldc_I4, int32 x)
                                  | :? bool as x -> ilg.Emit(OpCodes.Ldc_I4, if x then 1 else 0)
                                  | :? System.Enum as x when x.GetType().GetEnumUnderlyingType() = typeof<int32> -> ilg.Emit(OpCodes.Ldc_I4, unbox<int32> v)
                                  | null -> ilg.Emit(OpCodes.Ldnull)
                                  | _ -> failwithf "unknown constant '%A' in generated method" v
                              if callerDontNeedResult then ()
                              else emitC obj

                          | Quotations.Patterns.Let(v,e,b) -> 
                              let lb = ilg.DeclareLocal v.Type
                              locals.Add (v, lb) 
                              
                              emit false e
                              ilg.Emit(OpCodes.Stloc, lb.LocalIndex)
                              emit callerDontNeedResult b
                              
                          | Quotations.Patterns.NewArray(ty,es) -> 
                              let lb = ilg.DeclareLocal (ty.MakeArrayType())
                              ilg.Emit(OpCodes.Ldc_I4, es.Length)
                              ilg.Emit(OpCodes.Newarr, ty)
                              ilg.Emit(OpCodes.Stloc, lb.LocalIndex)
                              es |> List.iteri (fun i e ->
                                  ilg.Emit(OpCodes.Ldloc, lb.LocalIndex)
                                  ilg.Emit(OpCodes.Ldc_I4, i)
                                  emit false e
                                  ilg.Emit(OpCodes.Stelem, ty))
                              ilg.Emit(OpCodes.Ldloc, lb.LocalIndex)

                          | Quotations.Patterns.Sequential(e1, e2) ->
                              emit true e1
                              emit callerDontNeedResult e2                          

                          | Quotations.Patterns.TypeTest(e1, ty) ->
                              emit false e1
                              ilg.Emit(OpCodes.Isinst, ty)

                          | Quotations.Patterns.IfThenElse(cond, ifTrue, ifFalse) ->
                              let ifFalseLabel = ilg.DefineLabel()
                              let endLabel = ilg.DefineLabel()

                              emit false cond

                              ilg.Emit(OpCodes.Brfalse, ifFalseLabel)

                              emit callerDontNeedResult ifTrue
                              ilg.Emit(OpCodes.Br, endLabel)

                              ilg.MarkLabel(ifFalseLabel)
                              emit callerDontNeedResult ifFalse

                              ilg.Emit(OpCodes.Nop)
                              ilg.MarkLabel(endLabel)

                          | Quotations.Patterns.TryWith(body, _filterVar, _filterBody, catchVar, catchBody) ->                                                                                      
                              
                              let stres, ldres = 
                                  if callerDontNeedResult then ignore, ignore
                                  else
                                    let local = ilg.DeclareLocal body.Type
                                    let stres = fun () -> ilg.Emit(OpCodes.Stloc, local)
                                    let ldres = fun () -> ilg.Emit(OpCodes.Ldloc, local)
                                    stres, ldres

                              let exceptionVar = ilg.DeclareLocal(catchVar.Type)
                              locals.Add(catchVar, exceptionVar)

                              let _exnBlock = ilg.BeginExceptionBlock()
                              
                              emit callerDontNeedResult body
                              stres()

                              ilg.BeginCatchBlock(catchVar.Type)
                              ilg.Emit(OpCodes.Stloc, exceptionVar)
                              emit callerDontNeedResult catchBody
                              stres()
                              ilg.EndExceptionBlock()

                              ldres()

                          | Quotations.Patterns.TryFinally(body, catchBody) ->                                                                                      
                              
                              let stres, ldres = 
                                  if callerDontNeedResult then ignore, ignore
                                  else
                                    let local = ilg.DeclareLocal body.Type
                                    let stres = fun () -> ilg.Emit(OpCodes.Stloc, local)
                                    let ldres = fun () -> ilg.Emit(OpCodes.Ldloc, local)
                                    stres, ldres

                              let _exnBlock = ilg.BeginExceptionBlock()
                              
                              emit callerDontNeedResult body
                              stres()

                              ilg.BeginFinallyBlock()
                              emit true catchBody
                              ilg.EndExceptionBlock()

                              ldres()

                          | Quotations.Patterns.VarSet(v,e) -> 
                              emit false e
                              match locals.TryGetValue v with 
                              | true, localBuilder -> 
                                  ilg.Emit(OpCodes.Stloc, localBuilder.LocalIndex)
                              | false, _ -> 
                                  failwith "unknown parameter/field in assignment. Only assignments to locals are currently supported by TypeProviderEmit"
                              
                          | n -> 
                              failwith (sprintf "unknown expression '%A' in generated method" n)
                      
                      let callerDontNeedResult = (minfo.ReturnType = typeof<System.Void>)
                      emit callerDontNeedResult linqCode
                      ilg.Emit OpCodes.Ret
                    | _ -> ()
  
                  for minfo in ptd.GetMethods(BindingFlags.Public ||| BindingFlags.NonPublic) do
                      emitMethod minfo

                  for (bodyMethInfo,declMethInfo) in ptd.GetMethodOverrides() do 
                     let bodyMethBuilder = methMap.[bodyMethInfo]
                     tb.DefineMethodOverride(bodyMethBuilder,declMethInfo)

                  for pinfo in ptd.GetProperties(BindingFlags.Public ||| BindingFlags.NonPublic) do
                      let pb = tb.DefineProperty(pinfo.Name, pinfo.Attributes, convType pinfo.PropertyType, [| for p in pinfo.GetIndexParameters() -> convType p.ParameterType |])
                      if  pinfo.CanRead then 
                          let minfo = pinfo.GetGetMethod(true)
                          pb.SetGetMethod (methMap.[minfo :?> ProvidedMethod ])
                      if  pinfo.CanWrite then 
                          let minfo = pinfo.GetSetMethod(true)
                          pb.SetSetMethod (methMap.[minfo :?> ProvidedMethod ]))

              // phase 4 - complete types
              iterateTypes (fun tb _ptd -> tb.CreateType() |> ignore)


              assembly.Save (Path.GetFileName assemblyFileName)
              let assemblyBytes = File.ReadAllBytes assemblyFileName
              let assemblyLoadedInMemory = System.Reflection.Assembly.Load(assemblyBytes,null,System.Security.SecurityContextSource.CurrentAppDomain)
              File.Delete assemblyFileName

              iterateTypes (fun _tb ptd -> ptd.SetAssembly assemblyLoadedInMemory)

              match reportAssemblyElements with 
              | None -> GlobalProvidedAssemblyElementsTable.theTable.Add(assemblyLoadedInMemory, assemblyBytes) 
              | Some f -> f (assemblyLoadedInMemory, assemblyBytes)
              assemblyLoadedInMemory


        theAssembly <- theElementsLazy



#if TPEMIT_INTERNAL_AND_MINIMAL_FOR_TYPE_CONTAINERS
#else
module Local = 

    let makeProvidedNamespace (namespaceName:string) (types:ProvidedTypeDefinition list) =
        let types = [| for ty in types -> ty :> Type |]
        {new IProvidedNamespace with
            member __.GetNestedNamespaces() = [| |]
            member __.NamespaceName = namespaceName
            member __.GetTypes() = types |> Array.copy
            member __.ResolveTypeName typeName : System.Type = 
                match types |> Array.tryFind (fun ty -> ty.Name = typeName) with
                | Some ty -> ty
                | None    -> let typenames = String.concat "," (types |> Array.map (fun t -> t.Name))
                             failwith (sprintf "Unknown type '%s' in namespace '%s' (contains %s)" typeName namespaceName typenames)    
        }


type TypeProviderForNamespaces(namespacesAndTypes : list<(string * list<ProvidedTypeDefinition>)>) =
    let otherNamespaces = ResizeArray<string * list<ProvidedTypeDefinition>>()

    let providedNamespaces = 
        lazy [| for (namespaceName,types) in namespacesAndTypes do 
                     yield Local.makeProvidedNamespace namespaceName types 
                for (namespaceName,types) in otherNamespaces do 
                     yield Local.makeProvidedNamespace namespaceName types |]

    let invalidateE = new Event<EventHandler,EventArgs>()    

    new (namespaceName:string,types:list<ProvidedTypeDefinition>) = new TypeProviderForNamespaces([(namespaceName,types)])
    new () = new TypeProviderForNamespaces([])

    member __.AddNamespace (namespaceName,types:list<_>) = otherNamespaces.Add (namespaceName,types)
    member self.Invalidate() = invalidateE.Trigger(self,EventArgs())
    interface System.IDisposable with 
        member x.Dispose() = ()
    interface ITypeProvider with
        [<CLIEvent>]
        override this.Invalidate = invalidateE.Publish
        override this.GetNamespaces() = Array.copy providedNamespaces.Value
        member __.GetInvokerExpression(methodBase, parameters) = 
            match methodBase with
            | :? ProvidedMethod as m when (match methodBase.DeclaringType with :? ProvidedTypeDefinition as pt -> pt.IsErased | _ -> true) -> 
                m.InvokeCodeInternal parameters
            | :? ProvidedConstructor as m when (match methodBase.DeclaringType with :? ProvidedTypeDefinition as pt -> pt.IsErased | _ -> true) -> 
                m.InvokeCodeInternal parameters
            // Otherwise, assume this is a generative assembly and just emit a call to the constructor or method
            | :?  ConstructorInfo as cinfo ->  
                Quotations.Expr.NewObject(cinfo, Array.toList parameters) 
            | :? System.Reflection.MethodInfo as minfo ->  
                if minfo.IsStatic then 
                    Quotations.Expr.Call(minfo, Array.toList parameters) 
                else
                    Quotations.Expr.Call(parameters.[0], minfo, Array.toList parameters.[1..])
            | _ -> failwith ("TypeProviderForNamespaces.GetInvokerExpression: not a ProvidedMethod/ProvidedConstructor/ConstructorInfo/MethodInfo, name=" + methodBase.Name + " class=" + methodBase.GetType().FullName)

        override this.GetStaticParameters(ty) =
            match ty with
            | :? ProvidedTypeDefinition as t ->
                if ty.Name = t.Name (* REVIEW: use equality? *) then
                    t.GetStaticParameters()
                else
                    [| |]
            | _ -> [| |]

        override this.ApplyStaticArguments(ty,typePathAfterArguments:string[],objs) = 
            let typePathAfterArguments = typePathAfterArguments.[typePathAfterArguments.Length-1]
            match ty with
            | :? ProvidedTypeDefinition as t -> (t.MakeParametricType(typePathAfterArguments,objs) :> Type)
            | _ -> failwith (sprintf "ApplyStaticArguments: static params for type %s are unexpected" ty.FullName)

        override x.GetGeneratedAssemblyContents(assembly) = 
            System.Diagnostics.Debug.Assert(GlobalProvidedAssemblyElementsTable.theTable.ContainsKey assembly, "unexpected missing assembly in GlobalProvidedAssemblyElementsTable")
            GlobalProvidedAssemblyElementsTable.theTable.[assembly]

#endif

