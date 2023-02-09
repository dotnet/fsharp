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
                       Yes /root/NestedElifInIfThenElse.fs (3,0--3,11), false,
                       /root/NestedElifInIfThenElse.fs (3,0--3,13),
                       { IfKeyword = /root/NestedElifInIfThenElse.fs (3,0--3,4)
                         IsElif = true
                         ThenKeyword =
                          /root/NestedElifInIfThenElse.fs (3,7--3,11)
                         ElseKeyword = None
                         IfToThenRange =
                          /root/NestedElifInIfThenElse.fs (3,0--3,11) })),
                 Yes /root/NestedElifInIfThenElse.fs (1,0--1,9), false,
                 /root/NestedElifInIfThenElse.fs (1,0--3,13),
                 { IfKeyword = /root/NestedElifInIfThenElse.fs (1,0--1,2)
                   IsElif = false
                   ThenKeyword = /root/NestedElifInIfThenElse.fs (1,5--1,9)
                   ElseKeyword = None
                   IfToThenRange = /root/NestedElifInIfThenElse.fs (1,0--1,9) }),
              /root/NestedElifInIfThenElse.fs (1,0--3,13))], PreXmlDocEmpty, [],
          None, /root/NestedElifInIfThenElse.fs (1,0--3,13),
          { LeadingKeyword = None })], (true, false),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))