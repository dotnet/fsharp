ImplFile
  (ParsedImplFileInput
     ("/root/Pattern/Named field 57.fs", false, QualifiedNameOfFile Module, [],
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
                               (4,4--4,40),
                               Tuple
                                 (false,
                                  [Record
                                     ([NamePatPairField
                                         (SynLongIdent ([X], [], [None]),
                                          Some (4,12--4,13), (4,10--4,15),
                                          Const (Int32 1, (4,14--4,15)),
                                          Some
                                            (Semicolon
                                               ((4,15--4,16), Some (4,16))));
                                       NamePatPairField
                                         (SynLongIdent ([Y], [], [None]),
                                          Some (4,19--4,20), (4,17--4,22),
                                          Const (Int32 3, (4,21--4,22)), None)],
                                      (4,8--4,23));
                                   Record
                                     ([NamePatPairField
                                         (SynLongIdent ([X], [], [None]),
                                          Some (4,29--4,30), (4,27--4,32),
                                          Const (Int32 1, (4,31--4,32)),
                                          Some
                                            (Semicolon
                                               ((4,32--4,33), Some (4,33))));
                                       NamePatPairField
                                         (SynLongIdent ([Y], [], [None]),
                                          Some (4,36--4,37), (4,34--4,39),
                                          Const (Int32 3, (4,38--4,39)), None)],
                                      (4,25--4,40))], [(4,23--4,24)],
                                  (4,8--4,40)),
                               Some (Semicolon ((4,40--4,41), Some (4,41))));
                            NamePatPairField
                              (SynLongIdent ([b], [], [None]), Some (4,44--4,45),
                               (4,42--4,61),
                               Record
                                 ([NamePatPairField
                                     (SynLongIdent ([X], [], [None]),
                                      Some (4,50--4,51), (4,48--4,53),
                                      Const (Int32 1, (4,52--4,53)),
                                      Some
                                        (Semicolon ((4,53--4,54), Some (4,54))));
                                   NamePatPairField
                                     (SynLongIdent ([Y], [], [None]),
                                      Some (4,57--4,58), (4,55--4,60),
                                      Const (Int32 3, (4,59--4,60)), None)],
                                  (4,46--4,61)), None)], (4,4--4,62),
                           { ParenRange = (4,3--4,62) }), None, (4,2--4,62)),
                     None, Const (Unit, (4,66--4,68)), (4,2--4,68), Yes,
                     { ArrowRange = Some (4,63--4,65)
                       BarRange = Some (4,0--4,1) })], (3,0--4,68),
                 { MatchKeyword = (3,0--3,5)
                   WithKeyword = (3,8--3,12) }), (3,0--4,68))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--4,68), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [] }, set []))
