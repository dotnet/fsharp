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
                       /root/NoRangeOfBarInASingleSynMatchClauseInSynExprTryWith.fs (2,8--2,10)),
                    /root/NoRangeOfBarInASingleSynMatchClauseInSynExprTryWith.fs (2,4--2,10)),
                 [SynMatchClause
                    (Named
                       (SynIdent (exn, None), false, None,
                        /root/NoRangeOfBarInASingleSynMatchClauseInSynExprTryWith.fs (3,5--3,8)),
                     None,
                     Const
                       (Unit,
                        /root/NoRangeOfBarInASingleSynMatchClauseInSynExprTryWith.fs (5,4--5,6)),
                     /root/NoRangeOfBarInASingleSynMatchClauseInSynExprTryWith.fs (3,5--5,6),
                     Yes,
                     { ArrowRange =
                        Some
                          /root/NoRangeOfBarInASingleSynMatchClauseInSynExprTryWith.fs (3,9--3,11)
                       BarRange = None })],
                 /root/NoRangeOfBarInASingleSynMatchClauseInSynExprTryWith.fs (1,0--5,6),
                 Yes
                   /root/NoRangeOfBarInASingleSynMatchClauseInSynExprTryWith.fs (1,0--1,3),
                 Yes
                   /root/NoRangeOfBarInASingleSynMatchClauseInSynExprTryWith.fs (3,0--3,4),
                 { TryKeyword =
                    /root/NoRangeOfBarInASingleSynMatchClauseInSynExprTryWith.fs (1,0--1,3)
                   TryToWithRange =
                    /root/NoRangeOfBarInASingleSynMatchClauseInSynExprTryWith.fs (1,0--3,4)
                   WithKeyword =
                    /root/NoRangeOfBarInASingleSynMatchClauseInSynExprTryWith.fs (3,0--3,4)
                   WithToEndRange =
                    /root/NoRangeOfBarInASingleSynMatchClauseInSynExprTryWith.fs (3,0--5,6) }),
              /root/NoRangeOfBarInASingleSynMatchClauseInSynExprTryWith.fs (1,0--5,6))],
          PreXmlDocEmpty, [], None,
          /root/NoRangeOfBarInASingleSynMatchClauseInSynExprTryWith.fs (1,0--5,6),
          { LeadingKeyword = None })], (true, false),
      { ConditionalDirectives = []
        CodeComments =
         [LineComment
            /root/NoRangeOfBarInASingleSynMatchClauseInSynExprTryWith.fs (4,4--4,19)] },
      set []))