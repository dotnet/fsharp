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
                      (Ident c, Ident d, None,
                       Yes
                         /root/IfThenElse/NestedElifInIfThenElse.fs (4,0--4,11),
                       false,
                       /root/IfThenElse/NestedElifInIfThenElse.fs (4,0--4,13),
                       { IfKeyword =
                          /root/IfThenElse/NestedElifInIfThenElse.fs (4,0--4,4)
                         IsElif = true
                         ThenKeyword =
                          /root/IfThenElse/NestedElifInIfThenElse.fs (4,7--4,11)
                         ElseKeyword = None
                         IfToThenRange =
                          /root/IfThenElse/NestedElifInIfThenElse.fs (4,0--4,11) })),
                 Yes /root/IfThenElse/NestedElifInIfThenElse.fs (2,0--2,9),
                 false, /root/IfThenElse/NestedElifInIfThenElse.fs (2,0--4,13),
                 { IfKeyword =
                    /root/IfThenElse/NestedElifInIfThenElse.fs (2,0--2,2)
                   IsElif = false
                   ThenKeyword =
                    /root/IfThenElse/NestedElifInIfThenElse.fs (2,5--2,9)
                   ElseKeyword = None
                   IfToThenRange =
                    /root/IfThenElse/NestedElifInIfThenElse.fs (2,0--2,9) }),
              /root/IfThenElse/NestedElifInIfThenElse.fs (2,0--4,13))],
          PreXmlDocEmpty, [], None,
          /root/IfThenElse/NestedElifInIfThenElse.fs (2,0--5,0),
          { LeadingKeyword = None })], (true, false),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))
