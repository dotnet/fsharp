ImplFile
  (ParsedImplFileInput
     ("/root/MatchClause/Missing expr 03.fs", false, QualifiedNameOfFile Module,
      [], [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Expr
             (Match
                (Yes (3,0--3,13), Const (Unit, (3,6--3,8)),
                 [SynMatchClause
                    (Wild (4,2--4,3), None,
                     ArbitraryAfterError
                       ("typedSequentialExprBlockR1", (4,6--4,6)), (4,2--4,6),
                     Yes, { ArrowRange = Some (4,4--4,6)
                            BarRange = Some (4,0--4,1) });
                  SynMatchClause
                    (Wild (5,2--5,3), None, Const (Unit, (5,7--5,9)), (5,2--5,9),
                     Yes, { ArrowRange = Some (5,4--5,6)
                            BarRange = Some (5,0--5,1) })], (3,0--5,9),
                 { MatchKeyword = (3,0--3,5)
                   WithKeyword = (3,9--3,13) }), (3,0--5,9))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--5,9), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))

(5,0)-(5,1) parse error Unexpected symbol '|' in pattern matching
