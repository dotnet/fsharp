ImplFile
  (ParsedImplFileInput
     ("/root/Expression/Sequential 03.fs", false,
      QualifiedNameOfFile Sequential 03, [], [],
      [SynModuleOrNamespace
         ([Sequential 03], false, AnonModule,
          [Expr
             (Do
                (Sequential
                   (SuppressNeither, false, Ident a,
                    Paren
                      (Ident b, (1,10--1,15), Some (1,18--1,21), (1,10--1,21)),
                    (1,3--1,21), { SeparatorRange = Some (1,5--1,9) }),
                 (1,0--1,21)), (1,0--1,21))], PreXmlDocEmpty, [], None,
          (1,0--2,0), { LeadingKeyword = None })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))

(1,0)-(2,0) parse warning The declarations in this file will be placed in an implicit module 'Sequential 03' based on the file name 'Sequential 03.fs'. However this is not a valid F# identifier, so the contents will not be accessible from other files. Consider renaming the file or adding a 'module' or 'namespace' declaration at the top of the file.
