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
                        Fun
                          (WithNull
                             (LongIdent (SynLongIdent ([string], [], [None])),
                              false, (2,15--2,28), { BarRange = (2,22--2,23) }),
                           StaticConstant
                             (String ("456", Regular, (2,32--2,37)),
                              (2,32--2,37)), (2,15--2,37),
                           { ArrowRange = (2,29--2,31) }), (2,11--2,37)),
                     (2,2--2,37), Yes, { ArrowRange = Some (2,8--2,10)
                                         BarRange = Some (2,0--2,1) })],
                 (1,0--2,37), { MatchKeyword = (1,0--1,5)
                                WithKeyword = (1,8--1,12) }), (1,0--2,37))],
          PreXmlDocEmpty, [], None, (1,0--2,37), { LeadingKeyword = None })],
      (true, true), { ConditionalDirectives = []
                      CodeComments = [] }, set []))
