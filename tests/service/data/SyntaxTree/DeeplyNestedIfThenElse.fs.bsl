ImplFile
  (ParsedImplFileInput
     ("/root/DeeplyNestedIfThenElse.fs", false,
      QualifiedNameOfFile DeeplyNestedIfThenElse, [], [],
      [SynModuleOrNamespace
         ([DeeplyNestedIfThenElse], false, AnonModule,
          [Expr
             (IfThenElse
                (Ident a, Ident b,
                 Some
                   (IfThenElse
                      (Ident c, Ident d,
                       Some
                         (IfThenElse
                            (Ident e, Ident f, Some (Ident g),
                             Yes /root/DeeplyNestedIfThenElse.fs (6,8--6,17),
                             false, /root/DeeplyNestedIfThenElse.fs (6,8--9,13),
                             { IfKeyword =
                                /root/DeeplyNestedIfThenElse.fs (6,8--6,10)
                               IsElif = false
                               ThenKeyword =
                                /root/DeeplyNestedIfThenElse.fs (6,13--6,17)
                               ElseKeyword =
                                Some /root/DeeplyNestedIfThenElse.fs (8,8--8,12)
                               IfToThenRange =
                                /root/DeeplyNestedIfThenElse.fs (6,8--6,17) })),
                       Yes /root/DeeplyNestedIfThenElse.fs (3,0--3,11), false,
                       /root/DeeplyNestedIfThenElse.fs (3,0--9,13),
                       { IfKeyword = /root/DeeplyNestedIfThenElse.fs (3,0--3,4)
                         IsElif = true
                         ThenKeyword =
                          /root/DeeplyNestedIfThenElse.fs (3,7--3,11)
                         ElseKeyword =
                          Some /root/DeeplyNestedIfThenElse.fs (5,0--5,4)
                         IfToThenRange =
                          /root/DeeplyNestedIfThenElse.fs (3,0--3,11) })),
                 Yes /root/DeeplyNestedIfThenElse.fs (1,0--1,9), false,
                 /root/DeeplyNestedIfThenElse.fs (1,0--9,13),
                 { IfKeyword = /root/DeeplyNestedIfThenElse.fs (1,0--1,2)
                   IsElif = false
                   ThenKeyword = /root/DeeplyNestedIfThenElse.fs (1,5--1,9)
                   ElseKeyword = None
                   IfToThenRange = /root/DeeplyNestedIfThenElse.fs (1,0--1,9) }),
              /root/DeeplyNestedIfThenElse.fs (1,0--9,13))], PreXmlDocEmpty, [],
          None, /root/DeeplyNestedIfThenElse.fs (1,0--9,13),
          { LeadingKeyword = None })], (true, false),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))