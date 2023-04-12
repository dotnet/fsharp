ImplFile
  (ParsedImplFileInput
     ("/root/Lambda/DestructedLambdaHasArrowRange.fs", false,
      QualifiedNameOfFile DestructedLambdaHasArrowRange, [], [],
      [SynModuleOrNamespace
         ([DestructedLambdaHasArrowRange], false, AnonModule,
          [Expr
             (Lambda
                (false, false,
                 SimplePats
                   ([Id (_arg1, None, true, false, false, (2,4--2,13))],
                    (2,4--2,13)),
                 Match
                   (NoneAtInvisible, Ident _arg1,
                    [SynMatchClause
                       (Record
                          ([(([], X), (2,8--2,9),
                             Named
                               (SynIdent (x, None), false, None, (2,10--2,11)))],
                           (2,4--2,13)), None,
                        App
                          (NonAtomic, false,
                           App
                             (NonAtomic, true,
                              LongIdent
                                (false,
                                 SynLongIdent
                                   ([op_Multiply], [],
                                    [Some (OriginalNotation "*")]), None,
                                 (2,19--2,20)), Ident x, (2,17--2,20)),
                           Const (Int32 2, (2,21--2,22)), (2,17--2,22)),
                        (2,4--2,13), No, { ArrowRange = None
                                           BarRange = None })], (2,4--2,22),
                    { MatchKeyword = (2,4--2,22)
                      WithKeyword = (2,4--2,22) }),
                 Some
                   ([Record
                       ([(([], X), (2,8--2,9),
                          Named (SynIdent (x, None), false, None, (2,10--2,11)))],
                        (2,4--2,13))],
                    Match
                      (NoneAtInvisible, Ident _arg1,
                       [SynMatchClause
                          (Record
                             ([(([], X), (2,8--2,9),
                                Named
                                  (SynIdent (x, None), false, None, (2,10--2,11)))],
                              (2,4--2,13)), None,
                           App
                             (NonAtomic, false,
                              App
                                (NonAtomic, true,
                                 LongIdent
                                   (false,
                                    SynLongIdent
                                      ([op_Multiply], [],
                                       [Some (OriginalNotation "*")]), None,
                                    (2,19--2,20)), Ident x, (2,17--2,20)),
                              Const (Int32 2, (2,21--2,22)), (2,17--2,22)),
                           (2,4--2,13), No, { ArrowRange = None
                                              BarRange = None })], (2,4--2,22),
                       { MatchKeyword = (2,4--2,22)
                         WithKeyword = (2,4--2,22) })), (2,0--2,22),
                 { ArrowRange = Some (2,14--2,16) }), (2,0--2,22))],
          PreXmlDocEmpty, [], None, (2,0--3,0), { LeadingKeyword = None })],
      (true, true), { ConditionalDirectives = []
                      CodeComments = [] }, set []))
