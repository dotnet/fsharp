ImplFile
  (ParsedImplFileInput
     ("/root/IfThenElse/NestedElseIfOnTheSameLineInIfThenElse.fs", false,
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
                         /root/IfThenElse/NestedElseIfOnTheSameLineInIfThenElse.fs (4,5--4,14),
                       false,
                       /root/IfThenElse/NestedElseIfOnTheSameLineInIfThenElse.fs (4,5--5,1),
                       { IfKeyword =
                          /root/IfThenElse/NestedElseIfOnTheSameLineInIfThenElse.fs (4,5--4,7)
                         IsElif = false
                         ThenKeyword =
                          /root/IfThenElse/NestedElseIfOnTheSameLineInIfThenElse.fs (4,10--4,14)
                         ElseKeyword = None
                         IfToThenRange =
                          /root/IfThenElse/NestedElseIfOnTheSameLineInIfThenElse.fs (4,5--4,14) })),
                 Yes
                   /root/IfThenElse/NestedElseIfOnTheSameLineInIfThenElse.fs (2,0--2,9),
                 false,
                 /root/IfThenElse/NestedElseIfOnTheSameLineInIfThenElse.fs (2,0--5,1),
                 { IfKeyword =
                    /root/IfThenElse/NestedElseIfOnTheSameLineInIfThenElse.fs (2,0--2,2)
                   IsElif = false
                   ThenKeyword =
                    /root/IfThenElse/NestedElseIfOnTheSameLineInIfThenElse.fs (2,5--2,9)
                   ElseKeyword =
                    Some
                      /root/IfThenElse/NestedElseIfOnTheSameLineInIfThenElse.fs (4,0--4,4)
                   IfToThenRange =
                    /root/IfThenElse/NestedElseIfOnTheSameLineInIfThenElse.fs (2,0--2,9) }),
              /root/IfThenElse/NestedElseIfOnTheSameLineInIfThenElse.fs (2,0--5,1))],
          PreXmlDocEmpty, [], None,
          /root/IfThenElse/NestedElseIfOnTheSameLineInIfThenElse.fs (2,0--6,0),
          { LeadingKeyword = None })], (true, false),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))