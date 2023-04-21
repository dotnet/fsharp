ImplFile
  (ParsedImplFileInput
     ("/root/Expression/AnonymousRecords-03.fs", false,
      QualifiedNameOfFile AnonymousRecords-03, [], [],
      [SynModuleOrNamespace
         ([AnonymousRecords-03], false, AnonModule,
          [Expr
             (AnonRecd
                (true, None,
                 [(SynLongIdent ([X], [], [None]), Some (1,12--1,13),
                   Const (Int32 0, (1,14--1,15)))], (1,0--2,0),
                 { OpeningBraceRange = (1,7--1,9) }), (1,0--2,0))],
          PreXmlDocEmpty, [], None, (1,0--2,0), { LeadingKeyword = None })],
      (true, true), { ConditionalDirectives = []
                      CodeComments = [] }, set []))

(1,7)-(1,9) parse error Unmatched '{|'
(1,0)-(2,0) parse warning The declarations in this file will be placed in an implicit module 'AnonymousRecords-03' based on the file name 'AnonymousRecords-03.fs'. However this is not a valid F# identifier, so the contents will not be accessible from other files. Consider renaming the file or adding a 'module' or 'namespace' declaration at the top of the file.
