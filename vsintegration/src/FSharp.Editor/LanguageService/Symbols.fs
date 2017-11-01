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