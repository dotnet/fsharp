ImplFile
  (ParsedImplFileInput
     ("/root/Pattern/Record 02.fs", false, QualifiedNameOfFile Module, [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Expr
             (Match
                (Yes (3,0--3,13), Const (Unit, (3,6--3,8)),
                 [SynMatchClause
                    (Record
                       ([(([], A), Some (4,6--4,7), Wild (4,8--4,9));
                         (([], B), Some (4,13--4,14), Wild (4,15--4,16))],
                        (4,2--4,18)), None, Const (Unit, (4,22--4,24)),
                     (4,2--4,24), Yes, { ArrowRange = Some (4,19--4,21)
                                         BarRange = Some (4,0--4,1) })],
                 (3,0--4,24), { MatchKeyword = (3,0--3,5)
                                WithKeyword = (3,9--3,13) }), (3,0--4,24))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--4,24), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [] }, set []))
