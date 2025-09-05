ImplFile
  (ParsedImplFileInput
     ("/root/Pattern/Named field 55.fs", false, QualifiedNameOfFile Module, [],
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
                               (4,4--4,17),
                               Record
                                 ([NamePatPairField
                                     (SynLongIdent ([X], [], [None]),
                                      Some (4,12--4,13), (4,10--4,15),
                                      Const (Int32 1, (4,14--4,15)), None)],
                                  (4,8--4,17)),
                               Some (Comma ((4,17--4,18), Some (4,18))));
                            NamePatPairField
                              (SynLongIdent ([b], [], [None]), Some (4,21--4,22),
                               (4,19--4,24), Wild (4,23--4,24), None)],
                           (4,4--4,25), { ParenRange = (4,3--4,25) }), None,
                        (4,2--4,25)), None, Const (Unit, (4,29--4,31)),
                     (4,2--4,31), Yes, { ArrowRange = Some (4,26--4,28)
                                         BarRange = Some (4,0--4,1) })],
                 (3,0--4,31), { MatchKeyword = (3,0--3,5)
                                WithKeyword = (3,8--3,12) }), (3,0--4,31))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--4,31), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [] }, set []))
