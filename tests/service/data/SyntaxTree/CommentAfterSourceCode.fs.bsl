ImplFile
  (ParsedImplFileInput
     ("/root/CommentAfterSourceCode.fs", false,
      QualifiedNameOfFile CommentAfterSourceCode, [], [],
      [SynModuleOrNamespace
         ([CommentAfterSourceCode], false, AnonModule,
          [Expr
             (App
                (Atomic, false, Ident foo,
                 Const (Unit, /root/CommentAfterSourceCode.fs (1,3--1,5)),
                 /root/CommentAfterSourceCode.fs (1,0--1,5)),
              /root/CommentAfterSourceCode.fs (1,0--1,5))], PreXmlDocEmpty, [],
          None, /root/CommentAfterSourceCode.fs (1,0--1,5),
          { LeadingKeyword = None })], (true, false),
      { ConditionalDirectives = []
        CodeComments = [LineComment /root/CommentAfterSourceCode.fs (1,6--1,17)] },
      set []))