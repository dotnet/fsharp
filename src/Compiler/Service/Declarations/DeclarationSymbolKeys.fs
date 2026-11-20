// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

/// Text keys for symbols that hold across copies: a key reads names and types, never stamps, ranges or which object.
module internal FSharp.Compiler.CodeAnalysis.Declarations.DeclarationSymbolKeys

open System
open System.Collections.Generic
open System.Threading

open FSharp.Compiler
open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.NameResolution
open FSharp.Compiler.Symbols
open FSharp.Compiler.Syntax
open FSharp.Compiler.Text
open FSharp.Compiler.TypedTree
open FSharp.Compiler.TypedTreeBasics
open FSharp.Compiler.TypedTreeOps

let private propertyKind = "property"
let private setterSuffix = "|setter"
let private entityPath (e: FSharpEntity) = e.AccessPath + "." + e.LogicalName

/// Types with abbreviations stripped and type variables numbered, as a signature may spell either differently from
/// its implementation. The value's own variables are numbered in their order first, so a class's `'a` and `'b` stay
/// apart in `M(x: 'a)` and `M(x: 'b)`. What this doesn't know prints the same `_`: two overloads it can't tell apart
/// get one key, and a lookup that finds two definitions answers nothing.
let private typesText (cenv: SymbolEnv) (own: Typar list) (types: TType list) =
    let typars = Dictionary<string, int>()

    for tp in own do
        typars[tp.Name] <- typars.Count

    let rec text (t: FSharpType) =
        let t = t.StripAbbreviations()

        let arguments (separator: string) =
            t.GenericArguments |> Seq.map text |> String.concat separator

        if t.IsGenericParameter then
            let name = t.GenericParameter.Name

            match typars.TryGetValue name with
            | true, i -> $"'%d{i}"
            | _ ->
                typars[name] <- typars.Count
                $"'%d{typars.Count - 1}"
        elif t.IsFunctionType then
            "(" + arguments "->" + ")"
        elif t.IsTupleType then
            "(" + arguments (if t.IsStructTupleType then "#" else "*") + ")"
        elif t.IsAnonRecordType then
            "{"
            + String.concat "," t.AnonRecordTypeDetails.SortedFieldNames
            + ":"
            + arguments ","
            + "}"
        elif t.HasTypeDefinition then
            entityPath t.TypeDefinition + "<" + arguments "," + ">"
        else
            "_"

    types |> List.map (fun t -> text (FSharpType(cenv, t))) |> String.concat ";"

let private ofEntity (assembly: string) (e: FSharpEntity) =
    let kind =
        if e.IsNamespace then "namespace"
        elif e.IsFSharpModule then "module"
        else "type"

    $"%s{assembly}|%s{kind}|%s{entityPath e}"

let private ofVal (cenv: SymbolEnv) (assembly: string) (v: ValRef) =
    let parent =
        match v.TryDeclaringEntity with
        | Parent p -> entityPath (FSharpEntity(cenv, p))
        | ParentNone -> ""

    match v.MemberInfo with
    | None -> $"%s{assembly}|value|%s{parent}|%s{v.LogicalName}"
    // A union's `IsCase` isn't marked as generated, and is declared at its case's name
    | Some _ when
        v.MemberApparentEntity.IsUnionTycon
        && v.MemberApparentEntity.UnionCasesArray
           |> Array.exists (fun c -> Range.equals c.Range v.Range)
        ->
        let case =
            v.MemberApparentEntity.UnionCasesArray
            |> Array.find (fun c -> Range.equals c.Range v.Range)

        $"%s{assembly}|case|%s{entityPath (FSharpEntity(cenv, v.MemberApparentEntity))}|%s{case.DisplayName}"
    // What the compiler generated has no definition of its own: its location is its type's, as is a union's `Tag`'s
    | Some _ when
        v.IsCompilerGenerated
        || (v.MemberApparentEntity.IsUnionTycon
            && Range.equals v.Range v.MemberApparentEntity.Range)
        ->
        ofEntity assembly (FSharpEntity(cenv, v.MemberApparentEntity))
    | Some info ->
        let arguments = ArgInfosOfMember cenv.g v |> List.concat |> List.map fst
        let apparent = entityPath (FSharpEntity(cenv, v.MemberApparentEntity))

        let types = typesText cenv v.Typars

        // An interface's implementation can have the name and arguments of a member of the type, or of another
        // interface's: which interface tells them apart. A signature never lists one. It does list an override or a
        // default, with no slots of its own, so what a class's slot says can't be part of the key. An abstract slot
        // and its default have one signature, and are told apart by being the slot.
        let implements =
            info.ImplementedSlotSigs
            |> List.filter (fun slot -> isInterfaceTy cenv.g slot.DeclaringType)
            |> List.map (fun slot -> slot.Name + ":" + types [ slot.DeclaringType ])
            |> List.sort
            |> String.concat ","

        let role =
            if info.MemberFlags.IsDispatchSlot then
                "slot"
            else
                implements

        let start (kind: string) (name: string) =
            $"%s{assembly}|%s{kind}|%s{parent}|%s{apparent}|%s{name}|%b{v.IsInstanceMember}|%d{v.Typars.Length}|%s{role}"

        match info.MemberFlags.MemberKind with
        | SynMemberKind.PropertyGet -> start propertyKind v.PropertyName + "|" + types arguments
        // What it sets comes last: the index before that is what tells a property's overloads apart
        | SynMemberKind.PropertySet ->
            start propertyKind v.PropertyName
            + "|"
            + types (List.take (arguments.Length - 1) arguments)
        | _ ->
            // Conversions overload on what they return
            let compared =
                if v.LogicalName = "op_Explicit" || v.LogicalName = "op_Implicit" then
                    [ v.Type ]
                else
                    arguments

            start "member" v.LogicalName + "|" + types compared

let private ofSymbol (symbol: FSharpSymbol) : string option =
    let assembly () = symbol.Assembly.SimpleName

    try
        match symbol with
        | :? FSharpEntity as e -> Some(ofEntity (assembly ()) e)
        | :? FSharpUnionCase as c -> Some $"%s{assembly ()}|case|%s{entityPath c.DeclaringEntity}|%s{c.Name}"
        | :? FSharpField as f when f.IsAnonRecordField -> None
        | :? FSharpField as f ->
            match f.DeclaringUnionCase, f.DeclaringEntity with
            | Some c, _ -> Some $"%s{assembly ()}|field|%s{entityPath c.DeclaringEntity}|%s{c.Name}|%s{f.Name}"
            | None, Some e -> Some $"%s{assembly ()}|field|%s{entityPath e}|%s{f.Name}"
            | None, None -> None
        // A case goes to its pattern's value, as FCS's location does. One that names no value is inside its definition.
        | :? FSharpActivePatternCase as c ->
            match c.Item with
            | Item.ActivePatternCase apref when apref.ActivePatternVal.IsModuleBinding || apref.ActivePatternVal.IsMember ->
                Some(ofVal symbol.SymbolEnv (assembly ()) apref.ActivePatternVal)
            | _ -> None
        | :? FSharpMemberOrFunctionOrValue as v when not v.IsModuleValueOrMember -> None
        | :? FSharpMemberOrFunctionOrValue as v ->
            let valOf =
                match v.Item with
                | Item.Value vref
                | Item.CustomBuilder(_, vref) -> Some vref
                | Item.MethodGroup(_, m :: _, _)
                | Item.CtorGroup(_, m :: _) -> m.ArbitraryValRef
                | Item.Property(_, p :: _, _) -> p.ArbitraryValRef
                | Item.Event e -> e.ArbitraryValRef
                | _ -> None

            match valOf, v.Item with
            // The token of a setter itself: a property's other uses take its getter's location, as FCS does
            | Some vref, Item.Value _ when v.IsPropertySetterMethod -> Some(ofVal symbol.SymbolEnv (assembly ()) vref + setterSuffix)
            | Some vref, _ -> Some(ofVal symbol.SymbolEnv (assembly ()) vref)
            // A struct's default constructor is generated, as its location is its type's
            | None, Item.CtorGroup(_, Infos.DefaultStructCtor(g, ty) :: _) ->
                Some(ofEntity (assembly ()) (FSharpEntity(symbol.SymbolEnv, tcrefOfAppTy g ty)))
            // No F# value: an external assembly's member, which no source file defines
            | None, _ -> Some $"%s{assembly ()}|external|%s{v.FullName}|%s{v.XmlDocSig}"
        | _ -> None
    with :? InvalidOperationException ->
        None

let private withoutSetter (key: string) =
    if key.EndsWith setterSuffix then
        key.Substring(0, key.Length - setterSuffix.Length)
    else
        key

/// Whether a definition's key is what `key` asks for: a property's accessors are one definition
let private defines (key: string) (symbol: FSharpSymbol) =
    match ofSymbol symbol with
    | Some own -> withoutSetter own = withoutSetter key
    | None -> false

/// A member's key without what a signature can state differently from an implementation it conforms to, which can be
/// more generic: its types, keeping how many arguments it has. A value's key has none.
let private withoutTypes (key: string) =
    match key.Split '|' with
    | [| assembly; kind; parent; apparent; name; isInstance; _typars; role; types |] ->
        let arguments = if types = "" then 0 else types.Split(';').Length
        String.concat "|" [ assembly; kind; parent; apparent; name; isInstance; role; string arguments ]
    | _ -> key

/// Whether a signature's definition can be what an implementation of `key` conformed to
let private conformsTo (key: string) (symbol: FSharpSymbol) =
    match ofSymbol symbol with
    | Some own -> withoutTypes (withoutSetter own) = withoutTypes (withoutSetter key)
    | None -> false

/// The location the definitions of one key answer: a property's getter's, as FCS's location does for a property, or
/// its setter's for the token of the setter. Two definitions at other locations answer nothing: only a file that
/// doesn't compile has them, and nothing current says which of them is meant.
let private locationOf (key: string) (definitions: FSharpSymbol list) : range option =
    let wanted (v: FSharpMemberOrFunctionOrValue) =
        if key.EndsWith setterSuffix then
            v.IsPropertySetterMethod
        else
            v.IsPropertyGetterMethod

    let accessorFirst =
        definitions
        |> List.sortBy (function
            | :? FSharpMemberOrFunctionOrValue as v when wanted v -> 0
            | _ -> 1)

    match accessorFirst |> List.choose _.SignatureLocation |> List.distinct with
    | [ only ] -> Some only
    | first :: _ when key.Split('|')[1] = propertyKind -> Some first
    | _ -> None

/// Everything a signature file defines, as symbols
let rec private definedIn (cenv: SymbolEnv) (m: ModuleOrNamespaceType) : seq<FSharpSymbol> =
    seq {
        for v in m.AllValsAndMembers do
            yield FSharpMemberOrFunctionOrValue(cenv, mkLocalValRef v)

        for e in m.AllEntities do
            let eref = mkLocalEntityRef e
            yield FSharpEntity(cenv, eref)

            if e.IsModuleOrNamespace then
                yield! definedIn cenv e.ModuleOrNamespaceType
            else
                for f in e.AllFieldsArray do
                    yield FSharpField(cenv, mkRecdFieldRef eref f.LogicalName)

                for c in e.UnionCasesArray do
                    let cref = mkUnionCaseRef eref c.LogicalName
                    yield FSharpUnionCase(cenv, cref)

                    for i in 0 .. c.RecdFieldsArray.Length - 1 do
                        yield FSharpField(cenv, cref, i)
    }

let symbolKey (symbol: FSharpSymbol) : string option = ofSymbol symbol

let signatureLocation
    (ctx: ProjectContext)
    (signature: FileSignature)
    (key: string)
    (conformed: bool)
    (ct: CancellationToken)
    : range option =
    let state = ctx.State

    let cenv =
        SymbolEnv(state.tcGlobals, state.tcInitialState.Ccu, Some signature.ModuleType, state.tcImports)

    let defined =
        definedIn cenv signature.ModuleType
        |> Seq.filter (fun s ->
            ct.ThrowIfCancellationRequested()
            conformsTo key s)
        |> Seq.toList

    match defined |> List.filter (defines key) with
    | [] when conformed -> locationOf key defined
    | exact -> locationOf key exact

let signatureUses (ctx: ProjectContext) (signature: FileSignature) : (struct (range * bool * FSharpSymbol))[] =
    let state = ctx.State

    let cenv =
        SymbolEnv(state.tcGlobals, state.tcInitialState.Ccu, Some signature.ModuleType, state.tcImports)

    signature.Uses
    |> Array.map (fun (struct (item, occurrence, m)) -> struct (m, (occurrence = ItemOccurrence.Binding), FSharpSymbol.Create(cenv, item)))

let definitionLocation (check: FSharpCheckFileResults) (key: string) (ct: CancellationToken) : range option =
    check.GetAllUsesOfAllSymbolsInFile ct
    |> Seq.filter (fun u -> u.IsFromDefinition && defines key u.Symbol)
    |> Seq.map _.Symbol
    |> Seq.toList
    |> locationOf key
