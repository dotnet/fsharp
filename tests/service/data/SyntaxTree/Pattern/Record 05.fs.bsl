ImplFile
  (ParsedImplFileInput
     ("/root/Pattern/Record 05.fs", false, QualifiedNameOfFile Module, [], [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Expr
             (Match
                (Yes (3,0--3,13), Const (Unit, (3,6--3,8)),
                 [SynMatchClause
                    (Record ([], (4,2--4,5)), None, Const (Unit, (4,9--4,11)),
                     (4,2--4,11), Yes, { ArrowRange = Some (4,6--4,8)
                                         BarRange = Some (4,0--4,1) })],
                 (3,0--4,11), { MatchKeyword = (3,0--3,5)
                                WithKeyword = (3,9--3,13) }), (3,0--4,11))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--4,11), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))

(4,4)-(4,5) parse error Unexpected symbol '}' in pattern. Expected identifier, 'global' or other token.
