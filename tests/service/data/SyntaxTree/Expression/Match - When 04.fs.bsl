ImplFile
  (ParsedImplFileInput
     ("/root/Expression/Match - When 04.fs", false, QualifiedNameOfFile Module,
      [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Expr
             (Match
                (Yes (3,0--3,13), Const (Unit, (3,6--3,8)),
                 [SynMatchClause
                    (Named (SynIdent (a, None), false, None, (4,2--4,3)), None,
                     ArbitraryAfterError ("patternClauses5", (4,8--4,8)),
                     (4,2--4,8), Yes, { ArrowRange = None
                                        BarRange = Some (4,0--4,1) })],
                 (3,0--5,1), { MatchKeyword = (3,0--3,5)
                               WithKeyword = (3,9--3,13) }), (3,0--5,1));
           Expr (Const (Unit, (7,0--7,2)), (7,0--7,2))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--7,2), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [] }, set []))

(4,4)-(4,8) parse error Expecting expression
