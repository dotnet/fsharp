ImplFile
  (ParsedImplFileInput
     ("/root/Expression/AnonymousRecords-04.fs", false,
      QualifiedNameOfFile AnonymousRecords-04, [], [],
      [SynModuleOrNamespace
         ([AnonymousRecords-04], false, AnonModule,
          [Expr
             (AnonRecd
                (false, None, [], (1,0--1,2), { OpeningBraceRange = (1,0--1,2) }),
              (1,0--1,2))], PreXmlDocEmpty, [], None, (1,0--2,0),
          { LeadingKeyword = None })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))

(1,0)-(1,2) parse error Unmatched '{|'
(1,0)-(2,0) parse warning The declarations in this file will be placed in an implicit module 'AnonymousRecords-04' based on the file name 'AnonymousRecords-04.fs'. However this is not a valid F# identifier, so the contents will not be accessible from other files. Consider renaming the file or adding a 'module' or 'namespace' declaration at the top of the file.
