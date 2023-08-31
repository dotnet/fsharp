ImplFile
  (ParsedImplFileInput
     ("/root/Nullness/RegressionAnnotatedInlinePatternMatch.fs", false,
      QualifiedNameOfFile RegressionAnnotatedInlinePatternMatch, [], [],
      [SynModuleOrNamespace
         ([RegressionAnnotatedInlinePatternMatch], false, AnonModule,
          [Expr
             (Match
                (Yes (1,0--1,12), Ident x,
                 [SynMatchClause
                    (Const (String ("123", Regular, (2,2--2,7)), (2,2--2,7)),
                     None,
                     Typed
                       (Const (String ("", Regular, (2,11--2,13)), (2,11--2,13)),
                        LongIdent (SynLongIdent ([string], [], [None])),
                        (2,11--2,21)), (2,2--2,21), Yes,
                     { ArrowRange = Some (2,8--2,10)
                       BarRange = Some (2,0--2,1) });
                  SynMatchClause
                    (Null (3,2--3,6), None,
                     Const (String ("456", Regular, (3,10--3,15)), (3,10--3,15)),
                     (3,2--3,15), Yes, { ArrowRange = Some (3,7--3,9)
                                         BarRange = Some (3,0--3,1) })],
                 (1,0--3,15), { MatchKeyword = (1,0--1,5)
                                WithKeyword = (1,8--1,12) }), (1,0--3,15))],
          PreXmlDocEmpty, [], None, (1,0--3,15), { LeadingKeyword = None })],
      (true, true), { ConditionalDirectives = []
                      CodeComments = [] }, set []))
