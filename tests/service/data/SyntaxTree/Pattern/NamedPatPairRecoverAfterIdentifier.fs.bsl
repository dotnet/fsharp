ImplFile
  (ParsedImplFileInput
     ("/root/Pattern/NamedPatPairRecoverAfterIdentifier.fs", false,
      QualifiedNameOfFile NamedPatPairRecoverAfterIdentifier, [], [],
      [SynModuleOrNamespace
         ([NamedPatPairRecoverAfterIdentifier], false, AnonModule,
          [Expr
             (Match
                (Yes (1,0--1,12), Ident x,
                 [SynMatchClause
                    (LongIdent
                       (SynLongIdent ([Y], [], [None]), None, None,
                        NamePatPairs
                          ([(a, Some (2,6--2,7), Const (Int32 1, (2,8--2,9)));
                            (b, Some (2,13--2,14), Const (Int32 2, (2,15--2,16)));
                            (c, None,
                             FromParseError (Wild (2,19--2,19), (2,19--2,19)))],
                           (2,4--2,20), { ParenRange = (2,3--2,20) }), None,
                        (2,2--2,20)), None, Const (Unit, (2,24--2,26)),
                     (2,2--2,26), Yes, { ArrowRange = Some (2,21--2,23)
                                         BarRange = Some (2,0--2,1) })],
                 (1,0--2,26), { MatchKeyword = (1,0--1,5)
                                WithKeyword = (1,8--1,12) }), (1,0--2,26))],
          PreXmlDocEmpty, [], None, (1,0--3,0), { LeadingKeyword = None })],
      (true, true), { ConditionalDirectives = []
                      CodeComments = [] }, set []))

(2,19)-(2,20) parse error Unexpected symbol ')' in pattern. Expected '=' or other token.
