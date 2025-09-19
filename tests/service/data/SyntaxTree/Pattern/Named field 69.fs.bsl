ImplFile
  (ParsedImplFileInput
     ("/root/Pattern/Named field 69.fs", false, QualifiedNameOfFile Module, [],
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
                               (4,4--4,35),
                               Tuple
                                 (false,
                                  [As
                                     (Record
                                        ([NamePatPairField
                                            (SynLongIdent ([X], [], [None]),
                                             Some (4,12--4,13), (4,10--4,15),
                                             Const (Int32 1, (4,14--4,15)), None)],
                                         (4,8--4,17)),
                                      Named
                                        (SynIdent (res, None), false, None,
                                         (4,21--4,24)), (4,8--4,24));
                                   Record
                                     ([NamePatPairField
                                         (SynLongIdent ([X], [], [None]),
                                          Some (4,30--4,31), (4,28--4,33),
                                          Const (Int32 1, (4,32--4,33)), None)],
                                      (4,26--4,35))], [(4,24--4,25)],
                                  (4,8--4,35)),
                               Some (Comma ((4,35--4,36), Some (4,36))));
                            NamePatPairField
                              (SynLongIdent ([b], [], [None]), Some (4,39--4,40),
                               (4,37--4,69),
                               Tuple
                                 (false,
                                  [Record
                                     ([NamePatPairField
                                         (SynLongIdent ([X], [], [None]),
                                          Some (4,45--4,46), (4,43--4,48),
                                          Const (Int32 1, (4,47--4,48)), None)],
                                      (4,41--4,50));
                                   As
                                     (Record
                                        ([NamePatPairField
                                            (SynLongIdent ([Y], [], [None]),
                                             Some (4,56--4,57), (4,54--4,59),
                                             Const (Int32 3, (4,58--4,59)), None)],
                                         (4,52--4,61)),
                                      Named
                                        (SynIdent (res2, None), false, None,
                                         (4,65--4,69)), (4,52--4,69))],
                                  [(4,50--4,51)], (4,41--4,69)), None)],
                           (4,4--4,69), { ParenRange = (4,3--4,70) }), None,
                        (4,2--4,70)), None, Const (Unit, (4,74--4,76)),
                     (4,2--4,76), Yes, { ArrowRange = Some (4,71--4,73)
                                         BarRange = Some (4,0--4,1) })],
                 (3,0--4,76), { MatchKeyword = (3,0--3,5)
                                WithKeyword = (3,8--3,12) }), (3,0--4,76))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--4,76), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [] }, set []))
