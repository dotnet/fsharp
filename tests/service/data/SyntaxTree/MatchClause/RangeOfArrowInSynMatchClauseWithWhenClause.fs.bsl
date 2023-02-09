ImplFile
  (ParsedImplFileInput
     ("/root/MatchClause/RangeOfArrowInSynMatchClauseWithWhenClause.fs", false,
      QualifiedNameOfFile RangeOfArrowInSynMatchClauseWithWhenClause, [], [],
      [SynModuleOrNamespace
         ([RangeOfArrowInSynMatchClauseWithWhenClause], false, AnonModule,
          [Expr
             (Match
                (Yes
                   /root/MatchClause/RangeOfArrowInSynMatchClauseWithWhenClause.fs (2,0--2,14),
                 Ident foo,
                 [SynMatchClause
                    (LongIdent
                       (SynLongIdent ([Bar], [], [None]), None, None,
                        Pats
                          [Named
                             (SynIdent (bar, None), false, None,
                              /root/MatchClause/RangeOfArrowInSynMatchClauseWithWhenClause.fs (3,6--3,9))],
                        None,
                        /root/MatchClause/RangeOfArrowInSynMatchClauseWithWhenClause.fs (3,2--3,9)),
                     Some
                       (Paren
                          (App
                             (NonAtomic, false, Ident someCheck, Ident bar,
                              /root/MatchClause/RangeOfArrowInSynMatchClauseWithWhenClause.fs (3,16--3,29)),
                           /root/MatchClause/RangeOfArrowInSynMatchClauseWithWhenClause.fs (3,15--3,16),
                           Some
                             /root/MatchClause/RangeOfArrowInSynMatchClauseWithWhenClause.fs (3,29--3,30),
                           /root/MatchClause/RangeOfArrowInSynMatchClauseWithWhenClause.fs (3,15--3,30))),
                     Const
                       (Unit,
                        /root/MatchClause/RangeOfArrowInSynMatchClauseWithWhenClause.fs (3,34--3,36)),
                     /root/MatchClause/RangeOfArrowInSynMatchClauseWithWhenClause.fs (3,2--3,36),
                     Yes,
                     { ArrowRange =
                        Some
                          /root/MatchClause/RangeOfArrowInSynMatchClauseWithWhenClause.fs (3,31--3,33)
                       BarRange =
                        Some
                          /root/MatchClause/RangeOfArrowInSynMatchClauseWithWhenClause.fs (3,0--3,1) })],
                 /root/MatchClause/RangeOfArrowInSynMatchClauseWithWhenClause.fs (2,0--3,36),
                 { MatchKeyword =
                    /root/MatchClause/RangeOfArrowInSynMatchClauseWithWhenClause.fs (2,0--2,5)
                   WithKeyword =
                    /root/MatchClause/RangeOfArrowInSynMatchClauseWithWhenClause.fs (2,10--2,14) }),
              /root/MatchClause/RangeOfArrowInSynMatchClauseWithWhenClause.fs (2,0--3,36))],
          PreXmlDocEmpty, [], None,
          /root/MatchClause/RangeOfArrowInSynMatchClauseWithWhenClause.fs (2,0--4,0),
          { LeadingKeyword = None })], (true, false),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))