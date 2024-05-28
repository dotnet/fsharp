ImplFile
  (ParsedImplFileInput
     ("/root/MatchClause/Missing pat 01.fs", false, QualifiedNameOfFile Module,
      [], [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Expr
             (Match
                (Yes (3,0--3,13), Const (Unit, (3,6--3,8)),
                 [SynMatchClause
                    (Wild (4,2--4,3), None, Const (Unit, (4,7--4,9)), (4,2--4,9),
                     Yes, { ArrowRange = Some (4,4--4,6)
                            BarRange = Some (4,0--4,1) });
                  SynMatchClause
                    (Or
                       (Wild (5,1--5,1), Wild (6,2--6,3), (5,1--6,3),
                        { BarRange = (6,0--6,1) }), None,
                     Const (Unit, (6,7--6,9)), (6,2--6,9), Yes,
                     { ArrowRange = Some (6,4--6,6)
                       BarRange = Some (5,0--5,1) })], (3,0--6,9),
                 { MatchKeyword = (3,0--3,5)
                   WithKeyword = (3,9--3,13) }), (3,0--6,9))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--6,9), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))

(6,0)-(6,1) parse error Expecting pattern
