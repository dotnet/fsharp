ImplFile
  (ParsedImplFileInput
     ("/root/Pattern/Named field 45.fs", false, QualifiedNameOfFile Module, [],
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
                              (SynLongIdent ([x], [], [None]), Some (6,6--6,7),
                               (6,4--6,12),
                               Tuple
                                 (false,
                                  [Named
                                     (SynIdent (a, None), false, None,
                                      (6,8--6,9));
                                   Named
                                     (SynIdent (b, None), false, None,
                                      (6,11--6,12))], [(6,9--6,10)], (6,8--6,12)),
                               Some (Comma ((6,12--6,13), Some (6,13))));
                            NamePatPairField
                              (SynLongIdent ([y], [], [None]), Some (6,16--6,17),
                               (6,14--6,22),
                               Tuple
                                 (false,
                                  [Named
                                     (SynIdent (f, None), false, None,
                                      (6,18--6,19));
                                   Named
                                     (SynIdent (g, None), false, None,
                                      (6,21--6,22))], [(6,19--6,20)],
                                  (6,18--6,22)), None)], (6,4--6,22),
                           { ParenRange = (6,3--6,23) }), None, (6,2--6,23)),
                     None, Const (Unit, (6,27--6,29)), (6,2--6,29), Yes,
                     { ArrowRange = Some (6,24--6,26)
                       BarRange = Some (6,0--6,1) })], (3,0--6,29),
                 Yes (3,0--3,3), Yes (5,0--5,4),
                 { TryKeyword = (3,0--3,3)
                   TryToWithRange = (3,0--5,4)
                   WithKeyword = (5,0--5,4)
                   WithToEndRange = (5,0--6,29) }), (3,0--6,29))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--6,29), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [] }, set []))
