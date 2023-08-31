ImplFile
  (ParsedImplFileInput
     ("/root/Nullness/MatchWithTypeCast.fs", false,
      QualifiedNameOfFile MatchWithTypeCast, [], [],
      [SynModuleOrNamespace
         ([MatchWithTypeCast], false, AnonModule,
          [Expr
             (Match
                (Yes (1,0--1,12), Ident x,
                 [SynMatchClause
                    (Or
                       (IsInst
                          (LongIdent (SynLongIdent ([string], [], [None])),
                           (2,2--2,11)), Null (2,14--2,18), (2,2--2,18),
                        { BarRange = (2,12--2,13) }), None,
                     Const (Unit, (2,22--2,24)), (2,2--2,24), Yes,
                     { ArrowRange = Some (2,19--2,21)
                       BarRange = Some (2,0--2,1) })], (1,0--2,24),
                 { MatchKeyword = (1,0--1,5)
                   WithKeyword = (1,8--1,12) }), (1,0--2,24))], PreXmlDocEmpty,
          [], None, (1,0--3,0), { LeadingKeyword = None })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))
