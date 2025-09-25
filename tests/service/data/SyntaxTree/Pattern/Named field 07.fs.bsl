ImplFile
  (ParsedImplFileInput
     ("/root/Pattern/Named field 07.fs", false, QualifiedNameOfFile Module, [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Expr
             (Match
                (Yes (3,0--3,20),
                 Record
                   (None, None,
                    [SynExprRecordField
                       ((SynLongIdent ([A], [], [None]), true),
                        Some (3,10--3,11), Some (Const (Int32 1, (3,12--3,13))),
                        (3,8--3,13), None)], (3,6--3,15)),
                 [SynMatchClause
                    (Record
                       ([NamePatPairField
                           (SynLongIdent
                              ([Foo; Bar; A], [(4,7--4,8); (4,11--4,12)],
                               [None; None; None]), Some (4,14--4,15),
                            (4,4--4,17), Const (Int32 1, (4,16--4,17)),
                            Some ((4,17--4,18), Some (4,18)));
                         NamePatPairField
                           (SynLongIdent ([B], [], [None]), Some (4,21--4,22),
                            (4,19--4,24), Const (Int32 2, (4,23--4,24)),
                            Some ((4,24--4,25), Some (4,25)));
                         NamePatPairField
                           (SynLongIdent ([C], [], [None]), Some (4,28--4,29),
                            (4,26--4,31), Const (Int32 3, (4,30--4,31)), None)],
                        (4,2--4,33)), None, Const (Unit, (4,37--4,39)),
                     (4,2--4,39), Yes, { ArrowRange = Some (4,34--4,36)
                                         BarRange = Some (4,0--4,1) })],
                 (3,0--4,39), { MatchKeyword = (3,0--3,5)
                                WithKeyword = (3,16--3,20) }), (3,0--4,39))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--4,39), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [] }, set []))
