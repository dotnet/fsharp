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
                            (Ident e, Ident f, Some (Ident g), Yes (7,8--7,17),
                             false, (7,8--10,13),
                             { IfKeyword = (7,8--7,10)
                               IsElif = false
                               ThenKeyword = (7,13--7,17)
                               ElseKeyword = Some (9,8--9,12)
                               IfToThenRange = (7,8--7,17) })), Yes (4,0--4,11),
                       false, (4,0--10,13), { IfKeyword = (4,0--4,4)
                                              IsElif = true
                                              ThenKeyword = (4,7--4,11)
                                              ElseKeyword = Some (6,0--6,4)
                                              IfToThenRange = (4,0--4,11) })),
                 Yes (2,0--2,9), false, (2,0--10,13),
                 { IfKeyword = (2,0--2,2)
                   IsElif = false
                   ThenKeyword = (2,5--2,9)
                   ElseKeyword = None
                   IfToThenRange = (2,0--2,9) }), (2,0--10,13))], PreXmlDocEmpty,
          [], None, (2,0--11,0), { LeadingKeyword = None })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))
