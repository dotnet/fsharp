ImplFile
  (ParsedImplFileInput
     ("/root/IfThenElse/NestedElifInIfThenElse.fs", false,
      QualifiedNameOfFile NestedElifInIfThenElse, [], [],
      [SynModuleOrNamespace
         ([NestedElifInIfThenElse], false, AnonModule,
          [Expr
             (IfThenElse
                (Ident a, Ident b,
                 Some
                   (IfThenElse
                      (Ident c, Ident d, None, Yes (4,0--4,11), false,
                       (4,0--4,13), { IfKeyword = (4,0--4,4)
                                      IsElif = true
                                      ThenKeyword = (4,7--4,11)
                                      ElseKeyword = None
                                      IfToThenRange = (4,0--4,11) })),
                 Yes (2,0--2,9), false, (2,0--4,13),
                 { IfKeyword = (2,0--2,2)
                   IsElif = false
                   ThenKeyword = (2,5--2,9)
                   ElseKeyword = None
                   IfToThenRange = (2,0--2,9) }), (2,0--4,13))], PreXmlDocEmpty,
          [], None, (2,0--5,0), { LeadingKeyword = None })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))
