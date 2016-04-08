// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

//----------------------------------------------------------------------------
// Some general F# utilities for mangling / unmangling / manipulating names.
//--------------------------------------------------------------------------

/// Anything to do with special names of identifiers and other lexical rules 
module internal Microsoft.FSharp.Compiler.PrettyNaming
    open Internal.Utilities
    open Microsoft.FSharp.Compiler
    open Microsoft.FSharp.Compiler.AbstractIL.Internal
    open Microsoft.FSharp.Compiler.AbstractIL.Internal.Library
    open System.Globalization
    open System.Collections.Generic
    open System.Collections.Concurrent

#if FX_RESHAPED_REFLECTION
    open Microsoft.FSharp.Core.ReflectionAdapters
#endif

    //------------------------------------------------------------------------
    // Operator name compilation
    //-----------------------------------------------------------------------

    let [<Literal>] parenGet = ".()"
    let [<Literal>] parenSet = ".()<-"
    let [<Literal>] qmark = "?"
    let [<Literal>] qmarkSet = "?<-"

    /// Prefix for compiled (mangled) operator names.
    let [<Literal>] opNamePrefix = "op_"

    let private opNameTable = 
     [|("[]", "op_Nil");
       ("::", "op_ColonColon");
       ("+", "op_Addition");
       ("~%", "op_Splice");
       ("~%%", "op_SpliceUntyped");
       ("~++", "op_Increment");
       ("~--", "op_Decrement");
       ("-", "op_Subtraction");
       ("*", "op_Multiply");
       ("**", "op_Exponentiation");
       ("/", "op_Division");
       ("@", "op_Append");
       ("^", "op_Concatenate");
       ("%", "op_Modulus");
       ("&&&", "op_BitwiseAnd");
       ("|||", "op_BitwiseOr");
       ("^^^", "op_ExclusiveOr");
       ("<<<", "op_LeftShift");
       ("~~~", "op_LogicalNot");
       (">>>", "op_RightShift");
       ("~+", "op_UnaryPlus");
       ("~-", "op_UnaryNegation");
       ("~&", "op_AddressOf");
       ("~&&", "op_IntegerAddressOf");
       ("&&", "op_BooleanAnd");
       ("||", "op_BooleanOr");
       ("<=", "op_LessThanOrEqual");
       ("=","op_Equality");
       ("<>","op_Inequality");
       (">=", "op_GreaterThanOrEqual");
       ("<", "op_LessThan");
       (">", "op_GreaterThan");
       ("|>", "op_PipeRight");
       ("||>", "op_PipeRight2");
       ("|||>", "op_PipeRight3");
       ("<|", "op_PipeLeft");
       ("<||", "op_PipeLeft2");
       ("<|||", "op_PipeLeft3");
       ("!", "op_Dereference");
       (">>", "op_ComposeRight");
       ("<<", "op_ComposeLeft");
       ("<< >>", "op_TypedQuotationUnicode");
       ("<<| |>>", "op_ChevronsBar");
       ("<@ @>", "op_Quotation");
       ("<@@ @@>", "op_QuotationUntyped");
       ("+=", "op_AdditionAssignment");
       ("-=", "op_SubtractionAssignment");
       ("*=", "op_MultiplyAssignment");
       ("/=", "op_DivisionAssignment");
       ("..", "op_Range");
       (".. ..", "op_RangeStep"); 
       (qmark, "op_Dynamic");
       (qmarkSet, "op_DynamicAssignment");
       (parenGet, "op_ArrayLookup");
       (parenSet, "op_ArrayAssign");
       |]

    let private opCharTranslateTable =
      [|( '>', "Greater");
        ( '<', "Less"); 
        ( '+', "Plus");
        ( '-', "Minus");
        ( '*', "Multiply");
        ( '=', "Equals");
        ( '~', "Twiddle");
        ( '%', "Percent");
        ( '.', "Dot");
        ( '$', "Dollar");
        ( '&', "Amp");
        ( '|', "Bar");
        ( '@', "At");
        ( '#', "Hash");
        ( '^', "Hat");
        ( '!', "Bang");
        ( '?', "Qmark");
        ( '/', "Divide");
        ( ':', "Colon");
        ( '(', "LParen");
        ( ',', "Comma");
        ( ')', "RParen");
        ( ' ', "Space");
        ( '[', "LBrack");
        ( ']', "RBrack"); |]

    /// The set of characters usable in custom operators.
    let private opCharSet =
        let t = new HashSet<_>()
        for (c,_) in opCharTranslateTable do
            t.Add(c) |> ignore
        t
        
    let IsOpName (name:string) =
        let nameLen = name.Length
        let rec loop i = (i < nameLen && (opCharSet.Contains(name.[i]) || loop (i+1)))
        loop 0

    let IsMangledOpName (n:string) =
        n.StartsWith (opNamePrefix, System.StringComparison.Ordinal)

    // +++ GLOBAL STATE
    /// Compiles a custom operator into a mangled operator name.
    /// For example, "!%" becomes "op_DereferencePercent".
    /// This function should only be used for custom operators;
    /// if an operator is or potentially may be a built-in operator,
    /// use the 'CompileOpName' function instead.
    let private compileCustomOpName =
        let t2 =
            let t2 = Dictionary<_,_> (opCharTranslateTable.Length)
            for x, y in opCharTranslateTable do
                t2.Add (x, y)
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
        let compiledOperators = ConcurrentDictionary<_,_> (System.StringComparer.Ordinal)

        fun op ->
            // Has this operator already been compiled?
            match compiledOperators.TryGetValue op with
            | true, opName -> opName
            | false, _ ->
                let opLength = op.Length
                let sb = new System.Text.StringBuilder (opNamePrefix, opNamePrefix.Length + (opLength * maxOperatorNameLength))
                for i = 0 to opLength - 1 do
                    let c = op.[i]
                    match t2.TryGetValue c with
                    | true, x ->
                        sb.Append(x) |> ignore
                    | false, _ ->
                        sb.Append(c) |> ignore

                /// The compiled (mangled) operator name.
                let opName = sb.ToString ()

                // Cache the compiled name so it can be reused.
                compiledOperators.TryAdd (op, opName) |> ignore
                opName

    // +++ GLOBAL STATE
    /// Compiles an operator into a mangled operator name.
    /// For example, "!%" becomes "op_DereferencePercent".
    /// This function accepts both built-in and custom operators.
    let CompileOpName =
        /// Maps the built-in F# operators to their mangled operator names.
        let standardOpNames =
            let opNames = Dictionary<_,_> (opNameTable.Length, System.StringComparer.Ordinal)
            for x, y in opNameTable do
                opNames.Add (x, y)
            opNames

        fun op ->
            match standardOpNames.TryGetValue op with
            | true, x -> x
            | false, _ ->
                if IsOpName op then
                    compileCustomOpName op
                else op

    // +++ GLOBAL STATE
    /// Decompiles the mangled name of a custom operator back into an operator.
    /// For example, "op_DereferencePercent" becomes "!%".
    /// This function should only be used for mangled names of custom operators;
    /// if a mangled name potentially represents a built-in operator,
    /// use the 'DecompileOpName' function instead.
    let private decompileCustomOpName =
        // Memoize this operation. Custom operators are typically used more than once
        // so this avoids repeating decompilation.
        let decompiledOperators = ConcurrentDictionary<_,_> (System.StringComparer.Ordinal)

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
                /// Returns None if the name contains text which doesn't correspond to an operator;
                /// otherwise returns Some containing the original operator. 
                let rec decompile (sb : System.Text.StringBuilder) idx =
                    // Have we reached the end of 'opName'?
                    if idx = opNameLen then
                        // Finished decompiling.
                        // Cache the decompiled operator before returning so it can be reused.
                        let decompiledOp = sb.ToString ()
                        decompiledOperators.TryAdd (opName, decompiledOp) |> ignore
                        decompiledOp
                    else
                        let choice =
                            opCharTranslateTable
                            |> Array.tryFind (fun (_, opCharName) ->
                                // If this operator character name is longer than the remaining piece of 'opName',
                                // it's obviously not a match.
                                let opCharNameLen = opCharName.Length
                                if opNameLen - idx < opCharNameLen then false
                                else
                                    // Does 'opCharName' match the current position in 'opName'?
                                    System.String.Compare (opName, idx, opCharName, 0, opCharNameLen, System.StringComparison.Ordinal) = 0)

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
                    System.Text.StringBuilder (maxPossibleOpCharCount)

                // Start decompiling just after the operator prefix.
                decompile sb opNamePrefixLen

    // +++ GLOBAL STATE
    /// Decompiles a mangled operator name back into an operator.
    /// For example, "op_DereferencePercent" becomes "!%".
    /// This function accepts mangled names for both built-in and custom operators.
    let DecompileOpName =
        /// Maps the mangled operator names of built-in F# operators back to the operators.
        let standardOps =
            let ops = Dictionary<string, string> (opNameTable.Length, System.StringComparer.Ordinal)
            for x, y in opNameTable do
                ops.Add(y,x)
            ops

        fun opName ->
            match standardOps.TryGetValue opName with
            | true, res -> res
            | false, _ ->
                if IsMangledOpName opName then
                    decompileCustomOpName opName
                else
                    opName

    let DemangleOperatorName nm =
        let nm = DecompileOpName nm
        if IsOpName nm then "( " + nm + " )"
        else nm
                  
    let opNameCons = CompileOpName "::"
    let opNameNil = CompileOpName "[]"
    let opNameEquals = CompileOpName "="
    let opNameEqualsNullable = CompileOpName "=?"
    let opNameNullableEquals = CompileOpName "?="
    let opNameNullableEqualsNullable = CompileOpName "?=?"

    /// The characters that are allowed to be the first character of an identifier.
    let IsIdentifierFirstCharacter c =
        if c = '_' then true
        else
            match System.Char.GetUnicodeCategory c with
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
        if c = '\'' then true   // Tick
        else
            match System.Char.GetUnicodeCategory c with
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

    /// Is this character a part of a long identifier?
    let IsLongIdentifierPartCharacter c =
        c = '.'
        || IsIdentifierPartCharacter c

    let IsValidPrefixOperatorUse s =
        if System.String.IsNullOrEmpty s then false else
        match s with 
        | "?+" | "?-" | "+" | "-" | "+." | "-." | "%" | "%%" | "&" | "&&" -> true
        | _ ->
            s.[0] = '!'
            // The check for the first character here could be eliminated since it's covered
            // by the call to String.forall; it is a fast check used to avoid the call if possible.
            || (s.[0] = '~' && String.forall (fun c -> c = '~') s)
    
    let IsValidPrefixOperatorDefinitionName s = 
        if System.String.IsNullOrEmpty s then false else
        match s with 
        | "~?+" | "~?-" | "~+" | "~-" | "~+." | "~-." | "~%" | "~%%" | "~&" | "~&&" -> true
        | _ ->
            (s.[0] = '!' && s <> "!=")
            // The check for the first character here could be eliminated since it's covered
            // by the call to String.forall; it is a fast check used to avoid the call if possible.
            || (s.[0] = '~' && String.forall (fun c -> c = '~') s)
        
    let IsPrefixOperator s =
        if System.String.IsNullOrEmpty s then false else
        let s = DecompileOpName s
        match s with 
        | "~?+" | "~?-" | "~+" | "~-" | "~+." | "~-." | "~%" | "~%%" | "~&" | "~&&" -> true
        | _ ->
            (s.[0] = '!' && s <> "!=")
            // The check for the first character here could be eliminated since it's covered
            // by the call to String.forall; it is a fast check used to avoid the call if possible.
            || (s.[0] = '~' && String.forall (fun c -> c = '~') s)

    let IsTernaryOperator s = 
        (DecompileOpName s = qmarkSet)

    let IsInfixOperator =
        /// EQUALS, INFIX_COMPARE_OP, LESS, GREATER
        let relational = [| "=";"!=";"<";">";"$"|]
        /// INFIX_AT_HAT_OP
        let concat = [| "@";"^" |]
        /// PLUS_MINUS_OP, MINUS
        let plusMinus = [| "+"; "-" |]
        /// PERCENT_OP, STAR, INFIX_STAR_DIV_MOD_OP
        let otherMath = [| "*";"/";"%" |]

        /// Characters ignored at the start of the operator name
        /// when determining whether an operator is an infix operator.
        let ignoredChars = [| '.'; '?' |]

        fun s (* where s is assumed to be a compiled name *) ->
        // Certain operator idents are parsed as infix expression operators.
        // The parsing as infix operators is hardwired in the grammar [see declExpr productions]
        // where certain operator tokens are accepted in infix forms, i.e. <expr> <op> <expr>.
        // The lexer defines the strings that lead to those tokens.
        //------
        // This function recognises these "infix operator" names.
        let s = DecompileOpName s
        let skipIgnoredChars = s.TrimStart(ignoredChars)
        let afterSkipStartsWith prefix   = skipIgnoredChars.StartsWith(prefix,System.StringComparison.Ordinal)
        let afterSkipStarts     prefixes = Array.exists afterSkipStartsWith prefixes
        // The following conditions follow the declExpr infix clauses.
        // The test corresponds to the lexer definition for the token.
        s = ":=" ||                                    // COLON_EQUALS
        afterSkipStartsWith "|" ||                     // BAR_BAR, INFIX_BAR_OP
        (* REVIEW: OR is deadcode, now called BAR? *)  // OR
        afterSkipStartsWith "&"  ||                    // AMP, AMP_AMP, INFIX_AMP_OP
        afterSkipStarts relational ||                  // EQUALS, INFIX_COMPARE_OP, LESS, GREATER
        s = "$" ||                                     // DOLLAR
        afterSkipStarts concat ||                      // INFIX_AT_HAT_OP
        s = "::" ||                                    // COLON_COLON
        afterSkipStarts plusMinus ||                   // PLUS_MINUS_OP, MINUS
        afterSkipStarts otherMath ||                   // PERCENT_OP, STAR, INFIX_STAR_DIV_MOD_OP
        s = "**"                                       // INFIX_STAR_STAR_OP

    let (|Control|Equality|Relational|Indexer|FixedTypes|Other|) opName =
        match opName with
        | "&" | "or" | "&&" | "||" ->
            Control
        | "<>" | "=" ->
            Equality
        | "<" | ">" | "<=" | ">=" ->
            Relational
        | "<<" | "<|" | "<||" | "<||" | "|>" | "||>" | "|||>" | ">>" | "^" | ":=" | "@" ->
            FixedTypes
        | ".[]" ->
            Indexer
        | _ ->
            Other

    let [<Literal>] private compilerGeneratedMarker = "@"
    let [<Literal>] private compilerGeneratedMarkerChar = '@'
    
    let IsCompilerGeneratedName (nm:string) =
        nm.IndexOf compilerGeneratedMarkerChar <> -1
        
    let CompilerGeneratedName nm =
        if IsCompilerGeneratedName nm then nm else nm+compilerGeneratedMarker

    let GetBasicNameOfPossibleCompilerGeneratedName (name:string) =
            match name.IndexOf compilerGeneratedMarker with 
            | -1 | 0 -> name
            | n -> name.[0..n-1]

    let CompilerGeneratedNameSuffix (basicName:string) suffix =
        basicName+compilerGeneratedMarker+suffix


    //-------------------------------------------------------------------------
    // Handle mangled .NET generic type names
    //------------------------------------------------------------------------- 
     
    let [<Literal>] private mangledGenericTypeNameSym = '`'
    let IsMangledGenericName (n:string) = 
        n.IndexOf mangledGenericTypeNameSym <> -1 &&
        (* check what comes after the symbol is a number *)
        let m = n.LastIndexOf mangledGenericTypeNameSym
        let mutable res = m < n.Length - 1
        for i = m + 1 to n.Length - 1 do
            res <- res && n.[i] >= '0' && n.[i] <= '9'
        res

    type NameArityPair = NameArityPair of string * int
    let DecodeGenericTypeName n = 
        if IsMangledGenericName n then 
            let pos = n.LastIndexOf mangledGenericTypeNameSym
            let res = n.Substring(0,pos)
            let num = n.Substring(pos+1,n.Length - pos - 1)
            NameArityPair(res, int32 num)
        else NameArityPair(n,0)

    let DemangleGenericTypeName n = 
        if  IsMangledGenericName n then 
            let pos = n.LastIndexOf mangledGenericTypeNameSym
            n.Substring(0,pos)
        else n

    //-------------------------------------------------------------------------
    // Property name mangling.
    // Expecting s to be in the form (as returned by qualifiedMangledNameOfTyconRef) of:
    //    get_P                         or  set_P
    //    Names/Space/Class/NLPath-get_P  or  Names/Space/Class/NLPath.set_P
    // Required to return "P"
    //-------------------------------------------------------------------------

    let private chopStringTo (s:string) (c:char) =
        (* chopStringTo "abcdef" 'c' --> "def" *)
        match s.IndexOf c with
        | -1 -> s
        | idx ->
            let i = idx + 1
            s.Substring(i, s.Length - i)

    /// Try to chop "get_" or "set_" from a string
    let TryChopPropertyName (s: string) =
        // extract the logical name from any mangled name produced by MakeMemberDataAndMangledNameForMemberVal
        if s.Length <= 4 then None else
        if s.StartsWith("get_", System.StringComparison.Ordinal) ||
           s.StartsWith("set_", System.StringComparison.Ordinal)
        then Some (s.Substring(4, s.Length - 4))
        else
        let s = chopStringTo s '.'
        if s.StartsWith("get_", System.StringComparison.Ordinal) ||
           s.StartsWith("set_", System.StringComparison.Ordinal)
        then Some (s.Substring(4, s.Length - 4))
        else None

    /// Try to chop "get_" or "set_" from a string.
    /// If the string does not start with "get_" or "set_", this function raises an exception.
    let ChopPropertyName s =
        match TryChopPropertyName s with 
        | None -> 
            failwithf "Invalid internal property name: '%s'" s
        | Some res -> res

    let SplitNamesForILPath (s : string) : string list = 
        if s.StartsWith("``",System.StringComparison.Ordinal) && s.EndsWith("``",System.StringComparison.Ordinal) && s.Length > 4 then [s.Substring(2, s.Length-4)] // identifier is enclosed in `` .. ``, so it is only a single element (this is very approximate)
        else s.Split [| '.' ; '`' |] |> Array.toList      // '.' chops members / namespaces / modules; '`' chops generic parameters for .NET types
        
    // Return a string array delimited by the given separator.
    // Note that a quoted string is not going to be mangled into pieces. 
    let private splitAroundQuotation (text:string) (separator:char) =
        let length = text.Length
        let isNotQuotedQuotation n = n > 0 && text.[n-1] <> '\\'
        let rec split (i, cur, group, insideQuotation) =        
            if i>=length then List.rev (cur::group) else
            match text.[i], insideQuotation with
            // split when seeing a separator
            | c, false when c = separator -> split (i+1, "", cur::group, false)
            // keep reading if a separator is inside quotation
            | c, true when c = separator -> split (i+1, cur+(System.Char.ToString c), group, true)
            // open or close quotation 
            | '\"', _ when isNotQuotedQuotation i -> split (i+1, cur+"\"", group, not insideQuotation) 
            // keep reading
            | c, _ -> split (i+1, cur+(System.Char.ToString c), group, insideQuotation)
        split (0, "", [], false) |> Array.ofList

    // Return a string array delimited by the given separator up to the maximum number.
    // Note that a quoted string is not going to be mangled into pieces.
    let private splitAroundQuotationWithCount (text:string) (separator:char) (count:int)=
        if count <= 1 then [| text |] else
        let mangledText  = splitAroundQuotation text separator
        match mangledText.Length > count with
        | true -> Array.append (mangledText.[0..(count-2)]) ([| mangledText.[(count-1)..] |> String.concat (System.Char.ToString separator) |])
        | false -> mangledText

    let [<Literal>] FSharpModuleSuffix = "Module"

    let [<Literal>] MangledGlobalName = "`global`"
    
    let IllegalCharactersInTypeAndNamespaceNames = [| '.'; '+'; '$'; '&'; '['; ']'; '/'; '\\'; '*'; '\"'; '`'  |]

    /// Determines if the specified name is a valid name for an active pattern.
    let IsActivePatternName (nm:string) =
        let nameLen = nm.Length
        // The name must start and end with '|'
        (nm.IndexOf '|' = 0) &&
        (nm.LastIndexOf '|' = nameLen - 1) &&
        // The name must contain at least one character between the starting and ending delimiters.
        nameLen >= 3 &&
        (
           let core = nm.Substring(1, nameLen - 2)
           // no operator characters except '|' and ' '
           core |> String.forall (fun c -> c = '|' || c = ' ' || not (opCharSet.Contains c)) &&
           // at least one non-operator or space character
           core |> String.exists (fun c -> c = ' ' || not (opCharSet.Contains c))
        )

    //IsActivePatternName "|+|" = false
    //IsActivePatternName "|ABC|" = true
    //IsActivePatternName "|ABC|DEF|" = true
    //IsActivePatternName "|||" = false
    //IsActivePatternName "||S|" = true

    type ActivePatternInfo = 
        | APInfo of bool * (string  * Range.range) list * Range.range
        member x.IsTotal = let (APInfo(p,_,_)) = x in p
        member x.ActiveTags = let (APInfo(_,tags,_)) = x in List.map fst tags
        member x.ActiveTagsWithRanges = let (APInfo(_,tags,_)) = x in tags
        member x.Range = let (APInfo(_,_,m)) = x in m

    let ActivePatternInfoOfValName nm (m:Range.range) = 
        // Note: The approximate range calculations in this code assume the name is of the form "(|A|B|)" not "(|  A   |   B   |)"
        // The ranges are used for IDE refactoring support etc.  If names of the second type are used,
        // renaming may be inaccurate/buggy. However names of the first form are dominant in F# code.
        let rec loop (nm:string) (mp:Range.range) = 
            let n = nm.IndexOf '|'
            if n > 0 then 
               let m1 = Range.mkRange mp.FileName mp.Start (Range.mkPos mp.StartLine (mp.StartColumn + n))
               let m2 = Range.mkRange mp.FileName (Range.mkPos mp.StartLine (mp.StartColumn + n + 1)) mp.End
               (nm.[0..n-1], m1) :: loop nm.[n+1..] m2
            else
               let m1 = Range.mkRange mp.FileName mp.Start (Range.mkPos mp.StartLine (mp.StartColumn + nm.Length))
               [(nm, m1)]
        let nm = DecompileOpName nm
        if IsActivePatternName nm then 
            // Skip the '|' at each end when recovering ranges
            let m0 = Range.mkRange m.FileName (Range.mkPos m.StartLine (m.StartColumn + 1)) (Range.mkPos m.EndLine (m.EndColumn - 1)) 
            let names = loop nm.[1..nm.Length-2] m0
            let resH,resT = List.frontAndBack names
            Some(if fst resT = "_" then APInfo(false,resH,m) else APInfo(true,names,m))
        else 
            None
    
    let private mangleStaticStringArg (nm:string,v:string) = 
        nm + "=" + "\"" + v.Replace("\\", "\\\\").Replace("\"", "\\\"") + "\""

    let private tryDemangleStaticStringArg (mangledText:string) =
        match splitAroundQuotationWithCount mangledText '=' 2 with
        | [| nm; v |] ->
            if v.Length >= 2 then
                Some(nm,v.[1..v.Length-2].Replace("\\\\","\\").Replace("\\\"","\""))
            else
                Some(nm,v)
        | _ -> None

    // Demangle the static parameters
    exception InvalidMangledStaticArg of string

    let demangleProvidedTypeName (typeLogicalName:string) = 
        if typeLogicalName.Contains "," then 
            let pieces = splitAroundQuotation typeLogicalName ','
            match pieces with
            | [| x; "" |] -> x, [| |]
            | _ ->
                let argNamesAndValues = pieces.[1..] |> Array.choose tryDemangleStaticStringArg
                if argNamesAndValues.Length = (pieces.Length - 1) then
                    pieces.[0], argNamesAndValues
                else
                    typeLogicalName, [| |]
        else 
            typeLogicalName, [| |]

    let mangleProvidedTypeName (typeLogicalName,nonDefaultArgs) = 
        let nonDefaultArgsText = 
            nonDefaultArgs
            |> Array.map mangleStaticStringArg
            |> String.concat ","

        if nonDefaultArgsText = "" then
            typeLogicalName
        else
            typeLogicalName + "," + nonDefaultArgsText


    let computeMangledNameWithoutDefaultArgValues(nm,staticArgs,defaultArgValues) =
        let nonDefaultArgs = 
            (staticArgs,defaultArgValues) 
            ||> Array.zip 
            |> Array.choose (fun (staticArg, (defaultArgName, defaultArgValue)) -> 
                let actualArgValue = string staticArg 
                match defaultArgValue with 
                | Some v when v = actualArgValue -> None
                | _ -> Some (defaultArgName, actualArgValue))
        mangleProvidedTypeName (nm, nonDefaultArgs)
