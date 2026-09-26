[<AutoOpen>]
module internal Microsoft.VisualStudio.FSharp.Editor.Symbols

open System
open System.IO
open Microsoft.CodeAnalysis
open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.EditorServices
open FSharp.Compiler.Symbols

[<RequireQualifiedAccess; NoComparison>]
type SymbolScope =
    | CurrentDocument
    | SignatureAndImplementation
    | Projects of Project list * isLocalForProject: bool

[<NoComparison>]
type SymbolUse =
    {
        SymbolUse: FSharpSymbolUse
        IsUsed: bool
        FullNames: ShortIdent[]
    }

type FSharpSymbol with

    member this.IsInternalToProject =
        let publicOrInternal = this.Accessibility.IsPublic || this.Accessibility.IsInternal

        match this with
        | :? FSharpParameter -> true
        | :? FSharpMemberOrFunctionOrValue as m -> not m.IsModuleValueOrMember || not publicOrInternal
        | :? FSharpEntity -> not publicOrInternal
        | :? FSharpGenericParameter -> true
        | :? FSharpUnionCase -> not publicOrInternal
        | :? FSharpField -> not publicOrInternal
        | _ -> false

type FSharpSymbolUse with

    member this.GetSymbolScope(currentDocument: Document) : SymbolScope option =
        if this.IsPrivateToFile then
            Some SymbolScope.CurrentDocument
        elif this.IsPrivateToFileAndSignatureFile then
            Some SymbolScope.SignatureAndImplementation
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
                    Some SymbolScope.CurrentDocument
                elif isScript then
                    // The standalone script might include other files via '#load'
                    // These files appear in project options and the standalone file
                    // should be treated as an individual project
                    Some(SymbolScope.Projects([ currentDocument.Project ], isSymbolLocalForProject))
                else
                    let solution = currentDocument.Project.Solution

                    let projects =
                        solution.GetDocumentIdsWithFSharpFileName loc.FileName
                        |> Seq.map (fun x -> x.ProjectId)
                        |> Seq.distinct
                        |> Seq.map solution.GetProject
                        |> Seq.toList

                    let projects =
                        if isRootedPath loc.FileName then
                            projects
                        else
                            // A name a path map left relative is matched by its tail, which a file of an
                            // unrelated project can share. Only a project that compiles the symbol's own
                            // assembly may narrow the search; where none does, the search keeps the scope it
                            // would have had before the name reached a document at all.
                            let declaringAssembly = this.Symbol.Assembly.SimpleName

                            projects
                            |> List.filter (fun project ->
                                String.Equals(project.AssemblyName, declaringAssembly, StringComparison.OrdinalIgnoreCase))

                    match projects with
                    | [] -> None
                    | projects -> Some(SymbolScope.Projects(projects, isSymbolLocalForProject))
            | None -> None

type FSharpEntity with

    member x.AllBaseTypes =
        let rec allBaseTypes (entity: FSharpEntity) =
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
