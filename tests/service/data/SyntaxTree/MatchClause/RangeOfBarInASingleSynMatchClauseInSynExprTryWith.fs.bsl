ImplFile
  (ParsedImplFileInput
     ("/root/MatchClause/RangeOfBarInASingleSynMatchClauseInSynExprTryWith.fs",
      false,
      QualifiedNameOfFile RangeOfBarInASingleSynMatchClauseInSynExprTryWith, [],
      [],
      [SynModuleOrNamespace
         ([RangeOfBarInASingleSynMatchClauseInSynExprTryWith], false, AnonModule,
          [Expr
             (TryWith
                (App
                   (NonAtomic, false, Ident foo, Const (Unit, (3,8--3,10)),
                    (3,4--3,10)),
                 [SynMatchClause
                    (Named (SynIdent (exn, None), false, None, (5,2--5,5)), None,
                     Const (Unit, (5,9--5,11)), (5,2--5,11), Yes,
                     { ArrowRange = Some (5,6--5,8)
                       BarRange = Some (5,0--5,1) })], (2,0--5,11),
                 Yes (2,0--2,3), Yes (4,0--4,4),
                 { TryKeyword = (2,0--2,3)
                   TryToWithRange = (2,0--4,4)
                   WithKeyword = (4,0--4,4)
                   WithToEndRange = (4,0--5,11) }), (2,0--5,11))],
          PreXmlDocEmpty, [], None, (2,0--6,0), { LeadingKeyword = None })],
      (true, true), { ConditionalDirectives = []
                      CodeComments = [] }, set []))
