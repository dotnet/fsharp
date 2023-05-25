ImplFile
  (ParsedImplFileInput
     ("/root/ComputationExpression/MultipleSynExprAndBangHaveRangeThatStartsAtAndAndEndsAfterExpression.fs",
      false,
      QualifiedNameOfFile
        MultipleSynExprAndBangHaveRangeThatStartsAtAndAndEndsAfterExpression, [],
      [],
      [SynModuleOrNamespace
         ([MultipleSynExprAndBangHaveRangeThatStartsAtAndAndEndsAfterExpression],
          false, AnonModule,
          [Expr
             (App
                (NonAtomic, false, Ident async,
                 ComputationExpr
                   (false,
                    LetOrUseBang
                      (Yes (3,4--3,24), false, true,
                       Named (SynIdent (bar, None), false, None, (3,9--3,12)),
                       App
                         (NonAtomic, false, Ident getBar,
                          Const (Unit, (3,22--3,24)), (3,15--3,24)),
                       [SynExprAndBang
                          (Yes (4,4--4,27), false, true,
                           Named
                             (SynIdent (foo, None), false, None, (4,9--4,12)),
                           App
                             (NonAtomic, false, Ident getFoo,
                              Const (Unit, (4,22--4,24)), (4,15--4,24)),
                           (4,4--4,24), { EqualsRange = (4,13--4,14)
                                          InKeyword = Some (4,25--4,27) });
                        SynExprAndBang
                          (Yes (5,4--6,10), false, true,
                           Named
                             (SynIdent (meh, None), false, None, (5,9--5,12)),
                           App
                             (NonAtomic, false, Ident getMeh,
                              Const (Unit, (5,22--5,24)), (5,15--5,24)),
                           (5,4--5,24), { EqualsRange = (5,13--5,14)
                                          InKeyword = None })],
                       YieldOrReturn ((false, true), Ident bar, (6,4--6,14)),
                       (3,4--6,14), { EqualsRange = Some (3,13--3,14) }),
                    (2,6--7,1)), (2,0--7,1)), (2,0--7,1))], PreXmlDocEmpty, [],
          None, (2,0--7,1), { LeadingKeyword = None })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))
