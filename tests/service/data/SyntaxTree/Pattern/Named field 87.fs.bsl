ImplFile
  (ParsedImplFileInput
     ("/root/Pattern/Named field 87.fs", false, QualifiedNameOfFile Module, [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Expr
             (Match
                (Yes (3,0--3,12), Ident r,
                 [SynMatchClause
                    (LongIdent
                       (SynLongIdent ([R], [], [None]), None, None,
                        NamePatPairs
                          ([NamePatPairField
                              (SynLongIdent ([a], [], [None]), Some (5,6--5,7),
                               (5,4--5,9),
                               Named
                                 (SynIdent (x, None), false, None, (5,8--5,9)),
                               Some (Comma ((5,9--5,10), Some (5,10))));
                            NamePatPairField
                              (SynLongIdent ([b], [], [None]), Some (6,6--6,7),
                               (6,4--6,9),
                               Named
                                 (SynIdent (y, None), false, None, (6,8--6,9)),
                               None)], (5,4--7,3), { ParenRange = (4,3--7,3) }),
                        None, (4,2--7,3)), None, Const (Unit, (7,7--7,9)),
                     (4,2--7,9), Yes, { ArrowRange = Some (7,4--7,6)
                                        BarRange = Some (4,0--4,1) })],
                 (3,0--7,9), { MatchKeyword = (3,0--3,5)
                               WithKeyword = (3,8--3,12) }), (3,0--7,9))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--7,9), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [] }, set []))
