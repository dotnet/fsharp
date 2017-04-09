// Copyright 2011-2015, Tomas Petricek (http://tomasp.net), Gustavo Guerra (http://functionalflow.co.uk), and other contributors
// Licensed under the Apache License, Version 2.0, see LICENSE.md in this project
//
// An implementation of reflection objects over on-disk assemblies, sufficient to give
// System.Type, System.MethodInfo, System.ConstructorInfo etc. objects
// that can be referred to in quotations and used as backing information for cross-
// targeting F# type providers.
//
// The on-disk assemblies are read by AssemblyReader.
//
// Background
// ----------
//
// Provided type/member definitions need to refer to non-provided definitions like "System.Object" and "System.String".
//
// For cross-targeting F# type providers, these can be references to assemblies that can't easily be loaded by .NET
// reflection. For this reason, an implementation of the .NET reflection objects is needed. At minimum this
// implementation must support the operations used by the F# compiler to interrogate the reflection objects.
//
//     For a System.Assembly, the information must be sufficient to allow the Assembly --> ILScopeRef conversion 
//     in ExtensionTyping.fs of the F# compiler. This requires:
//         Assembly.GetName()
//
//     For a System.Type representing a reference to a named type definition, the information must be sufficient 
//     to allow the Type --> ILTypeRef conversion in the F# compiler. This requires:
//         typ.DeclaringType
//         typ.Name
//         typ.Namespace
//
//     For a System.Type representing a type expression, the information must be sufficient to allow the Type --> ILType.Var conversion in the F# compiler. 
//        typeof<System.Void>.Equals(typ)
//        typ.IsGenericParameter 
//           typ.GenericParameterPosition 
//        typ.IsArray
//           typ.GetElementType()
//           typ.GetArrayRank()
//        typ.IsByRef
//           typ.GetElementType()
//        typ.IsPointer
//           typ.GetElementType()
//        typ.IsGenericType
//           typ.GetGenericArguments()
//           typ.GetGenericTypeDefinition()
//
//     For a System.MethodBase --> ILType.ILMethodRef conversion:
//
//       :?> MethodInfo as minfo
//
//          minfo.IsGenericMethod || minfo.DeclaringType.IsGenericType
//             minfo.DeclaringType.GetGenericTypeDefinition
//             minfo.DeclaringType.GetMethods().MetadataToken
//             minfo.MetadataToken
//          minfo.IsGenericMethod 
//             minfo.GetGenericArguments().Length
//          minfo.ReturnType
//          minfo.GetParameters | .ParameterType
//          minfo.Name
//
//       :?> ConstructorInfo as cinfo
//
//          cinfo.DeclaringType.IsGenericType
//             cinfo.DeclaringType.GetGenericTypeDefinition
//             cinfo.DeclaringType.GetConstructors() GetParameters | .ParameterType
//

module internal Microsoft.FSharp.Compiler.AssemblyReaderReflection

#nowarn "40"

open System
open System.IO
open System.Collections.Generic
open System.Reflection
open Microsoft.FSharp.Compiler.Tast
open Microsoft.FSharp.Compiler.TcGlobals


[<AutoOpen>]
module Utils = 
    let nullToOption x = match x with null -> None | _ -> Some x
    let optionToNull x = match x with None -> null | Some x -> x

    let notRequired msg = 
       failwith (sprintf "SHOULD NOT BE REQUIRED! %s. Stack trace:\n%s" msg (System.Diagnostics.StackTrace().ToString()))

    // A table tracking how wrapped type definition objects are translated to cloned objects.
    // Unique wrapped type definition objects must be translated to unique wrapper objects, based 
    // on object identity.
    type TxTable<'T2>() = 
        let tab = Dictionary<Stamp, 'T2>()
        member __.Get inp f = 
            if tab.ContainsKey inp then 
                tab.[inp] 
            else 
                let res = f() 
                tab.[inp] <- res
                res

        member __.ContainsKey inp = tab.ContainsKey inp 
        member __.Values = tab.Values

    let lengthsEqAndForall2 (arr1: 'T1[]) (arr2: 'T2[]) f = 
        (arr1.Length = arr2.Length) &&
        (arr1,arr2) ||> Array.forall2 f

    // Instantiate a type's generic parameters
    let rec instType inst (ty:Type) = 
        if ty.IsGenericType then 
            let args = Array.map (instType inst) (ty.GetGenericArguments())
            ty.GetGenericTypeDefinition().MakeGenericType(args)
        elif ty.HasElementType then 
            let ety = instType inst (ty.GetElementType()) 
            if ty.IsArray then 
                let rank = ty.GetArrayRank()
                if rank = 1 then ety.MakeArrayType()
                else ety.MakeArrayType(rank)
            elif ty.IsPointer then ety.MakePointerType()
            elif ty.IsByRef then ety.MakeByRefType()
            else ty
        elif ty.IsGenericParameter then 
            let pos = ty.GenericParameterPosition
            let (inst1: Type[], inst2: Type[]) = inst 
            if pos < inst1.Length then inst1.[pos]
            elif pos < inst1.Length + inst2.Length then inst2.[pos - inst1.Length]
            else ty
        else ty

    let instParameterInfo inst (inp: ParameterInfo) = 
        { new ParameterInfo() with 
            override __.Name = inp.Name 
            override __.ParameterType = inp.ParameterType |> instType inst
            override __.Attributes = inp.Attributes
            override __.RawDefaultValue = inp.RawDefaultValue
            override __.GetCustomAttributesData() = inp.GetCustomAttributesData()
            override x.ToString() = inp.ToString() + "@inst" }

    let rec eqType (ty1:Type) (ty2:Type) = 
        if ty1.IsGenericType then ty2.IsGenericType && lengthsEqAndForall2 (ty1.GetGenericArguments()) (ty2.GetGenericArguments()) eqType
        elif ty1.IsArray then ty2.IsArray && ty1.GetArrayRank() = ty2.GetArrayRank() && eqType (ty1.GetElementType()) (ty2.GetElementType()) 
        elif ty1.IsPointer then ty2.IsPointer && eqType (ty1.GetElementType()) (ty2.GetElementType()) 
        elif ty1.IsByRef then ty2.IsByRef && eqType (ty1.GetElementType()) (ty2.GetElementType()) 
        else ty1.Equals(box ty2)

    //let hashILParameterTypes (ps: ILParameters) = 
    //   // This hash code doesn't need to be very good as hashing by name is sufficient to give decent hash granularity
    //   ps.Length 

    //let eqILScopeRef (_sco1: ILScopeRef) (_sco2: ILScopeRef) = 
    //    true // TODO (though omitting this is not a problem in practice since type equivalence by name is sufficient to bind methods)

    //let eqAssemblyAndILScopeRef (_ass1: Assembly) (_sco2: ILScopeRef) = 
    //    true // TODO (though omitting this is not a problem in practice since type equivalence by name is sufficient to bind methods)


    //let rec eqILTypeRef (ty1: ILTypeRef) (ty2: ILTypeRef) = 
    //    ty1.Name = ty2.Name && eqILTypeRefScope ty1.Scope ty2.Scope

    //and eqILTypeRefScope (ty1: ILTypeRefScope) (ty2: ILTypeRefScope) = 
    //    match ty1, ty2 with 
    //    | ILTypeRefScope.Top scoref1, ILTypeRefScope.Top scoref2 -> eqILScopeRef scoref1 scoref2
    //    | ILTypeRefScope.Nested tref1, ILTypeRefScope.Nested tref2 -> eqILTypeRef tref1 tref2
    //    | _ -> false

    //and eqILTypes (tys1: ILType[]) (tys2: ILType[]) = 
    //    lengthsEqAndForall2 tys1 tys2 eqILType

    //and eqILType (ty1: ILType) (ty2: ILType) = 
    //    match ty1, ty2 with 
    //    | (ILType.Value(tspec1) | ILType.Boxed(tspec1)), (ILType.Value(tspec2) | ILType.Boxed(tspec2))->
    //        eqILTypeRef tspec1.TypeRef tspec2.TypeRef && eqILTypes tspec1.GenericArgs tspec2.GenericArgs
    //    | ILType.Array(rank1, arg1), ILType.Array(rank2, arg2) ->
    //        rank1 = rank2 && eqILType arg1 arg2
    //    | ILType.Ptr(arg1), ILType.Ptr(arg2) ->
    //        eqILType arg1 arg2
    //    | ILType.Byref(arg1), ILType.Byref(arg2) ->
    //        eqILType arg1 arg2
    //    | ILType.Var(arg1), ILType.Var(arg2) ->
    //        arg1 = arg2
    //    | _ -> false

    //let rec eqTypeAndILTypeRef (ty1: Type) (ty2: ILTypeRef) = 
    //    ty1.Name = ty2.Name && 
    //    ty1.Namespace = (uoptionToNull ty2.Namespace) &&
    //    match ty2.Scope with 
    //    | ILTypeRefScope.Top scoref2 -> eqAssemblyAndILScopeRef ty1.Assembly scoref2
    //    | ILTypeRefScope.Nested tref2 -> ty1.IsNested && eqTypeAndILTypeRef ty1.DeclaringType tref2

    //let rec eqTypesAndILTypes (tys1: Type[]) (tys2: ILType[]) = 
    //    eqTypesAndILTypesWithInst [| |] tys1 tys2 

    //and eqTypesAndILTypesWithInst inst2 (tys1: Type[]) (tys2: ILType[]) = 
    //    lengthsEqAndForall2 tys1 tys2 (eqTypeAndILTypeWithInst inst2)

    //and eqTypeAndILTypeWithInst inst2 (ty1: Type) (ty2: ILType) = 
    //    match ty2 with 
    //    | (ILType.Value(tspec2) | ILType.Boxed(tspec2))->
    //        if tspec2.GenericArgs.Length > 0 then 
    //            ty1.IsGenericType && eqTypeAndILTypeRef (ty1.GetGenericTypeDefinition()) tspec2.TypeRef && eqTypesAndILTypesWithInst inst2 (ty1.GetGenericArguments()) tspec2.GenericArgs
    //        else 
    //            not ty1.IsGenericType && eqTypeAndILTypeRef ty1 tspec2.TypeRef
    //    | ILType.Array(rank2, arg2) ->
    //        ty1.IsArray && ty1.GetArrayRank() = rank2.Rank && eqTypeAndILTypeWithInst inst2 (ty1.GetElementType()) arg2
    //    | ILType.Ptr(arg2) -> 
    //        ty1.IsPointer && eqTypeAndILTypeWithInst inst2 (ty1.GetElementType()) arg2
    //    | ILType.Byref(arg2) ->
    //        ty1.IsByRef && eqTypeAndILTypeWithInst inst2 (ty1.GetElementType()) arg2
    //    | ILType.Var(arg2) ->
    //        if int arg2 < inst2.Length then 
    //             eqType ty1 inst2.[int arg2]  
    //        else
    //             ty1.IsGenericParameter && ty1.GenericParameterPosition = int arg2
                
    //    | _ -> false

    //let eqParametersAndILParameterTypesWithInst inst2 (ps1: ParameterInfo[])  (ps2: ILParameters) = 
    //    lengthsEqAndForall2 ps1 ps2 (fun p1 p2 -> eqTypeAndILTypeWithInst inst2 p1.ParameterType p2.ParameterType)

    //let adjustTypeAttributes isNested attributes = 
    //    let visibilityAttributes = 
    //        match attributes &&& TypeAttributes.VisibilityMask with 
    //        | TypeAttributes.Public when isNested -> TypeAttributes.NestedPublic
    //        | TypeAttributes.NotPublic when isNested -> TypeAttributes.NestedAssembly
    //        | TypeAttributes.NestedPublic when not isNested -> TypeAttributes.Public
    //        | TypeAttributes.NestedAssembly 
    //        | TypeAttributes.NestedPrivate 
    //        | TypeAttributes.NestedFamORAssem
    //        | TypeAttributes.NestedFamily
    //        | TypeAttributes.NestedFamANDAssem when not isNested -> TypeAttributes.NotPublic
    //        | a -> a
    //    (attributes &&& ~~~TypeAttributes.VisibilityMask) ||| visibilityAttributes



    //let convFieldInit x = 
    //    match x with 
    //    | ILFieldInit.String s       -> box s
    //    | ILFieldInit.Bool bool      -> box bool   
    //    | ILFieldInit.Char u16       -> box (char (int u16))  
    //    | ILFieldInit.Int8 i8        -> box i8     
    //    | ILFieldInit.Int16 i16      -> box i16    
    //    | ILFieldInit.Int32 i32      -> box i32    
    //    | ILFieldInit.Int64 i64      -> box i64    
    //    | ILFieldInit.UInt8 u8       -> box u8     
    //    | ILFieldInit.UInt16 u16     -> box u16    
    //    | ILFieldInit.UInt32 u32     -> box u32    
    //    | ILFieldInit.UInt64 u64     -> box u64    
    //    | ILFieldInit.Single ieee32 -> box ieee32 
    //    | ILFieldInit.Double ieee64 -> box ieee64 
    //    | ILFieldInit.Null            -> (null :> Object)

/// Represents the type constructor in a provided symbol type.
[<RequireQualifiedAccess>]
type ContextTypeSymbolKind = 
    | SDArray 
    | Array of int 
    | Pointer 
    | ByRef 
    | Generic of ContextTypeDefinition


/// Represents an array or other symbolic type involving a provided type as the argument.
/// See the type provider spec for the methods that must be implemented.
/// Note that the type provider specification does not require us to implement pointer-equality for provided types.
and ContextTypeSymbol(kind: ContextTypeSymbolKind, args: Type[]) =
    inherit Type()

    let notRequired msg = 
        System.Diagnostics.Debugger.Break()
        failwith ("not required: " + msg)

    override __.FullName =   
        match kind,args with 
        | ContextTypeSymbolKind.SDArray,[| arg |] -> arg.FullName + "[]" 
        | ContextTypeSymbolKind.Array _,[| arg |] -> arg.FullName + "[*]" 
        | ContextTypeSymbolKind.Pointer,[| arg |] -> arg.FullName + "*" 
        | ContextTypeSymbolKind.ByRef,[| arg |] -> arg.FullName + "&"
        | ContextTypeSymbolKind.Generic gtd, args -> gtd.FullName + "[" + (args |> Array.map (fun arg -> arg.FullName) |> String.concat ",") + "]"
        | _ -> failwith "unreachable"

    override __.DeclaringType =                                                                 
        match kind,args with 
        | ContextTypeSymbolKind.SDArray,[| arg |] 
        | ContextTypeSymbolKind.Array _,[| arg |] 
        | ContextTypeSymbolKind.Pointer,[| arg |] 
        | ContextTypeSymbolKind.ByRef,[| arg |] -> arg.DeclaringType
        | ContextTypeSymbolKind.Generic gtd,_ -> gtd.DeclaringType
        | _ -> failwith "unreachable"

    override __.IsAssignableFrom(otherTy) = 
        match kind with
        | ContextTypeSymbolKind.Generic gtd ->
            if otherTy.IsGenericType then
                let otherGtd = otherTy.GetGenericTypeDefinition()
                let otherArgs = otherTy.GetGenericArguments()
                let yes = gtd.Equals(otherGtd) && Seq.forall2 eqType args otherArgs
                yes
            else
                base.IsAssignableFrom(otherTy)
        | _ -> base.IsAssignableFrom(otherTy)

    override this.IsSubclassOf(otherTy) = 
        base.IsSubclassOf(otherTy) ||
        match kind with
        | ContextTypeSymbolKind.Generic gtd -> 
            let md : TyconRef = gtd.Metadata
            md.IsFSharpDelegateTycon && (otherTy = typeof<Delegate>) // F# quotations implementation
        | _ -> false

    override __.Name =
        match kind,args with 
        | ContextTypeSymbolKind.SDArray,[| arg |] -> arg.Name + "[]" 
        | ContextTypeSymbolKind.Array _,[| arg |] -> arg.Name + "[*]" 
        | ContextTypeSymbolKind.Pointer,[| arg |] -> arg.Name + "*" 
        | ContextTypeSymbolKind.ByRef,[| arg |] -> arg.Name + "&"
        | ContextTypeSymbolKind.Generic gtd, _args -> gtd.Name 
        | _ -> failwith "unreachable"

    override __.BaseType =
        match kind with 
        | ContextTypeSymbolKind.SDArray -> typeof<System.Array>
        | ContextTypeSymbolKind.Array _ -> typeof<System.Array>
        | ContextTypeSymbolKind.Pointer -> typeof<System.ValueType>
        | ContextTypeSymbolKind.ByRef -> typeof<System.ValueType>
        | ContextTypeSymbolKind.Generic gtd  -> instType (args, [| |]) gtd.BaseType
        
    override this.Assembly = 
        match kind, args with 
        | ContextTypeSymbolKind.SDArray,[| arg |] 
        | ContextTypeSymbolKind.Array _,[| arg |] 
        | ContextTypeSymbolKind.Pointer,[| arg |] 
        | ContextTypeSymbolKind.ByRef,[| arg |] -> arg.Assembly
        | ContextTypeSymbolKind.Generic gtd, _ -> gtd.Assembly
        | _ -> notRequired "Assembly" this.Name

    override this.Namespace = 
        match kind, args with 
        | ContextTypeSymbolKind.SDArray,[| arg |] 
        | ContextTypeSymbolKind.Array _,[| arg |] 
        | ContextTypeSymbolKind.Pointer,[| arg |] 
        | ContextTypeSymbolKind.ByRef,[| arg |] -> arg.Namespace
        | ContextTypeSymbolKind.Generic gtd, _ -> gtd.Namespace 
        | _ -> failwith "unreachable"

    override __.GetArrayRank() = (match kind with ContextTypeSymbolKind.Array n -> n | ContextTypeSymbolKind.SDArray -> 1 | _ -> invalidOp "non-array type")
    override __.IsValueTypeImpl() = (match kind with ContextTypeSymbolKind.Generic gtd -> gtd.IsValueType | _ -> false)
    override __.IsArrayImpl() = (match kind with ContextTypeSymbolKind.Array _ | ContextTypeSymbolKind.SDArray -> true | _ -> false)
    override __.IsByRefImpl() = (match kind with ContextTypeSymbolKind.ByRef _ -> true | _ -> false)
    override __.IsPointerImpl() = (match kind with ContextTypeSymbolKind.Pointer _ -> true | _ -> false)
    override __.IsPrimitiveImpl() = false
    override __.IsGenericType = (match kind with ContextTypeSymbolKind.Generic _ -> true | _ -> false)
    override __.GetGenericArguments() = (match kind with ContextTypeSymbolKind.Generic _ -> args | _ -> [| |])
    override __.GetGenericTypeDefinition() = (match kind with ContextTypeSymbolKind.Generic e -> (e :> Type) | _ -> invalidOp "non-generic type")
    override __.IsCOMObjectImpl() = false
    override __.HasElementTypeImpl() = (match kind with ContextTypeSymbolKind.Generic _ -> false | _ -> true)
    override __.GetElementType() = (match kind,args with (ContextTypeSymbolKind.Array _  | ContextTypeSymbolKind.SDArray | ContextTypeSymbolKind.ByRef | ContextTypeSymbolKind.Pointer),[| e |] -> e | _ -> invalidOp (sprintf "%A, %A: not an array, pointer or byref type" kind args))

    override this.Module : Module = notRequired "Module" this.Name

    override this.GetHashCode()                                                                    = 
        match kind,args with 
        | ContextTypeSymbolKind.SDArray,[| arg |] -> 10 + hash arg
        | ContextTypeSymbolKind.Array _,[| arg |] -> 163 + hash arg
        | ContextTypeSymbolKind.Pointer,[| arg |] -> 283 + hash arg
        | ContextTypeSymbolKind.ByRef,[| arg |] -> 43904 + hash arg
        | ContextTypeSymbolKind.Generic gtd,_ -> 9797 + hash gtd + Array.sumBy hash args
        | _ -> failwith "unreachable"
    
    override this.Equals(other: obj) =
        match other with
        | :? ContextTypeSymbol as otherTy -> (kind, args) = (otherTy.Kind, otherTy.Args)
        | _ -> false

    member this.Kind = kind
    member this.Args = args
    
    override this.GetConstructors _bindingAttr                                                      = notRequired "GetConstructors" this.Name
    override this.GetMethodImpl(_ (* name *), _bindingAttr, _binderBinder, _callConvention, _ (* types *), _modifiers) = notRequired "ContextTypeSymbol: GetMethodImpl" this.Name
        //match kind with
        //| ContextTypeSymbolKind.Generic gtd -> 

        //    let md = 
        //        match types with 
        //        | null -> 
        //            match gtd.Metadata.Methods.FindByName(name) with 
        //            | [| md |] -> md
        //            | [| |] -> failwith (sprintf "method %s not found" name)
        //            | _ -> failwith (sprintf "multiple methods called '%s' found" name)
        //        | _ -> 
        //            match gtd.Metadata.Methods.FindByNameAndArity(name, types.Length) with
        //            | [| |] ->  failwith (sprintf "method %s not found with arity %d" name types.Length)
        //            | mds -> 
        //                match mds |> Array.filter (fun md -> eqTypesAndILTypesWithInst args types md.ParameterTypes) with 
        //                | [| |] -> 
        //                    let md1 = mds.[0]
        //                    ignore md1
        //                    failwith (sprintf "no method %s with arity %d found with right types. Comparisons:" name types.Length
        //                              + ((types, md1.ParameterTypes) ||> Array.map2 (fun a pt -> eqTypeAndILTypeWithInst args a pt |> sprintf "%A") |> String.concat "\n"))
        //                | [| md |] -> md
        //                | _ -> failwith (sprintf "multiple methods %s with arity %d found with right types" name types.Length)

        //    gtd.MakeMethodInfo (this, md)

        //| _ -> notRequired "ContextTypeSymbol: GetMethodImpl" this.Name

    override this.GetConstructorImpl(_bindingAttr, _binderBinder, _callConvention, _ (* types *), _modifiers) = notRequired "ContextTypeSymbol: GetConstructorImpl" this.Name
        //match kind with
        //| ContextTypeSymbolKind.Generic gtd -> 
        //    let name = ".ctor"
        //    let md = 
        //        match types with 
        //        | null -> 
        //            match gtd.Metadata.Methods.FindByName(name) with 
        //            | [| md |] -> md
        //            | [| |] -> failwith (sprintf "method %s not found" name)
        //            | _ -> failwith (sprintf "multiple methods called '%s' found" name)
        //        | _ -> 
        //            gtd.Metadata.Methods.FindByNameAndArity(name, types.Length)
        //            |> Array.find (fun md -> eqTypesAndILTypesWithInst types args md.ParameterTypes)
        //    gtd.MakeConstructorInfo (this, md)

        //| _ -> notRequired "ContextTypeSymbol: GetConstructorImpl" this.Name

    override this.AssemblyQualifiedName                                                            = "[" + this.Assembly.FullName + "]" + this.FullName

    override this.GetMembers _bindingAttr                                                           = notRequired "GetMembers" this.Name
    override this.GetMethods _bindingAttr                                                           = notRequired "GetMethods" this.Name
    override this.GetField(_name, _bindingAttr)                                                     = notRequired "GetField" this.Name
    override this.GetFields _bindingAttr                                                            = notRequired "GetFields" this.Name
    override this.GetInterface(_name, _ignoreCase)                                                  = notRequired "GetInterface" this.Name
    override this.GetInterfaces()                                                                   = notRequired "GetInterfaces" this.Name
    override this.GetEvent(_name, _bindingAttr)                                                     = notRequired "GetEvent" this.Name
    override this.GetEvents _bindingAttr                                                            = notRequired "GetEvents" this.Name
    override this.GetProperties _bindingAttr                                                        = notRequired "GetProperties" this.Name
    override this.GetPropertyImpl(_name, _bindingAttr, _binder, _returnType, _types, _modifiers)    = notRequired "GetPropertyImpl" this.Name
    override this.GetNestedTypes _bindingAttr                                                       = notRequired "GetNestedTypes" this.Name
    override this.GetNestedType(_name, _bindingAttr)                                                = notRequired "GetNestedType" this.Name
    override this.GetAttributeFlagsImpl()                                                           = notRequired "GetAttributeFlagsImpl" this.Name
    
    override this.UnderlyingSystemType = (this :> Type)

    override this.GetCustomAttributesData()                                                        =  ([| |] :> IList<_>)
    override this.MemberType                                                                       = notRequired "MemberType" this.Name
    override this.GetMember(_name,_mt,_bindingAttr)                                                = notRequired "GetMember" this.Name
    override this.GUID                                                                             = notRequired "GUID" this.Name
    override this.InvokeMember(_name, _invokeAttr, _binder, _target, _args, _modifiers, _culture, _namedParameters) = notRequired "InvokeMember" this.Name
    override this.GetCustomAttributes(_inherit)                                                    = [| |]
    override this.GetCustomAttributes(_attributeType, _inherit)                                    = [| |]
    override this.IsDefined(_attributeType, _inherit)                                              = false
    override this.MakeArrayType() = ContextTypeSymbol(ContextTypeSymbolKind.SDArray, [| this |]) :> Type
    override this.MakeArrayType arg = ContextTypeSymbol(ContextTypeSymbolKind.Array arg, [| this |]) :> Type
    override this.MakePointerType() = ContextTypeSymbol(ContextTypeSymbolKind.Pointer, [| this |]) :> Type
    override this.MakeByRefType() = ContextTypeSymbol(ContextTypeSymbolKind.ByRef, [| this |]) :> Type

    override this.ToString() = this.FullName

and ContextMethodSymbol(gmd: MethodInfo, gargs: Type[]) =
    inherit MethodInfo() 

    override __.Attributes        = gmd.Attributes
    override __.Name              = gmd.Name
    override __.DeclaringType     = gmd.DeclaringType
    override __.MemberType        = gmd.MemberType

    override __.GetParameters()   = gmd.GetParameters() |> Array.map (instParameterInfo (gmd.DeclaringType.GetGenericArguments(), gargs))
    override __.CallingConvention = gmd.CallingConvention
    override __.ReturnType        = gmd.ReturnType |> instType (gmd.DeclaringType.GetGenericArguments(), gargs)
    override __.IsGenericMethod   = true
    override __.GetGenericArguments() = gargs
    override __.MetadataToken = gmd.MetadataToken

    override __.GetCustomAttributesData() = gmd.GetCustomAttributesData()

    override __.GetHashCode() = gmd.GetHashCode()
    override this.Equals(that:obj) = 
        match that with 
        | :? MethodInfo as thatMI -> thatMI.IsGenericMethod && gmd.Equals(thatMI.GetGenericMethodDefinition()) && lengthsEqAndForall2 (gmd.GetGenericArguments()) (thatMI.GetGenericArguments()) (=)
        | _ -> false

    override __.MethodHandle = notRequired "MethodHandle"
    override __.ReturnParameter   = notRequired "ReturnParameter" 
    override __.IsDefined(_attributeType, _inherited)                   = notRequired "IsDefined"
    override __.ReturnTypeCustomAttributes                            = notRequired "ReturnTypeCustomAttributes"
    override __.GetBaseDefinition()                                   = notRequired "GetBaseDefinition"
    override __.GetMethodImplementationFlags()                        = notRequired "GetMethodImplementationFlags"
    override __.Invoke(_obj, _invokeAttr, _binder, _parameters, _culture)  = notRequired "Invoke"
    override __.ReflectedType                                         = notRequired "ReflectedType"
    override __.GetCustomAttributes(_inherited)                        = notRequired "GetCustomAttributes"
    override __.GetCustomAttributes(_attributeType, _inherited)         = notRequired "GetCustomAttributes"

    override __.ToString() = gmd.ToString() + "@inst"
    

/// Clones namespaces, type providers, types and members provided by tp, renaming namespace nsp1 into namespace nsp2.

/// Makes a type definition read from a binary available as a System.Type. Not all methods are implemented.
and ContextTypeDefinition(ilGlobals: TcGlobals, asm: ContextAssembly, declTyOpt: Type option, inp: TyconRef) as this = 
    inherit Type()

    // Note: For F# type providers we never need to view the custom attributes
    let rec TxCustomAttributesArg (AttribExpr(_,v)) =
        match v with
        | Expr.Const (cnst, _, ttype) -> 
        //TODO: This can probably be removed completly.
            CustomAttributeTypedArgument(TxTType ttype, TxConst cnst)
        | _ -> failwithf "Missing case for CustomAttributesArg %A" v

    and TxCustomAttributesDatum (Attrib(_, _, exprs,_,_,_,_)) = 
         { new CustomAttributeData () with
            member __.Constructor =  
                inp.MembersOfFSharpTyconSorted 
                |> List.find (fun x -> x.IsConstructor)
                |> TxConstructorDef this
            member __.ConstructorArguments = [| for exp in exprs -> TxCustomAttributesArg exp |] :> IList<_>
            // Note, named arguments of custom attributes are not required by F# compiler on binding context elements.
            member __.NamedArguments = [| |] :> IList<_> 
         }

    and TxCustomAttributesData (attribs:Attribs) = //notRequired "custom attributes are not available for context assemblies"
         [| for a in attribs do 
              yield TxCustomAttributesDatum a |]
         :> IList<CustomAttributeData> 

    /// Makes a parameter definition read from a binary available as a ParameterInfo. Not all methods are implemented.
    //let rec TxILParameter gps (inp : TyconRef) = 
    //    { new ParameterInfo() with 

    //        override __.Name = inp.MembersOfFSharpTyconByName.["Foo"].[0].MemberInfo.Value.
    //        override __.ParameterType = inp.ParameterType |> TxILType gps
    //        override __.RawDefaultValue = (match inp.Default with None -> null | Some v -> convFieldInit v)
    //        override __.Attributes = inp.Attributes
    //        override __.GetCustomAttributesData() = inp.CustomAttrs  |> TxCustomAttributesData

    //        override x.ToString() = sprintf "ctxt parameter %s" x.Name }
 
    /// Makes a method definition read from a binary available as a ConstructorInfo. Not all methods are implemented.
    and TxConstructorDef (declTy: Type) (inp: ValRef) = 
        //let gps = if declTy.IsGenericType then declTy.GetGenericArguments() else [| |]
        { new ConstructorInfo() with

            override __.Name = ".ctor"
            override __.Attributes = notRequired "Attributes" //TODO: Constructor attributes
            override __.MemberType        = MemberTypes.Constructor
            override __.DeclaringType = declTy

            override __.GetParameters() = notRequired ".ctor parameters" //TODO: Constructor parameters
            override __.GetCustomAttributesData() = inp.Attribs |> TxCustomAttributesData

            override __.GetHashCode() = 0
            override __.Equals(that:obj) = 
                match that with 
                | :? ConstructorInfo as that -> 
                    eqType declTy that.DeclaringType //&&
                    //TODO: Equality on constructor parameters
                    //eqParametersAndILParameterTypesWithInst gps (that.GetParameters()) inp.Parameters 
                | _ -> false

            override __.IsDefined(attributeType, inherited) = notRequired "IsDefined" 
            override __.Invoke(invokeAttr, binder, parameters, culture) = notRequired "Invoke"
            override __.Invoke(obj, invokeAttr, binder, parameters, culture) = notRequired "Invoke"
            override __.ReflectedType = notRequired "ReflectedType"
            override __.GetMethodImplementationFlags() = notRequired "GetMethodImplementationFlags"
            override __.MethodHandle = notRequired "MethodHandle"
            override __.GetCustomAttributes(inherited) = notRequired "GetCustomAttributes"
            override __.GetCustomAttributes(attributeType, inherited) = notRequired "GetCustomAttributes"

            override __.ToString() = sprintf "ctxt constructor(...) in type %s" declTy.FullName }

    /// Makes a method definition read from a binary available as a MethodInfo. Not all methods are implemented.
    and TxMethodDef (declTy: Type) (inp: ValRef) =
        //TODO: Handle generic parameters
        let gps = if declTy.IsGenericType then declTy.GetGenericArguments() else [| |]
        let rec gps2 = 
            match inp.Type with
            | TType_forall (pars,_) -> 
                pars |> List.mapi (fun i gp -> TxGenericParam (fun () -> gps, gps2) (i + gps.Length) gp) |> List.toArray
            | _ -> [||]
        { new MethodInfo() with 

            override __.Name              = inp.CompiledName  
            override __.DeclaringType     = declTy
            override __.MemberType        = MemberTypes.Method
            override __.Attributes        = notRequired "TxMethodDef Attributes" //TODO: TxMethodDef method attributes
            override __.GetParameters()   = notRequired "TxMethodDef parameters" //TODO: TxMethodDef parameters
            override __.CallingConvention = CallingConventions.HasThis ||| CallingConventions.Standard // Provided types report this by default
            override __.ReturnType        = inp.Type |> TxTType
            override __.GetCustomAttributesData() = inp.Attribs |> TxCustomAttributesData
            override __.GetGenericArguments() = gps2
            override __.IsGenericMethod = (gps2.Length <> 0)
            override __.IsGenericMethodDefinition = __.IsGenericMethod

            override __.GetHashCode() = hash inp.Stamp //TODO: Implement correct hashing  + hashILParameterTypes inp.Parameters
            override this.Equals(that:obj) = 
                match that with 
                | :? MethodInfo as thatMI -> 
                    inp.CompiledName = thatMI.Name 
                    (*
                    TODO: Need to implement equality correctly for method defs
                    &&
                    eqType this.DeclaringType thatMI.DeclaringType &&
                    eqParametersAndILParameterTypesWithInst gps (thatMI.GetParameters()) inp.Parameters *)
                | _ -> false

            override this.MakeGenericMethod(args) = ContextMethodSymbol(this, args) :> MethodInfo

            override __.MetadataToken = int inp.Stamp //TODO: Fix me .MetadataToken

            // unused
            override __.MethodHandle = notRequired "MethodHandle"
            override __.ReturnParameter = notRequired "ReturnParameter" 
            override __.IsDefined(attributeType, inherited) = notRequired "IsDefined"
            override __.ReturnTypeCustomAttributes = notRequired "ReturnTypeCustomAttributes"
            override __.GetBaseDefinition() = notRequired "GetBaseDefinition"
            override __.GetMethodImplementationFlags() = notRequired "GetMethodImplementationFlags"
            override __.Invoke(obj, invokeAttr, binder, parameters, culture)  = notRequired "Invoke"
            override __.ReflectedType = notRequired "ReflectedType"
            override __.GetCustomAttributes(inherited) = notRequired "GetCustomAttributes"
            override __.GetCustomAttributes(attributeType, inherited) = notRequired "GetCustomAttributes" 

            override __.ToString() = sprintf "ctxt method %s(...) in type %s" inp.CompiledName declTy.FullName  }

    /// Makes a property definition read from a binary available as a PropertyInfo. Not all methods are implemented.
    and TxPropertyDefinition declTy _ (* gps *) (tycon:TyconRef) (inp: ValRef) = 
        { new PropertyInfo() with 

            override __.Name = inp.PropertyName
            override __.Attributes        = notRequired "TxPropertyDefinition Attributes" //TODO: TxPropertyDefinition method attributes
            override __.MemberType = MemberTypes.Property
            override __.DeclaringType = declTy

            override __.PropertyType = inp.Type |> TxTType
            override __.GetGetMethod(_nonPublic) = 
                tycon.MembersOfFSharpTyconByName.[inp.PropertyName] 
                |> List.tryFind (fun x -> x.IsPropertyGetterMethod) 
                |> Option.map (TxMethodDef declTy)
                |> optionToNull
            override __.GetSetMethod(_nonPublic) =
                tycon.MembersOfFSharpTyconByName.[inp.PropertyName] 
                |> List.tryFind (fun x -> x.IsPropertySetterMethod) 
                |> Option.map (TxMethodDef declTy)
                |> optionToNull
            override __.GetIndexParameters() = 
                //TODO: Implement Property index parameters
                notRequired "TxPropertyDefinition : GetIndexParameters"
                //inp.IndexParameters |> Array.map (TxILParameter (gps, [| |]))
            override __.CanRead =
                tycon.MembersOfFSharpTyconByName.[inp.PropertyName] 
                |> List.tryFind (fun x -> x.IsPropertyGetterMethod)
                |> fun x -> x.IsSome
            override __.CanWrite =
                tycon.MembersOfFSharpTyconByName.[inp.PropertyName] 
                |> List.tryFind (fun x -> x.IsPropertySetterMethod)
                |> fun x -> x.IsSome
            override __.GetCustomAttributesData() = inp.Attribs |> TxCustomAttributesData

            override this.GetHashCode() = hash inp.CompiledName
            override this.Equals(that:obj) = 
                match that with 
                | :? PropertyInfo as thatPI -> 
                    inp.CompiledName = thatPI.Name  &&
                    eqType this.DeclaringType thatPI.DeclaringType 
                | _ -> false

            override __.GetValue(obj, invokeAttr, binder, index, culture) = notRequired "GetValue"
            override __.SetValue(obj, _value, invokeAttr, binder, index, culture) = notRequired "SetValue"
            override __.GetAccessors(nonPublic) = notRequired "GetAccessors"
            override __.ReflectedType = notRequired "ReflectedType"
            override __.GetCustomAttributes(inherited) = notRequired "GetCustomAttributes"
            override __.GetCustomAttributes(attributeType, inherited) = notRequired "GetCustomAttributes"
            override __.IsDefined(attributeType, inherited) = notRequired "IsDefined"

            override __.ToString() = sprintf "ctxt property %s(...) in type %s" inp.CompiledName declTy.Name }

    /// Make an event definition read from a binary available as an EventInfo. Not all methods are implemented.
    //and TxEventDefinition declTy gps (inp: ILEventDef) = 
    //    { new EventInfo() with 

    //        override __.Name = inp.Name 
    //        override __.Attributes = inp.Attributes
    //        override __.MemberType = MemberTypes.Event
    //        override __.DeclaringType = declTy

    //        override __.EventHandlerType = inp.EventHandlerType |> TxILType (gps, [| |])
    //        override __.GetAddMethod(_nonPublic) = inp.AddMethod |> TxILMethodRef
    //        override __.GetRemoveMethod(_nonPublic) = inp.RemoveMethod |> TxILMethodRef
    //        override __.GetCustomAttributesData() = inp.CustomAttrs |> TxCustomAttributesData

    //        override __.GetHashCode() = hash inp.Name
    //        override this.Equals(that:obj) = 
    //            match that with 
    //            | :? EventInfo as thatEI -> 
    //                inp.Name = thatEI.Name  &&
    //                eqType this.DeclaringType thatEI.DeclaringType 
    //            | _ -> false

    //        override __.GetRaiseMethod(nonPublic) = notRequired "GetRaiseMethod"
    //        override __.ReflectedType = notRequired "ReflectedType"
    //        override __.GetCustomAttributes(inherited) = notRequired "GetCustomAttributes"
    //        override __.GetCustomAttributes(attributeType, inherited)  = notRequired "GetCustomAttributes"
    //        override __.IsDefined(attributeType, inherited) = notRequired "IsDefined"

    //        override __.ToString() = sprintf "ctxt event %s(...) in type %s" inp.Name declTy.FullName }

    /// Makes a field definition read from a binary available as a FieldInfo. Not all methods are implemented.
    and TxTType (typ:TType) = 
        match typ with
        | TType_app (tcref,_) -> 
            ContextTypeDefinition(ilGlobals, asm, declTyOpt, tcref) :> Type
        | _ -> failwithf "Unsupported TxTType %A" typ
      
    and TxConst (cnst:Const) = 
        match cnst with 
        | Const.Bool b -> box b
        | Const.Byte b -> box b
        | Const.Char c -> box c
        | Const.Decimal d -> box d
        | Const.Double d -> box d
        | Const.Int16 i -> box i
        | Const.Int32 i -> box i
        | Const.Int64 i -> box i
        | Const.IntPtr i -> box i
        | Const.SByte sb -> box sb
        | Const.Single f -> box f
        | Const.String s -> box s
        | Const.UInt16 i -> box i
        | Const.UInt32 i -> box i
        | Const.UInt64 i -> box i
        | Const.UIntPtr i -> box i
        | Const.Unit -> box ()
        | Const.Zero -> box 0
    
    and TxFieldDefinition declTy _ (* gps *) (inp: RecdField) = 
        { new FieldInfo() with 

            override __.Name = inp.Name 
            override __.Attributes = FieldAttributes.Static ||| FieldAttributes.Literal ||| FieldAttributes.Public 
            override __.MemberType = MemberTypes.Field 
            override __.DeclaringType = declTy

            override __.FieldType = inp.FormalType |> TxTType
            override __.GetRawConstantValue()  = match inp.LiteralValue with None -> null | Some v -> TxConst v
            override __.GetCustomAttributesData() = notRequired "CustomAttribute data" // inp.FieldAttribs |> TxCustomAttributesData

            override __.GetHashCode() = hash inp.Name
            override this.Equals(that:obj) = 
                match that with 
                | :? EventInfo as thatFI -> 
                    inp.Name = thatFI.Name  &&
                    eqType this.DeclaringType thatFI.DeclaringType 
                | _ -> false
    
            override __.ReflectedType = notRequired "ReflectedType"
            override __.GetCustomAttributes(inherited) = notRequired "GetCustomAttributes"
            override __.GetCustomAttributes(attributeType, inherited) = notRequired "GetCustomAttributes"
            override __.IsDefined(attributeType, inherited) = notRequired "IsDefined"
            override __.SetValue(obj, _value, invokeAttr, binder, culture) = notRequired "SetValue"
            override __.GetValue(obj) = notRequired "GetValue"
            override __.FieldHandle = notRequired "FieldHandle"

            override __.ToString() = sprintf "ctxt literal field %s(...) in type %s" inp.Name declTy.FullName }

    /// Bind a reference to an assembly
    //and TxScopeRef(sref: ILScopeRef) = 
    //    match sref with 
    //    | ILScopeRef.Assembly aref -> match tryBindAssembly aref with Choice1Of2 asm -> asm | Choice2Of2 exn -> raise exn
    //    | ILScopeRef.Local -> asm
    //    | ILScopeRef.Module _ -> asm

    /// Bind a reference to a type
    //and TxILTypeRef(tref: ILTypeRef) : Type = 
    //    match tref.Scope with 
    //    | ILTypeRefScope.Top scoref -> TxScopeRef(scoref).BindType(tref.Namespace, tref.Name)
    //    | ILTypeRefScope.Nested tref -> TxILTypeRef(tref).GetNestedType(tref.Name,BindingFlags.Public ||| BindingFlags.NonPublic)

    ///// Bind a reference to a constructor
    and TxConstructor (mref: ValRef) = 
        let argTypes = [||]//Array.map (TxILType ([| |], [| |])) mref.  
        let declTy = TxTType mref.Type
        let cons = declTy.GetConstructor(BindingFlags.Public ||| BindingFlags.NonPublic, null, argTypes, null)
        if cons = null then failwith (sprintf "constructor reference '%A' not resolved" mref)
        cons

    ///// Bind a reference to a metehod
    //and TxILMethodRef(mref: ILMethodRef) = 
    //    let argTypes = mref.ArgTypes |> Array.map (TxILType ([| |], [| |])) 
    //    let declTy = mref.EnclosingTypeRef |> TxILTypeRef
    //    let meth = declTy.GetMethod(mref.Name, BindingFlags.Public ||| BindingFlags.NonPublic, null, argTypes, null)
    //    if meth = null then failwith (sprintf "method reference '%A' not resolved" mref)
    //    meth

    /// Convert an ILType read from a binary to a System.Type backed by ContextTypeDefinitions
    //and TxILType gps (ty: ILType) = 
      
    //    match ty with 
    //    | ILType.Void -> typeof<System.Void>
    //    | ILType.Value tspec 
    //    | ILType.Boxed tspec ->
    //        let tdefR = TxILTypeRef tspec.TypeRef 
    //        match tspec.GenericArgs with 
    //        | [| |] -> tdefR
    //        | args -> tdefR.MakeGenericType(Array.map (TxILType gps) args)  
    //    | ILType.Array(rank, arg) ->
    //        let argR = TxILType gps arg
    //        if rank.Rank = 1 then argR.MakeArrayType()
    //        else argR.MakeArrayType(rank.Rank)
    //    | ILType.FunctionPointer _  -> failwith "unexpected function type"
    //    | ILType.Ptr(arg) -> (TxILType gps arg).MakePointerType()
    //    | ILType.Byref(arg) -> (TxILType gps arg).MakeByRefType()
    //    | ILType.Modified(_,_mod,arg) -> TxILType gps arg
    //    | ILType.Var(n) -> 
    //        let (gps1:Type[]),(gps2:Type[]) = gps
    //        if n < gps1.Length then gps1.[n] 
    //        elif n < gps1.Length + gps2.Length then gps2.[n - gps1.Length] 
    //        else failwith (sprintf "generic parameter index our of range: %d" n)

    /// Convert an ILGenericParameterDef read from a binary to a System.Type.
    and TxGenericParam _ (* gpsf *) pos (inp: Typar) =
        { new Type() with 
            override __.Name = inp.Name 
            override __.Assembly = (asm :> Assembly)
            override __.FullName = inp.Name
            override __.IsGenericParameter = true
            override __.GenericParameterPosition = pos
            override __.GetGenericParameterConstraints() = [||]
                //TODO: Implement generic parameter constraints
                //inp.Constraints |> Array.map (fun x -> x TxILType (gpsf()))
                    
            override __.MemberType = enum 0

            override __.Namespace = null //notRequired "Namespace"
            override __.DeclaringType = notRequired "DeclaringType"
            override __.BaseType = notRequired "BaseType"
            override __.GetInterfaces() = notRequired "GetInterfaces"

            override this.GetConstructors(_bindingFlags) = notRequired "GetConstructors"
            override this.GetMethods(_bindingFlags) = notRequired "GetMethods"
            override this.GetField(name, _bindingFlags) = notRequired "GetField"
            override this.GetFields(_bindingFlags) = notRequired "GetFields"
            override this.GetEvent(name, _bindingFlags) = notRequired "GetEvent"
            override this.GetEvents(_bindingFlags) = notRequired "GetEvents"
            override this.GetProperties(_bindingFlags) = notRequired "GetProperties"
            override this.GetMembers(_bindingFlags) = notRequired "GetMembers"
            override this.GetNestedTypes(_bindingFlags) = notRequired "GetNestedTypes"
            override this.GetNestedType(name, _bindingFlags) = notRequired "GetNestedType"
            override this.GetPropertyImpl(name, _bindingFlags, _binder, _returnType, _types, _modifiers) = notRequired "GetPropertyImpl"
            override this.MakeGenericType(args) = notRequired "MakeGenericType"
            override this.MakeArrayType() = ContextTypeSymbol(ContextTypeSymbolKind.SDArray, [| this |]) :> Type
            override this.MakeArrayType arg = ContextTypeSymbol(ContextTypeSymbolKind.Array arg, [| this |]) :> Type
            override this.MakePointerType() = ContextTypeSymbol(ContextTypeSymbolKind.Pointer, [| this |]) :> Type
            override this.MakeByRefType() = ContextTypeSymbol(ContextTypeSymbolKind.ByRef, [| this |]) :> Type

            override __.GetAttributeFlagsImpl() = TypeAttributes.Public ||| TypeAttributes.Class ||| TypeAttributes.Sealed 

            override __.IsArrayImpl() = false
            override __.IsByRefImpl() = false
            override __.IsPointerImpl() = false
            override __.IsPrimitiveImpl() = false
            override __.IsCOMObjectImpl() = false
            override __.IsGenericType = false
            override __.IsGenericTypeDefinition = false

            override __.HasElementTypeImpl() = false

            override this.UnderlyingSystemType = this
            override __.GetCustomAttributesData() = inp.Attribs |> TxCustomAttributesData

            override this.Equals(that:obj) = System.Object.ReferenceEquals (this, that) 

            override __.ToString() = sprintf "ctxt generic param %s" inp.Name 

            override this.AssemblyQualifiedName                                                            = "[" + this.Assembly.FullName + "]" + this.FullName

            override __.GetGenericArguments() = notRequired "GetGenericArguments"
            override __.GetGenericTypeDefinition() = notRequired "GetGenericTypeDefinition"
            override __.GetMember(name,mt,_bindingFlags)                                                      = notRequired "TxILGenericParam: GetMember"
            override __.GUID                                                                                      = notRequired "TxILGenericParam: GUID"
            override __.GetMethodImpl(name, _bindingFlags, binder, callConvention, types, modifiers)          = notRequired "TxILGenericParam: GetMethodImpl"
            override __.GetConstructorImpl(_bindingFlags, binder, callConvention, types, modifiers)           = notRequired "TxILGenericParam: GetConstructorImpl"
            override __.GetCustomAttributes(inherited)                                                            = notRequired "TxILGenericParam: GetCustomAttributes"
            override __.GetCustomAttributes(attributeType, inherited)                                             = notRequired "TxILGenericParam: GetCustomAttributes"
            override __.IsDefined(attributeType, inherited)                                                       = notRequired "TxILGenericParam: IsDefined"
            override __.GetInterface(name, ignoreCase)                                                            = notRequired "TxILGenericParam: GetInterface"
            override __.Module                                                                                    = notRequired "TxILGenericParam: Module" : Module 
            override __.GetElementType()                                                                          = notRequired "TxILGenericParam: GetElementType"
            override __.InvokeMember(name, invokeAttr, binder, target, args, modifiers, culture, namedParameters) = notRequired "TxILGenericParam: InvokeMember"

        }

    let rec gps = inp.TyparsNoRange |> List.mapi (fun i gp -> TxGenericParam (fun () -> gps, [| |]) i gp) |> List.toArray

    let isNested = declTyOpt.IsSome

    override __.Name = inp.CompiledName 
    override __.Assembly = (asm :> Assembly) 
    override __.DeclaringType = declTyOpt |> optionToNull
    override __.MemberType = if isNested then MemberTypes.NestedType else MemberTypes.TypeInfo

    override __.FullName = inp.CompiledRepresentationForNamedType.QualifiedName
                    
    override __.Namespace = inp.CompiledRepresentationForNamedType.Enclosing.[0]
    override __.BaseType = null//inp. |> Option.map (TxILType (gps, [| |])) |> optionToNull
    override __.GetInterfaces() = inp.ImmediateInterfaceTypesOfFSharpTycon |> List.map TxTType |> List.toArray

    override this.GetConstructors(_bindingFlags) = 
        inp.MembersOfFSharpTyconSorted 
        |> List.filter (fun x -> x.IsConstructor)
        |> List.map (TxConstructor)
        |> List.toArray

    override this.GetMethods(_bindingFlags) = 
        inp.Deref.entity_tycon_tcaug.tcaug_adhoc_list 
        |> Seq.map (snd >> TxMethodDef this)
        |> Seq.toArray
        //inp.Methods.Elements |> Array.map (TxILMethodDef this)

    override this.GetField(name, _bindingFlags) = 
        inp.AllFieldTable.FieldByName(name)
        |> Option.map (TxFieldDefinition this gps) 
        |> optionToNull

    override this.GetFields(_bindingFlags) = 
        inp.AllFieldsArray
        |> Array.map (TxFieldDefinition this gps)

    override this.GetEvent(name, _bindingFlags) = 
        //TODO: Need to implement events
        notRequired (sprintf "Could not get event %A" name) 
        //inp.Events.Elements 
        //|> Array.tryPick (fun ev -> if ev.Name = name then Some (TxEventDefinition this gps ev) else None) 
        //|> optionToNull

    override this.GetEvents(_bindingFlags) = 
        //TODO: Need to implement events
        notRequired "Could not get events"

    override this.GetProperties(_bindingFlags) = 
        let isProperty (kind:Ast.MemberKind option) =
            match kind with
            | Some kind ->
                kind = Ast.MemberKind.PropertyGet
                || kind = Ast.MemberKind.PropertyGetSet
                || kind = Ast.MemberKind.PropertySet
            | None -> false

        inp.MembersOfFSharpTyconSorted
        |> List.filter (fun x -> x.MemberInfo |> Option.map (fun x -> x.MemberFlags.MemberKind) |> isProperty)
        |> List.map (TxPropertyDefinition this gps inp)
        |> List.toArray

    override this.GetMembers(_bindingFlags) = 
        [| for x in this.GetMethods() do yield (x :> MemberInfo)
           for x in this.GetFields() do yield (x :> MemberInfo)
           for x in this.GetProperties() do yield (x :> MemberInfo)
           for x in this.GetEvents() do yield (x :> MemberInfo)
           for x in this.GetNestedTypes() do yield (x :> MemberInfo) |]
 
    override this.GetNestedTypes(_bindingFlags) = 
        //TODO: GetNested types
        notRequired "ContextTypeDefinition : GetNestedTypes"
        //inp.NestedTyconRef inp.Deref 
        //|> Array.map (asm.TxILTypeDef (Some (this :> Type)))
 
    // GetNestedType is used for linking to the binding context
    override this.GetNestedType(_ (* name *), _bindingFlags) = 
        //TODO: GetNested type by name
        notRequired "ContextTypeDefinition : GetNestedType by name"
        //inp.NestedTypes.TryFindByName(UNone, name) |> Option.map (asm.TxILTypeDef (Some (this :> Type))) |> optionToNull

    override this.GetPropertyImpl(_ (* name *), _bindingFlags, _binder, _returnType, (* types *) _, _modifiers) = 
        //TODO: GetPropertyImpl type by name
        notRequired "ContextTypeDefinition : GetPropertyImpl by name"
        //inp.Properties.Elements 
        //|> Array.tryPick (fun p -> if p.Name = name then Some (TxPropertyDefinition this gps p) else None) 
        //|> optionToNull
        
    override this.GetMethodImpl(_ (* name *), _bindingFlags, _binder, _callConvention, (* types *) _, _modifiers) =
        //TODO: GetMethodImpl type by name
        notRequired "ContextTypeDefinition : GetMethodImpl by name"
        //inp.Methods.FindByNameAndArity(name, types.Length)
        //|> Array.find (fun md -> eqTypesAndILTypes types md.ParameterTypes)
        //|> TxILMethodDef this

    override this.GetConstructorImpl(_bindingFlags, _binder, _callConvention, (* types *) _, _modifiers) = 
        //TODO: GetMethodImpl type by name
        notRequired "ContextTypeDefinition : GetConstructorImpl by name"
        //inp.Methods.FindByNameAndArity(".ctor", types.Length)
        //|> Array.find (fun md -> eqTypesAndILTypes types md.ParameterTypes)
        //|> TxILConstructorDef this

    // Every implementation of System.Type must meaningfully implement these
    override this.MakeGenericType(args) = ContextTypeSymbol(ContextTypeSymbolKind.Generic this, args) :> Type
    override this.MakeArrayType() = ContextTypeSymbol(ContextTypeSymbolKind.SDArray, [| this |]) :> Type
    override this.MakeArrayType arg = ContextTypeSymbol(ContextTypeSymbolKind.Array arg, [| this |]) :> Type
    override this.MakePointerType() = ContextTypeSymbol(ContextTypeSymbolKind.Pointer, [| this |]) :> Type
    override this.MakeByRefType() = ContextTypeSymbol(ContextTypeSymbolKind.ByRef, [| this |]) :> Type

    override __.GetAttributeFlagsImpl() = 
        //TODO: GetAttributeFlagsImpl this needs fully completing
        let attr = TypeAttributes.Public ||| TypeAttributes.Class 
      //  let attr = if inp.IsSealed then attr ||| TypeAttributes.Sealed else attr
        let attr = if inp.IsFSharpInterfaceTycon then attr ||| TypeAttributes.Interface else attr
       // let attr = if inp.Is then attr ||| TypeAttributes.Serializable else attr
       // if isNested then adjustTypeAttributes isNested attr else attr
        attr

    override __.IsValueTypeImpl() = inp.IsStructOrEnumTycon || inp.IsFSharpStructOrEnumTycon
    override __.IsArrayImpl() = false
    override __.IsByRefImpl() = false
    override __.IsPointerImpl() = false
    override __.IsPrimitiveImpl() = false
    override __.IsCOMObjectImpl() = false
    override __.IsGenericType = (gps.Length <> 0)
    override __.IsGenericTypeDefinition = (gps.Length <> 0)
    override __.HasElementTypeImpl() = false

    override this.UnderlyingSystemType = (this :> Type)
    override __.GetCustomAttributesData() = inp.Attribs |> TxCustomAttributesData

    override this.Equals(that:obj) = System.Object.ReferenceEquals (this, that)  
    override this.GetHashCode() =  hash inp.CompiledName
    override this.IsAssignableFrom(otherTy) = base.IsAssignableFrom(otherTy) || this.Equals(otherTy)
    override this.IsSubclassOf(otherTy) = base.IsSubclassOf(otherTy) || inp.IsFSharpDelegateTycon && otherTy = typeof<Delegate> // F# quotations implementation

    override this.AssemblyQualifiedName                                                            = "[" + this.Assembly.FullName + "]" + this.FullName

    override this.ToString() = this.FullName

    override __.GetGenericArguments() = gps
    override __.GetGenericTypeDefinition() = notRequired "GetGenericTypeDefinition"
    override __.GetMember(_name, _memberType, _bindingFlags)                                                      = notRequired "TxILTypeDef: GetMember"
    override __.GUID                                                                                      = notRequired "TxILTypeDef: GUID"
    override __.GetCustomAttributes(_inherited)                                                            = notRequired "TxILTypeDef: GetCustomAttributes"
    override __.GetCustomAttributes(_attributeType, _inherited)                                             = notRequired "TxILTypeDef: GetCustomAttributes"
    override __.IsDefined(_attributeType, _inherited)                                                       = notRequired "TxILTypeDef: IsDefined"
    override __.GetInterface(_name, _ignoreCase)                                                            = notRequired "TxILTypeDef: GetInterface"
    override __.Module                                                                                    = notRequired "TxILTypeDef: Module" : Module 
    override __.GetElementType()                                                                          = notRequired "TxILTypeDef: GetElementType"
    override __.InvokeMember(_name, _invokeAttr, _binder, _target, _args, _modifiers, _culture, _namedParameters) = notRequired "TxILTypeDef: InvokeMember"

    member x.Metadata = inp
    member x.MakeMethodInfo (declTy,md) = TxMethodDef declTy md
    member x.MakeConstructorInfo (declTy,md) = TxConstructorDef declTy md


and ContextAssembly(ilGlobals, thunk: CcuThunk, location:string) as asm =
    inherit Assembly()

    // A table tracking how type definition objects are translated.
    let txTable = TxTable<Type>()

    member __.TxTypeDef (declTyOpt: Type option) (inp: TyconRef) =
        txTable.Get inp.Stamp (fun () -> ContextTypeDefinition(ilGlobals, asm, declTyOpt, inp) :> System.Type)

    override x.GetTypes () = [| for td in thunk.RootModulesAndNamespaces -> x.TxTypeDef None (mkLocalEntityRef td) |]
    override x.Location = location

    override x.GetType (nm:string) = 
        if nm.Contains("+") then 
            let i = nm.LastIndexOf("+")
            let enc,nm2 = nm.[0..i-1], nm.[i+1..]
            match x.GetType(enc) with 
            | null -> null
            | t -> t.GetNestedType(nm2,BindingFlags.Public ||| BindingFlags.NonPublic)
        elif nm.Contains(".") then 
            let i = nm.LastIndexOf(".")
            let nsp,nm2 = nm.[0..i-1], nm.[i+1..]
            x.TryBindType(Some nsp, nm2) |> optionToNull
        else
            x.TryBindType(None, nm) |> optionToNull

    override x.GetName () = new AssemblyName(thunk.AssemblyName)

    override x.FullName = x.GetName().ToString()

    override x.ReflectionOnly = true

    override x.GetManifestResourceStream(_:string) = 
        //TODO: Implement Get Manifest Resource Stream
        notRequired "GetManifestResourceStreams"
        //let r = reader.ILModuleDef.Resources.Elements |> Seq.find (fun r -> r.Name = resourceName) 
        //match r.Location with 
        //| ILResourceLocation.Local f -> new MemoryStream(f()) :> Stream
        //| _ -> notRequired (sprintf "reading manifest resource %s from non-embedded location" resourceName)

    member x.BindType(nsp:string option, nm:string) = 
        match x.TryBindType(nsp, nm) with 
        | None -> failwithf "failed to bind type %s in assembly %s" nm asm.FullName
        | Some res -> res

    member x.TryBindType(nsp:string option, nm:string) : Type option = 
        match thunk.RootModulesAndNamespaces |> List.tryFind (fun x -> x.CompiledName = nm) with 
        | Some td -> asm.TxTypeDef None (mkLocalTyconRef td) |> Some
        | None -> 
            //TODO: How to bind an exported type for now just match a type that already may have been generated
            txTable.Values |> Seq.tryFind (fun t -> (match nsp with | Some ns -> t.Namespace = ns | None -> true) && t.Name = nm)
            //match reader.ILModuleDef.ManifestOfAssembly.ExportedTypes.TryFindByName(nsp, nm) with 
            //| Some tref -> 
            //    match tref.ScopeRef with 
            //    | ILScopeRef.Assembly aref2 -> 
            //        let ass2opt = tryBindAssembly(aref2)
            //        match ass2opt with 
            //        | Choice1Of2 ass2 -> ass2.TryBindType(nsp, nm)
            //        | Choice2Of2 _err -> None 
            //    | _ -> 
            //        printfn "unexpected non-forwarder during binding"
            //        None
            //| None -> None

    override x.ToString() = "ctxt assembly " + x.FullName
