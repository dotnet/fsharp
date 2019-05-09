namespace FSharp.Compiler.Compilation

open System
open System.IO
open System.Threading
open System.Threading.Tasks
open System.Collections.Immutable
open System.Collections.Generic
open System.Collections.Concurrent
open Internal.Utilities.Collections
open FSharp.Compiler
open FSharp.Compiler.Infos
open FSharp.Compiler.AbstractIL.Internal.Library
open FSharp.Compiler.Ast
open FSharp.Compiler.Tastops
open FSharp.Compiler.CompileOps
open FSharp.Compiler.Range
open FSharp.Compiler.Driver
open FSharp.Compiler.Tast
open FSharp.Compiler.TcGlobals
open FSharp.Compiler.AbstractIL.ILBinaryReader
open FSharp.Compiler.ErrorLogger
open FSharp.Compiler.CompileOptions
open FSharp.Compiler.TypeChecker
open FSharp.Compiler.NameResolution
open Internal.Utilities
open FSharp.Compiler.Compilation.Utilities

[<AutoOpen>]
module rec SymbolHelpers =

    type Table<'Key, 'T when 'Key : equality> () =
        let mutable nextId = 1u
        let idCache = Dictionary<'Key, uint32> ()
        let itemCache = Dictionary<uint32, 'T> ()

        member __.TryGetCachedItem key =
            match idCache.TryGetValue key with
            | true, id ->
                match itemCache.TryGetValue id with
                | true, item -> ValueSome (struct (id, item))
                | _ -> failwith "inconsistent"
            | _ -> ValueNone

        member this.SetCachedItem (key, item) =
            match this.TryGetCachedItem key with
            | ValueSome struct (id, _) -> id
            | _ ->
                let id = nextId
                nextId <- nextId + 1u
                idCache.Add (key, id)
                itemCache.Add (id, item)
                id    
                
    let globalTypeTable = Table<string, Lazy<TypeSymbol>> ()

    [<Sealed>]
    type cenv (senv: SymbolEnv) =

        member val GlobalTypeTable = globalTypeTable

        member val TypeTable = Table<string, Lazy<TypeSymbol>> ()

        member val AssemblyTable = Table<string, AssemblyOrModuleSymbol> ()

        member val MemberMethodTable = Table<struct (string * string), MemberMethodSymbol> ()
        
        member __.senv = senv

    let rec createTypeParameters (typars: Typars) =
        typars
        |> List.map (fun x -> { Name = x.Id.idText; Range = x.Id.idRange; Constraints = ImmutableArray.Empty })
        |> ImmutableArray.CreateRange

    let rec createAccessibility cenv (accessibility: Accessibility) =
        match accessibility with
        | TAccess [] -> ImmutableArray.Empty
        | TAccess compPaths ->
            compPaths
            |> List.map (fun x -> createAssemblyOrModuleSymbol cenv x.ILScopeRef)
            |> ImmutableArray.CreateRange

    let rec tryCreateMemberMethodFromValRef (cenv: cenv) (vref: ValRef) =
        match vref.IsMember, vref.DeclaringEntity with
        | true, Parent entity when entity.IsFSharpObjectModelTycon ->
            let key = struct (entity.CompiledRepresentationForNamedType.BasicQualifiedName, vref.CompiledName)
            match cenv.MemberMethodTable.TryGetCachedItem key with
            | ValueSome struct (memberMethodSymbolId, _) -> Some memberMethodSymbolId
            | _ ->
                let _, ty = generalizeTyconRef entity
                let methInfo = FSMeth(cenv.senv.g, ty, vref, None)
                let accessibility = createAccessibility cenv vref.Accessibility

                let memberMethodSymbol =
                    {
                        Name = methInfo.DisplayName
                        TypeParameters = createTypeParameters methInfo.FormalMethodTypars
                        Parameters = ImmutableArray.Empty
                        ReturnType = createTypeSymbol cenv (methInfo.GetFSharpReturnTy(cenv.senv.amap, Range.range0, methInfo.FormalMethodInst))
                        IsInstance = methInfo.IsInstance
                        DeclaringType = createTypeSymbolFromTyconRef cenv methInfo.DeclaringTyconRef
                        Accessibility = accessibility
                    }
                let memberMethodSymbolId = cenv.MemberMethodTable.SetCachedItem (key, memberMethodSymbol)
                Some memberMethodSymbolId

        | _ -> None

    let rec createAssemblyOrModuleSymbol (cenv: cenv) (scope: AbstractIL.IL.ILScopeRef) =
        match cenv.AssemblyTable.TryGetCachedItem scope.QualifiedName with
        | ValueSome (assemblyOrModuleId, _) -> assemblyOrModuleId
        | _ ->
            let assemblyOrModuleSymbol = 
                match scope with
                | AbstractIL.IL.ILScopeRef.Local -> { Name = cenv.senv.thisCcu.AssemblyName }
                | AbstractIL.IL.ILScopeRef.Assembly asmRef -> { Name = asmRef.Name }
                | AbstractIL.IL.ILScopeRef.Module modRef -> { Name = modRef.Name }
            cenv.AssemblyTable.SetCachedItem (assemblyOrModuleSymbol.Name, assemblyOrModuleSymbol)

    let rec createTypeSymbolFromTyconRef (cenv: cenv) (tcref: TyconRef) =
        let qualifiedName = tcref.CompiledRepresentationForNamedType.BasicQualifiedName

        match cenv.TypeTable.TryGetCachedItem qualifiedName with
        | ValueSome (struct (typeSymbolId, _)) -> typeSymbolId
        | _ ->
            let typeSymbol =
                lazy
                    let assemblyOrModuleSymbolId = createAssemblyOrModuleSymbol cenv tcref.CompiledRepresentationForNamedType.Scope
                    let typeParameters = createTypeParameters (tcref.Typars Range.range0)
                    let _, ty = generalizeTyconRef tcref
                    let pointsToTypeSymbolId = createTypeSymbol cenv (stripTyEqns cenv.senv.g ty)

                    let accessibility = createAccessibility cenv tcref.Accessibility

                    if tcref.IsTypeAbbrev then
                        {
                            QualifiedName = qualifiedName
                            AssemblyOrModule = assemblyOrModuleSymbolId
                            TypeParameters = typeParameters
                            Type = pointsToTypeSymbolId
                            Accessibility = accessibility
                        } |> TypeSymbol.Abbreviation
                    else
                        let memberMethodSymbolIds =
                            if tcref.IsFSharpObjectModelTycon then
                                tcref.MembersOfFSharpTyconSorted
                                |> List.choose (fun x -> tryCreateMemberMethodFromValRef cenv x)
                            else
                                []

                        {
                            QualifiedName = qualifiedName
                            AssemblyOrModule = assemblyOrModuleSymbolId
                            TypeParameters = typeParameters
                            MemberMethods = memberMethodSymbolIds |> ImmutableArray.CreateRange
                            Accessibility = accessibility
                        } |> TypeSymbol.Named

            let typeSymbolId = cenv.TypeTable.SetCachedItem (qualifiedName, typeSymbol)
            typeSymbol.Force() |> ignore
            typeSymbolId

    let rec createTypeSymbol (cenv: cenv) (ty: TType) =
        match stripTyparEqns ty with
        | TType.TType_app (tcref, _tyargs) ->
            createTypeSymbolFromTyconRef cenv tcref

        | TType.TType_forall (typars, ty) ->
            0u

        | TType.TType_fun (ty1, ty2) ->
            0u

        | _ -> 0u

[<Sealed>]
type SemanticModel (filePath, asyncLazyChecker: AsyncLazy<IncrementalChecker>) =

    let lookup = Dictionary()

    let mutable storage = None

    let asyncLazyGetAllSymbols =
        AsyncLazy(async {
            let! checker = asyncLazyChecker.GetValueAsync ()
            let! _tcAcc, result = checker.CheckAsync filePath
            match result with
            | None -> return None
            | Some (tcResolutions, senv) ->

                let cenv = cenv (senv)

                let s = System.Diagnostics.Stopwatch.StartNew()
                let symbols =
                    tcResolutions.CapturedNameResolutions
                    |> Seq.choose (fun cnr ->
                        match cnr.Item with
                        | Item.Value vref when not vref.IsConstructor ->
                            tryCreateMemberMethodFromValRef cenv vref
                        | _ -> None
                    )
                    |> Seq.toList

                s.Stop()

                printfn "Time: %A ms" s.Elapsed.TotalMilliseconds
                storage <- Some symbols

                return None
        })

    member __.TryFindSymbolAsync (line: int, column: int) : Async<Symbol option> =
        async {
            match! asyncLazyGetAllSymbols.GetValueAsync () with
            | None -> return None
            | Some linesSymbol -> return None      
        }
