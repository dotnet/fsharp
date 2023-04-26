ImplFile
  (ParsedImplFileInput
     ("/root/MatchClause/RangeOfBarInAMultipleSynMatchClausesInSynExprTryWith.fs",
      false,
      QualifiedNameOfFile RangeOfBarInAMultipleSynMatchClausesInSynExprTryWith,
      [], [],
      [SynModuleOrNamespace
         ([RangeOfBarInAMultipleSynMatchClausesInSynExprTryWith], false,
          AnonModule,
          [Expr
             (TryWith
                (App
                   (NonAtomic, false, Ident foo, Const (Unit, (3,8--3,10)),
                    (3,4--3,10)),
                 [SynMatchClause
                    (As
                       (LongIdent
                          (SynLongIdent ([IOException], [], [None]), None, None,
                           Pats [], None, (5,2--5,13)),
                        Named (SynIdent (ioex, None), false, None, (5,17--5,21)),
                        (5,2--5,21)), None, Const (Unit, (7,4--7,6)), (5,2--7,6),
                     Yes, { ArrowRange = Some (5,22--5,24)
                            BarRange = Some (5,0--5,1) });
                  SynMatchClause
                    (Named (SynIdent (ex, None), false, None, (8,2--8,4)), None,
                     Const (Unit, (8,8--8,10)), (8,2--8,10), Yes,
                     { ArrowRange = Some (8,5--8,7)
                       BarRange = Some (8,0--8,1) })], (2,0--8,10),
                 Yes (2,0--2,3), Yes (4,0--4,4),
                 { TryKeyword = (2,0--2,3)
                   TryToWithRange = (2,0--4,4)
                   WithKeyword = (4,0--4,4)
                   WithToEndRange = (4,0--8,10) }), (2,0--8,10))],
          PreXmlDocEmpty, [], None, (2,0--9,0), { LeadingKeyword = None })],
      (true, true), { ConditionalDirectives = []
                      CodeComments = [LineComment (6,4--6,19)] }, set []))
