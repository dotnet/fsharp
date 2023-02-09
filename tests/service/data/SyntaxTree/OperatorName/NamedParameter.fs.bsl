ImplFile
  (ParsedImplFileInput
     ("/root/OperatorName/NamedParameter.fs", false,
      QualifiedNameOfFile NamedParameter, [], [],
      [SynModuleOrNamespace
         ([NamedParameter], false, AnonModule,
          [Expr
             (App
                (Atomic, false, Ident f,
                 Paren
                   (App
                      (NonAtomic, false,
                       App
                         (NonAtomic, true,
                          LongIdent
                            (false,
                             SynLongIdent
                               ([op_Equality], [], [Some (OriginalNotation "=")]),
                             None,
                             /root/OperatorName/NamedParameter.fs (2,3--2,4)),
                          Ident x,
                          /root/OperatorName/NamedParameter.fs (2,2--2,4)),
                       Const
                         (Int32 4,
                          /root/OperatorName/NamedParameter.fs (2,4--2,5)),
                       /root/OperatorName/NamedParameter.fs (2,2--2,5)),
                    /root/OperatorName/NamedParameter.fs (2,1--2,2),
                    Some /root/OperatorName/NamedParameter.fs (2,5--2,6),
                    /root/OperatorName/NamedParameter.fs (2,1--2,6)),
                 /root/OperatorName/NamedParameter.fs (2,0--2,6)),
              /root/OperatorName/NamedParameter.fs (2,0--2,6))], PreXmlDocEmpty,
          [], None, /root/OperatorName/NamedParameter.fs (2,0--2,6),
          { LeadingKeyword = None })], (true, false),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))
