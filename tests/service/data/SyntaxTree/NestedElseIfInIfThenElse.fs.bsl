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
                       Yes /root/NestedElseIfInIfThenElse.fs (4,4--4,13), false,
                       /root/NestedElseIfInIfThenElse.fs (4,4--4,15),
                       { IfKeyword =
                          /root/NestedElseIfInIfThenElse.fs (4,4--4,6)
                         IsElif = false
                         ThenKeyword =
                          /root/NestedElseIfInIfThenElse.fs (4,9--4,13)
                         ElseKeyword = None
                         IfToThenRange =
                          /root/NestedElseIfInIfThenElse.fs (4,4--4,13) })),
                 Yes /root/NestedElseIfInIfThenElse.fs (1,0--1,9), false,
                 /root/NestedElseIfInIfThenElse.fs (1,0--4,15),
                 { IfKeyword = /root/NestedElseIfInIfThenElse.fs (1,0--1,2)
                   IsElif = false
                   ThenKeyword = /root/NestedElseIfInIfThenElse.fs (1,5--1,9)
                   ElseKeyword =
                    Some /root/NestedElseIfInIfThenElse.fs (3,0--3,4)
                   IfToThenRange = /root/NestedElseIfInIfThenElse.fs (1,0--1,9) }),
              /root/NestedElseIfInIfThenElse.fs (1,0--4,15))], PreXmlDocEmpty,
          [], None, /root/NestedElseIfInIfThenElse.fs (1,0--4,15),
          { LeadingKeyword = None })], (true, false),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))