ImplFile
  (ParsedImplFileInput
     ("/root/Pattern/Named field 58.fs", false, QualifiedNameOfFile Module, [],
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
                               ListCons
                                 (Named
                                    (SynIdent (h, None), false, None, (4,8--4,9)),
                                  Named
                                    (SynIdent (tail, None), false, None,
                                     (4,13--4,17)), (4,8--4,17),
                                  { ColonColonRange = (4,10--4,12) }),
                               Some (Semicolon ((4,17--4,18), Some (4,18))));
                            NamePatPairField
                              (SynLongIdent ([b], [], [None]), Some (4,21--4,22),
                               (4,19--4,31),
                               ListCons
                                 (Named
                                    (SynIdent (h, None), false, None,
                                     (4,23--4,24)),
                                  Named
                                    (SynIdent (tail, None), false, None,
                                     (4,27--4,31)), (4,23--4,31),
                                  { ColonColonRange = (4,24--4,26) }), None)],
                           (4,4--4,32), { ParenRange = (4,3--4,32) }), None,
                        (4,2--4,32)), None, Const (Unit, (4,36--4,38)),
                     (4,2--4,38), Yes, { ArrowRange = Some (4,33--4,35)
                                         BarRange = Some (4,0--4,1) })],
                 (3,0--4,38), { MatchKeyword = (3,0--3,5)
                                WithKeyword = (3,8--3,12) }), (3,0--4,38))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--4,38), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [] }, set []))
