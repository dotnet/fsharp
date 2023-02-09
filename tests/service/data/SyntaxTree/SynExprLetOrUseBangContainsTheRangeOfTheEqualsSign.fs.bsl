ImplFile
  (ParsedImplFileInput
     ("/root/SynExprLetOrUseBangContainsTheRangeOfTheEqualsSign.fs", false,
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
                      (Yes
                         /root/SynExprLetOrUseBangContainsTheRangeOfTheEqualsSign.fs (2,4--2,14),
                       false, true,
                       Named
                         (SynIdent (x, None), false, None,
                          /root/SynExprLetOrUseBangContainsTheRangeOfTheEqualsSign.fs (2,9--2,10)),
                       Ident y,
                       [SynExprAndBang
                          (Yes
                             /root/SynExprLetOrUseBangContainsTheRangeOfTheEqualsSign.fs (3,4--4,10),
                           false, true,
                           Named
                             (SynIdent (z, None), false, None,
                              /root/SynExprLetOrUseBangContainsTheRangeOfTheEqualsSign.fs (3,9--3,10)),
                           App
                             (NonAtomic, false, Ident someFunction,
                              Const
                                (Unit,
                                 /root/SynExprLetOrUseBangContainsTheRangeOfTheEqualsSign.fs (3,26--3,28)),
                              /root/SynExprLetOrUseBangContainsTheRangeOfTheEqualsSign.fs (3,13--3,28)),
                           /root/SynExprLetOrUseBangContainsTheRangeOfTheEqualsSign.fs (3,4--3,28),
                           { EqualsRange =
                              /root/SynExprLetOrUseBangContainsTheRangeOfTheEqualsSign.fs (3,11--3,12)
                             InKeyword = None })],
                       YieldOrReturn
                         ((false, true),
                          Const
                            (Unit,
                             /root/SynExprLetOrUseBangContainsTheRangeOfTheEqualsSign.fs (4,11--4,13)),
                          /root/SynExprLetOrUseBangContainsTheRangeOfTheEqualsSign.fs (4,4--4,13)),
                       /root/SynExprLetOrUseBangContainsTheRangeOfTheEqualsSign.fs (2,4--4,13),
                       { EqualsRange =
                          Some
                            /root/SynExprLetOrUseBangContainsTheRangeOfTheEqualsSign.fs (2,11--2,12) }),
                    /root/SynExprLetOrUseBangContainsTheRangeOfTheEqualsSign.fs (1,5--5,1)),
                 /root/SynExprLetOrUseBangContainsTheRangeOfTheEqualsSign.fs (1,0--5,1)),
              /root/SynExprLetOrUseBangContainsTheRangeOfTheEqualsSign.fs (1,0--5,1))],
          PreXmlDocEmpty, [], None,
          /root/SynExprLetOrUseBangContainsTheRangeOfTheEqualsSign.fs (1,0--5,1),
          { LeadingKeyword = None })], (true, false),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))