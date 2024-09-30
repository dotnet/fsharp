ImplFile
  (ParsedImplFileInput
     ("/root/Nullness/MatchWithTypeCastParens.fs", false,
      QualifiedNameOfFile MatchWithTypeCastParens, [], [],
      [SynModuleOrNamespace
         ([MatchWithTypeCastParens], false, AnonModule,
          [Expr
             (Match
                (Yes (1,0--1,12), Ident x,
                 [SynMatchClause
                    (IsInst
                       (Paren
                          (WithNull
                             (LongIdent (SynLongIdent ([string], [], [None])),
                              false, (2,6--2,19), { BarRange = (2,13--2,14) }),
                           (2,5--2,20)), (2,2--2,20)), None,
                     Const (Unit, (2,24--2,26)), (2,2--2,26), Yes,
                     { ArrowRange = Some (2,21--2,23)
                       BarRange = Some (2,0--2,1) })], (1,0--2,26),
                 { MatchKeyword = (1,0--1,5)
                   WithKeyword = (1,8--1,12) }), (1,0--2,26))], PreXmlDocEmpty,
          [], None, (1,0--3,0), { LeadingKeyword = None })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))
