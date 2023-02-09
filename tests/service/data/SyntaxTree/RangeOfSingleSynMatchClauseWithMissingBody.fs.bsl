ImplFile
  (ParsedImplFileInput
     ("/root/RangeOfSingleSynMatchClauseWithMissingBody.fs", false,
      QualifiedNameOfFile RangeOfSingleSynMatchClauseWithMissingBody, [], [],
      [SynModuleOrNamespace
         ([RangeOfSingleSynMatchClauseWithMissingBody], false, AnonModule,
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
                          (SynIdent (content, None), false, None,
                           /root/RangeOfSingleSynMatchClauseWithMissingBody.fs (3,8--3,15)),
                        None,
                        App
                          (NonAtomic, false, Ident tryDownloadFile, Ident url,
                           /root/RangeOfSingleSynMatchClauseWithMissingBody.fs (3,18--3,37)),
                        /root/RangeOfSingleSynMatchClauseWithMissingBody.fs (3,8--3,15),
                        Yes
                          /root/RangeOfSingleSynMatchClauseWithMissingBody.fs (3,4--3,37),
                        { LeadingKeyword =
                           Let
                             /root/RangeOfSingleSynMatchClauseWithMissingBody.fs (3,4--3,7)
                          InlineKeyword = None
                          EqualsRange =
                           Some
                             /root/RangeOfSingleSynMatchClauseWithMissingBody.fs (3,16--3,17) })],
                    App
                      (NonAtomic, false, Ident Some, Ident content,
                       /root/RangeOfSingleSynMatchClauseWithMissingBody.fs (4,4--4,16)),
                    /root/RangeOfSingleSynMatchClauseWithMissingBody.fs (3,4--4,16),
                    { InKeyword = None }),
                 [SynMatchClause
                    (Named
                       (SynIdent (ex, None), false, None,
                        /root/RangeOfSingleSynMatchClauseWithMissingBody.fs (6,2--6,4)),
                     None,
                     ArbitraryAfterError
                       ("patternClauses2",
                        /root/RangeOfSingleSynMatchClauseWithMissingBody.fs (6,4--6,4)),
                     /root/RangeOfSingleSynMatchClauseWithMissingBody.fs (6,2--6,4),
                     Yes,
                     { ArrowRange = None
                       BarRange =
                        Some
                          /root/RangeOfSingleSynMatchClauseWithMissingBody.fs (6,0--6,1) })],
                 /root/RangeOfSingleSynMatchClauseWithMissingBody.fs (2,0--6,4),
                 Yes
                   /root/RangeOfSingleSynMatchClauseWithMissingBody.fs (2,0--2,3),
                 Yes
                   /root/RangeOfSingleSynMatchClauseWithMissingBody.fs (5,0--5,4),
                 { TryKeyword =
                    /root/RangeOfSingleSynMatchClauseWithMissingBody.fs (2,0--2,3)
                   TryToWithRange =
                    /root/RangeOfSingleSynMatchClauseWithMissingBody.fs (2,0--5,4)
                   WithKeyword =
                    /root/RangeOfSingleSynMatchClauseWithMissingBody.fs (5,0--5,4)
                   WithToEndRange =
                    /root/RangeOfSingleSynMatchClauseWithMissingBody.fs (5,0--6,4) }),
              /root/RangeOfSingleSynMatchClauseWithMissingBody.fs (2,0--6,4))],
          PreXmlDocEmpty, [], None,
          /root/RangeOfSingleSynMatchClauseWithMissingBody.fs (2,0--7,0),
          { LeadingKeyword = None })], (true, false),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))