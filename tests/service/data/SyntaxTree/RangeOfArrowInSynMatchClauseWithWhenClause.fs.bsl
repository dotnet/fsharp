ImplFile
  (ParsedImplFileInput
     ("/root/RangeOfArrowInSynMatchClauseWithWhenClause.fs", false,
      QualifiedNameOfFile RangeOfArrowInSynMatchClauseWithWhenClause, [], [],
      [SynModuleOrNamespace
         ([RangeOfArrowInSynMatchClauseWithWhenClause], false, AnonModule,
          [Expr
             (Match
                (Yes
                   /root/RangeOfArrowInSynMatchClauseWithWhenClause.fs (1,0--1,14),
                 Ident foo,
                 [SynMatchClause
                    (LongIdent
                       (SynLongIdent ([Bar], [], [None]), None, None,
                        Pats
                          [Named
                             (SynIdent (bar, None), false, None,
                              /root/RangeOfArrowInSynMatchClauseWithWhenClause.fs (2,6--2,9))],
                        None,
                        /root/RangeOfArrowInSynMatchClauseWithWhenClause.fs (2,2--2,9)),
                     Some
                       (Paren
                          (App
                             (NonAtomic, false, Ident someCheck, Ident bar,
                              /root/RangeOfArrowInSynMatchClauseWithWhenClause.fs (2,16--2,29)),
                           /root/RangeOfArrowInSynMatchClauseWithWhenClause.fs (2,15--2,16),
                           Some
                             /root/RangeOfArrowInSynMatchClauseWithWhenClause.fs (2,29--2,30),
                           /root/RangeOfArrowInSynMatchClauseWithWhenClause.fs (2,15--2,30))),
                     Const
                       (Unit,
                        /root/RangeOfArrowInSynMatchClauseWithWhenClause.fs (2,34--2,36)),
                     /root/RangeOfArrowInSynMatchClauseWithWhenClause.fs (2,2--2,36),
                     Yes,
                     { ArrowRange =
                        Some
                          /root/RangeOfArrowInSynMatchClauseWithWhenClause.fs (2,31--2,33)
                       BarRange =
                        Some
                          /root/RangeOfArrowInSynMatchClauseWithWhenClause.fs (2,0--2,1) })],
                 /root/RangeOfArrowInSynMatchClauseWithWhenClause.fs (1,0--2,36),
                 { MatchKeyword =
                    /root/RangeOfArrowInSynMatchClauseWithWhenClause.fs (1,0--1,5)
                   WithKeyword =
                    /root/RangeOfArrowInSynMatchClauseWithWhenClause.fs (1,10--1,14) }),
              /root/RangeOfArrowInSynMatchClauseWithWhenClause.fs (1,0--2,36))],
          PreXmlDocEmpty, [], None,
          /root/RangeOfArrowInSynMatchClauseWithWhenClause.fs (1,0--2,36),
          { LeadingKeyword = None })], (true, false),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))