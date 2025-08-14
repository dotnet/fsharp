ImplFile
  (ParsedImplFileInput
     ("/root/Expression/SynExprLetOrUseBangContainsTheRangeOfTheEqualsSign.fs",
      false,
      QualifiedNameOfFile SynExprLetOrUseBangContainsTheRangeOfTheEqualsSign, [],
      [SynModuleOrNamespace
         ([SynExprLetOrUseBangContainsTheRangeOfTheEqualsSign], false,
          AnonModule,
          [Expr
             (App
                (NonAtomic, false, Ident comp,
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
                           Named (SynIdent (x, None), false, None, (3,9--3,10)),
                           None, Ident y, (3,4--5,13), Yes (3,4--3,14),
                           { LeadingKeyword = Let (3,4--3,8)
                             InlineKeyword = None
                             EqualsRange = Some (3,11--3,12) });
                        SynBinding
                          (None, Normal, false, false, [], PreXmlDocEmpty,
                           SynValData
                             (None,
                              SynValInfo ([], SynArgInfo ([], false, None)),
                              None),
                           Named (SynIdent (z, None), false, None, (4,9--4,10)),
                           None,
                           App
                             (NonAtomic, false, Ident someFunction,
                              Const (Unit, (4,26--4,28)), (4,13--4,28)),
                           (4,4--4,28), Yes (4,4--4,28),
                           { LeadingKeyword = And (4,4--4,8)
                             InlineKeyword = None
                             EqualsRange = Some (4,11--4,12) })],
                       YieldOrReturn
                         ((false, true), Const (Unit, (5,11--5,13)), (5,4--5,13),
                          { YieldOrReturnKeyword = (5,4--5,10) }), (3,4--5,13),
                       { LetOrUseKeyword = (3,4--3,8)
                         InKeyword = None
                         EqualsRange = Some (3,11--3,12) }), (2,5--6,1)),
                 (2,0--6,1)), (2,0--6,1))], PreXmlDocEmpty, [], None, (2,0--6,1),
          { LeadingKeyword = None })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [] }, set []))
