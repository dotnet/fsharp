ImplFile
  (ParsedImplFileInput
     ("/root/Pattern/Named field 31.fs", false, QualifiedNameOfFile Module, [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Expr
             (TryWith
                (Const (Unit, (4,2--4,4)),
                 [SynMatchClause
                    (LongIdent
                       (SynLongIdent ([A], [], [None]), None, None,
                        NamePatPairs
                          ([NamePatPairField
                              (SynLongIdent ([a], [], [None]), Some (6,6--6,7),
                               (6,4--6,9),
                               Named
                                 (SynIdent (a, None), false, None, (6,8--6,9)),
                               Some (Comma ((6,9--6,10), Some (6,10))));
                            NamePatPairField
                              (SynLongIdent ([b], [], [None]), Some (6,13--6,14),
                               (6,11--6,16),
                               Named
                                 (SynIdent (b, None), false, None, (6,15--6,16)),
                               Some (Comma ((6,16--6,17), Some (6,17))));
                            NamePatPairField
                              (SynLongIdent ([c], [], [None]), Some (6,20--6,21),
                               (6,18--6,23),
                               Named
                                 (SynIdent (c, None), false, None, (6,22--6,23)),
                               None)], (6,4--6,24), { ParenRange = (6,3--6,24) }),
                        None, (6,2--6,24)), None, Const (Int32 2, (6,28--6,29)),
                     (6,2--6,29), Yes, { ArrowRange = Some (6,25--6,27)
                                         BarRange = Some (6,0--6,1) })],
                 (3,0--6,29), Yes (3,0--3,3), Yes (5,0--5,4),
                 { TryKeyword = (3,0--3,3)
                   TryToWithRange = (3,0--5,4)
                   WithKeyword = (5,0--5,4)
                   WithToEndRange = (5,0--6,29) }), (3,0--6,29))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--6,29), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [] }, set []))
