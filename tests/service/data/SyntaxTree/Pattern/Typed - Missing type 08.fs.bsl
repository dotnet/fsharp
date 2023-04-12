ImplFile
  (ParsedImplFileInput
     ("/root/Pattern/Typed - Missing type 08.fs", false,
      QualifiedNameOfFile Module, [], [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Expr
             (Match
                (Yes (3,0--3,13), Const (Unit, (3,6--3,8)),
                 [SynMatchClause
                    (Typed
                       (Named (SynIdent (i, None), false, None, (4,2--4,3)),
                        FromParseError (4,4--4,4), (4,2--4,4)), None,
                     Const (Unit, (4,8--4,10)), (4,2--4,10), Yes,
                     { ArrowRange = Some (4,5--4,7)
                       BarRange = Some (4,0--4,1) })], (3,0--4,10),
                 { MatchKeyword = (3,0--3,5)
                   WithKeyword = (3,9--3,13) }), (3,0--4,10))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--4,10), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))

(4,5)-(4,7) parse error Unexpected symbol '->' in pattern
