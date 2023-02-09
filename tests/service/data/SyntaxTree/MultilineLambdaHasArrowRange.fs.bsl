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
                        /root/MultilineLambdaHasArrowRange.fs (2,4--2,5))],
                    /root/MultilineLambdaHasArrowRange.fs (2,4--2,5)),
                 Lambda
                   (false, true,
                    SimplePats
                      ([Id
                          (y, None, false, false, false,
                           /root/MultilineLambdaHasArrowRange.fs (2,6--2,7))],
                       /root/MultilineLambdaHasArrowRange.fs (2,6--2,7)),
                    Lambda
                      (false, true,
                       SimplePats
                         ([Id
                             (z, None, false, false, false,
                              /root/MultilineLambdaHasArrowRange.fs (2,8--2,9))],
                          /root/MultilineLambdaHasArrowRange.fs (2,8--2,9)),
                       App
                         (NonAtomic, false,
                          App
                            (NonAtomic, true,
                             LongIdent
                               (false,
                                SynLongIdent
                                  ([op_Multiply], [],
                                   [Some (OriginalNotation "*")]), None,
                                /root/MultilineLambdaHasArrowRange.fs (4,38--4,39)),
                             App
                               (NonAtomic, false,
                                App
                                  (NonAtomic, true,
                                   LongIdent
                                     (false,
                                      SynLongIdent
                                        ([op_Multiply], [],
                                         [Some (OriginalNotation "*")]), None,
                                      /root/MultilineLambdaHasArrowRange.fs (4,34--4,35)),
                                   Ident x,
                                   /root/MultilineLambdaHasArrowRange.fs (4,32--4,35)),
                                Ident y,
                                /root/MultilineLambdaHasArrowRange.fs (4,32--4,37)),
                             /root/MultilineLambdaHasArrowRange.fs (4,32--4,39)),
                          Ident z,
                          /root/MultilineLambdaHasArrowRange.fs (4,32--4,41)),
                       None, /root/MultilineLambdaHasArrowRange.fs (2,0--4,41),
                       { ArrowRange =
                          Some
                            /root/MultilineLambdaHasArrowRange.fs (3,28--3,30) }),
                    None, /root/MultilineLambdaHasArrowRange.fs (2,0--4,41),
                    { ArrowRange =
                       Some /root/MultilineLambdaHasArrowRange.fs (3,28--3,30) }),
                 Some
                   ([Named
                       (SynIdent (x, None), false, None,
                        /root/MultilineLambdaHasArrowRange.fs (2,4--2,5));
                     Named
                       (SynIdent (y, None), false, None,
                        /root/MultilineLambdaHasArrowRange.fs (2,6--2,7));
                     Named
                       (SynIdent (z, None), false, None,
                        /root/MultilineLambdaHasArrowRange.fs (2,8--2,9))],
                    App
                      (NonAtomic, false,
                       App
                         (NonAtomic, true,
                          LongIdent
                            (false,
                             SynLongIdent
                               ([op_Multiply], [], [Some (OriginalNotation "*")]),
                             None,
                             /root/MultilineLambdaHasArrowRange.fs (4,38--4,39)),
                          App
                            (NonAtomic, false,
                             App
                               (NonAtomic, true,
                                LongIdent
                                  (false,
                                   SynLongIdent
                                     ([op_Multiply], [],
                                      [Some (OriginalNotation "*")]), None,
                                   /root/MultilineLambdaHasArrowRange.fs (4,34--4,35)),
                                Ident x,
                                /root/MultilineLambdaHasArrowRange.fs (4,32--4,35)),
                             Ident y,
                             /root/MultilineLambdaHasArrowRange.fs (4,32--4,37)),
                          /root/MultilineLambdaHasArrowRange.fs (4,32--4,39)),
                       Ident z,
                       /root/MultilineLambdaHasArrowRange.fs (4,32--4,41))),
                 /root/MultilineLambdaHasArrowRange.fs (2,0--4,41),
                 { ArrowRange =
                    Some /root/MultilineLambdaHasArrowRange.fs (3,28--3,30) }),
              /root/MultilineLambdaHasArrowRange.fs (2,0--4,41))],
          PreXmlDocEmpty, [], None,
          /root/MultilineLambdaHasArrowRange.fs (2,0--5,0),
          { LeadingKeyword = None })], (true, false),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))