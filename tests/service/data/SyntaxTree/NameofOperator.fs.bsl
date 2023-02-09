ImplFile
  (ParsedImplFileInput
     ("/root/NameofOperator.fs", false, QualifiedNameOfFile NameofOperator, [],
      [],
      [SynModuleOrNamespace
         ([NameofOperator], false, AnonModule,
          [Expr
             (App
                (Atomic, false, Ident nameof,
                 LongIdent
                   (false,
                    SynLongIdent
                      ([op_Addition], [],
                       [Some
                          (OriginalNotationWithParen
                             (/root/NameofOperator.fs (1,6--1,7), "+",
                              /root/NameofOperator.fs (1,8--1,9)))]), None,
                    /root/NameofOperator.fs (1,6--1,9)),
                 /root/NameofOperator.fs (1,0--1,9)),
              /root/NameofOperator.fs (1,0--1,9))], PreXmlDocEmpty, [], None,
          /root/NameofOperator.fs (1,0--1,9), { LeadingKeyword = None })],
      (true, false), { ConditionalDirectives = []
                       CodeComments = [] }, set []))