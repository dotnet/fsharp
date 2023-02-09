ImplFile
  (ParsedImplFileInput
     ("/root/OptionalExpression.fs", false,
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
                             None, /root/OptionalExpression.fs (1,5--1,6)),
                          LongIdent
                            (true, SynLongIdent ([x], [], [None]), None,
                             /root/OptionalExpression.fs (1,3--1,4)),
                          /root/OptionalExpression.fs (1,3--1,6)),
                       Const (Int32 7, /root/OptionalExpression.fs (1,7--1,8)),
                       /root/OptionalExpression.fs (1,3--1,8)),
                    /root/OptionalExpression.fs (1,1--1,2),
                    Some /root/OptionalExpression.fs (1,8--1,9),
                    /root/OptionalExpression.fs (1,1--1,9)),
                 /root/OptionalExpression.fs (1,0--1,9)),
              /root/OptionalExpression.fs (1,0--1,9))], PreXmlDocEmpty, [], None,
          /root/OptionalExpression.fs (1,0--1,9), { LeadingKeyword = None })],
      (true, false), { ConditionalDirectives = []
                       CodeComments = [] }, set []))