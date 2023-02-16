ImplFile
  (ParsedImplFileInput
     ("/root/IfThenElse/NestedElseIfInIfThenElse.fs", false,
      QualifiedNameOfFile NestedElseIfInIfThenElse, [], [],
      [SynModuleOrNamespace
         ([NestedElseIfInIfThenElse], false, AnonModule,
          [Expr
             (IfThenElse
                (Ident a, Ident b,
                 Some
                   (IfThenElse
                      (Ident c, Ident d, None, Yes (5,4--5,13), false,
                       (5,4--5,15), { IfKeyword = (5,4--5,6)
                                      IsElif = false
                                      ThenKeyword = (5,9--5,13)
                                      ElseKeyword = None
                                      IfToThenRange = (5,4--5,13) })),
                 Yes (2,0--2,9), false, (2,0--5,15),
                 { IfKeyword = (2,0--2,2)
                   IsElif = false
                   ThenKeyword = (2,5--2,9)
                   ElseKeyword = Some (4,0--4,4)
                   IfToThenRange = (2,0--2,9) }), (2,0--5,15))], PreXmlDocEmpty,
          [], None, (2,0--6,0), { LeadingKeyword = None })], (true, false),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))
