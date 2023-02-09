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
                       None, /root/InfixOperation.fs (1,2--1,3)),
                    Const (Int32 1, /root/InfixOperation.fs (1,0--1,1)),
                    /root/InfixOperation.fs (1,0--1,3)),
                 Const (Int32 1, /root/InfixOperation.fs (1,4--1,5)),
                 /root/InfixOperation.fs (1,0--1,5)),
              /root/InfixOperation.fs (1,0--1,5))], PreXmlDocEmpty, [], None,
          /root/InfixOperation.fs (1,0--1,5), { LeadingKeyword = None })],
      (true, false), { ConditionalDirectives = []
                       CodeComments = [] }, set []))