ImplFile
  (ParsedImplFileInput
     ("/root/Expression/Match 02.fs", false, QualifiedNameOfFile Module, [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Expr
             (Do
                (Match
                   (Yes (4,4--4,17), Const (Unit, (4,10--4,12)),
                    [SynMatchClause
                       (Wild (5,6--5,7), None, Const (Unit, (5,11--5,13)),
                        (5,6--5,13), Yes, { ArrowRange = Some (5,8--5,10)
                                            BarRange = Some (5,4--5,5) })],
                    (4,4--5,13), { MatchKeyword = (4,4--4,9)
                                   WithKeyword = (4,13--4,17) }), (3,0--5,13)),
              (3,0--5,13))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--5,13), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [] }, set []))
