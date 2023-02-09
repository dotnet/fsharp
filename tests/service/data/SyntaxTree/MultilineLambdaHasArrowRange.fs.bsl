ImplFile
  (ParsedImplFileInput
     ("/root/MultilineLambdaHasArrowRange.fs", false,
      QualifiedNameOfFile MultilineLambdaHasArrowRange, [], [],
      [SynModuleOrNamespace
         ([MultilineLambdaHasArrowRange], false, AnonModule,
          [Expr
             (Lambda
                (false, false,
                 SimplePats
                   ([Id
                       (x, None, false, false, false,
                        /root/MultilineLambdaHasArrowRange.fs (1,4--1,5))],
                    /root/MultilineLambdaHasArrowRange.fs (1,4--1,5)),
                 Lambda
                   (false, true,
                    SimplePats
                      ([Id
                          (y, None, false, false, false,
                           /root/MultilineLambdaHasArrowRange.fs (1,6--1,7))],
                       /root/MultilineLambdaHasArrowRange.fs (1,6--1,7)),
                    Lambda
                      (false, true,
                       SimplePats
                         ([Id
                             (z, None, false, false, false,
                              /root/MultilineLambdaHasArrowRange.fs (1,8--1,9))],
                          /root/MultilineLambdaHasArrowRange.fs (1,8--1,9)),
                       App
                         (NonAtomic, false,
                          App
                            (NonAtomic, true,
                             LongIdent
                               (false,
                                SynLongIdent
                                  ([op_Multiply], [],
                                   [Some (OriginalNotation "*")]), None,
                                /root/MultilineLambdaHasArrowRange.fs (3,38--3,39)),
                             App
                               (NonAtomic, false,
                                App
                                  (NonAtomic, true,
                                   LongIdent
                                     (false,
                                      SynLongIdent
                                        ([op_Multiply], [],
                                         [Some (OriginalNotation "*")]), None,
                                      /root/MultilineLambdaHasArrowRange.fs (3,34--3,35)),
                                   Ident x,
                                   /root/MultilineLambdaHasArrowRange.fs (3,32--3,35)),
                                Ident y,
                                /root/MultilineLambdaHasArrowRange.fs (3,32--3,37)),
                             /root/MultilineLambdaHasArrowRange.fs (3,32--3,39)),
                          Ident z,
                          /root/MultilineLambdaHasArrowRange.fs (3,32--3,41)),
                       None, /root/MultilineLambdaHasArrowRange.fs (1,0--3,41),
                       { ArrowRange =
                          Some
                            /root/MultilineLambdaHasArrowRange.fs (2,28--2,30) }),
                    None, /root/MultilineLambdaHasArrowRange.fs (1,0--3,41),
                    { ArrowRange =
                       Some /root/MultilineLambdaHasArrowRange.fs (2,28--2,30) }),
                 Some
                   ([Named
                       (SynIdent (x, None), false, None,
                        /root/MultilineLambdaHasArrowRange.fs (1,4--1,5));
                     Named
                       (SynIdent (y, None), false, None,
                        /root/MultilineLambdaHasArrowRange.fs (1,6--1,7));
                     Named
                       (SynIdent (z, None), false, None,
                        /root/MultilineLambdaHasArrowRange.fs (1,8--1,9))],
                    App
                      (NonAtomic, false,
                       App
                         (NonAtomic, true,
                          LongIdent
                            (false,
                             SynLongIdent
                               ([op_Multiply], [], [Some (OriginalNotation "*")]),
                             None,
                             /root/MultilineLambdaHasArrowRange.fs (3,38--3,39)),
                          App
                            (NonAtomic, false,
                             App
                               (NonAtomic, true,
                                LongIdent
                                  (false,
                                   SynLongIdent
                                     ([op_Multiply], [],
                                      [Some (OriginalNotation "*")]), None,
                                   /root/MultilineLambdaHasArrowRange.fs (3,34--3,35)),
                                Ident x,
                                /root/MultilineLambdaHasArrowRange.fs (3,32--3,35)),
                             Ident y,
                             /root/MultilineLambdaHasArrowRange.fs (3,32--3,37)),
                          /root/MultilineLambdaHasArrowRange.fs (3,32--3,39)),
                       Ident z,
                       /root/MultilineLambdaHasArrowRange.fs (3,32--3,41))),
                 /root/MultilineLambdaHasArrowRange.fs (1,0--3,41),
                 { ArrowRange =
                    Some /root/MultilineLambdaHasArrowRange.fs (2,28--2,30) }),
              /root/MultilineLambdaHasArrowRange.fs (1,0--3,41))],
          PreXmlDocEmpty, [], None,
          /root/MultilineLambdaHasArrowRange.fs (1,0--3,41),
          { LeadingKeyword = None })], (true, false),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))