ImplFile
  (ParsedImplFileInput
     ("/root/OperatorInMatchPattern.fs", false,
      QualifiedNameOfFile OperatorInMatchPattern, [], [],
      [SynModuleOrNamespace
         ([OperatorInMatchPattern], false, AnonModule,
          [Expr
             (Match
                (Yes /root/OperatorInMatchPattern.fs (2,0--2,12), Ident x,
                 [SynMatchClause
                    (ListCons
                       (Paren
                          (Named
                             (SynIdent (head, None), false, None,
                              /root/OperatorInMatchPattern.fs (3,3--3,7)),
                           /root/OperatorInMatchPattern.fs (3,2--3,8)),
                        Paren
                          (Named
                             (SynIdent (tail, None), false, None,
                              /root/OperatorInMatchPattern.fs (3,13--3,17)),
                           /root/OperatorInMatchPattern.fs (3,12--3,18)),
                        /root/OperatorInMatchPattern.fs (3,2--3,18),
                        { ColonColonRange =
                           /root/OperatorInMatchPattern.fs (3,9--3,11) }), None,
                     Const (Unit, /root/OperatorInMatchPattern.fs (3,22--3,24)),
                     /root/OperatorInMatchPattern.fs (3,2--3,24), Yes,
                     { ArrowRange =
                        Some /root/OperatorInMatchPattern.fs (3,19--3,21)
                       BarRange =
                        Some /root/OperatorInMatchPattern.fs (3,0--3,1) })],
                 /root/OperatorInMatchPattern.fs (2,0--3,24),
                 { MatchKeyword = /root/OperatorInMatchPattern.fs (2,0--2,5)
                   WithKeyword = /root/OperatorInMatchPattern.fs (2,8--2,12) }),
              /root/OperatorInMatchPattern.fs (2,0--3,24))], PreXmlDocEmpty, [],
          None, /root/OperatorInMatchPattern.fs (2,0--4,0),
          { LeadingKeyword = None })], (true, false),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))