ImplFile
  (ParsedImplFileInput
     ("/root/SynExprAndBangRangeStartsAtAnd!AndEndsAfterExpression.fs", false,
      QualifiedNameOfFile SynExprAndBangRangeStartsAtAnd!AndEndsAfterExpression,
      [], [],
      [SynModuleOrNamespace
         ([SynExprAndBangRangeStartsAtAnd!AndEndsAfterExpression], false,
          AnonModule,
          [Expr
             (App
                (NonAtomic, false, Ident async,
                 ComputationExpr
                   (false,
                    LetOrUseBang
                      (Yes
                         /root/SynExprAndBangRangeStartsAtAnd!AndEndsAfterExpression.fs (2,4--2,24),
                       false, true,
                       Named
                         (SynIdent (bar, None), false, None,
                          /root/SynExprAndBangRangeStartsAtAnd!AndEndsAfterExpression.fs (2,9--2,12)),
                       App
                         (NonAtomic, false, Ident getBar,
                          Const
                            (Unit,
                             /root/SynExprAndBangRangeStartsAtAnd!AndEndsAfterExpression.fs (2,22--2,24)),
                          /root/SynExprAndBangRangeStartsAtAnd!AndEndsAfterExpression.fs (2,15--2,24)),
                       [SynExprAndBang
                          (Yes
                             /root/SynExprAndBangRangeStartsAtAnd!AndEndsAfterExpression.fs (4,4--6,10),
                           false, true,
                           Named
                             (SynIdent (foo, None), false, None,
                              /root/SynExprAndBangRangeStartsAtAnd!AndEndsAfterExpression.fs (4,9--4,12)),
                           App
                             (NonAtomic, false, Ident getFoo,
                              Const
                                (Unit,
                                 /root/SynExprAndBangRangeStartsAtAnd!AndEndsAfterExpression.fs (4,22--4,24)),
                              /root/SynExprAndBangRangeStartsAtAnd!AndEndsAfterExpression.fs (4,15--4,24)),
                           /root/SynExprAndBangRangeStartsAtAnd!AndEndsAfterExpression.fs (4,4--4,24),
                           { EqualsRange =
                              /root/SynExprAndBangRangeStartsAtAnd!AndEndsAfterExpression.fs (4,13--4,14)
                             InKeyword = None })],
                       YieldOrReturn
                         ((false, true), Ident bar,
                          /root/SynExprAndBangRangeStartsAtAnd!AndEndsAfterExpression.fs (6,4--6,14)),
                       /root/SynExprAndBangRangeStartsAtAnd!AndEndsAfterExpression.fs (2,4--6,14),
                       { EqualsRange =
                          Some
                            /root/SynExprAndBangRangeStartsAtAnd!AndEndsAfterExpression.fs (2,13--2,14) }),
                    /root/SynExprAndBangRangeStartsAtAnd!AndEndsAfterExpression.fs (1,6--7,1)),
                 /root/SynExprAndBangRangeStartsAtAnd!AndEndsAfterExpression.fs (1,0--7,1)),
              /root/SynExprAndBangRangeStartsAtAnd!AndEndsAfterExpression.fs (1,0--7,1))],
          PreXmlDocEmpty, [], None,
          /root/SynExprAndBangRangeStartsAtAnd!AndEndsAfterExpression.fs (1,0--7,1),
          { LeadingKeyword = None })], (true, false),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))