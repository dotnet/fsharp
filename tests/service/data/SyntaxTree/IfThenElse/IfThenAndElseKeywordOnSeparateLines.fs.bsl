ImplFile
  (ParsedImplFileInput
     ("/root/IfThenElse/IfThenAndElseKeywordOnSeparateLines.fs", false,
      QualifiedNameOfFile IfThenAndElseKeywordOnSeparateLines, [], [],
      [SynModuleOrNamespace
         ([IfThenAndElseKeywordOnSeparateLines], false, AnonModule,
          [Expr
             (IfThenElse
                (Ident a, Ident b, Some (Ident c), Yes (2,0--3,4), false,
                 (2,0--4,6), { IfKeyword = (2,0--2,2)
                               IsElif = false
                               ThenKeyword = (3,0--3,4)
                               ElseKeyword = Some (4,0--4,4)
                               IfToThenRange = (2,0--3,4) }), (2,0--4,6))],
          PreXmlDocEmpty, [], None, (2,0--5,0), { LeadingKeyword = None })],
      (true, false), { ConditionalDirectives = []
                       CodeComments = [] }, set []))
