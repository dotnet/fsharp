// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

// Reflection on F# values. Analyze an object to see if it the representation
// of an F# value.

#if FX_RESHAPED_REFLECTION

namespace Microsoft.FSharp.Core

open System
open System.Reflection

open Microsoft.FSharp.Core
open Microsoft.FSharp.Core.Operators
open Microsoft.FSharp.Core.LanguagePrimitives.IntrinsicOperators
open Microsoft.FSharp.Collections
open Microsoft.FSharp.Primitives.Basics

module ReflectionAdapters = 

    [<Flags>]
    type BindingFlags =
        | DeclaredOnly = 2
        | Instance = 4 
        | Static = 8
        | Public = 16
        | NonPublic = 32
    let inline hasFlag (flag : BindingFlags) f  = (f &&& flag) = flag
    let isDeclaredFlag  f    = hasFlag BindingFlags.DeclaredOnly f
    let isPublicFlag    f    = hasFlag BindingFlags.Public f
    let isStaticFlag    f    = hasFlag BindingFlags.Static f
    let isInstanceFlag  f    = hasFlag BindingFlags.Instance f
    let isNonPublicFlag f    = hasFlag BindingFlags.NonPublic f

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
        | Other     = 11
        
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
    
    open PrimReflectionAdapters

    type System.Type with        
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
                assert (mi <> null)
                isAcceptable bindingFlags mi.IsStatic mi.IsPublic
                )
            |> Seq.toArray
        // use different sources based on Declared flag
        member this.GetMethod(name, ?bindingFlags) =
            let bindingFlags = defaultArg bindingFlags publicFlags
            this.GetMethods(bindingFlags)
            |> Array.filter(fun m -> m.Name = name)
            |> commit
        // use different sources based on Declared flag
        member this.GetProperty(name, bindingFlags) = 
            this.GetProperties(bindingFlags)
            |> Array.filter (fun pi -> pi.Name = name)
            |> commit
        member this.IsGenericTypeDefinition = this.GetTypeInfo().IsGenericTypeDefinition
        member this.GetGenericArguments() = 
            if this.IsGenericTypeDefinition then this.GetTypeInfo().GenericTypeParameters
            elif this.IsGenericType then this.GenericTypeArguments
            else [||]
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
            else TypeCode.Other

    type System.Reflection.MemberInfo with
        member this.GetCustomAttributes(attrTy, inherits) : obj[] = downcast box(CustomAttributeExtensions.GetCustomAttributes(this, attrTy, inherits) |> Seq.toArray)

    type System.Reflection.MethodInfo with
        member this.GetCustomAttributes(inherits : bool) : obj[] = downcast box(CustomAttributeExtensions.GetCustomAttributes(this, inherits) |> Seq.toArray)

    type System.Reflection.PropertyInfo with
        member this.GetGetMethod(nonPublic) = 
            let mi = this.GetMethod
            if canUseAccessor mi nonPublic then mi 
            else null
        member this.GetSetMethod(nonPublic) = 
            let mi = this.SetMethod
            if canUseAccessor mi nonPublic then mi
            else null
    
    type System.Reflection.Assembly with
        member this.GetTypes() = 
            this.DefinedTypes 
            |> Seq.map (fun ti -> ti.AsType())
            |> Seq.toArray

    type System.Delegate with
        static member CreateDelegate(delegateType, methodInfo : MethodInfo) = methodInfo.CreateDelegate(delegateType)
        static member CreateDelegate(delegateType, obj : obj, methodInfo : MethodInfo) = methodInfo.CreateDelegate(delegateType, obj)            

#endif

namespace Microsoft.FSharp.Reflection 

module internal ReflectionUtils = 

    open Microsoft.FSharp.Core.Operators

#if FX_RESHAPED_REFLECTION
    type BindingFlags = Microsoft.FSharp.Core.ReflectionAdapters.BindingFlags
#else
    type BindingFlags = System.Reflection.BindingFlags
#endif

    let toBindingFlags allowAccessToNonPublicMembers =
        if allowAccessToNonPublicMembers then
            BindingFlags.NonPublic ||| BindingFlags.Public
        else
            BindingFlags.Public

open System
open System.Globalization
open System.Reflection
open Microsoft.FSharp.Core
open Microsoft.FSharp.Core.Operators
open Microsoft.FSharp.Core.LanguagePrimitives.IntrinsicOperators
open Microsoft.FSharp.Collections
open Microsoft.FSharp.Primitives.Basics

module internal Impl =

    let debug = false

#if FX_RESHAPED_REFLECTION
    
    open PrimReflectionAdapters
    open ReflectionAdapters    

#endif

    let getBindingFlags allowAccess = ReflectionUtils.toBindingFlags (defaultArg allowAccess false)

    let inline checkNonNull argName (v: 'T) = 
        match box v with 
        | null -> nullArg argName 
        | _ -> ()
     
        
    let emptyArray arr = (Array.length arr = 0)
    let nonEmptyArray arr = Array.length arr > 0

    let isNamedType(typ:Type) = not (typ.IsArray || typ.IsByRef || typ.IsPointer)

    let equivHeadTypes (ty1:Type) (ty2:Type) = 
        isNamedType(ty1) &&
        if ty1.IsGenericType then 
          ty2.IsGenericType && (ty1.GetGenericTypeDefinition()).Equals(ty2.GetGenericTypeDefinition())
        else 
          ty1.Equals(ty2)

    let option = typedefof<obj option>
    let func = typedefof<(obj -> obj)>

    let isOptionType typ = equivHeadTypes typ (typeof<int option>)
    let isFunctionType typ = equivHeadTypes typ (typeof<(int -> int)>)
    let isListType typ = equivHeadTypes typ (typeof<int list>)

    //-----------------------------------------------------------------
    // GENERAL UTILITIES
#if FX_ATLEAST_PORTABLE
    let instancePropertyFlags = BindingFlags.Instance 
    let staticPropertyFlags = BindingFlags.Static
    let staticFieldFlags = BindingFlags.Static 
    let staticMethodFlags = BindingFlags.Static 
#else    
    let instancePropertyFlags = BindingFlags.GetProperty ||| BindingFlags.Instance 
    let staticPropertyFlags = BindingFlags.GetProperty ||| BindingFlags.Static
    let staticFieldFlags = BindingFlags.GetField ||| BindingFlags.Static 
    let staticMethodFlags = BindingFlags.Static 
#endif

    let getInstancePropertyInfo (typ: Type,propName,bindingFlags) = typ.GetProperty(propName,instancePropertyFlags ||| bindingFlags) 
    let getInstancePropertyInfos (typ,names,bindingFlags) = names |> Array.map (fun nm -> getInstancePropertyInfo (typ,nm,bindingFlags)) 

    let getInstancePropertyReader (typ: Type,propName,bindingFlags) =
        match getInstancePropertyInfo(typ, propName, bindingFlags) with
        | null -> None
#if FX_ATLEAST_PORTABLE
        | prop -> Some(fun (obj:obj) -> prop.GetValue(obj,null))
#else        
        | prop -> Some(fun (obj:obj) -> prop.GetValue(obj,instancePropertyFlags ||| bindingFlags,null,null,null))
#endif
    //-----------------------------------------------------------------
    // ATTRIBUTE DECOMPILATION

    let tryFindCompilationMappingAttribute (attrs:obj[]) =
      match attrs with
      | null | [| |] -> None
      | [| res |] -> let a = (res :?> CompilationMappingAttribute) in Some (a.SourceConstructFlags, a.SequenceNumber, a.VariantNumber)
      | _ -> raise <| System.InvalidOperationException (SR.GetString(SR.multipleCompilationMappings))

    let findCompilationMappingAttribute (attrs:obj[]) =
      match tryFindCompilationMappingAttribute attrs with
      | None -> failwith "no compilation mapping attribute"
      | Some a -> a

#if FX_NO_CUSTOMATTRIBUTEDATA
    let tryFindCompilationMappingAttributeFromType       (typ:Type)        = tryFindCompilationMappingAttribute ( typ.GetCustomAttributes(typeof<CompilationMappingAttribute>, false))
    let tryFindCompilationMappingAttributeFromMemberInfo (info:MemberInfo) = tryFindCompilationMappingAttribute (info.GetCustomAttributes(typeof<CompilationMappingAttribute>, false))
    let    findCompilationMappingAttributeFromMemberInfo (info:MemberInfo) =    findCompilationMappingAttribute (info.GetCustomAttributes(typeof<CompilationMappingAttribute>, false))
#else
    let cmaName = typeof<CompilationMappingAttribute>.FullName
    let assemblyName = typeof<CompilationMappingAttribute>.Assembly.GetName().Name 
    let _ = assert (assemblyName = "FSharp.Core")
    
    let tryFindCompilationMappingAttributeFromData (attrs:System.Collections.Generic.IList<CustomAttributeData>) =
        match attrs with
        | null -> None
        | _ -> 
            let mutable res = None
            for a in attrs do
                if a.Constructor.DeclaringType.FullName = cmaName then 
                    let args = a.ConstructorArguments
                    let flags = 
                         match args.Count  with 
                         | 1 -> ((args.[0].Value :?> SourceConstructFlags), 0, 0)
                         | 2 -> ((args.[0].Value :?> SourceConstructFlags), (args.[1].Value :?> int), 0)
                         | 3 -> ((args.[0].Value :?> SourceConstructFlags), (args.[1].Value :?> int), (args.[2].Value :?> int))
                         | _ -> (enum 0, 0, 0)
                    res <- Some flags
            res

    let findCompilationMappingAttributeFromData attrs =
      match tryFindCompilationMappingAttributeFromData attrs with
      | None -> failwith "no compilation mapping attribute"
      | Some a -> a

    let tryFindCompilationMappingAttributeFromType       (typ:Type)        = 
        let assem = typ.Assembly
        if assem <> null && assem.ReflectionOnly then 
           tryFindCompilationMappingAttributeFromData ( typ.GetCustomAttributesData())
        else
           tryFindCompilationMappingAttribute ( typ.GetCustomAttributes (typeof<CompilationMappingAttribute>,false))

    let tryFindCompilationMappingAttributeFromMemberInfo (info:MemberInfo) = 
        let assem = info.DeclaringType.Assembly
        if assem <> null && assem.ReflectionOnly then 
           tryFindCompilationMappingAttributeFromData (info.GetCustomAttributesData())
        else
        tryFindCompilationMappingAttribute (info.GetCustomAttributes (typeof<CompilationMappingAttribute>,false))

    let    findCompilationMappingAttributeFromMemberInfo (info:MemberInfo) =    
        let assem = info.DeclaringType.Assembly
        if assem <> null && assem.ReflectionOnly then 
            findCompilationMappingAttributeFromData (info.GetCustomAttributesData())
        else
            findCompilationMappingAttribute (info.GetCustomAttributes (typeof<CompilationMappingAttribute>,false))

#endif

    let sequenceNumberOfMember          (x: MemberInfo) = let (_,n,_) = findCompilationMappingAttributeFromMemberInfo x in n
    let variantNumberOfMember           (x: MemberInfo) = let (_,_,vn) = findCompilationMappingAttributeFromMemberInfo x in vn

    let sortFreshArray f arr = Array.sortInPlaceWith f arr; arr

    let isFieldProperty (prop : PropertyInfo) =
        match tryFindCompilationMappingAttributeFromMemberInfo(prop) with
        | None -> false
        | Some (flags,_n,_vn) -> (flags &&& SourceConstructFlags.KindMask) = SourceConstructFlags.Field

    let allInstance  (ps : PropertyInfo[]) = (ps, false)
    let allStatic  (ps : PropertyInfo[]) = (ps, true)

    let tryFindSourceConstructFlagsOfType (typ:Type) = 
      match tryFindCompilationMappingAttributeFromType typ with 
      | None -> None
      | Some (flags,_n,_vn) -> Some flags

    //-----------------------------------------------------------------
    // UNION DECOMPILATION   

    // Get the type where the type definitions are stored
    let getUnionCasesTyp (typ: Type, _bindingFlags) = 
#if CASES_IN_NESTED_CLASS
       let casesTyp = typ.GetNestedType("Cases", bindingFlags)
       if casesTyp.IsGenericTypeDefinition then casesTyp.MakeGenericType(typ.GetGenericArguments())
       else casesTyp
#else
       typ
#endif
            
    let getUnionTypeTagNameMap (typ:Type,bindingFlags) = 
        let enumTyp = typ.GetNestedType("Tags", bindingFlags)
        // Unions with a singleton case do not get a Tags type (since there is only one tag), hence enumTyp may be null in this case
        match enumTyp with
        | null -> 
            typ.GetMethods(staticMethodFlags ||| bindingFlags) 
            |> Array.choose (fun minfo -> 
                match tryFindCompilationMappingAttributeFromMemberInfo(minfo) with
                | None -> None
                | Some (flags,n,_vn) -> 
                    if (flags &&& SourceConstructFlags.KindMask) = SourceConstructFlags.UnionCase then 
                        let nm = minfo.Name 
                        // chop "get_" or  "New" off the front 
                        let nm = 
                            if not (isListType typ) && not (isOptionType typ) then 
                                if   nm.Length > 4 && nm.[0..3] = "get_" then nm.[4..] 
                                elif nm.Length > 3 && nm.[0..2] = "New" then nm.[3..]
                                else nm
                            else nm
                        Some (n, nm)
                    else
                        None) 
        | _ -> 
            enumTyp.GetFields(staticFieldFlags ||| bindingFlags) 
            |> Array.filter (fun (f:FieldInfo) -> f.IsStatic && f.IsLiteral) 
            |> sortFreshArray (fun f1 f2 -> compare (f1.GetValue(null) :?> int) (f2.GetValue(null) :?> int))
            |> Array.map (fun tagfield -> (tagfield.GetValue(null) :?> int),tagfield.Name)

    let getUnionCaseTyp (typ: Type, tag: int, bindingFlags) = 
        let tagFields = getUnionTypeTagNameMap(typ,bindingFlags)
        let tagField = tagFields |> Array.pick (fun (i,f) -> if i = tag then Some f else None)
        if tagFields.Length = 1 then 
            typ
        else
            // special case: two-cased DU annotated with CompilationRepresentation(UseNullAsTrueValue)
            // in this case it will be compiled as one class: return self type for non-nullary case and null for nullary
            let isTwoCasedDU =
                if tagFields.Length = 2 then
                    match typ.GetCustomAttributes(typeof<CompilationRepresentationAttribute>, false) with
                    | [|:? CompilationRepresentationAttribute as attr|] -> 
                        (attr.Flags &&& CompilationRepresentationFlags.UseNullAsTrueValue) = CompilationRepresentationFlags.UseNullAsTrueValue
                    | _ -> false
                else
                    false
            if isTwoCasedDU then
                typ
            else
            let casesTyp = getUnionCasesTyp (typ, bindingFlags)
            let caseTyp = casesTyp.GetNestedType(tagField, bindingFlags) // if this is null then the union is nullary
            match caseTyp with 
            | null -> null
            | _ when caseTyp.IsGenericTypeDefinition -> caseTyp.MakeGenericType(casesTyp.GetGenericArguments())
            | _ -> caseTyp

    let getUnionTagConverter (typ:Type,bindingFlags) = 
        if isOptionType typ then (fun tag -> match tag with 0 -> "None" | 1 -> "Some" | _ -> invalidArg "tag" (SR.GetString(SR.outOfRange)))
        elif isListType typ then (fun tag -> match tag with  0 -> "Empty" | 1 -> "Cons" | _ -> invalidArg "tag" (SR.GetString(SR.outOfRange)))
        else 
          let tagfieldmap = getUnionTypeTagNameMap (typ,bindingFlags) |> Map.ofSeq
          (fun tag -> tagfieldmap.[tag])

    let isUnionType (typ:Type,bindingFlags:BindingFlags) = 
        isOptionType typ || 
        isListType typ || 
        match tryFindSourceConstructFlagsOfType(typ) with 
        | None -> false
        | Some(flags) ->
          (flags &&& SourceConstructFlags.KindMask) = SourceConstructFlags.SumType &&
          // We see private representations only if BindingFlags.NonPublic is set
          (if (flags &&& SourceConstructFlags.NonPublicRepresentation) <> enum(0) then 
              (bindingFlags &&& BindingFlags.NonPublic) <> enum(0)
           else 
              true)

    // Check the base type - if it is also an F# type then
    // for the moment we know it is a Discriminated Union
    let isConstructorRepr (typ:Type,bindingFlags:BindingFlags) = 
        let rec get (typ:Type) = isUnionType (typ,bindingFlags) || match typ.BaseType with null -> false | b -> get b
        get typ 

    let unionTypeOfUnionCaseType (typ:Type,bindingFlags) = 
        let rec get (typ:Type) = if isUnionType (typ,bindingFlags) then typ else match typ.BaseType with null -> typ | b -> get b
        get typ 
                   
    let swap (x,y) = (y,x)

    let fieldsPropsOfUnionCase(typ:Type, tag:int, bindingFlags) =
        if isOptionType typ then 
            match tag with 
            | 0 (* None *) -> getInstancePropertyInfos (typ,[| |],bindingFlags) 
            | 1 (* Some *) -> getInstancePropertyInfos (typ,[| "Value" |] ,bindingFlags) 
            | _ -> failwith "fieldsPropsOfUnionCase"
        elif isListType typ then 
            match tag with 
            | 0 (* Nil *)  -> getInstancePropertyInfos (typ,[| |],bindingFlags) 
            | 1 (* Cons *) -> getInstancePropertyInfos (typ,[| "Head"; "Tail" |],bindingFlags) 
            | _ -> failwith "fieldsPropsOfUnionCase"
        else
            // Lookup the type holding the fields for the union case
            let caseTyp = getUnionCaseTyp (typ, tag, bindingFlags)
            match caseTyp with 
            | null ->  [| |]
            | _ ->  caseTyp.GetProperties(instancePropertyFlags ||| bindingFlags) 
                    |> Array.filter isFieldProperty
                    |> Array.filter (fun prop -> variantNumberOfMember prop = tag)
                    |> sortFreshArray (fun p1 p2 -> compare (sequenceNumberOfMember p1) (sequenceNumberOfMember p2))
                

    let getUnionCaseRecordReader (typ:Type,tag:int,bindingFlags) = 
        let props = fieldsPropsOfUnionCase(typ,tag,bindingFlags)
#if FX_ATLEAST_PORTABLE
        (fun (obj:obj) -> props |> Array.map (fun prop -> prop.GetValue(obj,null)))
#else        
        (fun (obj:obj) -> props |> Array.map (fun prop -> prop.GetValue(obj,bindingFlags,null,null,null)))
#endif
    let getUnionTagReader (typ:Type,bindingFlags) : (obj -> int) = 
        if isOptionType typ then 
            (fun (obj:obj) -> match obj with null -> 0 | _ -> 1)
        else
            let tagMap = getUnionTypeTagNameMap (typ, bindingFlags)
            if tagMap.Length <= 1 then 
                (fun (_obj:obj) -> 0)
            else   
                match getInstancePropertyReader (typ,"Tag",bindingFlags) with
                | Some reader -> (fun (obj:obj) -> reader obj :?> int)
                | None -> 
                    (fun (obj:obj) -> 
#if FX_ATLEAST_PORTABLE
                        let m2b = typ.GetMethod("GetTag", [| typ |])
#else                    
                        let m2b = typ.GetMethod("GetTag", BindingFlags.Static ||| bindingFlags, null, [| typ |], null)
#endif                        
                        m2b.Invoke(null, [|obj|]) :?> int)
        
    let getUnionTagMemberInfo (typ:Type,bindingFlags) = 
        match getInstancePropertyInfo (typ,"Tag",bindingFlags) with
#if FX_ATLEAST_PORTABLE
        | null -> (typ.GetMethod("GetTag") :> MemberInfo)
#else        
        | null -> (typ.GetMethod("GetTag",BindingFlags.Static ||| bindingFlags) :> MemberInfo)
#endif        
        | info -> (info :> MemberInfo)

    let isUnionCaseNullary (typ:Type, tag:int, bindingFlags) = 
        let props = fieldsPropsOfUnionCase(typ, tag, bindingFlags) 
        emptyArray props

    let getUnionCaseConstructorMethod (typ:Type,tag:int,bindingFlags) = 
        let constrname = getUnionTagConverter (typ,bindingFlags) tag 
        let methname = 
            if isUnionCaseNullary (typ, tag, bindingFlags) then "get_"+constrname 
            elif isListType typ || isOptionType typ then constrname
            else "New"+constrname 
        match typ.GetMethod(methname, BindingFlags.Static  ||| bindingFlags) with
        | null -> raise <| System.InvalidOperationException (SR.GetString1(SR.constructorForUnionCaseNotFound, methname))
        | meth -> meth

    let getUnionCaseConstructor (typ:Type,tag:int,bindingFlags) = 
        let meth = getUnionCaseConstructorMethod (typ,tag,bindingFlags)
        (fun args -> 
#if FX_ATLEAST_PORTABLE
            meth.Invoke(null,args))
#else        
            meth.Invoke(null,BindingFlags.Static ||| BindingFlags.InvokeMethod ||| bindingFlags,null,args,null))
#endif
    let checkUnionType(unionType,bindingFlags) =
        checkNonNull "unionType" unionType;
        if not (isUnionType (unionType,bindingFlags)) then 
            if isUnionType (unionType,bindingFlags ||| BindingFlags.NonPublic) then 
                invalidArg "unionType" (SR.GetString1(SR.privateUnionType, unionType.FullName))
            else
                invalidArg "unionType" (SR.GetString1(SR.notAUnionType, unionType.FullName))
    let emptyObjArray : obj[] = [| |]

    //-----------------------------------------------------------------
    // TUPLE DECOMPILATION
    
    let tuple1 = typedefof<Tuple<obj>>
    let tuple2 = typedefof<obj * obj>
    let tuple3 = typedefof<obj * obj * obj>
    let tuple4 = typedefof<obj * obj * obj * obj>
    let tuple5 = typedefof<obj * obj * obj * obj * obj>
    let tuple6 = typedefof<obj * obj * obj * obj * obj * obj>
    let tuple7 = typedefof<obj * obj * obj * obj * obj * obj * obj>
    let tuple8 = typedefof<obj * obj * obj * obj * obj * obj * obj * obj>

    let stuple1 = typedefof<StructTuple<obj>>
    let stuple2 = typedefof<StructTuple<obj,obj>>
    let stuple3 = typedefof<StructTuple<obj,obj,obj>>
    let stuple4 = typedefof<StructTuple<obj,obj,obj,obj>>
    let stuple5 = typedefof<StructTuple<obj,obj,obj,obj,obj>>
    let stuple6 = typedefof<StructTuple<obj,obj,obj,obj,obj,obj>>
    let stuple7 = typedefof<StructTuple<obj,obj,obj,obj,obj,obj,obj>>
    let stuple8 = typedefof<StructTuple<obj,obj,obj,obj,obj,obj,obj,obj>>

    let isTuple1Type typ = equivHeadTypes typ tuple1
    let isTuple2Type typ = equivHeadTypes typ tuple2
    let isTuple3Type typ = equivHeadTypes typ tuple3
    let isTuple4Type typ = equivHeadTypes typ tuple4
    let isTuple5Type typ = equivHeadTypes typ tuple5
    let isTuple6Type typ = equivHeadTypes typ tuple6
    let isTuple7Type typ = equivHeadTypes typ tuple7
    let isTuple8Type typ = equivHeadTypes typ tuple8

    let isStructTuple1Type typ = equivHeadTypes typ stuple1
    let isStructTuple2Type typ = equivHeadTypes typ stuple2
    let isStructTuple3Type typ = equivHeadTypes typ stuple3
    let isStructTuple4Type typ = equivHeadTypes typ stuple4
    let isStructTuple5Type typ = equivHeadTypes typ stuple5
    let isStructTuple6Type typ = equivHeadTypes typ stuple6
    let isStructTuple7Type typ = equivHeadTypes typ stuple7
    let isStructTuple8Type typ = equivHeadTypes typ stuple8

    let isTupleType typ = 
           isTuple1Type typ
        || isTuple2Type typ
        || isTuple3Type typ 
        || isTuple4Type typ 
        || isTuple5Type typ 
        || isTuple6Type typ 
        || isTuple7Type typ 
        || isTuple8Type typ
        || isStructTuple1Type typ
        || isStructTuple2Type typ
        || isStructTuple3Type typ 
        || isStructTuple4Type typ 
        || isStructTuple5Type typ 
        || isStructTuple6Type typ 
        || isStructTuple7Type typ 
        || isStructTuple8Type typ

    let maxTuple = 8
    // Which field holds the nested tuple?
    let tupleEncField = maxTuple-1
    
    let rec mkTupleType isStruct (tys: Type[]) = 
        match tys.Length with 
        | 1 -> (if isStruct then stuple1 else tuple1).MakeGenericType(tys)
        | 2 -> (if isStruct then stuple2 else tuple2).MakeGenericType(tys)
        | 3 -> (if isStruct then stuple3 else tuple3).MakeGenericType(tys)
        | 4 -> (if isStruct then stuple4 else tuple4).MakeGenericType(tys)
        | 5 -> (if isStruct then stuple5 else tuple5).MakeGenericType(tys)
        | 6 -> (if isStruct then stuple6 else tuple6).MakeGenericType(tys)
        | 7 -> (if isStruct then stuple7 else tuple7).MakeGenericType(tys)
        | n when n >= maxTuple -> 
            let tysA = tys.[0..tupleEncField-1]
            let tysB = tys.[maxTuple-1..]
            let tyB = mkTupleType isStruct tysB
            (if isStruct then stuple8 else tuple8).MakeGenericType(Array.append tysA [| tyB |])
        | _ -> invalidArg "tys" (SR.GetString(SR.invalidTupleTypes))


    let rec getTupleTypeInfo    (typ:Type) = 
      if not (isTupleType (typ) ) then invalidArg "typ" (SR.GetString1(SR.notATupleType, typ.FullName));
      let tyargs = typ.GetGenericArguments()
      if tyargs.Length = maxTuple then 
          let tysA = tyargs.[0..tupleEncField-1]
          let tyB = tyargs.[tupleEncField]
          Array.append tysA (getTupleTypeInfo tyB)
      else 
          tyargs

    let orderTupleProperties (props:PropertyInfo[]) =
        // The tuple properties are of the form:
        //   Item1
        //   ..
        //   Item1, Item2, ..., Item<maxTuple-1>
        //   Item1, Item2, ..., Item<maxTuple-1>, Rest
        // The PropertyInfo may not come back in order, so ensure ordering here.
#if FX_ATLEAST_PORTABLE
#else
        assert(maxTuple < 10) // Alphasort will only works for upto 9 items: Item1, Item10, Item2, Item3, ..., Item9, Rest
#endif
        let props = props |> Array.sortBy (fun p -> p.Name) // they are not always in alphabetic order
#if FX_ATLEAST_PORTABLE  
#else      
        assert(props.Length <= maxTuple)
        assert(let haveNames   = props |> Array.map (fun p -> p.Name)
               let expectNames = Array.init props.Length (fun i -> let j = i+1 // index j = 1,2,..,props.Length <= maxTuple
                                                                   if   j<maxTuple then "Item" + string j
                                                                   elif j=maxTuple then "Rest"
                                                                   else (assert false; "")) // dead code under prior assert, props.Length <= maxTuple
               haveNames = expectNames)
#endif               
        props
            
    let getTupleConstructorMethod(typ:Type,bindingFlags) =
          let props = typ.GetProperties() |> orderTupleProperties
#if FX_ATLEAST_PORTABLE
          let ctor = typ.GetConstructor(props |> Array.map (fun p -> p.PropertyType))
          ignore bindingFlags
#else          
          let ctor = typ.GetConstructor(BindingFlags.Instance ||| bindingFlags,null,props |> Array.map (fun p -> p.PropertyType),null)
#endif          
          match ctor with
          | null -> raise <| ArgumentException(SR.GetString1(SR.invalidTupleTypeConstructorNotDefined, typ.FullName))
          | _ -> ()
          ctor
        
    let getTupleCtor(typ:Type,bindingFlags) =
          let ctor = getTupleConstructorMethod(typ,bindingFlags)
          (fun (args:obj[]) ->
#if FX_ATLEAST_PORTABLE   
              ctor.Invoke(args))
#else
              ctor.Invoke(BindingFlags.InvokeMethod ||| BindingFlags.Instance ||| bindingFlags,null,args,null))
#endif              

    let rec getTupleReader (typ:Type) = 
        let etys = typ.GetGenericArguments() 
        // Get the reader for the outer tuple record
        let props = typ.GetProperties(instancePropertyFlags ||| BindingFlags.Public) |> orderTupleProperties
        let reader = (fun (obj:obj) -> props |> Array.map (fun prop -> prop.GetValue(obj,null)))
        if etys.Length < maxTuple 
        then reader
        else
            let tyBenc = etys.[tupleEncField]
            let reader2 = getTupleReader(tyBenc)
            (fun obj ->
                let directVals = reader obj
                let encVals = reader2 directVals.[tupleEncField]
                Array.append directVals.[0..tupleEncField-1] encVals)
                
    let rec getTupleConstructor (typ:Type) = 
        let etys = typ.GetGenericArguments() 
        let maker1 =  getTupleCtor (typ,BindingFlags.Public)
        if etys.Length < maxTuple 
        then maker1
        else
            let tyBenc = etys.[tupleEncField]
            let maker2 = getTupleConstructor(tyBenc)
            (fun (args:obj[]) ->
                let encVal = maker2 args.[tupleEncField..]
                maker1 (Array.append args.[0..tupleEncField-1] [| encVal |]))
                
    let getTupleConstructorInfo (typ:Type) = 
        let etys = typ.GetGenericArguments() 
        let maker1 =  getTupleConstructorMethod (typ,BindingFlags.Public)
        if etys.Length < maxTuple then
            maker1,None
        else
            maker1,Some(etys.[tupleEncField])

    let getTupleReaderInfo (typ:Type,index:int) =         
        if index < 0 then invalidArg "index" (SR.GetString2(SR.tupleIndexOutOfRange, typ.FullName, index.ToString()))
        let props = typ.GetProperties(instancePropertyFlags ||| BindingFlags.Public) |> orderTupleProperties
        let get index = 
            if index >= props.Length then invalidArg "index" (SR.GetString2(SR.tupleIndexOutOfRange, typ.FullName, index.ToString()))
            props.[index]
        
        if index < tupleEncField then
            get index, None  
        else
            let etys = typ.GetGenericArguments()
            get tupleEncField, Some(etys.[tupleEncField],index-(maxTuple-1))
            
      
    //-----------------------------------------------------------------
    // FUNCTION DECOMPILATION
    
      
    let getFunctionTypeInfo (typ:Type) =
      if not (isFunctionType typ) then invalidArg "typ" (SR.GetString1(SR.notAFunctionType, typ.FullName))
      let tyargs = typ.GetGenericArguments()
      tyargs.[0], tyargs.[1]

    //-----------------------------------------------------------------
    // MODULE DECOMPILATION
    
    let isModuleType (typ:Type) = 
      match tryFindSourceConstructFlagsOfType(typ) with 
      | None -> false 
      | Some(flags) -> 
        (flags &&& SourceConstructFlags.KindMask) = SourceConstructFlags.Module 

    let rec isClosureRepr typ = 
        isFunctionType typ || 
        (match typ.BaseType with null -> false | bty -> isClosureRepr bty) 

    //-----------------------------------------------------------------
    // RECORD DECOMPILATION
    
    let isRecordType (typ:Type,bindingFlags:BindingFlags) = 
      match tryFindSourceConstructFlagsOfType(typ) with 
      | None -> false 
      | Some(flags) ->
        (flags &&& SourceConstructFlags.KindMask) = SourceConstructFlags.RecordType &&
        // We see private representations only if BindingFlags.NonPublic is set
        (if (flags &&& SourceConstructFlags.NonPublicRepresentation) <> enum(0) then 
            (bindingFlags &&& BindingFlags.NonPublic) <> enum(0)
         else 
            true) &&
        not (isTupleType typ)

    let fieldPropsOfRecordType(typ:Type,bindingFlags) =
      typ.GetProperties(instancePropertyFlags ||| bindingFlags) 
      |> Array.filter isFieldProperty
      |> sortFreshArray (fun p1 p2 -> compare (sequenceNumberOfMember p1) (sequenceNumberOfMember p2))

    let getRecd obj (props:PropertyInfo[]) = 
        props |> Array.map (fun prop -> prop.GetValue(obj,null))

    let getRecordReader(typ:Type,bindingFlags) = 
        let props = fieldPropsOfRecordType(typ,bindingFlags)
        (fun (obj:obj) -> props |> Array.map (fun prop -> prop.GetValue(obj,null)))

    let getRecordConstructorMethod(typ:Type,bindingFlags) = 
        let props = fieldPropsOfRecordType(typ,bindingFlags)
#if FX_ATLEAST_PORTABLE
        let ctor = typ.GetConstructor(props |> Array.map (fun p -> p.PropertyType))
#else        
        let ctor = typ.GetConstructor(BindingFlags.Instance ||| bindingFlags,null,props |> Array.map (fun p -> p.PropertyType),null)
#endif        
        match ctor with
        | null -> raise <| ArgumentException(SR.GetString1(SR.invalidRecordTypeConstructorNotDefined, typ.FullName))
        | _ -> ()
        ctor

    let getRecordConstructor(typ:Type,bindingFlags) = 
        let ctor = getRecordConstructorMethod(typ,bindingFlags)
        (fun (args:obj[]) -> 
#if FX_ATLEAST_PORTABLE
            ctor.Invoke(args))
#else        
            ctor.Invoke(BindingFlags.InvokeMethod  ||| BindingFlags.Instance ||| bindingFlags,null,args,null))
#endif            

    //-----------------------------------------------------------------
    // EXCEPTION DECOMPILATION
    

    // Check the base type - if it is also an F# type then
    // for the moment we know it is a Discriminated Union
    let isExceptionRepr (typ:Type,bindingFlags) = 
        match tryFindSourceConstructFlagsOfType(typ) with 
        | None -> false 
        | Some(flags) -> 
          ((flags &&& SourceConstructFlags.KindMask) = SourceConstructFlags.Exception) &&
          // We see private representations only if BindingFlags.NonPublic is set
          (if (flags &&& SourceConstructFlags.NonPublicRepresentation) <> enum(0) then 
              (bindingFlags &&& BindingFlags.NonPublic) <> enum(0)
           else 
              true)


    let getTypeOfReprType (typ:Type,bindingFlags) = 
        if isExceptionRepr(typ,bindingFlags) then typ.BaseType
        elif isConstructorRepr(typ,bindingFlags) then unionTypeOfUnionCaseType(typ,bindingFlags)
        elif isClosureRepr(typ) then 
          let rec get (typ:Type) = if isFunctionType typ then typ else match typ.BaseType with null -> typ | b -> get b
          get typ 
        else typ


    //-----------------------------------------------------------------
    // CHECKING ROUTINES

    let checkExnType (exceptionType, bindingFlags) =
        if not (isExceptionRepr (exceptionType,bindingFlags)) then 
            if isExceptionRepr (exceptionType,bindingFlags ||| BindingFlags.NonPublic) then 
                invalidArg "exceptionType" (SR.GetString1(SR.privateExceptionType, exceptionType.FullName))
            else
                invalidArg "exceptionType" (SR.GetString1(SR.notAnExceptionType, exceptionType.FullName))
           
    let checkRecordType(argName,recordType,bindingFlags) =
        checkNonNull argName recordType;
        if not (isRecordType (recordType,bindingFlags) ) then 
            if isRecordType (recordType,bindingFlags ||| BindingFlags.NonPublic) then 
                invalidArg argName (SR.GetString1(SR.privateRecordType, recordType.FullName))
            else
                invalidArg argName (SR.GetString1(SR.notARecordType, recordType.FullName))
        
    let checkTupleType(argName,tupleType) =
        checkNonNull argName tupleType;
        if not (isTupleType tupleType) then invalidArg argName (SR.GetString1(SR.notATupleType, tupleType.FullName))

#if FX_RESHAPED_REFLECTION
open ReflectionAdapters
type BindingFlags = ReflectionAdapters.BindingFlags
#endif
        
[<Sealed>]
type UnionCaseInfo(typ: System.Type, tag:int) =
    // Cache the tag -> name map
    let mutable names = None
    let getMethInfo() = Impl.getUnionCaseConstructorMethod (typ, tag, BindingFlags.Public ||| BindingFlags.NonPublic) 
    member x.Name = 
        match names with 
        | None -> (let conv = Impl.getUnionTagConverter (typ,BindingFlags.Public ||| BindingFlags.NonPublic) in names <- Some conv; conv tag)
        | Some conv -> conv tag
        
    member x.DeclaringType = typ
    //member x.CustomAttributes = failwith<obj[]> "nyi"
    member x.GetFields() = 
        let props = Impl.fieldsPropsOfUnionCase(typ,tag,BindingFlags.Public ||| BindingFlags.NonPublic) 
        props

    member x.GetCustomAttributes() = getMethInfo().GetCustomAttributes(false)
    
    member x.GetCustomAttributes(attributeType) = getMethInfo().GetCustomAttributes(attributeType,false)

#if FX_NO_CUSTOMATTRIBUTEDATA
#else
    member x.GetCustomAttributesData() = getMethInfo().GetCustomAttributesData()
#endif    
    member x.Tag = tag
    override x.ToString() = typ.Name + "." + x.Name
    override x.GetHashCode() = typ.GetHashCode() + tag
    override x.Equals(obj:obj) = 
        match obj with 
        | :? UnionCaseInfo as uci -> uci.DeclaringType = typ && uci.Tag = tag
        | _ -> false
    

[<AbstractClass; Sealed>]
type FSharpType = 

    static member IsTuple(typ:Type) =  
        Impl.checkNonNull "typ" typ;
        Impl.isTupleType typ

    static member IsRecord(typ:Type,?bindingFlags) =  
        let bindingFlags = defaultArg bindingFlags BindingFlags.Public 

        Impl.checkNonNull "typ" typ;
        Impl.isRecordType (typ,bindingFlags)

    static member IsUnion(typ:Type,?bindingFlags) =  
        let bindingFlags = defaultArg bindingFlags BindingFlags.Public 
        Impl.checkNonNull "typ" typ;
        let typ = Impl.getTypeOfReprType (typ ,BindingFlags.Public ||| BindingFlags.NonPublic)
        Impl.isUnionType (typ,bindingFlags)

    static member IsFunction(typ:Type) =  
        Impl.checkNonNull "typ" typ;
        let typ = Impl.getTypeOfReprType (typ ,BindingFlags.Public ||| BindingFlags.NonPublic)
        Impl.isFunctionType typ

    static member IsModule(typ:Type) =  
        Impl.checkNonNull "typ" typ;
        Impl.isModuleType typ

    static member MakeFunctionType(domain:Type,range:Type) = 
        Impl.checkNonNull "domain" domain;
        Impl.checkNonNull "range" range;
        Impl.func.MakeGenericType [| domain; range |]

    static member MakeTupleType(types:Type[]) =  
        Impl.checkNonNull "types" types;
        if types |> Array.exists (function null -> true | _ -> false) then 
             invalidArg "types" (SR.GetString(SR.nullsNotAllowedInArray))
        Impl.mkTupleType false types

    static member MakeStructTupleType(types:Type[]) =  
        Impl.checkNonNull "types" types;
        if types |> Array.exists (function null -> true | _ -> false) then 
             invalidArg "types" (SR.GetString(SR.nullsNotAllowedInArray))
        Impl.mkTupleType true types

    static member GetTupleElements(tupleType:Type) =
        Impl.checkTupleType("tupleType",tupleType);
        Impl.getTupleTypeInfo tupleType

    static member GetFunctionElements(functionType:Type) =
        Impl.checkNonNull "functionType" functionType;
        let functionType = Impl.getTypeOfReprType (functionType ,BindingFlags.Public ||| BindingFlags.NonPublic)
        Impl.getFunctionTypeInfo functionType

    static member GetRecordFields(recordType:Type,?bindingFlags) =
        let bindingFlags = defaultArg bindingFlags BindingFlags.Public 
        Impl.checkRecordType("recordType",recordType,bindingFlags);
        Impl.fieldPropsOfRecordType(recordType,bindingFlags)

    static member GetUnionCases (unionType:Type,?bindingFlags) = 
        let bindingFlags = defaultArg bindingFlags BindingFlags.Public 
        Impl.checkNonNull "unionType" unionType;
        let unionType = Impl.getTypeOfReprType (unionType ,bindingFlags)
        Impl.checkUnionType(unionType,bindingFlags);
        Impl.getUnionTypeTagNameMap(unionType,bindingFlags) |> Array.mapi (fun i _ -> UnionCaseInfo(unionType,i))

    static member IsExceptionRepresentation(exceptionType:Type, ?bindingFlags) = 
        let bindingFlags = defaultArg bindingFlags BindingFlags.Public
        Impl.checkNonNull "exceptionType" exceptionType;
        Impl.isExceptionRepr(exceptionType,bindingFlags)

    static member GetExceptionFields(exceptionType:Type, ?bindingFlags) = 
        let bindingFlags = defaultArg bindingFlags BindingFlags.Public 
        Impl.checkNonNull "exceptionType" exceptionType;
        Impl.checkExnType(exceptionType,bindingFlags);
        Impl.fieldPropsOfRecordType (exceptionType,bindingFlags) 

type DynamicFunction<'T1,'T2>() =
    inherit FSharpFunc<obj -> obj, obj>()
    override x.Invoke(impl: obj -> obj) : obj = 
        box<('T1 -> 'T2)> (fun inp -> unbox<'T2>(impl (box<'T1>(inp))))

[<AbstractClass; Sealed>]
type FSharpValue = 

    static member MakeRecord(recordType:Type,args,?bindingFlags) = 
        let bindingFlags = defaultArg bindingFlags BindingFlags.Public 
        Impl.checkRecordType("recordType",recordType,bindingFlags);
        Impl.getRecordConstructor (recordType,bindingFlags) args

    static member GetRecordField(record:obj,info:PropertyInfo) =
        Impl.checkNonNull "info" info;
        Impl.checkNonNull "record" record;
        let reprty = record.GetType() 
        if not (Impl.isRecordType(reprty,BindingFlags.Public ||| BindingFlags.NonPublic)) then invalidArg "record" (SR.GetString(SR.objIsNotARecord));
        info.GetValue(record,null)

    static member GetRecordFields(record:obj,?bindingFlags) =
        let bindingFlags = defaultArg bindingFlags BindingFlags.Public 
        Impl.checkNonNull "record" record;
        let typ = record.GetType() 
        if not (Impl.isRecordType(typ,bindingFlags)) then invalidArg "record" (SR.GetString(SR.objIsNotARecord));
        Impl.getRecordReader (typ,bindingFlags) record

    static member PreComputeRecordFieldReader(info:PropertyInfo) = 
        Impl.checkNonNull "info" info;
        (fun (obj:obj) -> info.GetValue(obj,null))

    static member PreComputeRecordReader(recordType:Type,?bindingFlags) : (obj -> obj[]) =  
        let bindingFlags = defaultArg bindingFlags BindingFlags.Public 
        Impl.checkRecordType("recordType",recordType,bindingFlags);
        Impl.getRecordReader (recordType,bindingFlags)

    static member PreComputeRecordConstructor(recordType:Type,?bindingFlags) = 
        let bindingFlags = defaultArg bindingFlags BindingFlags.Public 
        Impl.checkRecordType("recordType",recordType,bindingFlags);
        Impl.getRecordConstructor (recordType,bindingFlags)

    static member PreComputeRecordConstructorInfo(recordType:Type, ?bindingFlags) =
        let bindingFlags = defaultArg bindingFlags BindingFlags.Public 
        Impl.checkRecordType("recordType",recordType,bindingFlags);
        Impl.getRecordConstructorMethod(recordType,bindingFlags)

    static member MakeFunction(functionType:Type,implementation:(obj->obj)) = 
        Impl.checkNonNull "functionType" functionType;
        if not (Impl.isFunctionType functionType) then invalidArg "functionType" (SR.GetString1(SR.notAFunctionType, functionType.FullName));
        Impl.checkNonNull "implementation" implementation;
        let domain,range = Impl.getFunctionTypeInfo functionType
        let dynCloMakerTy = typedefof<DynamicFunction<obj,obj>>
        let saverTy = dynCloMakerTy.MakeGenericType [| domain; range |]
        let o = Activator.CreateInstance(saverTy)
        let (f : (obj -> obj) -> obj) = downcast o
        f implementation

    static member MakeTuple(tupleElements: obj[],tupleType:Type) =
        Impl.checkNonNull "tupleElements" tupleElements;
        Impl.checkTupleType("tupleType",tupleType) 
        Impl.getTupleConstructor tupleType tupleElements
    
    static member GetTupleFields(tuple:obj) = // argument name(s) used in error message
        Impl.checkNonNull "tuple" tuple;
        let typ = tuple.GetType() 
        if not (Impl.isTupleType typ ) then invalidArg "tuple" (SR.GetString1(SR.notATupleType, tuple.GetType().FullName));
        Impl.getTupleReader typ tuple

    static member GetTupleField(tuple:obj,index:int) = // argument name(s) used in error message
        Impl.checkNonNull "tuple" tuple;
        let typ = tuple.GetType() 
        if not (Impl.isTupleType typ ) then invalidArg "tuple" (SR.GetString1(SR.notATupleType, tuple.GetType().FullName));
        let fields = Impl.getTupleReader typ tuple
        if index < 0 || index >= fields.Length then invalidArg "index" (SR.GetString2(SR.tupleIndexOutOfRange, tuple.GetType().FullName, index.ToString()));
        fields.[index]
    
    static member PreComputeTupleReader(tupleType:Type) : (obj -> obj[])  =
        Impl.checkTupleType("tupleType",tupleType) 
        Impl.getTupleReader tupleType
    
    static member PreComputeTuplePropertyInfo(tupleType:Type,index:int) =
        Impl.checkTupleType("tupleType",tupleType) 
        Impl.getTupleReaderInfo (tupleType,index)
    
    static member PreComputeTupleConstructor(tupleType:Type) = 
        Impl.checkTupleType("tupleType",tupleType) 
        Impl.getTupleConstructor tupleType

    static member PreComputeTupleConstructorInfo(tupleType:Type) =
        Impl.checkTupleType("tupleType",tupleType) 
        Impl.getTupleConstructorInfo (tupleType) 

    static member MakeUnion(unionCase:UnionCaseInfo,args: obj [],?bindingFlags) = 
        let bindingFlags = defaultArg bindingFlags BindingFlags.Public
        Impl.checkNonNull "unionCase" unionCase;
        Impl.getUnionCaseConstructor (unionCase.DeclaringType,unionCase.Tag,bindingFlags) args

    static member PreComputeUnionConstructor (unionCase:UnionCaseInfo,?bindingFlags) = 
        let bindingFlags = defaultArg bindingFlags BindingFlags.Public 
        Impl.checkNonNull "unionCase" unionCase;
        Impl.getUnionCaseConstructor (unionCase.DeclaringType,unionCase.Tag,bindingFlags)

    static member PreComputeUnionConstructorInfo(unionCase:UnionCaseInfo, ?bindingFlags) =
        let bindingFlags = defaultArg bindingFlags BindingFlags.Public 
        Impl.checkNonNull "unionCase" unionCase;
        Impl.getUnionCaseConstructorMethod (unionCase.DeclaringType,unionCase.Tag,bindingFlags)

    static member GetUnionFields(obj:obj,unionType:Type,?bindingFlags) = 
        let bindingFlags = defaultArg bindingFlags BindingFlags.Public 
        let ensureType (typ:Type,obj:obj) = 
                match typ with 
                | null -> 
                    match obj with 
                    | null -> invalidArg "obj" (SR.GetString(SR.objIsNullAndNoType))
                    | _ -> obj.GetType()
                | _ -> typ 
        //System.Console.WriteLine("typ1 = {0}",box unionType)
        let unionType = ensureType(unionType,obj) 
        //System.Console.WriteLine("typ2 = {0}",box unionType)
        Impl.checkNonNull "unionType" unionType;
        let unionType = Impl.getTypeOfReprType (unionType ,bindingFlags)
        //System.Console.WriteLine("typ3 = {0}",box unionType)
        Impl.checkUnionType(unionType,bindingFlags);
        let tag = Impl.getUnionTagReader (unionType,bindingFlags) obj
        let flds = Impl.getUnionCaseRecordReader (unionType,tag,bindingFlags) obj 
        UnionCaseInfo(unionType,tag), flds
        
    static member PreComputeUnionTagReader(unionType: Type,?bindingFlags) : (obj -> int) = 
        let bindingFlags = defaultArg bindingFlags BindingFlags.Public 
        Impl.checkNonNull "unionType" unionType;
        let unionType = Impl.getTypeOfReprType (unionType ,bindingFlags)
        Impl.checkUnionType(unionType,bindingFlags);
        Impl.getUnionTagReader (unionType ,bindingFlags)


    static member PreComputeUnionTagMemberInfo(unionType: Type,?bindingFlags) = 
        let bindingFlags = defaultArg bindingFlags BindingFlags.Public 
        Impl.checkNonNull "unionType" unionType;
        let unionType = Impl.getTypeOfReprType (unionType ,bindingFlags)
        Impl.checkUnionType(unionType,bindingFlags);
        Impl.getUnionTagMemberInfo(unionType ,bindingFlags)

    static member PreComputeUnionReader(unionCase: UnionCaseInfo,?bindingFlags) : (obj -> obj[])  = 
        let bindingFlags = defaultArg bindingFlags BindingFlags.Public 
        Impl.checkNonNull "unionCase" unionCase;
        let typ = unionCase.DeclaringType 
        Impl.getUnionCaseRecordReader (typ,unionCase.Tag,bindingFlags)

    static member GetExceptionFields(exn:obj, ?bindingFlags) = 
        let bindingFlags = defaultArg bindingFlags BindingFlags.Public 
        Impl.checkNonNull "exn" exn;
        let typ = exn.GetType() 
        Impl.checkExnType(typ,bindingFlags);
        Impl.getRecordReader (typ,bindingFlags) exn

module FSharpReflectionExtensions =

    type FSharpType with

        static member GetExceptionFields(exceptionType:Type, ?allowAccessToPrivateRepresentation) = 
            let bindingFlags = Impl.getBindingFlags allowAccessToPrivateRepresentation
            FSharpType.GetExceptionFields(exceptionType, bindingFlags)

        static member IsExceptionRepresentation(exceptionType:Type, ?allowAccessToPrivateRepresentation) = 
            let bindingFlags = Impl.getBindingFlags allowAccessToPrivateRepresentation
            FSharpType.IsExceptionRepresentation(exceptionType, bindingFlags)

        static member GetUnionCases (unionType:Type,?allowAccessToPrivateRepresentation) = 
            let bindingFlags = Impl.getBindingFlags allowAccessToPrivateRepresentation
            FSharpType.GetUnionCases(unionType, bindingFlags)

        static member GetRecordFields(recordType:Type,?allowAccessToPrivateRepresentation) =
            let bindingFlags = Impl.getBindingFlags allowAccessToPrivateRepresentation
            FSharpType.GetRecordFields(recordType, bindingFlags)

        static member IsUnion(typ:Type,?allowAccessToPrivateRepresentation) =  
            let bindingFlags = Impl.getBindingFlags allowAccessToPrivateRepresentation
            FSharpType.IsUnion(typ, bindingFlags)

        static member IsRecord(typ:Type,?allowAccessToPrivateRepresentation) =  
            let bindingFlags = Impl.getBindingFlags allowAccessToPrivateRepresentation
            FSharpType.IsRecord(typ, bindingFlags)

    type FSharpValue with
        static member MakeRecord(recordType:Type,args,?allowAccessToPrivateRepresentation) = 
            let bindingFlags = Impl.getBindingFlags allowAccessToPrivateRepresentation
            FSharpValue.MakeRecord(recordType, args, bindingFlags)

        static member GetRecordFields(record:obj,?allowAccessToPrivateRepresentation) =
            let bindingFlags = Impl.getBindingFlags allowAccessToPrivateRepresentation
            FSharpValue.GetRecordFields(record, bindingFlags)

        static member PreComputeRecordReader(recordType:Type,?allowAccessToPrivateRepresentation) : (obj -> obj[]) =  
            let bindingFlags = Impl.getBindingFlags allowAccessToPrivateRepresentation
            FSharpValue.PreComputeRecordReader(recordType, bindingFlags)

        static member PreComputeRecordConstructor(recordType:Type,?allowAccessToPrivateRepresentation) = 
            let bindingFlags = Impl.getBindingFlags allowAccessToPrivateRepresentation 
            FSharpValue.PreComputeRecordConstructor(recordType, bindingFlags)

        static member PreComputeRecordConstructorInfo(recordType:Type, ?allowAccessToPrivateRepresentation) =
            let bindingFlags = Impl.getBindingFlags allowAccessToPrivateRepresentation 
            FSharpValue.PreComputeRecordConstructorInfo(recordType, bindingFlags)

        static member MakeUnion(unionCase:UnionCaseInfo,args: obj [],?allowAccessToPrivateRepresentation) = 
            let bindingFlags = Impl.getBindingFlags allowAccessToPrivateRepresentation 
            FSharpValue.MakeUnion(unionCase, args, bindingFlags)

        static member PreComputeUnionConstructor (unionCase:UnionCaseInfo,?allowAccessToPrivateRepresentation) = 
            let bindingFlags = Impl.getBindingFlags allowAccessToPrivateRepresentation 
            FSharpValue.PreComputeUnionConstructor(unionCase, bindingFlags)

        static member PreComputeUnionConstructorInfo(unionCase:UnionCaseInfo, ?allowAccessToPrivateRepresentation) =
            let bindingFlags = Impl.getBindingFlags allowAccessToPrivateRepresentation 
            FSharpValue.PreComputeUnionConstructorInfo(unionCase, bindingFlags)

        static member PreComputeUnionTagMemberInfo(unionType: Type,?allowAccessToPrivateRepresentation) = 
            let bindingFlags = Impl.getBindingFlags allowAccessToPrivateRepresentation
            FSharpValue.PreComputeUnionTagMemberInfo(unionType, bindingFlags)

        static member GetUnionFields(obj:obj,unionType:Type,?allowAccessToPrivateRepresentation) = 
            let bindingFlags = Impl.getBindingFlags allowAccessToPrivateRepresentation 
            FSharpValue.GetUnionFields(obj, unionType, bindingFlags)

        static member PreComputeUnionTagReader(unionType: Type,?allowAccessToPrivateRepresentation) : (obj -> int) = 
            let bindingFlags = Impl.getBindingFlags allowAccessToPrivateRepresentation
            FSharpValue.PreComputeUnionTagReader(unionType, bindingFlags)

        static member PreComputeUnionReader(unionCase: UnionCaseInfo,?allowAccessToPrivateRepresentation) : (obj -> obj[])  = 
            let bindingFlags = Impl.getBindingFlags allowAccessToPrivateRepresentation
            FSharpValue.PreComputeUnionReader(unionCase, bindingFlags)

        static member GetExceptionFields(exn:obj, ?allowAccessToPrivateRepresentation) = 
            let bindingFlags = Impl.getBindingFlags allowAccessToPrivateRepresentation
            FSharpValue.GetExceptionFields(exn, bindingFlags)

