ImplFile
  (ParsedImplFileInput
     ("/root/Pattern/Named field 60.fs", false, QualifiedNameOfFile Module, [],
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
                               (4,4--4,14),
                               ListCons
                                 (Named
                                    (SynIdent (h, None), false, None, (4,8--4,9)),
                                  Wild (4,13--4,14), (4,8--4,14),
                                  { ColonColonRange = (4,10--4,12) }),
                               Some (Semicolon ((4,14--4,15), Some (4,15))));
                            NamePatPairField
                              (SynLongIdent ([b], [], [None]), Some (4,18--4,19),
                               (4,16--4,28),
                               ListCons
                                 (Wild (4,20--4,21),
                                  Named
                                    (SynIdent (tail, None), false, None,
                                     (4,24--4,28)), (4,20--4,28),
                                  { ColonColonRange = (4,21--4,23) }), None)],
                           (4,4--4,29), { ParenRange = (4,3--4,29) }), None,
                        (4,2--4,29)), None, Const (Unit, (4,33--4,35)),
                     (4,2--4,35), Yes, { ArrowRange = Some (4,30--4,32)
                                         BarRange = Some (4,0--4,1) })],
                 (3,0--4,35), { MatchKeyword = (3,0--3,5)
                                WithKeyword = (3,8--3,12) }), (3,0--4,35))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--4,35), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [] }, set []))
