ImplFile
  (ParsedImplFileInput
     ("/root/CommentAfterSourceCode.fs", false,
      QualifiedNameOfFile CommentAfterSourceCode, [], [],
      [SynModuleOrNamespace
         ([CommentAfterSourceCode], false, AnonModule,
          [Expr
             (App
                (Atomic, false, Ident foo,
                 Const (Unit, /root/CommentAfterSourceCode.fs (2,3--2,5)),
                 /root/CommentAfterSourceCode.fs (2,0--2,5)),
              /root/CommentAfterSourceCode.fs (2,0--2,5))], PreXmlDocEmpty, [],
          None, /root/CommentAfterSourceCode.fs (2,0--2,5),
          { LeadingKeyword = None })], (true, false),
      { ConditionalDirectives = []
        CodeComments = [LineComment /root/CommentAfterSourceCode.fs (2,6--2,17)] },
      set []))