ImplFile
  (ParsedImplFileInput
     ("/root/IfThenElse/CommentBetweenElseAndIf.fs", false,
      QualifiedNameOfFile CommentBetweenElseAndIf, [], [],
      [SynModuleOrNamespace
         ([CommentBetweenElseAndIf], false, AnonModule,
          [Expr
             (IfThenElse
                (Ident a, Ident b,
                 Some
                   (IfThenElse
                      (Ident c, Ident d, None, Yes (4,34--4,43), false,
                       (4,34--5,1), { IfKeyword = (4,34--4,36)
                                      IsElif = false
                                      ThenKeyword = (4,39--4,43)
                                      ElseKeyword = None
                                      IfToThenRange = (4,34--4,43) })),
                 Yes (2,0--2,9), false, (2,0--5,1),
                 { IfKeyword = (2,0--2,2)
                   IsElif = false
                   ThenKeyword = (2,5--2,9)
                   ElseKeyword = Some (4,0--4,4)
                   IfToThenRange = (2,0--2,9) }), (2,0--5,1))], PreXmlDocEmpty,
          [], None, (2,0--6,0), { LeadingKeyword = None })], (true, false),
      { ConditionalDirectives = []
        CodeComments = [BlockComment (4,5--4,33)] }, set []))
