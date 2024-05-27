ImplFile
  (ParsedImplFileInput
     ("/root/Expression/Binary 04.fs", false, QualifiedNameOfFile Module, [], [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Expr
             (Do
                (Sequential
                   (SuppressNeither, true,
                    IfThenElse
                      (App
                         (NonAtomic, false,
                          App
                            (NonAtomic, true,
                             LongIdent
                               (false,
                                SynLongIdent
                                  ([op_EqualsEquals], [],
                                   [Some (OriginalNotation "==")]), None,
                                (4,9--4,11)), Const (Int32 1, (4,7--4,8)),
                             (4,7--4,11)),
                          ArbitraryAfterError ("declExprInfix2", (4,11--4,11)),
                          (4,7--4,11)),
                       ArbitraryAfterError ("if1", (4,11--4,11)), None,
                       Yes (4,4--4,11), true, (4,4--4,11),
                       { IfKeyword = (4,4--4,6)
                         IsElif = false
                         ThenKeyword = (4,11--4,11)
                         ElseKeyword = None
                         IfToThenRange = (4,4--4,11) }),
                    Const (Unit, (6,4--6,6)), (4,4--6,6),
                    { SeparatorRange = None }), (3,0--6,6)), (3,0--6,6))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--6,6), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))

(6,4)-(6,5) parse error Unexpected syntax or possible incorrect indentation: this token is offside of context started at position (4:5). Try indenting this further.
To continue using non-conforming indentation, pass the '--strict-indentation-' flag to the compiler, or set the language version to F# 7.
(4,9)-(4,11) parse error Unexpected token '==' or incomplete expression
(4,12)-(6,4) parse error Incomplete structured construct at or before this point in expression
(4,4)-(4,6) parse error Incomplete conditional. Expected 'if <expr> then <expr>' or 'if <expr> then <expr> else <expr>'.
