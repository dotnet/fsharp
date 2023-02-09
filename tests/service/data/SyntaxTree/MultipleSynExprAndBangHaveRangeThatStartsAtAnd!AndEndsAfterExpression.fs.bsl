ImplFile
  (ParsedImplFileInput
     ("/root/MultipleSynExprAndBangHaveRangeThatStartsAtAnd!AndEndsAfterExpression.fs",
      false,
      QualifiedNameOfFile
        MultipleSynExprAndBangHaveRangeThatStartsAtAnd!AndEndsAfterExpression,
      [], [],
      [SynModuleOrNamespace
         ([MultipleSynExprAndBangHaveRangeThatStartsAtAnd!AndEndsAfterExpression],
          false, AnonModule,
          [Expr
             (App
                (NonAtomic, false, Ident async,
                 ComputationExpr
                   (false,
                    LetOrUseBang
                      (Yes
                         /root/MultipleSynExprAndBangHaveRangeThatStartsAtAnd!AndEndsAfterExpression.fs (3,4--3,24),
                       false, true,
                       Named
                         (SynIdent (bar, None), false, None,
                          /root/MultipleSynExprAndBangHaveRangeThatStartsAtAnd!AndEndsAfterExpression.fs (3,9--3,12)),
                       App
                         (NonAtomic, false, Ident getBar,
                          Const
                            (Unit,
                             /root/MultipleSynExprAndBangHaveRangeThatStartsAtAnd!AndEndsAfterExpression.fs (3,22--3,24)),
                          /root/MultipleSynExprAndBangHaveRangeThatStartsAtAnd!AndEndsAfterExpression.fs (3,15--3,24)),
                       [SynExprAndBang
                          (Yes
                             /root/MultipleSynExprAndBangHaveRangeThatStartsAtAnd!AndEndsAfterExpression.fs (4,4--4,27),
                           false, true,
                           Named
                             (SynIdent (foo, None), false, None,
                              /root/MultipleSynExprAndBangHaveRangeThatStartsAtAnd!AndEndsAfterExpression.fs (4,9--4,12)),
                           App
                             (NonAtomic, false, Ident getFoo,
                              Const
                                (Unit,
                                 /root/MultipleSynExprAndBangHaveRangeThatStartsAtAnd!AndEndsAfterExpression.fs (4,22--4,24)),
                              /root/MultipleSynExprAndBangHaveRangeThatStartsAtAnd!AndEndsAfterExpression.fs (4,15--4,24)),
                           /root/MultipleSynExprAndBangHaveRangeThatStartsAtAnd!AndEndsAfterExpression.fs (4,4--4,24),
                           { EqualsRange =
                              /root/MultipleSynExprAndBangHaveRangeThatStartsAtAnd!AndEndsAfterExpression.fs (4,13--4,14)
                             InKeyword =
                              Some
                                /root/MultipleSynExprAndBangHaveRangeThatStartsAtAnd!AndEndsAfterExpression.fs (4,25--4,27) });
                        SynExprAndBang
                          (Yes
                             /root/MultipleSynExprAndBangHaveRangeThatStartsAtAnd!AndEndsAfterExpression.fs (5,4--6,10),
                           false, true,
                           Named
                             (SynIdent (meh, None), false, None,
                              /root/MultipleSynExprAndBangHaveRangeThatStartsAtAnd!AndEndsAfterExpression.fs (5,9--5,12)),
                           App
                             (NonAtomic, false, Ident getMeh,
                              Const
                                (Unit,
                                 /root/MultipleSynExprAndBangHaveRangeThatStartsAtAnd!AndEndsAfterExpression.fs (5,22--5,24)),
                              /root/MultipleSynExprAndBangHaveRangeThatStartsAtAnd!AndEndsAfterExpression.fs (5,15--5,24)),
                           /root/MultipleSynExprAndBangHaveRangeThatStartsAtAnd!AndEndsAfterExpression.fs (5,4--5,24),
                           { EqualsRange =
                              /root/MultipleSynExprAndBangHaveRangeThatStartsAtAnd!AndEndsAfterExpression.fs (5,13--5,14)
                             InKeyword = None })],
                       YieldOrReturn
                         ((false, true), Ident bar,
                          /root/MultipleSynExprAndBangHaveRangeThatStartsAtAnd!AndEndsAfterExpression.fs (6,4--6,14)),
                       /root/MultipleSynExprAndBangHaveRangeThatStartsAtAnd!AndEndsAfterExpression.fs (3,4--6,14),
                       { EqualsRange =
                          Some
                            /root/MultipleSynExprAndBangHaveRangeThatStartsAtAnd!AndEndsAfterExpression.fs (3,13--3,14) }),
                    /root/MultipleSynExprAndBangHaveRangeThatStartsAtAnd!AndEndsAfterExpression.fs (2,6--7,1)),
                 /root/MultipleSynExprAndBangHaveRangeThatStartsAtAnd!AndEndsAfterExpression.fs (2,0--7,1)),
              /root/MultipleSynExprAndBangHaveRangeThatStartsAtAnd!AndEndsAfterExpression.fs (2,0--7,1))],
          PreXmlDocEmpty, [], None,
          /root/MultipleSynExprAndBangHaveRangeThatStartsAtAnd!AndEndsAfterExpression.fs (2,0--7,1),
          { LeadingKeyword = None })], (true, false),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))