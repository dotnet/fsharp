ImplFile
  (ParsedImplFileInput
     ("/root/DotLambda/TopLevelStandaloneDotLambda.fs", false,
      QualifiedNameOfFile TopLevelStandaloneDotLambda, [], [],
      [SynModuleOrNamespace
         ([TopLevelStandaloneDotLambda], false, AnonModule,
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

(1,5)-(1,6) parse error  _. shorthand syntax for lambda functions can only be used with atomic expressions. That means expressions with no whitespace unless enclosed in parentheses.
