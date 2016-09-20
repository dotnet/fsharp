// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

// Reflection on F# values. Analyze an object to see if it the representation
// of an F# value.


namespace Microsoft.FSharp.Core

open System
open System.Reflection
open System.Threading
open Microsoft.FSharp.Core.LanguagePrimitives.IntrinsicOperators
open Microsoft.FSharp.Collections

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

#if FX_RESHAPED_REFLECTION
    open PrimReflectionAdapters
    open ReflectionAdapters    
#endif

    let getBindingFlags allowAccess = ReflectionUtils.toBindingFlags (defaultArg allowAccess false)

    let inline checkNonNull argName (v: 'T) = 
        match box v with 
        | null -> nullArg argName 
        | _ -> ()
         
    let isNamedType(typ:Type) = not (typ.IsArray || typ.IsByRef || typ.IsPointer)

    let equivHeadTypes (ty1:Type) (ty2:Type) = 
        isNamedType(ty1) &&
        if ty1.IsGenericType then 
          ty2.IsGenericType && (ty1.GetGenericTypeDefinition()).Equals(ty2.GetGenericTypeDefinition())
        else 
          ty1.Equals(ty2)

    let func = typedefof<(obj -> obj)>

    let isOptionType typ = equivHeadTypes typ (typeof<int option>)
    let isFunctionType typ = equivHeadTypes typ (typeof<(int -> int)>)
    let isListType typ = equivHeadTypes typ (typeof<int list>)

    //-----------------------------------------------------------------
    // GENERAL UTILITIES
#if FX_ATLEAST_PORTABLE
    let instanceFieldFlags = BindingFlags.Instance 
    let instancePropertyFlags = BindingFlags.Instance 
    let staticPropertyFlags = BindingFlags.Static
    let staticFieldFlags = BindingFlags.Static 
    let staticMethodFlags = BindingFlags.Static 
#else    
    let instanceFieldFlags = BindingFlags.GetField ||| BindingFlags.Instance 
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
                         | 1 -> ((let x = args.[0] in x.Value :?> SourceConstructFlags), 0, 0)
                         | 2 -> ((let x = args.[0] in x.Value :?> SourceConstructFlags), (let x = args.[1] in x.Value :?> int), 0)
                         | 3 -> ((let x = args.[0] in x.Value :?> SourceConstructFlags), (let x = args.[1] in x.Value :?> int), (let x = args.[2] in x.Value :?> int))
                         | _ -> (enum 0, 0, 0)
                    res <- Some flags
            res

    let findCompilationMappingAttributeFromData attrs =
      match tryFindCompilationMappingAttributeFromData attrs with
      | None -> failwith "no compilation mapping attribute"
      | Some a -> a

    let tryFindCompilationMappingAttributeFromType       (typ:Type)        = 
        let assem = typ.Assembly
        if isNotNull assem && assem.ReflectionOnly then 
           tryFindCompilationMappingAttributeFromData ( typ.GetCustomAttributesData())
        else
           tryFindCompilationMappingAttribute ( typ.GetCustomAttributes (typeof<CompilationMappingAttribute>,false))

    let tryFindCompilationMappingAttributeFromMemberInfo (info:MemberInfo) = 
        let assem = info.DeclaringType.Assembly
        if isNotNull assem && assem.ReflectionOnly then 
           tryFindCompilationMappingAttributeFromData (info.GetCustomAttributesData())
        else
        tryFindCompilationMappingAttribute (info.GetCustomAttributes (typeof<CompilationMappingAttribute>,false))

    let    findCompilationMappingAttributeFromMemberInfo (info:MemberInfo) =    
        let assem = info.DeclaringType.Assembly
        if isNotNull assem && assem.ReflectionOnly then 
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
            let caseTyp = match caseTyp with null ->  typ | _ -> caseTyp
            caseTyp.GetProperties(instancePropertyFlags ||| bindingFlags) 
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
        fieldsPropsOfUnionCase(typ, tag, bindingFlags).Length = 0

    let getUnionCaseConstructorMethod (typ:Type,tag:int,bindingFlags) = 
        let constrname = getUnionTagConverter (typ,bindingFlags) tag 
        let methname = 
            if isUnionCaseNullary (typ, tag, bindingFlags) then "get_" + constrname 
            elif isListType typ || isOptionType typ then constrname
            else "New" + constrname

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

    //-----------------------------------------------------------------
    // TUPLE DECOMPILATION
    let tupleNames = [|
        "System.Tuple`1";      "System.Tuple`2";      "System.Tuple`3";
        "System.Tuple`4";      "System.Tuple`5";      "System.Tuple`6";
        "System.Tuple`7";      "System.Tuple`8";      "System.Tuple";
        "System.ValueTuple`1"; "System.ValueTuple`2"; "System.ValueTuple`3";
        "System.ValueTuple`4"; "System.ValueTuple`5"; "System.ValueTuple`6";
        "System.ValueTuple`7"; "System.ValueTuple`8"; "System.ValueTuple" |]

    let isTupleType (typ:Type) = 
        // Simple Name Match on typ
        if typ.IsEnum || typ.IsArray || typ.IsPointer then false
        else tupleNames |> Seq.exists typ.FullName.StartsWith

    let maxTuple = 8
    // Which field holds the nested tuple?
    let tupleEncField = maxTuple - 1

    let dictionaryLock = obj()
    let refTupleTypes = System.Collections.Generic.Dictionary<Assembly, Type[]>()
    let valueTupleTypes = System.Collections.Generic.Dictionary<Assembly, Type[]>()

    let rec mkTupleType isStruct (asm:Assembly) (tys:Type[]) =
        let table =
            let makeIt n =
                let tupleFullName n =
                    let structOffset = if isStruct then 9 else 0
                    let index = n - 1 + structOffset
                    tupleNames.[index]

                match n with
                | 1 -> asm.GetType(tupleFullName 1)
                | 2 -> asm.GetType(tupleFullName 2)
                | 3 -> asm.GetType(tupleFullName 3)
                | 4 -> asm.GetType(tupleFullName 4)
                | 5 -> asm.GetType(tupleFullName 5)
                | 6 -> asm.GetType(tupleFullName 6)
                | 7 -> asm.GetType(tupleFullName 7)
                | 8 -> asm.GetType(tupleFullName 8)
                | _ -> invalidArg "tys" (SR.GetString(SR.invalidTupleTypes))

            let tables = if isStruct then valueTupleTypes else refTupleTypes
            match lock dictionaryLock (fun () -> tables.TryGetValue(asm)) with
            | false, _ ->
                // the Dictionary<>s here could be ConcurrentDictionary<>'s, but then
                // that would lock while initializing the Type array (maybe not an issue)
                let a = ref (Array.init<Type> 8 (fun i -> makeIt (i + 1)))
                lock dictionaryLock (fun () ->  match tables.TryGetValue(asm) with
                                                | true, t -> a := t
                                                | false, _ -> tables.Add(asm, !a))
                !a
            | true, t -> t

        match tys.Length with
        | 1 -> table.[0].MakeGenericType(tys)
        | 2 -> table.[1].MakeGenericType(tys)
        | 3 -> table.[2].MakeGenericType(tys)
        | 4 -> table.[3].MakeGenericType(tys)
        | 5 -> table.[4].MakeGenericType(tys)
        | 6 -> table.[5].MakeGenericType(tys)
        | 7 -> table.[6].MakeGenericType(tys)
        | n when n >= maxTuple ->
            let tysA = tys.[0..tupleEncField-1]
            let tysB = tys.[maxTuple-1..]
            let tyB = mkTupleType isStruct asm tysB
            table.[7].MakeGenericType(Array.append tysA [| tyB |])
        | _ -> invalidArg "tys" (SR.GetString(SR.invalidTupleTypes))

    let rec getTupleTypeInfo (typ:Type) = 
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

    let orderTupleFields (fields:FieldInfo[]) =
        // The tuple fields are of the form:
        //   Item1
        //   ..
        //   Item1, Item2, ..., Item<maxTuple-1>
        //   Item1, Item2, ..., Item<maxTuple-1>, Rest
        // The PropertyInfo may not come back in order, so ensure ordering here.
#if FX_ATLEAST_PORTABLE
#else
        assert(maxTuple < 10) // Alphasort will only works for upto 9 items: Item1, Item10, Item2, Item3, ..., Item9, Rest
#endif
        let fields = fields |> Array.sortBy (fun fi -> fi.Name) // they are not always in alphabetic order
#if FX_ATLEAST_PORTABLE  
#else      
        assert(fields.Length <= maxTuple)
        assert(let haveNames   = fields |> Array.map (fun fi -> fi.Name)
               let expectNames = Array.init fields.Length (fun i -> let j = i+1 // index j = 1,2,..,fields.Length <= maxTuple
                                                                    if   j<maxTuple then "Item" + string j
                                                                    elif j=maxTuple then "Rest"
                                                                    else (assert false; "")) // dead code under prior assert, props.Length <= maxTuple
               haveNames = expectNames)
#endif
        fields

    let getTupleConstructorMethod(typ:Type,bindingFlags) =
        let ctor =
            if typ.IsValueType then
                let fields = typ.GetFields(bindingFlags) |> orderTupleFields
#if FX_ATLEAST_PORTABLE
                ignore bindingFlags
                typ.GetConstructor(fields |> Array.map (fun fi -> fi.FieldType))
#else
                typ.GetConstructor(BindingFlags.Instance ||| bindingFlags,null,fields |> Array.map (fun fi -> fi.FieldType),null)
#endif
            else
                let props = typ.GetProperties() |> orderTupleProperties
#if FX_ATLEAST_PORTABLE
                ignore bindingFlags
                typ.GetConstructor(props |> Array.map (fun p -> p.PropertyType))
#else
                typ.GetConstructor(BindingFlags.Instance ||| bindingFlags,null,props |> Array.map (fun p -> p.PropertyType),null)
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
        let reader =
            if typ.IsValueType then
                let fields = (typ.GetFields(instanceFieldFlags ||| BindingFlags.Public) |> orderTupleFields)
                ((fun (obj:obj) -> fields |> Array.map (fun field -> field.GetValue(obj))))
            else
                let props = (typ.GetProperties(instancePropertyFlags ||| BindingFlags.Public) |> orderTupleProperties)
                ((fun (obj:obj) -> props |> Array.map (fun prop -> prop.GetValue(obj,null))))
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

        let get index =
            if typ.IsValueType then
                let props = typ.GetProperties(instancePropertyFlags ||| BindingFlags.Public) |> orderTupleProperties
                if index >= props.Length then invalidArg "index" (SR.GetString2(SR.tupleIndexOutOfRange, typ.FullName, index.ToString()))
                props.[index]
            else
                let props = typ.GetProperties(instancePropertyFlags ||| BindingFlags.Public) |> orderTupleProperties
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
            true)

    let fieldPropsOfRecordType(typ:Type,bindingFlags) =
      typ.GetProperties(instancePropertyFlags ||| bindingFlags) 
      |> Array.filter isFieldProperty
      |> sortFreshArray (fun p1 p2 -> compare (sequenceNumberOfMember p1) (sequenceNumberOfMember p2))

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
        
    let checkTupleType(argName,(tupleType:Type)) =
        checkNonNull argName tupleType;
        if not (isTupleType tupleType) then invalidArg argName (SR.GetString1(SR.notATupleType, tupleType.FullName))

#if FX_RESHAPED_REFLECTION
open ReflectionAdapters
type internal BindingFlags = ReflectionAdapters.BindingFlags
#endif
        
[<Sealed>]
type UnionCaseInfo(typ: System.Type, tag:int) =
    // Cache the tag -> name map
    let mutable names = None
    let getMethInfo() = Impl.getUnionCaseConstructorMethod (typ, tag, BindingFlags.Public ||| BindingFlags.NonPublic) 
    member __.Name = 
        match names with 
        | None -> (let conv = Impl.getUnionTagConverter (typ,BindingFlags.Public ||| BindingFlags.NonPublic) in names <- Some conv; conv tag)
        | Some conv -> conv tag
        
    member __.DeclaringType = typ

    member __.GetFields() = 
        let props = Impl.fieldsPropsOfUnionCase(typ,tag,BindingFlags.Public ||| BindingFlags.NonPublic) 
        props

    member __.GetCustomAttributes() = getMethInfo().GetCustomAttributes(false)
    
    member __.GetCustomAttributes(attributeType) = getMethInfo().GetCustomAttributes(attributeType,false)

#if FX_NO_CUSTOMATTRIBUTEDATA
#else
    member __.GetCustomAttributesData() = getMethInfo().GetCustomAttributesData()
#endif    
    member __.Tag = tag
    override x.ToString() = typ.Name + "." + x.Name
    override x.GetHashCode() = typ.GetHashCode() + tag
    override __.Equals(obj:obj) = 
        match obj with 
        | :? UnionCaseInfo as uci -> uci.DeclaringType = typ && uci.Tag = tag
        | _ -> false
    

[<AbstractClass; Sealed>]
type FSharpType = 

    static member IsTuple(typ:Type) =  
        Impl.checkNonNull "typ" typ
        Impl.isTupleType typ

    static member IsRecord(typ:Type,?bindingFlags) =  
        let bindingFlags = defaultArg bindingFlags BindingFlags.Public 

        Impl.checkNonNull "typ" typ
        Impl.isRecordType (typ,bindingFlags)

    static member IsUnion(typ:Type,?bindingFlags) =  
        let bindingFlags = defaultArg bindingFlags BindingFlags.Public 
        Impl.checkNonNull "typ" typ
        let typ = Impl.getTypeOfReprType (typ ,BindingFlags.Public ||| BindingFlags.NonPublic)
        Impl.isUnionType (typ,bindingFlags)

    static member IsFunction(typ:Type) =  
        Impl.checkNonNull "typ" typ
        let typ = Impl.getTypeOfReprType (typ ,BindingFlags.Public ||| BindingFlags.NonPublic)
        Impl.isFunctionType typ

    static member IsModule(typ:Type) =  
        Impl.checkNonNull "typ" typ
        Impl.isModuleType typ

    static member MakeFunctionType(domain:Type,range:Type) = 
        Impl.checkNonNull "domain" domain
        Impl.checkNonNull "range" range
        Impl.func.MakeGenericType [| domain; range |]

    static member MakeTupleType(types:Type[]) =  
        Impl.checkNonNull "types" types

        // No assembly passed therefore just get framework local version of Tuple
        let asm = typeof<System.Tuple>.Assembly
        if types |> Array.exists (function null -> true | _ -> false) then 
            invalidArg "types" (SR.GetString(SR.nullsNotAllowedInArray))
        Impl.mkTupleType false asm types

    static member MakeTupleType (asm:Assembly, types:Type[])  =
        Impl.checkNonNull "types" types
        if types |> Array.exists (function null -> true | _ -> false) then 
             invalidArg "types" (SR.GetString(SR.nullsNotAllowedInArray))
        Impl.mkTupleType false asm types

    static member MakeStructTupleType (asm:Assembly, types:Type[]) =
        Impl.checkNonNull "types" types
        if types |> Array.exists (function null -> true | _ -> false) then 
             invalidArg "types" (SR.GetString(SR.nullsNotAllowedInArray))
        Impl.mkTupleType true asm types

    static member GetTupleElements(tupleType:Type) =
        Impl.checkTupleType("tupleType",tupleType)
        Impl.getTupleTypeInfo tupleType

    static member GetFunctionElements(functionType:Type) =
        Impl.checkNonNull "functionType" functionType
        let functionType = Impl.getTypeOfReprType (functionType ,BindingFlags.Public ||| BindingFlags.NonPublic)
        Impl.getFunctionTypeInfo functionType

    static member GetRecordFields(recordType:Type,?bindingFlags) =
        let bindingFlags = defaultArg bindingFlags BindingFlags.Public
        Impl.checkRecordType("recordType",recordType,bindingFlags)
        Impl.fieldPropsOfRecordType(recordType,bindingFlags)

    static member GetUnionCases (unionType:Type,?bindingFlags) = 
        let bindingFlags = defaultArg bindingFlags BindingFlags.Public
        Impl.checkNonNull "unionType" unionType
        let unionType = Impl.getTypeOfReprType (unionType ,bindingFlags)
        Impl.checkUnionType(unionType,bindingFlags);
        Impl.getUnionTypeTagNameMap(unionType,bindingFlags) |> Array.mapi (fun i _ -> UnionCaseInfo(unionType,i))

    static member IsExceptionRepresentation(exceptionType:Type, ?bindingFlags) = 
        let bindingFlags = defaultArg bindingFlags BindingFlags.Public
        Impl.checkNonNull "exceptionType" exceptionType
        Impl.isExceptionRepr(exceptionType,bindingFlags)

    static member GetExceptionFields(exceptionType:Type, ?bindingFlags) = 
        let bindingFlags = defaultArg bindingFlags BindingFlags.Public
        Impl.checkNonNull "exceptionType" exceptionType
        Impl.checkExnType(exceptionType,bindingFlags)
        Impl.fieldPropsOfRecordType (exceptionType,bindingFlags) 

type DynamicFunction<'T1,'T2>() =
    inherit FSharpFunc<obj -> obj, obj>()
    override __.Invoke(impl: obj -> obj) : obj = 
        box<('T1 -> 'T2)> (fun inp -> unbox<'T2>(impl (box<'T1>(inp))))

[<AbstractClass; Sealed>]
type FSharpValue = 

    static member MakeRecord(recordType:Type,args,?bindingFlags) = 
        let bindingFlags = defaultArg bindingFlags BindingFlags.Public
        Impl.checkRecordType("recordType",recordType,bindingFlags)
        Impl.getRecordConstructor (recordType,bindingFlags) args

    static member GetRecordField(record:obj,info:PropertyInfo) =
        Impl.checkNonNull "info" info;
        Impl.checkNonNull "record" record;
        let reprty = record.GetType() 
        if not (Impl.isRecordType(reprty,BindingFlags.Public ||| BindingFlags.NonPublic)) then invalidArg "record" (SR.GetString(SR.objIsNotARecord));
        info.GetValue(record,null)

    static member GetRecordFields(record:obj,?bindingFlags) =
        let bindingFlags = defaultArg bindingFlags BindingFlags.Public
        Impl.checkNonNull "record" record
        let typ = record.GetType() 
        if not (Impl.isRecordType(typ,bindingFlags)) then invalidArg "record" (SR.GetString(SR.objIsNotARecord));
        Impl.getRecordReader (typ,bindingFlags) record

    static member PreComputeRecordFieldReader(info:PropertyInfo) = 
        Impl.checkNonNull "info" info
        (fun (obj:obj) -> info.GetValue(obj,null))

    static member PreComputeRecordReader(recordType:Type,?bindingFlags) : (obj -> obj[]) =  
        let bindingFlags = defaultArg bindingFlags BindingFlags.Public 
        Impl.checkRecordType("recordType",recordType,bindingFlags)
        Impl.getRecordReader (recordType,bindingFlags)

    static member PreComputeRecordConstructor(recordType:Type,?bindingFlags) = 
        let bindingFlags = defaultArg bindingFlags BindingFlags.Public 
        Impl.checkRecordType("recordType",recordType,bindingFlags)
        Impl.getRecordConstructor (recordType,bindingFlags)

    static member PreComputeRecordConstructorInfo(recordType:Type, ?bindingFlags) =
        let bindingFlags = defaultArg bindingFlags BindingFlags.Public 
        Impl.checkRecordType("recordType",recordType,bindingFlags)
        Impl.getRecordConstructorMethod(recordType,bindingFlags)

    static member MakeFunction(functionType:Type,implementation:(obj->obj)) = 
        Impl.checkNonNull "functionType" functionType
        if not (Impl.isFunctionType functionType) then invalidArg "functionType" (SR.GetString1(SR.notAFunctionType, functionType.FullName));
        Impl.checkNonNull "implementation" implementation
        let domain,range = Impl.getFunctionTypeInfo functionType
        let dynCloMakerTy = typedefof<DynamicFunction<obj,obj>>
        let saverTy = dynCloMakerTy.MakeGenericType [| domain; range |]
        let o = Activator.CreateInstance(saverTy)
        let (f : (obj -> obj) -> obj) = downcast o
        f implementation

    static member MakeTuple(tupleElements: obj[], tupleType:Type) =
        Impl.checkNonNull "tupleElements" tupleElements
        Impl.checkTupleType("tupleType",tupleType) 
        Impl.getTupleConstructor tupleType tupleElements
    
    static member GetTupleFields(tuple:obj) = // argument name(s) used in error message
        Impl.checkNonNull "tuple" tuple
        let typ = tuple.GetType() 
        if not (Impl.isTupleType typ ) then invalidArg "tuple" (SR.GetString1(SR.notATupleType, tuple.GetType().FullName));
        Impl.getTupleReader typ tuple

    static member GetTupleField(tuple:obj,index:int) = // argument name(s) used in error message
        Impl.checkNonNull "tuple" tuple
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
        Impl.checkNonNull "unionType" unionType
        let unionType = Impl.getTypeOfReprType (unionType ,bindingFlags)
        //System.Console.WriteLine("typ3 = {0}",box unionType)
        Impl.checkUnionType(unionType,bindingFlags)
        let tag = Impl.getUnionTagReader (unionType,bindingFlags) obj
        let flds = Impl.getUnionCaseRecordReader (unionType,tag,bindingFlags) obj 
        UnionCaseInfo(unionType,tag), flds
        
    static member PreComputeUnionTagReader(unionType: Type,?bindingFlags) : (obj -> int) = 
        let bindingFlags = defaultArg bindingFlags BindingFlags.Public 
        Impl.checkNonNull "unionType" unionType
        let unionType = Impl.getTypeOfReprType (unionType ,bindingFlags)
        Impl.checkUnionType(unionType,bindingFlags)
        Impl.getUnionTagReader (unionType ,bindingFlags)


    static member PreComputeUnionTagMemberInfo(unionType: Type,?bindingFlags) = 
        let bindingFlags = defaultArg bindingFlags BindingFlags.Public 
        Impl.checkNonNull "unionType" unionType;
        let unionType = Impl.getTypeOfReprType (unionType ,bindingFlags)
        Impl.checkUnionType(unionType,bindingFlags)
        Impl.getUnionTagMemberInfo(unionType ,bindingFlags)

    static member PreComputeUnionReader(unionCase: UnionCaseInfo,?bindingFlags) : (obj -> obj[])  = 
        let bindingFlags = defaultArg bindingFlags BindingFlags.Public 
        Impl.checkNonNull "unionCase" unionCase
        let typ = unionCase.DeclaringType 
        Impl.getUnionCaseRecordReader (typ,unionCase.Tag,bindingFlags)

    static member GetExceptionFields(exn:obj, ?bindingFlags) = 
        let bindingFlags = defaultArg bindingFlags BindingFlags.Public 
        Impl.checkNonNull "exn" exn;
        let typ = exn.GetType() 
        Impl.checkExnType(typ,bindingFlags)
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

