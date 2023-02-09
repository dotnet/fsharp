ImplFile
  (ParsedImplFileInput
     ("/root/OperatorInMatchPattern.fs", false,
      QualifiedNameOfFile OperatorInMatchPattern, [], [],
      [SynModuleOrNamespace
         ([OperatorInMatchPattern], false, AnonModule,
          [Expr
             (Match
                (Yes /root/OperatorInMatchPattern.fs (1,0--1,12), Ident x,
                 [SynMatchClause
                    (ListCons
                       (Paren
                          (Named
                             (SynIdent (head, None), false, None,
                              /root/OperatorInMatchPattern.fs (2,3--2,7)),
                           /root/OperatorInMatchPattern.fs (2,2--2,8)),
                        Paren
                          (Named
                             (SynIdent (tail, None), false, None,
                              /root/OperatorInMatchPattern.fs (2,13--2,17)),
                           /root/OperatorInMatchPattern.fs (2,12--2,18)),
                        /root/OperatorInMatchPattern.fs (2,2--2,18),
                        { ColonColonRange =
                           /root/OperatorInMatchPattern.fs (2,9--2,11) }), None,
                     Const (Unit, /root/OperatorInMatchPattern.fs (2,22--2,24)),
                     /root/OperatorInMatchPattern.fs (2,2--2,24), Yes,
                     { ArrowRange =
                        Some /root/OperatorInMatchPattern.fs (2,19--2,21)
                       BarRange =
                        Some /root/OperatorInMatchPattern.fs (2,0--2,1) })],
                 /root/OperatorInMatchPattern.fs (1,0--2,24),
                 { MatchKeyword = /root/OperatorInMatchPattern.fs (1,0--1,5)
                   WithKeyword = /root/OperatorInMatchPattern.fs (1,8--1,12) }),
              /root/OperatorInMatchPattern.fs (1,0--2,24))], PreXmlDocEmpty, [],
          None, /root/OperatorInMatchPattern.fs (1,0--2,24),
          { LeadingKeyword = None })], (true, false),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))