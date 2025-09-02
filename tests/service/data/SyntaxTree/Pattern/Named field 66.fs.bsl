ImplFile
  (ParsedImplFileInput
     ("/root/Pattern/Named field 66.fs", false, QualifiedNameOfFile Module, [],
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
                               (4,4--4,24),
                               As
                                 (Record
                                    ([NamePatPairField
                                        (SynLongIdent ([X], [], [None]),
                                         Some (4,12--4,13), (4,10--4,15),
                                         Const (Int32 1, (4,14--4,15)), None)],
                                     (4,8--4,17)),
                                  Named
                                    (SynIdent (res, None), false, None,
                                     (4,21--4,24)), (4,8--4,24)),
                               Some (Comma ((4,24--4,25), Some (4,25))));
                            NamePatPairField
                              (SynLongIdent ([b], [], [None]), Some (4,28--4,29),
                               (4,26--4,47),
                               As
                                 (Record
                                    ([NamePatPairField
                                        (SynLongIdent ([Y], [], [None]),
                                         Some (4,34--4,35), (4,32--4,37),
                                         Const (Int32 3, (4,36--4,37)), None)],
                                     (4,30--4,39)),
                                  Named
                                    (SynIdent (res2, None), false, None,
                                     (4,43--4,47)), (4,30--4,47)), None)],
                           (4,4--4,48), { ParenRange = (4,3--4,48) }), None,
                        (4,2--4,48)), None, Const (Unit, (4,52--4,54)),
                     (4,2--4,54), Yes, { ArrowRange = Some (4,49--4,51)
                                         BarRange = Some (4,0--4,1) })],
                 (3,0--4,54), { MatchKeyword = (3,0--3,5)
                                WithKeyword = (3,8--3,12) }), (3,0--4,54))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--4,54), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [] }, set []))
