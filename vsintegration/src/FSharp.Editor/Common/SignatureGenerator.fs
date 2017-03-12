module Microsoft.VisualStudio.FSharp.Editor.SignatureGenerator

open System
open System.Diagnostics
open System.CodeDom.Compiler
open System.Collections.Generic
open Microsoft.FSharp.Compiler.SourceCodeServices
open Microsoft.FSharp.Compiler.PrettyNaming
open System.IO
open System.Text.RegularExpressions
open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.Text
open Microsoft.FSharp.Compiler.Ast
open Microsoft.FSharp.Compiler.Range
open Microsoft.FSharp.Compiler.SourceCodeServices
open Microsoft.FSharp.Compiler.SourceCodeServices.PrettyNaming
open Microsoft.VisualStudio.FSharp.Editor.CommonHelpers
open Microsoft.VisualStudio.FSharp.Editor.Logging


[<AutoOpen>]
module internal CodeGenerationUtils =

    type ColumnIndentedTextWriter() =
        let stringWriter = new StringWriter()
        let indentWriter = new IndentedTextWriter(stringWriter, " ")

        member __.Write(s: string) =
            indentWriter.Write("{0}", s)

        member __.Write(s: string, [<ParamArray>] objs: obj []) =
            indentWriter.Write(s, objs)

        member __.WriteLine(s: string) =
            indentWriter.WriteLine("{0}", s)

        member __.WriteLine(s: string, [<ParamArray>] objs: obj []) =
            indentWriter.WriteLine(s, objs)

        member x.WriteBlankLines count =
            for _ in 0 .. count - 1 do
                x.WriteLine ""

        member __.Indent i = 
            indentWriter.Indent <- indentWriter.Indent + i

        member __.Unindent i = 
            indentWriter.Indent <- max 0 (indentWriter.Indent - i)

        member __.Dump() =
            indentWriter.InnerWriter.ToString()

        interface IDisposable with
            member __.Dispose() =
                stringWriter.Dispose()
                indentWriter.Dispose()

    let (|IndexerArg|) = function
        | SynIndexerArg.Two(e1, e2) -> [e1; e2]
        | SynIndexerArg.One e -> [e]

    let (|IndexerArgList|) xs =
        List.collect (|IndexerArg|) xs

    let hasAttribute<'T> (attrs: seq<FSharpAttribute>) =
        attrs |> Seq.exists (fun a -> a.AttributeType.CompiledName = typeof<'T>.Name)

    let tryFindTokenLPosInRange
         (range:range, docId:DocumentId, sourceText:SourceText, position:int, fileName:string, defines:string list,predicate: FSharpTokenInfo -> bool) =
        // Normalize range
        // NOTE: FCS compiler sometimes returns an invalid range. In particular, the
        // range end limit can exceed the end limit of the document
        let range =
            if range.EndLine > sourceText.Lines.Count then
                let newEndLine = sourceText.Lines.Count
                let newEndColumn =  (sourceText.Lines.Item (newEndLine-1)).End
                let newEndPos = mkPos newEndLine newEndColumn
                
                mkFileIndexRange (range.FileIndex) range.Start newEndPos
            else
                range
            
        let lineIdxAndTokenSeq = seq {
            for lineIdx = range.StartLine to range.EndLine do
                 yield!  
                    tokenizeLine (docId,sourceText,position,fileName,defines)      // codeGenService.TokenizeLine(project, document, (lineIdx * 1<Line1>)) 
                    |> Seq.map (fun tokenInfo -> lineIdx, tokenInfo)
        }

        lineIdxAndTokenSeq
        |> Seq.tryFind (fun (line1, tokenInfo) ->
            if range.StartLine = range.EndLine then
                tokenInfo.LeftColumn >= range.StartColumn &&
                tokenInfo.RightColumn < range.EndColumn &&
                predicate tokenInfo
            elif range.StartLine = int line1 then
                tokenInfo.LeftColumn >= range.StartColumn &&
                predicate tokenInfo
            elif int line1 = range.EndLine then
                tokenInfo.RightColumn < range.EndColumn &&
                predicate tokenInfo
            else
                predicate tokenInfo
        )
        |> Option.map (fun (line1, tokenInfo) ->
            tokenInfo, (Pos.fromZ (int line1 - 1) tokenInfo.LeftColumn)
        )

    /// Represent environment where a captured identifier should be renamed
    type NamesWithIndices = Map<string, Set<int>>

    let keywordSet = set KeywordNames

    /// Rename a given argument if the identifier has been used
    let normalizeArgName (namesWithIndices: NamesWithIndices) nm =
        match nm with
        | "()" -> nm, namesWithIndices
        | _ ->
            let nm = String.lowerCaseFirstChar nm
            let nm, index = String.extractTrailingIndex nm
                
            let index, namesWithIndices =
                match namesWithIndices |> Map.tryFind nm, index with
                | Some indexes, index ->
                    let rec getAvailableIndex idx =
                        if indexes |> Set.contains idx then 
                            getAvailableIndex (idx + 1)
                        else idx
                    let index = index |> Option.defaultValue  1 |> getAvailableIndex
                    Some index, namesWithIndices |> Map.add nm (indexes |> Set.add index)
                | None, Some index -> Some index, namesWithIndices |> Map.add nm (Set.ofList [index])
                | None, None -> None, namesWithIndices |> Map.add nm Set.empty

            let nm = 
                match index with
                | Some index -> sprintf "%s%d" nm index
                | None -> nm
                
            let nm = if Set.contains nm keywordSet then sprintf "``%s``" nm else nm
            nm, namesWithIndices



type OpenDeclaration = string
type XmlDocSig = string
type XmlDoc = string list

[<NoComparison; NoEquality>]
type internal Filterer =
    {
        MemberOrFunctionOrValueFilter: FSharpMemberOrFunctionOrValue -> bool
        ModuleFilter: FSharpEntity -> bool
    }
    static member NoFilters =
        { 
            MemberOrFunctionOrValueFilter = fun _ -> true
            ModuleFilter = fun _ -> true
        }

[<NoComparison; NoEquality>]
type BlankLines =
    {
        BeforeMembersFunctionsAndValues: int
        BeforeMemberOrFunctionOrValue: int
        BeforePublicNestedEntities: int
        AfterPublicNestedEntity: int
        BeforeModuleHeaderOpenDeclaration: int
        BeforeTypeHeaderOpenDeclaration: int
        AfterTypeHeader: int
        AfterUnionCase: int
    }
    static member Default =
        {
            BeforeMembersFunctionsAndValues = 1
            BeforeMemberOrFunctionOrValue = 0
            BeforePublicNestedEntities = 1
            AfterPublicNestedEntity = 1
            BeforeModuleHeaderOpenDeclaration = 1
            BeforeTypeHeaderOpenDeclaration = 1
            AfterTypeHeader = 1
            AfterUnionCase = 1
        }
    static member None =
        {
            BeforeMembersFunctionsAndValues = 0
            BeforeMemberOrFunctionOrValue = 0
            BeforePublicNestedEntities = 0
            AfterPublicNestedEntity = 0
            BeforeModuleHeaderOpenDeclaration = 0
            BeforeTypeHeaderOpenDeclaration = 0
            AfterTypeHeader = 0
            AfterUnionCase = 0
        }

[<NoComparison; NoEquality>]
type internal Context = 
    {
        Writer: ColumnIndentedTextWriter
        /// A temporary writer to store resolved open declarations
        OpenDeclWriter: ColumnIndentedTextWriter
        Indentation: int
        DisplayContext: FSharpDisplayContext
        /// Relevant open declarations that needs resolving
        ResolvingOpenDeclarations: (OpenDeclaration * bool) []
        GetXmlDocBySignature: XmlDocSig -> XmlDoc
        Filterer: Filterer
        BlankLines: BlankLines
    }

let private hasUnitOnlyParameter (mem: FSharpMemberOrFunctionOrValue) =
    mem.CurriedParameterGroups.Count = 1 && mem.CurriedParameterGroups.[0].Count = 0

let private mustAppearAsAbstractMember (mem: FSharpMemberOrFunctionOrValue) =
    let enclosingEntityIsFSharpClass = mem.EnclosingEntity.IsClass && mem.EnclosingEntity.IsFSharp

    if mem.IsDispatchSlot then
        match mem.EnclosingEntity with
        | TypedAstPatterns.Interface | TypedAstPatterns.AbstractClass -> true
        | _ -> enclosingEntityIsFSharpClass 
    else
        false

let private needsInlineAnnotation (mem: FSharpMemberOrFunctionOrValue) =
    match mem.InlineAnnotation with
    | FSharpInlineAnnotation.AlwaysInline
    | FSharpInlineAnnotation.PseudoValue -> true
    | _ -> false

[<NoComparison>]
type private MembersPartition = 
    {
        Constructors: FSharpMemberOrFunctionOrValue[]
        AbstractMembers: FSharpMemberOrFunctionOrValue[]
        ConcreteInstanceMembers: FSharpMemberOrFunctionOrValue[]
        StaticMembers: FSharpMemberOrFunctionOrValue[]
    }
    static member Create(members: seq<FSharpMemberOrFunctionOrValue>) =
        let filteredMembers = 
            members
            |> Seq.filter (fun mem ->
                mem.Accessibility.IsPublic &&
                not mem.IsExplicitInterfaceImplementation &&
                not mem.IsPropertyGetterMethod &&
                not mem.IsPropertySetterMethod &&
                not mem.IsEvent)

        let constructors = ResizeArray<_>()
        let abstractMembers = ResizeArray<_>()
        let concreteInstanceMembers = ResizeArray<_>()
        let staticMembers = ResizeArray<_>()

        for mem in filteredMembers do
            match mem with
            | TypedAstPatterns.Constructor _ -> constructors.Add(mem)
            | _ ->
                if not mem.IsInstanceMember then staticMembers.Add(mem)
                // Is abstract
                elif mustAppearAsAbstractMember mem then 
                    abstractMembers.Add(mem)
                else
                    concreteInstanceMembers.Add(mem)

        let sortByNameAndArgCount =
            Array.sortInPlaceBy (fun (mem: FSharpMemberOrFunctionOrValue) ->
                let paramCount =
                    if hasUnitOnlyParameter mem
                    then 0 
                    else mem.CurriedParameterGroups.Count

                mem.DisplayName.ToUpperInvariant(), paramCount)

        let res = 
            { Constructors = constructors.ToArray()
              AbstractMembers = abstractMembers.ToArray()
              ConcreteInstanceMembers = concreteInstanceMembers.ToArray()
              StaticMembers = staticMembers.ToArray() }

        sortByNameAndArgCount res.AbstractMembers
        sortByNameAndArgCount res.ConcreteInstanceMembers
        sortByNameAndArgCount res.StaticMembers

        res

/// Work around an FCS bug where:
///  'List<'T>.Enumerator' is formatted as 'List`1.Enumerator<'T>'
///  'Dictionary<'TKey,'TValue>.Enumerator' is formatted as 'Dictionary`2.Enumerator<'TKey,'TValue>'
let private formatType ctx (typ: FSharpType) =
    let definition = typ.Format(ctx.DisplayContext)
    let fullyQualifiedDefinition = typ.Format(FSharpDisplayContext.Empty)
    ctx.ResolvingOpenDeclarations
    |> Array.iteri (fun i (openDecl, isUsed) ->
        if not isUsed then
            // Try to check whether openDecl has been used in the type signature
            match fullyQualifiedDefinition.IndexOf(openDecl) with
            | j when j >= 0 && j < fullyQualifiedDefinition.Length && not (definition.Contains(openDecl)) ->
                // openDecl should be a real open statement, not an accidental match
                if j = 0 || (j > 0 && fullyQualifiedDefinition.[j-1] <> '.') then
                    ctx.ResolvingOpenDeclarations.[i] <- (openDecl, true)
            | _ -> ())

    if definition.Contains("`") && not (definition.Contains("``")) then
        match definition.LastIndexOf("<") with            
        | i when i >= 0 && i < definition.Length ->
            let typeParams = definition.[i..]
            let arity = (typeParams |> Seq.filter ((=) ',') |> Seq.length) + 1
            // This type is potentially a nested type; we use the fully qualified form to bypass compiler errors.
            let fullyQualifiedDefinition = typ.Format(FSharpDisplayContext.Empty)
            Debug.Assert(fullyQualifiedDefinition.EndsWith(definition), 
                sprintf "Fully qualified identifier '%s' should be consistent with the short identifier '%s'." fullyQualifiedDefinition definition)
            match fullyQualifiedDefinition.LastIndexOf("<") with            
            | j when j >= 0 && j < fullyQualifiedDefinition.Length ->
                fullyQualifiedDefinition.[0..j-1].Replace(sprintf "`%i" arity, typeParams)
            | _ -> definition
        | _ -> definition
    elif definition.Contains("'?") || definition.Contains("^?") then
        // This type consists of uninstantiated type variables.
        // Ideally this should be amended upstream.
        // Text replacement will not work for corner cases but we digress.
        definition.Replace("'?", "'T").Replace("^?", "^T")    
    else definition

let private isStaticallyResolved (param: FSharpGenericParameter) =
    param.Constraints
    |> Seq.exists (fun c -> c.IsMemberConstraint)

let keywordTable = Set KeywordNames

let QuoteIdentifierIfNeeded (s : string) : string =
        if not (String.forall IsIdentifierPartCharacter s)              // if it has funky chars
            || s.Length > 0 && (not(IsIdentifierFirstCharacter s.[0]))  // or if it starts with a non-(letter-or-underscore)
            || keywordTable.Contains s                                  // or if it's a language keyword like "type"
        then "``"+s+"``"  // then it needs to be ``quoted``
        else s


let private formatValueOrMemberName name =
    let newName = QuoteIdentifierIfNeeded name
    if newName = name then
        DemangleOperatorName name
    else
        newName

let private formatGenericParam (param: FSharpGenericParameter) =
    let escapedName = param.Name.Replace("?", "T")
    if isStaticallyResolved param then "^" + escapedName
    else "'" + escapedName

let private formatMemberConstraint ctx (c: FSharpGenericParameterMemberConstraint) =
    let hasPropertyShape =
        (c.MemberIsStatic && c.MemberArgumentTypes.Count = 0) ||
        (not c.MemberIsStatic && c.MemberArgumentTypes.Count = 1)
    let formattedMemberName, isProperty =
        match hasPropertyShape, TryChopPropertyName c.MemberName with
        | true, Some(chopped) when chopped <> c.MemberName ->
            formatValueOrMemberName chopped, true
        | _, _ -> formatValueOrMemberName c.MemberName, false
        
    [|
        yield "("
        yield
            sprintf "%smember %s"
                (if c.MemberIsStatic then "static " else "")
                formattedMemberName
        yield " : "
        
        if isProperty then
            yield (formatType ctx c.MemberReturnType)
        else
            if c.MemberArgumentTypes.Count <= 1
            then yield "unit"
            else
                let startIdx = if c.MemberIsStatic then 0 else 1
                yield
                    [| for i in startIdx .. c.MemberArgumentTypes.Count - 1 ->
                        formatType ctx c.MemberArgumentTypes.[i] |]
                    |> String.concat " * "
            yield sprintf " -> %s" (formatType ctx c.MemberReturnType)
        yield ")"
    |]
    |> String.concat ""

let private getConstraints ctx (genParams: IList<FSharpGenericParameter>) =
    let supportedConstraints =
        [|
            for param in genParams do
                let paramName = formatGenericParam param

                for c in param.Constraints do
                    if c.IsSupportsNullConstraint then
                        yield sprintf "%s : null" paramName
                    elif c.IsRequiresDefaultConstructorConstraint then
                        yield sprintf "%s : (new : unit -> %s)" paramName paramName
                    elif c.IsCoercesToConstraint then
                        yield sprintf "%s :> %s" paramName (formatType ctx c.CoercesToTarget)
                    elif c.IsNonNullableValueTypeConstraint then
                        yield sprintf "%s : struct" paramName
                    elif c.IsReferenceTypeConstraint then
                        yield sprintf "%s : not struct" paramName
                    elif c.IsComparisonConstraint then
                        yield sprintf "%s : comparison" paramName
                    elif c.IsEqualityConstraint then
                        yield sprintf "%s : equality" paramName
                    elif c.IsUnmanagedConstraint then
                        yield sprintf "%s : unmanaged" paramName
                    elif c.IsEnumConstraint then
                        yield sprintf "%s : enum<%s>" paramName (formatType ctx c.EnumConstraintTarget)
                    elif c.IsDelegateConstraint then
                        yield sprintf "%s : delegate<%s, %s>"
                                paramName
                                (formatType ctx c.DelegateConstraintData.DelegateTupledArgumentType)
                                (formatType ctx c.DelegateConstraintData.DelegateReturnType)
                    elif c.IsMemberConstraint then
                        yield sprintf "%s : %s" paramName (formatMemberConstraint ctx c.MemberConstraintData)
        |]

    if supportedConstraints.Length = 0 then ""
    else
        supportedConstraints
        |> String.concat " and "
        |> sprintf " when %s"

let private getTypeNameWithGenericParams ctx (typ: FSharpEntity) isInTypeName =
    [|
        yield QuoteIdentifierIfNeeded typ.DisplayName
        if typ.GenericParameters.Count > 0 then
            yield "<"
            let genericParamsRepr =
                [|
                    for i in 0 .. typ.GenericParameters.Count - 1 do
                        let p = typ.GenericParameters.[i]
                        if i = 0 && isStaticallyResolved p then
                            yield " " + formatGenericParam p
                        else
                            yield formatGenericParam p
                |]
                |> String.concat ", "
            yield genericParamsRepr
            if isInTypeName then
                yield getConstraints ctx typ.GenericParameters
            yield ">"
    |]
    |> String.concat ""

let private generateSignature ctx (mem: FSharpMemberOrFunctionOrValue) =
    let generateInputParamsPart (mem: FSharpMemberOrFunctionOrValue) =
        let formatParamTypeName (param: FSharpParameter) =
            let formattedTypeName = formatType ctx param.Type
            if param.IsOptionalArg then
                let result = formattedTypeName
                if mem.EnclosingEntity.IsFSharp && result.EndsWith(" option") then
                    // result has the 'XXXX option' format: we remove the trailing ' option'
                    result.Substring(0, result.Length - (" option").Length)
                else
                    result
            elif param.Type.IsFunctionType || param.Type.IsTupleType then
                sprintf "(%s)" formattedTypeName
            else
                formattedTypeName

        if not (hasUnitOnlyParameter mem) then
            [|
                for paramGroup in mem.CurriedParameterGroups do
                    yield [|
                        for (param: FSharpParameter) in paramGroup do
                            let formattedTypeName = formatParamTypeName param

                            match param.Name with
                            | Some paramName ->
                                let paramName = QuoteIdentifierIfNeeded paramName
                                if param.IsOptionalArg then
                                    yield sprintf "?%s:%s" paramName formattedTypeName
                                else
                                    yield sprintf "%s:%s" paramName formattedTypeName
                            | None -> yield formattedTypeName
                    |]
                    |> String.concat " * "
            |]
            |> String.concat " -> "
        else
            "unit"

    match mem with
    | TypedAstPatterns.Constructor entity ->
        let signatureInputParamsPart = generateInputParamsPart mem
        let signatureReturnTypePart = getTypeNameWithGenericParams ctx entity false

        sprintf "%s -> %s" signatureInputParamsPart signatureReturnTypePart

    | TypedAstPatterns.Event ->
        let returnParameterType = mem.ReturnParameter.Type
        formatType ctx returnParameterType

    | _ when not mem.IsPropertyGetterMethod && not mem.IsPropertySetterMethod ->
        let signatureReturnTypePart = formatType ctx mem.ReturnParameter.Type

        if mem.IsProperty then
            signatureReturnTypePart
        else
            let signatureInputParamsPart = generateInputParamsPart mem
            sprintf "%s -> %s" signatureInputParamsPart signatureReturnTypePart

    | _ -> ""

let private hasVisibleConstructor (typ: FSharpEntity) =
    typ.TryGetMembersFunctionsAndValues
    |> Seq.exists (function
        | TypedAstPatterns.Constructor _ -> true
        | _             -> false)

let private tryGetNeededTypeDefSyntaxDelimiter (typ: FSharpEntity) =
    let isStruct = typ.IsValueType && not typ.IsEnum
    let hasMembers = typ.TryGetMembersFunctionsAndValues.Count > 0

    if not hasMembers && not typ.IsFSharpModule then
        if typ.IsClass then Some "class"
        elif isStruct then Some "struct"
        elif typ.IsInterface then Some "interface"
        else None
    else None

let private tryRemoveModuleSuffix (fsModule: FSharpEntity) (moduleName: string) =
    let fullModuleNameWithoutModule =
        if fsModule.IsFSharpModule && hasModuleSuffixAttribute fsModule then
            if moduleName.EndsWith "Module" then
                moduleName.Substring(0, moduleName.Length - "Module".Length)
            else moduleName
        else moduleName

    let prefixToConsider =
        match fsModule.Namespace with
        | Some namespacePrefix -> namespacePrefix
        // NOTE: it's not because there's no namespace that the module is not inside a module
        // Ex: module Microsoft.FSharp.Compiler.Range.Pos
        // -> Pos is in the Range module
        | None -> if fsModule.AccessPath = "global" then "" else fsModule.AccessPath

    let moduleNameOnly =
        let prefixLength = prefixToConsider.Length + 1
        if prefixLength >= fullModuleNameWithoutModule.Length || prefixToConsider = ""
        then fullModuleNameWithoutModule
        else fullModuleNameWithoutModule.Remove(0, prefixLength)

    if prefixToConsider = "" then
        QuoteIdentifierIfNeeded moduleNameOnly
    else
        prefixToConsider + "." + (QuoteIdentifierIfNeeded moduleNameOnly)

let rec internal writeModule isTopLevel ctx (modul: FSharpEntity) =
    Debug.Assert(modul.IsFSharpModule, "The entity should be a valid F# module.")  
    if ctx.Filterer.ModuleFilter modul then
        if not isTopLevel then
            writeDocs ctx.Writer modul.XmlDoc (fun _ -> modul.XmlDocSig) ctx.GetXmlDocBySignature
            writeAttributes ctx ctx.Writer (Some modul) modul.Attributes
            ctx.Writer.WriteLine("module {0} = ", QuoteIdentifierIfNeeded modul.LogicalName)   
            ctx.Writer.Indent ctx.Indentation

        if modul.TryGetMembersFunctionsAndValues.Count > 0 then
            ctx.Writer.WriteBlankLines ctx.BlankLines.BeforeMembersFunctionsAndValues
        
        for value in modul.TryGetMembersFunctionsAndValues do
            writeFunctionOrValue ctx value

        if not (Seq.isEmpty modul.PublicNestedEntities) then
            ctx.Writer.WriteBlankLines ctx.BlankLines.BeforePublicNestedEntities

        for entity in modul.PublicNestedEntities do
            match entity with
            |  TypedAstPatterns.FSharpModule -> writeModule false ctx entity
            |  TypedAstPatterns.AbbreviatedType abbreviatedType -> writeTypeAbbrev true ctx entity abbreviatedType
            |  TypedAstPatterns.FSharpException -> writeFSharpExceptionType true ctx entity
            |  TypedAstPatterns.Delegate when entity.IsFSharp -> writeDelegateType true ctx entity
            | _ -> writeType true ctx entity

            ctx.Writer.WriteBlankLines ctx.BlankLines.AfterPublicNestedEntity
        if not isTopLevel then
            ctx.Writer.Unindent ctx.Indentation

        if isTopLevel then
            writeModuleHeader ctx modul

/// Write open declarations, XmlDoc and attributes of modules to a separate buffer.
/// It supposes to be called after all internal details have been written.
and internal writeModuleHeader ctx (modul: FSharpEntity) =
    let writer = ctx.OpenDeclWriter
    writeDocs writer modul.XmlDoc (fun _ -> modul.XmlDocSig) ctx.GetXmlDocBySignature
    writeAttributes ctx writer (Some modul) modul.Attributes
    let qualifiedModuleName = tryRemoveModuleSuffix modul modul.FullName
    writer.WriteLine("module {0}", qualifiedModuleName)
    ctx.ResolvingOpenDeclarations
    |> Array.iteri (fun i (openDecl, isUsed) -> 
        if not isUsed then () else
        if i = 0 then 
            writer.WriteBlankLines ctx.BlankLines.BeforeModuleHeaderOpenDeclaration
        writer.WriteLine("open {0}", openDecl)) 

and internal getParentPath (entity: FSharpEntity) =
    match entity.Namespace with
    | Some ns -> sprintf "namespace %s" ns
    | None -> sprintf "module %s" entity.AccessPath

/// Write open declarations to a separate buffer.
/// It supposes to be called after all internal details have been written.
and internal writeTypeHeader ctx (typ: FSharpEntity) =
    let writer = ctx.OpenDeclWriter
    let parent = getParentPath typ
    writer.WriteLine parent
    ctx.ResolvingOpenDeclarations
    |> Array.iteri (fun i (openDecl, isUsed) -> 
        if not isUsed then () else
        if i = 0 then 
            writer.WriteBlankLines ctx.BlankLines.BeforeTypeHeaderOpenDeclaration
        writer.WriteLine("open {0}", openDecl))
    writer.WriteBlankLines ctx.BlankLines.AfterTypeHeader

and internal writeType isNestedEntity ctx (typ: FSharpEntity) =
    Debug.Assert(not typ.IsFSharpModule, "The entity should be a type.")

    writeDocs ctx.Writer typ.XmlDoc (fun _ -> typ.XmlDocSig) ctx.GetXmlDocBySignature
    writeAttributes ctx ctx.Writer (Some typ) typ.Attributes

    let neededTypeDefSyntaxDelimiter = tryGetNeededTypeDefSyntaxDelimiter typ
    let hasVisibleConstructor = hasVisibleConstructor typ
    let classAttributeHasToBeAdded =
        (not typ.IsInterface)
        && (not typ.IsValueType)
        && (typ.IsOpaque || not hasVisibleConstructor)
        && not (typ.IsFSharpRecord || typ.IsFSharpUnion)
        && not (hasAttribute<AbstractClassAttribute> typ.Attributes)
        && not (hasAttribute<ClassAttribute> typ.Attributes)
        && neededTypeDefSyntaxDelimiter <> Some "class"

    let isStruct = typ.IsValueType && not typ.IsEnum
    let hasMembers = typ.TryGetMembersFunctionsAndValues.Count > 0

    if classAttributeHasToBeAdded then
        ctx.Writer.WriteLine("[<Class>]")
    elif typ.IsInterface && neededTypeDefSyntaxDelimiter = None then
        ctx.Writer.WriteLine("[<Interface>]")
    elif isStruct && hasMembers && not hasVisibleConstructor then
        ctx.Writer.WriteLine("[<Struct>]")

    ctx.Writer.WriteLine("type {0} =", getTypeNameWithGenericParams ctx typ true)
    ctx.Writer.Indent ctx.Indentation
    if typ.IsFSharpRecord then
        ctx.Writer.WriteLine("{")
        ctx.Writer.Indent ctx.Indentation
        for field in typ.FSharpFields do
            writeField true ctx field
        ctx.Writer.Unindent ctx.Indentation
        ctx.Writer.WriteLine("}")
    elif typ.IsFSharpUnion then
        for case in typ.UnionCases do
            writeUnionCase ctx case
    elif typ.IsEnum then
        for field in typ.FSharpFields do
            writeEnumValue ctx field

    if not hasMembers then
        match neededTypeDefSyntaxDelimiter with
        | Some startDelimiter ->
            ctx.Writer.WriteLine(startDelimiter)
            ctx.Writer.Indent ctx.Indentation
        | None -> ()

    // Inherited class
    try
        match typ.BaseType with
        | Some( TypedAstPatterns.TypeWithDefinition(baseTypeDef) as baseType) when baseTypeDef.DisplayName <> "obj" ->
            if not (typ.IsValueType || typ.IsEnum || typ.IsDelegate || typ.IsArrayType) then
                // There is an FCS bug where FSharpDisplayContext isn't correct wrt inheritance.                
                let revisedContext =
                    if baseTypeDef.AccessPath = typ.AccessPath then
                        ctx
                    else
                        { ctx with DisplayContext = FSharpDisplayContext.Empty }
                ctx.Writer.WriteLine("inherit {0}", formatType revisedContext baseType)
        | _ -> ()
    with _ -> ()

    // Interfaces
    [
        let ignoredInterfaces = set [ "IComparable"; "IComparable`1"; "IEquatable"; "IEquatable`1"; "IStructuralComparable"; "IStructuralEquatable" ]
        for inter in typ.DeclaredInterfaces do
            match inter with
            |  TypedAstPatterns.TypeWithDefinition entity ->
                if entity <> typ && entity.Accessibility.IsPublic 
                   && not (Set.contains entity.LogicalName ignoredInterfaces) then
                    let revisedContext =
                        if entity.AccessPath = typ.AccessPath then
                            ctx
                        else
                            { ctx with DisplayContext = FSharpDisplayContext.Empty }
                    yield inter, formatType revisedContext inter
            | _ -> ()
    ]
    |> List.sortBy (fun (inter, _name) ->
        // Sort by name without the namespace qualifier
        match inter with
        |  TypedAstPatterns.TypeWithDefinition entity ->
            entity.DisplayName.ToUpperInvariant()
        | _ ->
            String.Empty)
    |> List.iter (fun (_, name) ->
        if typ.IsInterface then
            ctx.Writer.WriteLine("inherit {0}", name)
        else
            ctx.Writer.WriteLine("interface {0}", name))

    let membersPartition = MembersPartition.Create typ.TryGetMembersFunctionsAndValues

    // Constructors
    for constr in membersPartition.Constructors do
        writeMember ctx constr

    // Fields
    if typ.IsClass || isStruct then
        for field in typ.FSharpFields do
            if field.Accessibility.IsPublic then
                writeClassOrStructField ctx field

    // Abstract members
    for mem in membersPartition.AbstractMembers do
        writeMember ctx mem

    // Concrete instance members
    for mem in membersPartition.ConcreteInstanceMembers do
        writeMember ctx mem

    // Static members
    for mem in membersPartition.StaticMembers do
        writeMember ctx mem

    // Nested entities
    // Deactivate nested types display for the moment (C# doesn't do it)
//        for entity in typ.NestedEntities do
//            Debug.Assert(not entity.IsFSharpModule, "F# type should not contain modules.")
//            // Nested types only happen due to C# interoperability
//            ctx.Writer.WriteLine("")
//            writeType ctx entity

    if not hasMembers && neededTypeDefSyntaxDelimiter <> None then
        ctx.Writer.Unindent ctx.Indentation
        ctx.Writer.WriteLine("end")

    ctx.Writer.Unindent ctx.Indentation

    if not isNestedEntity then
        writeTypeHeader ctx typ

and internal writeTypeAbbrev isNestedEntity ctx (abbreviatingType: FSharpEntity) (abbreviatedType: FSharpType) =
    writeDocs ctx.Writer abbreviatingType.XmlDoc (fun _ -> abbreviatingType.XmlDocSig) ctx.GetXmlDocBySignature
    ctx.Writer.WriteLine("type {0} = {1}",
        getTypeNameWithGenericParams ctx abbreviatingType true,
        formatType ctx abbreviatedType)

    if not isNestedEntity then
        writeTypeHeader ctx abbreviatingType

and internal writeFSharpExceptionType isNestedEntity ctx (exn: FSharpEntity) =
    writeDocs ctx.Writer exn.XmlDoc (fun _ -> exn.XmlDocSig) ctx.GetXmlDocBySignature

    if exn.FSharpFields.Count > 0 then
        let fields =
            [|
                for field in exn.FSharpFields ->
                    if field.FieldType.IsFunctionType || field.FieldType.IsTupleType then
                        sprintf "(%s)" (formatType ctx field.FieldType)
                    else
                        formatType ctx field.FieldType
            |]
            |> String.concat " * "

        ctx.Writer.WriteLine("exception {0} of {1}", QuoteIdentifierIfNeeded exn.LogicalName, fields)
    else
        ctx.Writer.WriteLine("exception {0}", QuoteIdentifierIfNeeded exn.LogicalName)

    if not isNestedEntity then
        writeTypeHeader ctx exn

and internal writeDelegateType isNestedEntity ctx (del: FSharpEntity) =
    writeDocs ctx.Writer del.XmlDoc (fun _ -> del.XmlDocSig) ctx.GetXmlDocBySignature

    let argsPart =
        [|
            for arg in del.FSharpDelegateSignature.DelegateArguments do
                match arg with
                | Some name, typ ->
                    if typ.IsFunctionType || typ.IsTupleType then
                        yield sprintf "%s:(%s)" name (formatType ctx typ)
                    else
                        yield sprintf "%s:%s" name (formatType ctx typ)
                | None, typ ->
                    if typ.IsFunctionType || typ.IsTupleType then
                        yield sprintf "(%s)" (formatType ctx typ)
                    else
                        yield sprintf "%s" (formatType ctx typ)
        |]
        |> String.concat " * "

    ctx.Writer.WriteLine("type {0} =", QuoteIdentifierIfNeeded del.LogicalName)
    ctx.Writer.Indent ctx.Indentation
    ctx.Writer.WriteLine("delegate of {0} -> {1}", argsPart, formatType ctx del.FSharpDelegateSignature.DelegateReturnType)
    ctx.Writer.Unindent ctx.Indentation

    if not isNestedEntity then
        writeTypeHeader ctx del

and internal writeUnionCase ctx (case: FSharpUnionCase) =
    writeDocs ctx.Writer case.XmlDoc (fun _ -> case.XmlDocSig) ctx.GetXmlDocBySignature

    if case.Attributes.Count > 0 then
        ctx.Writer.Write("| ")
        ctx.Writer.Indent(2)
        writeAttributes ctx ctx.Writer None case.Attributes
        ctx.Writer.Write(formatValueOrMemberName case.Name)
        ctx.Writer.Unindent(2)
    else
        ctx.Writer.Write("| {0}", formatValueOrMemberName case.Name)

    if case.UnionCaseFields.Count > 0 then
        case.UnionCaseFields
        |> Seq.iteri (fun i field ->
            if i = 0 then 
                ctx.Writer.Write(" of ")
            else 
                ctx.Writer.Write(" * ")
            writeUnionCaseField ctx field)

    ctx.Writer.WriteBlankLines ctx.BlankLines.AfterUnionCase

and internal formatAttribute ctx (attribute: FSharpAttribute) =
    let definition = attribute.Format(ctx.DisplayContext)
    let fullyQualifiedDefinition = attribute.Format(FSharpDisplayContext.Empty)
    ctx.ResolvingOpenDeclarations
    |> Array.iteri (fun i (openDecl, isUsed) ->
        if not isUsed then
            // Try to check whether openDecl has been used in the type signature
            match fullyQualifiedDefinition.IndexOf(openDecl) with
            | j when j >= 0 && j < fullyQualifiedDefinition.Length && not (definition.Contains(openDecl)) ->
                // openDecl should be a real open statement, not an accidental match
                if j = 0 || (j > 0 && fullyQualifiedDefinition.[j-1] <> '.') then
                    ctx.ResolvingOpenDeclarations.[i] <- (openDecl, true)
            | _ -> ())
    // Try to shorten the attributes. Hopefully they are still well-formed.
    definition.Replace("Attribute ()", "").Replace("Attribute (", "(")

and internal writeAttributes ctx writer (typ: option<FSharpEntity>) (attributes: IList<FSharpAttribute>) =
    let typeDefSyntaxDelimOpt = Option.bind tryGetNeededTypeDefSyntaxDelimiter typ
    let bypassAttribute (attrib: FSharpAttribute) =
        not attrib.AttributeType.Accessibility.IsPublic 
            // Skip a few attributes that can't be handled by FCS
            || isAttribute<AttributeUsageAttribute> attrib
            || isAttribute<Diagnostics.DebuggerTypeProxyAttribute> attrib
            || (typeDefSyntaxDelimOpt = Some "struct" && isAttribute<StructAttribute> attrib)

    for attr in attributes do
        if not (bypassAttribute attr) then
            let attrText = formatAttribute ctx attr
            // Bypass unsupported attribute arguments 
            // TODO: this should be fixed in FCS, albeit not easy.
            if not (attrText.Contains "(* unsupported attribute argument *)") then
                writer.WriteLine(attrText)  

and internal writeField hasNewLine ctx (field: FSharpField) =
    writeDocs ctx.Writer field.XmlDoc (fun _ -> field.XmlDocSig) ctx.GetXmlDocBySignature
    writeAttributes ctx ctx.Writer None field.FieldAttributes
    writeAttributes ctx ctx.Writer None field.PropertyAttributes

    let fieldName = QuoteIdentifierIfNeeded field.Name

    if hasNewLine then
        ctx.Writer.WriteLine("{0}: {1}", fieldName, formatType ctx field.FieldType)
    else
        ctx.Writer.Write("{0}: {1}", fieldName, formatType ctx field.FieldType)

and internal writeEnumValue ctx (field: FSharpField) =
    // NOTE: for enum values, the compiler generates a "value__" field which 
    //       is not visible from outside. That's why we don't display it
    if not field.IsCompilerGenerated then
        match field.LiteralValue with
        | Some value ->
            writeDocs ctx.Writer field.XmlDoc (fun _ -> field.XmlDocSig) ctx.GetXmlDocBySignature
            ctx.Writer.WriteLine("| {0} = {1}", field.Name, value)
        | None -> Debug.Fail("There should be a literal value for the enum field: " + field.FullName)

and internal writeUnionCaseField ctx (field: FSharpField) =
    if isUnnamedUnionCaseField field then
        ctx.Writer.Write(formatType ctx field.FieldType)
    else
        ctx.Writer.Write("{0}: {1}", QuoteIdentifierIfNeeded field.Name, formatType ctx field.FieldType)

and internal writeFunctionOrValue ctx (value: FSharpMemberOrFunctionOrValue) =
    Debug.Assert(value.LogicalEnclosingEntity.IsFSharpModule, "The enclosing entity should be a valid F# module.")

    if ctx.Filterer.MemberOrFunctionOrValueFilter value then
        ctx.Writer.WriteBlankLines ctx.BlankLines.BeforeMemberOrFunctionOrValue

        writeDocs ctx.Writer value.XmlDoc (fun _ -> value.XmlDocSig) ctx.GetXmlDocBySignature

        let constraints = getConstraints ctx value.GenericParameters
        let valueName = 
            if value.IsActivePattern then 
                sprintf "(%s)" value.LogicalName 
            else formatValueOrMemberName value.LogicalName

        if value.FullType.IsFunctionType then
            let inlineSpecifier = if needsInlineAnnotation value then "inline " else ""
            ctx.Writer.WriteLine("val {0}{1} : {2}{3}", inlineSpecifier, valueName, generateSignature ctx value, constraints)
        else
            ctx.Writer.WriteLine("val {0} : {1}{2}", valueName, formatType ctx value.FullType, constraints)

and internal writeClassOrStructField ctx (field: FSharpField) =
    Debug.Assert(field.DeclaringEntity.IsClass ||
                 (field.DeclaringEntity.IsValueType && not field.DeclaringEntity.IsEnum),
                 "The declaring entity should be a class or a struct.")


    writeDocs ctx.Writer field.XmlDoc (fun _ -> field.XmlDocSig) ctx.GetXmlDocBySignature
    ctx.Writer.WriteLine("val {0} : {1}", QuoteIdentifierIfNeeded field.DisplayName, formatType ctx field.FieldType)

and internal writeMember ctx (mem: FSharpMemberOrFunctionOrValue) =
    Debug.Assert(not mem.LogicalEnclosingEntity.IsFSharpModule, "The enclosing entity should be a type.")

    if ctx.Filterer.MemberOrFunctionOrValueFilter mem then
        ctx.Writer.WriteBlankLines ctx.BlankLines.BeforeMemberOrFunctionOrValue

        match mem with
        | TypedAstPatterns.Constructor _entity ->
            writeDocs ctx.Writer mem.XmlDoc (fun _ -> mem.XmlDocSig) ctx.GetXmlDocBySignature
            ctx.Writer.WriteLine("new : {0}", generateSignature ctx mem)
        | TypedAstPatterns.Event -> ()
        | _ when not mem.IsPropertyGetterMethod && not mem.IsPropertySetterMethod ->
            // Discard explicit getter/setter methods
            writeDocs ctx.Writer mem.XmlDoc (fun _ -> mem.XmlDocSig) ctx.GetXmlDocBySignature
            writeAttributes ctx ctx.Writer None mem.Attributes

            let memberType =
                // Is static?
                if not mem.IsInstanceMember then
                    "static member"
                // Is abstract && enclosing type is interface/abstract?
                elif mustAppearAsAbstractMember mem then
                    "abstract member"
                elif mem.IsOverrideOrExplicitInterfaceImplementation then
                    "override"
                else
                    "member"

            let propertyType =
                if mem.HasSetterMethod && mem.HasGetterMethod then " with get, set"
                elif mem.HasSetterMethod then " with set"
                else ""

            let inlineAnnotation =
                if needsInlineAnnotation mem then "inline " else ""

            let constraints = getConstraints ctx mem.GenericParameters

            ctx.Writer.WriteLine("{0} {1}{2} : {3}{4}{5}",
                                 memberType,
                                 inlineAnnotation,
                                 // We don't need to demangle operator names since member operators should appear in their compiled forms
                                 QuoteIdentifierIfNeeded mem.LogicalName,
                                 generateSignature ctx mem,
                                 propertyType,
                                 constraints)
        | _ -> ()

and internal writeDocs writer docs getXmlDocSig xmlDocBySig =
    let xmlDocs =
        if Seq.isEmpty docs then            
            getXmlDocSig()
            |> xmlDocBySig
            :> seq<_>
        else docs :> seq<_>

    xmlDocs
    |> Seq.collect String.getLines
    |> Seq.iter (fun line -> writer.WriteLine("/// {0}", line.TrimStart()))

and internal writeActivePattern ctx (case: FSharpActivePatternCase) =
    let group = case.Group
    writeDocs ctx.Writer case.XmlDoc (fun _ -> case.XmlDocSig) ctx.GetXmlDocBySignature
    ctx.Writer.Write("val (|")
    for name in group.Names do
        ctx.Writer.Write("{0}|", name)
    if not group.IsTotal then
        ctx.Writer.Write("_|")
    ctx.Writer.Write(")")
    ctx.Writer.Write(" : ")
    ctx.Writer.WriteLine("{0}", formatType ctx group.OverallType)

let internal formatSymbol getXmlDocBySignature indentation displayContext openDeclarations (symbol: FSharpSymbol) filterer blankLines =
    use writer = new ColumnIndentedTextWriter()
    use openDeclWriter = new ColumnIndentedTextWriter()
    let ctx = { Writer = writer; OpenDeclWriter = openDeclWriter;
                Indentation = indentation; DisplayContext = displayContext; 
                ResolvingOpenDeclarations = openDeclarations |> Seq.map (fun openDecl -> openDecl, false) |> Seq.toArray;
                GetXmlDocBySignature = getXmlDocBySignature;
                Filterer = filterer;
                BlankLines = blankLines }
    
    let rec writeSymbol (symbol: FSharpSymbol) =
        match symbol with
        | TypedAstPatterns.FSharpEntity(entity, _, _) ->
            match entity with
            | TypedAstPatterns.FSharpModule -> writeModule true ctx entity
            | TypedAstPatterns.AbbreviatedType abbreviatedType -> writeTypeAbbrev false ctx entity abbreviatedType
            | TypedAstPatterns.FSharpException -> writeFSharpExceptionType false ctx entity
            | TypedAstPatterns.Delegate when entity.IsFSharp -> writeDelegateType false ctx entity
            | _ -> writeType false ctx entity
            |> Some
        | TypedAstPatterns.MemberFunctionOrValue mem ->
            writeSymbol mem.LogicalEnclosingEntity
        | TypedAstPatterns.ActivePatternCase case ->
            match case.Group.EnclosingEntity with
            | Some entity ->
                writeSymbol entity
            | None ->
                Some (writeActivePattern ctx case)
        | TypedAstPatterns.UnionCase uc ->
            match uc.ReturnType with
            | TypedAstPatterns.TypeWithDefinition entity ->
                writeSymbol entity
            | _ -> None
        | TypedAstPatterns.Field(field, _) ->
            writeSymbol field.DeclaringEntity
        | _ ->
            debug "Invalid symbol in this context: %O" (symbol.GetType().Name)
            None
    writeSymbol symbol
    |> Option.map (fun _ -> 
        openDeclWriter.Write(writer.Dump())
        openDeclWriter.Dump())

let [<Literal>] private tempFileName = "tmp"

/// Get file name from symbol's full name and escape illegal characters
let internal getFileNameFromSymbol (symbol: FSharpSymbol) =    
    let fileName =        
        match symbol with
        | TypedAstPatterns.MemberFunctionOrValue mem ->
            mem.LogicalEnclosingEntity.TryGetFullName()
        | TypedAstPatterns.UnionCase uc ->
            match uc.ReturnType with
            | TypedAstPatterns.TypeWithDefinition entity ->
                entity.TryGetFullName()
            | _ -> None
        | TypedAstPatterns.Field (field, _) -> field.DeclaringEntity.TryGetFullName()
        | _ -> Option.attempt(fun _ -> symbol.FullName)
        |> Option.defaultValue tempFileName

    let regexSearch = String(Path.GetInvalidFileNameChars()) + String(Path.GetInvalidPathChars())
    let pattern = Regex(String.Format("[{0}]", Regex.Escape(regexSearch)))
    pattern.Replace(fileName + ".fsi", "")
