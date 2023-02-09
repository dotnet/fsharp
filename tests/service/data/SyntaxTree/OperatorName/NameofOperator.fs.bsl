ImplFile
  (ParsedImplFileInput
     ("/root/OperatorName/NameofOperator.fs", false,
      QualifiedNameOfFile NameofOperator, [], [],
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
                             (/root/OperatorName/NameofOperator.fs (2,6--2,7),
                              "+",
                              /root/OperatorName/NameofOperator.fs (2,8--2,9)))]),
                    None, /root/OperatorName/NameofOperator.fs (2,6--2,9)),
                 /root/OperatorName/NameofOperator.fs (2,0--2,9)),
              /root/OperatorName/NameofOperator.fs (2,0--2,9))], PreXmlDocEmpty,
          [], None, /root/OperatorName/NameofOperator.fs (2,0--2,9),
          { LeadingKeyword = None })], (true, false),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))