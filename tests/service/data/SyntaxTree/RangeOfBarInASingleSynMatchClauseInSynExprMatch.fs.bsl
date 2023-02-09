ImplFile
  (ParsedImplFileInput
     ("/root/RangeOfBarInASingleSynMatchClauseInSynExprMatch.fs", false,
      QualifiedNameOfFile RangeOfBarInASingleSynMatchClauseInSynExprMatch, [],
      [],
      [SynModuleOrNamespace
         ([RangeOfBarInASingleSynMatchClauseInSynExprMatch], false, AnonModule,
          [Expr
             (Match
                (Yes
                   /root/RangeOfBarInASingleSynMatchClauseInSynExprMatch.fs (2,0--2,14),
                 Ident foo,
                 [SynMatchClause
                    (LongIdent
                       (SynLongIdent ([Bar], [], [None]), None, None,
                        Pats
                          [Named
                             (SynIdent (bar, None), false, None,
                              /root/RangeOfBarInASingleSynMatchClauseInSynExprMatch.fs (3,6--3,9))],
                        None,
                        /root/RangeOfBarInASingleSynMatchClauseInSynExprMatch.fs (3,2--3,9)),
                     Some
                       (Paren
                          (App
                             (NonAtomic, false, Ident someCheck, Ident bar,
                              /root/RangeOfBarInASingleSynMatchClauseInSynExprMatch.fs (3,16--3,29)),
                           /root/RangeOfBarInASingleSynMatchClauseInSynExprMatch.fs (3,15--3,16),
                           Some
                             /root/RangeOfBarInASingleSynMatchClauseInSynExprMatch.fs (3,29--3,30),
                           /root/RangeOfBarInASingleSynMatchClauseInSynExprMatch.fs (3,15--3,30))),
                     Const
                       (Unit,
                        /root/RangeOfBarInASingleSynMatchClauseInSynExprMatch.fs (3,34--3,36)),
                     /root/RangeOfBarInASingleSynMatchClauseInSynExprMatch.fs (3,2--3,36),
                     Yes,
                     { ArrowRange =
                        Some
                          /root/RangeOfBarInASingleSynMatchClauseInSynExprMatch.fs (3,31--3,33)
                       BarRange =
                        Some
                          /root/RangeOfBarInASingleSynMatchClauseInSynExprMatch.fs (3,0--3,1) })],
                 /root/RangeOfBarInASingleSynMatchClauseInSynExprMatch.fs (2,0--3,36),
                 { MatchKeyword =
                    /root/RangeOfBarInASingleSynMatchClauseInSynExprMatch.fs (2,0--2,5)
                   WithKeyword =
                    /root/RangeOfBarInASingleSynMatchClauseInSynExprMatch.fs (2,10--2,14) }),
              /root/RangeOfBarInASingleSynMatchClauseInSynExprMatch.fs (2,0--3,36))],
          PreXmlDocEmpty, [], None,
          /root/RangeOfBarInASingleSynMatchClauseInSynExprMatch.fs (2,0--4,0),
          { LeadingKeyword = None })], (true, false),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))