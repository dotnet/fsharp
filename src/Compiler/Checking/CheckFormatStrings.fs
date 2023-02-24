// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module internal FSharp.Compiler.CheckFormatStrings

open System.Text
open Internal.Utilities.Library
open Internal.Utilities.Library.Extras
open FSharp.Compiler.ConstraintSolver
open FSharp.Compiler.DiagnosticsLogger
open FSharp.Compiler.NameResolution
open FSharp.Compiler.Syntax
open FSharp.Compiler.SyntaxTreeOps
open FSharp.Compiler.Text
open FSharp.Compiler.TypedTree
open FSharp.Compiler.TypedTreeOps
open FSharp.Compiler.TcGlobals

type FormatItem = Simple of TType | FuncAndVal

let copyAndFixupFormatTypar g m tp =
    let _,_,tinst = FreshenAndFixupTypars g m TyparRigidity.Flexible [] [] [tp]
    List.head tinst

let lowestDefaultPriority = 0 (* See comment on TyparConstraint.DefaultsTo *)

let mkFlexibleFormatTypar g m tys dfltTy =
    let tp = Construct.NewTypar (TyparKind.Type, TyparRigidity.Rigid, SynTypar(mkSynId m "fmt",TyparStaticReq.HeadType,true),false,TyparDynamicReq.Yes,[],false,false)
    tp.SetConstraints [ TyparConstraint.SimpleChoice (tys,m); TyparConstraint.DefaultsTo (lowestDefaultPriority,dfltTy,m)]
    copyAndFixupFormatTypar g m tp

let mkFlexibleIntFormatTypar (g: TcGlobals) m =
    mkFlexibleFormatTypar g m [ g.byte_ty; g.int16_ty; g.int32_ty; g.int64_ty;  g.sbyte_ty; g.uint16_ty; g.uint32_ty; g.uint64_ty;g.nativeint_ty;g.unativeint_ty; ] g.int_ty

let mkFlexibleDecimalFormatTypar (g: TcGlobals) m =
    mkFlexibleFormatTypar g m [ g.decimal_ty ] g.decimal_ty

let mkFlexibleFloatFormatTypar (g: TcGlobals) m =
    mkFlexibleFormatTypar g m [ g.float_ty; g.float32_ty; g.decimal_ty ] g.float_ty

type FormatInfoRegister =
  { mutable leftJustify    : bool
    mutable numPrefixIfPos : char option
    mutable addZeros       : bool
    mutable precision      : bool}

let newInfo () =
  { leftJustify    = false
    numPrefixIfPos = None
    addZeros       = false
    precision      = false}

let escapeDotnetFormatString str =
    str
    // We need to double '{' and '}', because even if they were escaped in the
    // original string, extra curly braces were stripped away by the F# lexer.
    |> Seq.collect (fun x -> if x = '{' || x = '}' then [x;x] else [x])
    |> System.String.Concat

let makeFmts (context: FormatStringCheckContext) (fragRanges: range list) (fmt: string) =
    // Splits the string on interpolation holes based on fragment ranges.
    // Returns a list of tuples in the form of: offset * fragment as a string * original range of the fragment
    // where "offset" is the offset between beginning of the original range and where the string content begins

    let numFrags = fragRanges.Length
    let sourceText = context.SourceText
    let lineStartPositions = context.LineStartPositions

    let (|PrefixedBy|_|) (prefix: string) (str: string) =
        if str.StartsWith prefix then
            Some prefix.Length
        else
            None

    let mutable nQuotes = 1
    [ for i, r in List.indexed fragRanges do
        if r.StartLine - 1 < lineStartPositions.Length && r.EndLine - 1 < lineStartPositions.Length then
            let startIndex = lineStartPositions[r.StartLine - 1] + r.StartColumn
            let rLength = lineStartPositions[r.EndLine - 1] + r.EndColumn - startIndex
            let offset =
                if i = 0 then
                    match sourceText.GetSubTextString(startIndex, rLength) with
                    | PrefixedBy "$\"\"\"" len
                    | PrefixedBy "\"\"\"" len ->
                        nQuotes <- 3
                        len
                    | PrefixedBy "$@\"" len
                    | PrefixedBy "@$\"" len
                    | PrefixedBy "$\"" len
                    | PrefixedBy "@\"" len -> len
                    | _ -> 1
                else
                    1 // <- corresponds to '}' that's closing an interpolation hole
            let fragLen = rLength - offset - (if i = numFrags - 1 then nQuotes else 1)
            (offset, sourceText.GetSubTextString(startIndex + offset, fragLen), r)
        else (1, fmt, r)
    ]


module internal Parsing =

    let flags (info: FormatInfoRegister) (fmt: string) (fmtPos: int) =
        let len = fmt.Length
        let rec go pos =
            if pos >= len then failwith (FSComp.SR.forMissingFormatSpecifier())
            match fmt[pos] with
            | '-' ->
                if info.leftJustify then failwith (FSComp.SR.forFlagSetTwice("-"))
                info.leftJustify <- true
                go (pos+1)
            | '+' ->
                if info.numPrefixIfPos <> None then failwith (FSComp.SR.forPrefixFlagSpacePlusSetTwice())
                info.numPrefixIfPos <- Some '+'
                go (pos+1)
            | '0' ->
                if info.addZeros then failwith (FSComp.SR.forFlagSetTwice("0"))
                info.addZeros <- true
                go (pos+1)
            | ' ' ->
                if info.numPrefixIfPos <> None then failwith (FSComp.SR.forPrefixFlagSpacePlusSetTwice())
                info.numPrefixIfPos <- Some ' '
                go (pos+1)
            | '#' -> failwith (FSComp.SR.forHashSpecifierIsInvalid())
            | _ -> pos
        go fmtPos

    let rec digitsPrecision (fmt: string) (fmtPos: int) =
        if fmtPos >= fmt.Length then failwith (FSComp.SR.forBadPrecision())
        match fmt[fmtPos] with
        | c when System.Char.IsDigit c -> digitsPrecision fmt (fmtPos+1)
        | _ -> fmtPos

    let precision (info: FormatInfoRegister) (fmt: string) (fmtPos: int) =
        if fmtPos >= fmt.Length then failwith (FSComp.SR.forBadWidth())
        match fmt[fmtPos] with
        | c when System.Char.IsDigit c -> info.precision <- true; false,digitsPrecision fmt (fmtPos+1)
        | '*' -> info.precision <- true; true,(fmtPos+1)
        | _ -> failwith (FSComp.SR.forPrecisionMissingAfterDot())

    let optionalDotAndPrecision (info: FormatInfoRegister) (fmt: string) (fmtPos: int) =
        if fmtPos >= fmt.Length then failwith (FSComp.SR.forBadPrecision())
        match fmt[fmtPos] with
        | '.' -> precision info fmt (fmtPos+1)
        | _ -> false,fmtPos

    let rec digitsWidthAndPrecision (info: FormatInfoRegister) (fmt: string) (fmtPos: int) (intAcc: int) =
        let len = fmt.Length
        let rec go pos n =
            if pos >= len then failwith (FSComp.SR.forBadPrecision())
            match fmt[pos] with
            | c when System.Char.IsDigit c -> go (pos+1) (n*10 + int c - int '0')
            | _ -> Some n, optionalDotAndPrecision info fmt pos
        go fmtPos intAcc

    let widthAndPrecision (info: FormatInfoRegister) (fmt: string) (fmtPos: int) =
        if fmtPos >= fmt.Length then failwith (FSComp.SR.forBadPrecision())
        match fmt[fmtPos] with
        | c when System.Char.IsDigit c -> false,digitsWidthAndPrecision info fmt fmtPos 0
        | '*' -> true, (None, optionalDotAndPrecision info fmt (fmtPos+1))
        | _ -> false, (None, optionalDotAndPrecision info fmt fmtPos)

    let position (fmt: string) (fmtPos: int) =
        let len = fmt.Length

        let rec digitsPosition n pos =
            if pos >= len then failwith (FSComp.SR.forBadPrecision())
            match fmt[pos] with
            | c when System.Char.IsDigit c -> digitsPosition (n*10 + int c - int '0') (pos+1)
            | '$' -> Some n, pos+1
            | _ -> None, pos

        match fmt[fmtPos] with
        | c when c >= '1' && c <= '9' ->
            let p, pos' = digitsPosition (int c - int '0') (fmtPos+1)
            if p = None then None, fmtPos else p, pos'
        | _ -> None, fmtPos

    // Explicitly typed holes in interpolated strings "....%d{x}..." get additional '%P()' as a hole place marker
    let skipPossibleInterpolationHole isInterpolated isFormattableString (fmt: string) i =
        let len = fmt.Length
        if isInterpolated then
            if i+1 < len && fmt[i] = '%' && fmt[i+1] = 'P'  then
                let i = i + 2
                if i+1 < len && fmt[i] = '('  && fmt[i+1] = ')' then
                    if isFormattableString then
                        failwith (FSComp.SR.forFormatInvalidForInterpolated4())
                    i + 2
                else
                    failwith (FSComp.SR.forFormatInvalidForInterpolated2())
            else
                failwith (FSComp.SR.forFormatInvalidForInterpolated())
        else i

let parseFormatStringInternal
        (m: range)
        (fragRanges: range list)
        (g: TcGlobals)
        isInterpolated
        isFormattableString
        (context: FormatStringCheckContext option)
        (fmt: string)
        printerArgTy
        printerResidueTy =

    // As background: the F# compiler tokenizes strings on the assumption that the only thing you need from
    // them is the actual corresponding text, e.g. of a string literal.  This means many different textual input strings
    // in the input file correspond to the 'fmt' string we have here.
    //
    // The problem with this is that when we go to colorize the format specifiers in string, we need to do
    // that with respect to the original string source text in order to lay down accurate colorizations.
    //
    // One approach would be to change the F# lexer to also crack every string in a more structured way, recording
    // both the original source text and the actual string literal.  However this would be invasive and possibly
    // too expensive since the vast majority of strings don't need this treatment.
    //
    // So instead, for format strings alone - and only when processing in the IDE - we crack the "original"
    // source of the string by going back and getting the format string from the input source file by using the
    // relevant ranges
    //
    // For interpolated strings this may involve many fragments, e.g.
    //     $"abc %d{"
    //     "} def %s{"
    //     "} xyz"
    // In this case we are given the range of each fragment. One annoying thing is that we must lop off the
    // quotations, $, {, } symbols off the end of each string fragment. This information should probably
    // be given to us by the lexer.
    //
    // Note this also means that when compiling (command-line or background IncrementalBuilder in the IDE
    // there are no accurate intra-string ranges available for exact error message locations within the string.
    // The 'm' range passed as an input is however accurate and covers the whole string.
    //
    let escapeFormatStringEnabled = g.langVersion.SupportsFeature Features.LanguageFeature.EscapeDotnetFormattableStrings

    let fmt, fragments =
        match context with
        | Some context when fragRanges.Length > 0 ->
            let fmts = makeFmts context fragRanges fmt

            // Join the fragments with holes. Note this join is only used on the IDE path,
            // the CheckExpressions.fs does its own joining with the right alignments etc. substituted
            // On the IDE path we don't do any checking of these in this file (some checking is
            // done in CheckExpressions.fs) so it's ok to join with just '%P()'.
            let fmt = fmts |> List.map p23 |> String.concat "%P()"
            let fragments, _ =
                (0, fmts) ||> List.mapFold (fun i (offset, fmt, fragRange) ->
                    (i, offset, fragRange), i + fmt.Length + 4) // the '4' is the length of '%P()' joins

            fmt, fragments
        | _ ->
            // Don't muck with the fmt when there is no source code context to go get the original
            // source code (i.e. when compiling or background checking)
            (if escapeFormatStringEnabled then escapeDotnetFormatString fmt else fmt), [ (0, 1, m) ]

    let len = fmt.Length

    let specifierLocations = ResizeArray()

    // For FormattableString we collect a .NET Format String with {0} etc. replacing text.  '%%' are replaced
    // by '%', we check there are no '%' formats, and '{{' and '}}' are *not* replaced since the subsequent
    // call to String.Format etc. will process them.
    let dotnetFormatString = StringBuilder()
    let appendToDotnetFormatString (s: string) = dotnetFormatString.Append(s) |> ignore
    let mutable dotnetFormatStringInterpolationHoleCount = 0
    let percentATys = ResizeArray<_>()

    // fragLine, fragCol - track our location w.r.t. the marker for the start of this chunk
    //
    let rec parseLoop acc (i, fragLine, fragCol) fragments =

       // Check if we've moved into the next fragment.  Note this will always activate on
       // the first step, i.e. when i=0
       let struct (fragLine, fragCol, fragments) =
           match fragments with
           | (idx, fragOffset, fragRange: range)::rest when i >= idx  ->
               struct (fragRange.StartLine, fragRange.StartColumn + fragOffset, rest)

           | _ -> struct (fragLine, fragCol, fragments)

       if i >= len then
           let argTys =
               if acc |> List.forall (fun (p, _) -> p = None) then // without positional specifiers
                   acc |> List.map snd |> List.rev
               else
                   failwith (FSComp.SR.forPositionalSpecifiersNotPermitted())
           argTys
       elif System.Char.IsSurrogatePair(fmt,i) then
          appendToDotnetFormatString fmt[i..i+1]
          parseLoop acc (i+2, fragLine, fragCol+2) fragments
       else
          let c = fmt[i]
          match c with
          | '%' -> parseSpecifier acc (i, fragLine, fragCol) fragments
          | '\n' ->
              appendToDotnetFormatString fmt[i..i]
              parseLoop acc (i+1, fragLine+1, 0) fragments
          | _ ->
              appendToDotnetFormatString fmt[i..i]
              parseLoop acc (i+1, fragLine, fragCol+1) fragments

    and parseSpecifier acc (i, fragLine, fragCol) fragments =
        let startFragCol = fragCol
        let fragCol = fragCol+1
        if fmt[i..(i+1)] = "%%" then
            match context with
            | Some _ ->
                specifierLocations.Add(
                    (Range.mkFileIndexRange m.FileIndex
                        (Position.mkPos fragLine startFragCol)
                        (Position.mkPos fragLine (fragCol + 1))), 0)
            | None -> ()
            appendToDotnetFormatString "%"
            parseLoop acc (i+2, fragLine, fragCol+1) fragments
        else
            let i = i+1
            if i >= len then failwith (FSComp.SR.forMissingFormatSpecifier())
            let info = newInfo()

            let oldI = i
            let posi, i = Parsing.position fmt i
            let fragCol = fragCol + i - oldI

            let oldI = i
            let i = Parsing.flags info fmt i
            let fragCol = fragCol + i - oldI

            let oldI = i
            let widthArg,(widthValue, (precisionArg,i)) = Parsing.widthAndPrecision info fmt i
            let fragCol = fragCol + i - oldI

            if i >= len then failwith (FSComp.SR.forBadPrecision())

            let acc = if precisionArg then (Option.map ((+)1) posi, g.int_ty) :: acc else acc

            let acc = if widthArg then (Option.map ((+)1) posi, g.int_ty) :: acc else acc

            let checkOtherFlags c =
                if info.precision then failwith (FSComp.SR.forFormatDoesntSupportPrecision(c.ToString()))
                if info.addZeros then failwith (FSComp.SR.forDoesNotSupportZeroFlag(c.ToString()))
                match info.numPrefixIfPos with
                | Some n -> failwith (FSComp.SR.forDoesNotSupportPrefixFlag(c.ToString(), n.ToString()))
                | None -> ()

            let skipPossibleInterpolationHole pos = Parsing.skipPossibleInterpolationHole isInterpolated isFormattableString fmt pos

            // Implicitly typed holes in interpolated strings are translated to '... %P(...)...' in the
            // type checker.  They should always have '(...)' after for format string.
            let requireAndSkipInterpolationHoleFormat i =
                if i < len && fmt[i] = '(' then
                    let i2 = fmt.IndexOf(")", i+1)
                    if i2 = -1 then
                        failwith (FSComp.SR.forFormatInvalidForInterpolated3())
                    else
                        let dotnetAlignment = match widthValue with None -> "" | Some w -> "," + (if info.leftJustify then "-" else "") + string w
                        let dotnetNumberFormat = match fmt[i+1..i2-1] with "" -> "" | s -> ":" + s
                        appendToDotnetFormatString ("{" + string dotnetFormatStringInterpolationHoleCount + dotnetAlignment  + dotnetNumberFormat + "}")
                        dotnetFormatStringInterpolationHoleCount <- dotnetFormatStringInterpolationHoleCount + 1
                        i2+1
                else
                    failwith (FSComp.SR.forFormatInvalidForInterpolated3())

            let collectSpecifierLocation fragLine fragCol numStdArgs =
                match context with
                | Some _ ->
                    let numArgsForSpecifier =
                        numStdArgs + (if widthArg then 1 else 0) + (if precisionArg then 1 else 0)
                    specifierLocations.Add(
                        (Range.mkFileIndexRange m.FileIndex
                            (Position.mkPos fragLine startFragCol)
                            (Position.mkPos fragLine (fragCol + 1))), numArgsForSpecifier)
                | None -> ()

            let ch = fmt[i]
            match ch with
            | 'd' | 'i' | 'u' | 'B' | 'o' | 'x' | 'X' ->
                if ch = 'B' then DiagnosticsLogger.checkLanguageFeatureError g.langVersion Features.LanguageFeature.PrintfBinaryFormat m
                if info.precision then failwith (FSComp.SR.forFormatDoesntSupportPrecision(ch.ToString()))
                collectSpecifierLocation fragLine fragCol 1
                let i = skipPossibleInterpolationHole (i+1)
                parseLoop ((posi, mkFlexibleIntFormatTypar g m) :: acc) (i, fragLine, fragCol+1) fragments

            | 'l' | 'L' ->
                if info.precision then failwith (FSComp.SR.forFormatDoesntSupportPrecision(ch.ToString()))
                let fragCol = fragCol+1
                let i = i+1

                // "bad format specifier ... In F# code you can use %d, %x, %o or %u instead ..."
                if i >= len then
                    failwith (FSComp.SR.forBadFormatSpecifier())
                // Always error for %l and %Lx
                failwith (FSComp.SR.forLIsUnnecessary())
                match fmt[i] with
                | 'd' | 'i' | 'o' | 'u' | 'x' | 'X' ->
                    collectSpecifierLocation fragLine fragCol 1
                    let i = skipPossibleInterpolationHole (i+1)
                    parseLoop ((posi, mkFlexibleIntFormatTypar g m) :: acc)  (i, fragLine, fragCol+1) fragments
                | _ -> failwith (FSComp.SR.forBadFormatSpecifier())

            | 'h' | 'H' ->
                failwith (FSComp.SR.forHIsUnnecessary())

            | 'M' ->
                collectSpecifierLocation fragLine fragCol 1
                let i = skipPossibleInterpolationHole (i+1)
                parseLoop ((posi, mkFlexibleDecimalFormatTypar g m) :: acc) (i, fragLine, fragCol+1) fragments

            | 'f' | 'F' | 'e' | 'E' | 'g' | 'G' ->
                collectSpecifierLocation fragLine fragCol 1
                let i = skipPossibleInterpolationHole (i+1)
                parseLoop ((posi, mkFlexibleFloatFormatTypar g m) :: acc) (i, fragLine, fragCol+1) fragments

            | 'b' ->
                checkOtherFlags ch
                collectSpecifierLocation fragLine fragCol 1
                let i = skipPossibleInterpolationHole (i+1)
                parseLoop ((posi, g.bool_ty)  :: acc) (i, fragLine, fragCol+1) fragments

            | 'c' ->
                checkOtherFlags ch
                collectSpecifierLocation fragLine fragCol 1
                let i = skipPossibleInterpolationHole (i+1)
                parseLoop ((posi, g.char_ty)  :: acc) (i, fragLine, fragCol+1) fragments

            | 's' ->
                checkOtherFlags ch
                collectSpecifierLocation fragLine fragCol 1
                let i = skipPossibleInterpolationHole (i+1)
                parseLoop ((posi, g.string_ty)  :: acc) (i, fragLine, fragCol+1) fragments

            | 'O' ->
                checkOtherFlags ch
                collectSpecifierLocation fragLine fragCol 1
                let i = skipPossibleInterpolationHole (i+1)
                parseLoop ((posi, NewInferenceType g) :: acc) (i, fragLine, fragCol+1) fragments

            // residue of hole "...{n}..." in interpolated strings become %P(...)
            | 'P' when isInterpolated ->
                checkOtherFlags ch
                let i = requireAndSkipInterpolationHoleFormat (i+1)
                // Note, the fragCol doesn't advance at all as these are magically inserted.
                parseLoop ((posi, NewInferenceType g) :: acc) (i, fragLine, startFragCol) fragments

            | 'A' ->
                if g.useReflectionFreeCodeGen then
                    failwith (FSComp.SR.forPercentAInReflectionFreeCode())

                match info.numPrefixIfPos with
                | None     // %A has BindingFlags=Public, %+A has BindingFlags=Public | NonPublic
                | Some '+' ->
                    collectSpecifierLocation fragLine fragCol 1
                    let i = skipPossibleInterpolationHole (i+1)
                    let aTy = NewInferenceType g
                    percentATys.Add(aTy)
                    parseLoop ((posi, aTy) :: acc)  (i, fragLine, fragCol+1) fragments
                | Some n ->
                    failwith (FSComp.SR.forDoesNotSupportPrefixFlag(ch.ToString(), n.ToString()))

            | 'a' ->
                checkOtherFlags ch
                let aTy = NewInferenceType g
                let fTy = mkFunTy g printerArgTy (mkFunTy g aTy printerResidueTy)
                collectSpecifierLocation fragLine fragCol 2
                let i = skipPossibleInterpolationHole (i+1)
                parseLoop ((Option.map ((+)1) posi, aTy) ::  (posi, fTy) :: acc) (i, fragLine, fragCol+1) fragments

            | 't' ->
                checkOtherFlags ch
                collectSpecifierLocation fragLine fragCol 1
                let i = skipPossibleInterpolationHole (i+1)
                parseLoop ((posi, mkFunTy g printerArgTy printerResidueTy) :: acc)  (i, fragLine, fragCol+1) fragments

            | '%' ->
                // This allows for things like `printf "%-4.2%"` to compile and print just a `%`
                // For now we are adding a warning, but keeping this behavior.
                warning(DiagnosticWithText(3376, FSComp.SR.forBadFormatSpecifierGeneral("%"), m))
                collectSpecifierLocation fragLine fragCol 0
                appendToDotnetFormatString "%"
                parseLoop acc (i+1, fragLine, fragCol+1) fragments
            | c -> failwith (FSComp.SR.forBadFormatSpecifierGeneral(String.make 1 c))

    let results = parseLoop [] (0, 0, m.StartColumn) fragments
    results, Seq.toList specifierLocations, dotnetFormatString.ToString(), percentATys.ToArray()

let ParseFormatString m fragmentRanges g isInterpolated isFormattableString formatStringCheckContext fmt printerArgTy printerResidueTy printerResultTy =
    let argTys, specifierLocations, dotnetFormatString, percentATys =
        parseFormatStringInternal m fragmentRanges g isInterpolated isFormattableString formatStringCheckContext fmt printerArgTy printerResidueTy
    let printerTy = List.foldBack (mkFunTy g) argTys printerResultTy
    let printerTupleTy = mkRefTupledTy g argTys
    argTys, printerTy, printerTupleTy, percentATys, specifierLocations, dotnetFormatString

let TryCountFormatStringArguments m g isInterpolated fmt printerArgTy printerResidueTy =
    try
        let argTys, _specifierLocations, _dotnetFormatString, _percentATys = parseFormatStringInternal m [] g isInterpolated false None fmt printerArgTy printerResidueTy
        Some argTys.Length
    with _ ->
        None

