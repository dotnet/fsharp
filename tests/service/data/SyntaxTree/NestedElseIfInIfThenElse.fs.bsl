ImplFile
  (ParsedImplFileInput
     ("/root/NestedElseIfInIfThenElse.fs", false,
      QualifiedNameOfFile NestedElseIfInIfThenElse, [], [],
      [SynModuleOrNamespace
         ([NestedElseIfInIfThenElse], false, AnonModule,
          [Expr
             (IfThenElse
                (Ident a, Ident b,
                 Some
                   (IfThenElse
                      (Ident c, Ident d, None,
                       Yes /root/NestedElseIfInIfThenElse.fs (5,4--5,13), false,
                       /root/NestedElseIfInIfThenElse.fs (5,4--5,15),
                       { IfKeyword =
                          /root/NestedElseIfInIfThenElse.fs (5,4--5,6)
                         IsElif = false
                         ThenKeyword =
                          /root/NestedElseIfInIfThenElse.fs (5,9--5,13)
                         ElseKeyword = None
                         IfToThenRange =
                          /root/NestedElseIfInIfThenElse.fs (5,4--5,13) })),
                 Yes /root/NestedElseIfInIfThenElse.fs (2,0--2,9), false,
                 /root/NestedElseIfInIfThenElse.fs (2,0--5,15),
                 { IfKeyword = /root/NestedElseIfInIfThenElse.fs (2,0--2,2)
                   IsElif = false
                   ThenKeyword = /root/NestedElseIfInIfThenElse.fs (2,5--2,9)
                   ElseKeyword =
                    Some /root/NestedElseIfInIfThenElse.fs (4,0--4,4)
                   IfToThenRange = /root/NestedElseIfInIfThenElse.fs (2,0--2,9) }),
              /root/NestedElseIfInIfThenElse.fs (2,0--5,15))], PreXmlDocEmpty,
          [], None, /root/NestedElseIfInIfThenElse.fs (2,0--6,0),
          { LeadingKeyword = None })], (true, false),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))