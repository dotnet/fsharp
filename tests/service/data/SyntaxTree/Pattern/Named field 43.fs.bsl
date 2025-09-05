ImplFile
  (ParsedImplFileInput
     ("/root/Pattern/Named field 43.fs", false, QualifiedNameOfFile Module, [],
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
                               (4,8--4,18),
                               Paren
                                 (Tuple
                                    (false,
                                     [Named
                                        (SynIdent (a, None), false, None,
                                         (4,13--4,14));
                                      Named
                                        (SynIdent (b, None), false, None,
                                         (4,16--4,17))], [(4,14--4,15)],
                                     (4,13--4,17)), (4,12--4,18)),
                               Some (Comma ((4,18--4,19), Some (4,19))));
                            NamePatPairField
                              (SynLongIdent ([y], [], [None]), Some (4,22--4,23),
                               (4,20--4,28),
                               Tuple
                                 (false,
                                  [Named
                                     (SynIdent (f, None), false, None,
                                      (4,24--4,25));
                                   Named
                                     (SynIdent (g, None), false, None,
                                      (4,27--4,28))], [(4,25--4,26)],
                                  (4,24--4,28)), None)], (4,8--4,28),
                           { ParenRange = (4,7--4,29) }), None, (4,2--4,29)),
                     None, Const (Unit, (4,33--4,35)), (4,2--4,35), Yes,
                     { ArrowRange = Some (4,30--4,32)
                       BarRange = Some (4,0--4,1) })], (3,0--4,35),
                 { MatchKeyword = (3,0--3,5)
                   WithKeyword = (3,8--3,12) }), (3,0--4,35))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--4,35), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [] }, set []))
