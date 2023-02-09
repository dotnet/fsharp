ImplFile
  (ParsedImplFileInput
     ("/root/CommentBetweenElseAndIf.fs", false,
      QualifiedNameOfFile CommentBetweenElseAndIf, [], [],
      [SynModuleOrNamespace
         ([CommentBetweenElseAndIf], false, AnonModule,
          [Expr
             (IfThenElse
                (Ident a, Ident b,
                 Some
                   (IfThenElse
                      (Ident c, Ident d, None,
                       Yes /root/CommentBetweenElseAndIf.fs (3,34--3,43), false,
                       /root/CommentBetweenElseAndIf.fs (3,34--4,1),
                       { IfKeyword =
                          /root/CommentBetweenElseAndIf.fs (3,34--3,36)
                         IsElif = false
                         ThenKeyword =
                          /root/CommentBetweenElseAndIf.fs (3,39--3,43)
                         ElseKeyword = None
                         IfToThenRange =
                          /root/CommentBetweenElseAndIf.fs (3,34--3,43) })),
                 Yes /root/CommentBetweenElseAndIf.fs (1,0--1,9), false,
                 /root/CommentBetweenElseAndIf.fs (1,0--4,1),
                 { IfKeyword = /root/CommentBetweenElseAndIf.fs (1,0--1,2)
                   IsElif = false
                   ThenKeyword = /root/CommentBetweenElseAndIf.fs (1,5--1,9)
                   ElseKeyword =
                    Some /root/CommentBetweenElseAndIf.fs (3,0--3,4)
                   IfToThenRange = /root/CommentBetweenElseAndIf.fs (1,0--1,9) }),
              /root/CommentBetweenElseAndIf.fs (1,0--4,1))], PreXmlDocEmpty, [],
          None, /root/CommentBetweenElseAndIf.fs (1,0--4,1),
          { LeadingKeyword = None })], (true, false),
      { ConditionalDirectives = []
        CodeComments =
         [BlockComment /root/CommentBetweenElseAndIf.fs (3,5--3,33)] }, set []))