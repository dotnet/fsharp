ImplFile
  (ParsedImplFileInput
     ("/root/MatchClause/RangeOfSingleSynMatchClauseFollowedByBar.fs", false,
      QualifiedNameOfFile RangeOfSingleSynMatchClauseFollowedByBar, [], [],
      [SynModuleOrNamespace
         ([RangeOfSingleSynMatchClauseFollowedByBar], false, AnonModule,
          [Expr
             (TryWith
                (LetOrUse
                   (false, false,
                    [SynBinding
                       (None, Normal, false, false, [],
                        PreXmlDoc ((3,4), FSharp.Compiler.Xml.XmlDocCollector),
                        SynValData
                          (None, SynValInfo ([], SynArgInfo ([], false, None)),
                           None),
                        Named
                          (SynIdent (content, None), false, None, (3,8--3,15)),
                        None,
                        App
                          (NonAtomic, false, Ident tryDownloadFile, Ident url,
                           (3,18--3,37)), (3,8--3,15), Yes (3,4--3,37),
                        { LeadingKeyword = Let (3,4--3,7)
                          InlineKeyword = None
                          EqualsRange = Some (3,16--3,17) })],
                    App
                      (NonAtomic, false, Ident Some, Ident content, (4,4--4,16)),
                    (3,4--4,16), { InKeyword = None }),
                 [SynMatchClause
                    (Named (SynIdent (ex, None), false, None, (6,2--6,4)), None,
                     Const (Unit, (7,4--7,6)), (6,2--7,6), Yes,
                     { ArrowRange = Some (6,5--6,7)
                       BarRange = Some (6,0--6,1) })], (2,0--8,1),
                 Yes (2,0--2,3), Yes (5,0--5,4), { TryKeyword = (2,0--2,3)
                                                   TryToWithRange = (2,0--5,4)
                                                   WithKeyword = (5,0--5,4)
                                                   WithToEndRange = (5,0--8,1) }),
              (2,0--8,1))], PreXmlDocEmpty, [], None, (2,0--9,0),
          { LeadingKeyword = None })], (true, false),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))
