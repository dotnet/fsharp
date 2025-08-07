ImplFile
  (ParsedImplFileInput
     ("/root/OpenDeclaration/InExp_dot_lambda.fs", false,
      QualifiedNameOfFile InExp_dot_lambda, [],
      [SynModuleOrNamespace
         ([InExp_dot_lambda], false, AnonModule,
          [Expr
             (App
                (NonAtomic, false,
                 App
                   (NonAtomic, true,
                    LongIdent
                      (false,
                       SynLongIdent
                         ([op_PipeRight], [], [Some (OriginalNotation "|>")]),
                       None, (1,2--1,4)), Const (Int32 1, (1,0--1,1)),
                    (1,0--1,4)),
                 DotLambda
                   (Paren
                      (Open
                         (ModuleOrNamespace
                            (SynLongIdent ([Checked], [], [None]), (1,13--1,20)),
                          (1,8--1,20), (1,8--1,36),
                          Lambda
                            (false, false,
                             SimplePats
                               ([Id (x, None, false, false, false, (1,26--1,27))],
                                [], (1,26--1,27)),
                             App
                               (NonAtomic, false,
                                App
                                  (NonAtomic, true,
                                   LongIdent
                                     (false,
                                      SynLongIdent
                                        ([op_Addition], [],
                                         [Some (OriginalNotation "+")]), None,
                                      (1,33--1,34)), Ident x, (1,31--1,34)),
                                Const (Int32 1, (1,35--1,36)), (1,31--1,36)),
                             Some
                               ([Named
                                   (SynIdent (x, None), false, None,
                                    (1,26--1,27))],
                                App
                                  (NonAtomic, false,
                                   App
                                     (NonAtomic, true,
                                      LongIdent
                                        (false,
                                         SynLongIdent
                                           ([op_Addition], [],
                                            [Some (OriginalNotation "+")]), None,
                                         (1,33--1,34)), Ident x, (1,31--1,34)),
                                   Const (Int32 1, (1,35--1,36)), (1,31--1,36))),
                             (1,22--1,36), { ArrowRange = Some (1,28--1,30) })),
                       (1,7--1,8), Some (1,36--1,37), (1,7--1,37)), (1,5--1,37),
                    { UnderscoreRange = (1,5--1,6)
                      DotRange = (1,6--1,7) }), (1,0--1,37)), (1,0--1,37))],
          PreXmlDocEmpty, [], None, (1,0--1,37), { LeadingKeyword = None })],
      (true, true), { ConditionalDirectives = []
                      WarnDirectives = []
                      CodeComments = [] }, set []))
