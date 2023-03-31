ImplFile
  (ParsedImplFileInput
     ("/root/Pattern/OperatorInMatchPattern.fs", false,
      QualifiedNameOfFile OperatorInMatchPattern, [], [],
      [SynModuleOrNamespace
         ([OperatorInMatchPattern], false, AnonModule,
          [Expr
             (Match
                (Yes (2,0--2,12), Ident x,
                 [SynMatchClause
                    (ListCons
                       (Paren
                          (Named
                             (SynIdent (head, None), false, None, (3,3--3,7)),
                           (3,2--3,8)),
                        Paren
                          (Named
                             (SynIdent (tail, None), false, None, (3,13--3,17)),
                           (3,12--3,18)), (3,2--3,18),
                        { ColonColonRange = (3,9--3,11) }), None,
                     Const (Unit, (3,22--3,24)), (3,2--3,24), Yes,
                     { ArrowRange = Some (3,19--3,21)
                       BarRange = Some (3,0--3,1) })], (2,0--3,24),
                 { MatchKeyword = (2,0--2,5)
                   WithKeyword = (2,8--2,12) }), (2,0--3,24))], PreXmlDocEmpty,
          [], None, (2,0--4,0), { LeadingKeyword = None })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))
