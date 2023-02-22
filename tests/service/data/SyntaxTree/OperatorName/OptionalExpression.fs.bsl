ImplFile
  (ParsedImplFileInput
     ("/root/OperatorName/OptionalExpression.fs", false,
      QualifiedNameOfFile OptionalExpression, [], [],
      [SynModuleOrNamespace
         ([OptionalExpression], false, AnonModule,
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
                             None, (2,5--2,6)),
                          LongIdent
                            (true, SynLongIdent ([x], [], [None]), None,
                             (2,3--2,4)), (2,3--2,6)),
                       Const (Int32 7, (2,7--2,8)), (2,3--2,8)), (2,1--2,2),
                    Some (2,8--2,9), (2,1--2,9)), (2,0--2,9)), (2,0--2,9))],
          PreXmlDocEmpty, [], None, (2,0--2,9), { LeadingKeyword = None })],
      (true, false), { ConditionalDirectives = []
                       CodeComments = [] }, set []))
