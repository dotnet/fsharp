ImplFile
  (ParsedImplFileInput
     ("/root/Expression/If 11.fs", false, QualifiedNameOfFile Module, [],
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
                                  ([op_BooleanAnd], [],
                                   [Some (OriginalNotation "&&")]), None,
                                (4,12--4,14)), Const (Bool true, (4,7--4,11)),
                             (4,7--4,14)),
                          ArbitraryAfterError
                            ("declExprInfixAmpAmp2", (4,14--4,14)), (4,7--4,14)),
                       ArbitraryAfterError ("if4", (4,14--4,14)), None,
                       Yes (4,4--4,14), true, (4,4--4,14),
                       { IfKeyword = (4,4--4,6)
                         IsElif = false
                         ThenKeyword = (4,14--4,14)
                         ElseKeyword = None
                         IfToThenRange = (4,4--4,14) }),
                    Const (Unit, (6,4--6,6)), (4,4--6,6),
                    { SeparatorRange = None }), (3,0--6,6)), (3,0--6,6))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--6,6), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [] }, set []))

(6,4)-(6,5) parse error Unexpected syntax or possible incorrect indentation: this token is offside of context started at position (4:5). Try indenting this further.
To continue using non-conforming indentation, pass the '--strict-indentation-' flag to the compiler, or set the language version to F# 7.
(4,12)-(4,14) parse error Unexpected token '&&' or incomplete expression
(4,4)-(4,6) parse error Incomplete conditional. Expected 'if <expr> then <expr>' or 'if <expr> then <expr> else <expr>'.
