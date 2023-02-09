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
                      (Ident c, Ident d, None,
                       Yes
                         /root/IfThenElse/NestedElseIfInIfThenElse.fs (5,4--5,13),
                       false,
                       /root/IfThenElse/NestedElseIfInIfThenElse.fs (5,4--5,15),
                       { IfKeyword =
                          /root/IfThenElse/NestedElseIfInIfThenElse.fs (5,4--5,6)
                         IsElif = false
                         ThenKeyword =
                          /root/IfThenElse/NestedElseIfInIfThenElse.fs (5,9--5,13)
                         ElseKeyword = None
                         IfToThenRange =
                          /root/IfThenElse/NestedElseIfInIfThenElse.fs (5,4--5,13) })),
                 Yes /root/IfThenElse/NestedElseIfInIfThenElse.fs (2,0--2,9),
                 false, /root/IfThenElse/NestedElseIfInIfThenElse.fs (2,0--5,15),
                 { IfKeyword =
                    /root/IfThenElse/NestedElseIfInIfThenElse.fs (2,0--2,2)
                   IsElif = false
                   ThenKeyword =
                    /root/IfThenElse/NestedElseIfInIfThenElse.fs (2,5--2,9)
                   ElseKeyword =
                    Some /root/IfThenElse/NestedElseIfInIfThenElse.fs (4,0--4,4)
                   IfToThenRange =
                    /root/IfThenElse/NestedElseIfInIfThenElse.fs (2,0--2,9) }),
              /root/IfThenElse/NestedElseIfInIfThenElse.fs (2,0--5,15))],
          PreXmlDocEmpty, [], None,
          /root/IfThenElse/NestedElseIfInIfThenElse.fs (2,0--6,0),
          { LeadingKeyword = None })], (true, false),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))
