module internal Fsharp.Compiler.SignatureHash

open System
open System.Globalization
open System.IO
open Internal.Utilities.Collections
open Internal.Utilities.Library
open Internal.Utilities.Library.Extras
open Internal.Utilities.Rational
open FSharp.Compiler 
open FSharp.Compiler.AbstractIL.IL 
open FSharp.Compiler.AccessibilityLogic
open FSharp.Compiler.AttributeChecking
open FSharp.Compiler.DiagnosticsLogger
open FSharp.Compiler.Infos
open FSharp.Compiler.InfoReader
open FSharp.Compiler.Syntax
open FSharp.Compiler.Syntax.PrettyNaming
open FSharp.Compiler.TcGlobals
open FSharp.Compiler.Text
open FSharp.Compiler.Text.Layout
open FSharp.Compiler.Text.LayoutRender
open FSharp.Compiler.Text.TaggedText
open FSharp.Compiler.TypedTree
open FSharp.Compiler.TypedTreeBasics
open FSharp.Compiler.TypedTreeOps
open FSharp.Compiler.TypeHierarchy
open FSharp.Compiler.Xml



(*
val layoutImpliedSignatureOfModuleOrNamespace:
    showHeader: bool ->
    denv: DisplayEnv ->
    infoReader: InfoReader ->
    ad: AccessorDomain ->
    m: range ->
    contents: ModuleOrNamespaceContents ->
        Layout

*)

let inline combineHash x y = (x <<< 1) + y + 631
let inline addToHash (s:string) (acc:int) = combineHash acc (hash s)

/// Layout the inferred signature of a compilation unit
let calculateHashOfImpliedSignature (infoReader:InfoReader) (ad:AccessorDomain) (m:range) (expr:ModuleOrNamespaceContents) =

    let rec isConcreteNamespace x = 
        match x with 
        | TMDefRec(_, _opens, tycons, mbinds, _) -> 
            not (isNil tycons) || (mbinds |> List.exists (function ModuleOrNamespaceBinding.Binding _ -> true | ModuleOrNamespaceBinding.Module(x, _) -> not x.IsNamespace))
        | TMDefLet _ -> true
        | TMDefDo _ -> true
        | TMDefOpens _ -> false
        | TMDefs defs -> defs |> List.exists isConcreteNamespace 

    let rec imdefsL denv x = aboveListL (x |> List.map (imdefL denv))

    and imdefL denv x = 
        let filterVal (v: Val) = not v.IsCompilerGenerated && Option.isNone v.MemberInfo
        let filterExtMem (v: Val) = v.IsExtensionMember

        match x with 
        | TMDefRec(_, _opens, tycons, mbinds, _) -> 
            TastDefinitionPrinting.layoutTyconDefns denv infoReader ad m tycons @@ 
            (mbinds 
                |> List.choose (function ModuleOrNamespaceBinding.Binding bind -> Some bind | _ -> None) 
                |> valsOfBinds 
                |> List.filter filterExtMem
                |> List.map mkLocalValRef
                |> TastDefinitionPrinting.layoutExtensionMembers denv infoReader) @@

            (mbinds 
                |> List.choose (function ModuleOrNamespaceBinding.Binding bind -> Some bind | _ -> None) 
                |> valsOfBinds 
                |> List.filter filterVal
                |> List.map mkLocalValRef
                |> List.map (PrintTastMemberOrVals.prettyLayoutOfValOrMemberNoInst denv infoReader)
                |> aboveListL) @@

            (mbinds 
                |> List.choose (function ModuleOrNamespaceBinding.Module (mspec, def) -> Some (mspec, def) | _ -> None) 
                |> List.map (imbindL denv) 
                |> aboveListL)

        | TMDefLet(bind, _) -> 
            ([bind.Var] 
                |> List.filter filterVal 
                |> List.map mkLocalValRef
                |> List.map (PrintTastMemberOrVals.prettyLayoutOfValOrMemberNoInst denv infoReader) 
                |> aboveListL)

        | TMDefOpens _ -> emptyL

        | TMDefs defs -> imdefsL denv defs

        | TMDefDo _ -> emptyL

    and imbindL denv (mspec, def) = 
        let innerPath = (fullCompPathOfModuleOrNamespace mspec).AccessPath
        let outerPath = mspec.CompilationPath.AccessPath

        let denv =
            if not (isConcreteNamespace def) then
                denv
            else
                denv.AddOpenPath (List.map fst innerPath)

        if mspec.IsImplicitNamespace then
            // The current mspec is a namespace that belongs to the `def` child (nested) module(s).                
            let fullModuleName, def, denv =
                let rec (|NestedModule|_|) (currentContents:ModuleOrNamespaceContents) =
                    match currentContents with
                    | ModuleOrNamespaceContents.TMDefRec (bindings = [ ModuleOrNamespaceBinding.Module(mn, NestedModule(path, contents)) ]) ->
                        Some ([ yield mn.DisplayNameCore; yield! path ], contents)
                    | ModuleOrNamespaceContents.TMDefs [ ModuleOrNamespaceContents.TMDefRec (bindings = [ ModuleOrNamespaceBinding.Module(mn, NestedModule(path, contents)) ]) ] ->
                        Some ([ yield mn.DisplayNameCore; yield! path ], contents)
                    | ModuleOrNamespaceContents.TMDefs [ ModuleOrNamespaceContents.TMDefRec (bindings = [ ModuleOrNamespaceBinding.Module(mn, nestedModuleContents) ]) ] ->
                        Some ([ mn.DisplayNameCore ], nestedModuleContents)
                    | _ ->
                        None

                match def with
                | NestedModule(path, nestedModuleContents) ->
                    let fullPath = mspec.DisplayNameCore :: path
                    fullPath, nestedModuleContents, denv.AddOpenPath(fullPath)
                | _ -> [ mspec.DisplayNameCore ], def, denv
                
            let nmL = List.map (tagModule >> wordL) fullModuleName |> sepListL SepL.dot
            let nmL = layoutAccessibility denv mspec.Accessibility nmL
            let denv = denv.AddAccessibility mspec.Accessibility
            let basic = imdefL denv def
            let modNameL = wordL (tagKeyword "module") ^^ nmL
            let basicL = modNameL @@ basic
            layoutXmlDoc denv true mspec.XmlDoc basicL
        elif mspec.IsNamespace then
            let basic = imdefL denv def
            let basicL =
                // Check if this namespace contains anything interesting
                if isConcreteNamespace def then
                    let pathL = innerPath |> List.map (fst >> ConvertLogicalNameToDisplayLayout (tagNamespace >> wordL))
                    // This is a container namespace. We print the header when we get to the first concrete module.
                    let headerL =
                        wordL (tagKeyword "namespace") ^^ sepListL SepL.dot pathL
                    headerL @@* basic
                else
                    // This is a namespace that only contains namespaces. Skip the header
                    basic
            // NOTE: explicitly not calling `layoutXmlDoc` here, because even though
            // `ModuleOrNamespace` has a field for XmlDoc, it is never present at the parser
            // level for namespaces.  This should be changed if the parser/spec changes.
            basicL
        else
            // This is a module 
            let nmL = ConvertLogicalNameToDisplayLayout (tagModule >> mkNav mspec.DefinitionRange >> wordL) mspec.DisplayNameCore
            let nmL = layoutAccessibility denv mspec.Accessibility nmL
            let denv = denv.AddAccessibility mspec.Accessibility
            let basic = imdefL denv def
            let modNameL =
                wordL (tagKeyword "module") ^^ nmL
                |> layoutAttribs denv None false mspec.TypeOrMeasureKind mspec.Attribs
            let modNameEqualsL = modNameL ^^ WordL.equals
            let isNamespace = function | Namespace _ -> true | _ -> false
            let modIsOuter = (outerPath |> List.forall (fun (_, istype) -> isNamespace istype) )
            let basicL =
                // Check if its an outer module or a nested module
                if modIsOuter then
                    // OK, this is an outer module
                    if showHeader then
                        // OK, we're not in F# Interactive
                        // Check if this is an outer module with no namespace
                        if isNil outerPath then
                            // If so print a "module" declaration, no indentation
                            modNameL @@ basic
                        else
                            // Otherwise this is an outer module contained immediately in a namespace
                            // We already printed the namespace declaration earlier. So just print the
                            // module now.
                            if isEmptyL basic then 
                                modNameEqualsL ^^ WordL.keywordBegin ^^ WordL.keywordEnd
                            else
                                modNameEqualsL @@* basic
                    else
                        // OK, we're in F# Interactive, presumably the implicit module for each interaction.
                        basic
                else
                    // OK, this is a nested module, with indentation
                    if isEmptyL basic then 
                        ((modNameEqualsL ^^ wordL (tagKeyword"begin")) @@* basic) @@ WordL.keywordEnd
                    else
                        modNameEqualsL @@* basic
            layoutXmlDoc denv true mspec.XmlDoc basicL

    let emptyModuleOrNamespace mspec =
        let innerPath = (fullCompPathOfModuleOrNamespace mspec).AccessPath
        let pathL = innerPath |> List.map (fst >> ConvertLogicalNameToDisplayLayout (tagNamespace >> wordL))

        let keyword =
            if not mspec.IsImplicitNamespace && mspec.IsNamespace then
                "namespace"
            else
                "module"

        wordL (tagKeyword keyword) ^^ sepListL SepL.dot pathL

    match expr with
    | EmptyModuleOrNamespaces mspecs when showHeader ->
        List.map emptyModuleOrNamespace mspecs
        |> aboveListL
    | expr -> imdefL denv expr

//--------------------------------------------------------------------------
