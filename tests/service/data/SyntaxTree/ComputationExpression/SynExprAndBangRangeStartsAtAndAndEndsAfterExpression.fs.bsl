ImplFile
  (ParsedImplFileInput
     ("/root/ComputationExpression/SynExprAndBangRangeStartsAtAndAndEndsAfterExpression.fs",
      false,
      QualifiedNameOfFile SynExprAndBangRangeStartsAtAndAndEndsAfterExpression,
      [], [],
      [SynModuleOrNamespace
         ([SynExprAndBangRangeStartsAtAndAndEndsAfterExpression], false,
          AnonModule,
          [Expr
             (App
                (NonAtomic, false, Ident async,
                 ComputationExpr
                   (false,
                    LetOrUseBang
                      (Yes
                         /root/ComputationExpression/SynExprAndBangRangeStartsAtAndAndEndsAfterExpression.fs (3,4--3,24),
                       false, true,
                       Named
                         (SynIdent (bar, None), false, None,
                          /root/ComputationExpression/SynExprAndBangRangeStartsAtAndAndEndsAfterExpression.fs (3,9--3,12)),
                       App
                         (NonAtomic, false, Ident getBar,
                          Const
                            (Unit,
                             /root/ComputationExpression/SynExprAndBangRangeStartsAtAndAndEndsAfterExpression.fs (3,22--3,24)),
                          /root/ComputationExpression/SynExprAndBangRangeStartsAtAndAndEndsAfterExpression.fs (3,15--3,24)),
                       [SynExprAndBang
                          (Yes
                             /root/ComputationExpression/SynExprAndBangRangeStartsAtAndAndEndsAfterExpression.fs (5,4--7,10),
                           false, true,
                           Named
                             (SynIdent (foo, None), false, None,
                              /root/ComputationExpression/SynExprAndBangRangeStartsAtAndAndEndsAfterExpression.fs (5,9--5,12)),
                           App
                             (NonAtomic, false, Ident getFoo,
                              Const
                                (Unit,
                                 /root/ComputationExpression/SynExprAndBangRangeStartsAtAndAndEndsAfterExpression.fs (5,22--5,24)),
                              /root/ComputationExpression/SynExprAndBangRangeStartsAtAndAndEndsAfterExpression.fs (5,15--5,24)),
                           /root/ComputationExpression/SynExprAndBangRangeStartsAtAndAndEndsAfterExpression.fs (5,4--5,24),
                           { EqualsRange =
                              /root/ComputationExpression/SynExprAndBangRangeStartsAtAndAndEndsAfterExpression.fs (5,13--5,14)
                             InKeyword = None })],
                       YieldOrReturn
                         ((false, true), Ident bar,
                          /root/ComputationExpression/SynExprAndBangRangeStartsAtAndAndEndsAfterExpression.fs (7,4--7,14)),
                       /root/ComputationExpression/SynExprAndBangRangeStartsAtAndAndEndsAfterExpression.fs (3,4--7,14),
                       { EqualsRange =
                          Some
                            /root/ComputationExpression/SynExprAndBangRangeStartsAtAndAndEndsAfterExpression.fs (3,13--3,14) }),
                    /root/ComputationExpression/SynExprAndBangRangeStartsAtAndAndEndsAfterExpression.fs (2,6--8,1)),
                 /root/ComputationExpression/SynExprAndBangRangeStartsAtAndAndEndsAfterExpression.fs (2,0--8,1)),
              /root/ComputationExpression/SynExprAndBangRangeStartsAtAndAndEndsAfterExpression.fs (2,0--8,1))],
          PreXmlDocEmpty, [], None,
          /root/ComputationExpression/SynExprAndBangRangeStartsAtAndAndEndsAfterExpression.fs (2,0--8,1),
          { LeadingKeyword = None })], (true, false),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))
