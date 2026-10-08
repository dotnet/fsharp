ImplFile
  (ParsedImplFileInput
     ("/root/MatchClause/WhenKeywordOnItsOwnLine.fs", false,
      QualifiedNameOfFile WhenKeywordOnItsOwnLine, [],
      [SynModuleOrNamespace
         ([WhenKeywordOnItsOwnLine], false, AnonModule,
          [Expr
             (Match
                (Yes (1,0--1,12), Ident x,
                 [SynMatchClause
                    (Wild (2,2--2,3), Some (Ident a), Ident b, (2,2--4,14), Yes,
                     { ArrowRange = Some (4,10--4,12)
                       BarRange = Some (2,0--2,1)
                       WhenKeyword = Some (3,4--3,8) })], (1,0--4,14),
                 { MatchKeyword = (1,0--1,5)
                   WithKeyword = (1,8--1,12) }), (1,0--4,14))], PreXmlDocEmpty,
          [], None, (1,0--5,0), { LeadingKeyword = None })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [LineComment (3,9--3,13)] }, set []))
