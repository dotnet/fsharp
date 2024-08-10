ImplFile
  (ParsedImplFileInput
     ("/root/Pattern/As 08.fs", false, QualifiedNameOfFile Module, [], [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Expr
             (Match
                (Yes (3,0--3,13), Const (Unit, (3,6--3,8)),
                 [SynMatchClause
                    (Or
                       (As (Wild (4,2--4,3), Wild (4,6--4,6), (4,2--4,6)),
                        Wild (5,2--5,3), (4,2--5,3), { BarRange = (5,0--5,1) }),
                     None, Const (Unit, (5,7--5,9)), (4,2--5,9), Yes,
                     { ArrowRange = Some (5,4--5,6)
                       BarRange = Some (4,0--4,1) })], (3,0--5,9),
                 { MatchKeyword = (3,0--3,5)
                   WithKeyword = (3,9--3,13) }), (3,0--5,9))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--5,9), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))

(4,4)-(4,6) parse error Expecting pattern
