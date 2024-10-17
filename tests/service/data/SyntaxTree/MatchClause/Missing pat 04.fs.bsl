ImplFile
  (ParsedImplFileInput
     ("/root/MatchClause/Missing pat 04.fs", false, QualifiedNameOfFile Module,
      [], [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Expr
             (Match
                (Yes (3,0--3,13), Const (Unit, (3,6--3,8)),
                 [SynMatchClause
                    (As
                       (Or
                          (Wild (4,1--4,1),
                           Or
                             (As (Wild (5,2--5,3), Wild (5,7--5,8), (5,2--5,8)),
                              Wild (6,2--6,3), (5,2--6,3),
                              { BarRange = (6,0--6,1) }), (4,1--6,3),
                           { BarRange = (5,0--5,1) }), Wild (6,7--6,8),
                        (5,2--6,8)), None, Const (Unit, (6,12--6,14)),
                     (5,2--6,14), Yes, { ArrowRange = Some (6,9--6,11)
                                         BarRange = Some (4,0--4,1) })],
                 (3,0--6,14), { MatchKeyword = (3,0--3,5)
                                WithKeyword = (3,9--3,13) }), (3,0--6,14))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--6,14), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))

(5,0)-(5,1) parse error Expecting pattern
