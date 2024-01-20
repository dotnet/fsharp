ImplFile
  (ParsedImplFileInput
     ("/root/Lambda/MultilineLambdaHasArrowRange.fs", false,
      QualifiedNameOfFile MultilineLambdaHasArrowRange, [], [],
      [SynModuleOrNamespace
         ([MultilineLambdaHasArrowRange], false, AnonModule,
          [Expr
             (Lambda
                (false, false,
                 SimplePats
                   ([Id (x, None, false, false, false, (2,4--2,5))], [],
                    (2,4--2,5)),
                 Lambda
                   (false, true,
                    SimplePats
                      ([Id (y, None, false, false, false, (2,6--2,7))], [],
                       (2,6--2,7)),
                    Lambda
                      (false, true,
                       SimplePats
                         ([Id (z, None, false, false, false, (2,8--2,9))], [],
                          (2,8--2,9)),
                       App
                         (NonAtomic, false,
                          App
                            (NonAtomic, true,
                             LongIdent
                               (false,
                                SynLongIdent
                                  ([op_Multiply], [],
                                   [Some (OriginalNotation "*")]), None,
                                (4,38--4,39)),
                             App
                               (NonAtomic, false,
                                App
                                  (NonAtomic, true,
                                   LongIdent
                                     (false,
                                      SynLongIdent
                                        ([op_Multiply], [],
                                         [Some (OriginalNotation "*")]), None,
                                      (4,34--4,35)), Ident x, (4,32--4,35)),
                                Ident y, (4,32--4,37)), (4,32--4,39)), Ident z,
                          (4,32--4,41)), None, (2,0--4,41),
                       { ArrowRange = Some (3,28--3,30) }), None, (2,0--4,41),
                    { ArrowRange = Some (3,28--3,30) }),
                 Some
                   ([Named (SynIdent (x, None), false, None, (2,4--2,5));
                     Named (SynIdent (y, None), false, None, (2,6--2,7));
                     Named (SynIdent (z, None), false, None, (2,8--2,9))],
                    App
                      (NonAtomic, false,
                       App
                         (NonAtomic, true,
                          LongIdent
                            (false,
                             SynLongIdent
                               ([op_Multiply], [], [Some (OriginalNotation "*")]),
                             None, (4,38--4,39)),
                          App
                            (NonAtomic, false,
                             App
                               (NonAtomic, true,
                                LongIdent
                                  (false,
                                   SynLongIdent
                                     ([op_Multiply], [],
                                      [Some (OriginalNotation "*")]), None,
                                   (4,34--4,35)), Ident x, (4,32--4,35)),
                             Ident y, (4,32--4,37)), (4,32--4,39)), Ident z,
                       (4,32--4,41))), (2,0--4,41),
                 { ArrowRange = Some (3,28--3,30) }), (2,0--4,41))],
          PreXmlDocEmpty, [], None, (2,0--5,0), { LeadingKeyword = None })],
      (true, true), { ConditionalDirectives = []
                      CodeComments = [] }, set []))
