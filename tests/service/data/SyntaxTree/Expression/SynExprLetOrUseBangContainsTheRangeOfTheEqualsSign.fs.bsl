ImplFile
  (ParsedImplFileInput
     ("/root/Expression/SynExprLetOrUseBangContainsTheRangeOfTheEqualsSign.fs",
      false,
      QualifiedNameOfFile SynExprLetOrUseBangContainsTheRangeOfTheEqualsSign, [],
      [],
      [SynModuleOrNamespace
         ([SynExprLetOrUseBangContainsTheRangeOfTheEqualsSign], false,
          AnonModule,
          [Expr
             (App
                (NonAtomic, false, Ident comp,
                 ComputationExpr
                   (false,
                    LetOrUseBang
                      (Yes (3,4--3,14), false, true,
                       Named (SynIdent (x, None), false, None, (3,9--3,10)),
                       Ident y,
                       [SynExprAndBang
                          (Yes (4,4--5,10), false, true,
                           Named (SynIdent (z, None), false, None, (4,9--4,10)),
                           App
                             (NonAtomic, false, Ident someFunction,
                              Const (Unit, (4,26--4,28)), (4,13--4,28)),
                           (4,4--4,28), { EqualsRange = (4,11--4,12)
                                          InKeyword = None })],
                       YieldOrReturn
                         ((false, true), Const (Unit, (5,11--5,13)), (5,4--5,13)),
                       (3,4--5,13), { EqualsRange = Some (3,11--3,12) }),
                    (2,5--6,1)), (2,0--6,1)), (2,0--6,1))], PreXmlDocEmpty, [],
          None, (2,0--6,1), { LeadingKeyword = None })], (true, false),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))
