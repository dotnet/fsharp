// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.Symbols

open System.Text.RegularExpressions
open Internal.Utilities.Library
open FSharp.Compiler.Syntax

/// Patterns over FSharpSymbol and derivatives.
[<RequireQualifiedAccess>]
module FSharpSymbolPatterns =

    module Option =
        let attempt f = try Some(f()) with _ -> None

    let hasModuleSuffixAttribute (entity: FSharpEntity) = 
            entity.TryGetAttribute<CompilationRepresentationAttribute>()
            |> Option.bind (fun a -> 
                Option.attempt (fun _ -> a.ConstructorArguments)
                |> Option.bind (fun args -> args |> Seq.tryPick (fun (_, arg) ->
                    let res =
                        match arg with
                        | :? int32 as arg when arg = int CompilationRepresentationFlags.ModuleSuffix -> 
                            Some() 
                        | :? CompilationRepresentationFlags as arg when arg = CompilationRepresentationFlags.ModuleSuffix -> 
                            Some() 
                        | _ -> 
                            None
                    res)))
            |> Option.isSome

    [<return: Struct>]
    let (|AbbreviatedType|_|) (entity: FSharpEntity) =
        if entity.IsFSharpAbbreviation then ValueSome entity.AbbreviatedType
        else ValueNone

    [<return: Struct>]
    let (|TypeWithDefinition|_|) (ty: FSharpType) =
        if ty.HasTypeDefinition then ValueSome ty.TypeDefinition
        else ValueNone

    let rec getEntityAbbreviatedType (entity: FSharpEntity) =
        if entity.IsFSharpAbbreviation then
            match entity.AbbreviatedType with
            | TypeWithDefinition def -> getEntityAbbreviatedType def
            | abbreviatedTy -> entity, Some abbreviatedTy
        else entity, None

    [<return: Struct>]
    let (|Attribute|_|) (entity: FSharpEntity) =
        let isAttribute (entity: FSharpEntity) =
            let getBaseType (entity: FSharpEntity) =
                try 
                    match entity.BaseType with
                    | Some (TypeWithDefinition def) -> Some def
                    | _ -> None
                with _ -> None

            let rec isAttributeType (ty: FSharpEntity option) =
                match ty with
                | None -> false
                | Some ty ->
                    try ty.FullName = "System.Attribute" || isAttributeType (getBaseType ty)
                    with _ -> false
            isAttributeType (Some entity)
        if isAttribute entity then ValueSome() else ValueNone

    [<return: Struct>]
    let (|ValueType|_|) (e: FSharpEntity) =
        if e.IsEnum || e.IsValueType || e.HasAttribute<MeasureAnnotatedAbbreviationAttribute>() then ValueSome()
        else ValueNone

#if !NO_TYPEPROVIDERS
    [<return: Struct>]
    let (|Class|_|) (original: FSharpEntity, abbreviated: FSharpEntity, _) = 
        if abbreviated.IsClass 
           && (not abbreviated.IsStaticInstantiation || original.IsFSharpAbbreviation) then ValueSome()
        else ValueNone
#else
    [<return: Struct>]
    let (|Class|_|) (original: FSharpEntity, abbreviated: FSharpEntity, _) = 
        if abbreviated.IsClass && original.IsFSharpAbbreviation then ValueSome()
        else ValueNone 
#endif        

    [<return: Struct>]
    let (|Record|_|) (e: FSharpEntity) = if e.IsFSharpRecord then ValueSome() else ValueNone

    [<return: Struct>]
    let (|UnionType|_|) (e: FSharpEntity) = if e.IsFSharpUnion then ValueSome() else ValueNone

    [<return: Struct>]
    let (|Delegate|_|) (e: FSharpEntity) = if e.IsDelegate then ValueSome() else ValueNone

    [<return: Struct>]
    let (|FSharpException|_|) (e: FSharpEntity) = if e.IsFSharpExceptionDeclaration then ValueSome() else ValueNone

    [<return: Struct>]
    let (|Interface|_|) (e: FSharpEntity) = if e.IsInterface then ValueSome() else ValueNone

    [<return: Struct>]
    let (|AbstractClass|_|) (e: FSharpEntity) =
        if e.HasAttribute<AbstractClassAttribute>() then ValueSome() else ValueNone
            
    [<return: Struct>]
    let (|FSharpType|_|) (e: FSharpEntity) = 
        if e.IsDelegate || e.IsFSharpExceptionDeclaration || e.IsFSharpRecord || e.IsFSharpUnion 
            || e.IsInterface || e.IsMeasure 
            || (e.IsFSharp && e.IsOpaque && not e.IsFSharpModule && not e.IsNamespace) then ValueSome() 
        else ValueNone

#if !NO_TYPEPROVIDERS
    [<return: Struct>]
    let (|ProvidedType|_|) (e: FSharpEntity) =
        if (e.IsProvided || e.IsProvidedAndErased || e.IsProvidedAndGenerated) && e.CompiledName = e.DisplayName then
            ValueSome()
        else ValueNone
#endif        

    [<return: Struct>]
    let (|ByRef|_|) (e: FSharpEntity) = if e.IsByRef then ValueSome() else ValueNone

    [<return: Struct>]
    let (|Array|_|) (e: FSharpEntity) = if e.IsArrayType then ValueSome() else ValueNone

    [<return: Struct>]
    let (|FSharpModule|_|) (entity: FSharpEntity) = if entity.IsFSharpModule then ValueSome() else ValueNone

    [<return: Struct>]
    let (|Namespace|_|) (entity: FSharpEntity) = if entity.IsNamespace then ValueSome() else ValueNone

#if !NO_TYPEPROVIDERS    
    [<return: Struct>]
    let (|ProvidedAndErasedType|_|) (entity: FSharpEntity) = if entity.IsProvidedAndErased then ValueSome() else ValueNone
#endif

    [<return: Struct>]
    let (|Enum|_|) (entity: FSharpEntity) = if entity.IsEnum then ValueSome() else ValueNone

    [<return: Struct>]
    let (|Tuple|_|) (ty: FSharpType) =
        if ty.IsTupleType then ValueSome() else ValueNone

    [<return: Struct>]
    let (|RefCell|_|) (ty: FSharpType) = 
        match ty.StripAbbreviations() with
        | TypeWithDefinition def when 
            def.IsFSharpRecord && def.FullName = "Microsoft.FSharp.Core.FSharpRef`1" -> ValueSome() 
        | _ -> ValueNone

    [<return: Struct>]
    let (|FunctionType|_|) (ty: FSharpType) = 
        if ty.IsFunctionType then ValueSome() 
        else ValueNone

    [<return: Struct>]
    let (|Pattern|_|) (symbol: FSharpSymbol) =
        match symbol with
        | :? FSharpUnionCase
        | :? FSharpActivePatternCase -> ValueSome()
        | _ -> ValueNone

    [<return: Struct>]
    let (|Field|_|) (symbol: FSharpSymbol) =
        match symbol with
        | :? FSharpField as field -> ValueSome (field, field.FieldType.StripAbbreviations())
        | _ -> ValueNone

    [<return: Struct>]
    let (|MutableVar|_|) (symbol: FSharpSymbol) = 
        let isMutable = 
            match symbol with
            | :? FSharpField as field -> field.IsMutable && not field.IsLiteral
            | :? FSharpMemberOrFunctionOrValue as func -> func.IsMutable
            | _ -> false
        if isMutable then ValueSome() else ValueNone

    /// Entity (originalEntity, abbreviatedEntity, abbreviatedTy)
    [<return: Struct>]
    let (|FSharpEntity|_|) (symbol: FSharpSymbol) =
        match symbol with
        | :? FSharpEntity as entity -> 
            let abbreviatedEntity, abbreviatedTy = getEntityAbbreviatedType entity
            ValueSome (entity, abbreviatedEntity, abbreviatedTy)
        | _ -> ValueNone

    [<return: Struct>]
    let (|Parameter|_|) (symbol: FSharpSymbol) = 
        match symbol with
        | :? FSharpParameter -> ValueSome()
        | _ -> ValueNone

    [<return: Struct>]
    let (|UnionCase|_|) (e: FSharpSymbol) = 
        match e with
        | :? FSharpUnionCase as uc -> ValueSome uc
        | _ -> ValueNone

    [<return: Struct>]
    let (|RecordField|_|) (e: FSharpSymbol) =
        match e with
        | :? FSharpField as field ->
            match field.DeclaringEntity with 
            | None -> ValueNone
            | Some e -> if e.IsFSharpRecord then ValueSome field else ValueNone
        | _ -> ValueNone

    [<return: Struct>]
    let (|ActivePatternCase|_|) (symbol: FSharpSymbol) =
        match symbol with
        | :? FSharpActivePatternCase as case -> ValueSome case
        | _ -> ValueNone

    /// Func (memberFunctionOrValue, fullType)
    [<return: Struct>]
    let (|MemberFunctionOrValue|_|) (symbol: FSharpSymbol) =
        match symbol with
        | :? FSharpMemberOrFunctionOrValue as func -> ValueSome func
        | _ -> ValueNone

    /// Constructor (enclosingEntity)
    [<return: Struct>]
    let (|Constructor|_|) (func: FSharpMemberOrFunctionOrValue) =
        match func.CompiledName with
        | ".ctor" | ".cctor" -> func.DeclaringEntity |> ValueOptionInternal.ofOption
        | _ -> ValueNone

    [<return: Struct>]
    let (|Function|_|) excluded (func: FSharpMemberOrFunctionOrValue) =
        try let ty = func.FullType.StripAbbreviations()
            if ty.IsFunctionType
               && not func.IsPropertyGetterMethod 
               && not func.IsPropertySetterMethod
               && not excluded
               && not (PrettyNaming.IsOperatorDisplayName func.DisplayName) then ValueSome()
            else ValueNone
        with _ -> ValueNone

    [<return: Struct>]
    let (|ExtensionMember|_|) (func: FSharpMemberOrFunctionOrValue) =
        if func.IsExtensionMember then ValueSome() else ValueNone

    [<return: Struct>]
    let (|Event|_|) (func: FSharpMemberOrFunctionOrValue) =
        if func.IsEvent then ValueSome () else ValueNone