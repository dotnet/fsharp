ImplFile
  (ParsedImplFileInput
     ("/root/Pattern/SynPatOrContainsTheRangeOfTheBar.fs", false,
      QualifiedNameOfFile SynPatOrContainsTheRangeOfTheBar, [], [],
      [SynModuleOrNamespace
         ([SynPatOrContainsTheRangeOfTheBar], false, AnonModule,
          [Expr
             (Match
                (Yes (2,0--2,12), Ident x,
                 [SynMatchClause
                    (Or
                       (LongIdent
                          (SynLongIdent ([A], [], [None]), None, None, Pats [],
                           None, (3,2--3,3)),
                        LongIdent
                          (SynLongIdent ([B], [], [None]), None, None, Pats [],
                           None, (4,2--4,3)), (3,2--4,3),
                        { BarRange = (4,0--4,1) }), None,
                     Const (Unit, (4,7--4,9)), (3,2--4,9), Yes,
                     { ArrowRange = Some (4,4--4,6)
                       BarRange = Some (3,0--3,1) });
                  SynMatchClause
                    (Wild (5,2--5,3), None, Const (Unit, (5,7--5,9)), (5,2--5,9),
                     Yes, { ArrowRange = Some (5,4--5,6)
                            BarRange = Some (5,0--5,1) })], (2,0--5,9),
                 { MatchKeyword = (2,0--2,5)
                   WithKeyword = (2,8--2,12) }), (2,0--5,9))], PreXmlDocEmpty,
          [], None, (2,0--6,0), { LeadingKeyword = None })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))
