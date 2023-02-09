ImplFile
  (ParsedImplFileInput
     ("/root/RangeOfArrowInSynMatchClause.fs", false,
      QualifiedNameOfFile RangeOfArrowInSynMatchClause, [], [],
      [SynModuleOrNamespace
         ([RangeOfArrowInSynMatchClause], false, AnonModule,
          [Expr
             (Match
                (Yes /root/RangeOfArrowInSynMatchClause.fs (1,0--1,14),
                 Ident foo,
                 [SynMatchClause
                    (LongIdent
                       (SynLongIdent ([Bar], [], [None]), None, None,
                        Pats
                          [Named
                             (SynIdent (bar, None), false, None,
                              /root/RangeOfArrowInSynMatchClause.fs (2,6--2,9))],
                        None, /root/RangeOfArrowInSynMatchClause.fs (2,2--2,9)),
                     None,
                     Const
                       (Unit, /root/RangeOfArrowInSynMatchClause.fs (2,13--2,15)),
                     /root/RangeOfArrowInSynMatchClause.fs (2,2--2,15), Yes,
                     { ArrowRange =
                        Some /root/RangeOfArrowInSynMatchClause.fs (2,10--2,12)
                       BarRange =
                        Some /root/RangeOfArrowInSynMatchClause.fs (2,0--2,1) })],
                 /root/RangeOfArrowInSynMatchClause.fs (1,0--2,15),
                 { MatchKeyword =
                    /root/RangeOfArrowInSynMatchClause.fs (1,0--1,5)
                   WithKeyword =
                    /root/RangeOfArrowInSynMatchClause.fs (1,10--1,14) }),
              /root/RangeOfArrowInSynMatchClause.fs (1,0--2,15))],
          PreXmlDocEmpty, [], None,
          /root/RangeOfArrowInSynMatchClause.fs (1,0--2,15),
          { LeadingKeyword = None })], (true, false),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))