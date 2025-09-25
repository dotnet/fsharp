ImplFile
  (ParsedImplFileInput
     ("/root/Pattern/Record 07.fs", false, QualifiedNameOfFile Module, [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Expr
             (Match
                (Yes (3,0--3,13), Const (Unit, (3,6--3,8)),
                 [SynMatchClause
                    (Record
                       ([NamePatPairField
                           (SynLongIdent ([A], [], [None]), Some (4,6--4,7),
                            (4,4--4,9), Const (Int32 1, (4,8--4,9)),
                            Some ((4,9--4,10), Some (4,10)));
                         NamePatPairField
                           (SynLongIdent ([B], [], [None]), Some (4,13--4,14),
                            (4,11--4,16), Const (Int32 2, (4,15--4,16)),
                            Some ((4,16--4,17), Some (4,17)));
                         NamePatPairField
                           (SynLongIdent ([C], [], [None]), Some (4,20--4,21),
                            (4,18--4,23), Const (Int32 3, (4,22--4,23)), None)],
                        (4,2--4,25)), None, Const (Unit, (4,29--4,31)),
                     (4,2--4,31), Yes, { ArrowRange = Some (4,26--4,28)
                                         BarRange = Some (4,0--4,1) })],
                 (3,0--4,31), { MatchKeyword = (3,0--3,5)
                                WithKeyword = (3,9--3,13) }), (3,0--4,31))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--4,31), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [] }, set []))
