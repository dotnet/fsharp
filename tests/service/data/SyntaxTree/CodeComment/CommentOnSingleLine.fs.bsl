ImplFile
  (ParsedImplFileInput
     ("/root/CodeComment/CommentOnSingleLine.fs", false,
      QualifiedNameOfFile CommentOnSingleLine, [], [],
      [SynModuleOrNamespace
         ([CommentOnSingleLine], false, AnonModule,
          [Expr
             (App
                (Atomic, false, Ident foo,
                 Const
                   (Unit, /root/CodeComment/CommentOnSingleLine.fs (3,3--3,5)),
                 /root/CodeComment/CommentOnSingleLine.fs (3,0--3,5)),
              /root/CodeComment/CommentOnSingleLine.fs (3,0--3,5))],
          PreXmlDocEmpty, [], None,
          /root/CodeComment/CommentOnSingleLine.fs (3,0--3,5),
          { LeadingKeyword = None })], (true, false),
      { ConditionalDirectives = []
        CodeComments =
         [LineComment /root/CodeComment/CommentOnSingleLine.fs (2,0--2,11)] },
      set []))
