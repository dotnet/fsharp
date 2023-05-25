ImplFile
  (ParsedImplFileInput
     ("/root/Pattern/SynPatRecordContainsTheRangeOfTheEqualsSign.fs", false,
      QualifiedNameOfFile SynPatRecordContainsTheRangeOfTheEqualsSign, [], [],
      [SynModuleOrNamespace
         ([SynPatRecordContainsTheRangeOfTheEqualsSign], false, AnonModule,
          [Expr
             (Match
                (Yes (2,0--2,12), Ident x,
                 [SynMatchClause
                    (Record
                       ([(([], Foo), (3,8--3,9),
                          Named
                            (SynIdent (bar, None), false, None, (3,10--3,13)))],
                        (3,2--3,15)), None, Const (Unit, (3,19--3,21)),
                     (3,2--3,21), Yes, { ArrowRange = Some (3,16--3,18)
                                         BarRange = Some (3,0--3,1) });
                  SynMatchClause
                    (Wild (4,2--4,3), None, Const (Unit, (4,7--4,9)), (4,2--4,9),
                     Yes, { ArrowRange = Some (4,4--4,6)
                            BarRange = Some (4,0--4,1) })], (2,0--4,9),
                 { MatchKeyword = (2,0--2,5)
                   WithKeyword = (2,8--2,12) }), (2,0--4,9))], PreXmlDocEmpty,
          [], None, (2,0--5,0), { LeadingKeyword = None })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))
