ImplFile
  (ParsedImplFileInput
     ("/root/Pattern/Named field 54.fs", false, QualifiedNameOfFile Module, [],
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
                               (4,19--4,32),
                               Record
                                 ([NamePatPairField
                                     (SynLongIdent ([Y], [], [None]),
                                      Some (4,27--4,28), (4,25--4,30),
                                      Const (Int32 3, (4,29--4,30)), None)],
                                  (4,23--4,32)), None)], (4,4--4,33),
                           { ParenRange = (4,3--4,33) }), None, (4,2--4,33)),
                     None, Const (Unit, (4,37--4,39)), (4,2--4,39), Yes,
                     { ArrowRange = Some (4,34--4,36)
                       BarRange = Some (4,0--4,1) })], (3,0--4,39),
                 { MatchKeyword = (3,0--3,5)
                   WithKeyword = (3,8--3,12) }), (3,0--4,39))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--4,39), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [] }, set []))
