ImplFile
  (ParsedImplFileInput
     ("/root/MatchClause/RangeOfBarInMultipleSynMatchClausesInSynExprMatch.fs",
      false,
      QualifiedNameOfFile RangeOfBarInMultipleSynMatchClausesInSynExprMatch, [],
      [],
      [SynModuleOrNamespace
         ([RangeOfBarInMultipleSynMatchClausesInSynExprMatch], false, AnonModule,
          [Expr
             (Match
                (Yes
                   /root/MatchClause/RangeOfBarInMultipleSynMatchClausesInSynExprMatch.fs (2,0--2,14),
                 Ident foo,
                 [SynMatchClause
                    (LongIdent
                       (SynLongIdent ([Bar], [], [None]), None, None,
                        Pats
                          [Named
                             (SynIdent (bar, None), false, None,
                              /root/MatchClause/RangeOfBarInMultipleSynMatchClausesInSynExprMatch.fs (3,6--3,9))],
                        None,
                        /root/MatchClause/RangeOfBarInMultipleSynMatchClausesInSynExprMatch.fs (3,2--3,9)),
                     Some
                       (Paren
                          (App
                             (NonAtomic, false, Ident someCheck, Ident bar,
                              /root/MatchClause/RangeOfBarInMultipleSynMatchClausesInSynExprMatch.fs (3,16--3,29)),
                           /root/MatchClause/RangeOfBarInMultipleSynMatchClausesInSynExprMatch.fs (3,15--3,16),
                           Some
                             /root/MatchClause/RangeOfBarInMultipleSynMatchClausesInSynExprMatch.fs (3,29--3,30),
                           /root/MatchClause/RangeOfBarInMultipleSynMatchClausesInSynExprMatch.fs (3,15--3,30))),
                     Const
                       (Unit,
                        /root/MatchClause/RangeOfBarInMultipleSynMatchClausesInSynExprMatch.fs (3,34--3,36)),
                     /root/MatchClause/RangeOfBarInMultipleSynMatchClausesInSynExprMatch.fs (3,2--3,36),
                     Yes,
                     { ArrowRange =
                        Some
                          /root/MatchClause/RangeOfBarInMultipleSynMatchClausesInSynExprMatch.fs (3,31--3,33)
                       BarRange =
                        Some
                          /root/MatchClause/RangeOfBarInMultipleSynMatchClausesInSynExprMatch.fs (3,0--3,1) });
                  SynMatchClause
                    (LongIdent
                       (SynLongIdent ([Far], [], [None]), None, None,
                        Pats
                          [Named
                             (SynIdent (too, None), false, None,
                              /root/MatchClause/RangeOfBarInMultipleSynMatchClausesInSynExprMatch.fs (4,6--4,9))],
                        None,
                        /root/MatchClause/RangeOfBarInMultipleSynMatchClausesInSynExprMatch.fs (4,2--4,9)),
                     None,
                     App
                       (NonAtomic, false, Ident near,
                        Const
                          (Unit,
                           /root/MatchClause/RangeOfBarInMultipleSynMatchClausesInSynExprMatch.fs (4,18--4,20)),
                        /root/MatchClause/RangeOfBarInMultipleSynMatchClausesInSynExprMatch.fs (4,13--4,20)),
                     /root/MatchClause/RangeOfBarInMultipleSynMatchClausesInSynExprMatch.fs (4,2--4,20),
                     Yes,
                     { ArrowRange =
                        Some
                          /root/MatchClause/RangeOfBarInMultipleSynMatchClausesInSynExprMatch.fs (4,10--4,12)
                       BarRange =
                        Some
                          /root/MatchClause/RangeOfBarInMultipleSynMatchClausesInSynExprMatch.fs (4,0--4,1) })],
                 /root/MatchClause/RangeOfBarInMultipleSynMatchClausesInSynExprMatch.fs (2,0--4,20),
                 { MatchKeyword =
                    /root/MatchClause/RangeOfBarInMultipleSynMatchClausesInSynExprMatch.fs (2,0--2,5)
                   WithKeyword =
                    /root/MatchClause/RangeOfBarInMultipleSynMatchClausesInSynExprMatch.fs (2,10--2,14) }),
              /root/MatchClause/RangeOfBarInMultipleSynMatchClausesInSynExprMatch.fs (2,0--4,20))],
          PreXmlDocEmpty, [], None,
          /root/MatchClause/RangeOfBarInMultipleSynMatchClausesInSynExprMatch.fs (2,0--5,0),
          { LeadingKeyword = None })], (true, false),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))
