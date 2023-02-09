ImplFile
  (ParsedImplFileInput
     ("/root/NoRangeOfBarInASingleSynMatchClauseInSynExprTryWith.fs", false,
      QualifiedNameOfFile NoRangeOfBarInASingleSynMatchClauseInSynExprTryWith,
      [], [],
      [SynModuleOrNamespace
         ([NoRangeOfBarInASingleSynMatchClauseInSynExprTryWith], false,
          AnonModule,
          [Expr
             (TryWith
                (App
                   (NonAtomic, false, Ident foo,
                    Const
                      (Unit,
                       /root/NoRangeOfBarInASingleSynMatchClauseInSynExprTryWith.fs (3,8--3,10)),
                    /root/NoRangeOfBarInASingleSynMatchClauseInSynExprTryWith.fs (3,4--3,10)),
                 [SynMatchClause
                    (Named
                       (SynIdent (exn, None), false, None,
                        /root/NoRangeOfBarInASingleSynMatchClauseInSynExprTryWith.fs (4,5--4,8)),
                     None,
                     Const
                       (Unit,
                        /root/NoRangeOfBarInASingleSynMatchClauseInSynExprTryWith.fs (6,4--6,6)),
                     /root/NoRangeOfBarInASingleSynMatchClauseInSynExprTryWith.fs (4,5--6,6),
                     Yes,
                     { ArrowRange =
                        Some
                          /root/NoRangeOfBarInASingleSynMatchClauseInSynExprTryWith.fs (4,9--4,11)
                       BarRange = None })],
                 /root/NoRangeOfBarInASingleSynMatchClauseInSynExprTryWith.fs (2,0--6,6),
                 Yes
                   /root/NoRangeOfBarInASingleSynMatchClauseInSynExprTryWith.fs (2,0--2,3),
                 Yes
                   /root/NoRangeOfBarInASingleSynMatchClauseInSynExprTryWith.fs (4,0--4,4),
                 { TryKeyword =
                    /root/NoRangeOfBarInASingleSynMatchClauseInSynExprTryWith.fs (2,0--2,3)
                   TryToWithRange =
                    /root/NoRangeOfBarInASingleSynMatchClauseInSynExprTryWith.fs (2,0--4,4)
                   WithKeyword =
                    /root/NoRangeOfBarInASingleSynMatchClauseInSynExprTryWith.fs (4,0--4,4)
                   WithToEndRange =
                    /root/NoRangeOfBarInASingleSynMatchClauseInSynExprTryWith.fs (4,0--6,6) }),
              /root/NoRangeOfBarInASingleSynMatchClauseInSynExprTryWith.fs (2,0--6,6))],
          PreXmlDocEmpty, [], None,
          /root/NoRangeOfBarInASingleSynMatchClauseInSynExprTryWith.fs (2,0--7,0),
          { LeadingKeyword = None })], (true, false),
      { ConditionalDirectives = []
        CodeComments =
         [LineComment
            /root/NoRangeOfBarInASingleSynMatchClauseInSynExprTryWith.fs (5,4--5,19)] },
      set []))