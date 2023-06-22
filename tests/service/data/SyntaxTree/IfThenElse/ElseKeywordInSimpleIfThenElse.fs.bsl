ImplFile
  (ParsedImplFileInput
     ("/root/IfThenElse/ElseKeywordInSimpleIfThenElse.fs", false,
      QualifiedNameOfFile ElseKeywordInSimpleIfThenElse, [], [],
      [SynModuleOrNamespace
         ([ElseKeywordInSimpleIfThenElse], false, AnonModule,
          [Expr
             (IfThenElse
                (Ident a, Ident b, Some (Ident c), Yes (2,0--2,9), false,
                 (2,0--2,18), { IfKeyword = (2,0--2,2)
                                IsElif = false
                                ThenKeyword = (2,5--2,9)
                                ElseKeyword = Some (2,12--2,16)
                                IfToThenRange = (2,0--2,9) }), (2,0--2,18))],
          PreXmlDocEmpty, [], None, (2,0--3,0), { LeadingKeyword = None })],
      (true, true), { ConditionalDirectives = []
                      CodeComments = [] }, set []))
