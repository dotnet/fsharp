ImplFile
  (ParsedImplFileInput
     ("/root/Expression/AnonymousRecords-02.fs", false,
      QualifiedNameOfFile AnonymousRecords-02, [], [],
      [SynModuleOrNamespace
         ([AnonymousRecords-02], false, AnonModule,
          [Expr
             (AnonRecd
                (false, None,
                 [(SynLongIdent ([X], [], [None]), Some (1,5--1,6),
                   Const (Int32 0, (1,7--1,8)))], (1,0--2,0),
                 { OpeningBraceRange = (1,0--1,2) }), (1,0--2,0))],
          PreXmlDocEmpty, [], None, (1,0--2,0), { LeadingKeyword = None })],
      (true, true), { ConditionalDirectives = []
                      CodeComments = [] }, set []))

(1,0)-(1,2) parse error Unmatched '{|'
(1,0)-(2,0) parse warning The declarations in this file will be placed in an implicit module 'AnonymousRecords-02' based on the file name 'AnonymousRecords-02.fs'. However this is not a valid F# identifier, so the contents will not be accessible from other files. Consider renaming the file or adding a 'module' or 'namespace' declaration at the top of the file.
