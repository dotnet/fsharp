ImplFile
  (ParsedImplFileInput
     ("/root/NestedElseIfOnTheSameLineInIfThenElse.fs", false,
      QualifiedNameOfFile NestedElseIfOnTheSameLineInIfThenElse, [], [],
      [SynModuleOrNamespace
         ([NestedElseIfOnTheSameLineInIfThenElse], false, AnonModule,
          [Expr
             (IfThenElse
                (Ident a, Ident b,
                 Some
                   (IfThenElse
                      (Ident c, Ident d, None,
                       Yes
                         /root/NestedElseIfOnTheSameLineInIfThenElse.fs (3,5--3,14),
                       false,
                       /root/NestedElseIfOnTheSameLineInIfThenElse.fs (3,5--4,1),
                       { IfKeyword =
                          /root/NestedElseIfOnTheSameLineInIfThenElse.fs (3,5--3,7)
                         IsElif = false
                         ThenKeyword =
                          /root/NestedElseIfOnTheSameLineInIfThenElse.fs (3,10--3,14)
                         ElseKeyword = None
                         IfToThenRange =
                          /root/NestedElseIfOnTheSameLineInIfThenElse.fs (3,5--3,14) })),
                 Yes /root/NestedElseIfOnTheSameLineInIfThenElse.fs (1,0--1,9),
                 false,
                 /root/NestedElseIfOnTheSameLineInIfThenElse.fs (1,0--4,1),
                 { IfKeyword =
                    /root/NestedElseIfOnTheSameLineInIfThenElse.fs (1,0--1,2)
                   IsElif = false
                   ThenKeyword =
                    /root/NestedElseIfOnTheSameLineInIfThenElse.fs (1,5--1,9)
                   ElseKeyword =
                    Some
                      /root/NestedElseIfOnTheSameLineInIfThenElse.fs (3,0--3,4)
                   IfToThenRange =
                    /root/NestedElseIfOnTheSameLineInIfThenElse.fs (1,0--1,9) }),
              /root/NestedElseIfOnTheSameLineInIfThenElse.fs (1,0--4,1))],
          PreXmlDocEmpty, [], None,
          /root/NestedElseIfOnTheSameLineInIfThenElse.fs (1,0--4,1),
          { LeadingKeyword = None })], (true, false),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))