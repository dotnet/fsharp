ImplFile
  (ParsedImplFileInput
     ("/root/Expression/Match - When 02.fs", false, QualifiedNameOfFile Module,
      [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Expr
             (Match
                (Yes (3,0--3,13), Const (Unit, (3,6--3,8)),
                 [SynMatchClause
                    (Wild (4,2--4,3), None, Const (Unit, (4,12--4,14)),
                     (4,2--4,14), Yes, { ArrowRange = Some (4,9--4,11)
                                         BarRange = Some (4,0--4,1) })],
                 (3,0--4,14), { MatchKeyword = (3,0--3,5)
                                WithKeyword = (3,9--3,13) }), (3,0--4,14))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--4,14), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [] }, set []))

(4,4)-(4,8) parse error Expecting expression
