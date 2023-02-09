ImplFile
  (ParsedImplFileInput
     ("/root/RangeOfArrowInSynMatchClauseWithWhenClause.fs", false,
      QualifiedNameOfFile RangeOfArrowInSynMatchClauseWithWhenClause, [], [],
      [SynModuleOrNamespace
         ([RangeOfArrowInSynMatchClauseWithWhenClause], false, AnonModule,
          [Expr
             (Match
                (Yes
                   /root/RangeOfArrowInSynMatchClauseWithWhenClause.fs (2,0--2,14),
                 Ident foo,
                 [SynMatchClause
                    (LongIdent
                       (SynLongIdent ([Bar], [], [None]), None, None,
                        Pats
                          [Named
                             (SynIdent (bar, None), false, None,
                              /root/RangeOfArrowInSynMatchClauseWithWhenClause.fs (3,6--3,9))],
                        None,
                        /root/RangeOfArrowInSynMatchClauseWithWhenClause.fs (3,2--3,9)),
                     Some
                       (Paren
                          (App
                             (NonAtomic, false, Ident someCheck, Ident bar,
                              /root/RangeOfArrowInSynMatchClauseWithWhenClause.fs (3,16--3,29)),
                           /root/RangeOfArrowInSynMatchClauseWithWhenClause.fs (3,15--3,16),
                           Some
                             /root/RangeOfArrowInSynMatchClauseWithWhenClause.fs (3,29--3,30),
                           /root/RangeOfArrowInSynMatchClauseWithWhenClause.fs (3,15--3,30))),
                     Const
                       (Unit,
                        /root/RangeOfArrowInSynMatchClauseWithWhenClause.fs (3,34--3,36)),
                     /root/RangeOfArrowInSynMatchClauseWithWhenClause.fs (3,2--3,36),
                     Yes,
                     { ArrowRange =
                        Some
                          /root/RangeOfArrowInSynMatchClauseWithWhenClause.fs (3,31--3,33)
                       BarRange =
                        Some
                          /root/RangeOfArrowInSynMatchClauseWithWhenClause.fs (3,0--3,1) })],
                 /root/RangeOfArrowInSynMatchClauseWithWhenClause.fs (2,0--3,36),
                 { MatchKeyword =
                    /root/RangeOfArrowInSynMatchClauseWithWhenClause.fs (2,0--2,5)
                   WithKeyword =
                    /root/RangeOfArrowInSynMatchClauseWithWhenClause.fs (2,10--2,14) }),
              /root/RangeOfArrowInSynMatchClauseWithWhenClause.fs (2,0--3,36))],
          PreXmlDocEmpty, [], None,
          /root/RangeOfArrowInSynMatchClauseWithWhenClause.fs (2,0--4,0),
          { LeadingKeyword = None })], (true, false),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))