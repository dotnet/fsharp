module internal Fsharp.Compiler.SignatureHash

let x = 42

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

let inline hashText (s:string) = hash s
let inline combineHash x y = (x <<< 1) + y + 631
let inline addToHash (value) (acc:int) = combineHash acc (hash value)
let inline combineHashes (hashes: int list) = (0,hashes) ||> List.fold (fun acc curr -> combineHash acc curr)
let (@@) h1 h2 = combineHash h1 h2


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


    let isDiscard (name: string) = name.StartsWith("_")

    let ensureFloat (s: string) =
        if String.forall (fun c -> Char.IsDigit c || c = '-') s then
            s + ".0" 
        else s

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

            let hash =
                argTyL
                ^^ (if idx = lastIndex then
                        wordL (tagPunctuation retTyDelim)
                    else
                        wordL (tagPunctuation "->"))

            isTupled, hash)
        |> List.rev
        |> fun reversedArgs -> (true, retTyL) :: reversedArgs
        |> List.fold (fun acc (shouldBreak, hash) -> (if shouldBreak then (---) else (++)) hash acc) 0 (* empty hash *)

    let tagNavArbValRef (valRefOpt: ValRef option) tag =
        match valRefOpt with
        | Some vref ->
            tag |> mkNav vref.DefinitionRange
        | None ->
            tag


    
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

    let hashTyconRefImpl isAttribute (denv: DisplayEnv) (tcref: TyconRef) =

        let prefix = usePrefix denv tcref
        let isArray = not prefix && isArrayTyconRef denv.g tcref
        let demangled = 
            if isArray then
                let numberOfCommas = tcref.CompiledName |> Seq.filter (fun c -> c = ',') |> Seq.length
                if numberOfCommas = 0 then
                    "array"
                else
                    $"array{numberOfCommas + 1}d"
            else
                let name = tcref.DisplayNameWithStaticParameters
                    
                if isAttribute && name.EndsWithOrdinal("Attribute") then
                    String.dropSuffix name "Attribute"
                else 
                    name

        let tyconTagged =
            tagEntityRefName denv tcref demangled
            |> mkNav tcref.DefinitionRange

        let tyconTextL = tyconTagged |> wordL

        if denv.shortTypeNames then 
            tyconTextL
        else
            let path = tcref.CompilationPath.DemangledPath           
            let pathText = trimPathByDisplayEnv denv path
            if pathText = "" then tyconTextL else leftL (tagUnknownEntity pathText) ^^ tyconTextL

    let hashBuiltinAttribute (denv: DisplayEnv) (attrib: BuiltinAttribInfo) =
        let tcref = attrib.TyconRef
        squareAngleL (hashTyconRefImpl true denv tcref)
    

    let squashToWidth width hash =
        match width with
        | Some w -> Display.squashTo w hash
        | None -> hash
        
module PrintIL = 

    let fullySplitILTypeRef (tref: ILTypeRef) = 
        (List.collect splitNamespace (tref.Enclosing @ [DemangleGenericTypeName tref.Name])) 

    let hashILTypeRefName denv path =
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
        leftL (tagNamespace (trimPathByDisplayEnv denv p2)) ^^ wordL tagged
            

    let hashILTypeRef denv tref =
        let path = fullySplitILTypeRef tref
        hashILTypeRefName denv path

    let hashILArrayShape (ILArrayShape sh) = 
        SepL.leftBracket ^^ wordL (tagPunctuation (sh |> List.tail |> List.map (fun _ -> ",") |> String.concat "")) ^^ RightL.rightBracket // drop off one "," so that a n-dimensional array has n - 1 ","'s

    let paramsL (ps: Layout list) : Layout = 
        match ps with
        | [] -> 0 (* empty hash *)
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
 
    let rec hashILType (denv: DisplayEnv) (ilTyparSubst: Layout list) (ty: ILType) : Layout =
        match ty with
        | ILType.Void -> WordL.structUnit // These are type-theoretically totally different type-theoretically `void` is Fin 0 and `unit` is Fin (S 0) ... but, this looks like as close as we can get.
        | ILType.Array (sh, t) -> hashILType denv ilTyparSubst t ^^ hashILArrayShape sh
        | ILType.Value t
        | ILType.Boxed t -> hashILTypeRef denv t.TypeRef ^^ (t.GenericArgs |> List.map (hashILType denv ilTyparSubst) |> paramsL)
        | ILType.Ptr t
        | ILType.Byref t -> hashILType denv ilTyparSubst t
        | ILType.FunctionPointer t -> hashILCallingSignature denv ilTyparSubst None t
        | ILType.TypeVar n -> List.item (int n) ilTyparSubst
        | ILType.Modified (_, _, t) -> hashILType denv ilTyparSubst t // Just recurse through them to the contained ILType

    /// Layout a function pointer signature using type-only-F#-style. No argument names are printed.
    and hashILCallingSignature denv ilTyparSubst cons (signature: ILCallingSignature) =
        // We need a special case for
        // constructors (Their return types are reported as `void`, but this is
        // incorrect; so if we're dealing with a constructor we require that the
        // return type be passed along as the `cons` parameter.)
        let args = signature.ArgTypes |> List.map (hashILType denv ilTyparSubst) 
        let res = 
            match cons with
            | Some className -> 
                let names = SplitNamesForILPath (DemangleGenericTypeName className)
                // special case for constructor return-type (viz., the class itself)
                hashILTypeRefName denv names ^^ (pruneParams className ilTyparSubst |> paramsL) 
            | None -> 
                signature.ReturnType |> hashILType denv ilTyparSubst
        
        match args with
        | [] -> WordL.structUnit ^^ WordL.arrow ^^ res
        | [x] -> x ^^ WordL.arrow ^^ res
        | _ -> sepListL WordL.star args ^^ WordL.arrow ^^ res

    let hashILFieldInit x =  Unchecked.defaulto
    let hashILEnumCase nm litVal =
        let nameL = ConvertLogicalNameToDisplayLayout (tagEnum >> wordL) nm
        WordL.bar ^^ nameL ^^ hashILFieldInit litVal

module PrintTypes = 
    // Note: We need nice printing of constants in order to print literals and attributes 
    let hashConst g ty c =
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

    let hashAccessibilityCore (denv: DisplayEnv) accessibility =
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
        | Public, Internal -> WordL.keywordInternal
        | Public, Private -> WordL.keywordPrivate
        | Internal, Private -> WordL.keywordPrivate
        | _ -> 0 (* empty hash *)
    
    let hashAccessibility (denv: DisplayEnv) accessibility itemL =
        hashAccessibilityCore denv accessibility ++ itemL

    /// Layout a reference to a type 
    let hashTyconRef denv tcref = hashTyconRefImpl false denv tcref

    /// Layout the flags of a member 
    let hashMemberFlags (memFlags: SynMemberFlags) = 
        let stat = 
            if memFlags.IsInstance || (memFlags.MemberKind = SynMemberKind.Constructor) then 0 (* empty hash *) 
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
    let rec hashAttribArg denv arg = 
        match arg with 
        | Expr.Const (c, _, ty) -> 
            if isEnumTy denv.g ty then 
                WordL.keywordEnum ^^ angleL (hashType denv ty) ^^ bracketL (hashConst denv.g ty c)
            else
                hashConst denv.g ty c

        | Expr.Op (TOp.Array, [_elemTy], args, _) ->
            LeftL.leftBracketBar ^^ semiListL (List.map (hashAttribArg denv) args) ^^ RightL.rightBracketBar

        // Detect 'typeof<ty>' calls 
        | TypeOfExpr denv.g ty ->
            LeftL.keywordTypeof ^^ wordL (tagPunctuation "<") ^^ hashType denv ty ^^ rightL (tagPunctuation ">")

        // Detect 'typedefof<ty>' calls 
        | TypeDefOfExpr denv.g ty ->
            LeftL.keywordTypedefof ^^ wordL (tagPunctuation "<") ^^ hashType denv ty ^^ rightL (tagPunctuation ">")

        | Expr.Op (TOp.Coerce, [tgtTy;_], [arg2], _) ->
            leftL (tagPunctuation "(") ^^ hashAttribArg denv arg2 ^^ wordL (tagPunctuation ":>") ^^ hashType denv tgtTy ^^ rightL (tagPunctuation ")")

        | AttribBitwiseOrExpr denv.g (arg1, arg2) ->
            hashAttribArg denv arg1 ^^ wordL (tagPunctuation "|||") ^^ hashAttribArg denv arg2

        // Detect explicit enum values 
        | EnumExpr denv.g arg1 ->
            WordL.keywordEnum ++ bracketL (hashAttribArg denv arg1)

        | _ -> comment "(* unsupported attribute argument *)"

    /// Layout arguments of an attribute 'arg1, ..., argN' 
    and hashAttribArgs denv args props = 
        let argsL =  args |> List.map (fun (AttribExpr(e1, _)) -> hashAttribArg denv e1)
        let propsL =
            props
            |> List.map (fun (AttribNamedArg(name,_, _, AttribExpr(e1, _))) ->
                wordL (tagProperty name) ^^ WordL.equals ^^ hashAttribArg denv e1)
        sepListL (rightL (tagPunctuation ",")) (argsL @ propsL)

    /// Layout an attribute 'Type(arg1, ..., argN)' 
    and hashAttrib denv (Attrib(tcref, _, args, props, _, _, _)) = 
        let tcrefL = hashTyconRefImpl true denv tcref
        let argsL = bracketL (hashAttribArgs denv args props)
        if List.isEmpty args && List.isEmpty props then
            tcrefL
        else
            tcrefL ++ argsL

    and hashILAttribElement denv arg = Unchecked.defaultof<_>
    // Why null? Because for hash of a signature, contents of attribute should not matter


    and hashILAttrib denv (ty, args) = 
        let argsL = bracketL (sepListL (rightL (tagPunctuation ",")) (List.map (hashILAttribElement denv) args))
        PrintIL.hashILType denv [] ty ++ argsL

    /// Layout '[<attribs>]' above another block 
    and hashAttribs denv startOpt isLiteral kind attrs restL = 
        
        let attrsL = 
            [ if denv.showAttributes then
                for attr in attrs do
                    hashAttrib denv attr

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
            
    and hashTyparAttribs denv kind attrs restL =
        match attrs, kind with
        | [], TyparKind.Type -> restL 
        | _, _ -> squareAngleL (sepListL (rightL (tagPunctuation ";")) ((match kind with TyparKind.Type -> [] | TyparKind.Measure -> [wordL (tagText "Measure")]) @ List.map (hashAttrib denv) attrs)) ^^ restL

    and hashTyparRef denv (typar: Typar) =
        tagTypeParameter 
            (sprintf "%s%s%s"
                (prefixOfStaticReq typar.StaticReq )
                (prefixOfInferenceTypar typar )
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
    and hashTyparRefWithInfo denv (env: SimplifyTypes.TypeSimplificationInfo) (typar: Typar) =
        let varL = hashTyparRef denv typar
        let varL =  hashTyparAttribs denv typar.Kind typar.Attribs varL 

        match Zmap.tryFind typar env.inplaceConstraints with
        | Some typarConstraintTy ->
            if Zset.contains typar env.singletons then
                leftL (tagPunctuation "#") ^^ hashTypeWithInfo denv env typarConstraintTy
            else
                (varL ^^ sepL (tagPunctuation ":>") ^^ hashTypeWithInfo denv env typarConstraintTy) |> bracketL

        | _ -> varL

      
    /// Layout type parameter constraints, taking TypeSimplificationInfo into account 
    and hashConstraintsWithInfo denv env cxs = 
        
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

        let cxsL = List.collect (hashConstraintWithInfo denv env) cxs
        match cxsL with 
        | [] -> 0 (* empty hash *) 
        | _ ->  wordL (tagKeyword "when") ^^ sepListL (wordL (tagKeyword "and")) cxsL
                

    /// Layout constraints, taking TypeSimplificationInfo into account 
    and hashConstraintWithInfo denv env (tp, tpc) =
        let longConstraintPrefix l = (hashTyparRefWithInfo denv env tp |> addColonL) ^^ l
        match tpc with 
        | TyparConstraint.CoercesTo(tgtTy, _) -> 
            [hashTyparRefWithInfo denv env tp ^^ wordL (tagOperator ":>") --- hashTypeWithInfo denv env tgtTy]

        | TyparConstraint.MayResolveMember(traitInfo, _) ->
            [hashTraitWithInfo denv env traitInfo]

        | TyparConstraint.DefaultsTo(_, ty, _) ->
              if denv.showTyparDefaultConstraints then 
                  [wordL (tagKeyword "default") ^^ (hashTyparRefWithInfo denv env tp  |> addColonL) ^^ hashTypeWithInfo denv env ty]
              else []

        | TyparConstraint.IsEnum(ty, _) ->
            if denv.shortConstraints then 
                [wordL (tagKeyword "enum")]
            else
                [longConstraintPrefix (hashTypeAppWithInfoAndPrec denv env (wordL (tagKeyword "enum")) 2 true [ty])]

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
                [hashTypeAppWithInfoAndPrec denv env WordL.keywordDelegate 2 true [aty;bty] |> longConstraintPrefix]

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
            [bracketL (sepListL (sepL (tagPunctuation "|")) (List.map (hashTypeWithInfo denv env) tys)) |> longConstraintPrefix]

        | TyparConstraint.RequiresDefaultConstructor _ -> 
            if denv.shortConstraints then 
                [wordL (tagKeyword "default") ^^ wordL (tagKeyword "constructor")]
            else
                [bracketL (
                    (WordL.keywordNew |> addColonL) ^^
                    WordL.structUnit ^^ 
                    WordL.arrow ^^
                    (hashTyparRefWithInfo denv env tp)) |> longConstraintPrefix]

    and hashTraitWithInfo denv env traitInfo =
        let g = denv.g
        let (TTrait(tys, _, memFlags, _, _, _)) = traitInfo
        let nm = traitInfo.MemberDisplayNameCore
        let nameL = ConvertValLogicalNameToDisplayLayout false (tagMember >> wordL) nm
        if denv.shortConstraints then 
            WordL.keywordMember ^^ nameL
        else
            let retTy = traitInfo.GetReturnType(g)
            let argTys = traitInfo.GetLogicalArgumentTypes(g)
            let argTys, retTy =
                match memFlags.MemberKind with
                | SynMemberKind.PropertySet ->
                    match List.tryFrontAndBack argTys with
                    | Some res -> res
                    | None -> argTys, retTy
                | _ ->
                    argTys, retTy

            let stat = hashMemberFlags memFlags
            let tys = ListSet.setify (typeEquiv g) tys
            let tysL = 
                match tys with 
                | [ty] -> hashTypeWithInfo denv env ty 
                | tys -> bracketL (hashTypesWithInfoAndPrec denv env 2 (wordL (tagKeyword "or")) tys)

            let retTyL = hashReturnType denv env retTy
            let sigL =
                match argTys with
                // Empty arguments indicates a non-indexer property constraint
                | [] -> retTyL
                | _ ->
                    let argTysL = hashTypesWithInfoAndPrec denv env 2 (wordL (tagPunctuation "*")) argTys
                    curriedLayoutsL "->" [argTysL] retTyL
            let getterSetterL =
                match memFlags.MemberKind with
                | SynMemberKind.PropertyGet when not argTys.IsEmpty ->
                    wordL (tagKeyword "with") ^^ wordL (tagText "get")
                | SynMemberKind.PropertySet ->
                    wordL (tagKeyword "with") ^^ wordL (tagText "set")
                | _ ->
                    0 (* empty hash *)
            (tysL |> addColonL) --- bracketL (stat ++ (nameL |> addColonL) --- sigL --- getterSetterL)

    /// Layout a unit of measure expression 
    and hashMeasure denv unt =
        let sortVars vs = vs |> List.sortBy (fun (tp: Typar, _) -> tp.DisplayName) 
        let sortCons cs = cs |> List.sortBy (fun (tcref: TyconRef, _) -> tcref.DisplayName) 
        let negvs, posvs = ListMeasureVarOccsWithNonZeroExponents unt |> sortVars |> List.partition (fun (_, e) -> SignRational e < 0)
        let negcs, poscs = ListMeasureConOccsWithNonZeroExponents denv.g false unt |> sortCons |> List.partition (fun (_, e) -> SignRational e < 0)
        let unparL uv = hashTyparRef denv uv
        let unconL tc = hashTyconRef denv tc
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
    and hashTypeAppWithInfoAndPrec denv env tcL prec prefix argTys =
        if prefix then 
            match argTys with
            | [] -> tcL
            | [argTy] -> tcL ^^ sepL (tagPunctuation "<") ^^ (hashTypeWithInfoAndPrec denv env 4 argTy) ^^ rightL (tagPunctuation">")
            | _ -> bracketIfL (prec <= 1) (tcL ^^ angleL (hashTypesWithInfoAndPrec denv env 2 (sepL (tagPunctuation ",")) argTys))
        else
            match argTys with
            | [] -> tcL
            | [arg] -> hashTypeWithInfoAndPrec denv env 2 arg ^^ tcL
            | args -> bracketIfL (prec <= 1) (bracketL (hashTypesWithInfoAndPrec denv env 2 (sepL (tagPunctuation ",")) args) --- tcL)
    
    /// Layout a type, taking precedence into account to insert brackets where needed
    and hashTypeWithInfoAndPrec denv env prec ty =
        let g = denv.g
        match stripTyparEqns ty with 

        // Always prefer to format 'byref<ty, ByRefKind.In>' as 'inref<ty>'
        | ty when isInByrefTy g ty && (match ty with TType_app (tc, _, _) when g.inref_tcr.CanDeref && tyconRefEq g tc g.byref2_tcr -> true | _ -> false) ->
            hashTypeWithInfoAndPrec denv env prec (mkInByrefTy g (destByrefTy g ty))

        // Always prefer to format 'byref<ty, ByRefKind.Out>' as 'outref<ty>'
        | ty when isOutByrefTy g ty && (match ty with TType_app (tc, _, _) when g.outref_tcr.CanDeref && tyconRefEq g tc g.byref2_tcr -> true | _ -> false) ->
            hashTypeWithInfoAndPrec denv env prec (mkOutByrefTy g (destByrefTy g ty))

        // Always prefer to format 'byref<ty, ByRefKind.InOut>' as 'byref<ty>'
        | ty when isByrefTy g ty && (match ty with TType_app (tc, _, _) when g.byref_tcr.CanDeref && tyconRefEq g tc g.byref2_tcr -> true | _ -> false) ->
            hashTypeWithInfoAndPrec denv env prec (mkByrefTy g (destByrefTy g ty))

        // Always prefer 'float' to 'float<1>'
        | TType_app (tc, args, _) when tc.IsMeasureableReprTycon && List.forall (isDimensionless g) args ->
          hashTypeWithInfoAndPrec denv env prec (reduceTyconRefMeasureableOrProvided g tc args)

        // Layout a type application
        | TType_ucase (UnionCaseRef(tc, _), args)
        | TType_app (tc, args, _) ->
          let prefix = usePrefix denv tc
          hashTypeAppWithInfoAndPrec denv env (hashTyconRef denv tc) prec prefix args

        // Layout a tuple type 
        | TType_anon (anonInfo, tys) ->
            let core = sepListL (rightL (tagPunctuation ";")) (List.map2 (fun nm ty -> wordL (tagField nm) ^^ rightL (tagPunctuation ":") ^^ hashTypeWithInfoAndPrec denv env prec ty) (Array.toList anonInfo.SortedNames) tys)
            if evalAnonInfoIsStruct anonInfo then 
                WordL.keywordStruct --- braceBarL core
            else 
                braceBarL core

        // Layout a tuple type 
        | TType_tuple (tupInfo, t) ->
            let elsL = hashTypesWithInfoAndPrec denv env 2 (wordL (tagPunctuation "*")) t
            if evalTupInfoIsStruct tupInfo then 
                WordL.keywordStruct --- bracketL elsL
            else 
                bracketIfL (prec <= 2) elsL

        // Layout a first-class generic type. 
        | TType_forall (tps, tau) ->
            let tauL = hashTypeWithInfoAndPrec denv env prec tau
            match tps with 
            | [] -> tauL
            | [h] -> hashTyparRefWithInfo denv env h ^^ rightL (tagPunctuation ".") --- tauL
            | h :: t -> spaceListL (List.map (hashTyparRefWithInfo denv env) (h :: t)) ^^ rightL (tagPunctuation ".") --- tauL

        | TType_fun _ ->
            let argTys, retTy = stripFunTy g ty
            let retTyL = hashTypeWithInfoAndPrec denv env 5 retTy
            let argTysL = argTys |> List.map (hashTypeWithInfoAndPrec denv env 4)
            let funcTyL = curriedLayoutsL "->" argTysL retTyL
            bracketIfL (prec <= 4) funcTyL

        // Layout a type variable . 
        | TType_var (r, _) ->
            hashTyparRefWithInfo denv env r

        | TType_measure unt -> hashMeasure denv unt

    /// Layout a list of types, separated with the given separator, either '*' or ','
    and hashTypesWithInfoAndPrec denv env prec sep typl = 
        sepListL sep (List.map (hashTypeWithInfoAndPrec denv env prec) typl)

    and hashReturnType denv env retTy = hashTypeWithInfoAndPrec denv env 4 retTy

    /// Layout a single type, taking TypeSimplificationInfo into account 
    and hashTypeWithInfo denv env ty = 
        hashTypeWithInfoAndPrec denv env 5 ty

    and hashType denv ty = 
        hashTypeWithInfo denv SimplifyTypes.typeSimplificationInfo0 ty

    // Format each argument, including its name and type 
    let hashArgInfo denv env (ty, argInfo: ArgReprInfo) = 
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
            hashTypeWithInfoAndPrec denv env 2 ty 

        // Layout an unnamed argument 
        | None, _, _, _ -> 
            hashTypeWithInfoAndPrec denv env 2 ty

        // Layout a named argument 
        | Some id, _, isParamArray, _ -> 
            let idL = ConvertValLogicalNameToDisplayLayout false (tagParameter >> wordL) id.idText
            let prefix =
                if isParamArray then
                    hashBuiltinAttribute denv g.attrib_ParamArrayAttribute ^^ idL
                else
                    idL
            (prefix |> addColonL) ^^ hashTypeWithInfoAndPrec denv env 2 ty

    let hashCurriedArgInfos denv env argInfos =
        argInfos 
        |> List.mapSquared (hashArgInfo denv env)
        |> List.map (sepListL (wordL (tagPunctuation "*")))

    let hashGenericParameterTypes denv env genParamTys = 
      match genParamTys with
      | [] -> 0 (* empty hash *)
      | _ ->
        wordL (tagPunctuation "<")
        ^^
        (
          genParamTys
          |> List.map (hashTypeWithInfoAndPrec denv env 4)
          |> sepListL (wordL (tagPunctuation ","))
        ) 
        ^^
        wordL (tagPunctuation ">")

    /// Layout a single type used as the type of a member or value 
    let hashTopType denv env argInfos retTy cxs =
        let retTyL = hashReturnType denv env retTy
        let cxsL = hashConstraintsWithInfo denv env cxs
        match argInfos with
        | [] -> retTyL --- cxsL
        | _ -> 
            let retTyDelim = if denv.useColonForReturnType then ":" else "->"
            let allArgsL = hashCurriedArgInfos denv env argInfos
            curriedLayoutsL retTyDelim allArgsL retTyL --- cxsL

    /// Layout type parameters
    let hashTyparDecls denv nmL prefix (typars: Typars) =
        let env = SimplifyTypes.typeSimplificationInfo0 
        let tpcs = typars |> List.collect (fun tp -> List.map (fun tpc -> tp, tpc) tp.Constraints) 
        match typars, tpcs with 
        | [], []  -> 
            nmL

        | [h], [] when not prefix -> 
            hashTyparRefWithInfo denv env h --- nmL

        | _ -> 
            let tpcsL = hashConstraintsWithInfo denv env tpcs
            let coreL = sepListL (sepL (tagPunctuation ",")) (List.map (hashTyparRefWithInfo denv env) typars)
            if prefix || not (isNil tpcs) then
                nmL ^^ angleL (coreL --- tpcsL)
            else
                bracketL coreL --- nmL

    let hashTrait denv traitInfo =
        hashTraitWithInfo denv SimplifyTypes.typeSimplificationInfo0 traitInfo

    let hashTyparConstraint denv (tp, tpc) =
        match hashConstraintWithInfo denv SimplifyTypes.typeSimplificationInfo0 (tp, tpc) with 
        | h :: _ -> h 
        | [] -> 0 (* empty hash *)

    let prettyLayoutOfInstAndSig denv (typarInst, tys, retTy) =
        let (prettyTyparInst, prettyTys, prettyRetTy), cxs = PrettyTypes.PrettifyInstAndSig denv.g (typarInst, tys, retTy)
        let env = SimplifyTypes.CollectInfo true (prettyRetTy :: prettyTys) cxs
        let prettyTysL = List.map (hashTypeWithInfo denv env) prettyTys
        let prettyRetTyL = hashTopType denv env [[]] prettyRetTy []
        prettyTyparInst, (prettyTys, prettyRetTy), (prettyTysL, prettyRetTyL), hashConstraintsWithInfo denv env env.postfixConstraints

    let prettyLayoutOfTopTypeInfoAux denv prettyArgInfos prettyRetTy cxs = 
        let env = SimplifyTypes.CollectInfo true (prettyRetTy :: List.collect (List.map fst) prettyArgInfos) cxs
        hashTopType denv env prettyArgInfos prettyRetTy env.postfixConstraints

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
        let prettyTyparInst, hash = prettyLayoutOfCurriedMemberSig denv typarInst argInfos retTy parentTyparTys

        prettyTyparInst, niceMethodTypars, hash

    let prettyLayoutOfMemberType denv vref typarInst argInfos retTy = 
        match PartitionValRefTypars denv.g vref with
        | Some(_, _, memberMethodTypars, memberToParentInst, _) ->
            prettyLayoutOfMemberSigCore denv memberToParentInst (typarInst, memberMethodTypars, argInfos, retTy)
        | None -> 
            let prettyTyparInst, hash = prettyLayoutOfUncurriedSig denv typarInst (List.concat argInfos) retTy 
            prettyTyparInst, [], hash

    let prettyLayoutOfMemberSig denv (memberToParentInst, nm, methTypars, argInfos, retTy) = 
        let _, niceMethodTypars, tauL = prettyLayoutOfMemberSigCore denv memberToParentInst (emptyTyparInst, methTypars, argInfos, retTy)
        let nameL = ConvertValLogicalNameToDisplayLayout false (tagMember >> wordL) nm
        let nameL =
            if denv.showTyparBinding then
                hashTyparDecls denv nameL true niceMethodTypars
            else
                nameL
        (nameL |> addColonL) ^^ tauL

    /// hashs the elements of an unresolved overloaded method call:
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
        let cxsL = hashConstraintsWithInfo denv env env.postfixConstraints

        (List.foldBack (---) (hashCurriedArgInfos denv env [argInfos]) cxsL,
            hashReturnType denv env retTy,
            hashGenericParameterTypes denv env genParamTys)

    let prettyLayoutOfType denv ty = 
        let ty, cxs = PrettyTypes.PrettifyType denv.g ty
        let env = SimplifyTypes.CollectInfo true [ty] cxs
        let cxsL = hashConstraintsWithInfo denv env env.postfixConstraints
        hashTypeWithInfoAndPrec denv env 2 ty --- cxsL

    let prettyLayoutOfTrait denv traitInfo =
        let compgenId = SyntaxTreeOps.mkSynId Range.range0 unassignedTyparName
        let fakeTypar = Construct.NewTypar (TyparKind.Type, TyparRigidity.Flexible, SynTypar(compgenId, TyparStaticReq.None, true), false, TyparDynamicReq.No, [], false, false)
        fakeTypar.SetConstraints [TyparConstraint.MayResolveMember(traitInfo, Range.range0)]
        let ty, cxs = PrettyTypes.PrettifyType denv.g (mkTyparTy fakeTypar)
        let env = SimplifyTypes.CollectInfo true [ty] cxs
        // We expect one constraint, since we put one in.
        match env.postfixConstraints with
        | cx :: _ ->
             // We expect at most one per constraint
             sepListL 0 (* empty hash *) (hashConstraintWithInfo denv env cx)
        | [] -> 0 (* empty hash *)

    let prettyLayoutOfTypeNoConstraints denv ty =
        let ty, _cxs = PrettyTypes.PrettifyType denv.g ty
        hashTypeWithInfoAndPrec denv SimplifyTypes.typeSimplificationInfo0 5 ty

    let hashOfValReturnType denv (vref: ValRef) =
        match vref.ValReprInfo with 
        | None ->
            let tau = vref.TauType
            let _argTysl, retTy = stripFunTy denv.g tau
            hashReturnType denv SimplifyTypes.typeSimplificationInfo0 retTy
        | Some (ValReprInfo(_typars, argInfos, _retInfo)) -> 
            let tau = vref.TauType
            let _c, retTy = GetTopTauTypeInFSharpForm denv.g argInfos tau Range.range0
            hashReturnType denv SimplifyTypes.typeSimplificationInfo0 retTy

    let hashAssemblyName _denv (ty: TType) =
        ty.GetAssemblyName()

/// Printing TAST objects
module PrintTastMemberOrVals =
    open PrintTypes 

    let mkInlineL denv (v: Val) nameL = 
        if v.MustInline && not denv.suppressInlineKeyword then 
            wordL (tagKeyword "inline") ++ nameL 
        else 
            nameL

    let hashMemberName (denv: DisplayEnv) (vref: ValRef) niceMethodTypars tagFunction name =
        let nameL = ConvertValLogicalNameToDisplayLayout vref.IsBaseVal (tagFunction >> mkNav vref.DefinitionRange >> wordL) name
        let nameL =
            if denv.showMemberContainers then 
                hashTyconRef denv vref.MemberApparentEntity ^^ SepL.dot ^^ nameL
            else
                nameL
        let nameL = if denv.showTyparBinding then hashTyparDecls denv nameL true niceMethodTypars else nameL
        let nameL = hashAccessibility denv vref.Accessibility nameL
        nameL

    let prettyLayoutOfMemberShortOption denv typarInst (v: Val) short =
        let vref = mkLocalValRef v
        let membInfo = Option.get vref.MemberInfo
        let stat = hashMemberFlags membInfo.MemberFlags
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
                        let nameL = hashMemberName denv vref niceMethodTypars tagMember vref.DisplayNameCoreMangled
                        let nameL = if short then nameL else mkInlineL denv vref.Deref nameL
                        stat --- ((nameL  |> addColonL) ^^ tauL)
                prettyTyparInst, resL

            | SynMemberKind.ClassConstructor
            | SynMemberKind.Constructor ->
                let prettyTyparInst, _, tauL = prettyLayoutOfMemberType denv vref typarInst argInfos retTy
                let resL = 
                    if short then tauL
                    else
                        let newL = hashAccessibility denv vref.Accessibility WordL.keywordNew
                        stat ++ (newL |> addColonL) ^^ tauL
                prettyTyparInst, resL

            | SynMemberKind.PropertyGetSet ->
                emptyTyparInst, stat

            | SynMemberKind.PropertyGet ->
                if isNil argInfos then
                    // use error recovery because intellisense on an incomplete file will show this
                    errorR(Error(FSComp.SR.tastInvalidFormForPropertyGetter(), vref.Id.idRange))
                    let nameL = hashMemberName denv vref [] tagProperty vref.DisplayNameCoreMangled
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
                            let nameL = hashMemberName denv vref niceMethodTypars tagProperty vref.DisplayNameCoreMangled
                            stat --- ((nameL  |> addColonL) ^^ (if isNil argInfos then tauL else tauL --- (WordL.keywordWith ^^ WordL.keywordGet)))
                    prettyTyparInst, resL

            | SynMemberKind.PropertySet ->
                if argInfos.Length <> 1 || isNil argInfos.Head then
                    // use error recovery because intellisense on an incomplete file will show this
                    errorR(Error(FSComp.SR.tastInvalidFormForPropertySetter(), vref.Id.idRange))
                    let nameL = hashMemberName denv vref [] tagProperty vref.DisplayNameCoreMangled
                    let resL = stat --- nameL --- (WordL.keywordWith ^^ WordL.keywordSet)
                    emptyTyparInst, resL
                else
                    let argInfos, valueInfo = List.frontAndBack argInfos.Head
                    let prettyTyparInst, niceMethodTypars, tauL = prettyLayoutOfMemberType denv vref typarInst (if isNil argInfos then [] else [argInfos]) (fst valueInfo)
                    let resL =
                        if short then
                            (tauL --- (WordL.keywordWith ^^ WordL.keywordSet))
                        else
                            let nameL = hashMemberName denv vref niceMethodTypars tagProperty vref.DisplayNameCoreMangled
                            stat --- ((nameL |> addColonL) ^^ (tauL --- (WordL.keywordWith ^^ WordL.keywordSet)))
                    prettyTyparInst, resL

        prettyTyparInst, memberL

    let prettyLayoutOfMember denv typarInst (v:Val) = prettyLayoutOfMemberShortOption denv typarInst v false

    let prettyLayoutOfMemberNoInstShort denv v = 
        prettyLayoutOfMemberShortOption denv emptyTyparInst v true |> snd

    let hashOfLiteralValue literalValue =
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

    let hashNonMemberVal denv (tps, v: Val, tau, cxs) =
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
        let nameL = hashAccessibility denv v.Accessibility nameL
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
                hashTyparDecls denv nameL true tps 
            else nameL
        let valAndTypeL = (WordL.keywordVal ^^ (typarBindingsL |> addColonL)) --- hashTopType denv env argInfos retTy cxs
        let valAndTypeL =
            match denv.generatedValueLayout v with
            | None -> valAndTypeL
            | Some rhsL -> (valAndTypeL ++ WordL.equals) --- rhsL
        match v.LiteralValue with
        | Some literalValue -> valAndTypeL --- hashOfLiteralValue literalValue
        | None -> valAndTypeL

    let prettyLayoutOfValOrMember denv infoReader typarInst (vref: ValRef) =
        let prettyTyparInst, valL =
            match vref.MemberInfo with 
            | None ->
                let tps, tau = vref.GeneralizedType

                // adjust the type in case this is the 'this' pointer stored in a reference cell
                let tau = StripSelfRefCell(denv.g, vref.BaseOrThisInfo, tau)

                let (prettyTyparInst, prettyTypars, prettyTauTy), cxs = PrettyTypes.PrettifyInstAndTyparsAndType denv.g (typarInst, tps, tau)
                let resL = hashNonMemberVal denv (prettyTypars, vref.Deref, prettyTauTy, cxs)
                prettyTyparInst, resL
            | Some _ -> 
                prettyLayoutOfMember denv typarInst vref.Deref

        let valL =
            valL
            |> hashAttribs denv None vref.LiteralValue.IsSome TyparKind.Type vref.Attribs     

        prettyTyparInst, valL

    let prettyLayoutOfValOrMemberNoInst denv infoReader v =
        prettyLayoutOfValOrMember denv infoReader emptyTyparInst v |> snd

let hashTyparConstraint denv x = x |> PrintTypes.hashTyparConstraint denv

let outputType denv os x = x |> PrintTypes.hashType denv |> bufferL os

let hashType denv x = x |> PrintTypes.hashType denv

let outputTypars denv nm os x = x |> PrintTypes.hashTyparDecls denv (wordL nm) true |> bufferL os

let outputTyconRef denv os x = x |> PrintTypes.hashTyconRef denv |> bufferL os

let hashTyconRef denv x = x |> PrintTypes.hashTyconRef denv

let hashConst g ty c = PrintTypes.hashConst g ty c

let prettyLayoutOfMemberSig denv x = x |> PrintTypes.prettyLayoutOfMemberSig denv 

let prettyLayoutOfUncurriedSig denv argInfos tau = PrintTypes.prettyLayoutOfUncurriedSig denv argInfos tau

let prettyLayoutsOfUnresolvedOverloading denv argInfos retTy genericParameters = PrintTypes.prettyLayoutsOfUnresolvedOverloading denv argInfos retTy genericParameters

//-------------------------------------------------------------------------

/// Printing info objects
module InfoMemberPrinting = 

    /// Format the arguments of a method
    let hashParamData denv (ParamData(isParamArray, _isInArg, _isOutArg, optArgInfo, _callerInfo, nmOpt, _reflArgInfo, pty)) =
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
            PrintTypes.hashType denv pty

        // Layout an unnamed argument 
        | _, None, _ -> 
            PrintTypes.hashType denv pty

        // Layout a named ParamArray argument 
        | true, Some id, _ -> 
            let idL = ConvertValLogicalNameToDisplayLayout false (tagParameter >> wordL) id.idText
            hashBuiltinAttribute denv denv.g.attrib_ParamArrayAttribute ^^
            (idL  |> addColonL) ^^
            PrintTypes.hashType denv pty

        // Layout a named normal argument 
        | false, Some id, _ -> 
            let idL = ConvertValLogicalNameToDisplayLayout false (tagParameter >> wordL) id.idText
            (idL  |> addColonL) ^^
            PrintTypes.hashType denv pty

    let formatParamDataToBuffer denv os pd =
        hashParamData denv pd |> bufferL os
        
    /// Format a method info using "F# style".
    //
    // That is, this style:
    //     new: argName1: argType1 * ... * argNameN: argTypeN -> retType
    //     Method: argName1: argType1 * ... * argNameN: argTypeN -> retType
    //
    let hashMethInfoFSharpStyleCore (infoReader: InfoReader) m denv (minfo: MethInfo) minst =
        let amap = infoReader.amap

        match minfo.ArbitraryValRef with
        | Some vref ->
            PrintTastMemberOrVals.prettyLayoutOfValOrMemberNoInst denv infoReader vref
        | None ->
            let hash = 
                if not minfo.IsConstructor && not minfo.IsInstance then WordL.keywordStatic
                else 0 (* empty hash *)

            let nameL =
                if minfo.IsConstructor then
                    WordL.keywordNew
                else
                    let idL = ConvertValLogicalNameToDisplayLayout false (tagMethod >> tagNavArbValRef minfo.ArbitraryValRef >> wordL) minfo.LogicalName
                    WordL.keywordMember ^^
                    PrintTypes.hashTyparDecls denv idL true minfo.FormalMethodTypars

            let hash = hash ^^ (nameL |> addColonL)        

            let paramsL =
                let paramDatas = minfo.GetParamDatas(amap, m, minst)
                if List.forall isNil paramDatas then
                    WordL.structUnit
                else
                    sepListL WordL.arrow (List.map ((List.map (hashParamData denv)) >> sepListL WordL.star) paramDatas)

            let hash = hash ^^ paramsL
            
            let retL =
                let retTy = minfo.GetFSharpReturnType(amap, m, minst)
                WordL.arrow ^^
                PrintTypes.hashType denv retTy

            hash ^^ retL 

    /// Format a method info using "half C# style".
    //
    // That is, this style:
    //          Container(argName1: argType1, ..., argNameN: argTypeN) : retType
    //          Container.Method(argName1: argType1, ..., argNameN: argTypeN) : retType
    let hashMethInfoCSharpStyle amap m denv (minfo: MethInfo) minst =
        let retTy = if minfo.IsConstructor then minfo.ApparentEnclosingType else minfo.GetFSharpReturnType(amap, m, minst) 
        let hash = 
            if minfo.IsExtensionMember then
                LeftL.leftParen ^^ wordL (tagKeyword (FSComp.SR.typeInfoExtension())) ^^ RightL.rightParen
            else 0 (* empty hash *)
        let hash = 
            hash ^^
                if isAppTy minfo.TcGlobals minfo.ApparentEnclosingAppType then
                    let tcref = minfo.ApparentEnclosingTyconRef 
                    PrintTypes.hashTyconRef denv tcref
                else
                    0 (* empty hash *)
        let hash = 
            hash ^^
                if minfo.IsConstructor then
                    SepL.leftParen
                else
                    let idL = ConvertValLogicalNameToDisplayLayout false (tagMethod >> tagNavArbValRef minfo.ArbitraryValRef >> wordL) minfo.LogicalName
                    SepL.dot ^^
                    PrintTypes.hashTyparDecls denv idL true minfo.FormalMethodTypars ^^
                    SepL.leftParen

        let paramDatas = minfo.GetParamDatas (amap, m, minst)
        let hash = hash ^^ sepListL RightL.comma ((List.concat >> List.map (hashParamData denv)) paramDatas)
        hash ^^ RightL.rightParen ^^ WordL.colon ^^ PrintTypes.hashType denv retTy

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
            let resL = PrintTypes.hashTyconRef denv methInfo.ApparentEnclosingTyconRef ^^ wordL (tagPunctuation "()")
            prettyTyparInst, resL
        | FSMeth(_, _, vref, _) -> 
            let prettyTyparInst, resL = PrintTastMemberOrVals.prettyLayoutOfValOrMember { denv with showMemberContainers=true } infoReader typarInst vref
            prettyTyparInst, resL
        | ILMeth(_, ilminfo, _) -> 
            let prettyTyparInst, prettyMethInfo, minst = prettifyILMethInfo amap m methInfo typarInst ilminfo
            let resL = hashMethInfoCSharpStyle amap m denv prettyMethInfo minst
            prettyTyparInst, resL
#if !NO_TYPEPROVIDERS
        | ProvidedMeth _ -> 
            let prettyTyparInst, _ = PrettyTypes.PrettifyInst amap.g typarInst 
            prettyTyparInst, hashMethInfoCSharpStyle amap m denv methInfo methInfo.FormalMethodInst
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
                0 (* empty hash *)

        wordL (tagText (FSComp.SR.typeInfoProperty())) ^^
        hashTyconRef denv pinfo.ApparentEnclosingTyconRef ^^
        SepL.dot ^^
        (nameL  |> addColonL) ^^
        hashType denv retTy ^^
        getterSetter

    let formatPropInfoToBufferFreeStyle g amap m denv os (pinfo: PropInfo) = 
        let resL = prettyLayoutOfPropInfoFreeStyle g amap m denv pinfo 
        resL |> bufferL os

    let formatMethInfoToBufferFreeStyle amap m denv os (minfo: MethInfo) = 
        let _, resL = prettyLayoutOfMethInfoFreeStyle amap m denv emptyTyparInst minfo 
        resL |> bufferL os

    /// Format a method to a hash (actually just containing a string) using "free style" (aka "standalone"). 
    let hashMethInfoFSharpStyle amap m denv (minfo: MethInfo) =
        hashMethInfoFSharpStyleCore amap m denv minfo minfo.FormalMethodInst

//-------------------------------------------------------------------------

/// Printing TAST objects
module TashDefinitionHashes = 
    open PrintTypes

    let hashExtensionMember denv infoReader (vref: ValRef) =
        let (@@*) = if denv.printVerboseSignatures then (@@----) else (@@--)
        let tycon = vref.MemberApparentEntity.Deref
        let nameL = hashTyconRefImpl false denv vref.MemberApparentEntity
        let nameL = hashAccessibility denv tycon.Accessibility nameL // "type-accessibility"
        let tps =
            match PartitionValTyparsForApparentEnclosingType denv.g vref.Deref with
              | Some(_, memberParentTypars, _, _, _) -> memberParentTypars
              | None -> []
        let lhsL = WordL.keywordType ^^ hashTyparDecls denv nameL tycon.IsPrefixDisplay tps
        let memberL = PrintTastMemberOrVals.prettyLayoutOfValOrMemberNoInst denv infoReader vref
        (lhsL ^^ WordL.keywordWith) @@* memberL

    let hashExtensionMembers denv infoReader vs =
        combineHashes (List.map (hashExtensionMember denv infoReader) vs) 

    let hashRecdField prefix isClassDecl denv infoReader (enclosingTcref: TyconRef) (fld: RecdField) =
        let lhs = ConvertLogicalNameToDisplayLayout (tagRecordField >> mkNav fld.DefinitionRange >> wordL) fld.DisplayNameCore
        let lhs = (if isClassDecl then hashAccessibility denv fld.Accessibility lhs else lhs)
        let lhs = if fld.IsMutable then wordL (tagKeyword "mutable") --- lhs else lhs
        let fieldL =
            let rhs =
                match stripTyparEqns fld.FormalType with
                | TType_fun _ -> LeftL.leftParen ^^ hashType denv fld.FormalType ^^ RightL.rightParen
                | _ -> hashType denv fld.FormalType
            
            (lhs |> addColonL) --- rhs
        let fieldL = prefix fieldL
        let fieldL = fieldL |> hashAttribs denv None false TyparKind.Type (fld.FieldAttribs @ fld.PropertyAttribs)

        // The enclosing TyconRef might be a union and we can only get fields from union cases, so we need ignore unions here.
        fieldL

    let hashUnionOrExceptionField denv infoReader isGenerated enclosingTcref i (fld: RecdField) =
        if isGenerated i fld then
            hashTypeWithInfoAndPrec denv SimplifyTypes.typeSimplificationInfo0 2 fld.FormalType
        else
            hashRecdField id false denv infoReader enclosingTcref fld
    
    let isGeneratedUnionCaseField pos (f: RecdField) = 
        if pos < 0 then f.LogicalName = "Item"
        else f.LogicalName = "Item" + string (pos + 1)

    let isGeneratedExceptionField pos (f: RecdField) = 
        f.LogicalName = "Data" + (string pos)

    let hashUnionCaseFields denv infoReader isUnionCase enclosingTcref fields = 
        match fields with
        | [f] when isUnionCase ->
            hashUnionOrExceptionField denv infoReader isGeneratedUnionCaseField enclosingTcref -1 f
        | _ -> 
            let isGenerated = if isUnionCase then isGeneratedUnionCaseField else isGeneratedExceptionField
            sepListL WordL.star (List.mapi (hashUnionOrExceptionField denv infoReader isGenerated enclosingTcref) fields)

    let hashUnionCase denv infoReader prefixL enclosingTcref (ucase: UnionCase) =
        let nmL = ConvertLogicalNameToDisplayLayout (tagUnionCase >> mkNav ucase.DefinitionRange >> wordL) ucase.Id.idText
        //let nmL = hashAccessibility denv ucase.Accessibility nmL
        let caseL =
            match ucase.RecdFields with
            | []     -> (prefixL ^^ nmL)
            | fields -> (prefixL ^^ nmL ^^ WordL.keywordOf) --- hashUnionCaseFields denv infoReader true enclosingTcref fields
        caseL

    let hashUnionCases denv infoReader enclosingTcref ucases =
        let prefixL = WordL.bar // See bug://2964 - always prefix in case preceded by accessibility modifier
        List.map (hashUnionCase denv infoReader prefixL enclosingTcref) ucases

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
      
    let hashILFieldInfo denv (infoReader: InfoReader) m (finfo: ILFieldInfo) =
        let staticL = if finfo.IsStatic then WordL.keywordStatic else 0 (* empty hash *)
        let nameL = ConvertLogicalNameToDisplayLayout (tagField >> wordL) finfo.FieldName
        let typL = hashType denv (finfo.FieldType(infoReader.amap, m))
        let fieldL = staticL ^^ WordL.keywordVal ^^ (nameL |> addColonL) ^^ typL
        fieldL

    let hashEventInfo denv (infoReader: InfoReader) m (einfo: EventInfo) =
        let amap = infoReader.amap
        let staticL = if einfo.IsStatic then WordL.keywordStatic else 0 (* empty hash *)
        let nameL = ConvertValLogicalNameToDisplayLayout false (tagEvent >> tagNavArbValRef einfo.ArbitraryValRef >> wordL) einfo.EventName
        let typL = hashType denv (einfo.GetDelegateType(amap, m))
        let overallL = staticL ^^ WordL.keywordMember ^^ (nameL |> addColonL) ^^ typL
        overallL

    let hashPropInfo denv (infoReader: InfoReader) m (pinfo: PropInfo) =
        let amap = infoReader.amap

        let isPublicGetterSetter (getter: MethInfo) (setter: MethInfo) =
            let isPublicAccess access = access = TAccess []
            match getter.ArbitraryValRef, setter.ArbitraryValRef with
            | Some gRef, Some sRef -> isPublicAccess gRef.Accessibility && isPublicAccess sRef.Accessibility
            | _ -> false

        match pinfo.ArbitraryValRef with
        | Some vref ->
            let propL = PrintTastMemberOrVals.prettyLayoutOfValOrMemberNoInst denv infoReader vref
            if pinfo.HasGetter && pinfo.HasSetter && not pinfo.IsIndexer && isPublicGetterSetter pinfo.GetterMethod pinfo.SetterMethod then
                propL ^^ wordL (tagKeyword "with") ^^ wordL (tagText "get, set")
            else
                propL
        | None ->

            let modifierAndMember =
                if pinfo.IsStatic then
                    WordL.keywordStatic ^^ WordL.keywordMember
                else
                    WordL.keywordMember
        
            let nameL = ConvertValLogicalNameToDisplayLayout false (tagProperty >> tagNavArbValRef pinfo.ArbitraryValRef >> wordL) pinfo.PropertyName
            let typL = hashType denv (pinfo.GetPropertyType(amap, m))
            let overallL = modifierAndMember ^^ (nameL |> addColonL) ^^ typL
            overallL

    let hashTyconDefn (denv: DisplayEnv) (infoReader: InfoReader) ad m simplified isFirstType (tcref: TyconRef) =        
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

        let typewordL =
            if isFirstType then
                WordL.keywordType
            else
                wordL (tagKeyword "and") ^^ hashAttribs denv start false tycon.TypeOrMeasureKind tycon.Attribs 0 (* empty hash *)

        let nameL = ConvertLogicalNameToDisplayLayout (tagger >> mkNav tycon.DefinitionRange >> wordL) tycon.DisplayNameCore

        let nameL = hashAccessibility denv tycon.Accessibility nameL
        let denv = denv.AddAccessibility tycon.Accessibility 

        let lhsL =
            let tps = tycon.TyparsNoRange
            let tpsL = hashTyparDecls denv nameL tycon.IsPrefixDisplay tps
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
            if isRecdTy g ty || isUnionTy g ty || tycon.IsStructOrEnumTycon then
                tycon.ImmediateInterfacesOfFSharpTycon
                |> List.filter (fun (_, compgen, _) -> not compgen)
                |> List.map p13
            else 
                GetImmediateInterfacesOfType SkipUnrefInterfaces.Yes g amap m ty

        let iimplsLs =
            iimpls
            |> List.map (fun intfTy -> wordL (tagKeyword (if isInterfaceTy g ty then "inherit" else "interface")) -* hashType denv intfTy)

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
            ctors
            |> List.map (fun ctor -> InfoMemberPrinting.hashMethInfoFSharpStyle infoReader m denv ctor)

        let methLs = 
            meths
            |> List.groupBy (fun md -> md.DisplayNameCore)
            |> List.collect (fun (_, group) ->
                group
                |> List.sortBy sortKey
                |> List.map (fun methinfo -> ((not methinfo.IsConstructor, methinfo.IsInstance, methinfo.DisplayName, List.sum methinfo.NumArgs, methinfo.NumArgs.Length), InfoMemberPrinting.hashMethInfoFSharpStyle infoReader m denv methinfo)))
            |> List.sortBy fst
            |> List.map snd

        let ilFieldsL =
            ilFields
            |> List.map (fun x -> (true, x.IsStatic, x.FieldName, 0, 0), hashILFieldInfo denv infoReader m x)
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
            |> List.map (fun f -> hashRecdField (fun l -> WordL.keywordStatic ^^ WordL.keywordVal ^^ l) true denv infoReader tcref f)

        let instanceVals =
            if isRecdTy g ty then
                []
            else
                tycon.TrueInstanceFieldsAsList
                |> List.filter (fun f -> IsAccessible ad f.Accessibility && not (isDiscard f.DisplayNameCore))

        let instanceValLs =
            instanceVals
            |> List.map (fun f -> hashRecdField (fun l -> WordL.keywordVal ^^ l) true denv infoReader tcref f)
    
        let propLs =
            props
            |> List.map (fun x -> (true, x.IsStatic, x.PropertyName, 0, 0), hashPropInfo denv infoReader m x)
            |> List.sortBy fst
            |> List.map snd

        let eventLs = 
            events
            |> List.map (fun x -> (true, x.IsStatic, x.EventName, 0, 0), hashEventInfo denv infoReader m x)
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
            [ 
                match GetSuperTypeOfType g amap m ty with 
                | Some superTy when not (isObjTy g superTy) && not (isValueTypeTy g superTy) ->
                    superTy
                | _ -> ()
            ]

        let inheritsL = 
            inherits
            |> List.map (fun super -> wordL (tagKeyword "inherit") ^^ (hashType denv super))

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
                let memberLs = allDecls
                reprL @@ combineHashes memberLs

        let addReprAccessL l =
            hashAccessibility denv tycon.TypeReprAccessibility l 

        let addReprAccessRecord l =
            hashAccessibilityCore denv tycon.TypeReprAccessibility --- l
        
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

                // For records, use multi-line hash as soon as there is XML doc 
                //   type R =
                //     { 
                //         /// ABC
                //         Field1: int 
                //
                //         /// ABC
                //         Field2: int 
                //     }
                //
                // For records, use multi-line hash as soon as there is more than one field
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
                |> List.map (hashRecdField id false denv infoReader tcref)           
                |> combineHashes
                |> (if useMultiLine then braceMultiLineL else braceL)
                |> addReprAccessRecord
                |> addMaxMembers
                |> addLhs

            | TFSharpUnionRepr _ -> 
                let denv = denv.AddAccessibility tycon.TypeReprAccessibility 
                tycon.UnionCasesAsList
                |> hashUnionCases denv infoReader tcref           
                |> combineHashes
                |> addReprAccessL
                |> addMaxMembers
                |> addLhs
                  
            | TFSharpObjectRepr { fsobjmodel_kind = TFSharpDelegate slotSig } ->
                let (TSlotSig(_, _, _, _, paraml, retTy)) = slotSig
                let retTy = GetFSharpViewOfReturnType denv.g retTy
                let delegateL = WordL.keywordDelegate ^^ WordL.keywordOf -* hashTopType denv SimplifyTypes.typeSimplificationInfo0 (paraml |> List.mapSquared (fun sp -> (sp.Type, ValReprInfo.unnamedTopArg1))) retTy []
                delegateL
                |> addLhs

            // Measure declarations are '[<Measure>] type kg' unless abbreviations
            | TFSharpObjectRepr _ when isMeasure ->
                lhsL

            | TFSharpObjectRepr { fsobjmodel_kind = TFSharpEnum } ->
                tycon.TrueFieldsAsList
                |> List.map (fun f -> 
                    match f.LiteralValue with 
                    | None -> 0 (* empty hash *)
                    | Some c ->
                        WordL.bar ^^
                        wordL (tagField f.DisplayName) ^^
                        WordL.equals ^^ 
                        hashConst denv.g ty c)
                |> combineHashes
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
                |> combineHashes
                |> addLhs

            | TAsmRepr _ -> 
                let asmL = wordL (tagText "(# \"<Common IL Type Omitted>\" #)")
                asmL
                |> addLhs

            | TMeasureableRepr ty ->
                hashType denv ty
                |> addLhs

            | TILObjectRepr _ when tycon.ILTyconRawMetadata.IsEnum ->
                infoReader.GetILFieldInfosOfType (None, ad, m, ty) 
                |> List.filter (fun x -> x.FieldName <> "value__")
                |> List.map (fun x -> PrintIL.hashILEnumCase x.FieldName x.LiteralValue)
                |> combineHashes
                |> addLhs

            | TILObjectRepr _ ->
                allDecls          
                |> combineHashes
                |> addLhs

            | TNoRepr when tycon.TypeAbbrev.IsSome ->
                let abbreviatedTy = tycon.TypeAbbrev.Value
                (lhsL ^^ WordL.equals) -* (hashType { denv with shortTypeNames = false } abbreviatedTy)

            | _ when isNil allDecls ->
                lhsL
#if !NO_TYPEPROVIDERS
            | TProvidedNamespaceRepr _
            | TProvidedTypeRepr _
#endif
            | TNoRepr -> 
                allDecls      
                |> combineHashes
                |> addLhs

        typeDeclL 
        |> fun tdl -> if isFirstType then hashAttribs denv start false tycon.TypeOrMeasureKind tycon.Attribs tdl else tdl  

    // Layout: exception definition
    let hashExnDefn denv infoReader (exncref: EntityRef) =
        let (-*) = if denv.printVerboseSignatures then (-----) else (---)
        let exnc = exncref.Deref
        let nameL = ConvertLogicalNameToDisplayLayout (tagClass >> mkNav exncref.DefinitionRange >> wordL) exnc.DisplayNameCore
        let nameL = hashAccessibility denv exnc.TypeReprAccessibility nameL
        let exnL = wordL (tagKeyword "exception") ^^ nameL // need to tack on the Exception at the right of the name for goto definition
        let reprL = 
            match exnc.ExceptionInfo with 
            | TExnAbbrevRepr ecref -> WordL.equals -* hashTyconRef denv ecref
            | TExnAsmRepr _ -> WordL.equals -* wordL (tagText "(# ... #)")
            | TExnNone -> 0 (* empty hash *)
            | TExnFresh r -> 
                match r.TrueFieldsAsList with
                | [] -> 0 (* empty hash *)
                | r -> WordL.keywordOf -* hashUnionCaseFields denv infoReader false exncref r

        let overallL = exnL ^^ reprL
        overallL

    // Layout: module spec 

    let hashTyconDefns denv infoReader ad m (tycons: Tycon list) =
        match tycons with 
        | [] -> 0 (* empty hash *)
        | [h] when h.IsFSharpException -> hashExnDefn denv infoReader (mkLocalEntityRef h)
        | h :: t -> 
            let x = hashTyconDefn denv infoReader ad m false true (mkLocalEntityRef h)
            let xs = List.map (mkLocalEntityRef >> hashTyconDefn denv infoReader ad m false false) t
            combineHashes (x :: xs)

    let rec fullPath (mspec: ModuleOrNamespace) acc =
        if mspec.IsNamespace then
            match mspec.ModuleOrNamespaceType.ModuleAndNamespaceDefinitions |> List.tryHead with
            | Some next when next.IsNamespace ->
                fullPath next (acc @ [next.DisplayNameCore])
            | _ ->
                acc, mspec
        else
            acc, mspec

    let rec hashModuleOrNamespace (denv: DisplayEnv) (infoReader: InfoReader) ad m isFirstTopLevel (mspec: ModuleOrNamespace) =
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
            hashAttribs denv None false mspec.TypeOrMeasureKind mspec.Attribs headerL

        let shouldShow (v: Val) =
            (denv.showObsoleteMembers || not (CheckFSharpAttributesForObsolete denv.g v.Attribs)) &&
            (denv.showHiddenMembers || not (CheckFSharpAttributesForHidden denv.g v.Attribs))

        let entityLs =
            if mspec.IsNamespace then []
            else
                mspec.ModuleOrNamespaceType.AllEntities
                |> QueueList.toList
                |> List.map (fun entity -> hashEntityDefn denv infoReader ad m (mkLocalEntityRef entity))
            
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
                |> combineHashes

            let valsL =
                valLs
                |> combineHashes

            if isFirstTopLevel then
                combineHashes
                    [
                        headerL
                        entitiesL
                        valsL
                    ]
            else
                headerL @@* entitiesL @@ valsL

    and hashEntityDefn (denv: DisplayEnv) (infoReader: InfoReader) ad m (eref: EntityRef) =
        if eref.IsModuleOrNamespace then
            hashModuleOrNamespace denv infoReader ad m false eref.Deref       
        elif eref.IsFSharpException then
            hashExnDefn denv infoReader eref
        else
            hashTyconDefn denv infoReader ad m true true eref

open PrintTypes

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

    let rec imdefsL denv x = combineHashes (x |> List.map imdefL )

    and imdefL  x = 
        let filterVal (v: Val) = not v.IsCompilerGenerated && Option.isNone v.MemberInfo
        let filterExtMem (v: Val) = v.IsExtensionMember

        match x with 
        | TMDefRec(_, _opens, tycons, mbinds, _) -> 
            TashDefinitionHashes.hashTyconDefns denv infoReader ad m tycons @@ 
            (mbinds 
                |> List.choose (function ModuleOrNamespaceBinding.Binding bind -> Some bind | _ -> None) 
                |> valsOfBinds 
                |> List.filter filterExtMem
                |> List.map mkLocalValRef
                |> TashDefinitionHashes.hashExtensionMembers denv infoReader) @@

            (mbinds 
                |> List.choose (function ModuleOrNamespaceBinding.Binding bind -> Some bind | _ -> None) 
                |> valsOfBinds 
                |> List.filter filterVal
                |> List.map mkLocalValRef
                |> List.map (PrintTastMemberOrVals.prettyLayoutOfValOrMemberNoInst denv infoReader)
                |> combineHashes) @@

            (mbinds 
                |> List.choose (function ModuleOrNamespaceBinding.Module (mspec, def) -> Some (mspec, def) | _ -> None) 
                |> List.map (imbindL denv) 
                |> combineHashes)

        | TMDefLet(bind, _) -> 
            ([bind.Var] 
                |> List.filter filterVal 
                |> List.map mkLocalValRef
                |> List.map (PrintTastMemberOrVals.prettyLayoutOfValOrMemberNoInst denv infoReader) 
                |> combineHashes)

        | TMDefOpens _ -> 0 (* empty hash *)

        | TMDefs defs -> imdefsL denv defs

        | TMDefDo _ -> 0 (* empty hash *)

    and imbindL denv (mspec, def) = 
        let innerPath = (fullCompPathOfModuleOrNamespace mspec).AccessPath
        let outerPath = mspec.CompilationPath.AccessPath


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
            let nmL = hashAccessibility denv mspec.Accessibility nmL
            let denv = denv.AddAccessibility mspec.Accessibility
            let basic = imdefL denv def
            let modNameL = wordL (tagKeyword "module") ^^ nmL
            let basicL = modNameL @@ basic
            basicL
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
            // NOTE: explicitly not calling `hashXmlDoc` here, because even though
            // `ModuleOrNamespace` has a field for XmlDoc, it is never present at the parser
            // level for namespaces.  This should be changed if the parser/spec changes.
            basicL
        else
            // This is a module 
            let nmL = ConvertLogicalNameToDisplayLayout (tagModule >> mkNav mspec.DefinitionRange >> wordL) mspec.DisplayNameCore
            let nmL = hashAccessibility denv mspec.Accessibility nmL
            let denv = denv.AddAccessibility mspec.Accessibility
            let basic = imdefL denv def
            let modNameL =
                wordL (tagKeyword "module") ^^ nmL
                |> hashAttribs denv None false mspec.TypeOrMeasureKind mspec.Attribs
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
            basicL

    let emptyModuleOrNamespace mspec =
        let innerPath = (fullCompPathOfModuleOrNamespace mspec).AccessPath
        let pathL = innerPath |> List.map (fst >> ConvertLogicalNameToDisplayLayout (tagNamespace >> wordL))

        let keyword =
            if not mspec.IsImplicitNamespace && mspec.IsNamespace then
                "namespace"
            else
                "module"

        wordL (tagKeyword keyword) ^^ sepListL SepL.dot pathL

    imdefL denv expr

//--------------------------------------------------------------------------
