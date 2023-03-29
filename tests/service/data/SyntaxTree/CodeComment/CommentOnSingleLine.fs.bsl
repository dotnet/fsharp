ImplFile
  (ParsedImplFileInput
     ("/root/CodeComment/CommentOnSingleLine.fs", false,
      QualifiedNameOfFile CommentOnSingleLine, [], [],
      [SynModuleOrNamespace
         ([CommentOnSingleLine], false, AnonModule,
          [Expr
             (App
                (Atomic, false, Ident foo, Const (Unit, (3,3--3,5)), (3,0--3,5)),
              (3,0--3,5))], PreXmlDocEmpty, [], None, (3,0--3,5),
          { LeadingKeyword = None })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [LineComment (2,0--2,11)] }, set []))
