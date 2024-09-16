ImplFile
  (ParsedImplFileInput
     ("/root/Nullness/MatchWithTypeCastParensAndSeparateNullCase.fs", false,
      QualifiedNameOfFile MatchWithTypeCastParensAndSeparateNullCase, [], [],
      [SynModuleOrNamespace
         ([MatchWithTypeCastParensAndSeparateNullCase], false, AnonModule,
          [Expr
             (Match
                (Yes (1,0--1,12), Ident x,
                 [SynMatchClause
                    (Or
                       (IsInst
                          (Paren
                             (WithNull
                                (LongIdent (SynLongIdent ([string], [], [None])),
                                 false, (2,6--2,19), { BarRange = (2,13--2,14) }),
                              (2,5--2,20)), (2,2--2,20)), Null (2,23--2,27),
                        (2,2--2,27), { BarRange = (2,21--2,22) }), None,
                     Const (Unit, (2,31--2,33)), (2,2--2,33), Yes,
                     { ArrowRange = Some (2,28--2,30)
                       BarRange = Some (2,0--2,1) })], (1,0--2,33),
                 { MatchKeyword = (1,0--1,5)
                   WithKeyword = (1,8--1,12) }), (1,0--2,33))], PreXmlDocEmpty,
          [], None, (1,0--3,0), { LeadingKeyword = None })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))
