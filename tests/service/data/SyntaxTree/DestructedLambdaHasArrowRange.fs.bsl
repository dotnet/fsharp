ImplFile
  (ParsedImplFileInput
     ("/root/DestructedLambdaHasArrowRange.fs", false,
      QualifiedNameOfFile DestructedLambdaHasArrowRange, [], [],
      [SynModuleOrNamespace
         ([DestructedLambdaHasArrowRange], false, AnonModule,
          [Expr
             (Lambda
                (false, false,
                 SimplePats
                   ([Id
                       (_arg1, None, true, false, false,
                        /root/DestructedLambdaHasArrowRange.fs (1,4--1,13))],
                    /root/DestructedLambdaHasArrowRange.fs (1,4--1,13)),
                 Match
                   (NoneAtInvisible, Ident _arg1,
                    [SynMatchClause
                       (Record
                          ([(([], X),
                             /root/DestructedLambdaHasArrowRange.fs (1,8--1,9),
                             Named
                               (SynIdent (x, None), false, None,
                                /root/DestructedLambdaHasArrowRange.fs (1,10--1,11)))],
                           /root/DestructedLambdaHasArrowRange.fs (1,4--1,13)),
                        None,
                        App
                          (NonAtomic, false,
                           App
                             (NonAtomic, true,
                              LongIdent
                                (false,
                                 SynLongIdent
                                   ([op_Multiply], [],
                                    [Some (OriginalNotation "*")]), None,
                                 /root/DestructedLambdaHasArrowRange.fs (1,19--1,20)),
                              Ident x,
                              /root/DestructedLambdaHasArrowRange.fs (1,17--1,20)),
                           Const
                             (Int32 2,
                              /root/DestructedLambdaHasArrowRange.fs (1,21--1,22)),
                           /root/DestructedLambdaHasArrowRange.fs (1,17--1,22)),
                        /root/DestructedLambdaHasArrowRange.fs (1,4--1,13), No,
                        { ArrowRange = None
                          BarRange = None })],
                    /root/DestructedLambdaHasArrowRange.fs (1,4--1,22),
                    { MatchKeyword =
                       /root/DestructedLambdaHasArrowRange.fs (1,4--1,22)
                      WithKeyword =
                       /root/DestructedLambdaHasArrowRange.fs (1,4--1,22) }),
                 Some
                   ([Record
                       ([(([], X),
                          /root/DestructedLambdaHasArrowRange.fs (1,8--1,9),
                          Named
                            (SynIdent (x, None), false, None,
                             /root/DestructedLambdaHasArrowRange.fs (1,10--1,11)))],
                        /root/DestructedLambdaHasArrowRange.fs (1,4--1,13))],
                    Match
                      (NoneAtInvisible, Ident _arg1,
                       [SynMatchClause
                          (Record
                             ([(([], X),
                                /root/DestructedLambdaHasArrowRange.fs (1,8--1,9),
                                Named
                                  (SynIdent (x, None), false, None,
                                   /root/DestructedLambdaHasArrowRange.fs (1,10--1,11)))],
                              /root/DestructedLambdaHasArrowRange.fs (1,4--1,13)),
                           None,
                           App
                             (NonAtomic, false,
                              App
                                (NonAtomic, true,
                                 LongIdent
                                   (false,
                                    SynLongIdent
                                      ([op_Multiply], [],
                                       [Some (OriginalNotation "*")]), None,
                                    /root/DestructedLambdaHasArrowRange.fs (1,19--1,20)),
                                 Ident x,
                                 /root/DestructedLambdaHasArrowRange.fs (1,17--1,20)),
                              Const
                                (Int32 2,
                                 /root/DestructedLambdaHasArrowRange.fs (1,21--1,22)),
                              /root/DestructedLambdaHasArrowRange.fs (1,17--1,22)),
                           /root/DestructedLambdaHasArrowRange.fs (1,4--1,13),
                           No, { ArrowRange = None
                                 BarRange = None })],
                       /root/DestructedLambdaHasArrowRange.fs (1,4--1,22),
                       { MatchKeyword =
                          /root/DestructedLambdaHasArrowRange.fs (1,4--1,22)
                         WithKeyword =
                          /root/DestructedLambdaHasArrowRange.fs (1,4--1,22) })),
                 /root/DestructedLambdaHasArrowRange.fs (1,0--1,22),
                 { ArrowRange =
                    Some /root/DestructedLambdaHasArrowRange.fs (1,14--1,16) }),
              /root/DestructedLambdaHasArrowRange.fs (1,0--1,22))],
          PreXmlDocEmpty, [], None,
          /root/DestructedLambdaHasArrowRange.fs (1,0--1,22),
          { LeadingKeyword = None })], (true, false),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))