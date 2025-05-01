ImplFile
  (ParsedImplFileInput
     ("/root/ModuleOrNamespace/Anon module 01.fs", false,
      QualifiedNameOfFile Anon module 01, [], [],
      [SynModuleOrNamespace
         ([Anon module 01], false, AnonModule,
          [Expr (Const (Unit, (1,0--1,2)), (1,0--1,2))], PreXmlDocEmpty, [],
          None, (1,0--1,2), { LeadingKeyword = None })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))

(1,0)-(1,2) parse warning The declarations in this file will be placed in an implicit module 'Anon module 01' based on the file name 'Anon module 01.fs'. However this is not a valid F# identifier, so the contents will not be accessible from other files. Consider renaming the file or adding a 'module' or 'namespace' declaration at the top of the file.
