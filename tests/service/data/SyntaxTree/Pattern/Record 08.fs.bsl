ImplFile
  (ParsedImplFileInput
     ("/root/Pattern/Record 08.fs", false, QualifiedNameOfFile Module, [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Expr
             (Match
                (Yes (3,0--3,13), Const (Unit, (3,6--3,8)),
                 [SynMatchClause
                    (Record ([], (4,2--4,25)), None, Const (Unit, (4,29--4,31)),
                     (4,2--4,31), Yes, { ArrowRange = Some (4,26--4,28)
                                         BarRange = Some (4,0--4,1) })],
                 (3,0--4,31), { MatchKeyword = (3,0--3,5)
                                WithKeyword = (3,9--3,13) }), (3,0--4,31))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--4,31), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [] }, set []))

(4,13)-(4,14) parse error Unexpected symbol '=' in pattern. Expected '}' or other token.
