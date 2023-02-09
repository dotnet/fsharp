ImplFile
  (ParsedImplFileInput
     ("/root/RangeOfArrowInSynMatchClause.fs", false,
      QualifiedNameOfFile RangeOfArrowInSynMatchClause, [], [],
      [SynModuleOrNamespace
         ([RangeOfArrowInSynMatchClause], false, AnonModule,
          [Expr
             (Match
                (Yes /root/RangeOfArrowInSynMatchClause.fs (2,0--2,14),
                 Ident foo,
                 [SynMatchClause
                    (LongIdent
                       (SynLongIdent ([Bar], [], [None]), None, None,
                        Pats
                          [Named
                             (SynIdent (bar, None), false, None,
                              /root/RangeOfArrowInSynMatchClause.fs (3,6--3,9))],
                        None, /root/RangeOfArrowInSynMatchClause.fs (3,2--3,9)),
                     None,
                     Const
                       (Unit, /root/RangeOfArrowInSynMatchClause.fs (3,13--3,15)),
                     /root/RangeOfArrowInSynMatchClause.fs (3,2--3,15), Yes,
                     { ArrowRange =
                        Some /root/RangeOfArrowInSynMatchClause.fs (3,10--3,12)
                       BarRange =
                        Some /root/RangeOfArrowInSynMatchClause.fs (3,0--3,1) })],
                 /root/RangeOfArrowInSynMatchClause.fs (2,0--3,15),
                 { MatchKeyword =
                    /root/RangeOfArrowInSynMatchClause.fs (2,0--2,5)
                   WithKeyword =
                    /root/RangeOfArrowInSynMatchClause.fs (2,10--2,14) }),
              /root/RangeOfArrowInSynMatchClause.fs (2,0--3,15))],
          PreXmlDocEmpty, [], None,
          /root/RangeOfArrowInSynMatchClause.fs (2,0--4,0),
          { LeadingKeyword = None })], (true, false),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))