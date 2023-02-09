ImplFile
  (ParsedImplFileInput
     ("/root/NestedElifInIfThenElse.fs", false,
      QualifiedNameOfFile NestedElifInIfThenElse, [], [],
      [SynModuleOrNamespace
         ([NestedElifInIfThenElse], false, AnonModule,
          [Expr
             (IfThenElse
                (Ident a, Ident b,
                 Some
                   (IfThenElse
                      (Ident c, Ident d, None,
                       Yes /root/NestedElifInIfThenElse.fs (4,0--4,11), false,
                       /root/NestedElifInIfThenElse.fs (4,0--4,13),
                       { IfKeyword = /root/NestedElifInIfThenElse.fs (4,0--4,4)
                         IsElif = true
                         ThenKeyword =
                          /root/NestedElifInIfThenElse.fs (4,7--4,11)
                         ElseKeyword = None
                         IfToThenRange =
                          /root/NestedElifInIfThenElse.fs (4,0--4,11) })),
                 Yes /root/NestedElifInIfThenElse.fs (2,0--2,9), false,
                 /root/NestedElifInIfThenElse.fs (2,0--4,13),
                 { IfKeyword = /root/NestedElifInIfThenElse.fs (2,0--2,2)
                   IsElif = false
                   ThenKeyword = /root/NestedElifInIfThenElse.fs (2,5--2,9)
                   ElseKeyword = None
                   IfToThenRange = /root/NestedElifInIfThenElse.fs (2,0--2,9) }),
              /root/NestedElifInIfThenElse.fs (2,0--4,13))], PreXmlDocEmpty, [],
          None, /root/NestedElifInIfThenElse.fs (2,0--5,0),
          { LeadingKeyword = None })], (true, false),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))