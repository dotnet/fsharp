// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

module internal Microsoft.VisualStudio.FSharp.Editor.CodePrinter

open System
open System.Collections.Generic
open Microsoft.FSharp.Compiler.Ast
open Microsoft.VisualStudio.FSharp.Editor.FormatConfig
open Microsoft.VisualStudio.FSharp.Editor.SourceParser
open Microsoft.VisualStudio.FSharp.Editor.SourceTransformer

/// This type consists of contextual information which is important for formatting
type ASTContext =
    {
      /// Original file name without extension of the parsed AST 
      TopLevelModuleName: string 
      /// Current node is the first child of its parent
      IsFirstChild: bool
      /// Current node is a subnode deep down in an interface
      IsInterface: bool 
      /// This pattern matters for formatting extern declarations
      IsCStylePattern: bool
      /// Range operators are naked in 'for..in..do' constructs
      IsNakedRange: bool
      /// The optional `|` in pattern matching and union type definitions
      HasVerticalBar: bool
      /// A field is rendered as union field or not
      IsUnionField: bool
      /// First type param might need extra spaces to avoid parsing errors on `<^`, `<'`, etc.
      IsFirstTypeParam: bool
      /// Check whether the context is inside DotGet to suppress whitespaces
      IsInsideDotGet: bool
    }
    static member Default =
        { TopLevelModuleName = "" 
          IsFirstChild = false; IsInterface = false 
          IsCStylePattern = false; IsNakedRange = false
          HasVerticalBar = false; IsUnionField = false
          IsFirstTypeParam = false; IsInsideDotGet = false }

let rec addSpaceBeforeParensInFunCall functionOrMethod arg = 
    match functionOrMethod, arg with
    | _, ConstExpr(Const "()", _) -> false
    | SynExpr.LongIdent(_, LongIdentWithDots s, _, _), _ ->
        let parts = s.Split '.'
        not <| Char.IsUpper parts.[parts.Length - 1].[0]
    | SynExpr.Ident(Ident s), _ -> not <| Char.IsUpper s.[0]
    | SynExpr.TypeApp(e, _, _, _, _, _, _), _ -> addSpaceBeforeParensInFunCall e arg
    | _ -> true

let addSpaceBeforeParensInFunDef functionOrMethod args =
    match functionOrMethod, args with
    | _, PatParen (PatConst(Const "()", _)) -> false
    | "new", _ -> false
    | (s:string), _ -> 
        let parts = s.Split '.'
        not <| Char.IsUpper parts.[parts.Length - 1].[0]
    | _ -> true

let rec genParsedInput astContext = function
    | ImplFile im -> genImpFile astContext im
    | SigFile si -> genSigFile astContext si

and genImpFile astContext (ParsedImplFileInput(hs, mns)) = 
    col sepNone hs genParsedHashDirective +> (if hs.IsEmpty then sepNone else sepNln)
    +> col sepNln mns (genModuleOrNamespace astContext)

and genSigFile astContext (ParsedSigFileInput(hs, mns)) =
    col sepNone hs genParsedHashDirective +> (if hs.IsEmpty then sepNone else sepNln)
    +> col sepNln mns (genSigModuleOrNamespace astContext)

and genParsedHashDirective (ParsedHashDirective(h, s)) =
    let printArgument arg =
        match arg with
        | "" -> sepNone
        // Use verbatim string to escape '\' correctly
        | _ when arg.Contains("\\") -> !- (sprintf "@\"%O\"" arg)
        | _ -> !- (sprintf "\"%O\"" arg)

    !- "#" -- h +> sepSpace +> col sepSpace s printArgument

and genModuleOrNamespace astContext (ModuleOrNamespace(ats, px, ao, s, mds, isModule)) =
    genPreXmlDoc px
    +> genAttributes astContext ats
    +> ifElse (String.Equals(s, astContext.TopLevelModuleName, StringComparison.InvariantCultureIgnoreCase)) sepNone 
         (ifElse isModule (!- "module ") (!- "namespace ")
            +> opt sepSpace ao genAccess +> ifElse (s = "") (!- "global") (!- s) +> rep 2 sepNln)
    +> genModuleDeclList astContext mds

and genSigModuleOrNamespace astContext (SigModuleOrNamespace(ats, px, ao, s, mds, isModule)) =
    genPreXmlDoc px
    +> genAttributes astContext ats
    +> ifElse (String.Equals(s, astContext.TopLevelModuleName, StringComparison.InvariantCultureIgnoreCase)) sepNone 
          (ifElse isModule (!- "module ") (!- "namespace ")
    +> opt sepSpace ao genAccess -- s +> rep 2 sepNln)
    +> genSigModuleDeclList astContext mds

and genModuleDeclList astContext = function
    | [x] -> genModuleDecl astContext x

    | OpenL(xs, ys) ->
        fun ctx ->
            let xs = sortAndDeduplicate ((|Open|_|) >> Option.get) xs ctx
            match ys with
            | [] -> col sepNln xs (genModuleDecl astContext) ctx
            | _ -> (col sepNln xs (genModuleDecl astContext) +> rep 2 sepNln +> genModuleDeclList astContext ys) ctx

    | HashDirectiveL(xs, ys)
    | DoExprAttributesL(xs, ys) 
    | ModuleAbbrevL(xs, ys) 
    | OneLinerLetL(xs, ys) ->
        match ys with
        | [] -> col sepNln xs (genModuleDecl astContext)
        | _ -> col sepNln xs (genModuleDecl astContext) +> rep 2 sepNln +> genModuleDeclList astContext ys

    | MultilineModuleDeclL(xs, ys) ->
        match ys with
        | [] -> col (rep 2 sepNln) xs (genModuleDecl astContext)
        | _ -> col (rep 2 sepNln) xs (genModuleDecl astContext) +> rep 2 sepNln +> genModuleDeclList astContext ys
    | _ -> sepNone    

and genSigModuleDeclList astContext = function
    | [x] -> genSigModuleDecl astContext x

    | SigOpenL(xs, ys) ->
        fun ctx ->
            let xs = sortAndDeduplicate ((|SigOpen|_|) >> Option.get) xs ctx
            match ys with
            | [] -> col sepNln xs (genSigModuleDecl astContext) ctx
            | _ -> (col sepNln xs (genSigModuleDecl astContext) +> rep 2 sepNln +> genSigModuleDeclList astContext ys) ctx

    | SigHashDirectiveL(xs, ys) ->
        match ys with
        | [] -> col sepNone xs (genSigModuleDecl astContext)
        | _ -> col sepNone xs (genSigModuleDecl astContext) +> sepNln +> genSigModuleDeclList astContext ys

    | SigModuleAbbrevL(xs, ys) 
    | SigValL(xs, ys) ->
        match ys with
        | [] -> col sepNln xs (genSigModuleDecl astContext)
        | _ -> col sepNln xs (genSigModuleDecl astContext) +> rep 2 sepNln +> genSigModuleDeclList astContext ys

    | SigMultilineModuleDeclL(xs, ys) ->
        match ys with
        | [] -> col (rep 2 sepNln) xs (genSigModuleDecl astContext)
        | _ -> col (rep 2 sepNln) xs (genSigModuleDecl astContext) +> rep 2 sepNln +> genSigModuleDeclList astContext ys

    | _ -> sepNone

and genModuleDecl astContext = function
    | Attributes(ats) ->
        col sepNln ats (genAttribute astContext)
    | DoExpr(e) ->
        genExpr astContext e
    | Exception(ex) ->
        genException astContext ex
    | HashDirective(p) -> 
        genParsedHashDirective p
    | Extern(ats, px, ao, t, s, ps) ->
        genPreXmlDoc px
        +> genAttributes astContext ats
        -- "extern " +> genType astContext false t +> sepSpace +> opt sepSpace ao genAccess
        -- s +> sepOpenT +> col sepComma ps (genPat { astContext with IsCStylePattern = true }) +> sepCloseT
    // Add a new line after module-level let bindings
    | Let(b) ->
        genLetBinding { astContext with IsFirstChild = true } "let " b
    | LetRec(b::bs) -> 
        genLetBinding { astContext with IsFirstChild = true } "let rec " b 
        +> colPre (rep 2 sepNln) (rep 2 sepNln) bs (genLetBinding { astContext with IsFirstChild = false } "and ")

    | ModuleAbbrev(s1, s2) ->
        !- "module " -- s1 +> sepEq -- s2
    | NamespaceFragment(m) ->
        failwithf "NamespaceFragment hasn't been implemented yet: %O" m
    | NestedModule(ats, px, ao, s, mds) -> 
        genPreXmlDoc px
        +> genAttributes astContext ats -- "module " +> opt sepSpace ao genAccess -- s +> sepEq
        +> indent +> sepNln +> genModuleDeclList astContext mds +> unindent

    | Open(s) ->
        !- (sprintf "open %s" s)
    // There is no nested types and they are recursive if there are more than one definition
    | Types(t::ts) ->
        genTypeDefn { astContext with IsFirstChild = true } t 
        +> colPre (rep 2 sepNln) (rep 2 sepNln) ts (genTypeDefn { astContext with IsFirstChild = false })
    | md ->
        failwithf "Unexpected module declaration: %O" md

and genSigModuleDecl astContext = function
    | SigException(ex) ->
        genSigException astContext ex
    | SigHashDirective(p) -> 
        genParsedHashDirective p
    | SigVal(v) ->
        genVal astContext v
    | SigModuleAbbrev(s1, s2) ->
        !- "module " -- s1 +> sepEq -- s2
    | SigNamespaceFragment(m) ->
        failwithf "NamespaceFragment is not supported yet: %O" m
    | SigNestedModule(ats, px, ao, s, mds) -> 
        genPreXmlDoc px
        +> genAttributes astContext ats -- "module " +> opt sepSpace ao genAccess -- s +> sepEq
        +> indent +> sepNln +> genSigModuleDeclList astContext mds +> unindent

    | SigOpen(s) ->
        !- (sprintf "open %s" s)
    | SigTypes(t::ts) ->
        genSigTypeDefn { astContext with IsFirstChild = true } t 
        +> colPre (rep 2 sepNln) (rep 2 sepNln) ts (genSigTypeDefn { astContext with IsFirstChild = false })
    | md ->
        failwithf "Unexpected module signature declaration: %O" md

and genAccess (Access s) = !- s

and genAttribute astContext (Attribute(s, e, target)) = 
    match e with
    // Special treatment for function application on attributes
    | ConstExpr(Const "()", _) -> 
        !- "[<" +> opt sepColonFixed target (!-) -- s -- ">]"
    | e -> 
        !- "[<"  +> opt sepColonFixed target (!-) -- s +> genExpr astContext e -- ">]"
    
and genAttributesCore astContext ats = 
    let genAttributeExpr astContext (Attribute(s, e, target)) = 
        match e with
        | ConstExpr(Const "()", _) -> 
            opt sepColonFixed target (!-) -- s
        | e -> 
            opt sepColonFixed target (!-) -- s +> genExpr astContext e
    ifElse (Seq.isEmpty ats) sepNone (!- "[<" +> col sepSemi ats (genAttributeExpr astContext) -- ">]")

and genOnelinerAttributes astContext ats =
    ifElse (Seq.isEmpty ats) sepNone (genAttributesCore astContext ats +> sepSpace)

/// Try to group attributes if they are on the same line
/// Separate same-line attributes by ';'
/// Each bucket is printed in a different line
and genAttributes astContext ats = 
    ats
    |> Seq.groupBy (fun at -> at.Range.StartLine)
    |> Seq.map snd
    |> Seq.toList
    |> fun atss -> colPost sepNln sepNln atss (genAttributesCore astContext)

and genPreXmlDoc (PreXmlDoc lines) ctx = 
    if ctx.Config.StrictMode then
        colPost sepNln sepNln lines (sprintf "///%s" >> (!-)) ctx
    else ctx

and breakNln astContext brk e = 
    ifElse brk (indent +> sepNln +> genExpr astContext e +> unindent) 
        (indent +> autoNln (genExpr astContext e) +> unindent)

/// Preserve a break even if the expression is a one-liner
and preserveBreakNln astContext e ctx = 
    breakNln astContext (checkPreserveBreakForExpr e ctx) e ctx

/// Break but doesn't indent the expression
and noIndentBreakNln astContext e ctx = 
    ifElse (checkPreserveBreakForExpr e ctx) (sepNln +> genExpr astContext e) (autoNln (genExpr astContext e)) ctx

and genTyparList astContext tps = 
    ifElse (List.atMostOne tps) (col wordOr tps (genTypar astContext)) (sepOpenT +> col wordOr tps (genTypar astContext) +> sepCloseT)

and genTypeParam astContext tds tcs =
    ifElse (List.isEmpty tds) sepNone
        (!- "<" +> coli sepComma tds (fun i decl -> genTyparDecl { astContext with IsFirstTypeParam = i = 0 } decl) 
         +> colPre (!- " when ") wordAnd tcs (genTypeConstraint astContext) -- ">")

and genLetBinding astContext pref b = 
    match b with 
    | LetBinding(ats, px, ao, isInline, isMutable, p, e) ->
        let prefix =
            genPreXmlDoc px
            +> ifElse astContext.IsFirstChild (genAttributes astContext ats -- pref) 
                (!- pref +> genOnelinerAttributes astContext ats)
            +> opt sepSpace ao genAccess
            +> ifElse isMutable (!- "mutable ") sepNone +> ifElse isInline (!- "inline ") sepNone
            +> genPat astContext p

        match e with
        | TypedExpr(Typed, e, t) -> prefix +> sepColon +> genType astContext false t +> sepEq +> preserveBreakNln astContext e
        | e -> prefix +> sepEq +> preserveBreakNln astContext e

    | DoBinding(ats, px, e) ->
        let prefix = if pref.Contains("let") then pref.Replace("let", "do") else "do "
        genPreXmlDoc px
        +> genAttributes astContext ats -- prefix +> preserveBreakNln astContext e

    | b ->
        failwithf "%O isn't a let binding" b

and genShortGetProperty astContext e = 
    sepEq +> preserveBreakNln astContext e

and genProperty astContext prefix ao propertyKind ps e =
    let tuplerize ps =
        let rec loop acc = function
            | [p] -> (List.rev acc, p)
            | p1::ps -> loop (p1::acc) ps
            | [] -> invalidArg "p" "Patterns should not be empty"
        loop [] ps

    match ps with
    | [PatTuple ps] -> 
        let (ps, p) = tuplerize ps
        !- prefix +> opt sepSpace ao genAccess -- propertyKind
        +> ifElse (List.atMostOne ps) (col sepComma ps (genPat astContext) +> sepSpace) 
            (sepOpenT +> col sepComma ps (genPat astContext) +> sepCloseT +> sepSpace)
        +> genPat astContext p +> sepEq +> preserveBreakNln astContext e

    | ps -> 
        !- prefix +> opt sepSpace ao genAccess -- propertyKind +> col sepSpace ps (genPat astContext) 
        +> sepEq +> preserveBreakNln astContext e

and genPropertyWithGetSet astContext (b1, b2) =
    match b1, b2 with
    | PropertyBinding(ats, px, ao, isInline, mf1, PatLongIdent(ao1, s1, ps1, _), e1), 
      PropertyBinding(_, _, _, _, _, PatLongIdent(ao2, _, ps2, _), e2) ->
        let prefix =
            genPreXmlDoc px
            +> genAttributes astContext ats +> genMemberFlags astContext mf1
            +> ifElse isInline (!- "inline ") sepNone +> opt sepSpace ao genAccess
        assert(ps1 |> Seq.map fst |> Seq.forall Option.isNone)
        assert(ps2 |> Seq.map fst |> Seq.forall Option.isNone)
        let ps1 = List.map snd ps1
        let ps2 = List.map snd ps2
        prefix -- s1 +> sepSpace +> indent +> sepNln
        +> genProperty astContext "with " ao1 "get " ps1 e1 +> sepNln 
        +> genProperty astContext "and " ao2 "set " ps2 e2
        +> unindent
    | _ -> sepNone

/// Each member is separated by a new line.
and genMemberBindingList astContext = function
    | [x] -> genMemberBinding astContext x

    | MultilineBindingL(xs, ys) ->
        let prefix = sepNln +> col (rep 2 sepNln) xs (function 
                                   | Pair(x1, x2) -> genPropertyWithGetSet astContext (x1, x2) 
                                   | Single x -> genMemberBinding astContext x)
        match ys with
        | [] -> prefix
        | _ -> prefix +> rep 2 sepNln +> genMemberBindingList astContext ys

    | OneLinerBindingL(xs, ys) ->
        match ys with
        | [] -> col sepNln xs (genMemberBinding astContext)
        | _ -> col sepNln xs (genMemberBinding astContext) +> sepNln +> genMemberBindingList astContext ys
    | _ -> sepNone

and genMemberBinding astContext b = 
    match b with 
    | PropertyBinding(ats, px, ao, isInline, mf, p, e) -> 
        let prefix =
            genPreXmlDoc px
            +> genAttributes astContext ats +> genMemberFlags astContext mf
            +> ifElse isInline (!- "inline ") sepNone +> opt sepSpace ao genAccess

        let propertyKind =
            match mf with
            | MFProperty PropertyGet -> "get "
            | MFProperty PropertySet -> "set "
            | mf -> failwithf "Unexpected member flags: %O" mf

        match p with
        | PatLongIdent(ao, s, ps, _) ->   
            assert (ps |> Seq.map fst |> Seq.forall Option.isNone)
            match ao, propertyKind, ps with
            | None, "get ", [_, PatParen(PatConst(Const "()", _))] ->
                // Provide short-hand notation `x.Member = ...` for `x.Member with get()` getters
                prefix -- s +> genShortGetProperty astContext e
            | _ ->
                let ps = List.map snd ps              
                prefix -- s +> sepSpace +> indent +> sepNln +> 
                genProperty astContext "with " ao propertyKind ps e
                +> unindent
        | p -> failwithf "Unexpected pattern: %O" p

    | MemberBinding(ats, px, ao, isInline, mf, p, e) ->
        let prefix =
            genPreXmlDoc px
            +> genAttributes astContext ats +> genMemberFlags astContext mf
            +> ifElse isInline (!- "inline ") sepNone +> opt sepSpace ao genAccess +> genPat astContext p

        match e with
        | TypedExpr(Typed, e, t) -> prefix +> sepColon +> genType astContext false t +> sepEq +> preserveBreakNln astContext e
        | e -> prefix +> sepEq +> preserveBreakNln astContext e

    | ExplicitCtor(ats, px, ao, p, e, so) ->
        let prefix =
            genPreXmlDoc px
            +> genAttributes astContext ats
            +> opt sepSpace ao genAccess +> genPat astContext p 
            +> opt sepNone so (sprintf " as %s" >> (!-))

        match e with
        // Handle special "then" block i.e. fake sequential expressions in constructors
        | Sequential(e1, e2, false) -> 
            prefix +> sepEq +> indent +> sepNln 
            +> genExpr astContext e1 ++ "then " +> preserveBreakNln astContext e2 +> unindent

        | e -> prefix +> sepEq +> preserveBreakNln astContext e

    | b -> failwithf "%O isn't a member binding" b

and genMemberFlags astContext = function
    | MFMember _ -> !- "member "
    | MFStaticMember _ -> !- "static member "
    | MFConstructor _ -> sepNone
    | MFOverride _ -> ifElse astContext.IsInterface (!- "member ") (!- "override ")

and genVal astContext (Val(ats, px, ao, s, t, vi, _)) = 
    let (FunType namedArgs) = (t, vi)
    genPreXmlDoc px
    +> genAttributes astContext ats 
    +> atCurrentColumn (indent -- "val " +> opt sepSpace ao genAccess -- s 
                        +> sepColon +> genTypeList astContext namedArgs +> unindent)

and genRecordFieldName astContext (RecordFieldName(s, eo)) =
    opt sepNone eo (fun e -> !- s +> sepEq +> preserveBreakNln astContext e)

and genExpr astContext = function
    | SingleExpr(kind, e) -> str kind +> genExpr astContext e
    | ConstExpr(c) -> genConst c
    | NullExpr -> !- "null"
    // Not sure about the role of e1
    | Quote(_, e2, isRaw) ->         
        let e = genExpr astContext e2
        ifElse isRaw (!- "<@@ " +> e -- " @@>") (!- "<@ " +> e -- " @>")
    | TypedExpr(TypeTest, e, t) -> genExpr astContext e -- " :? " +> genType astContext false t
    | TypedExpr(New, e, t) -> 
        !- "new " +> genType astContext false t +> ifElse (hasParenthesis e) sepNone sepSpace +> genExpr astContext e
    | TypedExpr(Downcast, e, t) -> genExpr astContext e -- " :?> " +> genType astContext false t
    | TypedExpr(Upcast, e, t) -> genExpr astContext e -- " :> " +> genType astContext false t
    | TypedExpr(Typed, e, t) -> genExpr astContext e +> sepColon +> genType astContext false t
    | Tuple es -> atCurrentColumn (coli sepComma es (fun i -> if i = 0 then genExpr astContext else noIndentBreakNln astContext))
    | ArrayOrList(isArray, [], _) -> 
        ifElse isArray (sepOpenAFixed +> sepCloseAFixed) (sepOpenLFixed +> sepCloseLFixed)
    | ArrayOrList(isArray, xs, isSimple) -> 
        let sep = ifElse isSimple sepSemi sepSemiNln
        ifElse isArray (sepOpenA +> atCurrentColumn (colAutoNlnSkip0 sep xs (genExpr astContext)) +> sepCloseA) 
            (sepOpenL +> atCurrentColumn (colAutoNlnSkip0 sep xs (genExpr astContext)) +> sepCloseL)

    | Record(inheritOpt, xs, eo) -> 
        sepOpenS 
        +> opt (if xs.IsEmpty then sepNone else sepSemi) inheritOpt 
            (fun (typ, expr) -> !- "inherit " +> genType astContext false typ +> genExpr astContext expr)
        +> opt (!- " with ") eo (genExpr astContext) +> atCurrentColumn (col sepSemiNln xs (genRecordFieldName astContext))
        +> sepCloseS

    | ObjExpr(t, eio, bd, ims) ->
        // Check the role of the second part of eio
        let param = opt sepNone (Option.map fst eio) (genExpr astContext)
        sepOpenS +> 
        atCurrentColumn (!- "new " +> genType astContext false t +> param -- " with" 
            +> indent +> sepNln +> genMemberBindingList { astContext with IsInterface = true } bd +> unindent
            +> colPre sepNln sepNln ims (genInterfaceImpl astContext)) +> sepCloseS

    | While(e1, e2) -> 
        atCurrentColumn (!- "while " +> genExpr astContext e1 -- " do" 
        +> indent +> sepNln +> genExpr astContext e2 +> unindent)

    | For(s, e1, e2, e3, isUp) ->
        atCurrentColumn (!- (sprintf "for %s = " s) +> genExpr astContext e1 
            +> ifElse isUp (!- " to ") (!- " downto ") +> genExpr astContext e2 -- " do" 
            +> indent +> sepNln +> genExpr astContext e3 +> unindent)

    // Handle the form 'for i in e1 -> e2'
    | ForEach(p, e1, e2, isArrow) ->
        atCurrentColumn (!- "for " +> genPat astContext p -- " in " +> genExpr { astContext with IsNakedRange = true } e1 
            +> ifElse isArrow (sepArrow +> preserveBreakNln astContext e2) (!- " do" +> indent +> sepNln +> genExpr astContext e2 +> unindent))

    | CompExpr(isArrayOrList, e) ->
        let astContext = { astContext with IsNakedRange = true }
        ifElse isArrayOrList (genExpr astContext e) 
            (sepOpenS +> noIndentBreakNln astContext e 
             +> ifElse (checkBreakForExpr e) (unindent +> sepNln +> sepCloseSFixed) sepCloseS) 

    | ArrayOrListOfSeqExpr(isArray, e) -> 
        let astContext = { astContext with IsNakedRange = true }
        ifElse isArray (sepOpenA +> genExpr astContext e +> sepCloseA) (sepOpenL +> genExpr astContext e +> sepCloseL)
    | JoinIn(e1, e2) -> genExpr astContext e1 -- " in " +> genExpr astContext e2
    | Paren(DesugaredLambda(cps, e)) ->
        sepOpenT -- "fun " +>  col sepSpace cps (genComplexPats astContext) +> sepArrow +> noIndentBreakNln astContext e +> sepCloseT
    | DesugaredLambda(cps, e) -> 
        !- "fun " +>  col sepSpace cps (genComplexPats astContext) +> sepArrow +> preserveBreakNln astContext e 
    | Paren(Lambda(e, sps)) ->
        sepOpenT -- "fun " +> col sepSpace sps (genSimplePats astContext) +> sepArrow +> noIndentBreakNln astContext e +> sepCloseT
    // When there are parentheses, most likely lambda will appear in function application
    | Lambda(e, sps) -> 
        !- "fun " +> col sepSpace sps (genSimplePats astContext) +> sepArrow +> preserveBreakNln astContext e
    | MatchLambda(sp, _) -> !- "function " +> colPre sepNln sepNln sp (genClause astContext true)
    | Match(e, cs) -> 
        atCurrentColumn (!- "match " +> genExpr astContext e -- " with" +> colPre sepNln sepNln cs (genClause astContext true))
    | TraitCall(tps, msg, e) -> 
        genTyparList astContext tps +> sepColon +> sepOpenT +> genMemberSig astContext msg +> sepCloseT 
        +> sepSpace +> genExpr astContext e

    | Paren e -> 
        // Parentheses nullify effects of no space inside DotGet
        sepOpenT +> genExpr { astContext with IsInsideDotGet = false } e +> sepCloseT
    | CompApp(s, e) ->
        !- s +> sepSpace +> sepOpenS +> genExpr { astContext with IsNakedRange = true } e 
        +> ifElse (checkBreakForExpr e) (sepNln +> sepCloseSFixed) sepCloseS
    // This supposes to be an infix function, but for some reason it isn't picked up by InfixApps
    | App(Var "?", e::es) -> genExpr astContext e -- "?" +> col sepSpace es (genExpr astContext)
    | App(Var "..", [e1; e2]) ->
        let expr = genExpr astContext e1 -- ".." +> genExpr astContext e2
        ifElse astContext.IsNakedRange expr (sepOpenS +> expr +> sepCloseS)
    | App(Var ".. ..", [e1; e2; e3]) -> 
        let expr = genExpr astContext e1 -- ".." +> genExpr astContext e2 -- ".." +> genExpr astContext e3
        ifElse astContext.IsNakedRange expr (sepOpenS +> expr +> sepCloseS)
    // Separate two prefix ops by spaces
    | PrefixApp(s1, PrefixApp(s2, e)) -> !- (sprintf "%s %s" s1 s2) +> genExpr astContext e
    | PrefixApp(s, e) -> !- s +> genExpr astContext e
    // Handle spaces of infix application based on which category it belongs to
    | InfixApps(e, es) -> 
        // Only put |> on the same line in a very trivial expression
        atCurrentColumn (genExpr astContext e +> genInfixApps astContext (checkNewLine e es) es)

    | TernaryApp(e1,e2,e3) -> 
        atCurrentColumn (genExpr astContext e1 +> !- "?" +> genExpr astContext e2 +> sepSpace +> !- "<-" +> sepSpace +> genExpr astContext e3)

    // This filters a few long examples of App
    | DotGetAppSpecial(s, es) ->
        !- s 
        +> atCurrentColumn 
             (colAutoNlnSkip0 sepNone es (fun (s, e) ->
                (!- (sprintf ".%s" s) 
                    +> ifElse (hasParenthesis e) sepNone sepSpace +> genExpr astContext e)))

    | DotGetApp(e, es) -> 
        let expr = 
            match e with
            | App(e1, [e2]) -> 
                noNln (genExpr astContext e1 +> ifElse (hasParenthesis e2) sepNone sepSpace +> genExpr astContext e2)
            | _ -> 
                noNln (genExpr astContext e)
        expr
        +> indent 
        +> (col sepNone es (fun (s, e) -> 
                autoNln (!- (sprintf ".%s" s) 
                    +> ifElse (hasParenthesis e) sepNone sepSpace +> genExpr astContext e)))
        +> unindent

    // Unlike infix app, function application needs a level of indentation
    | App(e1, [e2]) -> 
        atCurrentColumn (genExpr astContext e1 +> 
            ifElse (not astContext.IsInsideDotGet)
                (ifElse (hasParenthesis e2) 
                    (ifElse (addSpaceBeforeParensInFunCall e1 e2) sepBeforeArg sepNone) 
                    sepSpace)
                sepNone
            +> indent +> autoNln (genExpr astContext e2) +> unindent)

    // Always spacing in multiple arguments
    | App(e, es) -> 
        atCurrentColumn (genExpr astContext e +> 
            colPre sepSpace sepSpace es (fun e -> indent +> autoNln (genExpr astContext e) +> unindent))

    | TypeApp(e, ts) -> genExpr astContext e -- "<" +> col sepComma ts (genType astContext false) -- ">"
    | LetOrUses(bs, e) ->
        atCurrentColumn (genLetOrUseList astContext bs +> sepNln +> genExpr astContext e)

    // Could customize a bit if e is single line
    | TryWith(e, cs) -> 
        let prefix = !- "try " +> indent +> sepNln +> genExpr astContext e +> unindent ++ "with"
        match cs with
        | [c] -> 
            atCurrentColumn (prefix +> sepSpace +> genClause astContext false c)
        | _ -> 
            atCurrentColumn (prefix +> indentOnWith +> sepNln +> col sepNln cs (genClause astContext true) +> unindentOnWith)

    | TryFinally(e1, e2) -> 
        atCurrentColumn (!- "try " +> indent +> sepNln +> genExpr astContext e1 +> unindent ++ "finally" 
            +> indent +> sepNln +> genExpr astContext e2 +> unindent)    

    | SequentialSimple es -> atCurrentColumn (colAutoNlnSkip0 sepSemi es (genExpr astContext))
    // It seems too annoying to use sepSemiNln
    | Sequentials es -> atCurrentColumn (col sepNln es (genExpr astContext))
    // A generalization of IfThenElse
    | ElIf((e1,e2, _)::es, en) ->
        atCurrentColumn (!- "if " +> ifElse (checkBreakForExpr e1) (genExpr astContext e1 ++ "then") (genExpr astContext e1 +- "then") -- " " 
            +> preserveBreakNln astContext e2
            +> fun ctx -> col sepNone es (fun (e1, e2, r) ->
                             ifElse (startWith "elif" r ctx) (!+ "elif ") (!+ "else if ")
                             +> ifElse (checkBreakForExpr e1) (genExpr astContext e1 ++ "then") (genExpr astContext e1 +- "then") 
                             -- " " +> preserveBreakNln astContext e2) ctx
            ++ "else " +> preserveBreakNln astContext en)

    | IfThenElse(e1, e2, None) -> 
        atCurrentColumn (!- "if " +> ifElse (checkBreakForExpr e1) (genExpr astContext e1 ++ "then") (genExpr astContext e1 +- "then") 
                         -- " " +> preserveBreakNln astContext e2)
    // At this stage, all symbolic operators have been handled.
    | OptVar(s, isOpt) -> ifElse isOpt (!- "?") sepNone -- s
    | LongIdentSet(s, e) -> !- (sprintf "%s <- " s) +> genExpr astContext e
    | DotIndexedGet(e, es) -> genExpr astContext e -- "." +> sepOpenLFixed +> genIndexers astContext es +> sepCloseLFixed
    | DotIndexedSet(e1, es, e2) -> genExpr astContext e1 -- ".[" +> genIndexers astContext es -- "] <- " +> genExpr astContext e2
    | DotGet(e, s) -> 
        genExpr { astContext with IsInsideDotGet = true } e -- sprintf ".%s" s
    | DotSet(e1, s, e2) -> genExpr astContext e1 -- sprintf ".%s <- " s +> genExpr astContext e2
    | LetOrUseBang(isUse, p, e1, e2) ->
        atCurrentColumn (ifElse isUse (!- "use! ") (!- "let! ") 
            +> genPat astContext p -- " = " +> genExpr astContext e1 +> sepNln +> genExpr astContext e2)

    | ParsingError r -> 
        raise <| FormatException (sprintf "Parsing error(s) between line %i column %i and line %i column %i" 
            r.StartLine (r.StartColumn + 1) r.EndLine (r.EndColumn + 1))
    | UnsupportedExpr r -> 
        raise <| FormatException (sprintf "Unsupported construct(s) between line %i column %i and line %i column %i" 
            r.StartLine (r.StartColumn + 1) r.EndLine (r.EndColumn + 1))
    | e -> failwithf "Unexpected expression: %O" e

and genLetOrUseList astContext = function
    | [p, x] -> genLetBinding { astContext with IsFirstChild = true } p x
    | OneLinerLetOrUseL(xs, ys) ->
        match ys with
        | [] -> 
            col sepNln xs (fun (p, x) -> genLetBinding { astContext with IsFirstChild = p <> "and" } p x)
        | _ -> 
            col sepNln xs (fun (p, x) -> genLetBinding { astContext with IsFirstChild = p <> "and" } p x) 
            +> rep 2 sepNln +> genLetOrUseList astContext ys

    | MultilineLetOrUseL(xs, ys) ->
        match ys with
        | [] -> 
            col (rep 2 sepNln) xs (fun (p, x) -> genLetBinding { astContext with IsFirstChild = p <> "and" } p x)
            // Add a trailing new line to separate these with the main expression
            +> sepNln 
        | _ -> 
            col (rep 2 sepNln) xs (fun (p, x) -> genLetBinding { astContext with IsFirstChild = p <> "and" } p x) 
            +> rep 2 sepNln +> genLetOrUseList astContext ys

    | _ -> sepNone   

/// When 'hasNewLine' is set, the operator is forced to be in a new line
and genInfixApps astContext hasNewLine = function
    | (s, e)::es ->
        (ifElse hasNewLine (sepNln -- s +> sepSpace +> genExpr astContext e)
           (ifElse (NoSpaceInfixOps.Contains s) (!- s +> autoNln (genExpr astContext e))
              (ifElse (NoBreakInfixOps.Contains s) (sepSpace -- s +> sepSpace +> genExpr astContext e)
                (sepSpace +> autoNln (!- s +> sepSpace +> genExpr astContext e)))))
        +> genInfixApps astContext (hasNewLine || checkNewLine e es) es

    | [] -> sepNone

/// Use in indexed set and get only
and genIndexers astContext = function
    | Indexer(Pair(IndexedVar eo1, IndexedVar eo2)) :: es ->
        ifElse (eo1.IsNone && eo2.IsNone) (!- "*") 
            (opt sepNone eo1 (genExpr astContext) -- ".." +> opt sepNone eo2 (genExpr astContext))
        +> ifElse es.IsEmpty sepNone (sepComma +> genIndexers astContext es)
    | Indexer(Single(IndexedVar eo)) :: es -> 
        ifElse eo.IsNone (!- "*") (opt sepNone eo (genExpr astContext))
        +> ifElse es.IsEmpty sepNone (sepComma +> genIndexers astContext es)
    | Indexer(Single e) :: es -> 
            genExpr astContext e +> ifElse es.IsEmpty sepNone (sepComma +> genIndexers astContext es)
    | _ -> sepNone

and genTypeDefn astContext (TypeDef(ats, px, ao, tds, tcs, tdr, ms, s)) = 
    let typeName = 
        genPreXmlDoc px 
        +> ifElse astContext.IsFirstChild (genAttributes astContext ats -- "type ") 
            (!- "and " +> genOnelinerAttributes astContext ats) 
        +> opt sepSpace ao genAccess -- s
        +> genTypeParam astContext tds tcs

    match tdr with
    | Simple(TDSREnum ecs) ->
        typeName +> sepEq 
        +> indent +> sepNln
        +> col sepNln ecs (genEnumCase { astContext with HasVerticalBar = true })
        +> genMemberDefnList { astContext with IsInterface = false } ms
        // Add newline after un-indent to be spacing-correct
        +> unindent

    | Simple(TDSRUnion(ao', xs)) ->
        typeName +> sepEq 
        +> indent +> sepNln +> opt sepNln ao' genAccess 
        +> col sepNln xs (genUnionCase { astContext with HasVerticalBar = true })
        +> genMemberDefnList { astContext with IsInterface = false } ms
        +> unindent

    | Simple(TDSRRecord(ao', fs)) ->
        typeName +> sepEq 
        +> indent +> sepNln +> opt sepSpace ao' genAccess +> sepOpenS 
        +> atCurrentColumn (col sepSemiNln fs (genField astContext "")) +> sepCloseS
        +> genMemberDefnList { astContext with IsInterface = false } ms 
        +> unindent 

    | Simple TDSRNone -> 
        typeName
    | Simple(TDSRTypeAbbrev t) -> 
        typeName +> sepEq +> genType astContext false t
    | Simple(TDSRException(ExceptionDefRepr(ats, px, ao, uc))) ->
        genExceptionBody astContext ats px ao uc

    | ObjectModel(TCSimple (TCStruct | TCInterface | TCClass) as tdk, MemberDefnList(impCtor, others)) ->
        let isInterface =
            match tdk with
            | TCSimple TCInterface -> true
            | _ -> false
        let astContext = { astContext with IsInterface = isInterface}
        typeName +> opt sepNone impCtor (genMemberDefn astContext) +> sepEq 
        +> indent +> sepNln +> genTypeDefKind tdk
        +> indent +> genMemberDefnList astContext others +> unindent
        ++ "end" +> unindent

    | ObjectModel(TCSimple TCAugmentation, _) ->
        typeName -- " with" +> indent
        // Remember that we use MemberDefn of parent node
        +> genMemberDefnList { astContext with IsInterface = false } ms +> unindent

    | ObjectModel(TCDelegate(FunType ts), _) ->
        typeName +> sepEq -- "delegate of " +> genTypeList astContext ts
    | ObjectModel(_, MemberDefnList(impCtor, others)) ->
        typeName +> opt sepNone impCtor (genMemberDefn { astContext with IsInterface = false }) +> sepEq +> indent
        +> genMemberDefnList { astContext with IsInterface = false } others +> unindent

    | ExceptionRepr(ExceptionDefRepr(ats, px, ao, uc)) ->
        genExceptionBody astContext ats px ao uc

and genSigTypeDefn astContext (SigTypeDef(ats, px, ao, tds, tcs, tdr, ms, s)) = 
    let typeName = 
        genPreXmlDoc px 
        +> ifElse astContext.IsFirstChild (genAttributes astContext ats -- "type ") 
            (!- "and " +> genOnelinerAttributes astContext ats) 
        +> opt sepSpace ao genAccess -- s
        +> genTypeParam astContext tds tcs

    match tdr with
    | SigSimple(TDSREnum ecs) ->
        typeName +> sepEq 
        +> indent +> sepNln
        +> col sepNln ecs (genEnumCase { astContext with HasVerticalBar = true })
        +> colPre sepNln sepNln ms (genMemberSig astContext)
        // Add newline after un-indent to be spacing-correct
        +> unindent
         
    | SigSimple(TDSRUnion(ao', xs)) ->
        typeName +> sepEq 
        +> indent +> sepNln +> opt sepNln ao' genAccess 
        +> col sepNln xs (genUnionCase { astContext with HasVerticalBar = true })
        +> colPre sepNln sepNln ms (genMemberSig astContext)
        +> unindent

    | SigSimple(TDSRRecord(ao', fs)) ->
        typeName +> sepEq 
        +> indent +> sepNln +> opt sepNln ao' genAccess +> sepOpenS 
        +> atCurrentColumn (col sepSemiNln fs (genField astContext "")) +> sepCloseS
        +> colPre sepNln sepNln ms (genMemberSig astContext)
        +> unindent 

    | SigSimple TDSRNone -> 
        typeName
    | SigSimple(TDSRTypeAbbrev t) -> 
        typeName +> sepEq +> genType astContext false t
    | SigSimple(TDSRException(ExceptionDefRepr(ats, px, ao, uc))) ->
            genExceptionBody astContext ats px ao uc

    | SigObjectModel(TCSimple (TCStruct | TCInterface | TCClass) as tdk, mds) ->
        typeName +> sepEq +> indent +> sepNln +> genTypeDefKind tdk
        +> indent +> colPre sepNln sepNln mds (genMemberSig astContext) +> unindent
        ++ "end" +> unindent

    | SigObjectModel(TCSimple TCAugmentation, _) ->
        typeName -- " with" +> indent +> sepNln 
        // Remember that we use MemberSig of parent node
        +> col sepNln ms (genMemberSig astContext) +> unindent

    | SigObjectModel(TCDelegate(FunType ts), _) ->
        typeName +> sepEq -- "delegate of " +> genTypeList astContext ts
    | SigObjectModel(_, mds) -> 
        typeName +> sepEq +> indent +> sepNln 
        +> col sepNln mds (genMemberSig astContext) +> unindent

    | SigExceptionRepr(SigExceptionDefRepr(ats, px, ao, uc)) ->
        genExceptionBody astContext ats px ao uc

and genMemberSig astContext = function
    | MSMember(Val(ats, px, ao, s, t, vi, _), mf) -> 
        let (FunType namedArgs) = (t, vi)
        genPreXmlDoc px +> genAttributes astContext ats 
        +> atCurrentColumn (indent +> genMemberFlags { astContext with IsInterface = false } mf +> opt sepNone ao genAccess
                                   +> ifElse (s = "``new``") (!- "new") (!- s) 
                                   +> sepColon +> genTypeList astContext namedArgs +> unindent)

    | MSInterface t -> !- "interface " +> genType astContext false t
    | MSInherit t -> !- "inherit " +> genType astContext false t
    | MSValField f -> genField astContext "val " f
    | MSNestedType _ -> invalidArg "md" "This is not implemented in F# compiler"

and genTyparDecl astContext (TyparDecl(ats, tp)) =
    genOnelinerAttributes astContext ats +> genTypar astContext tp

and genTypeDefKind = function
    | TCSimple TCUnspecified -> sepNone
    | TCSimple TCClass -> !- "class"
    | TCSimple TCInterface -> !- "interface"
    | TCSimple TCStruct -> !- "struct"
    | TCSimple TCRecord -> sepNone
    | TCSimple TCUnion -> sepNone
    | TCSimple TCAbbrev -> sepNone
    | TCSimple TCHiddenRepr -> sepNone
    | TCSimple TCAugmentation -> sepNone
    | TCSimple TCILAssemblyCode -> sepNone
    | TCDelegate _ -> sepNone

and genExceptionBody astContext ats px ao uc = 
    genPreXmlDoc px
    +> genAttributes astContext ats  -- "exception " 
    +> opt sepSpace ao genAccess +> genUnionCase { astContext with HasVerticalBar = false } uc

and genException astContext (ExceptionDef(ats, px, ao, uc, ms)) = 
    genExceptionBody astContext ats px ao uc 
    +> ifElse ms.IsEmpty sepNone 
        (!- " with" +> indent +> genMemberDefnList { astContext with IsInterface = false } ms +> unindent)

and genSigException astContext (SigExceptionDef(ats, px, ao, uc, ms)) = 
    genExceptionBody astContext ats px ao uc 
    +> colPre sepNln sepNln ms (genMemberSig astContext)

and genUnionCase astContext (UnionCase(ats, px, _, s, UnionCaseType fs)) =
    genPreXmlDoc px
    +> ifElse astContext.HasVerticalBar sepBar sepNone
    +> genOnelinerAttributes astContext ats -- s 
    +> colPre wordOf sepStar fs (genField { astContext with IsUnionField = true } "")

and genEnumCase astContext (EnumCase(ats, px, _, c)) =
    genPreXmlDoc px 
    +> ifElse astContext.HasVerticalBar sepBar sepNone 
    +> genOnelinerAttributes astContext ats +> genConst c

and genField astContext prefix (Field(ats, px, ao, isStatic, isMutable, t, so)) = 
    // Being protective on union case declaration
    let t = genType astContext astContext.IsUnionField t
    genPreXmlDoc px 
    +> genAttributes astContext ats +> ifElse isStatic (!- "static ") sepNone -- prefix
    +> ifElse isMutable (!- "mutable ") sepNone +> opt sepSpace ao genAccess  
    +> opt sepColon so (!-) +> t

and genType astContext outerBracket t =
    let rec loop = function
        | THashConstraint t -> !- "#" +> loop t
        | TMeasurePower(t, n) -> loop t -- "^" +> str n
        | TMeasureDivide(t1, t2) -> loop t1 -- " / " +> loop t2
        | TStaticConstant(c) -> genConst c
        | TStaticConstantExpr(e) -> genExpr astContext e
        | TStaticConstantNamed(t1, t2) -> loop t1 -- "=" +> loop t2
        | TArray(t, n) -> loop t -- " [" +> rep (n - 1) (!- ",") -- "]"
        | TAnon -> sepWild
        | TVar tp -> genTypar astContext tp
        // Drop bracket around tuples before an arrow
        | TFun(TTuple ts, t) -> sepOpenT +> loopTTupleList ts +> sepArrow +> loop t +> sepCloseT
        // Do similar for tuples after an arrow
        | TFun(t, TTuple ts) -> sepOpenT +> loop t +> sepArrow +> loopTTupleList ts +> sepCloseT
        | TFuns ts -> sepOpenT +> col sepArrow ts loop +> sepCloseT
        | TApp(t, ts, isPostfix) -> 
            let postForm = 
                match ts with
                | [] ->  loop t
                | [t'] -> loop t' +> sepSpace +> loop t
                | ts -> sepOpenT +> col sepComma ts loop +> sepCloseT +> loop t

            ifElse isPostfix postForm (loop t +> genPrefixTypes astContext ts)

        | TLongIdentApp(t, s, ts) -> loop t -- sprintf ".%s" s +> genPrefixTypes astContext ts
        | TTuple ts -> sepOpenT +> loopTTupleList ts +> sepCloseT
        | TWithGlobalConstraints(TFuns ts, tcs) -> col sepArrow ts loop +> colPre (!- " when ") wordAnd tcs (genTypeConstraint astContext)        
        | TWithGlobalConstraints(t, tcs) -> loop t +> colPre (!- " when ") wordAnd tcs (genTypeConstraint astContext)
        | TLongIdent s -> !- s
        | t -> failwithf "Unexpected type: %O" t

    and loopTTupleList = function
        | [] -> sepNone
        | [(_, t)] -> loop t
        | (isDivide, t) :: ts ->
            loop t -- (if isDivide then " / " else " * ") +> loopTTupleList ts

    match t with
    | TFun(TTuple ts, t) -> 
        ifElse outerBracket (sepOpenT +> loopTTupleList ts +> sepArrow +> loop t +> sepCloseT)
            (loopTTupleList ts +> sepArrow +> loop t)
    | TFuns ts -> ifElse outerBracket (sepOpenT +> col sepArrow ts loop +> sepCloseT) (col sepArrow ts loop)
    | TTuple ts -> ifElse outerBracket (sepOpenT +> loopTTupleList ts +> sepCloseT) (loopTTupleList ts)
    | _ -> loop t
  
and genPrefixTypes astContext = function
    | [] -> sepNone
    // Some patterns without spaces could cause a parsing error
    | (TStaticConstant _ | TStaticConstantExpr _ | TStaticConstantNamed _ | TVar(Typar(_, true)) as t)::ts -> 
        !- "< " +> col sepComma (t::ts) (genType astContext false) -- " >"
    | ts -> !- "<" +> col sepComma ts (genType astContext false) -- ">"

and genTypeList astContext = function
    | [] -> sepNone
    | (t, [ArgInfo(attribs, so, isOpt)])::ts -> 
        let hasBracket = not ts.IsEmpty
        let gt =
            match t with
            | TTuple _ ->
                opt sepColonFixed so (if isOpt then (sprintf "?%s" >> (!-)) else (!-)) 
                +> genType astContext hasBracket t 
            | TFun _ ->
                // Fun is grouped by brackets inside 'genType astContext true t'
                opt sepColonFixed so (if isOpt then (sprintf "?%s" >> (!-)) else (!-)) 
                +> genType astContext true t
            | _ -> 
                opt sepColonFixed so (!-) +> genType astContext false t
        genOnelinerAttributes astContext attribs
        +> gt +> ifElse ts.IsEmpty sepNone (autoNln (sepArrow +> genTypeList astContext ts))

    | (TTuple ts', argInfo)::ts -> 
        // The '/' separator shouldn't appear here
        let hasBracket = not ts.IsEmpty
        let gt = col sepStar (Seq.zip argInfo (Seq.map snd ts')) 
                    (fun (ArgInfo(attribs, so, isOpt), t) ->
                        genOnelinerAttributes astContext attribs
                        +> opt sepColonFixed so (if isOpt then (sprintf "?%s" >> (!-)) else (!-))
                        +> genType astContext hasBracket t)
        gt +> ifElse ts.IsEmpty sepNone (autoNln (sepArrow +> genTypeList astContext ts))

    | (t, _)::ts -> 
        let gt = genType astContext false t
        gt +> ifElse ts.IsEmpty sepNone (autoNln (sepArrow +> genTypeList astContext ts))

and genTypar astContext (Typar(s, isHead)) = 
    ifElse isHead (ifElse astContext.IsFirstTypeParam (!- " ^") (!- "^")) (!-"'") -- s
    
and genTypeConstraint astContext = function
    | TyparSingle(kind, tp) -> genTypar astContext tp +> sepColon -- sprintf "%O" kind
    | TyparDefaultsToType(tp, t) -> !- "default " +> genTypar astContext tp +> sepColon +> genType astContext false t
    | TyparSubtypeOfType(tp, t) -> genTypar astContext tp -- " :> " +> genType astContext false t
    | TyparSupportsMember(tps, msg) -> 
        genTyparList astContext tps +> sepColon +> sepOpenT +> genMemberSig astContext msg +> sepCloseT
    | TyparIsEnum(tp, ts) -> 
        genTypar astContext tp +> sepColon -- "enum<" +> col sepComma ts (genType astContext false) -- ">"
    | TyparIsDelegate(tp, ts) ->
        genTypar astContext tp +> sepColon -- "delegate<" +> col sepComma ts (genType astContext false) -- ">"

and genInterfaceImpl astContext (InterfaceImpl(t, bs)) = 
    match bs with
    | [] -> !- "interface " +> genType astContext false t
    | bs ->
        !- "interface " +> genType astContext false t -- " with"
        +> indent +> sepNln +> genMemberBindingList { astContext with IsInterface = true } bs +> unindent

and genClause astContext hasBar (Clause(p, e, eo)) = 
    ifElse hasBar sepBar sepNone +> genPat astContext p 
    +> optPre (!- " when ") sepNone eo (genExpr astContext) +> sepArrow +> preserveBreakNln astContext e

/// Each multiline member definition has a pre and post new line. 
and genMemberDefnList astContext = function
    | [x] -> sepNln +> genMemberDefn astContext x

    | MDOpenL(xs, ys) ->
        fun ctx ->
            let xs = sortAndDeduplicate ((|MDOpen|_|) >> Option.get) xs ctx
            match ys with
            | [] -> col sepNln xs (genMemberDefn astContext) ctx
            | _ -> (col sepNln xs (genMemberDefn astContext) +> rep 2 sepNln +> genMemberDefnList astContext ys) ctx

    | MultilineMemberDefnL(xs, []) ->
        rep 2 sepNln 
        +> col (rep 2 sepNln) xs (function
                | Pair(x1, x2) -> genPropertyWithGetSet astContext (x1, x2)
                | Single x -> genMemberDefn astContext x)

    | MultilineMemberDefnL(xs, ys) ->
        rep 2 sepNln 
        +> col (rep 2 sepNln) xs (function
                | Pair(x1, x2) -> genPropertyWithGetSet astContext (x1, x2)
                | Single x -> genMemberDefn astContext x) 
        +> sepNln +> genMemberDefnList astContext ys

    | OneLinerMemberDefnL(xs, ys) ->
        sepNln +> col sepNln xs (genMemberDefn astContext) +> genMemberDefnList astContext ys
    | _ -> sepNone

and genMemberDefn astContext = function
    | MDNestedType _ -> invalidArg "md" "This is not implemented in F# compiler"
    | MDOpen(s) -> !- (sprintf "open %s" s)
    // What is the role of so
    | MDImplicitInherit(t, e, _) -> !- "inherit " +> genType astContext false t +> genExpr astContext e
    | MDInherit(t, _) -> !- "inherit " +> genType astContext false t
    | MDValField f -> genField astContext "val " f
    | MDImplicitCtor(ats, ao, ps, so) -> 
        // In implicit constructor, attributes should come even before access qualifiers
        ifElse ats.IsEmpty sepNone (sepSpace +> genOnelinerAttributes astContext ats)
        +> optPre sepSpace sepSpace ao genAccess +> sepOpenT
        +> col sepComma ps (genSimplePat astContext) +> sepCloseT
        +> optPre (!- " as ") sepNone so (!-)

    | MDMember(b) -> genMemberBinding astContext b
    | MDLetBindings(isStatic, isRec, b::bs) ->
        let prefix = 
            if isStatic && isRec then "static let rec "
            elif isStatic then "static let "
            elif isRec then "let rec "
            else "let "

        genLetBinding { astContext with IsFirstChild = true } prefix b 
        +> colPre sepNln sepNln bs (genLetBinding { astContext with IsFirstChild = false } "and ")

    | MDInterface(t, mdo) -> 
        !- "interface " +> genType astContext false t
        +> opt sepNone mdo 
            (fun mds -> !- " with" +> indent +> genMemberDefnList { astContext with IsInterface = true } mds +> unindent)

    | MDAutoProperty(ats, px, ao, mk, e, s, _isStatic, typeOpt, memberKindToMemberFlags) ->
        let isFunctionProperty =
            match typeOpt with
            | Some (TFun _) -> true
            | _ -> false
        genPreXmlDoc px
        +> genAttributes astContext ats +> genMemberFlags astContext (memberKindToMemberFlags mk) +> str "val "
        +> opt sepSpace ao genAccess -- s +> optPre sepColon sepNone typeOpt (genType astContext false)
         +> sepEq +> genExpr astContext e -- genPropertyKind (not isFunctionProperty) mk

    | MDAbstractSlot(ats, px, ao, s, t, vi, ValTyparDecls(tds, _, tcs), MFMemberFlags mk) ->
        let (FunType namedArgs) = (t, vi)
        let isFunctionProperty =
            match t with
            | TFun _ -> true
            | _ -> false
        genPreXmlDoc px 
        +> genAttributes astContext ats
        +> opt sepSpace ao genAccess -- sprintf "abstract %s" s
        +> genTypeParam astContext tds tcs
        +> sepColon +> genTypeList astContext namedArgs -- genPropertyKind (not isFunctionProperty) mk

    | md -> failwithf "Unexpected member definition: %O" md

and genPropertyKind useSyntacticSugar = function
    | PropertyGet -> 
        // Try to use syntactic sugar on real properties (not methods in disguise)
        if useSyntacticSugar then "" else " with get"
    | PropertySet -> " with set"
    | PropertyGetSet -> " with get, set"
    | _ -> ""

and genSimplePat astContext = function
    | SPId(s, isOptArg, _) -> ifElse isOptArg (!- (sprintf "?%s" s)) (!- s)
    | SPTyped(sp, t) -> genSimplePat astContext sp +> sepColon +> genType astContext false t
    | SPAttrib(ats, sp) -> genOnelinerAttributes astContext ats +> genSimplePat astContext sp
    
and genSimplePats astContext = function
    // Remove parentheses on an extremely simple pattern
    | SimplePats [SPId _ as sp] -> genSimplePat astContext sp
    | SimplePats ps -> sepOpenT +> col sepComma ps (genSimplePat astContext) +> sepCloseT
    | SPSTyped(ps, t) -> genSimplePats astContext ps +> sepColon +> genType astContext false t

and genComplexPat astContext = function
    | CPId p -> genPat astContext p
    | CPSimpleId(s, isOptArg, _) -> ifElse isOptArg (!- (sprintf "?%s" s)) (!- s)
    | CPTyped(sp, t) -> genComplexPat astContext sp +> sepColon +> genType astContext false t
    | CPAttrib(ats, sp) -> genOnelinerAttributes astContext ats +> genComplexPat astContext sp

and genComplexPats astContext = function
    | ComplexPats [c] -> genComplexPat astContext c
    | ComplexPats ps -> sepOpenT +> col sepComma ps (genComplexPat astContext) +> sepCloseT
    | ComplexTyped(ps, t) -> genComplexPats astContext ps +> sepColon +> genType astContext false t

and genPatRecordFieldName astContext (PatRecordFieldName(s1, s2, p)) =
    ifElse (s1 = "") (!- (sprintf "%s = " s2)) (!- (sprintf "%s.%s = " s1 s2)) +> genPat astContext p

and genPatWithIdent astContext (ido, p) = 
    opt sepEq ido (!-) +> genPat astContext p

and genPat astContext = function
    | PatOptionalVal(s) -> !- (sprintf "?%s" s)
    | PatAttrib(p, ats) -> genOnelinerAttributes astContext ats +> genPat astContext p
    | PatOr(p1, p2) -> genPat astContext p1 -- " | " +> genPat astContext p2
    | PatAnds(ps) -> col (!- " & ") ps (genPat astContext)
    | PatNullary PatNull -> !- "null"
    | PatNullary PatWild -> sepWild
    | PatTyped(p, t) -> 
        // CStyle patterns only occur on extern declaration so it doesn't escalate to expressions
        // We lookup sources to get extern types since it has quite many exceptions compared to normal F# types
        let genTypeByLookup t =
            fun ctx -> 
                if ctx.Config.StrictMode then
                    genType astContext false t ctx
                else
                    match lookup t.Range ctx with
                    | Some typ ->
                        str typ ctx
                    | None ->
                        genType astContext false t ctx

        ifElse astContext.IsCStylePattern (genTypeByLookup t +> sepSpace +> genPat astContext p)
            (genPat astContext p +> sepColon +> genType astContext false t) 
    | PatNamed(ao, PatNullary PatWild, s) -> opt sepSpace ao genAccess -- s
    | PatNamed(ao, p, s) -> opt sepSpace ao genAccess +> genPat astContext p -- sprintf " as %s" s 
    | PatLongIdent(ao, s, ps, tpso) -> 
        let aoc = opt sepSpace ao genAccess
        let tpsoc = opt sepNone tpso (fun (ValTyparDecls(tds, _, tcs)) -> genTypeParam astContext tds tcs)
        // Override escaped new keyword
        let s = if s = "``new``" then "new" else s
        match ps with
        | [] ->  aoc -- s +> tpsoc
        | [(_, PatTuple [p1; p2])] when s = "(::)" -> 
            aoc +> genPat astContext p1 -- " :: " +> genPat astContext p2
        | [(ido, p) as ip] -> 
            aoc -- s +> tpsoc +> 
            ifElse (hasParenInPat p || Option.isSome ido) (ifElse (addSpaceBeforeParensInFunDef s p) sepBeforeArg sepNone) sepSpace 
            +> ifElse (Option.isSome ido) (sepOpenT +> genPatWithIdent astContext ip +> sepCloseT) (genPatWithIdent astContext ip)
        // This pattern is potentially long
        | ps -> 
            let hasBracket = ps |> Seq.map fst |> Seq.exists Option.isSome
            atCurrentColumn (aoc -- s +> tpsoc +> sepSpace 
                +> ifElse hasBracket sepOpenT sepNone 
                +> colAutoNlnSkip0 (ifElse hasBracket sepSemi sepSpace) ps (genPatWithIdent astContext)
                +> ifElse hasBracket sepCloseT sepNone)

    | PatParen(PatConst(Const "()", _)) -> !- "()"
    | PatParen(p) -> sepOpenT +> genPat astContext p +> sepCloseT
    | PatTuple ps -> 
        atCurrentColumn (colAutoNlnSkip0 sepComma ps (genPat astContext))
    | PatSeq(PatList, ps) -> 
        ifElse ps.IsEmpty (sepOpenLFixed +> sepCloseLFixed) 
            (sepOpenL +> atCurrentColumn (colAutoNlnSkip0 sepSemi ps (genPat astContext)) +> sepCloseL)

    | PatSeq(PatArray, ps) -> 
        ifElse ps.IsEmpty (sepOpenAFixed +> sepCloseAFixed)
            (sepOpenA +> atCurrentColumn (colAutoNlnSkip0 sepSemi ps (genPat astContext)) +> sepCloseA)

    | PatRecord(xs) -> 
        sepOpenS +> atCurrentColumn (colAutoNlnSkip0 sepSemi xs (genPatRecordFieldName astContext)) +> sepCloseS
    | PatConst(c) -> genConst c
    | PatIsInst(t) -> 
        // Should have brackets around in the type test patterns
        !- ":? " +> genType astContext true t
    // Quotes will be printed by inner expression
    | PatQuoteExpr e -> genExpr astContext e
    | p -> failwithf "Unexpected pattern: %O" p
