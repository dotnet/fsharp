ImplFile
  (ParsedImplFileInput
     ("/root/Pattern/Named field 75.fs", false, QualifiedNameOfFile Module, [],
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
                                  [LongIdent
                                     (SynLongIdent ([B], [], [None]), None, None,
                                      NamePatPairs
                                        ([NamePatPairField
                                            (SynLongIdent ([b], [], [None]),
                                             Some (4,12--4,13), (4,10--4,31),
                                             Tuple
                                               (false,
                                                [Named
                                                   (SynIdent (b, None), false,
                                                    None, (4,14--4,15));
                                                 LongIdent
                                                   (SynLongIdent
                                                      ([Some], [], [None]), None,
                                                    None,
                                                    Pats
                                                      [Record
                                                         ([NamePatPairField
                                                             (SynLongIdent
                                                                ([C], [], [None]),
                                                              Some (4,26--4,27),
                                                              (4,24--4,29),
                                                              Const
                                                                (Int32 2,
                                                                 (4,28--4,29)),
                                                              None)],
                                                          (4,22--4,31))], None,
                                                    (4,17--4,31))],
                                                [(4,15--4,16)], (4,14--4,31)),
                                             None)], (4,10--4,31),
                                         { ParenRange = (4,9--4,32) }), None,
                                      (4,8--4,32)); Wild (4,34--4,35)],
                                  [(4,32--4,33)], (4,8--4,35)), None)],
                           (4,4--4,35), { ParenRange = (4,3--4,36) }), None,
                        (4,2--4,36)), None, Const (Unit, (4,40--4,42)),
                     (4,2--4,42), Yes, { ArrowRange = Some (4,37--4,39)
                                         BarRange = Some (4,0--4,1) })],
                 (3,0--4,42), { MatchKeyword = (3,0--3,5)
                                WithKeyword = (3,8--3,12) }), (3,0--4,42))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--4,42), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [] }, set []))
