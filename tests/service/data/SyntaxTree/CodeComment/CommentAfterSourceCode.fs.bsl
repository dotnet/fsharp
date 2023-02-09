ImplFile
  (ParsedImplFileInput
     ("/root/CodeComment/CommentAfterSourceCode.fs", false,
      QualifiedNameOfFile CommentAfterSourceCode, [], [],
      [SynModuleOrNamespace
         ([CommentAfterSourceCode], false, AnonModule,
          [Expr
             (App
                (Atomic, false, Ident foo,
                 Const
                   (Unit, /root/CodeComment/CommentAfterSourceCode.fs (2,3--2,5)),
                 /root/CodeComment/CommentAfterSourceCode.fs (2,0--2,5)),
              /root/CodeComment/CommentAfterSourceCode.fs (2,0--2,5))],
          PreXmlDocEmpty, [], None,
          /root/CodeComment/CommentAfterSourceCode.fs (2,0--2,5),
          { LeadingKeyword = None })], (true, false),
      { ConditionalDirectives = []
        CodeComments =
         [LineComment /root/CodeComment/CommentAfterSourceCode.fs (2,6--2,17)] },
      set []))