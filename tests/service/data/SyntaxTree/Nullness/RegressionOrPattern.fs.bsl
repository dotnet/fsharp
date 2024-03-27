ImplFile
  (ParsedImplFileInput
     ("/root/Nullness/RegressionOrPattern.fs", false,
      QualifiedNameOfFile RegressionOrPattern, [], [],
      [SynModuleOrNamespace
         ([RegressionOrPattern], false, AnonModule,
          [Expr
             (Match
                (Yes (1,0--1,14), Ident exn,
                 [SynMatchClause
                    (As
                       (Or
                          (LongIdent
                             (SynLongIdent ([InternalError], [], [None]), None,
                              None,
                              Pats
                                [Paren
                                   (Tuple
                                      (false,
                                       [Named
                                          (SynIdent (s, None), false, None,
                                           (2,17--2,18)); Wild (2,20--2,21)],
                                       [(2,18--2,19)], (2,17--2,21)),
                                    (2,16--2,22))], None, (2,2--2,22)),
                           LongIdent
                             (SynLongIdent ([Failure], [], [None]), None, None,
                              Pats
                                [Named
                                   (SynIdent (s, None), false, None,
                                    (3,10--3,11))], None, (3,2--3,11)),
                           (2,2--3,11), { BarRange = (3,0--3,1) }),
                        Named (SynIdent (exn, None), false, None, (3,15--3,18)),
                        (2,2--3,18)), None, Const (Unit, (3,22--3,24)),
                     (2,2--3,24), Yes, { ArrowRange = Some (3,19--3,21)
                                         BarRange = Some (2,0--2,1) })],
                 (1,0--3,24), { MatchKeyword = (1,0--1,5)
                                WithKeyword = (1,10--1,14) }), (1,0--3,24))],
          PreXmlDocEmpty, [], None, (1,0--3,24), { LeadingKeyword = None })],
      (true, true), { ConditionalDirectives = []
                      CodeComments = [] }, set []))
