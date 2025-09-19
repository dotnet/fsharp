ImplFile
  (ParsedImplFileInput
     ("/root/Pattern/Named field 72.fs", false, QualifiedNameOfFile Module, [],
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
                               (4,4--4,12),
                               Tuple
                                 (false,
                                  [Named
                                     (SynIdent (a, None), false, None,
                                      (4,8--4,9));
                                   Named
                                     (SynIdent (r, None), false, None,
                                      (4,11--4,12))], [(4,9--4,10)], (4,8--4,12)),
                               Some (Semicolon ((4,12--4,13), Some (4,13))));
                            NamePatPairField
                              (SynLongIdent ([b], [], [None]), Some (4,16--4,17),
                               (4,14--4,19),
                               Named
                                 (SynIdent (b, None), false, None, (4,18--4,19)),
                               Some (Comma ((4,19--4,20), Some (4,20))));
                            NamePatPairField
                              (SynLongIdent ([c], [], [None]), Some (4,23--4,24),
                               (4,21--4,26),
                               Named
                                 (SynIdent (c, None), false, None, (4,25--4,26)),
                               None)], (4,4--4,27), { ParenRange = (4,3--4,27) }),
                        None, (4,2--4,27)), None, Const (Int32 2, (4,31--4,32)),
                     (4,2--4,32), Yes, { ArrowRange = Some (4,28--4,30)
                                         BarRange = Some (4,0--4,1) })],
                 (3,0--4,32), { MatchKeyword = (3,0--3,5)
                                WithKeyword = (3,8--3,12) }), (3,0--4,32))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--4,32), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [] }, set []))

(4,19)-(4,20) parse error Inconsistent separators in pattern. Use either all commas or all semicolons, but not both.
