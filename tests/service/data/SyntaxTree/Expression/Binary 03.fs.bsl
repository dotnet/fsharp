ImplFile
  (ParsedImplFileInput
     ("/root/Expression/Binary 03.fs", false, QualifiedNameOfFile Module, [], [],
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
                                (4,9--4,10)), Const (Int32 1, (4,7--4,8)),
                             (4,7--4,10)),
                          ArbitraryAfterError
                            ("declExprInfixEquals2", (4,10--4,10)), (4,7--4,10)),
                       ArbitraryAfterError ("if1", (4,10--4,10)), None,
                       Yes (4,4--4,10), true, (4,4--4,10),
                       { IfKeyword = (4,4--4,6)
                         IsElif = false
                         ThenKeyword = (4,10--4,10)
                         ElseKeyword = None
                         IfToThenRange = (4,4--4,10) }),
                    Const (Unit, (6,4--6,6)), (4,4--6,6),
                    { SeparatorRange = None }), (3,0--6,6)), (3,0--6,6))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--6,6), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))

(4,9)-(4,10) parse error Unexpected token '=' or incomplete expression
(4,11)-(6,4) parse error Incomplete structured construct at or before this point in expression
(4,4)-(4,6) parse error Incomplete conditional. Expected 'if <expr> then <expr>' or 'if <expr> then <expr> else <expr>'.
