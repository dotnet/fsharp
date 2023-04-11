ImplFile
  (ParsedImplFileInput
     ("/root/OperatorName/OperatorAsFunction.fs", false,
      QualifiedNameOfFile OperatorAsFunction, [], [],
      [SynModuleOrNamespace
         ([OperatorAsFunction], false, AnonModule,
          [Expr
             (App
                (NonAtomic, false,
                 App
                   (NonAtomic, false,
                    LongIdent
                      (false,
                       SynLongIdent
                         ([op_Addition], [],
                          [Some
                             (OriginalNotationWithParen
                                ((2,0--2,1), "+", (2,2--2,3)))]), None,
                       (2,0--2,3)), Const (Int32 3, (2,4--2,5)), (2,0--2,5)),
                 Const (Int32 4, (2,6--2,7)), (2,0--2,7)), (2,0--2,7))],
          PreXmlDocEmpty, [], None, (2,0--2,7), { LeadingKeyword = None })],
      (true, true), { ConditionalDirectives = []
                      CodeComments = [] }, set []))
