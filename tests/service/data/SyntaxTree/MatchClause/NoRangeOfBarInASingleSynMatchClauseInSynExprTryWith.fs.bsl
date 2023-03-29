ImplFile
  (ParsedImplFileInput
     ("/root/MatchClause/NoRangeOfBarInASingleSynMatchClauseInSynExprTryWith.fs",
      false,
      QualifiedNameOfFile NoRangeOfBarInASingleSynMatchClauseInSynExprTryWith,
      [], [],
      [SynModuleOrNamespace
         ([NoRangeOfBarInASingleSynMatchClauseInSynExprTryWith], false,
          AnonModule,
          [Expr
             (TryWith
                (App
                   (NonAtomic, false, Ident foo, Const (Unit, (3,8--3,10)),
                    (3,4--3,10)),
                 [SynMatchClause
                    (Named (SynIdent (exn, None), false, None, (4,5--4,8)), None,
                     Const (Unit, (6,4--6,6)), (4,5--6,6), Yes,
                     { ArrowRange = Some (4,9--4,11)
                       BarRange = None })], (2,0--6,6), Yes (2,0--2,3),
                 Yes (4,0--4,4), { TryKeyword = (2,0--2,3)
                                   TryToWithRange = (2,0--4,4)
                                   WithKeyword = (4,0--4,4)
                                   WithToEndRange = (4,0--6,6) }), (2,0--6,6))],
          PreXmlDocEmpty, [], None, (2,0--7,0), { LeadingKeyword = None })],
      (true, true), { ConditionalDirectives = []
                      CodeComments = [LineComment (5,4--5,19)] }, set []))
