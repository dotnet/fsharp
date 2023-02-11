ImplFile
  (ParsedImplFileInput
     ("/root/IfThenElse/CommentBetweenElseAndIf.fs", false,
      QualifiedNameOfFile CommentBetweenElseAndIf, [], [],
      [SynModuleOrNamespace
         ([CommentBetweenElseAndIf], false, AnonModule,
          [Expr
             (IfThenElse
                (Ident a, Ident b,
                 Some
                   (IfThenElse
                      (Ident c, Ident d, None,
                       Yes
                         /root/IfThenElse/CommentBetweenElseAndIf.fs (4,34--4,43),
                       false,
                       /root/IfThenElse/CommentBetweenElseAndIf.fs (4,34--5,1),
                       { IfKeyword =
                          /root/IfThenElse/CommentBetweenElseAndIf.fs (4,34--4,36)
                         IsElif = false
                         ThenKeyword =
                          /root/IfThenElse/CommentBetweenElseAndIf.fs (4,39--4,43)
                         ElseKeyword = None
                         IfToThenRange =
                          /root/IfThenElse/CommentBetweenElseAndIf.fs (4,34--4,43) })),
                 Yes /root/IfThenElse/CommentBetweenElseAndIf.fs (2,0--2,9),
                 false, /root/IfThenElse/CommentBetweenElseAndIf.fs (2,0--5,1),
                 { IfKeyword =
                    /root/IfThenElse/CommentBetweenElseAndIf.fs (2,0--2,2)
                   IsElif = false
                   ThenKeyword =
                    /root/IfThenElse/CommentBetweenElseAndIf.fs (2,5--2,9)
                   ElseKeyword =
                    Some /root/IfThenElse/CommentBetweenElseAndIf.fs (4,0--4,4)
                   IfToThenRange =
                    /root/IfThenElse/CommentBetweenElseAndIf.fs (2,0--2,9) }),
              /root/IfThenElse/CommentBetweenElseAndIf.fs (2,0--5,1))],
          PreXmlDocEmpty, [], None,
          /root/IfThenElse/CommentBetweenElseAndIf.fs (2,0--6,0),
          { LeadingKeyword = None })], (true, false),
      { ConditionalDirectives = []
        CodeComments =
         [BlockComment /root/IfThenElse/CommentBetweenElseAndIf.fs (4,5--4,33)] },
      set []))
