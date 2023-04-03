ImplFile
  (ParsedImplFileInput
     ("/root/Pattern/NamedPatPairRecoverAfterEquals.fs", false,
      QualifiedNameOfFile NamedPatPairRecoverAfterEquals, [], [],
      [SynModuleOrNamespace
         ([NamedPatPairRecoverAfterEquals], false, AnonModule,
          [Expr
             (Match
                (Yes (1,0--1,12), Ident x,
                 [SynMatchClause
                    (LongIdent
                       (SynLongIdent ([Y], [], [None]), None, None,
                        NamePatPairs
                          ([(z, Some (2,6--2,7),
                             FromParseError (Wild (2,7--2,7), (2,7--2,7)))],
                           (2,4--2,9), { ParenRange = (2,3--2,9) }), None,
                        (2,2--2,9)), None, Const (Unit, (2,13--2,15)),
                     (2,2--2,15), Yes, { ArrowRange = Some (2,10--2,12)
                                         BarRange = Some (2,0--2,1) })],
                 (1,0--2,15), { MatchKeyword = (1,0--1,5)
                                WithKeyword = (1,8--1,12) }), (1,0--2,15))],
          PreXmlDocEmpty, [], None, (1,0--3,0), { LeadingKeyword = None })],
      (true, true), { ConditionalDirectives = []
                      CodeComments = [] }, set []))

(2,8)-(2,9) parse error Unexpected symbol ')' in pattern
