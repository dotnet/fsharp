ImplFile
  (ParsedImplFileInput
     ("/root/Pattern/Record 06.fs", false, QualifiedNameOfFile Module, [], [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Expr
             (Match
                (Yes (3,0--3,13), Const (Unit, (3,6--3,8)),
                 [SynMatchClause
                    (Record ([], (4,2--4,7)), None, Const (Unit, (4,11--4,13)),
                     (4,2--4,13), Yes, { ArrowRange = Some (4,8--4,10)
                                         BarRange = Some (4,0--4,1) })],
                 (3,0--4,13), { MatchKeyword = (3,0--3,5)
                                WithKeyword = (3,9--3,13) }), (3,0--4,13))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--4,13), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))

(4,4)-(4,5) parse error Unexpected symbol '_' in pattern. Expected identifier, 'global' or other token.
