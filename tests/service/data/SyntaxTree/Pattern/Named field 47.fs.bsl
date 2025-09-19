ImplFile
  (ParsedImplFileInput
     ("/root/Pattern/Named field 47.fs", false, QualifiedNameOfFile Module, [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Expr
             (Match
                (Yes (3,0--3,12), Const (Int32 1, (3,6--3,7)),
                 [SynMatchClause
                    (LongIdent
                       (SynLongIdent ([CaseA], [], [None]), None, None,
                        NamePatPairs
                          ([NamePatPairField
                              (SynLongIdent ([x], [], [None]), Some (4,10--4,11),
                               (4,8--4,16),
                               Tuple
                                 (false, [Wild (4,12--4,13); Wild (4,15--4,16)],
                                  [(4,13--4,14)], (4,12--4,16)),
                               Some (Comma ((4,16--4,17), Some (4,17))));
                            NamePatPairField
                              (SynLongIdent ([y], [], [None]), Some (4,20--4,21),
                               (4,18--4,26),
                               Tuple
                                 (false, [Wild (4,22--4,23); Wild (4,25--4,26)],
                                  [(4,23--4,24)], (4,22--4,26)), None)],
                           (4,8--4,26), { ParenRange = (4,7--4,27) }), None,
                        (4,2--4,27)), None, Const (Unit, (4,31--4,33)),
                     (4,2--4,33), Yes, { ArrowRange = Some (4,28--4,30)
                                         BarRange = Some (4,0--4,1) })],
                 (3,0--4,33), { MatchKeyword = (3,0--3,5)
                                WithKeyword = (3,8--3,12) }), (3,0--4,33))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--4,33), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [] }, set []))
