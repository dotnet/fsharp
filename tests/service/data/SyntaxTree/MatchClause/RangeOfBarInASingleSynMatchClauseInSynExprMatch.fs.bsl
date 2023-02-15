ImplFile
  (ParsedImplFileInput
     ("/root/MatchClause/RangeOfBarInASingleSynMatchClauseInSynExprMatch.fs",
      false, QualifiedNameOfFile RangeOfBarInASingleSynMatchClauseInSynExprMatch,
      [], [],
      [SynModuleOrNamespace
         ([RangeOfBarInASingleSynMatchClauseInSynExprMatch], false, AnonModule,
          [Expr
             (Match
                (Yes (2,0--2,14), Ident foo,
                 [SynMatchClause
                    (LongIdent
                       (SynLongIdent ([Bar], [], [None]), None, None,
                        Pats
                          [Named (SynIdent (bar, None), false, None, (3,6--3,9))],
                        None, (3,2--3,9)),
                     Some
                       (Paren
                          (App
                             (NonAtomic, false, Ident someCheck, Ident bar,
                              (3,16--3,29)), (3,15--3,16), Some (3,29--3,30),
                           (3,15--3,30))), Const (Unit, (3,34--3,36)),
                     (3,2--3,36), Yes, { ArrowRange = Some (3,31--3,33)
                                         BarRange = Some (3,0--3,1) })],
                 (2,0--3,36), { MatchKeyword = (2,0--2,5)
                                WithKeyword = (2,10--2,14) }), (2,0--3,36))],
          PreXmlDocEmpty, [], None, (2,0--4,0), { LeadingKeyword = None })],
      (true, false), { ConditionalDirectives = []
                       CodeComments = [] }, set []))
