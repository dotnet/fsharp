// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

/// Some general F# utilities for mangling / unmangling / manipulating names.
/// Anything to do with special names of identifiers and other lexical rules
module public FSharp.Compiler.Syntax.PrettyNaming

open System
open System.Collections.Generic
open System.Collections.Concurrent
open System.Globalization
open System.Text

open FSharp.Compiler.AbstractIL
open Internal.Utilities.Library
open FSharp.Compiler.Text
open FSharp.Compiler.Text.Layout

//------------------------------------------------------------------------
// Operator name compilation
//-----------------------------------------------------------------------

[<Literal>]
let parenGet = ".()"

[<Literal>]
let parenSet = ".()<-"

[<Literal>]
let qmark = "?"

[<Literal>]
let qmarkSet = "?<-"

/// Prefix for compiled (mangled) operator names.
[<Literal>]
let opNamePrefix = "op_"

let opNameTable =
    [|
        ("[]", "op_Nil")
        ("::", "op_ColonColon")
        ("+", "op_Addition")
        ("~%", "op_Splice")
        ("~%%", "op_SpliceUntyped")
        ("~++", "op_Increment")
        ("~--", "op_Decrement")
        ("-", "op_Subtraction")
        ("*", "op_Multiply")
        ("**", "op_Exponentiation")
        ("/", "op_Division")
        ("@", "op_Append")
        ("^", "op_Concatenate")
        ("%", "op_Modulus")
        ("&&&", "op_BitwiseAnd")
        ("|||", "op_BitwiseOr")
        ("^^^", "op_ExclusiveOr")
        ("<<<", "op_LeftShift")
        ("~~~", "op_LogicalNot")
        (">>>", "op_RightShift")
        ("~+", "op_UnaryPlus")
        ("~-", "op_UnaryNegation")
        ("~&", "op_AddressOf")
        ("~&&", "op_IntegerAddressOf")
        ("&&", "op_BooleanAnd")
        ("||", "op_BooleanOr")
        ("<=", "op_LessThanOrEqual")
        ("=", "op_Equality")
        ("<>", "op_Inequality")
        (">=", "op_GreaterThanOrEqual")
        ("<", "op_LessThan")
        (">", "op_GreaterThan")
        ("|>", "op_PipeRight")
        ("||>", "op_PipeRight2")
        ("|||>", "op_PipeRight3")
        ("<|", "op_PipeLeft")
        ("<||", "op_PipeLeft2")
        ("<|||", "op_PipeLeft3")
        ("!", "op_Dereference")
        (">>", "op_ComposeRight")
        ("<<", "op_ComposeLeft")
        ("<< >>", "op_TypedQuotationUnicode")
        ("<<| |>>", "op_ChevronsBar")
        ("<@ @>", "op_Quotation")
        ("<@@ @@>", "op_QuotationUntyped")
        ("+=", "op_AdditionAssignment")
        ("-=", "op_SubtractionAssignment")
        ("*=", "op_MultiplyAssignment")
        ("/=", "op_DivisionAssignment")
        ("..", "op_Range")
        (".. ..", "op_RangeStep")
        (qmark, "op_Dynamic")
        (qmarkSet, "op_DynamicAssignment")
        (parenGet, "op_ArrayLookup")
        (parenSet, "op_ArrayAssign")
    |]

let opCharTranslateTable =
    [|
        ('>', "Greater")
        ('<', "Less")
        ('+', "Plus")
        ('-', "Minus")
        ('*', "Multiply")
        ('=', "Equals")
        ('~', "Twiddle")
        ('%', "Percent")
        ('.', "Dot")
        ('$', "Dollar")
        ('&', "Amp")
        ('|', "Bar")
        ('@', "At")
        ('#', "Hash")
        ('^', "Hat")
        ('!', "Bang")
        ('?', "Qmark")
        ('/', "Divide")
        (':', "Colon")
        ('(', "LParen")
        (',', "Comma")
        (')', "RParen")
        (' ', "Space")
        ('[', "LBrack")
        (']', "RBrack")
    |]

/// The set of characters usable in custom operators.
let opCharSet =
    let t = HashSet<_>()

    for c, _ in opCharTranslateTable do
        t.Add(c) |> ignore

    t

/// The characters that are allowed to be the first character of an identifier.
let IsIdentifierFirstCharacter c =
    if c = '_' then
        true
    else
        match Char.GetUnicodeCategory c with
        // Letters
        | UnicodeCategory.UppercaseLetter
        | UnicodeCategory.LowercaseLetter
        | UnicodeCategory.TitlecaseLetter
        | UnicodeCategory.ModifierLetter
        | UnicodeCategory.OtherLetter
        | UnicodeCategory.LetterNumber -> true
        | _ -> false

/// The characters that are allowed to be in an identifier.
let IsIdentifierPartCharacter c =
    if c = '\'' then
        true // Tick
    else
        match Char.GetUnicodeCategory c with
        // Letters
        | UnicodeCategory.UppercaseLetter
        | UnicodeCategory.LowercaseLetter
        | UnicodeCategory.TitlecaseLetter
        | UnicodeCategory.ModifierLetter
        | UnicodeCategory.OtherLetter
        | UnicodeCategory.LetterNumber
        // Numbers
        | UnicodeCategory.DecimalDigitNumber
        // Connectors
        | UnicodeCategory.ConnectorPunctuation
        // Combiners
        | UnicodeCategory.NonSpacingMark
        | UnicodeCategory.SpacingCombiningMark -> true
        | _ -> false

/// Keywords paired with their descriptions. Used in completion and quick info.
let keywordsWithDescription: (string * string) list =
    [
        "abstract", FSComp.SR.keywordDescriptionAbstract ()
        "and", FSComp.SR.keyworkDescriptionAnd ()
        "as", FSComp.SR.keywordDescriptionAs ()
        "assert", FSComp.SR.keywordDescriptionAssert ()
        "base", FSComp.SR.keywordDescriptionBase ()
        "begin", FSComp.SR.keywordDescriptionBegin ()
        "class", FSComp.SR.keywordDescriptionClass ()
        "const", FSComp.SR.keywordDescriptionConst ()
        "default", FSComp.SR.keywordDescriptionDefault ()
        "delegate", FSComp.SR.keywordDescriptionDelegate ()
        "do", FSComp.SR.keywordDescriptionDo ()
        "done", FSComp.SR.keywordDescriptionDone ()
        "downcast", FSComp.SR.keywordDescriptionDowncast ()
        "downto", FSComp.SR.keywordDescriptionDownto ()
        "elif", FSComp.SR.keywordDescriptionElif ()
        "else", FSComp.SR.keywordDescriptionElse ()
        "end", FSComp.SR.keywordDescriptionEnd ()
        "exception", FSComp.SR.keywordDescriptionException ()
        "extern", FSComp.SR.keywordDescriptionExtern ()
        "false", FSComp.SR.keywordDescriptionTrueFalse ()
        "finally", FSComp.SR.keywordDescriptionFinally ()
        "for", FSComp.SR.keywordDescriptionFor ()
        "fun", FSComp.SR.keywordDescriptionFun ()
        "function", FSComp.SR.keywordDescriptionFunction ()
        "global", FSComp.SR.keywordDescriptionGlobal ()
        "if", FSComp.SR.keywordDescriptionIf ()
        "in", FSComp.SR.keywordDescriptionIn ()
        "inherit", FSComp.SR.keywordDescriptionInherit ()
        "inline", FSComp.SR.keywordDescriptionInline ()
        "interface", FSComp.SR.keywordDescriptionInterface ()
        "internal", FSComp.SR.keywordDescriptionInternal ()
        "lazy", FSComp.SR.keywordDescriptionLazy ()
        "let", FSComp.SR.keywordDescriptionLet ()
        "let!", FSComp.SR.keywordDescriptionLetBang ()
        "match", FSComp.SR.keywordDescriptionMatch ()
        "match!", FSComp.SR.keywordDescriptionMatchBang ()
        "member", FSComp.SR.keywordDescriptionMember ()
        "module", FSComp.SR.keywordDescriptionModule ()
        "mutable", FSComp.SR.keywordDescriptionMutable ()
        "namespace", FSComp.SR.keywordDescriptionNamespace ()
        "new", FSComp.SR.keywordDescriptionNew ()
        "not", FSComp.SR.keywordDescriptionNot ()
        "null", FSComp.SR.keywordDescriptionNull ()
        "of", FSComp.SR.keywordDescriptionOf ()
        "open", FSComp.SR.keywordDescriptionOpen ()
        "or", FSComp.SR.keywordDescriptionOr ()
        "override", FSComp.SR.keywordDescriptionOverride ()
        "private", FSComp.SR.keywordDescriptionPrivate ()
        "public", FSComp.SR.keywordDescriptionPublic ()
        "rec", FSComp.SR.keywordDescriptionRec ()
        "return", FSComp.SR.keywordDescriptionReturn ()
        "return!", FSComp.SR.keywordDescriptionReturnBang ()
        "static", FSComp.SR.keywordDescriptionStatic ()
        "struct", FSComp.SR.keywordDescriptionStruct ()
        "then", FSComp.SR.keywordDescriptionThen ()
        "to", FSComp.SR.keywordDescriptionTo ()
        "true", FSComp.SR.keywordDescriptionTrueFalse ()
        "try", FSComp.SR.keywordDescriptionTry ()
        "type", FSComp.SR.keywordDescriptionType ()
        "upcast", FSComp.SR.keywordDescriptionUpcast ()
        "use", FSComp.SR.keywordDescriptionUse ()
        "use!", FSComp.SR.keywordDescriptionUseBang ()
        "val", FSComp.SR.keywordDescriptionVal ()
        "void", FSComp.SR.keywordDescriptionVoid ()
        "when", FSComp.SR.keywordDescriptionWhen ()
        "while", FSComp.SR.keywordDescriptionWhile ()
        "with", FSComp.SR.keywordDescriptionWith ()
        "yield", FSComp.SR.keywordDescriptionYield ()
        "yield!", FSComp.SR.keywordDescriptionYieldBang ()
        "->", FSComp.SR.keywordDescriptionRightArrow ()
        "<-", FSComp.SR.keywordDescriptionLeftArrow ()
        ":>", FSComp.SR.keywordDescriptionCast ()
        ":?>", FSComp.SR.keywordDescriptionDynamicCast ()
        "<@", FSComp.SR.keywordDescriptionTypedQuotation ()
        "@>", FSComp.SR.keywordDescriptionTypedQuotation ()
        "<@@", FSComp.SR.keywordDescriptionUntypedQuotation ()
        "@@>", FSComp.SR.keywordDescriptionUntypedQuotation ()
    ]

let keywordLookup = set (List.map fst keywordsWithDescription)

// Some legacy compat operator names are not encode using op_XYZ and this
// do not carry sufficient information to distinguish between
//     let (or) x y = x || y
//     let ``or`` x y = x || y
//     let (land) x y = x || y
//     let ``land`` x y = x || y
// All are deprecated except 'mod'. All except those two get double-backticks
let IsUnencodedOpName (name: string) =
    match name with
    | "mod" -> true
    | _ -> false

let IsUnencodedLegacyOpName (name: string) =
    match name with
    | "or"
    | "land"
    | "lor"
    | "lsl"
    | "lsr"
    | "asr"
    | "lxor" -> true
    | _ -> false

let IsIdentifierName (name: string) =
    not (keywordLookup.Contains name)
    && not (IsUnencodedOpName name)
    && not (IsUnencodedLegacyOpName name)
    && let nameLen = name.Length in

       nameLen > 0
       && IsIdentifierFirstCharacter name[0]
       && let rec loop i =
           (i >= nameLen || (IsIdentifierPartCharacter(name[i]) && loop (i + 1))) in
          loop 1

let rec isCoreActivePatternName (name: string) idx seenNonOpChar =
    if idx = name.Length - 1 then
        seenNonOpChar
    else
        let c = name[idx]

        if opCharSet.Contains(c) && c <> '|' && c <> ' ' then
            false
        else
            isCoreActivePatternName name (idx + 1) (seenNonOpChar || c <> '|')

/// Determines if the specified name is a valid name for an active pattern.
let IsActivePatternName (name: string) =
    // The name must contain at least one character between the starting and ending delimiters.
    let nameLen = name.Length

    if nameLen < 3 || name[0] <> '|' || name[nameLen - 1] <> '|' then
        false
    else
        isCoreActivePatternName name 1 false

/// Returns `true` if given string is an operator display name (DisplayName), e.g.
///    (::)
///    ([])
///    (|>>)
///    (+)
///    ( * )
/// Also returns true for core display names (DisplayNameCore) without the parens:
///    ::
///    []
///    |>>
///    +
///    *
let IsOperatorDisplayName (name: string) =
    let rec loop (name: string) idx endIndex =
        if idx = endIndex then
            true
        else
            let c = name[idx]

            if not (opCharSet.Contains(c)) || c = ' ' then
                false
            else
                loop name (idx + 1) endIndex

    let skipParens2 = name.StartsWithOrdinal("( ") && name.EndsWithOrdinal(" )")
    let skipParens1 = name.StartsWithOrdinal("(") && name.EndsWithOrdinal(")")

    let skip =
        if skipParens2 then 2
        elif skipParens1 then 1
        else 0

    let startIndex = skip
    let endIndex = name.Length - skip

    (startIndex < endIndex && loop name startIndex endIndex)
    || (name = ".. ..")
    || (name = "(.. ..)")

//IsOperatorDisplayName "+"
//IsOperatorDisplayName "(+)"
//IsOperatorDisplayName "(::)"
//IsOperatorDisplayName "::"
//IsOperatorDisplayName "([])"
//IsOperatorDisplayName "(*)"
//IsOperatorDisplayName "( ** )"
//IsOperatorDisplayName "(  )" // false
//IsOperatorDisplayName "( +)" // false

let IsPossibleOpName (name: string) = name.StartsWithOrdinal(opNamePrefix)

/// Compiles a custom operator into a mangled operator name.
/// For example, "!%" becomes "op_DereferencePercent".
/// This function should only be used for custom operators
/// if an operator is or potentially may be a built-in operator,
/// use the 'CompileOpName' function instead.
let compileCustomOpName =
    let t2 =
        let t2 = Dictionary<_, _> opCharTranslateTable.Length

        for x, y in opCharTranslateTable do
            t2.Add(x, y)

        t2

    /// The maximum length of the name for a custom operator character.
    /// This value is used when initializing StringBuilders to avoid resizing.
    let maxOperatorNameLength =
        opCharTranslateTable
        |> Array.maxBy (snd >> String.length)
        |> snd
        |> String.length

    /// Memoize compilation of custom operators.
    /// They're typically used more than once so this avoids some CPU and GC overhead.
    let compiledOperators = ConcurrentDictionary<_, string> StringComparer.Ordinal

    fun opp ->
        // Has this operator already been compiled?
        compiledOperators.GetOrAdd(
            opp,
            fun (op: string) ->
                let opLength = op.Length

                let sb =
                    StringBuilder(opNamePrefix, opNamePrefix.Length + (opLength * maxOperatorNameLength))

                for i = 0 to opLength - 1 do
                    let c = op[i]

                    match t2.TryGetValue c with
                    | true, x -> sb.Append(x) |> ignore
                    | false, _ -> sb.Append(c) |> ignore

                /// The compiled (mangled) operator name.
                let opName = sb.ToString()

                // Cache the compiled name so it can be reused.
                opName
        )

/// Maps the built-in F# operators to their mangled operator names.
let standardOpNames =
    let opNames = Dictionary<_, _>(opNameTable.Length, StringComparer.Ordinal)

    for x, y in opNameTable do
        opNames.Add(x, y)

    opNames

let CompileOpName op =
    match standardOpNames.TryGetValue op with
    | true, x -> x
    | false, _ ->
        if IsUnencodedOpName op || IsUnencodedLegacyOpName op || IsIdentifierName op then
            op
        else
            compileCustomOpName op

/// Decompiles the mangled name of a custom operator back into an operator.
/// For example, "op_DereferencePercent" becomes "!%".
/// This function should only be used for mangled names of custom operators
/// if a mangled name potentially represents a built-in operator,
/// use the 'ConvertValLogicalNameToDisplayNameCore' function instead.
let decompileCustomOpName =
    // Memoize this operation. Custom operators are typically used more than once
    // so this avoids repeating decompilation.
    let decompiledOperators = ConcurrentDictionary<_, _> StringComparer.Ordinal

    /// The minimum length of the name for a custom operator character.
    /// This value is used when initializing StringBuilders to avoid resizing.
    let minOperatorNameLength =
        opCharTranslateTable
        |> Array.minBy (snd >> String.length)
        |> snd
        |> String.length

    fun opName ->
        // Has this operator name already been decompiled?
        match decompiledOperators.TryGetValue opName with
        | true, op -> op
        | false, _ ->
            let opNameLen = opName.Length

            /// Function which decompiles the mangled operator name back into a string of operator characters.
            /// Returns None if the name contains text which doesn't correspond to an operator
            /// otherwise returns Some containing the original operator.
            let rec decompile (sb: StringBuilder) idx =
                // Have we reached the end of 'opName'?
                if idx = opNameLen then
                    // Finished decompiling.
                    // Cache the decompiled operator before returning so it can be reused.
                    let decompiledOp = sb.ToString()
                    decompiledOperators.TryAdd(opName, decompiledOp) |> ignore
                    decompiledOp
                else
                    let choice =
                        opCharTranslateTable
                        |> Array.tryFind (fun (_, opCharName) ->
                            // If this operator character name is longer than the remaining piece of 'opName',
                            // it's obviously not a match.
                            let opCharNameLen = opCharName.Length

                            if opNameLen - idx < opCharNameLen then
                                false
                            else
                                // Does 'opCharName' match the current position in 'opName'?
                                String.Compare(opName, idx, opCharName, 0, opCharNameLen, StringComparison.Ordinal) = 0)

                    match choice with
                    | None ->
                        // Couldn't decompile, so just return the original 'opName'.
                        opName
                    | Some (opChar, opCharName) ->
                        // 'opCharName' matched the current position in 'opName'.
                        // Append the corresponding operator character to the StringBuilder
                        // and continue decompiling at the index following this instance of 'opCharName'.
                        sb.Append opChar |> ignore
                        decompile sb (idx + opCharName.Length)

            let opNamePrefixLen = opNamePrefix.Length

            let sb =
                /// The maximum number of operator characters that could be contained in the
                /// decompiled operator given the length of the mangled custom operator name.
                let maxPossibleOpCharCount = (opNameLen - opNamePrefixLen) / minOperatorNameLength

                StringBuilder maxPossibleOpCharCount

            // Start decompiling just after the operator prefix.
            decompile sb opNamePrefixLen

/// Maps the mangled operator names of built-in F# operators back to the operators.
let standardOpsDecompile =
    let ops = Dictionary<string, string>(opNameTable.Length, StringComparer.Ordinal)

    for x, y in opNameTable do
        ops.Add(y, x)

    ops

let ConvertValLogicalNameToDisplayNameCore opName =
    match standardOpsDecompile.TryGetValue opName with
    | true, res -> res
    | false, _ ->
        if IsPossibleOpName opName then
            decompileCustomOpName opName
        else
            opName

let DoesIdentifierNeedBackticks (name: string) : bool =
    not (IsUnencodedOpName name)
    && not (IsIdentifierName name)
    && not (IsActivePatternName name)

/// A utility to help determine if an identifier needs to be quoted
let AddBackticksToIdentifierIfNeeded (name: string) : string =
    if
        DoesIdentifierNeedBackticks name
        && not (name.StartsWithOrdinal("`"))
        && not (name.EndsWithOrdinal("`"))
    then
        "``" + name + "``"
    else
        name

/// Quote identifier with double backticks if needed, remove unnecessary double backticks quotation.
let NormalizeIdentifierBackticks (name: string) : string =
    let s =
        if name.StartsWithOrdinal("``") && name.EndsWithOrdinal("``") then
            name[2 .. name.Length - 3]
        else
            name

    AddBackticksToIdentifierIfNeeded s

let ConvertLogicalNameToDisplayName name = AddBackticksToIdentifierIfNeeded name

let ConvertValLogicalNameToDisplayName isBaseVal name =
    if isBaseVal && name = "base" then
        "base"
    elif IsUnencodedOpName name || IsPossibleOpName name || IsActivePatternName name then
        let nm = ConvertValLogicalNameToDisplayNameCore name
        // Check for no decompilation, e.g. op_Implicit, op_NotAMangledOpName, op_A-B
        if IsPossibleOpName name && (nm = name) then
            AddBackticksToIdentifierIfNeeded nm
        // Add parentheses for multiply-like symbols, with spacing to avoid confusion with comments
        elif nm <> "*" && (nm.StartsWithOrdinal "*" || nm.EndsWithOrdinal "*") then
            "( " + nm + " )"
        // Add parentheses for other symbols, no spacing
        else
            "(" + nm + ")"
    else
        ConvertLogicalNameToDisplayName name

let ConvertLogicalNameToDisplayLayout nonOpLayout name =
    if DoesIdentifierNeedBackticks name then
        leftL (TaggedText.tagPunctuation "``")
        ^^ wordL (TaggedText.tagOperator name) ^^ rightL (TaggedText.tagPunctuation "``")
    else
        nonOpLayout name

let ConvertValLogicalNameToDisplayLayout isBaseVal nonOpLayout name =
    if isBaseVal && name = "base" then
        nonOpLayout "base"
    elif IsUnencodedOpName name || IsPossibleOpName name || IsActivePatternName name then
        let nm = ConvertValLogicalNameToDisplayNameCore name
        // Check for no decompilation, e.g. op_Implicit, op_NotAMangledOpName, op_A-B
        if IsPossibleOpName name && (nm = name) then
            ConvertLogicalNameToDisplayLayout nonOpLayout name
        elif nm.StartsWithOrdinal "*" || nm.EndsWithOrdinal "*" then
            wordL (TaggedText.tagPunctuation "(")
            ^^ wordL (TaggedText.tagOperator nm) ^^ wordL (TaggedText.tagPunctuation ")")
        else
            leftL (TaggedText.tagPunctuation "(")
            ^^ wordL (TaggedText.tagOperator nm) ^^ rightL (TaggedText.tagPunctuation ")")
    elif name = "get_Zero" then
        ConvertLogicalNameToDisplayLayout nonOpLayout "Zero"
    else
        ConvertLogicalNameToDisplayLayout nonOpLayout name

let opNameCons = CompileOpName "::"

let opNameNil = CompileOpName "[]"

let opNameEquals = CompileOpName "="

let opNameEqualsNullable = CompileOpName "=?"

let opNameNullableEquals = CompileOpName "?="

let opNameNullableEqualsNullable = CompileOpName "?=?"

/// Is this character a part of a long identifier?
let IsLongIdentifierPartCharacter c = c = '.' || IsIdentifierPartCharacter c

let isTildeOnlyString (s: string) =
    let rec loop (s: string) idx =
        if idx >= s.Length then true
        elif s[idx] <> '~' then false
        else loop s (idx + 1)

    loop s 0

let IsValidPrefixOperatorUse s =
    if String.IsNullOrEmpty s then
        false
    else
        match s with
        | "?+"
        | "?-"
        | "+"
        | "-"
        | "+."
        | "-."
        | "%"
        | "%%"
        | "&"
        | "&&" -> true
        | _ -> s[0] = '!' || isTildeOnlyString s

let IsValidPrefixOperatorDefinitionName s =
    if String.IsNullOrEmpty s then
        false
    else

        match s[0] with
        | '~' ->
            isTildeOnlyString s
            ||

            match s with
            | "~?+"
            | "~?-"
            | "~+"
            | "~-"
            | "~+."
            | "~-."
            | "~%"
            | "~%%"
            | "~&"
            | "~&&" -> true
            | _ -> false

        | '!' -> s <> "!="
        | _ -> false

let IsLogicalPrefixOperator logicalName =
    if String.IsNullOrEmpty logicalName then
        false
    else
        let displayName = ConvertValLogicalNameToDisplayNameCore logicalName
        displayName <> logicalName && IsValidPrefixOperatorDefinitionName displayName

let IsLogicalTernaryOperator logicalName =
    let displayName = ConvertValLogicalNameToDisplayNameCore logicalName
    displayName <> logicalName && (displayName = qmarkSet)

let IsPunctuation s =
    if String.IsNullOrEmpty s then
        false
    else
        match s with
        | ","
        | ";"
        | "|"
        | ":"
        | "."
        | "*"
        | "("
        | ")"
        | "["
        | "]"
        | "{"
        | "}"
        | "<"
        | ">"
        | "[|"
        | "|]"
        | "[<"
        | ">]" -> true
        | _ -> false

/// EQUALS, INFIX_COMPARE_OP, LESS, GREATER
let relational = [| "="; "!="; "<"; ">"; "$" |]

/// INFIX_AT_HAT_OP
let concat = [| "@"; "^" |]

/// PLUS_MINUS_OP, MINUS
let plusMinus = [| "+"; "-" |]

/// PERCENT_OP, STAR, INFIX_STAR_DIV_MOD_OP
let otherMath = [| "*"; "/"; "%" |]

/// Characters ignored at the start of the operator name
/// when determining whether an operator is an infix operator.
let ignoredChars = [| '.'; '?' |]

// Certain operator idents are parsed as infix expression operators.
// The parsing as infix operators is hardwired in the grammar [see declExpr productions]
// where certain operator tokens are accepted in infix forms, i.e. <expr> <op> <expr>.
// The lexer defines the strings that lead to those tokens.
//------
// This function recognises these "infix operator" names.
let IsLogicalInfixOpName logicalName =
    let s = ConvertValLogicalNameToDisplayNameCore logicalName
    let skipIgnoredChars = s.TrimStart(ignoredChars)

    let afterSkipStartsWith prefix =
        skipIgnoredChars.StartsWithOrdinal(prefix)

    let afterSkipStarts prefixes =
        Array.exists afterSkipStartsWith prefixes
    // The following conditions follow the declExpr infix clauses.
    // The test corresponds to the lexer definition for the token.
    s <> logicalName
    && ((s = // COLON_EQUALS
        ":=")
        ||
        // BAR_BAR, INFIX_BAR_OP
        afterSkipStartsWith "|"
        ||
        // AMP, AMP_AMP, INFIX_AMP_OP
        afterSkipStartsWith "&"
        ||
        // EQUALS, INFIX_COMPARE_OP, LESS, GREATER
        afterSkipStarts relational
        ||
        // DOLLAR
        (s = "$")
        ||
        // INFIX_AT_HAT_OP
        afterSkipStarts concat
        ||
        // COLON_COLON
        (s = "::")
        ||
        (s = qmark)
        ||
        (s = qmarkSet)
        ||
        (s = parenGet)
        ||
        (s = parenSet)
        ||
        // PLUS_MINUS_OP, MINUS
        afterSkipStarts plusMinus
        ||
        // PERCENT_OP, STAR, INFIX_STAR_DIV_MOD_OP
        afterSkipStarts otherMath
        ||
        // INFIX_STAR_STAR_OP
        (s = "**"))

let IsLogicalOpName (logicalName: string) =
    IsLogicalPrefixOperator logicalName
    || IsLogicalInfixOpName logicalName
    || IsLogicalTernaryOperator logicalName

let (|Control|Equality|Relational|Indexer|FixedTypes|Other|) opName =
    match opName with
    | "&"
    | "or"
    | "&&"
    | "||" -> Control
    | "<>"
    | "=" -> Equality
    | "<"
    | ">"
    | "<="
    | ">=" -> Relational
    | "<<"
    | "<|"
    | "<||"
    | "<||"
    | "|>"
    | "||>"
    | "|||>"
    | ">>"
    | "^"
    | ":="
    | "@" -> FixedTypes
    | ".[]" -> Indexer
    | _ -> Other

[<Literal>]
let compilerGeneratedMarker = "@"

[<Literal>]
let compilerGeneratedMarkerChar = '@'

let IsCompilerGeneratedName (nm: string) =
    nm.IndexOf compilerGeneratedMarkerChar <> -1

let CompilerGeneratedName nm =
    if IsCompilerGeneratedName nm then
        nm
    else
        nm + compilerGeneratedMarker

let GetBasicNameOfPossibleCompilerGeneratedName (name: string) =
    match name.IndexOf(compilerGeneratedMarker, StringComparison.Ordinal) with
    | -1
    | 0 -> name
    | n -> name[0 .. n - 1]

let CompilerGeneratedNameSuffix (basicName: string) suffix =
    basicName + compilerGeneratedMarker + suffix

//-------------------------------------------------------------------------
// Handle mangled .NET generic type names
//-------------------------------------------------------------------------

[<Literal>]
let mangledGenericTypeNameSym = '`'

let TryDemangleGenericNameAndPos (n: string) =
    // check what comes after the symbol is a number
    let pos = n.LastIndexOf mangledGenericTypeNameSym

    if pos = -1 then
        ValueNone
    else
        let mutable res = pos < n.Length - 1
        let mutable i = pos + 1

        while res && i < n.Length do
            let char = n[i]

            if not (char >= '0' && char <= '9') then
                res <- false

            i <- i + 1

        if res then ValueSome pos else ValueNone

type NameArityPair = NameArityPair of string * int

let DemangleGenericTypeNameWithPos pos (mangledName: string) = mangledName.Substring(0, pos)

let DecodeGenericTypeNameWithPos pos (mangledName: string) =
    let res = DemangleGenericTypeNameWithPos pos mangledName
    let num = mangledName.Substring(pos + 1, mangledName.Length - pos - 1)
    NameArityPair(res, int32 num)

let DemangleGenericTypeName (mangledName: string) =
    match TryDemangleGenericNameAndPos mangledName with
    | ValueSome pos -> DemangleGenericTypeNameWithPos pos mangledName
    | _ -> mangledName

let DecodeGenericTypeName (mangledName: string) =
    match TryDemangleGenericNameAndPos mangledName with
    | ValueSome pos -> DecodeGenericTypeNameWithPos pos mangledName
    | _ -> NameArityPair(mangledName, 0)

let chopStringTo (s: string) (c: char) =
    match s.IndexOf c with
    | -1 -> s
    | idx ->
        let i = idx + 1
        s.Substring(i, s.Length - i)

/// Try to chop "get_" or "set_" from a string
let TryChopPropertyName (s: string) =
    // extract the logical name from any mangled name produced by MakeMemberDataAndMangledNameForMemberVal
    if s.Length <= 4 then
        None
    else if s.StartsWithOrdinal("get_") || s.StartsWithOrdinal("set_") then
        Some(s.Substring(4, s.Length - 4))
    else
        let s = chopStringTo s '.'

        if s.StartsWithOrdinal("get_") || s.StartsWithOrdinal("set_") then
            Some(s.Substring(4, s.Length - 4))
        else
            None

/// Try to chop "get_" or "set_" from a string.
/// If the string does not start with "get_" or "set_", this function raises an exception.
let ChopPropertyName s =
    match TryChopPropertyName s with
    | None -> failwithf "Invalid internal property name: '%s'" s
    | Some res -> res

let SplitNamesForILPath (s: string) : string list =
    if s.StartsWithOrdinal("``") && s.EndsWithOrdinal("``") && s.Length > 4 then
        [ s.Substring(2, s.Length - 4) ] // identifier is enclosed in `` .. ``, so it is only a single element (this is very approximate)
    else
        s.Split [| '.'; '`' |] |> Array.toList // '.' chops members / namespaces / modules; '`' chops generic parameters for .NET types

/// Return a string array delimited by the given separator.
/// Note that a quoted string is not going to be mangled into pieces.
let inline isNotQuotedQuotation (text: string) n = n > 0 && text[n - 1] <> '\\'

let splitAroundQuotation (text: string) (separator: char) =
    let length = text.Length
    let result = ResizeArray()
    let mutable insideQuotation = false
    let mutable start = 0

    for i = 0 to length - 1 do
        match text[i], insideQuotation with
        // split when seeing a separator
        | c, false when c = separator ->
            result.Add(text.Substring(start, i - start))
            insideQuotation <- false
            start <- i + 1
        | _, _ when i = length - 1 -> result.Add(text.Substring(start, i - start + 1))
        // keep reading if a separator is inside quotation
        | c, true when c = separator -> insideQuotation <- true
        // open or close quotation
        | '\"', _ when isNotQuotedQuotation text i -> insideQuotation <- not insideQuotation
        // keep reading
        | _ -> ()

    result.ToArray()

/// Return a string array delimited by the given separator up to the maximum number.
/// Note that a quoted string is not going to be mangled into pieces.
let splitAroundQuotationWithCount (text: string) (separator: char) (count: int) =
    if count <= 1 then
        [| text |]
    else
        let mangledText = splitAroundQuotation text separator

        match mangledText.Length > count with
        | true -> Array.append mangledText[0 .. (count - 2)] [| mangledText[(count - 1) ..] |> String.concat (Char.ToString separator) |]
        | false -> mangledText

[<Literal>]
let FSharpModuleSuffix = "Module"

[<Literal>]
let MangledGlobalName = "`global`"

let IllegalCharactersInTypeAndNamespaceNames =
    [| '.'; '+'; '$'; '&'; '['; ']'; '/'; '\\'; '*'; '\"'; '`' |]

type ActivePatternInfo =
    | APInfo of bool * (string * range) list * range

    member x.IsTotal = let (APInfo (p, _, _)) = x in p

    member x.ActiveTags = let (APInfo (_, tags, _)) = x in List.map fst tags

    member x.ActiveTagsWithRanges = let (APInfo (_, tags, _)) = x in tags

    member x.Range = let (APInfo (_, _, m)) = x in m

let ActivePatternInfoOfValName nm (m: range) =
    // Note: The approximate range calculations in this code assume the name is of the form "(|A|B|)" not "(|  A   |   B   |)"
    // The ranges are used for IDE refactoring support etc.  If names of the second type are used,
    // renaming may be inaccurate/buggy. However names of the first form are dominant in F# code.
    let rec loop (nm: string) (mp: range) =
        let n = nm.IndexOf '|'

        if n > 0 then
            let m1 =
                Range.mkRange mp.FileName mp.Start (Position.mkPos mp.StartLine (mp.StartColumn + n))

            let m2 =
                Range.mkRange mp.FileName (Position.mkPos mp.StartLine (mp.StartColumn + n + 1)) mp.End

            (nm[0 .. n - 1], m1) :: loop nm[n + 1 ..] m2
        else
            let m1 =
                Range.mkRange mp.FileName mp.Start (Position.mkPos mp.StartLine (mp.StartColumn + nm.Length))

            [ (nm, m1) ]

    let nm = ConvertValLogicalNameToDisplayNameCore nm

    if IsActivePatternName nm then
        // Skip the '|' at each end when recovering ranges
        let m0 =
            Range.mkRange m.FileName (Position.mkPos m.StartLine (m.StartColumn + 1)) (Position.mkPos m.EndLine (m.EndColumn - 1))

        let names = loop nm[1 .. nm.Length - 2] m0
        let resH, resT = List.frontAndBack names

        Some(
            if fst resT = "_" then
                APInfo(false, resH, m)
            else
                APInfo(true, names, m)
        )
    else
        None

let mangleStaticStringArg (nm: string, v: string) =
    nm + "=" + "\"" + v.Replace("\\", "\\\\").Replace("\"", "\\\"") + "\""

let tryDemangleStaticStringArg (mangledText: string) =
    match splitAroundQuotationWithCount mangledText '=' 2 with
    | [| nm; v |] ->
        if v.Length >= 2 then
            Some(nm, v[ 1 .. v.Length - 2 ].Replace("\\\\", "\\").Replace("\\\"", "\""))
        else
            Some(nm, v)
    | _ -> None

exception InvalidMangledStaticArg of string

/// Demangle the static parameters
let DemangleProvidedTypeName (typeLogicalName: string) =
    if typeLogicalName.Contains "," then
        let pieces = splitAroundQuotation typeLogicalName ','

        match pieces with
        | [| x; "" |] -> x, [||]
        | _ ->
            let argNamesAndValues = pieces[1..] |> Array.choose tryDemangleStaticStringArg

            if argNamesAndValues.Length = (pieces.Length - 1) then
                pieces[0], argNamesAndValues
            else
                typeLogicalName, [||]
    else
        typeLogicalName, [||]

/// Mangle the static parameters for a provided type or method
let MangleProvidedTypeName (typeLogicalName, nonDefaultArgs) =
    let nonDefaultArgsText =
        nonDefaultArgs |> Array.map mangleStaticStringArg |> String.concat ","

    if nonDefaultArgsText = "" then
        typeLogicalName
    else
        typeLogicalName + "," + nonDefaultArgsText

/// Mangle the static parameters for a provided type or method
let ComputeMangledNameWithoutDefaultArgValues (nm, staticArgs, defaultArgValues) =
    let nonDefaultArgs =
        (staticArgs, defaultArgValues)
        ||> Array.zip
        |> Array.choose (fun (staticArg, (defaultArgName, defaultArgValue)) ->
            let actualArgValue = string staticArg

            match defaultArgValue with
            | Some v when v = actualArgValue -> None
            | _ -> Some(defaultArgName, actualArgValue))

    MangleProvidedTypeName(nm, nonDefaultArgs)

let outArgCompilerGeneratedName = "outArg"

let ExtraWitnessMethodName nm = nm + "$W"

/// Reuses generated union case field name objects for common field numbers
let mkUnionCaseFieldName =
    let names = [| 1..10 |] |> Array.map (fun i -> "Item" + string i)

    fun nFields i ->
        match nFields with
        | 0
        | 1 -> "Item"
        | _ -> if i < 10 then names[i] else "Item" + string (i + 1)

/// Reuses generated exception field name objects for common field numbers
let mkExceptionFieldName =
    let names = [| 0..9 |] |> Array.map (fun i -> "Data" + string i)

    fun i -> if i < 10 then names[i] else "Data" + string i

/// The prefix of the names used for the fake namespace path added to all dynamic code entries in FSI.EXE
let FsiDynamicModulePrefix = "FSI_"

[<RequireQualifiedAccess>]
module FSharpLib =
    let Root = "Microsoft.FSharp"
    let RootPath = IL.splitNamespace Root
    let Core = Root + ".Core"
    let CorePath = IL.splitNamespace Core

[<RequireQualifiedAccess>]
module CustomOperations =
    [<Literal>]
    let Into = "into"

let unassignedTyparName = "?"

let FormatAndOtherOverloadsString remainingOverloads =
    FSComp.SR.typeInfoOtherOverloads (remainingOverloads)

let GetLongNameFromString x = SplitNamesForILPath x

//--------------------------------------------------------------------------
// Resource format for pickled data
//--------------------------------------------------------------------------

let FSharpOptimizationDataResourceName = "FSharpOptimizationData."

let FSharpSignatureDataResourceName = "FSharpSignatureData."

// For historical reasons, we use a different resource name for FSharp.Core, so older F# compilers
// don't complain when they see the resource. The prefix of these names must not be 'FSharpOptimizationData'
// or 'FSharpSignatureData'
let FSharpOptimizationDataResourceName2 = "FSharpOptimizationInfo."

let FSharpSignatureDataResourceName2 = "FSharpSignatureInfo."

[<Literal>]
let suffixForVariablesThatMayNotBeEliminated = "$cont"

[<Literal>]
let suffixForTupleElementAssignmentTarget = "$tupleElem"

[<Literal>]
let stackVarPrefix = "__stack_"
