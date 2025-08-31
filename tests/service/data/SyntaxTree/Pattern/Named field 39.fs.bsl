ImplFile
  (ParsedImplFileInput
     ("/root/Pattern/Named field 39.fs", false, QualifiedNameOfFile Module, [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Expr
             (Match
                (Yes (3,0--3,12), Const (Int32 1, (3,6--3,7)),
                 [SynMatchClause
                    (LongIdent
                       (SynLongIdent ([SynExprRecordField], [], [None]), None,
                        None,
                        NamePatPairs
                          ([NamePatPairField
                              (SynLongIdent ([fieldName], [], [None]),
                               Some (4,31--4,32), (4,21--4,62),
                               Tuple
                                 (false,
                                  [LongIdent
                                     (SynLongIdent ([SynLongIdent], [], [None]),
                                      None, None,
                                      NamePatPairs
                                        ([NamePatPairField
                                            (SynLongIdent ([id], [], [None]),
                                             Some (4,49--4,50), (4,46--4,58),
                                             ListCons
                                               (Named
                                                  (SynIdent (id, None), false,
                                                   None, (4,51--4,53)),
                                                Wild (4,57--4,58), (4,51--4,58),
                                                { ColonColonRange = (4,54--4,56) }),
                                             None)], (4,46--4,59),
                                         { ParenRange = (4,45--4,59) }), None,
                                      (4,33--4,59)); Wild (4,61--4,62)],
                                  [(4,59--4,60)], (4,33--4,62)), None)],
                           (4,21--4,62), { ParenRange = (4,20--4,63) }), None,
                        (4,2--4,63)), None, Const (Int32 2, (4,67--4,68)),
                     (4,2--4,68), Yes, { ArrowRange = Some (4,64--4,66)
                                         BarRange = Some (4,0--4,1) })],
                 (3,0--4,68), { MatchKeyword = (3,0--3,5)
                                WithKeyword = (3,8--3,12) }), (3,0--4,68))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--4,68), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [] }, set []))
