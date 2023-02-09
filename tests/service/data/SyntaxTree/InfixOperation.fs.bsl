ImplFile
  (ParsedImplFileInput
     ("/root/InfixOperation.fs", false, QualifiedNameOfFile InfixOperation, [],
      [],
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
                       None, /root/InfixOperation.fs (2,2--2,3)),
                    Const (Int32 1, /root/InfixOperation.fs (2,0--2,1)),
                    /root/InfixOperation.fs (2,0--2,3)),
                 Const (Int32 1, /root/InfixOperation.fs (2,4--2,5)),
                 /root/InfixOperation.fs (2,0--2,5)),
              /root/InfixOperation.fs (2,0--2,5))], PreXmlDocEmpty, [], None,
          /root/InfixOperation.fs (2,0--2,5), { LeadingKeyword = None })],
      (true, false), { ConditionalDirectives = []
                       CodeComments = [] }, set []))