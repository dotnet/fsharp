ImplFile
  (ParsedImplFileInput
     ("/root/ComputationExpression/SynExprAndBangRangeStartsAtAndAndEndsAfterExpression.fs",
      false,
      QualifiedNameOfFile SynExprAndBangRangeStartsAtAndAndEndsAfterExpression,
      [],
      [SynModuleOrNamespace
         ([SynExprAndBangRangeStartsAtAndAndEndsAfterExpression], false,
          AnonModule,
          [Expr
             (App
                (NonAtomic, false, Ident async,
                 ComputationExpr
                   (false,
                    LetOrUse
                      (false, false, true, true,
                       [SynBinding
                          (None, Normal, false, false, [], PreXmlDocEmpty,
                           SynValData
                             (None,
                              SynValInfo ([], SynArgInfo ([], false, None)),
                              None),
                           Named
                             (SynIdent (bar, None), false, None, (3,9--3,12)),
                           None,
                           App
                             (NonAtomic, false, Ident getBar,
                              Const (Unit, (3,22--3,24)), (3,15--3,24)),
                           (3,4--7,14), Yes (3,4--3,24),
                           { LeadingKeyword = Let (3,4--3,8)
                             InlineKeyword = None
                             EqualsRange = Some (3,13--3,14) });
                        SynBinding
                          (None, Normal, false, false, [], PreXmlDocEmpty,
                           SynValData
                             (None,
                              SynValInfo ([], SynArgInfo ([], false, None)),
                              None),
                           Named
                             (SynIdent (foo, None), false, None, (5,9--5,12)),
                           None,
                           App
                             (NonAtomic, false, Ident getFoo,
                              Const (Unit, (5,22--5,24)), (5,15--5,24)),
                           (5,4--5,24), Yes (5,4--5,24),
                           { LeadingKeyword = And (5,4--5,8)
                             InlineKeyword = None
                             EqualsRange = Some (5,13--5,14) })],
                       YieldOrReturn
                         ((false, true), Ident bar, (7,4--7,14),
                          { YieldOrReturnKeyword = (7,4--7,10) }), (3,4--7,14),
                       { LetOrUseKeyword = (3,4--3,8)
                         InKeyword = None
                         EqualsRange = Some (3,13--3,14) }), (2,6--8,1)),
                 (2,0--8,1)), (2,0--8,1))], PreXmlDocEmpty, [], None, (2,0--8,1),
          { LeadingKeyword = None })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [] }, set []))
