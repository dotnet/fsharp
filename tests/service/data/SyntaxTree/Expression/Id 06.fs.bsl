ImplFile
  (ParsedImplFileInput
     ("/root/Expression/Id 06.fs", false, QualifiedNameOfFile Id 06, [],
      [SynModuleOrNamespace
         ([Id 06], false, AnonModule,
          [Expr (FromParseError (Ident , (1,0--1,2)), (1,0--1,2))],
          PreXmlDocEmpty, [], None, (1,0--1,2), { LeadingKeyword = None })],
      (true, true), { ConditionalDirectives = []
                      WarnDirectives = []
                      CodeComments = [] }, set []))

(1,0)-(1,2) parse error This is not a valid identifier
(1,0)-(1,2) parse warning The declarations in this file will be placed in an implicit module 'Id 06' based on the file name 'Id 06.fs'. However this is not a valid F# identifier, so the contents will not be accessible from other files. Consider renaming the file or adding a 'module' or 'namespace' declaration at the top of the file.
