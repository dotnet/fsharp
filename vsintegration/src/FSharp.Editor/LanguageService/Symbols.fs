[<AutoOpen>]
module internal Microsoft.VisualStudio.FSharp.Editor.Symbols

open System.IO
open Microsoft.CodeAnalysis
open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.EditorServices
open FSharp.Compiler.Symbols

[<RequireQualifiedAccess; NoComparison>] 
type SymbolDeclarationLocation = 
    | CurrentDocument
    | Projects of Project list * isLocalForProject: bool

[<NoComparison>]
type SymbolUse =
    { SymbolUse: FSharpSymbolUse 
      IsUsed: bool
      FullNames: ShortIdent[] }

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