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
                        /root/DestructedLambdaHasArrowRange.fs (2,4--2,13))],
                    /root/DestructedLambdaHasArrowRange.fs (2,4--2,13)),
                 Match
                   (NoneAtInvisible, Ident _arg1,
                    [SynMatchClause
                       (Record
                          ([(([], X),
                             /root/DestructedLambdaHasArrowRange.fs (2,8--2,9),
                             Named
                               (SynIdent (x, None), false, None,
                                /root/DestructedLambdaHasArrowRange.fs (2,10--2,11)))],
                           /root/DestructedLambdaHasArrowRange.fs (2,4--2,13)),
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
                                 /root/DestructedLambdaHasArrowRange.fs (2,19--2,20)),
                              Ident x,
                              /root/DestructedLambdaHasArrowRange.fs (2,17--2,20)),
                           Const
                             (Int32 2,
                              /root/DestructedLambdaHasArrowRange.fs (2,21--2,22)),
                           /root/DestructedLambdaHasArrowRange.fs (2,17--2,22)),
                        /root/DestructedLambdaHasArrowRange.fs (2,4--2,13), No,
                        { ArrowRange = None
                          BarRange = None })],
                    /root/DestructedLambdaHasArrowRange.fs (2,4--2,22),
                    { MatchKeyword =
                       /root/DestructedLambdaHasArrowRange.fs (2,4--2,22)
                      WithKeyword =
                       /root/DestructedLambdaHasArrowRange.fs (2,4--2,22) }),
                 Some
                   ([Record
                       ([(([], X),
                          /root/DestructedLambdaHasArrowRange.fs (2,8--2,9),
                          Named
                            (SynIdent (x, None), false, None,
                             /root/DestructedLambdaHasArrowRange.fs (2,10--2,11)))],
                        /root/DestructedLambdaHasArrowRange.fs (2,4--2,13))],
                    Match
                      (NoneAtInvisible, Ident _arg1,
                       [SynMatchClause
                          (Record
                             ([(([], X),
                                /root/DestructedLambdaHasArrowRange.fs (2,8--2,9),
                                Named
                                  (SynIdent (x, None), false, None,
                                   /root/DestructedLambdaHasArrowRange.fs (2,10--2,11)))],
                              /root/DestructedLambdaHasArrowRange.fs (2,4--2,13)),
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
                                    /root/DestructedLambdaHasArrowRange.fs (2,19--2,20)),
                                 Ident x,
                                 /root/DestructedLambdaHasArrowRange.fs (2,17--2,20)),
                              Const
                                (Int32 2,
                                 /root/DestructedLambdaHasArrowRange.fs (2,21--2,22)),
                              /root/DestructedLambdaHasArrowRange.fs (2,17--2,22)),
                           /root/DestructedLambdaHasArrowRange.fs (2,4--2,13),
                           No, { ArrowRange = None
                                 BarRange = None })],
                       /root/DestructedLambdaHasArrowRange.fs (2,4--2,22),
                       { MatchKeyword =
                          /root/DestructedLambdaHasArrowRange.fs (2,4--2,22)
                         WithKeyword =
                          /root/DestructedLambdaHasArrowRange.fs (2,4--2,22) })),
                 /root/DestructedLambdaHasArrowRange.fs (2,0--2,22),
                 { ArrowRange =
                    Some /root/DestructedLambdaHasArrowRange.fs (2,14--2,16) }),
              /root/DestructedLambdaHasArrowRange.fs (2,0--2,22))],
          PreXmlDocEmpty, [], None,
          /root/DestructedLambdaHasArrowRange.fs (2,0--3,0),
          { LeadingKeyword = None })], (true, false),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))