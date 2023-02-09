ImplFile
  (ParsedImplFileInput
     ("/root/IfThenElse/DeeplyNestedIfThenElse.fs", false,
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
                             Yes
                               /root/IfThenElse/DeeplyNestedIfThenElse.fs (7,8--7,17),
                             false,
                             /root/IfThenElse/DeeplyNestedIfThenElse.fs (7,8--10,13),
                             { IfKeyword =
                                /root/IfThenElse/DeeplyNestedIfThenElse.fs (7,8--7,10)
                               IsElif = false
                               ThenKeyword =
                                /root/IfThenElse/DeeplyNestedIfThenElse.fs (7,13--7,17)
                               ElseKeyword =
                                Some
                                  /root/IfThenElse/DeeplyNestedIfThenElse.fs (9,8--9,12)
                               IfToThenRange =
                                /root/IfThenElse/DeeplyNestedIfThenElse.fs (7,8--7,17) })),
                       Yes
                         /root/IfThenElse/DeeplyNestedIfThenElse.fs (4,0--4,11),
                       false,
                       /root/IfThenElse/DeeplyNestedIfThenElse.fs (4,0--10,13),
                       { IfKeyword =
                          /root/IfThenElse/DeeplyNestedIfThenElse.fs (4,0--4,4)
                         IsElif = true
                         ThenKeyword =
                          /root/IfThenElse/DeeplyNestedIfThenElse.fs (4,7--4,11)
                         ElseKeyword =
                          Some
                            /root/IfThenElse/DeeplyNestedIfThenElse.fs (6,0--6,4)
                         IfToThenRange =
                          /root/IfThenElse/DeeplyNestedIfThenElse.fs (4,0--4,11) })),
                 Yes /root/IfThenElse/DeeplyNestedIfThenElse.fs (2,0--2,9),
                 false, /root/IfThenElse/DeeplyNestedIfThenElse.fs (2,0--10,13),
                 { IfKeyword =
                    /root/IfThenElse/DeeplyNestedIfThenElse.fs (2,0--2,2)
                   IsElif = false
                   ThenKeyword =
                    /root/IfThenElse/DeeplyNestedIfThenElse.fs (2,5--2,9)
                   ElseKeyword = None
                   IfToThenRange =
                    /root/IfThenElse/DeeplyNestedIfThenElse.fs (2,0--2,9) }),
              /root/IfThenElse/DeeplyNestedIfThenElse.fs (2,0--10,13))],
          PreXmlDocEmpty, [], None,
          /root/IfThenElse/DeeplyNestedIfThenElse.fs (2,0--11,0),
          { LeadingKeyword = None })], (true, false),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))