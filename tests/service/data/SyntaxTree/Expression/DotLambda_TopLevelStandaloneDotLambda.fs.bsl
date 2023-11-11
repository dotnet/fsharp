ImplFile
  (ParsedImplFileInput
     ("/root/Expression/DotLambda_TopLevelStandaloneDotLambda.fs", false,
      QualifiedNameOfFile DotLambda_TopLevelStandaloneDotLambda, [], [],
      [SynModuleOrNamespace
         ([DotLambda_TopLevelStandaloneDotLambda], false, AnonModule,
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
                   (App
                      (Atomic, false, Ident ToString, Const (Unit, (1,15--1,17)),
                       (1,7--1,17)), (1,5--1,17), { UnderscoreRange = (1,5--1,6)
                                                    DotRange = (1,6--1,7) }),
                 (1,0--1,17)), (1,0--1,17))], PreXmlDocEmpty, [], None,
          (1,0--1,17), { LeadingKeyword = None })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))
