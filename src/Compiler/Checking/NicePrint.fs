// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

/// Print Signatures/Types, for signatures, intellisense, quick info, FSI responses
module internal FSharp.Compiler.NicePrint

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

open FSharp.Core.Printf

[<AutoOpen>]
module internal PrintUtilities = 
    let bracketIfL x lyt = if x then bracketL lyt else lyt

    let squareAngleL x = LeftL.leftBracketAngle ^^ x ^^ RightL.rightBracketAngle

    let angleL x = sepL leftAngle ^^ x ^^ rightL rightAngle

    let braceL x = wordL leftBrace ^^ x ^^ wordL rightBrace

    let braceMultiLineL x = (wordL leftBrace @@-- x) @@ wordL rightBrace

    let braceBarL x = wordL leftBraceBar ^^ x ^^ wordL rightBraceBar

    // Use a space before a colon if there is an unusual character to the left
    let addColonL l =
        if endsWithL ">" l || endsWithL ")" l || endsWithL "`" l then 
            l ^^ WordL.colon
        else
            l ^^ RightL.colon

    let comment str = wordL (tagText (sprintf "(* %s *)" str))

    let isDiscard (name: string) = name.StartsWith("_")

    let ensureFloat (s: string) =
        if String.forall (fun c -> Char.IsDigit c || c = '-') s then
            s + ".0" 
        else s

    let layoutsL (ls: Layout list) : Layout =
        match ls with
        | [] -> emptyL
        | [x] -> x
        | x :: xs -> List.fold (^^) x xs 

    // Layout a curried function type. Over multiple lines breaking takes some care, e.g.
    //
    // val SampleFunctionTupledAllBreakA:
    //    longLongLongArgName1: string * longLongLongArgName2: TType *
    //    longLongLongArgName3: TType * longLongLongArgName4: TType ->
    //      TType list
    //
    // val SampleFunctionTupledAllBreakA:
    //    longLongLongArgName1: string *
    //    longLongLongArgName2: TType *
    //    longLongLongArgName3: TType *
    //    longLongLongArgName4: TType ->
    //      TType list
    //
    // val SampleFunctionCurriedOneBreakA:
    //    arg1: string -> arg2: TType -> arg3: TType ->
    //      arg4: TType -> TType list
    //
    // val SampleFunctionCurriedAllBreaksA:
    //    longLongLongArgName1: string ->
    //      longLongLongArgName2: TType ->
    //      longLongLongArgName3: TType ->
    //      longLongLongArgName4: TType ->
    //        TType list
    //
    //  val SampleFunctionMixedA:
    //    longLongLongArgName1: string *
    //    longLongLongArgName2: string ->
    //      longLongLongArgName3: string *
    //      longLongLongArgName4: string *
    //      longLongLongArgName5: TType ->
    //        longLongLongArgName6: TType *
    //        longLongLongArgName7: TType ->
    //          longLongLongArgName8: TType *
    //          longLongLongArgName9: TType *
    //          longLongLongArgName10: TType ->
    //            TType list
    let curriedLayoutsL retTyDelim (argTysL: Layout list) (retTyL: Layout) =
        let lastIndex = List.length argTysL - 1

        argTysL
        |> List.mapi (fun idx argTyL ->
            let isTupled =
                idx = 0 ||
                match argTyL with
                | Node(leftLayout = Node(rightLayout = Leaf (text = starText))) -> starText.Text = "*"
                | _ -> false

            let layout =
                argTyL
                ^^ (if idx = lastIndex then
                        wordL (tagPunctuation retTyDelim)
                    else
                        wordL (tagPunctuation "->"))

            isTupled, layout)
        |> List.rev
        |> fun reversedArgs -> (true, retTyL) :: reversedArgs
        |> List.fold (fun acc (shouldBreak, layout) -> (if shouldBreak then (---) else (++)) layout acc) emptyL

    let tagNavArbValRef (valRefOpt: ValRef option) tag =
        match valRefOpt with
        | Some vref ->
            tag |> mkNav vref.DefinitionRange
        | None ->
            tag

    let suppressInheritanceAndInterfacesForTyInSimplifiedDisplays g amap m ty = 
        isEnumTy g ty || isDelegateTy g ty || ExistsHeadTypeInEntireHierarchy g amap m ty g.exn_tcr

    let applyMaxMembers maxMembers (allDecls: _ list) = 
        match maxMembers with 
        | Some n when allDecls.Length > n -> (allDecls |> List.truncate n) @ [wordL (tagPunctuation "...")] 
        | _ -> allDecls

    // Put the "+ N overloads" into the layout
    let shrinkOverloads layoutFunction resultFunction group = 
        match group with 
        | [x] -> [resultFunction x (layoutFunction x)] 
        | x :: rest -> [ resultFunction x (layoutFunction x -- leftL (tagText (match rest.Length with 1 -> FSComp.SR.nicePrintOtherOverloads1() | n -> FSComp.SR.nicePrintOtherOverloadsN(n)))) ] 
        | _ -> []
    
    let tagEntityRefName(denv: DisplayEnv) (xref: EntityRef) name =
        if xref.IsNamespace then tagNamespace name
        elif xref.IsModule then tagModule name
        elif xref.IsTypeAbbrev then
            let ty = xref.TypeAbbrev.Value
            match stripTyEqns denv.g ty with
            | TType_app(tcref, _, _) when tcref.IsStructOrEnumTycon ->
                tagStruct name
            | _ ->
                tagAlias name
        elif xref.IsFSharpDelegateTycon then tagDelegate name
        elif xref.IsILEnumTycon || xref.IsFSharpEnumTycon then tagEnum name
        elif xref.IsStructOrEnumTycon then tagStruct name
        elif isInterfaceTyconRef xref then tagInterface name
        elif xref.IsUnionTycon then tagUnion name
        elif xref.IsRecordTycon then tagRecord name
        else tagClass name

    let usePrefix (denv: DisplayEnv) (tcref: TyconRef) =
        match denv.genericParameterStyle with
        | GenericParameterStyle.Implicit -> tcref.IsPrefixDisplay
        | GenericParameterStyle.Prefix -> true
        | GenericParameterStyle.Suffix -> false

    let layoutTyconRefImpl isAttribute (denv: DisplayEnv) (tcref: TyconRef) =

        let prefix = usePrefix denv tcref
        let isArray = not prefix && isArrayTyconRef denv.g tcref
        let demangled = 
            if isArray then
                tcref.DisplayNameCore // no backticks for arrays "int[]"
            else
                let name =
                    if denv.includeStaticParametersInTypeNames then 
                        tcref.DisplayNameWithStaticParameters 
                    elif tcref.DisplayName = tcref.DisplayNameWithStaticParameters then
                        tcref.DisplayName // has no static params
                    else
                        tcref.DisplayName+"<...>" // shorten
                if isAttribute && name.EndsWithOrdinal("Attribute") then
                    String.dropSuffix name "Attribute"
                else 
                    name

        let tyconTagged =
            tagEntityRefName denv tcref demangled
            |> mkNav tcref.DefinitionRange

        let tyconTextL =
            if isArray then
                tyconTagged |> rightL
            else
                tyconTagged |> wordL

        if denv.shortTypeNames then 
            tyconTextL
        else
            let path = tcref.CompilationPath.DemangledPath
            let path =
                if denv.includeStaticParametersInTypeNames then
                    path
                else
                    path |> List.map (fun s -> 
                        let i = s.IndexOf(',')
                        if i <> -1 then s.Substring(0, i)+"<...>" // apparently has static params, shorten
                        else s)
            let pathText = trimPathByDisplayEnv denv path
            if pathText = "" then tyconTextL else leftL (tagUnknownEntity pathText) ^^ tyconTextL

    let layoutBuiltinAttribute (denv: DisplayEnv) (attrib: BuiltinAttribInfo) =
        let tcref = attrib.TyconRef
        squareAngleL (layoutTyconRefImpl true denv tcref)

    /// layout the xml docs immediately before another block
    let layoutXmlDoc (denv: DisplayEnv) alwaysAddEmptyLine (xml: XmlDoc) restL =
        if denv.showDocumentation then
            let xmlDocL =
                let linesL =
                    [ for lineText in xml.UnprocessedLines do
                            // These lines may have new-lines in them and we need to split them so we can format it
                            for line in lineText.Split('\n') do
                            // note here that we don't add a space after the triple-slash, because
                            // the implicit spacing hasn't been trimmed here.
                            yield ("///" + line) |> tagText |> wordL
                    ]

                // Always add an empty line before any "///" comment
                let linesL = 
                    if linesL.Length > 0 || alwaysAddEmptyLine then 
                        [ yield "" |> tagText |> wordL
                          yield! linesL ]
                    else
                        linesL
                     
                linesL |> aboveListL

            xmlDocL @@ restL
        else restL

    let layoutXmlDocFromSig (denv: DisplayEnv) (infoReader: InfoReader) alwaysAddEmptyLine (possibleXmlDoc: XmlDoc) restL (info: (string option * string) option) =
        let xmlDoc =
            if possibleXmlDoc.IsEmpty then
                match info with
                | Some(Some ccuFileName, xmlDocSig) ->
                    infoReader.amap.assemblyLoader.TryFindXmlDocumentationInfo(Path.GetFileNameWithoutExtension ccuFileName)
                    |> Option.bind (fun xmlDocInfo ->
                        xmlDocInfo.TryGetXmlDocBySig(xmlDocSig)
                    )
                    |> Option.defaultValue possibleXmlDoc
                | _ ->
                    possibleXmlDoc
            else
                possibleXmlDoc
        layoutXmlDoc denv alwaysAddEmptyLine xmlDoc restL

    let layoutXmlDocOfVal (denv: DisplayEnv) (infoReader: InfoReader) (vref: ValRef) restL =
        if denv.showDocumentation then
            GetXmlDocSigOfValRef denv.g vref
            |> layoutXmlDocFromSig denv infoReader true vref.XmlDoc restL             
        else
            restL

    let layoutXmlDocOfMethInfo (denv: DisplayEnv) (infoReader: InfoReader) (minfo: MethInfo) restL =
        if denv.showDocumentation then
            GetXmlDocSigOfMethInfo infoReader Range.range0 minfo
            |> layoutXmlDocFromSig denv infoReader true minfo.XmlDoc restL             
        else
            restL

    let layoutXmlDocOfPropInfo (denv: DisplayEnv) (infoReader: InfoReader) (pinfo: PropInfo) restL =
        if denv.showDocumentation then
            GetXmlDocSigOfProp infoReader Range.range0 pinfo
            |> layoutXmlDocFromSig denv infoReader true pinfo.XmlDoc restL             
        else
            restL

    let layoutXmlDocOfEventInfo (denv: DisplayEnv) (infoReader: InfoReader) (einfo: EventInfo) restL =
        if denv.showDocumentation then
            GetXmlDocSigOfEvent infoReader Range.range0 einfo
            |> layoutXmlDocFromSig denv infoReader true einfo.XmlDoc restL             
        else
            restL

    let layoutXmlDocOfILFieldInfo (denv: DisplayEnv) (infoReader: InfoReader) (finfo: ILFieldInfo) restL =
        if denv.showDocumentation then
            GetXmlDocSigOfILFieldInfo infoReader Range.range0 finfo
            |> layoutXmlDocFromSig denv infoReader true XmlDoc.Empty restL             
        else
            restL

    let layoutXmlDocOfRecdField (denv: DisplayEnv) (infoReader: InfoReader) isClassDecl (rfref: RecdFieldRef) restL =
        if denv.showDocumentation then
            GetXmlDocSigOfRecdFieldRef rfref
            |> layoutXmlDocFromSig denv infoReader isClassDecl rfref.RecdField.XmlDoc restL             
        else
            restL

    let layoutXmlDocOfUnionCase (denv: DisplayEnv) (infoReader: InfoReader) (ucref: UnionCaseRef) restL =
        if denv.showDocumentation then
            GetXmlDocSigOfUnionCaseRef ucref
            |> layoutXmlDocFromSig denv infoReader false ucref.UnionCase.XmlDoc restL             
        else
            restL

    let layoutXmlDocOfEntity (denv: DisplayEnv) (infoReader: InfoReader) (eref: EntityRef) restL =
        if denv.showDocumentation then
            GetXmlDocSigOfEntityRef infoReader Range.range0 eref
            |> layoutXmlDocFromSig denv infoReader true eref.XmlDoc restL             
        else
            restL

module PrintIL = 

    let fullySplitILTypeRef (tref: ILTypeRef) = 
        (List.collect splitNamespace (tref.Enclosing @ [DemangleGenericTypeName tref.Name])) 

    let layoutILTypeRefName denv path =
        let path = 
            match path with 
            | [ "System"; "Void" ] -> ["unit"]
            | [ "System"; "Object" ] -> ["obj"]
            | [ "System"; "String" ] -> ["string"]
            | [ "System"; "Single" ] -> ["float32"]
            | [ "System"; "Double" ] -> ["float"]
            | [ "System"; "Decimal"] -> ["decimal"]
            | [ "System"; "Char" ] -> ["char"]
            | [ "System"; "Byte" ] -> ["byte"]
            | [ "System"; "SByte" ] -> ["sbyte"]
            | [ "System"; "Int16" ] -> ["int16"]
            | [ "System"; "Int32" ] -> ["int" ]
            | [ "System"; "Int64" ] -> ["int64" ]
            | [ "System"; "UInt16" ] -> ["uint16" ]
            | [ "System"; "UInt32" ] -> ["uint" ]
            | [ "System"; "UInt64" ] -> ["uint64" ]
            | [ "System"; "IntPtr" ] -> ["nativeint" ]
            | [ "System"; "UIntPtr" ] -> ["unativeint" ]
            | [ "System"; "Boolean"] -> ["bool"]
            | _ -> path
        let p2, n = List.frontAndBack path
        let tagged = if n = "obj" || n = "string" then tagClass n else tagStruct n
        if denv.shortTypeNames then 
            wordL tagged
          else
            leftL (tagNamespace (trimPathByDisplayEnv denv p2)) ^^ wordL tagged

    let layoutILTypeRef denv tref =
        let path = fullySplitILTypeRef tref
        layoutILTypeRefName denv path

    let layoutILArrayShape (ILArrayShape sh) = 
        SepL.leftBracket ^^ wordL (tagPunctuation (sh |> List.tail |> List.map (fun _ -> ",") |> String.concat "")) ^^ RightL.rightBracket // drop off one "," so that a n-dimensional array has n - 1 ","'s

    let paramsL (ps: Layout list) : Layout = 
        match ps with
        | [] -> emptyL
        | _ -> 
            let body = commaListL ps
            SepL.leftAngle ^^ body ^^ RightL.rightAngle

    let pruneParams (className: string) (ilTyparSubst: Layout list) =
        let numParams = 
            // can't find a way to see the number of generic parameters for *this* class (the GenericParams also include type variables for enclosing classes); this will have to do
            let rightMost = className |> SplitNamesForILPath |> List.last
            match Int32.TryParse(rightMost, NumberStyles.Integer, CultureInfo.InvariantCulture) with 
            | true, n -> n
            | false, _ -> 0 // looks like it's non-generic
        ilTyparSubst |> List.rev |> List.truncate numParams |> List.rev
 
    let rec layoutILType (denv: DisplayEnv) (ilTyparSubst: Layout list) (ty: ILType) : Layout =
        match ty with
        | ILType.Void -> WordL.structUnit // These are type-theoretically totally different type-theoretically `void` is Fin 0 and `unit` is Fin (S 0) ... but, this looks like as close as we can get.
        | ILType.Array (sh, t) -> layoutILType denv ilTyparSubst t ^^ layoutILArrayShape sh
        | ILType.Value t
        | ILType.Boxed t -> layoutILTypeRef denv t.TypeRef ^^ (t.GenericArgs |> List.map (layoutILType denv ilTyparSubst) |> paramsL)
        | ILType.Ptr t
        | ILType.Byref t -> layoutILType denv ilTyparSubst t
        | ILType.FunctionPointer t -> layoutILCallingSignature denv ilTyparSubst None t
        | ILType.TypeVar n -> List.item (int n) ilTyparSubst
        | ILType.Modified (_, _, t) -> layoutILType denv ilTyparSubst t // Just recurse through them to the contained ILType

    /// Layout a function pointer signature using type-only-F#-style. No argument names are printed.
    and layoutILCallingSignature denv ilTyparSubst cons (signature: ILCallingSignature) =
        // We need a special case for
        // constructors (Their return types are reported as `void`, but this is
        // incorrect; so if we're dealing with a constructor we require that the
        // return type be passed along as the `cons` parameter.)
        let args = signature.ArgTypes |> List.map (layoutILType denv ilTyparSubst) 
        let res = 
            match cons with
            | Some className -> 
                let names = SplitNamesForILPath (DemangleGenericTypeName className)
                // special case for constructor return-type (viz., the class itself)
                layoutILTypeRefName denv names ^^ (pruneParams className ilTyparSubst |> paramsL) 
            | None -> 
                signature.ReturnType |> layoutILType denv ilTyparSubst
        
        match args with
        | [] -> WordL.structUnit ^^ WordL.arrow ^^ res
        | [x] -> x ^^ WordL.arrow ^^ res
        | _ -> sepListL WordL.star args ^^ WordL.arrow ^^ res

    let layoutILFieldInit x =
        let textOpt = 
            match x with
            | Some init -> 
                match init with
                | ILFieldInit.Bool x -> 
                    if x then
                        Some keywordTrue
                    else
                        Some keywordFalse
                | ILFieldInit.Char c -> ("'" + (char c).ToString () + "'") |> (tagStringLiteral >> Some)
                | ILFieldInit.Int8 x -> ((x |> int32 |> string) + "y") |> (tagNumericLiteral >> Some)
                | ILFieldInit.Int16 x -> ((x |> int32 |> string) + "s") |> (tagNumericLiteral >> Some)
                | ILFieldInit.Int32 x -> x |> (string >> tagNumericLiteral >> Some)
                | ILFieldInit.Int64 x -> ((x |> string) + "L") |> (tagNumericLiteral >> Some)
                | ILFieldInit.UInt8 x -> ((x |> int32 |> string) + "uy") |> (tagNumericLiteral >> Some)
                | ILFieldInit.UInt16 x -> ((x |> int32 |> string) + "us") |> (tagNumericLiteral >> Some)
                | ILFieldInit.UInt32 x -> (x |> int64 |> string) + "u" |> (tagNumericLiteral >> Some)
                | ILFieldInit.UInt64 x -> ((x |> int64 |> string) + "UL") |> (tagNumericLiteral >> Some)
                | ILFieldInit.Single d -> 
                    let s = d.ToString ("g12", CultureInfo.InvariantCulture)
                    let s = ensureFloat s
                    (s + "f") |> (tagNumericLiteral >> Some)
                | ILFieldInit.Double d -> 
                      let s = d.ToString ("g12", CultureInfo.InvariantCulture)
                      let s = ensureFloat s
                      s |> (tagNumericLiteral >> Some)
                | _ -> None
            | None -> None
        match textOpt with
        | None -> WordL.equals ^^ (comment "value unavailable")
        | Some s -> WordL.equals ^^ wordL s

    let layoutILEnumCase nm litVal =
        let nameL = ConvertLogicalNameToDisplayLayout (tagEnum >> wordL) nm
        WordL.bar ^^ nameL ^^ layoutILFieldInit litVal

module PrintTypes = 
    // Note: We need nice printing of constants in order to print literals and attributes 
    let layoutConst g ty c =
        let str = 
            match c with
            | Const.Bool x -> if x then keywordTrue else keywordFalse
            | Const.SByte x -> (x |> string)+"y" |> tagNumericLiteral
            | Const.Byte x -> (x |> string)+"uy" |> tagNumericLiteral
            | Const.Int16 x -> (x |> string)+"s" |> tagNumericLiteral
            | Const.UInt16 x -> (x |> string)+"us" |> tagNumericLiteral
            | Const.Int32 x -> (x |> string) |> tagNumericLiteral
            | Const.UInt32 x -> (x |> string)+"u" |> tagNumericLiteral
            | Const.Int64 x -> (x |> string)+"L" |> tagNumericLiteral
            | Const.UInt64 x -> (x |> string)+"UL" |> tagNumericLiteral
            | Const.IntPtr x -> (x |> string)+"n" |> tagNumericLiteral
            | Const.UIntPtr x -> (x |> string)+"un" |> tagNumericLiteral
            | Const.Single d -> 
                 let s = d.ToString("g12", CultureInfo.InvariantCulture)
                 let s = ensureFloat s
                 (s + "f") |> tagNumericLiteral
            | Const.Double d -> 
                let s = d.ToString("g12", CultureInfo.InvariantCulture)
                let s = ensureFloat s
                s |> tagNumericLiteral
            | Const.Char c -> "'" + c.ToString() + "'" |> tagStringLiteral
            | Const.String bs -> "\"" + bs + "\"" |> tagNumericLiteral
            | Const.Unit -> "()" |> tagPunctuation
            | Const.Decimal bs -> string bs + "M" |> tagNumericLiteral
            // either "null" or "the default value for a struct"
            | Const.Zero -> tagKeyword(if isRefTy g ty then "null" else "default")
        wordL str

    let layoutAccessibility (denv: DisplayEnv) accessibility itemL =
        let isInternalCompPath x = 
            match x with 
            | CompPath(ILScopeRef.Local, []) -> true 
            | _ -> false
        let (|Public|Internal|Private|) (TAccess p) = 
            match p with 
            | [] -> Public 
            | _ when List.forall isInternalCompPath p -> Internal 
            | _ -> Private
        match denv.contextAccessibility, accessibility with
        | Public, Internal -> WordL.keywordInternal ++ itemL   // print modifier, since more specific than context
        | Public, Private -> WordL.keywordPrivate ++ itemL     // print modifier, since more specific than context
        | Internal, Private -> WordL.keywordPrivate ++ itemL   // print modifier, since more specific than context
        | _ -> itemL

    /// Layout a reference to a type 
    let layoutTyconRef denv tcref = layoutTyconRefImpl false denv tcref

    /// Layout the flags of a member 
    let layoutMemberFlags (memFlags: SynMemberFlags) = 
        let stat = 
            if memFlags.IsInstance || (memFlags.MemberKind = SynMemberKind.Constructor) then emptyL 
            else WordL.keywordStatic

        let stat = 
            if memFlags.IsOverrideOrExplicitImpl then stat ++ WordL.keywordOverride
            else stat

        let stat = 
            if memFlags.IsDispatchSlot then stat ++ WordL.keywordAbstract
            elif memFlags.IsOverrideOrExplicitImpl then stat
            else
                match memFlags.MemberKind with 
                | SynMemberKind.ClassConstructor 
                | SynMemberKind.Constructor 
                | SynMemberKind.PropertyGetSet -> stat
                | SynMemberKind.Member 
                | SynMemberKind.PropertyGet 
                | SynMemberKind.PropertySet -> stat ++ WordL.keywordMember

        // let stat = if memFlags.IsFinal then stat ++ wordL "final" else stat in
        stat

    /// Layout a single attribute arg, following the cases of 'gen_attr_arg' in ilxgen.fs
    /// This is the subset of expressions we display in the NicePrint pretty printer 
    /// See also dataExprL - there is overlap between these that should be removed 
    let rec layoutAttribArg denv arg = 
        match arg with 
        | Expr.Const (c, _, ty) -> 
            if isEnumTy denv.g ty then 
                WordL.keywordEnum ^^ angleL (layoutType denv ty) ^^ bracketL (layoutConst denv.g ty c)
            else
                layoutConst denv.g ty c

        | Expr.Op (TOp.Array, [_elemTy], args, _) ->
            LeftL.leftBracketBar ^^ semiListL (List.map (layoutAttribArg denv) args) ^^ RightL.rightBracketBar

        // Detect 'typeof<ty>' calls 
        | TypeOfExpr denv.g ty ->
            LeftL.keywordTypeof ^^ wordL (tagPunctuation "<") ^^ layoutType denv ty ^^ rightL (tagPunctuation ">")

        // Detect 'typedefof<ty>' calls 
        | TypeDefOfExpr denv.g ty ->
            LeftL.keywordTypedefof ^^ wordL (tagPunctuation "<") ^^ layoutType denv ty ^^ rightL (tagPunctuation ">")

        | Expr.Op (TOp.Coerce, [tgtTy;_], [arg2], _) ->
            leftL (tagPunctuation "(") ^^ layoutAttribArg denv arg2 ^^ wordL (tagPunctuation ":>") ^^ layoutType denv tgtTy ^^ rightL (tagPunctuation ")")

        | AttribBitwiseOrExpr denv.g (arg1, arg2) ->
            layoutAttribArg denv arg1 ^^ wordL (tagPunctuation "|||") ^^ layoutAttribArg denv arg2

        // Detect explicit enum values 
        | EnumExpr denv.g arg1 ->
            WordL.keywordEnum ++ bracketL (layoutAttribArg denv arg1)

        | _ -> comment "(* unsupported attribute argument *)"

    /// Layout arguments of an attribute 'arg1, ..., argN' 
    and layoutAttribArgs denv args = 
        let argsL =  args |> List.map (fun (AttribExpr(e1, _)) -> layoutAttribArg denv e1)
        sepListL (rightL (tagPunctuation ",")) argsL

    /// Layout an attribute 'Type(arg1, ..., argN)' 
    //
    // REVIEW: we are ignoring "props" here
    and layoutAttrib denv (Attrib(tcref, _, args, _props, _, _, _)) = 
        let tcrefL = layoutTyconRefImpl true denv tcref
        let argsL = bracketL (layoutAttribArgs denv args)
        match args with 
        | [] -> tcrefL
        | _ -> tcrefL ++ argsL

    and layoutILAttribElement denv arg = 
        match arg with 
        | ILAttribElem.String (Some x) -> wordL (tagStringLiteral ("\"" + x + "\""))
        | ILAttribElem.String None -> wordL (tagStringLiteral "")
        | ILAttribElem.Bool x -> if x then WordL.keywordTrue else WordL.keywordFalse
        | ILAttribElem.Char x -> wordL (tagStringLiteral ("'" + x.ToString() + "'" ))
        | ILAttribElem.SByte x -> wordL (tagNumericLiteral ((x |> string)+"y"))
        | ILAttribElem.Int16 x -> wordL (tagNumericLiteral ((x |> string)+"s"))
        | ILAttribElem.Int32 x -> wordL (tagNumericLiteral (x |> string))
        | ILAttribElem.Int64 x -> wordL (tagNumericLiteral ((x |> string)+"L"))
        | ILAttribElem.Byte x -> wordL (tagNumericLiteral ((x |> string)+"uy"))
        | ILAttribElem.UInt16 x -> wordL (tagNumericLiteral ((x |> string)+"us"))
        | ILAttribElem.UInt32 x -> wordL (tagNumericLiteral ((x |> string)+"u"))
        | ILAttribElem.UInt64 x -> wordL (tagNumericLiteral ((x |> string)+"UL"))
        | ILAttribElem.Single x -> 
            let str =
                let s = x.ToString("g12", CultureInfo.InvariantCulture)
                let s = ensureFloat s 
                s + "f"
            wordL (tagNumericLiteral str)
        | ILAttribElem.Double x -> 
            let str =
                let s = x.ToString("g12", CultureInfo.InvariantCulture)
                let s = ensureFloat s 
                s
            wordL (tagNumericLiteral str)
        | ILAttribElem.Null -> wordL (tagKeyword "null")
        | ILAttribElem.Array (_, xs) -> 
            leftL (tagPunctuation "[|") ^^ semiListL (List.map (layoutILAttribElement denv) xs) ^^ RightL.rightBracketBar
        | ILAttribElem.Type (Some ty) -> 
            LeftL.keywordTypeof ^^ SepL.leftAngle ^^ PrintIL.layoutILType denv [] ty ^^ RightL.rightAngle
        | ILAttribElem.Type None -> wordL (tagText "")
        | ILAttribElem.TypeRef (Some ty) -> 
            LeftL.keywordTypedefof ^^ SepL.leftAngle ^^ PrintIL.layoutILTypeRef denv ty ^^ RightL.rightAngle
        | ILAttribElem.TypeRef None -> emptyL

    and layoutILAttrib denv (ty, args) = 
        let argsL = bracketL (sepListL (rightL (tagPunctuation ",")) (List.map (layoutILAttribElement denv) args))
        PrintIL.layoutILType denv [] ty ++ argsL

    /// Layout '[<attribs>]' above another block 
    and layoutAttribs denv startOpt isLiteral kind attrs restL = 
        
        let attrsL = 
            [ if denv.showAttributes then
                // Don't display DllImport and other attributes in generated signatures
                let attrs = attrs |> List.filter (IsMatchingFSharpAttributeOpt denv.g denv.g.attrib_DllImportAttribute >> not)
                let attrs = attrs |> List.filter (IsMatchingFSharpAttributeOpt denv.g denv.g.attrib_ContextStaticAttribute >> not)
                let attrs = attrs |> List.filter (IsMatchingFSharpAttributeOpt denv.g denv.g.attrib_ThreadStaticAttribute >> not)
                let attrs = attrs |> List.filter (IsMatchingFSharpAttribute denv.g denv.g.attrib_EntryPointAttribute >> not)
                let attrs = attrs |> List.filter (IsMatchingFSharpAttributeOpt denv.g denv.g.attrib_MarshalAsAttribute >> not)
                let attrs = attrs |> List.filter (IsMatchingFSharpAttribute denv.g denv.g.attrib_ReflectedDefinitionAttribute >> not)
                let attrs = attrs |> List.filter (IsMatchingFSharpAttribute denv.g denv.g.attrib_StructLayoutAttribute >> not)
                let attrs = attrs |> List.filter (IsMatchingFSharpAttribute denv.g denv.g.attrib_AutoSerializableAttribute >> not)
                let attrs = attrs |> List.filter (IsMatchingFSharpAttribute denv.g denv.g.attrib_LiteralAttribute >> not)
                let attrs = attrs |> List.filter (IsMatchingFSharpAttribute denv.g denv.g.attrib_MeasureAttribute >> not)
                let attrs = attrs |> List.filter (IsMatchingFSharpAttribute denv.g denv.g.attrib_StructAttribute >> not)
                let attrs = attrs |> List.filter (IsMatchingFSharpAttribute denv.g denv.g.attrib_ClassAttribute >> not)
                let attrs = attrs |> List.filter (IsMatchingFSharpAttribute denv.g denv.g.attrib_InterfaceAttribute >> not)
            
                for attr in attrs do
                    layoutAttrib denv attr

              // Always show the 'Struct', 'Class, 'Interface' attributes if needed
              match startOpt with 
              | Some "struct" ->
                  wordL (tagClass "Struct")
              | Some "class" ->
                  wordL (tagClass "Class")
              | Some "interface" ->
                  wordL (tagClass "Interface")
              | _ ->
                  ()

              // Always show the 'Literal' attribute if needed
              if isLiteral then
                wordL (tagClass "Literal")

              // Always show the 'Measure' attribute if needed
              if kind = TyparKind.Measure then
                wordL (tagClass "Measure")
            ]

        match attrsL with 
        | [] -> restL 
        | _ -> squareAngleL (sepListL (rightL (tagPunctuation ";")) attrsL) @@ restL
            
    and layoutTyparAttribs denv kind attrs restL =
        match attrs, kind with
        | [], TyparKind.Type -> restL 
        | _, _ -> squareAngleL (sepListL (rightL (tagPunctuation ";")) ((match kind with TyparKind.Type -> [] | TyparKind.Measure -> [wordL (tagText "Measure")]) @ List.map (layoutAttrib denv) attrs)) ^^ restL

    and layoutTyparRef denv (typar: Typar) =
        tagTypeParameter 
            (sprintf "%s%s%s"
                (if denv.showStaticallyResolvedTyparAnnotations then prefixOfStaticReq typar.StaticReq else "'")
                (if denv.showInferenceTyparAnnotations then prefixOfInferenceTypar typar else "")
                typar.DisplayName)
        |> mkNav typar.Range
        |> wordL

    /// Layout a single type parameter declaration, taking TypeSimplificationInfo into account
    /// There are several printing-cases for a typar:
    ///
    ///  'a              - is multiple occurrence.
    ///  _               - singleton occurrence, an underscore preferred over 'b. (OCaml accepts but does not print)
    ///  #Type           - inplace coercion constraint and singleton.
    ///  ('a :> Type)    - inplace coercion constraint not singleton.
    ///  ('a.opM: S->T) - inplace operator constraint.
    ///
    and layoutTyparRefWithInfo denv (env: SimplifyTypes.TypeSimplificationInfo) (typar: Typar) =
        let varL = layoutTyparRef denv typar
        let varL = if denv.showAttributes then layoutTyparAttribs denv typar.Kind typar.Attribs varL else varL

        match Zmap.tryFind typar env.inplaceConstraints with
        | Some typarConstraintTy ->
            if Zset.contains typar env.singletons then
                leftL (tagPunctuation "#") ^^ layoutTypeWithInfo denv env typarConstraintTy
            else
                (varL ^^ sepL (tagPunctuation ":>") ^^ layoutTypeWithInfo denv env typarConstraintTy) |> bracketL

        | _ -> varL

      
    /// Layout type parameter constraints, taking TypeSimplificationInfo into account 
    and layoutConstraintsWithInfo denv env cxs = 
        
        // Internally member constraints get attached to each type variable in their support. 
        // This means we get too many constraints being printed. 
        // So we normalize the constraints to eliminate duplicate member constraints 
        let cxs = 
            cxs
            |> ListSet.setify (fun (_, cx1) (_, cx2) ->
                match cx1, cx2 with 
                | TyparConstraint.MayResolveMember(traitInfo1, _),
                  TyparConstraint.MayResolveMember(traitInfo2, _) -> traitsAEquiv denv.g TypeEquivEnv.Empty traitInfo1 traitInfo2
                | _ -> false)

        let cxsL = List.collect (layoutConstraintWithInfo denv env) cxs
        match cxsL with 
        | [] -> emptyL 
        | _ -> 
            if denv.abbreviateAdditionalConstraints then 
                wordL (tagKeyword "when") ^^ wordL(tagText "<constraints>")
            elif denv.shortConstraints then 
                leftL (tagPunctuation "(") ^^ wordL (tagKeyword "requires") ^^ sepListL (wordL (tagKeyword "and")) cxsL ^^ rightL (tagPunctuation ")")
            else
                wordL (tagKeyword "when") ^^ sepListL (wordL (tagKeyword "and")) cxsL

    /// Layout constraints, taking TypeSimplificationInfo into account 
    and layoutConstraintWithInfo denv env (tp, tpc) =
        let longConstraintPrefix l = (layoutTyparRefWithInfo denv env tp |> addColonL) ^^ l
        match tpc with 
        | TyparConstraint.CoercesTo(tgtTy, _) -> 
            [layoutTyparRefWithInfo denv env tp ^^ wordL (tagOperator ":>") --- layoutTypeWithInfo denv env tgtTy]

        | TyparConstraint.MayResolveMember(traitInfo, _) ->
            [layoutTraitWithInfo denv env traitInfo]

        | TyparConstraint.DefaultsTo(_, ty, _) ->
              if denv.showTyparDefaultConstraints then 
                  [wordL (tagKeyword "default") ^^ (layoutTyparRefWithInfo denv env tp  |> addColonL) ^^ layoutTypeWithInfo denv env ty]
              else []

        | TyparConstraint.IsEnum(ty, _) ->
            if denv.shortConstraints then 
                [wordL (tagKeyword "enum")]
            else
                [longConstraintPrefix (layoutTypeAppWithInfoAndPrec denv env (wordL (tagKeyword "enum")) 2 true [ty])]

        | TyparConstraint.SupportsComparison _ ->
            if denv.shortConstraints then 
                [wordL (tagKeyword "comparison")]
            else
                [wordL (tagKeyword "comparison") |> longConstraintPrefix]

        | TyparConstraint.SupportsEquality _ ->
            if denv.shortConstraints then 
                [wordL (tagKeyword "equality")]
            else
                [wordL (tagKeyword "equality") |> longConstraintPrefix]

        | TyparConstraint.IsDelegate(aty, bty, _) ->
            if denv.shortConstraints then 
                [WordL.keywordDelegate]
            else
                [layoutTypeAppWithInfoAndPrec denv env WordL.keywordDelegate 2 true [aty;bty] |> longConstraintPrefix]

        | TyparConstraint.SupportsNull _ ->
            [wordL (tagKeyword "null") |> longConstraintPrefix]

        | TyparConstraint.IsNonNullableStruct _ ->
            if denv.shortConstraints then 
                [wordL (tagText "value type")]
            else
                [WordL.keywordStruct |> longConstraintPrefix]

        | TyparConstraint.IsUnmanaged _ ->
            if denv.shortConstraints then
                [wordL (tagKeyword "unmanaged")]
            else
                [wordL (tagKeyword "unmanaged") |> longConstraintPrefix]

        | TyparConstraint.IsReferenceType _ ->
            if denv.shortConstraints then 
                [wordL (tagText "reference type")]
            else
                [(wordL (tagKeyword "not") ^^ wordL(tagKeyword "struct")) |> longConstraintPrefix]

        | TyparConstraint.SimpleChoice(tys, _) ->
            [bracketL (sepListL (sepL (tagPunctuation "|")) (List.map (layoutTypeWithInfo denv env) tys)) |> longConstraintPrefix]

        | TyparConstraint.RequiresDefaultConstructor _ -> 
            if denv.shortConstraints then 
                [wordL (tagKeyword "default") ^^ wordL (tagKeyword "constructor")]
            else
                [bracketL (
                    (WordL.keywordNew |> addColonL) ^^
                    WordL.structUnit ^^ 
                    WordL.arrow ^^
                    (layoutTyparRefWithInfo denv env tp)) |> longConstraintPrefix]

    and layoutTraitWithInfo denv env (TTrait(tys, nm, memFlags, argTys, retTy, _)) =
        let nameL = ConvertValLogicalNameToDisplayLayout false (tagMember >> wordL) nm
        if denv.shortConstraints then 
            WordL.keywordMember ^^ nameL
        else
            let retTy = GetFSharpViewOfReturnType denv.g retTy
            let stat = layoutMemberFlags memFlags
            let tys = ListSet.setify (typeEquiv denv.g) tys
            let tysL = 
                match tys with 
                | [ty] -> layoutTypeWithInfo denv env ty 
                | tys -> bracketL (layoutTypesWithInfoAndPrec denv env 2 (wordL (tagKeyword "or")) tys)

            let argTysL = layoutTypesWithInfoAndPrec denv env 2 (wordL (tagPunctuation "*")) argTys
            let retTyL = layoutReturnType denv env retTy
            let sigL =
                match argTys with
                | [] -> retTyL
                | _ -> curriedLayoutsL "->" [argTysL] retTyL
            (tysL |> addColonL) --- bracketL (stat ++ (nameL |> addColonL) --- sigL)

    /// Layout a unit of measure expression 
    and layoutMeasure denv unt =
        let sortVars vs = vs |> List.sortBy (fun (tp: Typar, _) -> tp.DisplayName) 
        let sortCons cs = cs |> List.sortBy (fun (tcref: TyconRef, _) -> tcref.DisplayName) 
        let negvs, posvs = ListMeasureVarOccsWithNonZeroExponents unt |> sortVars |> List.partition (fun (_, e) -> SignRational e < 0)
        let negcs, poscs = ListMeasureConOccsWithNonZeroExponents denv.g false unt |> sortCons |> List.partition (fun (_, e) -> SignRational e < 0)
        let unparL uv = layoutTyparRef denv uv
        let unconL tc = layoutTyconRef denv tc
        let rationalL e = wordL (tagNumericLiteral (RationalToString e))
        let measureToPowerL x e = if e = OneRational then x else x -- wordL (tagPunctuation "^") -- rationalL e
        let prefix = spaceListL (List.map (fun (v, e) -> measureToPowerL (unparL v) e) posvs @
                                 List.map (fun (c, e) -> measureToPowerL (unconL c) e) poscs)
        let postfix = spaceListL (List.map (fun (v, e) -> measureToPowerL (unparL v) (NegRational e)) negvs @
                                  List.map (fun (c, e) -> measureToPowerL (unconL c) (NegRational e)) negcs)
        match (negvs, negcs) with 
        | [], [] -> (match posvs, poscs with [], [] -> wordL (tagNumericLiteral "1") | _ -> prefix)
        | _ -> prefix ^^ sepL (tagPunctuation "/") ^^ (if List.length negvs + List.length negcs > 1 then sepL (tagPunctuation "(") ^^ postfix ^^ sepL (tagPunctuation ")") else postfix)

    /// Layout type arguments, either NAME<ty, ..., ty> or (ty, ..., ty) NAME *)
    and layoutTypeAppWithInfoAndPrec denv env tcL prec prefix argTys =
        if prefix then 
            match argTys with
            | [] -> tcL
            | [argTy] -> tcL ^^ sepL (tagPunctuation "<") ^^ (layoutTypeWithInfoAndPrec denv env 4 argTy) ^^ rightL (tagPunctuation">")
            | _ -> bracketIfL (prec <= 1) (tcL ^^ angleL (layoutTypesWithInfoAndPrec denv env 2 (sepL (tagPunctuation ",")) argTys))
        else
            match argTys with
            | [] -> tcL
            | [arg] -> layoutTypeWithInfoAndPrec denv env 2 arg ^^ tcL
            | args -> bracketIfL (prec <= 1) (bracketL (layoutTypesWithInfoAndPrec denv env 2 (sepL (tagPunctuation ",")) args) --- tcL)

    /// Layout a type, taking precedence into account to insert brackets where needed
    and layoutTypeWithInfoAndPrec denv env prec ty =
        let g = denv.g
        match stripTyparEqns ty with 

        // Always prefer to format 'byref<ty, ByRefKind.In>' as 'inref<ty>'
        | ty when isInByrefTy g ty && (match ty with TType_app (tc, _, _) when g.inref_tcr.CanDeref && tyconRefEq g tc g.byref2_tcr -> true | _ -> false) ->
            layoutTypeWithInfoAndPrec denv env prec (mkInByrefTy g (destByrefTy g ty))

        // Always prefer to format 'byref<ty, ByRefKind.Out>' as 'outref<ty>'
        | ty when isOutByrefTy g ty && (match ty with TType_app (tc, _, _) when g.outref_tcr.CanDeref && tyconRefEq g tc g.byref2_tcr -> true | _ -> false) ->
            layoutTypeWithInfoAndPrec denv env prec (mkOutByrefTy g (destByrefTy g ty))

        // Always prefer to format 'byref<ty, ByRefKind.InOut>' as 'byref<ty>'
        | ty when isByrefTy g ty && (match ty with TType_app (tc, _, _) when g.byref_tcr.CanDeref && tyconRefEq g tc g.byref2_tcr -> true | _ -> false) ->
            layoutTypeWithInfoAndPrec denv env prec (mkByrefTy g (destByrefTy g ty))

        // Always prefer 'float' to 'float<1>'
        | TType_app (tc, args, _) when tc.IsMeasureableReprTycon && List.forall (isDimensionless g) args ->
          layoutTypeWithInfoAndPrec denv env prec (reduceTyconRefMeasureableOrProvided g tc args)

        // Layout a type application
        | TType_ucase (UnionCaseRef(tc, _), args)
        | TType_app (tc, args, _) ->
          let prefix = usePrefix denv tc
          layoutTypeAppWithInfoAndPrec denv env (layoutTyconRef denv tc) prec prefix args

        // Layout a tuple type 
        | TType_anon (anonInfo, tys) ->
            let core = sepListL (rightL (tagPunctuation ";")) (List.map2 (fun nm ty -> wordL (tagField nm) ^^ rightL (tagPunctuation ":") ^^ layoutTypeWithInfoAndPrec denv env prec ty) (Array.toList anonInfo.SortedNames) tys)
            if evalAnonInfoIsStruct anonInfo then 
                WordL.keywordStruct --- braceBarL core
            else 
                braceBarL core

        // Layout a tuple type 
        | TType_tuple (tupInfo, t) ->
            let elsL = layoutTypesWithInfoAndPrec denv env 2 (wordL (tagPunctuation "*")) t
            if evalTupInfoIsStruct tupInfo then 
                WordL.keywordStruct --- bracketL elsL
            else 
                bracketIfL (prec <= 2) elsL

        // Layout a first-class generic type. 
        | TType_forall (tps, tau) ->
            let tauL = layoutTypeWithInfoAndPrec denv env prec tau
            match tps with 
            | [] -> tauL
            | [h] -> layoutTyparRefWithInfo denv env h ^^ rightL (tagPunctuation ".") --- tauL
            | h :: t -> spaceListL (List.map (layoutTyparRefWithInfo denv env) (h :: t)) ^^ rightL (tagPunctuation ".") --- tauL

        | TType_fun _ ->
            let argTys, retTy = stripFunTy g ty
            let retTyL = layoutTypeWithInfoAndPrec denv env 5 retTy
            let argTysL = argTys |> List.map (layoutTypeWithInfoAndPrec denv env 4)
            let funcTyL = curriedLayoutsL "->" argTysL retTyL
            bracketIfL (prec <= 4) funcTyL

        // Layout a type variable . 
        | TType_var (r, _) ->
            layoutTyparRefWithInfo denv env r

        | TType_measure unt -> layoutMeasure denv unt

    /// Layout a list of types, separated with the given separator, either '*' or ','
    and layoutTypesWithInfoAndPrec denv env prec sep typl = 
        sepListL sep (List.map (layoutTypeWithInfoAndPrec denv env prec) typl)

    and layoutReturnType denv env retTy = layoutTypeWithInfoAndPrec denv env 4 retTy

    /// Layout a single type, taking TypeSimplificationInfo into account 
    and layoutTypeWithInfo denv env ty = 
        layoutTypeWithInfoAndPrec denv env 5 ty

    and layoutType denv ty = 
        layoutTypeWithInfo denv SimplifyTypes.typeSimplificationInfo0 ty

    // Format each argument, including its name and type 
    let layoutArgInfo denv env (ty, argInfo: ArgReprInfo) = 
        let g = denv.g
       
        // Detect an optional argument 
        let isOptionalArg = HasFSharpAttribute g g.attrib_OptionalArgumentAttribute argInfo.Attribs
        let isParamArray = HasFSharpAttribute g g.attrib_ParamArrayAttribute argInfo.Attribs

        match argInfo.Name, isOptionalArg, isParamArray, tryDestOptionTy g ty with 
        // Layout an optional argument 
        | Some id, true, _, ValueSome ty -> 
            let idL = ConvertValLogicalNameToDisplayLayout false (tagParameter >> rightL) id.idText
            LeftL.questionMark ^^ 
            (idL |> addColonL) ^^
            layoutTypeWithInfoAndPrec denv env 2 ty 

        // Layout an unnamed argument 
        | None, _, _, _ -> 
            layoutTypeWithInfoAndPrec denv env 2 ty

        // Layout a named argument 
        | Some id, _, isParamArray, _ -> 
            let idL = ConvertValLogicalNameToDisplayLayout false (tagParameter >> wordL) id.idText
            let prefix =
                if isParamArray then
                    layoutBuiltinAttribute denv g.attrib_ParamArrayAttribute ^^ idL
                else
                    idL
            (prefix |> addColonL) ^^ layoutTypeWithInfoAndPrec denv env 2 ty

    let layoutCurriedArgInfos denv env argInfos =
        argInfos 
        |> List.mapSquared (layoutArgInfo denv env)
        |> List.map (sepListL (wordL (tagPunctuation "*")))

    let layoutGenericParameterTypes denv env genParamTys = 
      match genParamTys with
      | [] -> emptyL
      | _ ->
        wordL (tagPunctuation "<")
        ^^
        (
          genParamTys
          |> List.map (layoutTypeWithInfoAndPrec denv env 4)
          |> sepListL (wordL (tagPunctuation ","))
        ) 
        ^^
        wordL (tagPunctuation ">")

    /// Layout a single type used as the type of a member or value 
    let layoutTopType denv env argInfos retTy cxs =
        let retTyL = layoutReturnType denv env retTy
        let cxsL = layoutConstraintsWithInfo denv env cxs
        match argInfos with
        | [] -> retTyL --- cxsL
        | _ -> 
            let retTyDelim = if denv.useColonForReturnType then ":" else "->"
            let allArgsL = layoutCurriedArgInfos denv env argInfos
            curriedLayoutsL retTyDelim allArgsL retTyL --- cxsL

    /// Layout type parameters
    let layoutTyparDecls denv nmL prefix (typars: Typars) =
        let env = SimplifyTypes.typeSimplificationInfo0 
        let tpcs = typars |> List.collect (fun tp -> List.map (fun tpc -> tp, tpc) tp.Constraints) 
        match typars, tpcs with 
        | [], []  -> 
            nmL

        | [h], [] when not prefix -> 
            layoutTyparRefWithInfo denv env h --- nmL

        | _ -> 
            let tpcsL = layoutConstraintsWithInfo denv env tpcs
            let coreL = sepListL (sepL (tagPunctuation ",")) (List.map (layoutTyparRefWithInfo denv env) typars)
            if prefix || not (isNil tpcs) then
                nmL ^^ angleL (coreL --- tpcsL)
            else
                bracketL coreL --- nmL

    let layoutTyparConstraint denv (tp, tpc) = 
        match layoutConstraintWithInfo denv SimplifyTypes.typeSimplificationInfo0 (tp, tpc) with 
        | h :: _ -> h 
        | [] -> emptyL

    let prettyLayoutOfInstAndSig denv (typarInst, tys, retTy) =
        let (prettyTyparInst, prettyTys, prettyRetTy), cxs = PrettyTypes.PrettifyInstAndSig denv.g (typarInst, tys, retTy)
        let env = SimplifyTypes.CollectInfo true (prettyRetTy :: prettyTys) cxs
        let prettyTysL = List.map (layoutTypeWithInfo denv env) prettyTys
        let prettyRetTyL = layoutTopType denv env [[]] prettyRetTy []
        prettyTyparInst, (prettyTys, prettyRetTy), (prettyTysL, prettyRetTyL), layoutConstraintsWithInfo denv env env.postfixConstraints

    let prettyLayoutOfTopTypeInfoAux denv prettyArgInfos prettyRetTy cxs = 
        let env = SimplifyTypes.CollectInfo true (prettyRetTy :: List.collect (List.map fst) prettyArgInfos) cxs
        layoutTopType denv env prettyArgInfos prettyRetTy env.postfixConstraints

    // Oddly this is called in multiple places with argInfos=[] and denv.useColonForReturnType=true, as a complex
    // way of giving give ": ty"
    let prettyLayoutOfUncurriedSig denv typarInst argInfos retTy = 
        let (prettyTyparInst, prettyArgInfos, prettyRetTy), cxs = PrettyTypes.PrettifyInstAndUncurriedSig denv.g (typarInst, argInfos, retTy)
        prettyTyparInst, prettyLayoutOfTopTypeInfoAux denv [prettyArgInfos] prettyRetTy cxs

    let prettyLayoutOfCurriedMemberSig denv typarInst argInfos retTy parentTyparTys = 
        let (prettyTyparInst, parentTyparTys, argInfos, retTy), cxs = PrettyTypes.PrettifyInstAndCurriedSig denv.g (typarInst, parentTyparTys, argInfos, retTy)
        // Filter out the parent typars, which don't get shown in the member signature 
        let cxs = cxs |> List.filter (fun (tp, _) -> not (parentTyparTys |> List.exists (fun ty -> match tryDestTyparTy denv.g ty with ValueSome destTypar -> typarEq tp destTypar | _ -> false))) 
        prettyTyparInst, prettyLayoutOfTopTypeInfoAux denv argInfos retTy cxs

    let prettyArgInfos denv allTyparInst =
        function 
        | [] -> [(denv.g.unit_ty, ValReprInfo.unnamedTopArg1)] 
        | infos -> infos |> List.map (map1Of2 (instType allTyparInst)) 

    // Layout: type spec - class, datatype, record, abbrev 
    let prettyLayoutOfMemberSigCore denv memberToParentInst (typarInst, methTypars: Typars, argInfos, retTy) = 
        let niceMethodTypars, allTyparInst = 
            let methTyparNames = methTypars |> List.mapi (fun i tp -> if (PrettyTypes.NeedsPrettyTyparName tp) then sprintf "a%d" (List.length memberToParentInst + i) else tp.Name)
            PrettyTypes.NewPrettyTypars memberToParentInst methTypars methTyparNames

        let retTy = instType allTyparInst retTy
        let argInfos = argInfos |> List.map (prettyArgInfos denv allTyparInst) 

        // Also format dummy types corresponding to any type variables on the container to make sure they 
        // aren't chosen as names for displayed variables. 
        let memberParentTypars = List.map fst memberToParentInst
        let parentTyparTys = List.map (mkTyparTy >> instType allTyparInst) memberParentTypars
        let prettyTyparInst, layout = prettyLayoutOfCurriedMemberSig denv typarInst argInfos retTy parentTyparTys

        prettyTyparInst, niceMethodTypars, layout

    let prettyLayoutOfMemberType denv vref typarInst argInfos retTy = 
        match PartitionValRefTypars denv.g vref with
        | Some(_, _, memberMethodTypars, memberToParentInst, _) ->
            prettyLayoutOfMemberSigCore denv memberToParentInst (typarInst, memberMethodTypars, argInfos, retTy)
        | None -> 
            let prettyTyparInst, layout = prettyLayoutOfUncurriedSig denv typarInst (List.concat argInfos) retTy 
            prettyTyparInst, [], layout

    let prettyLayoutOfMemberSig denv (memberToParentInst, nm, methTypars, argInfos, retTy) = 
        let _, niceMethodTypars, tauL = prettyLayoutOfMemberSigCore denv memberToParentInst (emptyTyparInst, methTypars, argInfos, retTy)
        let nameL = ConvertValLogicalNameToDisplayLayout false (tagMember >> wordL) nm
        let nameL =
            if denv.showTyparBinding then
                layoutTyparDecls denv nameL true niceMethodTypars
            else
                nameL
        (nameL |> addColonL) ^^ tauL

    /// layouts the elements of an unresolved overloaded method call:
    /// argInfos: unammed and named arguments
    /// retTy: return type
    /// genParamTy: generic parameter types
    let prettyLayoutsOfUnresolvedOverloading denv argInfos retTy genParamTys =

        let _niceMethodTypars, typarInst =
            let memberToParentInst = List.empty
            let typars = argInfos |> List.choose (function TType_var (typar, _),_ -> Some typar | _ -> None)
            let methTyparNames = typars |> List.mapi (fun i tp -> if (PrettyTypes.NeedsPrettyTyparName tp) then sprintf "a%d" (List.length memberToParentInst + i) else tp.Name)
            PrettyTypes.NewPrettyTypars memberToParentInst typars methTyparNames

        let retTy = instType typarInst retTy
        let argInfos = prettyArgInfos denv typarInst argInfos
        let argInfos,retTy,genParamTys, cxs =
            // using 0, 1, 2 as discriminant for return, arguments and generic parameters
            // respectively, in order to easily retrieve each of the types with their
            // expected quality below.
            let typesWithDiscrimants =
                [
                    yield 0, retTy 
                    for ty,_ in argInfos do
                        yield 1, ty
                    for ty in genParamTys do
                        yield 2, ty
                ]
            let typesWithDiscrimants,typarsAndCxs = PrettyTypes.PrettifyDiscriminantAndTypePairs denv.g typesWithDiscrimants
            let retTy = typesWithDiscrimants |> List.find (function 0, _ -> true | _ -> false) |> snd
            let argInfos = 
                typesWithDiscrimants
                |> List.choose (function 1,ty -> Some ty | _ -> None)
                |> List.map2 (fun (_, argInfo) tTy -> tTy, argInfo) argInfos
            let genParamTys = 
                typesWithDiscrimants
                |> List.choose (function 2,ty -> Some ty | _ -> None)
              
            argInfos, retTy, genParamTys, typarsAndCxs

        let env = SimplifyTypes.CollectInfo true (List.collect (List.map fst) [argInfos]) cxs
        let cxsL = layoutConstraintsWithInfo denv env env.postfixConstraints

        (List.foldBack (---) (layoutCurriedArgInfos denv env [argInfos]) cxsL,
            layoutReturnType denv env retTy,
            layoutGenericParameterTypes denv env genParamTys)

    let prettyLayoutOfType denv ty = 
        let ty, cxs = PrettyTypes.PrettifyType denv.g ty
        let env = SimplifyTypes.CollectInfo true [ty] cxs
        let cxsL = layoutConstraintsWithInfo denv env env.postfixConstraints
        layoutTypeWithInfoAndPrec denv env 2 ty --- cxsL

    let prettyLayoutOfTypeNoConstraints denv ty = 
        let ty, _cxs = PrettyTypes.PrettifyType denv.g ty
        layoutTypeWithInfoAndPrec denv SimplifyTypes.typeSimplificationInfo0 5 ty

    let layoutOfValReturnType denv (vref: ValRef) =
        match vref.ValReprInfo with 
        | None ->
            let tau = vref.TauType
            let _argTysl, retTy = stripFunTy denv.g tau
            layoutReturnType denv SimplifyTypes.typeSimplificationInfo0 retTy
        | Some (ValReprInfo(_typars, argInfos, _retInfo)) -> 
            let tau = vref.TauType
            let _c, retTy = GetTopTauTypeInFSharpForm denv.g argInfos tau Range.range0
            layoutReturnType denv SimplifyTypes.typeSimplificationInfo0 retTy

    let layoutAssemblyName _denv (ty: TType) =
        ty.GetAssemblyName()

/// Printing TAST objects
module PrintTastMemberOrVals =
    open PrintTypes 

    let mkInlineL denv (v: Val) nameL = 
        if v.MustInline && not denv.suppressInlineKeyword then 
            wordL (tagKeyword "inline") ++ nameL 
        else 
            nameL

    let layoutMemberName (denv: DisplayEnv) (vref: ValRef) niceMethodTypars tagFunction name =
        let nameL = ConvertValLogicalNameToDisplayLayout vref.IsBaseVal (tagFunction >> mkNav vref.DefinitionRange >> wordL) name
        let nameL =
            if denv.showMemberContainers then 
                layoutTyconRef denv vref.MemberApparentEntity ^^ SepL.dot ^^ nameL
            else
                nameL
        let nameL = if denv.showTyparBinding then layoutTyparDecls denv nameL true niceMethodTypars else nameL
        let nameL = layoutAccessibility denv vref.Accessibility nameL
        nameL

    let prettyLayoutOfMemberShortOption denv typarInst (v: Val) short =
        let vref = mkLocalValRef v
        let membInfo = Option.get vref.MemberInfo
        let stat = layoutMemberFlags membInfo.MemberFlags
        let _tps, argInfos, retTy, _ = GetTypeOfMemberInFSharpForm denv.g vref
        
        if short then
            for argInfo in argInfos do
                for _,info in argInfo do
                    info.Attribs <- []
                    info.Name <- None

        let prettyTyparInst, memberL =
            match membInfo.MemberFlags.MemberKind with
            | SynMemberKind.Member ->
                let prettyTyparInst, niceMethodTypars,tauL = prettyLayoutOfMemberType denv vref typarInst argInfos retTy
                let resL =
                    if short then tauL
                    else
                        let nameL = layoutMemberName denv vref niceMethodTypars tagMember vref.DisplayNameCoreMangled
                        let nameL = if short then nameL else mkInlineL denv vref.Deref nameL
                        stat --- ((nameL  |> addColonL) ^^ tauL)
                prettyTyparInst, resL

            | SynMemberKind.ClassConstructor
            | SynMemberKind.Constructor ->
                let prettyTyparInst, _, tauL = prettyLayoutOfMemberType denv vref typarInst argInfos retTy
                let resL = 
                    if short then tauL
                    else
                        let newL = layoutAccessibility denv vref.Accessibility WordL.keywordNew
                        stat ++ (newL |> addColonL) ^^ tauL
                prettyTyparInst, resL

            | SynMemberKind.PropertyGetSet ->
                emptyTyparInst, stat

            | SynMemberKind.PropertyGet ->
                if isNil argInfos then
                    // use error recovery because intellisense on an incomplete file will show this
                    errorR(Error(FSComp.SR.tastInvalidFormForPropertyGetter(), vref.Id.idRange))
                    let nameL = layoutMemberName denv vref [] tagProperty vref.DisplayNameCoreMangled
                    let resL =
                        if short then nameL --- (WordL.keywordWith ^^ WordL.keywordGet)
                        else stat --- nameL --- (WordL.keywordWith ^^ WordL.keywordGet)
                    emptyTyparInst, resL
                else
                    let argInfos =
                        match argInfos with
                        | [[(ty, _)]] when isUnitTy denv.g ty -> []
                        | _ -> argInfos
                    let prettyTyparInst, niceMethodTypars,tauL = prettyLayoutOfMemberType denv vref typarInst argInfos retTy
                    let resL =
                        if short then
                            if isNil argInfos then tauL
                            else tauL --- (WordL.keywordWith ^^ WordL.keywordGet)
                        else
                            let nameL = layoutMemberName denv vref niceMethodTypars tagProperty vref.DisplayNameCoreMangled
                            stat --- ((nameL  |> addColonL) ^^ (if isNil argInfos then tauL else tauL --- (WordL.keywordWith ^^ WordL.keywordGet)))
                    prettyTyparInst, resL

            | SynMemberKind.PropertySet ->
                if argInfos.Length <> 1 || isNil argInfos.Head then
                    // use error recovery because intellisense on an incomplete file will show this
                    errorR(Error(FSComp.SR.tastInvalidFormForPropertySetter(), vref.Id.idRange))
                    let nameL = layoutMemberName denv vref [] tagProperty vref.DisplayNameCoreMangled
                    let resL = stat --- nameL --- (WordL.keywordWith ^^ WordL.keywordSet)
                    emptyTyparInst, resL
                else
                    let argInfos, valueInfo = List.frontAndBack argInfos.Head
                    let prettyTyparInst, niceMethodTypars, tauL = prettyLayoutOfMemberType denv vref typarInst (if isNil argInfos then [] else [argInfos]) (fst valueInfo)
                    let resL =
                        if short then
                            (tauL --- (WordL.keywordWith ^^ WordL.keywordSet))
                        else
                            let nameL = layoutMemberName denv vref niceMethodTypars tagProperty vref.DisplayNameCoreMangled
                            stat --- ((nameL |> addColonL) ^^ (tauL --- (WordL.keywordWith ^^ WordL.keywordSet)))
                    prettyTyparInst, resL

        prettyTyparInst, memberL

    let prettyLayoutOfMember denv typarInst (v:Val) = prettyLayoutOfMemberShortOption denv typarInst v false

    let prettyLayoutOfMemberNoInstShort denv v = 
        prettyLayoutOfMemberShortOption denv emptyTyparInst v true |> snd

    let layoutOfLiteralValue literalValue =
        let literalValue =
            match literalValue with
            | Const.Bool value -> if value then WordL.keywordTrue else WordL.keywordFalse
            | Const.SByte _
            | Const.Byte _
            | Const.Int16 _
            | Const.UInt16 _
            | Const.Int32 _
            | Const.UInt32 _
            | Const.Int64 _
            | Const.UInt64 _
            | Const.IntPtr _
            | Const.UIntPtr _
            | Const.Single _
            | Const.Double _
            | Const.Decimal _ -> literalValue.ToString() |> tagNumericLiteral |> wordL
            | Const.Char _
            | Const.String _ -> literalValue.ToString() |> tagStringLiteral |> wordL
            | Const.Unit
            | Const.Zero -> literalValue.ToString() |> tagText |> wordL
        WordL.equals ++ literalValue

    let layoutNonMemberVal denv (tps, v: Val, tau, cxs) =
        let env = SimplifyTypes.CollectInfo true [tau] cxs
        let cxs = env.postfixConstraints
        let valReprInfo = arityOfValForDisplay v
        let argInfos, retTy = GetTopTauTypeInFSharpForm denv.g valReprInfo.ArgInfos tau v.Range
        let nameL =

            let tagF =
                if isForallFunctionTy denv.g v.Type && not (isDiscard v.DisplayNameCore) then
                    if IsOperatorDisplayName v.DisplayName then
                        tagOperator
                    else
                        tagFunction
                elif not v.IsCompiledAsTopLevel && not(isDiscard v.DisplayNameCore) then
                    tagLocal
                elif v.IsModuleBinding then
                    tagModuleBinding
                else
                    tagUnknownEntity

            v.DisplayName
            |> tagF
            |> mkNav v.DefinitionRange
            |> wordL 
        let nameL = layoutAccessibility denv v.Accessibility nameL
        let nameL = 
            if v.IsMutable && not denv.suppressMutableKeyword then 
                wordL (tagKeyword "mutable") ++ nameL 
                else 
                    nameL
        let nameL = mkInlineL denv v nameL

        let isOverGeneric = List.length (Zset.elements (freeInType CollectTyparsNoCaching tau).FreeTypars) < List.length tps // Bug: 1143 
        let isTyFunction = v.IsTypeFunction // Bug: 1143, and innerpoly tests 
        let typarBindingsL = 
            if isTyFunction || isOverGeneric || denv.showTyparBinding then 
                layoutTyparDecls denv nameL true tps 
            else nameL
        let valAndTypeL = (WordL.keywordVal ^^ (typarBindingsL |> addColonL)) --- layoutTopType denv env argInfos retTy cxs
        let valAndTypeL =
            match denv.generatedValueLayout v with
            | None -> valAndTypeL
            | Some rhsL -> (valAndTypeL ++ WordL.equals) --- rhsL
        match v.LiteralValue with
        | Some literalValue -> valAndTypeL --- layoutOfLiteralValue literalValue
        | None -> valAndTypeL

    let prettyLayoutOfValOrMember denv infoReader typarInst (vref: ValRef) =
        let prettyTyparInst, valL =
            match vref.MemberInfo with 
            | None ->
                let tps, tau = vref.GeneralizedType

                // adjust the type in case this is the 'this' pointer stored in a reference cell
                let tau = StripSelfRefCell(denv.g, vref.BaseOrThisInfo, tau)

                let (prettyTyparInst, prettyTypars, prettyTauTy), cxs = PrettyTypes.PrettifyInstAndTyparsAndType denv.g (typarInst, tps, tau)
                let resL = layoutNonMemberVal denv (prettyTypars, vref.Deref, prettyTauTy, cxs)
                prettyTyparInst, resL
            | Some _ -> 
                prettyLayoutOfMember denv typarInst vref.Deref

        let valL =
            valL
            |> layoutAttribs denv None vref.LiteralValue.IsSome TyparKind.Type vref.Attribs
            |> layoutXmlDocOfVal denv infoReader vref

        prettyTyparInst, valL

    let prettyLayoutOfValOrMemberNoInst denv infoReader v =
        prettyLayoutOfValOrMember denv infoReader emptyTyparInst v |> snd

let layoutTyparConstraint denv x = x |> PrintTypes.layoutTyparConstraint denv 

let outputType denv os x = x |> PrintTypes.layoutType denv |> bufferL os

let layoutType denv x = x |> PrintTypes.layoutType denv

let outputTypars denv nm os x = x |> PrintTypes.layoutTyparDecls denv (wordL nm) true |> bufferL os

let outputTyconRef denv os x = x |> PrintTypes.layoutTyconRef denv |> bufferL os

let layoutTyconRef denv x = x |> PrintTypes.layoutTyconRef denv

let layoutConst g ty c = PrintTypes.layoutConst g ty c

let prettyLayoutOfMemberSig denv x = x |> PrintTypes.prettyLayoutOfMemberSig denv 

let prettyLayoutOfUncurriedSig denv argInfos tau = PrintTypes.prettyLayoutOfUncurriedSig denv argInfos tau

let prettyLayoutsOfUnresolvedOverloading denv argInfos retTy genericParameters = PrintTypes.prettyLayoutsOfUnresolvedOverloading denv argInfos retTy genericParameters

//-------------------------------------------------------------------------

/// Printing info objects
module InfoMemberPrinting = 

    /// Format the arguments of a method
    let layoutParamData denv (ParamData(isParamArray, _isInArg, _isOutArg, optArgInfo, _callerInfo, nmOpt, _reflArgInfo, pty)) =
        let isOptArg = optArgInfo.IsOptional
        // detect parameter type, if ptyOpt is None - this is .NET style optional argument
        let ptyOpt = tryDestOptionTy denv.g pty

        match isParamArray, nmOpt, isOptArg with 
        // Layout an optional argument 
        | _, Some id, true -> 
            let idL = ConvertValLogicalNameToDisplayLayout false (tagParameter >> rightL) id.idText
            let pty = match ptyOpt with ValueSome x -> x | _ -> pty
            LeftL.questionMark ^^
            (idL |> addColonL) ^^
            PrintTypes.layoutType denv pty

        // Layout an unnamed argument 
        | _, None, _ -> 
            PrintTypes.layoutType denv pty

        // Layout a named ParamArray argument 
        | true, Some id, _ -> 
            let idL = ConvertValLogicalNameToDisplayLayout false (tagParameter >> wordL) id.idText
            layoutBuiltinAttribute denv denv.g.attrib_ParamArrayAttribute ^^
            (idL  |> addColonL) ^^
            PrintTypes.layoutType denv pty

        // Layout a named normal argument 
        | false, Some id, _ -> 
            let idL = ConvertValLogicalNameToDisplayLayout false (tagParameter >> wordL) id.idText
            (idL  |> addColonL) ^^
            PrintTypes.layoutType denv pty

    let formatParamDataToBuffer denv os pd =
        layoutParamData denv pd |> bufferL os
        
    /// Format a method info using "F# style".
    //
    // That is, this style:
    //     new: argName1: argType1 * ... * argNameN: argTypeN -> retType
    //     Method: argName1: argType1 * ... * argNameN: argTypeN -> retType
    //
    let layoutMethInfoFSharpStyleCore (infoReader: InfoReader) m denv (minfo: MethInfo) minst =
        let amap = infoReader.amap

        match minfo.ArbitraryValRef with
        | Some vref ->
            PrintTastMemberOrVals.prettyLayoutOfValOrMemberNoInst denv infoReader vref
        | None ->
            let layout = 
                if not minfo.IsConstructor && not minfo.IsInstance then WordL.keywordStatic
                else emptyL

            let nameL =
                if minfo.IsConstructor then
                    WordL.keywordNew
                else
                    let idL = ConvertValLogicalNameToDisplayLayout false (tagMethod >> tagNavArbValRef minfo.ArbitraryValRef >> wordL) minfo.LogicalName
                    WordL.keywordMember ^^
                    PrintTypes.layoutTyparDecls denv idL true minfo.FormalMethodTypars

            let layout = layout ^^ (nameL |> addColonL)
            let layout = layoutXmlDocOfMethInfo denv infoReader minfo layout

            let paramsL =
                let paramDatas = minfo.GetParamDatas(amap, m, minst)
                if List.forall isNil paramDatas then
                    WordL.structUnit
                else
                    sepListL WordL.arrow (List.map ((List.map (layoutParamData denv)) >> sepListL WordL.star) paramDatas)

            let layout = layout ^^ paramsL
            
            let retL =
                let retTy = minfo.GetFSharpReturnType(amap, m, minst)
                WordL.arrow ^^
                PrintTypes.layoutType denv retTy

            layout ^^ retL 

    /// Format a method info using "half C# style".
    //
    // That is, this style:
    //          Container(argName1: argType1, ..., argNameN: argTypeN) : retType
    //          Container.Method(argName1: argType1, ..., argNameN: argTypeN) : retType
    let layoutMethInfoCSharpStyle amap m denv (minfo: MethInfo) minst =
        let retTy = if minfo.IsConstructor then minfo.ApparentEnclosingType else minfo.GetFSharpReturnType(amap, m, minst) 
        let layout = 
            if minfo.IsExtensionMember then
                LeftL.leftParen ^^ wordL (tagKeyword (FSComp.SR.typeInfoExtension())) ^^ RightL.rightParen
            else emptyL
        let layout = 
            layout ^^
                if isAppTy minfo.TcGlobals minfo.ApparentEnclosingAppType then
                    let tcref = minfo.ApparentEnclosingTyconRef 
                    PrintTypes.layoutTyconRef denv tcref
                else
                    emptyL
        let layout = 
            layout ^^
                if minfo.IsConstructor then
                    SepL.leftParen
                else
                    let idL = ConvertValLogicalNameToDisplayLayout false (tagMethod >> tagNavArbValRef minfo.ArbitraryValRef >> wordL) minfo.LogicalName
                    SepL.dot ^^
                    PrintTypes.layoutTyparDecls denv idL true minfo.FormalMethodTypars ^^
                    SepL.leftParen

        let paramDatas = minfo.GetParamDatas (amap, m, minst)
        let layout = layout ^^ sepListL RightL.comma ((List.concat >> List.map (layoutParamData denv)) paramDatas)
        layout ^^ RightL.rightParen ^^ WordL.colon ^^ PrintTypes.layoutType denv retTy

    // Prettify an ILMethInfo
    let prettifyILMethInfo (amap: Import.ImportMap) m (minfo: MethInfo) typarInst ilMethInfo = 
        let (ILMethInfo(_, apparentTy, dty, mdef, _)) = ilMethInfo
        let (prettyTyparInst, prettyTys), _ = PrettyTypes.PrettifyInstAndTypes amap.g (typarInst, (apparentTy :: minfo.FormalMethodInst))
        match prettyTys with
        | prettyApparentTy :: prettyFormalMethInst ->
            let prettyMethInfo = 
                match dty with 
                | None -> MethInfo.CreateILMeth (amap, m, prettyApparentTy, mdef)
                | Some declaringTyconRef -> MethInfo.CreateILExtensionMeth(amap, m, prettyApparentTy, declaringTyconRef, minfo.ExtensionMemberPriorityOption, mdef)
            prettyTyparInst, prettyMethInfo, prettyFormalMethInst
        | _ -> failwith "prettifyILMethInfo - prettyTys empty"

    /// Format a method to a buffer using "standalone" display style. 
    /// For example, these are the formats used when printing signatures of methods that have not been overridden,
    /// and the format used when showing the individual member in QuickInfo and DeclarationInfo.
    /// The formats differ between .NET/provided methods and F# methods. Surprisingly people don't really seem 
    /// to notice this, or they find it helpful. It feels that moving from this position should not be done lightly.
    //
    // For F# members:
    //          new: unit -> retType
    //          new: argName1: argType1 * ... * argNameN: argTypeN -> retType
    //          Container.Method: unit -> retType
    //          Container.Method: argName1: argType1 * ... * argNameN: argTypeN -> retType
    //
    // For F# extension members:
    //          ApparentContainer.Method: argName1: argType1 * ... * argNameN: argTypeN -> retType
    //
    // For C# and provided members:
    //          Container(argName1: argType1, ..., argNameN: argTypeN) : retType
    //          Container.Method(argName1: argType1, ..., argNameN: argTypeN) : retType
    //
    // For C# extension members:
    //          ApparentContainer.Method(argName1: argType1, ..., argNameN: argTypeN) : retType
    let prettyLayoutOfMethInfoFreeStyle (infoReader: InfoReader) m denv typarInst methInfo =
        let amap = infoReader.amap

        match methInfo with 
        | DefaultStructCtor _ -> 
            let prettyTyparInst, _ = PrettyTypes.PrettifyInst amap.g typarInst 
            let resL = PrintTypes.layoutTyconRef denv methInfo.ApparentEnclosingTyconRef ^^ wordL (tagPunctuation "()")
            prettyTyparInst, resL
        | FSMeth(_, _, vref, _) -> 
            let prettyTyparInst, resL = PrintTastMemberOrVals.prettyLayoutOfValOrMember { denv with showMemberContainers=true } infoReader typarInst vref
            prettyTyparInst, resL
        | ILMeth(_, ilminfo, _) -> 
            let prettyTyparInst, prettyMethInfo, minst = prettifyILMethInfo amap m methInfo typarInst ilminfo
            let resL = layoutMethInfoCSharpStyle amap m denv prettyMethInfo minst
            prettyTyparInst, resL
#if !NO_TYPEPROVIDERS
        | ProvidedMeth _ -> 
            let prettyTyparInst, _ = PrettyTypes.PrettifyInst amap.g typarInst 
            prettyTyparInst, layoutMethInfoCSharpStyle amap m denv methInfo methInfo.FormalMethodInst
    #endif

    let prettyLayoutOfPropInfoFreeStyle g amap m denv (pinfo: PropInfo) =
        let retTy = pinfo.GetPropertyType(amap, m) 
        let retTy = if pinfo.IsIndexer then mkFunTy g (mkRefTupledTy g (pinfo.GetParamTypes(amap, m))) retTy else  retTy 
        let retTy, _ = PrettyTypes.PrettifyType g retTy
        let nameL = ConvertValLogicalNameToDisplayLayout false (tagProperty >> tagNavArbValRef pinfo.ArbitraryValRef >> wordL) pinfo.PropertyName
        let getterSetter =
            match pinfo.HasGetter, pinfo.HasSetter with
            | true, false ->
                wordL (tagKeyword "with") ^^ wordL (tagText "get")
            | false, true ->
                wordL (tagKeyword "with") ^^ wordL (tagText "set")
            | true, true ->
                wordL (tagKeyword "with") ^^ wordL (tagText "get, set")
            | false, false ->
                emptyL

        wordL (tagText (FSComp.SR.typeInfoProperty())) ^^
        layoutTyconRef denv pinfo.ApparentEnclosingTyconRef ^^
        SepL.dot ^^
        (nameL  |> addColonL) ^^
        layoutType denv retTy ^^
        getterSetter

    let formatPropInfoToBufferFreeStyle g amap m denv os (pinfo: PropInfo) = 
        let resL = prettyLayoutOfPropInfoFreeStyle g amap m denv pinfo 
        resL |> bufferL os

    let formatMethInfoToBufferFreeStyle amap m denv os (minfo: MethInfo) = 
        let _, resL = prettyLayoutOfMethInfoFreeStyle amap m denv emptyTyparInst minfo 
        resL |> bufferL os

    /// Format a method to a layout (actually just containing a string) using "free style" (aka "standalone"). 
    let layoutMethInfoFSharpStyle amap m denv (minfo: MethInfo) =
        layoutMethInfoFSharpStyleCore amap m denv minfo minfo.FormalMethodInst

//-------------------------------------------------------------------------

/// Printing TAST objects
module TastDefinitionPrinting = 
    open PrintTypes

    let layoutExtensionMember denv infoReader (vref: ValRef) =
        let (@@*) = if denv.printVerboseSignatures then (@@----) else (@@--)
        let tycon = vref.MemberApparentEntity.Deref
        let nameL = layoutTyconRefImpl false denv vref.MemberApparentEntity
        let nameL = layoutAccessibility denv tycon.Accessibility nameL // "type-accessibility"
        let tps =
            match PartitionValTyparsForApparentEnclosingType denv.g vref.Deref with
              | Some(_, memberParentTypars, _, _, _) -> memberParentTypars
              | None -> []
        let lhsL = WordL.keywordType ^^ layoutTyparDecls denv nameL tycon.IsPrefixDisplay tps
        let memberL = PrintTastMemberOrVals.prettyLayoutOfValOrMemberNoInst denv infoReader vref
        (lhsL ^^ WordL.keywordWith) @@* memberL

    let layoutExtensionMembers denv infoReader vs =
        aboveListL (List.map (layoutExtensionMember denv infoReader) vs) 

    let layoutRecdField prefix isClassDecl denv infoReader (enclosingTcref: TyconRef) (fld: RecdField) =
        let lhs = ConvertLogicalNameToDisplayLayout (tagRecordField >> mkNav fld.DefinitionRange >> wordL) fld.DisplayNameCore
        let lhs = (if isClassDecl then layoutAccessibility denv fld.Accessibility lhs else lhs)
        let lhs = if fld.IsMutable then wordL (tagKeyword "mutable") --- lhs else lhs
        let fieldL =
            let rhs =
                match stripTyparEqns fld.FormalType with
                | TType_fun _ -> LeftL.leftParen ^^ layoutType denv fld.FormalType ^^ RightL.rightParen
                | _ -> layoutType denv fld.FormalType
            
            (lhs |> addColonL) --- rhs
        let fieldL = prefix fieldL
        let fieldL = fieldL |> layoutAttribs denv None false TyparKind.Type (fld.FieldAttribs @ fld.PropertyAttribs)

        // The enclosing TyconRef might be a union and we can only get fields from union cases, so we need ignore unions here.
        if not enclosingTcref.IsUnionTycon then
            layoutXmlDocOfRecdField denv infoReader isClassDecl (RecdFieldRef(enclosingTcref, fld.Id.idText)) fieldL
        else
            fieldL

    let layoutUnionOrExceptionField denv infoReader isGenerated enclosingTcref i (fld: RecdField) =
        if isGenerated i fld then
            layoutTypeWithInfoAndPrec denv SimplifyTypes.typeSimplificationInfo0 2 fld.FormalType
        else
            layoutRecdField id false denv infoReader enclosingTcref fld
    
    let isGeneratedUnionCaseField pos (f: RecdField) = 
        if pos < 0 then f.LogicalName = "Item"
        else f.LogicalName = "Item" + string (pos + 1)

    let isGeneratedExceptionField pos (f: RecdField) = 
        f.LogicalName = "Data" + (string pos)

    let layoutUnionCaseFields denv infoReader isUnionCase enclosingTcref fields = 
        match fields with
        | [f] when isUnionCase ->
            layoutUnionOrExceptionField denv infoReader isGeneratedUnionCaseField enclosingTcref -1 f
        | _ -> 
            let isGenerated = if isUnionCase then isGeneratedUnionCaseField else isGeneratedExceptionField
            sepListL WordL.star (List.mapi (layoutUnionOrExceptionField denv infoReader isGenerated enclosingTcref) fields)

    let layoutUnionCase denv infoReader prefixL enclosingTcref (ucase: UnionCase) =
        let nmL = ConvertLogicalNameToDisplayLayout (tagUnionCase >> mkNav ucase.DefinitionRange >> wordL) ucase.Id.idText
        //let nmL = layoutAccessibility denv ucase.Accessibility nmL
        let caseL =
            match ucase.RecdFields with
            | []     -> (prefixL ^^ nmL)
            | fields -> (prefixL ^^ nmL ^^ WordL.keywordOf) --- layoutUnionCaseFields denv infoReader true enclosingTcref fields
        layoutXmlDocOfUnionCase denv infoReader (UnionCaseRef(enclosingTcref, ucase.Id.idText)) caseL

    let layoutUnionCases denv infoReader enclosingTcref ucases =
        let prefixL = WordL.bar // See bug://2964 - always prefix in case preceded by accessibility modifier
        List.map (layoutUnionCase denv infoReader prefixL enclosingTcref) ucases

    /// When to force a break? "type tyname = <HERE> repn"
    /// When repn is class or datatype constructors (not single one).
    let breakTypeDefnEqn repr =
        match repr with 
        | TILObjectRepr _ -> true
        | TFSharpObjectRepr _ -> true
        | TFSharpRecdRepr _ -> true
        | TFSharpUnionRepr r ->
             not (isNilOrSingleton r.CasesTable.UnionCasesAsList) ||
             r.CasesTable.UnionCasesAsList |> List.exists (fun uc -> not uc.XmlDoc.IsEmpty)
        | TAsmRepr _ 
        | TMeasureableRepr _ 
#if !NO_TYPEPROVIDERS
        | TProvidedTypeRepr _
        | TProvidedNamespaceRepr _
#endif
        | TNoRepr -> false
      
    let layoutILFieldInfo denv (infoReader: InfoReader) m (finfo: ILFieldInfo) =
        let staticL = if finfo.IsStatic then WordL.keywordStatic else emptyL
        let nameL = ConvertLogicalNameToDisplayLayout (tagField >> wordL) finfo.FieldName
        let typL = layoutType denv (finfo.FieldType(infoReader.amap, m))
        let fieldL = staticL ^^ WordL.keywordVal ^^ (nameL |> addColonL) ^^ typL
        layoutXmlDocOfILFieldInfo denv infoReader finfo fieldL

    let layoutEventInfo denv (infoReader: InfoReader) m (einfo: EventInfo) =
        let amap = infoReader.amap
        let staticL = if einfo.IsStatic then WordL.keywordStatic else emptyL
        let nameL = ConvertValLogicalNameToDisplayLayout false (tagEvent >> tagNavArbValRef einfo.ArbitraryValRef >> wordL) einfo.EventName
        let typL = layoutType denv (einfo.GetDelegateType(amap, m))
        let overallL = staticL ^^ WordL.keywordMember ^^ (nameL |> addColonL) ^^ typL
        layoutXmlDocOfEventInfo denv infoReader einfo overallL

    let layoutPropInfo denv (infoReader: InfoReader) m (pinfo: PropInfo) =
        let amap = infoReader.amap

        match pinfo.ArbitraryValRef with
        | Some vref ->
            PrintTastMemberOrVals.prettyLayoutOfValOrMemberNoInst denv infoReader vref
        | None ->

            let modifierAndMember =
                if pinfo.IsStatic then
                    WordL.keywordStatic ^^ WordL.keywordMember
                else
                    WordL.keywordMember
        
            let nameL = ConvertValLogicalNameToDisplayLayout false (tagProperty >> tagNavArbValRef pinfo.ArbitraryValRef >> wordL) pinfo.PropertyName
            let typL = layoutType denv (pinfo.GetPropertyType(amap, m))
            let overallL = modifierAndMember ^^ (nameL |> addColonL) ^^ typL
            layoutXmlDocOfPropInfo denv infoReader pinfo overallL

    let layoutTyconDefn (denv: DisplayEnv) (infoReader: InfoReader) ad m simplified typewordL (tcref: TyconRef) =
        let g = denv.g
        // use 4-indent 
        let (-*) = if denv.printVerboseSignatures then (-----) else (---)
        let (@@*) = if denv.printVerboseSignatures then (@@----) else (@@--)
        let amap = infoReader.amap
        let tycon = tcref.Deref
        let repr = tycon.TypeReprInfo
        let isMeasure = (tycon.TypeOrMeasureKind = TyparKind.Measure)
        let ty = generalizedTyconRef g tcref 

        let start, tagger =
            if isStructTy g ty && not tycon.TypeAbbrev.IsSome then
                // Always show [<Struct>] whether verbose or not
                Some "struct", tagStruct
            elif isInterfaceTy g ty then
                if denv.printVerboseSignatures then
                    Some "interface", tagInterface
                else
                    None, tagInterface
            elif isMeasure then
                None, tagClass
            elif isClassTy g ty then
                if denv.printVerboseSignatures then
                    (if simplified then None else Some "class"), tagClass
                else
                    None, tagClass
            else
                None, tagUnknownType

        let nameL = ConvertLogicalNameToDisplayLayout (tagger >> mkNav tycon.DefinitionRange >> wordL) tycon.DisplayNameCore

        let nameL = layoutAccessibility denv tycon.Accessibility nameL
        let denv = denv.AddAccessibility tycon.Accessibility 

        let lhsL =
            let tps = tycon.TyparsNoRange
            let tpsL = layoutTyparDecls denv nameL tycon.IsPrefixDisplay tps
            typewordL ^^ tpsL


        let sortKey (minfo: MethInfo) = 
            (not minfo.IsConstructor,
             not minfo.IsInstance, // instance first
             minfo.DisplayNameCore, // sort by name 
             List.sum minfo.NumArgs, // sort by #curried
             minfo.NumArgs.Length)     // sort by arity 

        let shouldShow (vrefOpt: ValRef option) =
            match vrefOpt with
            | None -> true
            | Some vref ->
                (denv.showObsoleteMembers || not (CheckFSharpAttributesForObsolete denv.g vref.Attribs)) &&
                (denv.showHiddenMembers || not (CheckFSharpAttributesForHidden denv.g vref.Attribs))
                
        let ctors =
            GetIntrinsicConstructorInfosOfType infoReader m ty
            |> List.filter (fun minfo -> IsMethInfoAccessible amap m ad minfo && not minfo.IsClassConstructor && shouldShow minfo.ArbitraryValRef)

        let iimpls =
            if suppressInheritanceAndInterfacesForTyInSimplifiedDisplays g amap m ty then 
                []
            elif isRecdTy g ty || isUnionTy g ty || tycon.IsStructOrEnumTycon then
                tycon.ImmediateInterfacesOfFSharpTycon
                |> List.filter (fun (_, compgen, _) -> not compgen)
                |> List.map p13
            else 
                GetImmediateInterfacesOfType SkipUnrefInterfaces.Yes g amap m ty

        let iimplsLs =
            iimpls
            |> List.map (fun intfTy -> wordL (tagKeyword (if isInterfaceTy g ty then "inherit" else "interface")) -* layoutType denv intfTy)

        let props =
            GetImmediateIntrinsicPropInfosOfType (None, ad) g amap m ty
            |> List.filter (fun pinfo -> shouldShow pinfo.ArbitraryValRef)

        let events = 
            infoReader.GetEventInfosOfType(None, ad, m, ty)
            |> List.filter (fun einfo -> shouldShow einfo.ArbitraryValRef && typeEquiv g ty einfo.ApparentEnclosingType)

        let impliedNames = 
            try 
                [ for p in props do 
                    if p.HasGetter then p.GetterMethod.DisplayName
                    if p.HasSetter then p.SetterMethod.DisplayName
                  for e in events do 
                    e.AddMethod.DisplayName 
                    e.RemoveMethod.DisplayName ]
                |> Set.ofList 
            with _ ->
                Set.empty

        let meths =
            GetImmediateIntrinsicMethInfosOfType (None, ad) g amap m ty
            |> List.filter (fun minfo ->
                not minfo.IsClassConstructor &&
                not minfo.IsConstructor &&
                shouldShow minfo.ArbitraryValRef &&
                not (impliedNames.Contains minfo.DisplayName) &&
                IsMethInfoAccessible amap m ad minfo &&
                // Discard method impls such as System.IConvertible.ToBoolean
                not (minfo.IsILMethod && minfo.DisplayName.Contains(".")) &&
                not (minfo.DisplayName.Split('.') |> Array.exists (fun part -> isDiscard part)))

        let ilFields =
            infoReader.GetILFieldInfosOfType (None, ad, m, ty)
            |> List.filter (fun fld -> 
                IsILFieldInfoAccessible g amap m ad fld &&
                not (isDiscard fld.FieldName) &&
                typeEquiv g ty fld.ApparentEnclosingType)

        let ctorLs =
            if denv.shrinkOverloads then
                ctors 
                |> shrinkOverloads (InfoMemberPrinting.layoutMethInfoFSharpStyle infoReader m denv) (fun _ xL -> xL) 
            else
                ctors
                |> List.map (fun ctor -> InfoMemberPrinting.layoutMethInfoFSharpStyle infoReader m denv ctor)

        let methLs = 
            meths
            |> List.groupBy (fun md -> md.DisplayNameCore)
            |> List.collect (fun (_, group) ->
                if denv.shrinkOverloads then
                    shrinkOverloads (InfoMemberPrinting.layoutMethInfoFSharpStyle infoReader m denv) (fun x xL -> (sortKey x, xL)) group
                else
                    group
                    |> List.sortBy sortKey
                    |> List.map (fun methinfo -> ((not methinfo.IsConstructor, methinfo.IsInstance, methinfo.DisplayName, List.sum methinfo.NumArgs, methinfo.NumArgs.Length), InfoMemberPrinting.layoutMethInfoFSharpStyle infoReader m denv methinfo)))
            |> List.sortBy fst
            |> List.map snd

        let ilFieldsL =
            ilFields
            |> List.map (fun x -> (true, x.IsStatic, x.FieldName, 0, 0), layoutILFieldInfo denv infoReader m x)
            |> List.sortBy fst
            |> List.map snd

        let staticVals =
            if isRecdTy g ty then
                []
            else
                tycon.TrueFieldsAsList
                |> List.filter (fun f -> IsAccessible ad f.Accessibility && f.IsStatic && not (isDiscard f.DisplayNameCore))

        let staticValLs =
            staticVals
            |> List.map (fun f -> layoutRecdField (fun l -> WordL.keywordStatic ^^ WordL.keywordVal ^^ l) true denv infoReader tcref f)

        let instanceVals =
            if isRecdTy g ty then
                []
            else
                tycon.TrueInstanceFieldsAsList
                |> List.filter (fun f -> IsAccessible ad f.Accessibility && not (isDiscard f.DisplayNameCore))

        let instanceValLs =
            instanceVals
            |> List.map (fun f -> layoutRecdField (fun l -> WordL.keywordVal ^^ l) true denv infoReader tcref f)
    
        let propLs =
            props
            |> List.map (fun x -> (true, x.IsStatic, x.PropertyName, 0, 0), layoutPropInfo denv infoReader m x)
            |> List.sortBy fst
            |> List.map snd

        let eventLs = 
            events
            |> List.map (fun x -> (true, x.IsStatic, x.EventName, 0, 0), layoutEventInfo denv infoReader m x)
            |> List.sortBy fst
            |> List.map snd

        let nestedTypeLs =
#if !NO_TYPEPROVIDERS
            match tryTcrefOfAppTy g ty with
            | ValueSome tcref ->
                match tcref.TypeReprInfo with 
                | TProvidedTypeRepr info ->
                    [ 
                        for nestedType in info.ProvidedType.PApplyArray((fun sty -> sty.GetNestedTypes() |> Array.filter (fun t -> t.IsPublic || t.IsNestedPublic)), "GetNestedTypes", m) do 
                            yield nestedType.PUntaint((fun t -> t.IsClass, t.Name), m)
                    ] 
                    |> List.sortBy snd
                    |> List.map (fun (isClass, t) -> WordL.keywordNested ^^ WordL.keywordType ^^ wordL ((if isClass then tagClass else tagStruct) t))
                | _ ->
                    []
            | ValueNone ->
#endif
                []

        let inherits = 
            [ if not (suppressInheritanceAndInterfacesForTyInSimplifiedDisplays g amap m ty) then 
                match GetSuperTypeOfType g amap m ty with 
                | Some superTy when not (isObjTy g superTy) && not (isValueTypeTy g superTy) ->
                    superTy
                | _ -> ()
            ]

        let inheritsL = 
            inherits
            |> List.map (fun super -> wordL (tagKeyword "inherit") ^^ (layoutType denv super))

        let allDecls = inheritsL @ iimplsLs @ ctorLs @ instanceValLs @ methLs @ ilFieldsL @ propLs @ eventLs @ staticValLs @ nestedTypeLs

        let needsStartEnd =
            match start with 
            | Some "class" ->
                // 'inherits' is not enough for F# type kind inference to infer a class
                // inherits.IsEmpty &&
                ilFields.IsEmpty &&
                // 'abstract' is not enough for F# type kind inference to infer a class by default in signatures
                // 'static member' is surprisingly not enough for F# type kind inference to infer a class by default in signatures
                // 'overrides' is surprisingly not enough for F# type kind inference to infer a class by default in signatures
                //(meths |> List.forall (fun m -> m.IsAbstract || m.IsDefiniteFSharpOverride || not m.IsInstance)) &&
                //(props |> List.forall (fun m -> (not m.HasGetter || m.GetterMethod.IsAbstract))) &&
                //(props |> List.forall (fun m -> (not m.HasSetter || m.SetterMethod.IsAbstract))) &&
                ctors.IsEmpty &&
                instanceVals.IsEmpty &&
                staticVals.IsEmpty
            | Some "struct" -> 
                true
            | Some "interface" -> 
                meths.IsEmpty &&
                props.IsEmpty
            | _ ->
                false

        let start = if needsStartEnd then start else None
        
        let addMaxMembers reprL =
            if isNil allDecls then
                reprL
            else
                let memberLs = applyMaxMembers denv.maxMembers allDecls
                reprL @@ aboveListL memberLs

        let addReprAccessL l =
            layoutAccessibility denv tycon.TypeReprAccessibility l 

        let addLhs rhsL =
            let brk = not (isNil allDecls) || breakTypeDefnEqn repr
            if brk then 
                (lhsL ^^ WordL.equals) @@* rhsL 
            else 
                (lhsL ^^ WordL.equals) -* rhsL

        let typeDeclL = 

            match repr with 
            | TFSharpRecdRepr _ ->
                let denv = denv.AddAccessibility tycon.TypeReprAccessibility 

                // For records, use multi-line layout as soon as there is XML doc 
                //   type R =
                //     { 
                //         /// ABC
                //         Field1: int 
                //
                //         /// ABC
                //         Field2: int 
                //     }
                //
                // For records, use multi-line layout as soon as there is more than one field
                //   type R =
                //     { 
                //         Field1: int 
                //         Field2: int 
                //     }
                let useMultiLine =
                    let members =
                        match denv.maxMembers with 
                        | None -> tycon.TrueFieldsAsList
                        | Some n -> tycon.TrueFieldsAsList |> List.truncate n
                    members.Length > 1 ||
                    members |> List.exists (fun m -> not m.XmlDoc.IsEmpty)

                tycon.TrueFieldsAsList
                |> List.map (layoutRecdField id false denv infoReader tcref)
                |> applyMaxMembers denv.maxMembers
                |> aboveListL
                |> (if useMultiLine then braceMultiLineL else braceL)
                |> addReprAccessL
                |> addMaxMembers
                |> addLhs

            | TFSharpUnionRepr _ -> 
                let denv = denv.AddAccessibility tycon.TypeReprAccessibility 
                tycon.UnionCasesAsList
                |> layoutUnionCases denv infoReader tcref
                |> applyMaxMembers denv.maxMembers
                |> aboveListL
                |> addReprAccessL
                |> addMaxMembers
                |> addLhs
                  
            | TFSharpObjectRepr { fsobjmodel_kind = TFSharpDelegate slotSig } ->
                let (TSlotSig(_, _, _, _, paraml, retTy)) = slotSig
                let retTy = GetFSharpViewOfReturnType denv.g retTy
                let delegateL = WordL.keywordDelegate ^^ WordL.keywordOf -* layoutTopType denv SimplifyTypes.typeSimplificationInfo0 (paraml |> List.mapSquared (fun sp -> (sp.Type, ValReprInfo.unnamedTopArg1))) retTy []
                delegateL
                |> addLhs

            // Measure declarations are '[<Measure>] type kg' unless abbreviations
            | TFSharpObjectRepr _ when isMeasure ->
                lhsL

            | TFSharpObjectRepr { fsobjmodel_kind = TFSharpEnum } ->
                tycon.TrueFieldsAsList
                |> List.map (fun f -> 
                    match f.LiteralValue with 
                    | None -> emptyL
                    | Some c ->
                        WordL.bar ^^
                        wordL (tagField f.DisplayName) ^^
                        WordL.equals ^^ 
                        layoutConst denv.g ty c)
                |> aboveListL
                |> addLhs

            | TFSharpObjectRepr objRepr when isNil allDecls ->
                match objRepr.fsobjmodel_kind with
                | TFSharpClass ->
                    WordL.keywordClass ^^ WordL.keywordEnd
                    |> addLhs
                | TFSharpInterface ->
                    WordL.keywordInterface ^^ WordL.keywordEnd
                    |> addLhs
                | TFSharpStruct ->
                    WordL.keywordStruct ^^ WordL.keywordEnd
                    |> addLhs
                | _ -> lhsL

            | TFSharpObjectRepr _ ->
                allDecls
                |> applyMaxMembers denv.maxMembers
                |> aboveListL
                |> addLhs

            | TAsmRepr _ -> 
                let asmL = wordL (tagText "(# \"<Common IL Type Omitted>\" #)")
                asmL
                |> addLhs

            | TMeasureableRepr ty ->
                layoutType denv ty
                |> addLhs

            | TILObjectRepr _ when tycon.ILTyconRawMetadata.IsEnum ->
                infoReader.GetILFieldInfosOfType (None, ad, m, ty) 
                |> List.filter (fun x -> x.FieldName <> "value__")
                |> List.map (fun x -> PrintIL.layoutILEnumCase x.FieldName x.LiteralValue)
                |> applyMaxMembers denv.maxMembers
                |> aboveListL
                |> addLhs

            | TILObjectRepr _ ->
                allDecls
                |> applyMaxMembers denv.maxMembers
                |> aboveListL
                |> addLhs

            | TNoRepr when tycon.TypeAbbrev.IsSome ->
                let abbreviatedTy = tycon.TypeAbbrev.Value
                (lhsL ^^ WordL.equals) -* (layoutType { denv with shortTypeNames = false } abbreviatedTy)

            | _ when isNil allDecls ->
                lhsL

            | TProvidedNamespaceRepr _
            | TProvidedTypeRepr _
            | TNoRepr -> 
                allDecls
                |> applyMaxMembers denv.maxMembers
                |> aboveListL
                |> addLhs

        typeDeclL 
        |> layoutAttribs denv start false tycon.TypeOrMeasureKind tycon.Attribs 
        |> layoutXmlDocOfEntity denv infoReader tcref 

    // Layout: exception definition
    let layoutExnDefn denv infoReader (exncref: EntityRef) =
        let (-*) = if denv.printVerboseSignatures then (-----) else (---)
        let exnc = exncref.Deref
        let nameL = ConvertLogicalNameToDisplayLayout (tagClass >> mkNav exncref.DefinitionRange >> wordL) exnc.DisplayNameCore
        let nameL = layoutAccessibility denv exnc.TypeReprAccessibility nameL
        let exnL = wordL (tagKeyword "exception") ^^ nameL // need to tack on the Exception at the right of the name for goto definition
        let reprL = 
            match exnc.ExceptionInfo with 
            | TExnAbbrevRepr ecref -> WordL.equals -* layoutTyconRef denv ecref
            | TExnAsmRepr _ -> WordL.equals -* wordL (tagText "(# ... #)")
            | TExnNone -> emptyL
            | TExnFresh r -> 
                match r.TrueFieldsAsList with
                | [] -> emptyL
                | r -> WordL.keywordOf -* layoutUnionCaseFields denv infoReader false exncref r

        let overallL = exnL ^^ reprL
        layoutXmlDocOfEntity denv infoReader exncref overallL

    // Layout: module spec 

    let layoutTyconDefns denv infoReader ad m (tycons: Tycon list) =
        match tycons with 
        | [] -> emptyL
        | [h] when h.IsFSharpException -> layoutExnDefn denv infoReader (mkLocalEntityRef h)
        | h :: t -> 
            let x = layoutTyconDefn denv infoReader ad m false WordL.keywordType (mkLocalEntityRef h)
            let xs = List.map (mkLocalEntityRef >> layoutTyconDefn denv infoReader ad m false (wordL (tagKeyword "and"))) t
            aboveListL (x :: xs)

    let rec fullPath (mspec: ModuleOrNamespace) acc =
        if mspec.IsNamespace then
            match mspec.ModuleOrNamespaceType.ModuleAndNamespaceDefinitions |> List.tryHead with
            | Some next when next.IsNamespace ->
                fullPath next (acc @ [next.DisplayNameCore])
            | _ ->
                acc, mspec
        else
            acc, mspec

    let rec layoutModuleOrNamespace (denv: DisplayEnv) (infoReader: InfoReader) ad m isFirstTopLevel (mspec: ModuleOrNamespace) =
        let (@@*) = if denv.printVerboseSignatures then (@@----) else (@@--)

        let outerPath = mspec.CompilationPath.AccessPath

        let path, mspec = fullPath mspec [mspec.DisplayNameCore]

        let denv =
            let outerPath = outerPath |> List.map fst
            denv.AddOpenPath (outerPath @ path)

        let headerL =
            if mspec.IsNamespace then
                // This is a container namespace. We print the header when we get to the first concrete module.
                let pathL = path |> List.map (ConvertLogicalNameToDisplayLayout (tagNamespace >> wordL))
                wordL (tagKeyword "namespace") ^^ sepListL SepL.dot pathL
            else
                // This is a module 
                let name = path |> List.last
                let nameL = ConvertLogicalNameToDisplayLayout (tagModule >> mkNav mspec.DefinitionRange >> wordL) name
                let nameL = 
                    match path with
                    | [_] -> 
                        nameL
                    | _ ->
                        let innerPath = path[..path.Length - 2]
                        let innerPathL = innerPath |> List.map (ConvertLogicalNameToDisplayLayout (tagNamespace >> wordL))
                        sepListL SepL.dot innerPathL ^^ SepL.dot ^^ nameL

                let modNameL = wordL (tagKeyword "module") ^^ nameL
                let modNameEqualsL = modNameL ^^ WordL.equals
                let modIsEmpty =
                    mspec.ModuleOrNamespaceType.AllEntities |> Seq.isEmpty &&
                    mspec.ModuleOrNamespaceType.AllValsAndMembers |> Seq.isEmpty

                // Check if its an outer module or a nested module
                let isNamespace = function | Namespace _ -> true | _ -> false
                if (outerPath |> List.forall (fun (_, istype) -> isNamespace istype)) && isNil outerPath then
                    // If so print a "module" declaration
                    modNameL
                elif modIsEmpty then
                    modNameEqualsL ^^ wordL (tagKeyword "begin") ^^ WordL.keywordEnd
                else
                    // Otherwise this is an outer module contained immediately in a namespace
                    // We already printed the namespace declaration earlier. So just print the 
                    // module now.
                    modNameEqualsL

        let headerL =
            layoutAttribs denv None false mspec.TypeOrMeasureKind mspec.Attribs headerL

        let shouldShow (v: Val) =
            (denv.showObsoleteMembers || not (CheckFSharpAttributesForObsolete denv.g v.Attribs)) &&
            (denv.showHiddenMembers || not (CheckFSharpAttributesForHidden denv.g v.Attribs))

        let entityLs =
            if mspec.IsNamespace then []
            else
                mspec.ModuleOrNamespaceType.AllEntities
                |> QueueList.toList
                |> List.map (fun entity -> layoutEntityDefn denv infoReader ad m (mkLocalEntityRef entity))
            
        let valLs =
            if mspec.IsNamespace then []
            else
                mspec.ModuleOrNamespaceType.AllValsAndMembers
                |> QueueList.toList
                |> List.filter shouldShow
                |> List.sortBy (fun v -> v.DisplayNameCore)
                |> List.map (mkLocalValRef >> PrintTastMemberOrVals.prettyLayoutOfValOrMemberNoInst denv infoReader)

        if List.isEmpty entityLs && List.isEmpty valLs then
            headerL
        else
            let entitiesL =
                entityLs
                |> aboveListL

            let valsL =
                valLs
                |> aboveListL

            if isFirstTopLevel then
                aboveListL
                    [
                        headerL
                        entitiesL
                        valsL
                    ]
            else
                headerL @@* entitiesL @@ valsL

    and layoutEntityDefn (denv: DisplayEnv) (infoReader: InfoReader) ad m (eref: EntityRef) =
        if eref.IsModuleOrNamespace then
            layoutModuleOrNamespace denv infoReader ad m false eref.Deref
            |> layoutXmlDocOfEntity denv infoReader eref
        elif eref.IsFSharpException then
            layoutExnDefn denv infoReader eref
        else
            layoutTyconDefn denv infoReader ad m true WordL.keywordType eref

//--------------------------------------------------------------------------

module InferredSigPrinting = 
    open PrintTypes

    /// Layout the inferred signature of a compilation unit
    let layoutImpliedSignatureOfModuleOrNamespace showHeader denv infoReader ad m expr =

        let (@@*) = if denv.printVerboseSignatures then (@@----) else (@@--)

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
                innerPath
                |> List.choose (fun (path, kind) ->
                    match kind with
                    | ModuleOrNamespaceKind.Namespace false -> None
                    | _ -> Some path)
                |> denv.AddOpenPath

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
                let modNameL = wordL (tagKeyword "module") ^^ nmL
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

        imdefL denv expr

//--------------------------------------------------------------------------

module PrintData = 
    open PrintTypes

    /// Nice printing of a subset of expressions, e.g. for refutations in pattern matching
    let rec dataExprL denv expr = dataExprWrapL denv false expr

    and dataExprWrapL denv isAtomic expr =
        match expr with
        | Expr.Const (c, _, ty) -> 
            if isEnumTy denv.g ty then 
                wordL (tagKeyword "enum") ^^ angleL (layoutType denv ty) ^^ bracketL (layoutConst denv.g ty c)
            else
                layoutConst denv.g ty c

        | Expr.Val (vref, _, _) ->
            wordL (tagLocal vref.DisplayName)

        | Expr.Link rX ->
            dataExprWrapL denv isAtomic rX.Value

        | Expr.Op (TOp.UnionCase c, _, args, _) -> 
            if denv.g.unionCaseRefEq c denv.g.nil_ucref then wordL (tagPunctuation "[]")
            elif denv.g.unionCaseRefEq c denv.g.cons_ucref then 
                let rec strip = function Expr.Op (TOp.UnionCase _, _, [h;t], _) -> h :: strip t | _ -> []
                listL (dataExprL denv) (strip expr)
            elif isNil args then 
                wordL (tagUnionCase c.CaseName)
            else 
                (wordL (tagUnionCase c.CaseName) ++ bracketL (commaListL (dataExprsL denv args)))
            
        | Expr.Op (TOp.ExnConstr c, _, args, _) ->
            (wordL (tagMethod c.LogicalName) ++ bracketL (commaListL (dataExprsL denv args)))

        | Expr.Op (TOp.Tuple _, _, xs, _) ->
            tupleL (dataExprsL denv xs)

        | Expr.Op (TOp.Recd (_, tc), _, xs, _) -> 
            let fields = tc.TrueInstanceFieldsAsList
            let lay fs x = (wordL (tagRecordField fs.rfield_id.idText) ^^ sepL (tagPunctuation "=")) --- (dataExprL denv x)
            braceL (semiListL (List.map2 lay fields xs))

        | Expr.Op (TOp.ValFieldGet (RecdFieldRef.RecdFieldRef (tcref, name)), _, _, _) ->
            (layoutTyconRef denv tcref) ^^ sepL (tagPunctuation ".") ^^ wordL (tagField name)

        | Expr.Op (TOp.Array, [_], xs, _) ->
            leftL (tagPunctuation "[|") ^^ semiListL (dataExprsL denv xs) ^^ RightL.rightBracketBar

        | _ ->
            wordL (tagPunctuation "?")

    and dataExprsL denv xs = List.map (dataExprL denv) xs

let dataExprL denv expr = PrintData.dataExprL denv expr

//--------------------------------------------------------------------------
// Print Signatures/Types - output functions 
//-------------------------------------------------------------------------- 

let outputValOrMember denv infoReader os x = x |> PrintTastMemberOrVals.prettyLayoutOfValOrMemberNoInst denv infoReader |> bufferL os

let stringValOrMember denv infoReader x = x |> PrintTastMemberOrVals.prettyLayoutOfValOrMemberNoInst denv infoReader |> showL

/// Print members with a qualification showing the type they are contained in 
let layoutQualifiedValOrMember denv infoReader typarInst vref =
    PrintTastMemberOrVals.prettyLayoutOfValOrMember { denv with showMemberContainers=true; } infoReader typarInst vref

let outputQualifiedValOrMember denv infoReader os vref =
    outputValOrMember { denv with showMemberContainers=true; } infoReader os vref

let outputQualifiedValSpec denv infoReader os vref =
    outputQualifiedValOrMember denv infoReader os vref

let stringOfQualifiedValOrMember denv infoReader vref =
    PrintTastMemberOrVals.prettyLayoutOfValOrMemberNoInst { denv with showMemberContainers=true; } infoReader vref |> showL

/// Convert a MethInfo to a string
let formatMethInfoToBufferFreeStyle infoReader m denv buf d =
    InfoMemberPrinting.formatMethInfoToBufferFreeStyle infoReader m denv buf d

let prettyLayoutOfMethInfoFreeStyle infoReader m denv typarInst minfo =
    InfoMemberPrinting.prettyLayoutOfMethInfoFreeStyle infoReader m denv typarInst minfo

/// Convert a PropInfo to a string
let prettyLayoutOfPropInfoFreeStyle g amap m denv d =
    InfoMemberPrinting.prettyLayoutOfPropInfoFreeStyle g amap m denv d

/// Convert a MethInfo to a string
let stringOfMethInfo infoReader m denv minfo =
    buildString (fun buf -> InfoMemberPrinting.formatMethInfoToBufferFreeStyle infoReader m denv buf minfo)

/// Convert MethInfos to lines separated by newline including a newline as the first character
let multiLineStringOfMethInfos infoReader m denv minfos =
     minfos
     |> List.map (stringOfMethInfo infoReader m denv)
     |> List.map (sprintf "%s   %s" Environment.NewLine)
     |> String.concat ""

let stringOfPropInfo g amap m denv pinfo =
         buildString (fun buf -> InfoMemberPrinting.formatPropInfoToBufferFreeStyle g amap m denv buf pinfo)

/// Convert PropInfos to lines separated by newline including a newline as the first character
let multiLineStringOfPropInfos g amap m denv pinfos =
     pinfos
     |> List.map (stringOfPropInfo g amap m denv)
     |> List.map (sprintf "%s   %s" Environment.NewLine)
     |> String.concat ""

/// Convert a ParamData to a string
let stringOfParamData denv paramData = buildString (fun buf -> InfoMemberPrinting.formatParamDataToBuffer denv buf paramData)

let layoutOfParamData denv paramData = InfoMemberPrinting.layoutParamData denv paramData

let layoutExnDef denv infoReader x = x |> TastDefinitionPrinting.layoutExnDefn denv infoReader

let stringOfTyparConstraints denv x = x |> PrintTypes.layoutConstraintsWithInfo denv SimplifyTypes.typeSimplificationInfo0 |> showL

let layoutTyconDefn denv infoReader ad m (* width *) x = TastDefinitionPrinting.layoutTyconDefn denv infoReader ad m true WordL.keywordType (mkLocalEntityRef x) (* |> Display.squashTo width *)

let layoutEntityDefn denv infoReader ad m x = TastDefinitionPrinting.layoutEntityDefn denv infoReader ad m x

let layoutUnionCases denv infoReader enclosingTcref x = x |> TastDefinitionPrinting.layoutUnionCaseFields denv infoReader true enclosingTcref

/// Pass negative number as pos in case of single cased discriminated unions
let isGeneratedUnionCaseField pos f = TastDefinitionPrinting.isGeneratedUnionCaseField pos f

let isGeneratedExceptionField pos f = TastDefinitionPrinting.isGeneratedExceptionField pos f

let stringOfTyparConstraint denv tpc = stringOfTyparConstraints denv [tpc]

let stringOfTy denv x = x |> PrintTypes.layoutType denv |> showL

let prettyLayoutOfType denv x = x |> PrintTypes.prettyLayoutOfType denv

let prettyLayoutOfTypeNoCx denv x = x |> PrintTypes.prettyLayoutOfTypeNoConstraints denv

let prettyLayoutOfTypar denv x = x |> PrintTypes.layoutTyparRef denv

let prettyStringOfTy denv x = x |> PrintTypes.prettyLayoutOfType denv |> showL

let prettyStringOfTyNoCx denv x = x |> PrintTypes.prettyLayoutOfTypeNoConstraints denv |> showL

let stringOfRecdField denv infoReader enclosingTcref x = x |> TastDefinitionPrinting.layoutRecdField id false denv infoReader enclosingTcref |> showL

let stringOfUnionCase denv infoReader enclosingTcref x = x |> TastDefinitionPrinting.layoutUnionCase denv infoReader WordL.bar enclosingTcref |> showL

let stringOfExnDef denv infoReader x = x |> TastDefinitionPrinting.layoutExnDefn denv infoReader |> showL

let stringOfFSAttrib denv x = x |> PrintTypes.layoutAttrib denv |> squareAngleL |> showL

let stringOfILAttrib denv x = x |> PrintTypes.layoutILAttrib denv |> squareAngleL |> showL

let layoutImpliedSignatureOfModuleOrNamespace showHeader denv infoReader ad m contents =
    InferredSigPrinting.layoutImpliedSignatureOfModuleOrNamespace showHeader denv infoReader ad m contents 

let prettyLayoutOfValOrMember denv infoReader typarInst vref =
    PrintTastMemberOrVals.prettyLayoutOfValOrMember denv infoReader typarInst vref  

let prettyLayoutOfValOrMemberNoInst denv infoReader vref =
    PrintTastMemberOrVals.prettyLayoutOfValOrMemberNoInst denv infoReader vref

let prettyLayoutOfMemberNoInstShort denv v = PrintTastMemberOrVals.prettyLayoutOfMemberNoInstShort denv v

let layoutOfValReturnType denv vref = vref |> PrintTypes.layoutOfValReturnType denv

let prettyLayoutOfInstAndSig denv x = PrintTypes.prettyLayoutOfInstAndSig denv x

/// Generate text for comparing two types.
///
/// If the output text is different without showing constraints and/or imperative type variable 
/// annotations and/or fully qualifying paths then don't show them! 
let minimalStringsOfTwoTypes denv ty1 ty2 =
    let (ty1, ty2), tpcs = PrettyTypes.PrettifyTypePair denv.g (ty1, ty2)

    // try denv + no type annotations 
    let attempt1 = 
        let denv = { denv with showInferenceTyparAnnotations=false; showStaticallyResolvedTyparAnnotations=false }
        let min1 = stringOfTy denv ty1
        let min2 = stringOfTy denv ty2
        if min1 <> min2 then Some (min1, min2, "") else None

    match attempt1 with 
    | Some res -> res
    | None -> 

    // try denv + no type annotations + show full paths
    let attempt2 = 
        let denv = { denv with showInferenceTyparAnnotations=false; showStaticallyResolvedTyparAnnotations=false }.SetOpenPaths []
        let min1 = stringOfTy denv ty1
        let min2 = stringOfTy denv ty2
        if min1 <> min2 then Some (min1, min2, "") else None

    match attempt2 with 
    | Some res -> res
    | None -> 

    let attempt3 = 
        let min1 = stringOfTy denv ty1
        let min2 = stringOfTy denv ty2
        if min1 <> min2 then Some (min1, min2, stringOfTyparConstraints denv tpcs) else None

    match attempt3 with 
    | Some res -> res 
    | None -> 

    let attempt4 = 
        // try denv + show full paths + static parameters
        let denv = denv.SetOpenPaths []
        let denv = { denv with includeStaticParametersInTypeNames=true }
        let min1 = stringOfTy denv ty1
        let min2 = stringOfTy denv ty2
        if min1 <> min2 then Some (min1, min2, stringOfTyparConstraints denv tpcs) else None

    match attempt4 with
    | Some res -> res
    | None ->
        // https://github.com/dotnet/fsharp/issues/2561
        // still identical, we better (try to) show assembly qualified name to disambiguate
        let denv = denv.SetOpenPaths []
        let denv = { denv with includeStaticParametersInTypeNames=true }
        let makeName t =
            let assemblyName = PrintTypes.layoutAssemblyName denv t |> function | null | "" -> "" | name -> sprintf " (%s)" name
            sprintf "%s%s" (stringOfTy denv t) assemblyName

        (makeName ty1, makeName ty2, stringOfTyparConstraints denv tpcs)
    
// Note: Always show imperative annotations when comparing value signatures 
let minimalStringsOfTwoValues denv infoReader vref1 vref2 = 
    let denvMin = { denv with showInferenceTyparAnnotations=true; showStaticallyResolvedTyparAnnotations=false }
    let min1 = buildString (fun buf -> outputQualifiedValOrMember denvMin infoReader buf vref1)
    let min2 = buildString (fun buf -> outputQualifiedValOrMember denvMin infoReader buf vref2) 
    if min1 <> min2 then 
        (min1, min2) 
    else
        let denvMax = { denv with showInferenceTyparAnnotations=true; showStaticallyResolvedTyparAnnotations=true }
        let max1 = buildString (fun buf -> outputQualifiedValOrMember denvMax infoReader buf vref1)
        let max2 = buildString (fun buf -> outputQualifiedValOrMember denvMax infoReader buf vref2) 
        max1, max2
    
let minimalStringOfType denv ty = 
    let ty, _cxs = PrettyTypes.PrettifyType denv.g ty
    let denvMin = { denv with showInferenceTyparAnnotations=false; showStaticallyResolvedTyparAnnotations=false }
    showL (PrintTypes.layoutTypeWithInfoAndPrec denvMin SimplifyTypes.typeSimplificationInfo0 2 ty)

