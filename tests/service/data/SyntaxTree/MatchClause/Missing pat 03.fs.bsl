ImplFile
  (ParsedImplFileInput
     ("/root/MatchClause/Missing pat 03.fs", false, QualifiedNameOfFile Module,
      [], [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Expr
             (Match
                (Yes (3,0--3,13), Const (Unit, (3,6--3,8)),
                 [SynMatchClause
                    (As
                       (Or
                          (Wild (4,1--4,1), Wild (5,2--5,3), (4,1--5,3),
                           { BarRange = (5,0--5,1) }), Wild (5,7--5,8),
                        (5,2--5,8)), None, Const (Unit, (5,12--5,14)),
                     (5,2--5,14), Yes, { ArrowRange = Some (5,9--5,11)
                                         BarRange = Some (4,0--4,1) })],
                 (3,0--5,14), { MatchKeyword = (3,0--3,5)
                                WithKeyword = (3,9--3,13) }), (3,0--5,14))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--5,14), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))

(5,0)-(5,1) parse error Expecting pattern
