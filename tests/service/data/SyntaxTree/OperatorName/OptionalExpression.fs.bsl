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
                             None,
                             /root/OperatorName/OptionalExpression.fs (2,5--2,6)),
                          LongIdent
                            (true, SynLongIdent ([x], [], [None]), None,
                             /root/OperatorName/OptionalExpression.fs (2,3--2,4)),
                          /root/OperatorName/OptionalExpression.fs (2,3--2,6)),
                       Const
                         (Int32 7,
                          /root/OperatorName/OptionalExpression.fs (2,7--2,8)),
                       /root/OperatorName/OptionalExpression.fs (2,3--2,8)),
                    /root/OperatorName/OptionalExpression.fs (2,1--2,2),
                    Some /root/OperatorName/OptionalExpression.fs (2,8--2,9),
                    /root/OperatorName/OptionalExpression.fs (2,1--2,9)),
                 /root/OperatorName/OptionalExpression.fs (2,0--2,9)),
              /root/OperatorName/OptionalExpression.fs (2,0--2,9))],
          PreXmlDocEmpty, [], None,
          /root/OperatorName/OptionalExpression.fs (2,0--2,9),
          { LeadingKeyword = None })], (true, false),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))
