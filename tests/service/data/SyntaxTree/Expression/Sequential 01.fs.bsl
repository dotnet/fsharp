ImplFile
  (ParsedImplFileInput
     ("/root/Expression/Sequential 01.fs", false,
      QualifiedNameOfFile Sequential 01, [], [],
      [SynModuleOrNamespace
         ([Sequential 01], false, AnonModule,
          [Expr
             (Do
                (Sequential
                   (SuppressNeither, true, Ident a, Ident b, (1,3--1,8),
                    { SeparatorRange = Some (1,5--1,6) }), (1,0--1,8)),
              (1,0--1,8))], PreXmlDocEmpty, [], None, (1,0--2,0),
          { LeadingKeyword = None })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))

(1,0)-(2,0) parse warning The declarations in this file will be placed in an implicit module 'Sequential 01' based on the file name 'Sequential 01.fs'. However this is not a valid F# identifier, so the contents will not be accessible from other files. Consider renaming the file or adding a 'module' or 'namespace' declaration at the top of the file.
