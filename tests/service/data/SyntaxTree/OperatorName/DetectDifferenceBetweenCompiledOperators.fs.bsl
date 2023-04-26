ImplFile
  (ParsedImplFileInput
     ("/root/OperatorName/DetectDifferenceBetweenCompiledOperators.fs", false,
      QualifiedNameOfFile DetectDifferenceBetweenCompiledOperators, [], [],
      [SynModuleOrNamespace
         ([DetectDifferenceBetweenCompiledOperators], false, AnonModule,
          [Expr
             (App
                (NonAtomic, false,
                 App
                   (NonAtomic, false,
                    LongIdent
                      (false,
                       SynLongIdent
                         ([op_Addition], [],
                          [Some
                             (OriginalNotationWithParen
                                ((2,0--2,1), "+", (2,2--2,3)))]), None,
                       (2,0--2,3)), Ident a, (2,0--2,5)), Ident b, (2,0--2,7)),
              (2,0--2,7));
           Expr
             (App
                (NonAtomic, false,
                 App (NonAtomic, false, Ident op_Addition, Ident a, (3,0--3,13)),
                 Ident b, (3,0--3,15)), (3,0--3,15))], PreXmlDocEmpty, [], None,
          (2,0--3,15), { LeadingKeyword = None })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))
