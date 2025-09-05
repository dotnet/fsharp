ImplFile
  (ParsedImplFileInput
     ("/root/Pattern/Named field 56.fs", false, QualifiedNameOfFile Module, [],
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
                               (4,4--4,28),
                               Tuple
                                 (false,
                                  [Record
                                     ([NamePatPairField
                                         (SynLongIdent ([X], [], [None]),
                                          Some (4,12--4,13), (4,10--4,15),
                                          Const (Int32 1, (4,14--4,15)), None)],
                                      (4,8--4,17));
                                   Record
                                     ([NamePatPairField
                                         (SynLongIdent ([X], [], [None]),
                                          Some (4,23--4,24), (4,21--4,26),
                                          Const (Int32 1, (4,25--4,26)), None)],
                                      (4,19--4,28))], [(4,17--4,18)],
                                  (4,8--4,28)),
                               Some (Comma ((4,28--4,29), Some (4,29))));
                            NamePatPairField
                              (SynLongIdent ([b], [], [None]), Some (4,32--4,33),
                               (4,30--4,43),
                               Record
                                 ([NamePatPairField
                                     (SynLongIdent ([X], [], [None]),
                                      Some (4,38--4,39), (4,36--4,41),
                                      Const (Int32 1, (4,40--4,41)), None)],
                                  (4,34--4,43)), None)], (4,4--4,44),
                           { ParenRange = (4,3--4,44) }), None, (4,2--4,44)),
                     None, Const (Unit, (4,48--4,50)), (4,2--4,50), Yes,
                     { ArrowRange = Some (4,45--4,47)
                       BarRange = Some (4,0--4,1) })], (3,0--4,50),
                 { MatchKeyword = (3,0--3,5)
                   WithKeyword = (3,8--3,12) }), (3,0--4,50))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--4,50), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [] }, set []))
