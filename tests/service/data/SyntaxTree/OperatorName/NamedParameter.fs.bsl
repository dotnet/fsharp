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
                             None, (2,3--2,4)), Ident x, (2,2--2,4)),
                       Const (Int32 4, (2,4--2,5)), (2,2--2,5)), (2,1--2,2),
                    Some (2,5--2,6), (2,1--2,6)), (2,0--2,6)), (2,0--2,6))],
          PreXmlDocEmpty, [], None, (2,0--2,6), { LeadingKeyword = None })],
      (true, true), { ConditionalDirectives = []
                      CodeComments = [] }, set []))
