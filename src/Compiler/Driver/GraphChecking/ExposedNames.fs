module internal FSharp.Compiler.GraphChecking.ExposedNames

open System
open System.Collections.Generic
open FSharp.Compiler.Syntax
open FSharp.Compiler.Syntax.PrettyNaming

// Any attribute can be `Extension` under another name.
let private mayBeExtension (attributes: SynAttributes) = not attributes.IsEmpty

let private isAutoOpen = TrieMapping.isAnyAttributeAutoOpen

let private attributeSuffix = "Attribute"

let private isOperatorName (name: string) =
    name.StartsWith(opNamePrefix, StringComparison.Ordinal)

let private isNumericLiteralModuleName (name: string) =
    name.StartsWith("NumericLiteral", StringComparison.Ordinal)

let private isFoundWithoutItsArity (name: string) =
    (TryDemangleGenericNameAndPos name).IsSome

let private namesUsedByExternDeclarations =
    set [ "unit"; "nativeint"; "nativeptr"; "byref"; "failwith" ]

let private namespaceNamesUsedByCompiler = set [ "System"; "Microsoft" ]

let private isImplicitlyUsedName (name: string) =
    isOperatorName name
    || isNumericLiteralModuleName name
    || isFoundWithoutItsArity name
    || namesUsedByExternDeclarations.Contains name

let tryGet (input: ParsedInput) : Set<Identifier> voption =
    let names = HashSet<Identifier>()
    let isUsedByKnownNamesOnly = ref true
    let markUsedWithoutName () = isUsedByKnownNamesOnly.Value <- false
    let markHasUnknownNames () = isUsedByKnownNamesOnly.Value <- false

    let addName (ident: Ident) =
        names.Add ident.idText |> ignore

        if isImplicitlyUsedName ident.idText then
            markUsedWithoutName ()

    let addTypeName (SynComponentInfo(typeParams = typars) as info) =
        for ident in info.LongIdent do
            addName ident
            let name = ident.idText

            if name.EndsWith(attributeSuffix, StringComparison.Ordinal) then
                let nameWithoutAttributeSuffix =
                    name.Substring(0, name.Length - attributeSuffix.Length)

                names.Add nameWithoutAttributeSuffix |> ignore

            match typars with
            | Some typars ->
                let nameWithArity = $"{name}`{typars.TyparDecls.Length}"
                names.Add nameWithArity |> ignore
            | None -> ()

    let visitStaticMember (typeAttributes: SynAttributes) (attributes: SynAttributes) (addMemberName: unit -> unit) =
        if mayBeExtension attributes then
            markUsedWithoutName ()
        elif isAutoOpen typeAttributes then
            addMemberName ()

    let visitBinding typeAttributes (SynBinding(attributes = attributes; headPat = pat; valData = SynValData(memberFlags = flags))) =
        if not (flags |> Option.exists _.IsInstance) then
            visitStaticMember typeAttributes attributes (fun () ->
                match pat with
                | SynPat.LongIdent(longDotId = SynLongIdent(id = idents)) -> List.iter addName idents
                | SynPat.Named(ident = SynIdent(ident, _)) -> addName ident
                | _ -> markHasUnknownNames ())

    let visitValSig typeAttributes (SynValSig(attributes = attributes; ident = SynIdent(ident, _))) (flags: SynMemberFlags) =
        if not flags.IsInstance then
            visitStaticMember typeAttributes attributes (fun () -> addName ident)

    let visitField typeAttributes (SynField(attributes = attributes; isStatic = isStatic; idOpt = ident)) =
        if isStatic then
            visitStaticMember typeAttributes attributes (fun () -> Option.iter addName ident)

    let visitBaseType typeAttributes =
        if isAutoOpen typeAttributes then
            markHasUnknownNames ()

    let rec visitMember typeAttributes (memberDefn: SynMemberDefn) =
        match memberDefn with
        | SynMemberDefn.Member(memberDefn = binding) -> visitBinding typeAttributes binding
        | SynMemberDefn.GetSetMember(memberDefnForGet = getter; memberDefnForSet = setter) ->
            Option.iter (visitBinding typeAttributes) getter
            Option.iter (visitBinding typeAttributes) setter
        | SynMemberDefn.AutoProperty(attributes = attributes; isStatic = isStatic; ident = ident) ->
            if isStatic then
                visitStaticMember typeAttributes attributes (fun () -> addName ident)
        | SynMemberDefn.AbstractSlot(slotSig = valSig; flags = flags) -> visitValSig typeAttributes valSig flags
        | SynMemberDefn.ValField(fieldInfo = field) -> visitField typeAttributes field
        | SynMemberDefn.Interface(members = members) -> members |> Option.iter (List.iter (visitMember typeAttributes))
        | SynMemberDefn.ImplicitInherit _
        | SynMemberDefn.Inherit _ -> visitBaseType typeAttributes
        | _ -> ()

    let visitMemberSig typeAttributes (memberSig: SynMemberSig) =
        match memberSig with
        | SynMemberSig.Member(memberSig = valSig; flags = flags) -> visitValSig typeAttributes valSig flags
        | SynMemberSig.ValField(field = field) -> visitField typeAttributes field
        | SynMemberSig.Inherit _ -> visitBaseType typeAttributes
        | _ -> ()

    let visitUnionCase (SynUnionCase(ident = SynIdent(ident, _))) = addName ident

    let visitSimpleRepr typeAttributes (repr: SynTypeDefnSimpleRepr) =
        match repr with
        | SynTypeDefnSimpleRepr.Union(unionCases = cases) -> List.iter visitUnionCase cases
        | SynTypeDefnSimpleRepr.Record(recordFieldsAndSpreads = fields) ->
            for field in fields do
                match field with
                | SynFieldOrSpread.Field(SynField(idOpt = ident)) -> Option.iter addName ident
                | SynFieldOrSpread.Spread _ -> markHasUnknownNames ()
        | SynTypeDefnSimpleRepr.Enum(cases = cases) ->
            if isAutoOpen typeAttributes then
                for SynEnumCase(ident = SynIdent(ident, _)) in cases do
                    addName ident
        | SynTypeDefnSimpleRepr.TypeAbbrev _ ->
            if isAutoOpen typeAttributes then
                markHasUnknownNames ()
        | _ -> ()

    let visitType (SynTypeDefn(typeInfo = SynComponentInfo(attributes = typeAttributes) as info; typeRepr = repr; members = members)) =
        addTypeName info
        List.iter (visitMember typeAttributes) members

        match repr with
        | SynTypeDefnRepr.ObjectModel(kind = SynTypeDefnKind.Augmentation _) -> markUsedWithoutName ()
        | SynTypeDefnRepr.ObjectModel(members = members) -> List.iter (visitMember typeAttributes) members
        | SynTypeDefnRepr.Simple(simpleRepr = repr) -> visitSimpleRepr typeAttributes repr
        | SynTypeDefnRepr.Exception _ -> markHasUnknownNames ()

    let visitTypeSig
        (SynTypeDefnSig(typeInfo = SynComponentInfo(attributes = typeAttributes) as info; typeRepr = repr; members = members))
        =
        addTypeName info
        List.iter (visitMemberSig typeAttributes) members

        match repr with
        | SynTypeDefnSigRepr.ObjectModel(kind = SynTypeDefnKind.Augmentation _) -> markUsedWithoutName ()
        | SynTypeDefnSigRepr.ObjectModel(memberSigs = members) -> List.iter (visitMemberSig typeAttributes) members
        | SynTypeDefnSigRepr.Simple(repr = repr) -> visitSimpleRepr typeAttributes repr
        | SynTypeDefnSigRepr.Exception _ -> markHasUnknownNames ()

    let visitModule (SynComponentInfo(attributes = attributes) as info) =
        List.iter addName info.LongIdent

        if isAutoOpen attributes then
            markUsedWithoutName ()

    let visitDecl (decl: SynModuleDecl) =
        match decl with
        | SynModuleDecl.Open _
        | SynModuleDecl.HashDirective _ -> ()
        | SynModuleDecl.Types(typeDefns = typeDefns) -> List.iter visitType typeDefns
        | SynModuleDecl.Exception(
            exnDefn = SynExceptionDefn(exnRepr = SynExceptionDefnRepr(attributes = attributes; caseName = case); members = members)) ->
            visitUnionCase case
            List.iter (visitMember attributes) members
        | SynModuleDecl.NestedModule(moduleInfo = info) -> visitModule info
        | _ -> markHasUnknownNames ()

    let visitSigDecl (decl: SynModuleSigDecl) =
        match decl with
        | SynModuleSigDecl.Open _
        | SynModuleSigDecl.HashDirective _ -> ()
        | SynModuleSigDecl.Types(types = types) -> List.iter visitTypeSig types
        | SynModuleSigDecl.Exception(
            exnSig = SynExceptionSig(exnRepr = SynExceptionDefnRepr(attributes = attributes; caseName = case); members = members)) ->
            visitUnionCase case
            List.iter (visitMemberSig attributes) members
        | SynModuleSigDecl.NestedModule(moduleInfo = info) -> visitModule info
        | _ -> markHasUnknownNames ()

    let visitNamespace (kind: SynModuleOrNamespaceKind) (longId: LongIdent) =
        let isUsedByCompiler =
            longId
            |> List.exists (fun ident -> namespaceNamesUsedByCompiler.Contains ident.idText)

        if kind <> SynModuleOrNamespaceKind.DeclaredNamespace || isUsedByCompiler then
            markUsedWithoutName ()

    match input with
    | ParsedInput.ImplFile(ParsedImplFileInput(contents = contents)) ->
        for SynModuleOrNamespace(longId = longId; kind = kind; decls = decls) in contents do
            visitNamespace kind longId
            List.iter visitDecl decls
    | ParsedInput.SigFile(ParsedSigFileInput(contents = contents)) ->
        for SynModuleOrNamespaceSig(longId = longId; kind = kind; decls = decls) in contents do
            visitNamespace kind longId
            List.iter visitSigDecl decls

    if isUsedByKnownNamesOnly.Value then
        ValueSome(Set.ofSeq names)
    else
        ValueNone
