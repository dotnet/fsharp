ImplFile
  (ParsedImplFileInput
     ("/root/OperatorName/InfixOperation.fs", false,
      QualifiedNameOfFile InfixOperation, [], [],
      [SynModuleOrNamespace
         ([InfixOperation], false, AnonModule,
          [Expr
             (App
                (NonAtomic, false,
                 App
                   (NonAtomic, true,
                    LongIdent
                      (false,
                       SynLongIdent
                         ([op_Addition], [], [Some (OriginalNotation "+")]),
                       None, /root/OperatorName/InfixOperation.fs (2,2--2,3)),
                    Const
                      (Int32 1, /root/OperatorName/InfixOperation.fs (2,0--2,1)),
                    /root/OperatorName/InfixOperation.fs (2,0--2,3)),
                 Const
                   (Int32 1, /root/OperatorName/InfixOperation.fs (2,4--2,5)),
                 /root/OperatorName/InfixOperation.fs (2,0--2,5)),
              /root/OperatorName/InfixOperation.fs (2,0--2,5))], PreXmlDocEmpty,
          [], None, /root/OperatorName/InfixOperation.fs (2,0--2,5),
          { LeadingKeyword = None })], (true, false),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))
