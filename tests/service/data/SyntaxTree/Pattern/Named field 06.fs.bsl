ImplFile
  (ParsedImplFileInput
     ("/root/Pattern/Named field 06.fs", false, QualifiedNameOfFile Module, [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Expr
             (Match
                (Yes (3,0--3,12), Const (Int32 1, (3,6--3,7)),
                 [SynMatchClause
                    (LongIdent
                       (SynLongIdent ([A], [], [None]), None, None,
                        NamePatPairs
                          ([NamePatPairField
                              (SynLongIdent ([a], [], [None]), Some (4,6--4,7),
                               (4,4--4,9),
                               Named
                                 (SynIdent (a, None), false, None, (4,8--4,9)),
                               Some ((4,9--4,10), Some (4,10)));
                            NamePatPairField
                              (SynLongIdent ([b], [], [None]), Some (4,13--4,14),
                               (4,11--4,16),
                               Named
                                 (SynIdent (b, None), false, None, (4,15--4,16)),
                               Some ((4,16--4,17), Some (4,17)));
                            NamePatPairField
                              (SynLongIdent ([c], [], [None]), Some (4,20--4,21),
                               (4,18--4,23),
                               Named
                                 (SynIdent (c, None), false, None, (4,22--4,23)),
                               None)], (4,4--4,24), { ParenRange = (4,3--4,24) }),
                        None, (4,2--4,24)), None, Const (Int32 2, (4,28--4,29)),
                     (4,2--4,29), Yes, { ArrowRange = Some (4,25--4,27)
                                         BarRange = Some (4,0--4,1) })],
                 (3,0--4,29), { MatchKeyword = (3,0--3,5)
                                WithKeyword = (3,8--3,12) }), (3,0--4,29))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--4,29), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [] }, set []))
