ImplFile
  (ParsedImplFileInput
     ("/root/Pattern/Named field 76.fs", false, QualifiedNameOfFile Module, [],
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
                               (4,4--4,19),
                               LongIdent
                                 (SynLongIdent ([B], [], [None]), None, None,
                                  NamePatPairs
                                    ([NamePatPairField
                                        (SynLongIdent ([b], [], [None]),
                                         Some (4,12--4,13), (4,10--4,18),
                                         Tuple
                                           (false,
                                            [Named
                                               (SynIdent (b, None), false, None,
                                                (4,14--4,15)); Wild (4,17--4,18)],
                                            [(4,15--4,16)], (4,14--4,18)), None)],
                                     (4,10--4,18), { ParenRange = (4,9--4,19) }),
                                  None, (4,8--4,19)),
                               Some (Semicolon ((4,19--4,20), Some (4,20))))],
                           (4,4--4,22), { ParenRange = (4,3--4,23) }), None,
                        (4,2--4,23)), None, Const (Unit, (4,27--4,29)),
                     (4,2--4,29), Yes, { ArrowRange = Some (4,24--4,26)
                                         BarRange = Some (4,0--4,1) })],
                 (3,0--4,29), { MatchKeyword = (3,0--3,5)
                                WithKeyword = (3,8--3,12) }), (3,0--4,29))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--4,29), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [] }, set []))
