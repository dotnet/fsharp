// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace Microsoft.FSharp.Core

//Replacement for: System.Security.SecurityElement.Escape(line) All platforms
module internal XmlAdapters =
    open System.Text
    open Microsoft.FSharp.Collections

    let s_escapeChars = [| '<'; '>'; '\"'; '\''; '&' |]

    let getEscapeSequence c =
        match c with
        | '<'  -> "&lt;"
        | '>'  -> "&gt;"
        | '\"' -> "&quot;"
        | '\'' -> "&apos;"
        | '&'  -> "&amp;"
        | _ as ch -> ch.ToString()

    let escape str = String.collect getEscapeSequence str

#if FX_RESHAPED_REFLECTION
module internal ReflectionAdapters = 
    open System
    open System.Reflection
    open Microsoft.FSharp.Core.LanguagePrimitives.IntrinsicOperators
    open Microsoft.FSharp.Collections
    open PrimReflectionAdapters

    [<System.FlagsAttribute>]
    type BindingFlags =
    | DeclaredOnly = 2
    | Instance = 4 
    | Static = 8
    | Public = 16
    | NonPublic = 32
    | InvokeMethod = 0x100

    let inline hasFlag (flag : BindingFlags) f  = (f &&& flag) = flag
    let isDeclaredFlag  f    = hasFlag BindingFlags.DeclaredOnly f
    let isPublicFlag    f    = hasFlag BindingFlags.Public f
    let isStaticFlag    f    = hasFlag BindingFlags.Static f
    let isInstanceFlag  f    = hasFlag BindingFlags.Instance f
    let isNonPublicFlag f    = hasFlag BindingFlags.NonPublic f

#if FX_NO_EXIT
    let exit (_n:int) = failwith "System.Environment.Exit does not exist!"
#endif

#if !FX_HAS_TYPECODE
    [<System.Flags>]
    type TypeCode = 
        | Int32     = 0
        | Int64     = 1
        | Byte      = 2
        | SByte     = 3
        | Int16     = 4
        | UInt16    = 5
        | UInt32    = 6
        | UInt64    = 7
        | Single    = 8
        | Double    = 9
        | Decimal   = 10
        | Object    = 11
#endif

    let isAcceptable bindingFlags isStatic isPublic =
        // 1. check if member kind (static\instance) was specified in flags
        ((isStaticFlag bindingFlags && isStatic) || (isInstanceFlag bindingFlags && not isStatic)) && 
        // 2. check if member accessibility was specified in flags
        ((isPublicFlag bindingFlags && isPublic) || (isNonPublicFlag bindingFlags && not isPublic))

    let publicFlags = BindingFlags.Public ||| BindingFlags.Instance ||| BindingFlags.Static

    let commit (results : _[]) = 
        match results with
        | [||] -> null
        | [| m |] -> m
        | _ -> raise (AmbiguousMatchException())

    let canUseAccessor (accessor : MethodInfo) nonPublic = 
        box accessor <> null && (accessor.IsPublic || nonPublic)

    type System.Type with
        member this.GetTypeInfo() = IntrospectionExtensions.GetTypeInfo(this)
        member this.GetRuntimeProperties() = RuntimeReflectionExtensions.GetRuntimeProperties(this)
        member this.GetRuntimeEvents() = RuntimeReflectionExtensions.GetRuntimeEvents(this)
        member this.Attributes = this.GetTypeInfo().Attributes
        member this.GetCustomAttributes(attrTy, inherits) : obj[] = downcast box(CustomAttributeExtensions.GetCustomAttributes(this.GetTypeInfo(), attrTy, inherits) |> Seq.toArray)
        member this.GetNestedType (name, bindingFlags) = 
            // MSDN: http://msdn.microsoft.com/en-us/library/0dcb3ad5.aspx
            // The following BindingFlags filter flags can be used to define which nested types to include in the search:
            // You must specify either BindingFlags.Public or BindingFlags.NonPublic to get a return.
            // Specify BindingFlags.Public to include public nested types in the search.
            // Specify BindingFlags.NonPublic to include non-public nested types (that is, private, internal, and protected nested types) in the search.
            // This method returns only the nested types of the current type. It does not search the base classes of the current type. 
            // To find types that are nested in base classes, you must walk the inheritance hierarchy, calling GetNestedType at each level.
            let nestedTyOpt =                
                this.GetTypeInfo().DeclaredNestedTypes
                |> Seq.tryFind (fun nestedTy -> 
                    nestedTy.Name = name && (
                        (isPublicFlag bindingFlags && nestedTy.IsNestedPublic) || 
                        (isNonPublicFlag bindingFlags && (nestedTy.IsNestedPrivate || nestedTy.IsNestedFamily || nestedTy.IsNestedAssembly || nestedTy.IsNestedFamORAssem || nestedTy.IsNestedFamANDAssem))
                        )
                    )
                |> Option.map (fun ti -> ti.AsType())
            defaultArg nestedTyOpt null
        // use different sources based on Declared flag
        member this.GetMethods(bindingFlags) =
            (if isDeclaredFlag bindingFlags then this.GetTypeInfo().DeclaredMethods else this.GetRuntimeMethods())
            |> Seq.filter (fun m -> isAcceptable bindingFlags m.IsStatic m.IsPublic)
            |> Seq.toArray
        // use different sources based on Declared flag
        member this.GetFields(bindingFlags) = 
            (if isDeclaredFlag bindingFlags then this.GetTypeInfo().DeclaredFields else this.GetRuntimeFields())
            |> Seq.filter (fun f -> isAcceptable bindingFlags f.IsStatic f.IsPublic)
            |> Seq.toArray
        // use different sources based on Declared flag
        member this.GetProperties(?bindingFlags) = 
            let bindingFlags = defaultArg bindingFlags publicFlags
            (if isDeclaredFlag bindingFlags then this.GetTypeInfo().DeclaredProperties else this.GetRuntimeProperties())
            |> Seq.filter (fun pi-> 
                let mi = if pi.GetMethod <> null then pi.GetMethod else pi.SetMethod
                if mi = null then false
                else isAcceptable bindingFlags mi.IsStatic mi.IsPublic
                )
            |> Seq.toArray
#if FX_RESHAPED_REFLECTION_CORECLR
        member this.GetEvents(?bindingFlags) = 
            let bindingFlags = defaultArg bindingFlags publicFlags
            (if isDeclaredFlag bindingFlags then this.GetTypeInfo().DeclaredEvents else this.GetRuntimeEvents())
            |> Seq.filter (fun ei-> 
                let m = ei.GetAddMethod(true)
                if m = null then false
                else isAcceptable bindingFlags m.IsStatic m.IsPublic
                )
            |> Seq.toArray
        member this.GetEvent(name, ?bindingFlags) = 
            let bindingFlags = defaultArg bindingFlags publicFlags
            this.GetEvents(bindingFlags)
            |> Array.filter (fun ei -> ei.Name = name)
            |> commit
#endif
        member this.GetConstructor(_bindingFlags, _binder, argsT:Type[], _parameterModifiers) =
            this.GetConstructor(argsT)
        member this.GetMethod(name, ?bindingFlags) =
            let bindingFlags = defaultArg bindingFlags publicFlags
            this.GetMethods(bindingFlags)
            |> Array.filter(fun m -> m.Name = name)
            |> commit
        member this.GetMethod(name, _bindingFlags, _binder, argsT:Type[], _parameterModifiers) =
            this.GetMethod(name, argsT)
        // use different sources based on Declared flag
        member this.GetProperty(name, bindingFlags) = 
            this.GetProperties(bindingFlags)
            |> Array.filter (fun pi -> pi.Name = name)
            |> commit
        member this.GetMethod(methodName, args:Type[], ?bindingFlags) =
            let bindingFlags = defaultArg bindingFlags publicFlags
            let compareSequences parms args = 
                Seq.compareWith (fun parm arg -> if parm <> arg then 1 else 0) parms args
            this.GetMethods(bindingFlags)
            |> Array.filter(fun m -> m.Name = methodName && (compareSequences (m.GetParameters() |> Seq.map(fun x -> x.ParameterType)) args) = 0)
            |> commit
        member this.GetNestedTypes(?bindingFlags) =
            let bindingFlags = defaultArg bindingFlags publicFlags
            this.GetTypeInfo().DeclaredNestedTypes
            |> Seq.filter (fun nestedTy->
                    (isPublicFlag bindingFlags && nestedTy.IsNestedPublic) || 
                    (isNonPublicFlag bindingFlags && (nestedTy.IsNestedPrivate || nestedTy.IsNestedFamily || nestedTy.IsNestedAssembly || nestedTy.IsNestedFamORAssem || nestedTy.IsNestedFamANDAssem)))
            |> Seq.map (fun ti -> ti.AsType())
            |> Seq.toArray
        member this.GetEnumUnderlyingType() =
            Enum.GetUnderlyingType(this)
#if FX_RESHAPED_REFLECTION_CORECLR
        member this.InvokeMember(memberName, bindingFlags, _binder, target:obj, arguments:obj[], _cultureInfo) =
            let m = this.GetMethod(memberName, (arguments |> Seq.map(fun x -> x.GetType()) |> Seq.toArray), bindingFlags)
            if m <> null then m.Invoke(target, arguments)
            else raise <| System.MissingMethodException(String.Format("Method '{0}.{1}' not found.", this.FullName, memberName))
#endif
        member this.IsGenericType = this.GetTypeInfo().IsGenericType
        member this.IsGenericTypeDefinition = this.GetTypeInfo().IsGenericTypeDefinition
        member this.GetGenericArguments() = 
            if this.IsGenericTypeDefinition then this.GetTypeInfo().GenericTypeParameters
            elif this.IsGenericType then this.GenericTypeArguments
            else [||]
        member this.IsInterface = this.GetTypeInfo().IsInterface
        member this.IsPublic = this.GetTypeInfo().IsPublic
        member this.IsNestedPublic = this.GetTypeInfo().IsNestedPublic
        member this.IsClass = this.GetTypeInfo().IsClass
        member this.IsValueType = this.GetTypeInfo().IsValueType
        member this.IsSealed = this.GetTypeInfo().IsSealed
        
        member this.BaseType = this.GetTypeInfo().BaseType
        member this.GetConstructor(parameterTypes : Type[]) = 
            this.GetTypeInfo().DeclaredConstructors
            |> Seq.filter (fun ci ->
                not ci.IsStatic && //exclude type initializer
                (
                    let parameters = ci.GetParameters()
                    (parameters.Length = parameterTypes.Length) &&
                    (parameterTypes, parameters) ||> Array.forall2 (fun ty pi -> pi.ParameterType.Equals ty) 
                )
            )
            |> Seq.toArray
            |> commit
        // MSDN: returns an array of Type objects representing all the interfaces implemented or inherited by the current Type.
        member this.GetInterfaces() = this.GetTypeInfo().ImplementedInterfaces |> Seq.toArray
        member this.GetConstructors(?bindingFlags) = 
            let bindingFlags = defaultArg bindingFlags publicFlags
            // type initializer will also be included in resultset
            this.GetTypeInfo().DeclaredConstructors 
            |> Seq.filter (fun ci -> isAcceptable bindingFlags ci.IsStatic ci.IsPublic)
            |> Seq.toArray
        member this.GetMethods() = this.GetMethods(publicFlags)
        member this.Assembly = this.GetTypeInfo().Assembly
        member this.IsSubclassOf(otherTy : Type) = this.GetTypeInfo().IsSubclassOf(otherTy)
        member this.IsEnum = this.GetTypeInfo().IsEnum;
        member this.GetField(name, bindingFlags) = 
            this.GetFields(bindingFlags)
            |> Array.filter (fun fi -> fi.Name = name)
            |> commit
        member this.GetField(name) = RuntimeReflectionExtensions.GetRuntimeField(this, name)
        member this.GetProperty(name, propertyType, parameterTypes : Type[]) = 
            this.GetProperties()
            |> Array.filter (fun pi ->
                pi.Name = name &&
                pi.PropertyType = propertyType &&
                (
                    let parameters = pi.GetIndexParameters()
                    (parameters.Length = parameterTypes.Length) &&
                    (parameterTypes, parameters) ||> Array.forall2 (fun ty pi -> pi.ParameterType.Equals ty)
                )
            )
            |> commit

        static member GetTypeCode(ty : Type) = 
            if   typeof<System.Int32>.Equals ty  then TypeCode.Int32
            elif typeof<System.Int64>.Equals ty  then TypeCode.Int64
            elif typeof<System.Byte>.Equals ty   then TypeCode.Byte
            elif ty = typeof<System.SByte>  then TypeCode.SByte
            elif ty = typeof<System.Int16>  then TypeCode.Int16
            elif ty = typeof<System.UInt16> then TypeCode.UInt16
            elif ty = typeof<System.UInt32> then TypeCode.UInt32
            elif ty = typeof<System.UInt64> then TypeCode.UInt64
            elif ty = typeof<System.Single> then TypeCode.Single
            elif ty = typeof<System.Double> then TypeCode.Double
            elif ty = typeof<System.Decimal> then TypeCode.Decimal
            else TypeCode.Object

        member this.Module =
            this.GetTypeInfo().Module

        member this.MetadataToken =
            // TODO: is this an adequate replacement for MetadataToken
            let s = String.Format("{0}", this.ToString())
            s.GetHashCode()

    type System.Reflection.EventInfo with
        member this.GetAddMethod() =
            this.AddMethod
        member this.GetRemoveMethod() =
            this.RemoveMethod
        member this.MetadataToken =
            // TODO: is this an adequate replacement for MetadataToken
            let s = String.Format("{0},{0}", this.DeclaringType.ToString(), this.ToString())
            s.GetHashCode()

    type System.Reflection.FieldInfo with
        member this.GetRawConstantValue() =
            this.GetValue(null)
        member this.MetadataToken =
            // TODO: is this an adequate replacement for MetadataToken
            let s = String.Format("{0},{0}", this.DeclaringType.ToString(), this.ToString())
            s.GetHashCode()

    type System.Reflection.MemberInfo with
        member this.GetCustomAttributes(attrTy, inherits) : obj[] = downcast box(CustomAttributeExtensions.GetCustomAttributes(this, attrTy, inherits) |> Seq.toArray)
        // TODO: is this an adequate replacement for MetadataToken
        member this.MetadataToken =
            // TODO: is this an adequate replacement for MetadataToken
            let s = String.Format("{0},{0}", this.DeclaringType.ToString(), this.ToString())
            s.GetHashCode()

    type System.Reflection.MethodInfo with
        member this.GetCustomAttributes(inherits : bool) : obj[] = downcast box(CustomAttributeExtensions.GetCustomAttributes(this, inherits) |> Seq.toArray)
        member this.Invoke(obj, _bindingFlags, _binder, args, _ci) =
            this.Invoke(obj, args)
        member this.MetadataToken =
            // TODO: is this an adequate replacement for MetadataToken
            let s = String.Format("{0},{0}", this.DeclaringType.ToString(), this.ToString())
            s.GetHashCode()

    type System.Reflection.ParameterInfo with
        member this.RawDefaultValue = this.DefaultValue
        member this.MetadataToken =
            // TODO: is this an adequate replacement for MetadataToken
            // I really do not understand why: sprintf "%s,%s" (this.ReflectedType.ToString()) (this.ToString()) did not work
            let s = String.Format("{0},{0},{0}", this.Member.DeclaringType.ToString(),this.Member.ToString(), this.ToString())
            s.GetHashCode()

    type System.Reflection.PropertyInfo with
        member this.GetGetMethod(nonPublic) =
            let mi = this.GetMethod
            if canUseAccessor mi nonPublic then mi
            else null
        member this.GetSetMethod(nonPublic) =
            let mi = this.SetMethod
            if canUseAccessor mi nonPublic then mi
            else null
        member this.GetGetMethod() = this.GetMethod
        member this.GetSetMethod() = this.SetMethod

#if FX_RESHAPED_REFLECTION_CORECLR
    let globalLoadContext = System.Runtime.Loader.AssemblyLoadContext.Default
#endif
    type System.Reflection.Assembly with
        member this.GetTypes() = 
            this.DefinedTypes 
            |> Seq.map (fun ti -> ti.AsType())
            |> Seq.toArray
        member this.GetExportedTypes() = 
            this.DefinedTypes 
            |> Seq.filter(fun ti -> ti.IsPublic)
            |> Seq.map (fun ti -> ti.AsType()) 
            |> Seq.toArray
        member this.Location = 
            this.ManifestModule.FullyQualifiedName

#if FX_RESHAPED_REFLECTION_CORECLR
        static member LoadFrom(filename:string) =
            globalLoadContext.LoadFromAssemblyName(System.Runtime.Loader.AssemblyLoadContext.GetAssemblyName(filename))

        static member UnsafeLoadFrom(filename:string) =
            globalLoadContext.LoadFromAssemblyName(System.Runtime.Loader.AssemblyLoadContext.GetAssemblyName(filename))

    type System.Reflection.AssemblyName with
        static member GetAssemblyName(path) = 
            System.Runtime.Loader.AssemblyLoadContext.GetAssemblyName(path)
#endif

    type System.Delegate with
        static member CreateDelegate(delegateType, methodInfo : MethodInfo) = methodInfo.CreateDelegate(delegateType)
        static member CreateDelegate(delegateType, obj : obj, methodInfo : MethodInfo) = methodInfo.CreateDelegate(delegateType, obj)

    type System.Object with
        member this.GetPropertyValue(propName) =
            this.GetType().GetProperty(propName, BindingFlags.Public).GetValue(this, null)
        member this.SetPropertyValue(propName, propValue) =
            this.GetType().GetProperty(propName, BindingFlags.Public).SetValue(this, propValue, null)
        member this.GetMethod(methodName, argTypes) =
            this.GetType().GetMethod(methodName, argTypes, BindingFlags.Public)

    type System.Char with
        static member GetUnicodeCategory(c) = System.Globalization.CharUnicodeInfo.GetUnicodeCategory(c)

    type System.Reflection.Module with
        member this.MetadataToken =
            // TODO: is this an adequate replacement for MetadataToken
            let s = this.FullyQualifiedName
            s.GetHashCode()

#endif
