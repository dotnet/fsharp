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
                       /root/RangeOfBarInAMultipleSynMatchClausesInSynExprTryWith.fs (2,8--2,10)),
                    /root/RangeOfBarInAMultipleSynMatchClausesInSynExprTryWith.fs (2,4--2,10)),
                 [SynMatchClause
                    (As
                       (LongIdent
                          (SynLongIdent ([IOException], [], [None]), None, None,
                           Pats [], None,
                           /root/RangeOfBarInAMultipleSynMatchClausesInSynExprTryWith.fs (4,2--4,13)),
                        Named
                          (SynIdent (ioex, None), false, None,
                           /root/RangeOfBarInAMultipleSynMatchClausesInSynExprTryWith.fs (4,17--4,21)),
                        /root/RangeOfBarInAMultipleSynMatchClausesInSynExprTryWith.fs (4,2--4,21)),
                     None,
                     Const
                       (Unit,
                        /root/RangeOfBarInAMultipleSynMatchClausesInSynExprTryWith.fs (6,4--6,6)),
                     /root/RangeOfBarInAMultipleSynMatchClausesInSynExprTryWith.fs (4,2--6,6),
                     Yes,
                     { ArrowRange =
                        Some
                          /root/RangeOfBarInAMultipleSynMatchClausesInSynExprTryWith.fs (4,22--4,24)
                       BarRange =
                        Some
                          /root/RangeOfBarInAMultipleSynMatchClausesInSynExprTryWith.fs (4,0--4,1) });
                  SynMatchClause
                    (Named
                       (SynIdent (ex, None), false, None,
                        /root/RangeOfBarInAMultipleSynMatchClausesInSynExprTryWith.fs (7,2--7,4)),
                     None,
                     Const
                       (Unit,
                        /root/RangeOfBarInAMultipleSynMatchClausesInSynExprTryWith.fs (7,8--7,10)),
                     /root/RangeOfBarInAMultipleSynMatchClausesInSynExprTryWith.fs (7,2--7,10),
                     Yes,
                     { ArrowRange =
                        Some
                          /root/RangeOfBarInAMultipleSynMatchClausesInSynExprTryWith.fs (7,5--7,7)
                       BarRange =
                        Some
                          /root/RangeOfBarInAMultipleSynMatchClausesInSynExprTryWith.fs (7,0--7,1) })],
                 /root/RangeOfBarInAMultipleSynMatchClausesInSynExprTryWith.fs (1,0--7,10),
                 Yes
                   /root/RangeOfBarInAMultipleSynMatchClausesInSynExprTryWith.fs (1,0--1,3),
                 Yes
                   /root/RangeOfBarInAMultipleSynMatchClausesInSynExprTryWith.fs (3,0--3,4),
                 { TryKeyword =
                    /root/RangeOfBarInAMultipleSynMatchClausesInSynExprTryWith.fs (1,0--1,3)
                   TryToWithRange =
                    /root/RangeOfBarInAMultipleSynMatchClausesInSynExprTryWith.fs (1,0--3,4)
                   WithKeyword =
                    /root/RangeOfBarInAMultipleSynMatchClausesInSynExprTryWith.fs (3,0--3,4)
                   WithToEndRange =
                    /root/RangeOfBarInAMultipleSynMatchClausesInSynExprTryWith.fs (3,0--7,10) }),
              /root/RangeOfBarInAMultipleSynMatchClausesInSynExprTryWith.fs (1,0--7,10))],
          PreXmlDocEmpty, [], None,
          /root/RangeOfBarInAMultipleSynMatchClausesInSynExprTryWith.fs (1,0--7,10),
          { LeadingKeyword = None })], (true, false),
      { ConditionalDirectives = []
        CodeComments =
         [LineComment
            /root/RangeOfBarInAMultipleSynMatchClausesInSynExprTryWith.fs (5,4--5,19)] },
      set []))