ImplFile
  (ParsedImplFileInput
     ("/root/RangeOfBarInAMultipleSynMatchClausesInSynExprTryWith.fs", false,
      QualifiedNameOfFile RangeOfBarInAMultipleSynMatchClausesInSynExprTryWith,
      [], [],
      [SynModuleOrNamespace
         ([RangeOfBarInAMultipleSynMatchClausesInSynExprTryWith], false,
          AnonModule,
          [Expr
             (TryWith
                (App
                   (NonAtomic, false, Ident foo,
                    Const
                      (Unit,
                       /root/RangeOfBarInAMultipleSynMatchClausesInSynExprTryWith.fs (3,8--3,10)),
                    /root/RangeOfBarInAMultipleSynMatchClausesInSynExprTryWith.fs (3,4--3,10)),
                 [SynMatchClause
                    (As
                       (LongIdent
                          (SynLongIdent ([IOException], [], [None]), None, None,
                           Pats [], None,
                           /root/RangeOfBarInAMultipleSynMatchClausesInSynExprTryWith.fs (5,2--5,13)),
                        Named
                          (SynIdent (ioex, None), false, None,
                           /root/RangeOfBarInAMultipleSynMatchClausesInSynExprTryWith.fs (5,17--5,21)),
                        /root/RangeOfBarInAMultipleSynMatchClausesInSynExprTryWith.fs (5,2--5,21)),
                     None,
                     Const
                       (Unit,
                        /root/RangeOfBarInAMultipleSynMatchClausesInSynExprTryWith.fs (7,4--7,6)),
                     /root/RangeOfBarInAMultipleSynMatchClausesInSynExprTryWith.fs (5,2--7,6),
                     Yes,
                     { ArrowRange =
                        Some
                          /root/RangeOfBarInAMultipleSynMatchClausesInSynExprTryWith.fs (5,22--5,24)
                       BarRange =
                        Some
                          /root/RangeOfBarInAMultipleSynMatchClausesInSynExprTryWith.fs (5,0--5,1) });
                  SynMatchClause
                    (Named
                       (SynIdent (ex, None), false, None,
                        /root/RangeOfBarInAMultipleSynMatchClausesInSynExprTryWith.fs (8,2--8,4)),
                     None,
                     Const
                       (Unit,
                        /root/RangeOfBarInAMultipleSynMatchClausesInSynExprTryWith.fs (8,8--8,10)),
                     /root/RangeOfBarInAMultipleSynMatchClausesInSynExprTryWith.fs (8,2--8,10),
                     Yes,
                     { ArrowRange =
                        Some
                          /root/RangeOfBarInAMultipleSynMatchClausesInSynExprTryWith.fs (8,5--8,7)
                       BarRange =
                        Some
                          /root/RangeOfBarInAMultipleSynMatchClausesInSynExprTryWith.fs (8,0--8,1) })],
                 /root/RangeOfBarInAMultipleSynMatchClausesInSynExprTryWith.fs (2,0--8,10),
                 Yes
                   /root/RangeOfBarInAMultipleSynMatchClausesInSynExprTryWith.fs (2,0--2,3),
                 Yes
                   /root/RangeOfBarInAMultipleSynMatchClausesInSynExprTryWith.fs (4,0--4,4),
                 { TryKeyword =
                    /root/RangeOfBarInAMultipleSynMatchClausesInSynExprTryWith.fs (2,0--2,3)
                   TryToWithRange =
                    /root/RangeOfBarInAMultipleSynMatchClausesInSynExprTryWith.fs (2,0--4,4)
                   WithKeyword =
                    /root/RangeOfBarInAMultipleSynMatchClausesInSynExprTryWith.fs (4,0--4,4)
                   WithToEndRange =
                    /root/RangeOfBarInAMultipleSynMatchClausesInSynExprTryWith.fs (4,0--8,10) }),
              /root/RangeOfBarInAMultipleSynMatchClausesInSynExprTryWith.fs (2,0--8,10))],
          PreXmlDocEmpty, [], None,
          /root/RangeOfBarInAMultipleSynMatchClausesInSynExprTryWith.fs (2,0--9,0),
          { LeadingKeyword = None })], (true, false),
      { ConditionalDirectives = []
        CodeComments =
         [LineComment
            /root/RangeOfBarInAMultipleSynMatchClausesInSynExprTryWith.fs (6,4--6,19)] },
      set []))