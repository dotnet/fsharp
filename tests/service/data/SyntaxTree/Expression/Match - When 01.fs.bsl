ImplFile
  (ParsedImplFileInput
     ("/root/Expression/Match - When 01.fs", false, QualifiedNameOfFile Module,
      [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Expr
             (Match
                (Yes (3,0--3,13), Const (Unit, (3,6--3,8)),
                 [SynMatchClause
                    (Wild (4,2--4,3), Some (Const (Bool true, (4,9--4,13))),
                     Const (Unit, (4,17--4,19)), (4,2--4,19), Yes,
                     { ArrowRange = Some (4,14--4,16)
                       BarRange = Some (4,0--4,1) })], (3,0--4,19),
                 { MatchKeyword = (3,0--3,5)
                   WithKeyword = (3,9--3,13) }), (3,0--4,19))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--4,19), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [] }, set []))
