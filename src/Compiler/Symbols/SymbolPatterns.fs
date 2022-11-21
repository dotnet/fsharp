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

    let UnnamedUnionFieldRegex = Regex("^Item(\d+)?$", RegexOptions.Compiled)
    
    let (|AbbreviatedType|_|) (entity: FSharpEntity) =
        if entity.IsFSharpAbbreviation then Some entity.AbbreviatedType
        else None

    let (|TypeWithDefinition|_|) (ty: FSharpType) =
        if ty.HasTypeDefinition then Some ty.TypeDefinition
        else None

    let rec getEntityAbbreviatedType (entity: FSharpEntity) =
        if entity.IsFSharpAbbreviation then
            match entity.AbbreviatedType with
            | TypeWithDefinition def -> getEntityAbbreviatedType def
            | abbreviatedTy -> entity, Some abbreviatedTy
        else entity, None

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
        if isAttribute entity then Some() else None

    let (|ValueType|_|) (e: FSharpEntity) =
        if e.IsEnum || e.IsValueType || e.HasAttribute<MeasureAnnotatedAbbreviationAttribute>() then Some()
        else None

#if !NO_TYPEPROVIDERS
    let (|Class|_|) (original: FSharpEntity, abbreviated: FSharpEntity, _) = 
        if abbreviated.IsClass 
           && (not abbreviated.IsStaticInstantiation || original.IsFSharpAbbreviation) then Some()
        else None
#else
    let (|Class|_|) (original: FSharpEntity, abbreviated: FSharpEntity, _) = 
        if abbreviated.IsClass && original.IsFSharpAbbreviation then Some()
        else None 
#endif        

    let (|Record|_|) (e: FSharpEntity) = if e.IsFSharpRecord then Some() else None

    let (|UnionType|_|) (e: FSharpEntity) = if e.IsFSharpUnion then Some() else None

    let (|Delegate|_|) (e: FSharpEntity) = if e.IsDelegate then Some() else None

    let (|FSharpException|_|) (e: FSharpEntity) = if e.IsFSharpExceptionDeclaration then Some() else None

    let (|Interface|_|) (e: FSharpEntity) = if e.IsInterface then Some() else None

    let (|AbstractClass|_|) (e: FSharpEntity) =
        if e.HasAttribute<AbstractClassAttribute>() then Some() else None
            
    let (|FSharpType|_|) (e: FSharpEntity) = 
        if e.IsDelegate || e.IsFSharpExceptionDeclaration || e.IsFSharpRecord || e.IsFSharpUnion 
            || e.IsInterface || e.IsMeasure 
            || (e.IsFSharp && e.IsOpaque && not e.IsFSharpModule && not e.IsNamespace) then Some() 
        else None

#if !NO_TYPEPROVIDERS
    let (|ProvidedType|_|) (e: FSharpEntity) =
        if (e.IsProvided || e.IsProvidedAndErased || e.IsProvidedAndGenerated) && e.CompiledName = e.DisplayName then
            Some()
        else None
#endif        

    let (|ByRef|_|) (e: FSharpEntity) = if e.IsByRef then Some() else None

    let (|Array|_|) (e: FSharpEntity) = if e.IsArrayType then Some() else None

    let (|FSharpModule|_|) (entity: FSharpEntity) = if entity.IsFSharpModule then Some() else None

    let (|Namespace|_|) (entity: FSharpEntity) = if entity.IsNamespace then Some() else None

#if !NO_TYPEPROVIDERS    
    let (|ProvidedAndErasedType|_|) (entity: FSharpEntity) = if entity.IsProvidedAndErased then Some() else None
#endif

    let (|Enum|_|) (entity: FSharpEntity) = if entity.IsEnum then Some() else None

    let (|Tuple|_|) (ty: FSharpType) =
        if ty.IsTupleType then Some() else None

    let (|RefCell|_|) (ty: FSharpType) = 
        match ty.StripAbbreviations() with
        | TypeWithDefinition def when 
            def.IsFSharpRecord && def.FullName = "Microsoft.FSharp.Core.FSharpRef`1" -> Some() 
        | _ -> None

    let (|FunctionType|_|) (ty: FSharpType) = 
        if ty.IsFunctionType then Some() 
        else None

    let (|Pattern|_|) (symbol: FSharpSymbol) =
        match symbol with
        | :? FSharpUnionCase
        | :? FSharpActivePatternCase -> Some()
        | _ -> None

    let (|Field|_|) (symbol: FSharpSymbol) =
        match symbol with
        | :? FSharpField as field -> Some (field, field.FieldType.StripAbbreviations())
        | _ -> None

    let (|MutableVar|_|) (symbol: FSharpSymbol) = 
        let isMutable = 
            match symbol with
            | :? FSharpField as field -> field.IsMutable && not field.IsLiteral
            | :? FSharpMemberOrFunctionOrValue as func -> func.IsMutable
            | _ -> false
        if isMutable then Some() else None

    /// Entity (originalEntity, abbreviatedEntity, abbreviatedTy)
    let (|FSharpEntity|_|) (symbol: FSharpSymbol) =
        match symbol with
        | :? FSharpEntity as entity -> 
            let abbreviatedEntity, abbreviatedTy = getEntityAbbreviatedType entity
            Some (entity, abbreviatedEntity, abbreviatedTy)
        | _ -> None

    let (|Parameter|_|) (symbol: FSharpSymbol) = 
        match symbol with
        | :? FSharpParameter -> Some()
        | _ -> None

    let (|UnionCase|_|) (e: FSharpSymbol) = 
        match e with
        | :? FSharpUnionCase as uc -> Some uc
        | _ -> None

    let (|RecordField|_|) (e: FSharpSymbol) =
        match e with
        | :? FSharpField as field ->
            match field.DeclaringEntity with 
            | None -> None
            | Some e -> if e.IsFSharpRecord then Some field else None
        | _ -> None

    let (|ActivePatternCase|_|) (symbol: FSharpSymbol) =
        match symbol with
        | :? FSharpActivePatternCase as case -> Some case
        | _ -> None

    /// Func (memberFunctionOrValue, fullType)
    let (|MemberFunctionOrValue|_|) (symbol: FSharpSymbol) =
        match symbol with
        | :? FSharpMemberOrFunctionOrValue as func -> Some func
        | _ -> None

    /// Constructor (enclosingEntity)
    let (|Constructor|_|) (func: FSharpMemberOrFunctionOrValue) =
        match func.CompiledName with
        | ".ctor" | ".cctor" -> func.DeclaringEntity
        | _ -> None

    let (|Function|_|) excluded (func: FSharpMemberOrFunctionOrValue) =
        try let ty = func.FullType.StripAbbreviations()
            if ty.IsFunctionType
               && not func.IsPropertyGetterMethod 
               && not func.IsPropertySetterMethod
               && not excluded
               && not (PrettyNaming.IsOperatorDisplayName func.DisplayName) then Some()
            else None
        with _ -> None

    let (|ExtensionMember|_|) (func: FSharpMemberOrFunctionOrValue) =
        if func.IsExtensionMember then Some() else None

    let (|Event|_|) (func: FSharpMemberOrFunctionOrValue) =
        if func.IsEvent then Some () else None