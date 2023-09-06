ImplFile
  (ParsedImplFileInput
     ("/root/IfThenElse/Comment after else 01.fs", false,
      QualifiedNameOfFile Comment after else 01, [], [],
      [SynModuleOrNamespace
         ([Comment after else 01], false, AnonModule,
          [Expr
             (IfThenElse
                (Ident a, Ident b,
                 Some
                   (IfThenElse
                      (Ident c, Ident d, None, Yes (3,34--3,43), false,
                       (3,34--4,5), { IfKeyword = (3,34--3,36)
                                      IsElif = false
                                      ThenKeyword = (3,39--3,43)
                                      ElseKeyword = None
                                      IfToThenRange = (3,34--3,43) })),
                 Yes (1,0--1,9), false, (1,0--4,5),
                 { IfKeyword = (1,0--1,2)
                   IsElif = false
                   ThenKeyword = (1,5--1,9)
                   ElseKeyword = Some (3,0--3,4)
                   IfToThenRange = (1,0--1,9) }), (1,0--4,5))], PreXmlDocEmpty,
          [], None, (1,0--5,0), { LeadingKeyword = None })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [BlockComment (3,5--3,33)] }, set []))

(1,0)-(2,0) parse warning The declarations in this file will be placed in an implicit module 'Comment after else 01' based on the file name 'Comment after else 01.fs'. However this is not a valid F# identifier, so the contents will not be accessible from other files. Consider renaming the file or adding a 'module' or 'namespace' declaration at the top of the file.
