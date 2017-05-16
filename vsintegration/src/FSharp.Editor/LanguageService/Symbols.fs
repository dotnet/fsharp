[<AutoOpen>]
module internal Microsoft.VisualStudio.FSharp.Editor.Symbols

open System
open System.Collections.Generic
open System.Threading
open System.Threading.Tasks
open System.Runtime.CompilerServices

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.Classification
open Microsoft.CodeAnalysis.Text

open Microsoft.VisualStudio.FSharp.LanguageService
open Microsoft.FSharp.Compiler
open Microsoft.FSharp.Compiler.Ast
open Microsoft.FSharp.Compiler.SourceCodeServices
open System.IO


[<RequireQualifiedAccess; NoComparison>] 
type SymbolDeclarationLocation = 
    | CurrentDocument
    | Projects of Project list * isLocalForProject: bool


[<NoComparison>]
type SymbolUse =
    { SymbolUse: FSharpSymbolUse 
      IsUsed: bool
      FullNames: Idents[] }


type FSharpSymbol with
    member this.IsInternalToProject =
        match this with 
        | :? FSharpParameter -> true
        | :? FSharpMemberOrFunctionOrValue as m -> not m.IsModuleValueOrMember || not m.Accessibility.IsPublic
        | :? FSharpEntity as m -> not m.Accessibility.IsPublic
        | :? FSharpGenericParameter -> true
        | :? FSharpUnionCase as m -> not m.Accessibility.IsPublic
        | :? FSharpField as m -> not m.Accessibility.IsPublic
        | _ -> false


type FSharpSymbolUse with
    member this.GetDeclarationLocation (currentDocument: Document) : SymbolDeclarationLocation option =
        if this.IsPrivateToFile then
            Some SymbolDeclarationLocation.CurrentDocument
        else
            let isSymbolLocalForProject = this.Symbol.IsInternalToProject
                
            let declarationLocation = 
                match this.Symbol.ImplementationLocation with
                | Some x -> Some x
                | None -> this.Symbol.DeclarationLocation
                
            match declarationLocation with
            | Some loc ->
                let filePath = Path.GetFullPathSafe loc.FileName
                let isScript = isScriptFile filePath 
                if isScript && filePath = currentDocument.FilePath then 
                    Some SymbolDeclarationLocation.CurrentDocument
                elif isScript then
                    // The standalone script might include other files via '#load'
                    // These files appear in project options and the standalone file 
                    // should be treated as an individual project
                    Some (SymbolDeclarationLocation.Projects ([currentDocument.Project], isSymbolLocalForProject))
                else
                    let projects =
                        currentDocument.Project.Solution.GetDocumentIdsWithFilePath(filePath)
                        |> Seq.map (fun x -> x.ProjectId)
                        |> Seq.distinct
                        |> Seq.map currentDocument.Project.Solution.GetProject
                        |> Seq.toList
                    match projects with
                    | [] -> None
                    | projects -> Some (SymbolDeclarationLocation.Projects (projects, isSymbolLocalForProject))
            | None -> None

    member this.IsPrivateToFile = 
        let isPrivate =
            match this.Symbol with
            | :? FSharpMemberOrFunctionOrValue as m -> not m.IsModuleValueOrMember || m.Accessibility.IsPrivate
            | :? FSharpEntity as m -> m.Accessibility.IsPrivate
            | :? FSharpGenericParameter -> true
            | :? FSharpUnionCase as m -> m.Accessibility.IsPrivate
            | :? FSharpField as m -> m.Accessibility.IsPrivate
            | _ -> false
            
        let declarationLocation =
            match this.Symbol.SignatureLocation with
            | Some x -> Some x
            | _ ->
                match this.Symbol.DeclarationLocation with
                | Some x -> Some x
                | _ -> this.Symbol.ImplementationLocation
            
        let declaredInTheFile = 
            match declarationLocation with
            | Some declRange -> declRange.FileName = this.RangeAlternate.FileName
            | _ -> false
            
        isPrivate && declaredInTheFile   


type FSharpMemberOrFunctionOrValue with
        
    member x.IsConstructor = x.CompiledName = ".ctor"
        
    member x.IsOperatorOrActivePattern =
        let name = x.DisplayName
        if name.StartsWith "( " && name.EndsWith " )" && name.Length > 4
        then name.Substring (2, name.Length - 4) |> String.forall (fun c -> c <> ' ')
        else false

    member x.EnclosingEntitySafe =
        try
            Some x.EnclosingEntity
        with :? InvalidOperationException -> None


type FSharpEntity with
    member x.AllBaseTypes =
        let rec allBaseTypes (entity:FSharpEntity) =
            [
                match entity.TryFullName with
                | Some _ ->
                    match entity.BaseType with
                    | Some bt ->
                        yield bt
                        if bt.HasTypeDefinition then
                            yield! allBaseTypes bt.TypeDefinition
                    | _ -> ()
                | _ -> ()
            ]
        allBaseTypes x




/// Active patterns over `FSharpSymbolUse`.
module SymbolUse =

    let (|ActivePatternCase|_|) (symbol : FSharpSymbolUse) =
        match symbol.Symbol with
        | :? FSharpActivePatternCase as ap-> ActivePatternCase(ap) |> Some
        | _ -> None

    let private attributeSuffixLength = "Attribute".Length

    let (|Entity|_|) (symbol : FSharpSymbolUse) : (FSharpEntity * (* cleanFullNames *) string list) option =
        match symbol.Symbol with
        | :? FSharpEntity as ent -> 
            // strip generic parameters count suffix (List`1 => List)
            let cleanFullName =
                // `TryFullName` for type aliases is always `None`, so we have to make one by our own
                if ent.IsFSharpAbbreviation then
                    [ent.AccessPath + "." + ent.DisplayName]
                else
                    ent.TryFullName
                    |> Option.toList
                    |> List.map (fun fullName ->
                        if ent.GenericParameters.Count > 0 && fullName.Length > 2 then
                            fullName.[0..fullName.Length - 3]
                        else fullName)
            
            let cleanFullNames =
                cleanFullName
                |> List.collect (fun cleanFullName ->
                    if ent.IsAttributeType then
                        [cleanFullName; cleanFullName.[0..cleanFullName.Length - attributeSuffixLength - 1]]
                    else [cleanFullName]
                    )
            Some (ent, cleanFullNames)
        | _ -> None


    let (|Field|_|) (symbol : FSharpSymbolUse) =
        match symbol.Symbol with
        | :? FSharpField as field-> Some field
        |  _ -> None

    let (|GenericParameter|_|) (symbol: FSharpSymbolUse) =
        match symbol.Symbol with
        | :? FSharpGenericParameter as gp -> Some gp
        | _ -> None

    let (|MemberFunctionOrValue|_|) (symbol : FSharpSymbolUse) =
        match symbol.Symbol with
        | :? FSharpMemberOrFunctionOrValue as func -> Some func
        | _ -> None

    let (|ActivePattern|_|) = function
        | MemberFunctionOrValue m when m.IsActivePattern -> Some m | _ -> None

    let (|Parameter|_|) (symbol : FSharpSymbolUse) =
        match symbol.Symbol with
        | :? FSharpParameter as param -> Some param
        | _ -> None

    let (|StaticParameter|_|) (symbol : FSharpSymbolUse) =
        match symbol.Symbol with
        | :? FSharpStaticParameter as sp -> Some sp
        | _ -> None

    let (|UnionCase|_|) (symbol : FSharpSymbolUse) =
        match symbol.Symbol with
        | :? FSharpUnionCase as uc-> Some uc
        | _ -> None

    //let (|Constructor|_|) = function
    //    | MemberFunctionOrValue func when func.IsConstructor || func.IsImplicitConstructor -> Some func
    //    | _ -> None

    let (|TypeAbbreviation|_|) = function
        | Entity (entity, _) when entity.IsFSharpAbbreviation -> Some entity
        | _ -> None

    let (|Class|_|) = function
        | Entity (entity, _) when entity.IsClass -> Some entity
        | Entity (entity, _) when entity.IsFSharp &&
            entity.IsOpaque &&
            not entity.IsFSharpModule &&
            not entity.IsNamespace &&
            not entity.IsDelegate &&
            not entity.IsFSharpUnion &&
            not entity.IsFSharpRecord &&
            not entity.IsInterface &&
            not entity.IsValueType -> Some entity
        | _ -> None

    let (|Delegate|_|) = function
        | Entity (entity, _) when entity.IsDelegate -> Some entity
        | _ -> None

    let (|Event|_|) = function
        | MemberFunctionOrValue symbol when symbol.IsEvent -> Some symbol
        | _ -> None

    let (|Property|_|) = function
        | MemberFunctionOrValue symbol when symbol.IsProperty || symbol.IsPropertyGetterMethod || symbol.IsPropertySetterMethod -> Some symbol
        | _ -> None

    let inline private notCtorOrProp (symbol:FSharpMemberOrFunctionOrValue) =
        not symbol.IsConstructor && not symbol.IsPropertyGetterMethod && not symbol.IsPropertySetterMethod

    let (|Method|_|) (symbolUse:FSharpSymbolUse) =
        match symbolUse with
        | MemberFunctionOrValue symbol when 
            symbol.IsModuleValueOrMember  &&
            not symbolUse.IsFromPattern &&
            not symbol.IsOperatorOrActivePattern &&
            not symbol.IsPropertyGetterMethod &&
            not symbol.IsPropertySetterMethod -> Some symbol
        | _ -> None

    let (|Function|_|) (symbolUse:FSharpSymbolUse) =
        match symbolUse with
        | MemberFunctionOrValue symbol when 
            notCtorOrProp symbol  &&
            symbol.IsModuleValueOrMember &&
            not symbol.IsOperatorOrActivePattern &&
            not symbolUse.IsFromPattern ->
            
            match symbol.FullTypeSafe with
            | Some fullType when fullType.IsFunctionType -> Some symbol
            | _ -> None
        | _ -> None

    let (|Operator|_|) (symbolUse:FSharpSymbolUse) =
        match symbolUse with
        | MemberFunctionOrValue symbol when 
            notCtorOrProp symbol &&
            not symbolUse.IsFromPattern &&
            not symbol.IsActivePattern &&
            symbol.IsOperatorOrActivePattern ->
            
            match symbol.FullTypeSafe with
            | Some fullType when fullType.IsFunctionType -> Some symbol
            | _ -> None
        | _ -> None

    let (|Pattern|_|) (symbolUse:FSharpSymbolUse) =
        match symbolUse with
        | MemberFunctionOrValue symbol when 
            notCtorOrProp symbol &&
            not symbol.IsOperatorOrActivePattern &&
            symbolUse.IsFromPattern ->
            
            match symbol.FullTypeSafe with
            | Some fullType when fullType.IsFunctionType ->Some symbol
            | _ -> None
        | _ -> None


    let (|ClosureOrNestedFunction|_|) = function
        | MemberFunctionOrValue symbol when 
            notCtorOrProp symbol &&
            not symbol.IsOperatorOrActivePattern &&
            not symbol.IsModuleValueOrMember ->
            
            match symbol.FullTypeSafe with
            | Some fullType when fullType.IsFunctionType -> Some symbol
            | _ -> None
        | _ -> None

    
    let (|Val|_|) = function
        | MemberFunctionOrValue symbol when notCtorOrProp symbol &&
                                            not symbol.IsOperatorOrActivePattern ->
            match symbol.FullTypeSafe with
            | Some _fullType -> Some symbol
            | _ -> None
        | _ -> None

    let (|Enum|_|) = function
        | Entity (entity, _) when entity.IsEnum -> Some entity
        | _ -> None

    let (|Interface|_|) = function
        | Entity (entity, _) when entity.IsInterface -> Some entity
        | _ -> None

    let (|Module|_|) = function
        | Entity (entity, _) when entity.IsFSharpModule -> Some entity
        | _ -> None

    let (|Namespace|_|) = function
        | Entity (entity, _) when entity.IsNamespace -> Some entity
        | _ -> None

    let (|Record|_|) = function
        | Entity (entity, _) when entity.IsFSharpRecord -> Some entity
        | _ -> None

    let (|Union|_|) = function
        | Entity (entity, _) when entity.IsFSharpUnion -> Some entity
        | _ -> None

    let (|ValueType|_|) = function
        | Entity (entity, _) when entity.IsValueType && not entity.IsEnum -> Some entity
        | _ -> None

    let (|ComputationExpression|_|) (symbol:FSharpSymbolUse) =
        if symbol.IsFromComputationExpression then Some symbol
        else None
        
    let (|Attribute|_|) = function
        | Entity (entity, _) when entity.IsAttributeType -> Some entity
        | _ -> None