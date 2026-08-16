ImplFile
  (ParsedImplFileInput
     ("/root/Expression/If 13.fs", false, QualifiedNameOfFile Module, [],
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
                                  ([op_Equality], [],
                                   [Some (OriginalNotation "=")]), None,
                                (4,12--4,13)), Const (Bool true, (4,7--4,11)),
                             (4,7--4,13)),
                          ArbitraryAfterError
                            ("declExprInfixEquals2", (4,13--4,13)), (4,7--4,13)),
                       ArbitraryAfterError ("if4", (4,13--4,13)), None,
                       Yes (4,4--4,13), true, (4,4--4,13),
                       { IfKeyword = (4,4--4,6)
                         IsElif = false
                         ThenKeyword = (4,13--4,13)
                         ElseKeyword = None
                         IfToThenRange = (4,4--4,13) }),
                    Const (Unit, (6,4--6,6)), (4,4--6,6),
                    { SeparatorRange = None }), (3,0--6,6)), (3,0--6,6))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--6,6), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [] }, set []))

(4,12)-(4,13) parse error Unexpected token '=' or incomplete expression
(4,4)-(4,6) parse error Incomplete conditional. Expected 'if <expr> then <expr>' or 'if <expr> then <expr> else <expr>'.
