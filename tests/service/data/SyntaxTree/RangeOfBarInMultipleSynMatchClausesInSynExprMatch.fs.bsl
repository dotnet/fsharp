ImplFile
  (ParsedImplFileInput
     ("/root/RangeOfBarInMultipleSynMatchClausesInSynExprMatch.fs", false,
      QualifiedNameOfFile RangeOfBarInMultipleSynMatchClausesInSynExprMatch, [],
      [],
      [SynModuleOrNamespace
         ([RangeOfBarInMultipleSynMatchClausesInSynExprMatch], false, AnonModule,
          [Expr
             (Match
                (Yes
                   /root/RangeOfBarInMultipleSynMatchClausesInSynExprMatch.fs (1,0--1,14),
                 Ident foo,
                 [SynMatchClause
                    (LongIdent
                       (SynLongIdent ([Bar], [], [None]), None, None,
                        Pats
                          [Named
                             (SynIdent (bar, None), false, None,
                              /root/RangeOfBarInMultipleSynMatchClausesInSynExprMatch.fs (2,6--2,9))],
                        None,
                        /root/RangeOfBarInMultipleSynMatchClausesInSynExprMatch.fs (2,2--2,9)),
                     Some
                       (Paren
                          (App
                             (NonAtomic, false, Ident someCheck, Ident bar,
                              /root/RangeOfBarInMultipleSynMatchClausesInSynExprMatch.fs (2,16--2,29)),
                           /root/RangeOfBarInMultipleSynMatchClausesInSynExprMatch.fs (2,15--2,16),
                           Some
                             /root/RangeOfBarInMultipleSynMatchClausesInSynExprMatch.fs (2,29--2,30),
                           /root/RangeOfBarInMultipleSynMatchClausesInSynExprMatch.fs (2,15--2,30))),
                     Const
                       (Unit,
                        /root/RangeOfBarInMultipleSynMatchClausesInSynExprMatch.fs (2,34--2,36)),
                     /root/RangeOfBarInMultipleSynMatchClausesInSynExprMatch.fs (2,2--2,36),
                     Yes,
                     { ArrowRange =
                        Some
                          /root/RangeOfBarInMultipleSynMatchClausesInSynExprMatch.fs (2,31--2,33)
                       BarRange =
                        Some
                          /root/RangeOfBarInMultipleSynMatchClausesInSynExprMatch.fs (2,0--2,1) });
                  SynMatchClause
                    (LongIdent
                       (SynLongIdent ([Far], [], [None]), None, None,
                        Pats
                          [Named
                             (SynIdent (too, None), false, None,
                              /root/RangeOfBarInMultipleSynMatchClausesInSynExprMatch.fs (3,6--3,9))],
                        None,
                        /root/RangeOfBarInMultipleSynMatchClausesInSynExprMatch.fs (3,2--3,9)),
                     None,
                     App
                       (NonAtomic, false, Ident near,
                        Const
                          (Unit,
                           /root/RangeOfBarInMultipleSynMatchClausesInSynExprMatch.fs (3,18--3,20)),
                        /root/RangeOfBarInMultipleSynMatchClausesInSynExprMatch.fs (3,13--3,20)),
                     /root/RangeOfBarInMultipleSynMatchClausesInSynExprMatch.fs (3,2--3,20),
                     Yes,
                     { ArrowRange =
                        Some
                          /root/RangeOfBarInMultipleSynMatchClausesInSynExprMatch.fs (3,10--3,12)
                       BarRange =
                        Some
                          /root/RangeOfBarInMultipleSynMatchClausesInSynExprMatch.fs (3,0--3,1) })],
                 /root/RangeOfBarInMultipleSynMatchClausesInSynExprMatch.fs (1,0--3,20),
                 { MatchKeyword =
                    /root/RangeOfBarInMultipleSynMatchClausesInSynExprMatch.fs (1,0--1,5)
                   WithKeyword =
                    /root/RangeOfBarInMultipleSynMatchClausesInSynExprMatch.fs (1,10--1,14) }),
              /root/RangeOfBarInMultipleSynMatchClausesInSynExprMatch.fs (1,0--3,20))],
          PreXmlDocEmpty, [], None,
          /root/RangeOfBarInMultipleSynMatchClausesInSynExprMatch.fs (1,0--3,20),
          { LeadingKeyword = None })], (true, false),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))