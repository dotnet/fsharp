ImplFile
  (ParsedImplFileInput
     ("/root/MatchClause/RangeOfArrowInSynMatchClause.fs", false,
      QualifiedNameOfFile RangeOfArrowInSynMatchClause, [], [],
      [SynModuleOrNamespace
         ([RangeOfArrowInSynMatchClause], false, AnonModule,
          [Expr
             (Match
                (Yes (2,0--2,14), Ident foo,
                 [SynMatchClause
                    (LongIdent
                       (SynLongIdent ([Bar], [], [None]), None, None,
                        Pats
                          [Named (SynIdent (bar, None), false, None, (3,6--3,9))],
                        None, (3,2--3,9)), None, Const (Unit, (3,13--3,15)),
                     (3,2--3,15), Yes, { ArrowRange = Some (3,10--3,12)
                                         BarRange = Some (3,0--3,1) })],
                 (2,0--3,15), { MatchKeyword = (2,0--2,5)
                                WithKeyword = (2,10--2,14) }), (2,0--3,15))],
          PreXmlDocEmpty, [], None, (2,0--4,0), { LeadingKeyword = None })],
      (true, false), { ConditionalDirectives = []
                       CodeComments = [] }, set []))
