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
                        PreXmlDoc ((2,4), FSharp.Compiler.Xml.XmlDocCollector),
                        SynValData
                          (None, SynValInfo ([], SynArgInfo ([], false, None)),
                           None),
                        Named
                          (SynIdent (content, None), false, None,
                           /root/RangeOfSingleSynMatchClauseWithMissingBody.fs (2,8--2,15)),
                        None,
                        App
                          (NonAtomic, false, Ident tryDownloadFile, Ident url,
                           /root/RangeOfSingleSynMatchClauseWithMissingBody.fs (2,18--2,37)),
                        /root/RangeOfSingleSynMatchClauseWithMissingBody.fs (2,8--2,15),
                        Yes
                          /root/RangeOfSingleSynMatchClauseWithMissingBody.fs (2,4--2,37),
                        { LeadingKeyword =
                           Let
                             /root/RangeOfSingleSynMatchClauseWithMissingBody.fs (2,4--2,7)
                          InlineKeyword = None
                          EqualsRange =
                           Some
                             /root/RangeOfSingleSynMatchClauseWithMissingBody.fs (2,16--2,17) })],
                    App
                      (NonAtomic, false, Ident Some, Ident content,
                       /root/RangeOfSingleSynMatchClauseWithMissingBody.fs (3,4--3,16)),
                    /root/RangeOfSingleSynMatchClauseWithMissingBody.fs (2,4--3,16),
                    { InKeyword = None }),
                 [SynMatchClause
                    (Named
                       (SynIdent (ex, None), false, None,
                        /root/RangeOfSingleSynMatchClauseWithMissingBody.fs (5,2--5,4)),
                     None,
                     ArbitraryAfterError
                       ("patternClauses2",
                        /root/RangeOfSingleSynMatchClauseWithMissingBody.fs (5,4--5,4)),
                     /root/RangeOfSingleSynMatchClauseWithMissingBody.fs (5,2--5,4),
                     Yes,
                     { ArrowRange = None
                       BarRange =
                        Some
                          /root/RangeOfSingleSynMatchClauseWithMissingBody.fs (5,0--5,1) })],
                 /root/RangeOfSingleSynMatchClauseWithMissingBody.fs (1,0--5,4),
                 Yes
                   /root/RangeOfSingleSynMatchClauseWithMissingBody.fs (1,0--1,3),
                 Yes
                   /root/RangeOfSingleSynMatchClauseWithMissingBody.fs (4,0--4,4),
                 { TryKeyword =
                    /root/RangeOfSingleSynMatchClauseWithMissingBody.fs (1,0--1,3)
                   TryToWithRange =
                    /root/RangeOfSingleSynMatchClauseWithMissingBody.fs (1,0--4,4)
                   WithKeyword =
                    /root/RangeOfSingleSynMatchClauseWithMissingBody.fs (4,0--4,4)
                   WithToEndRange =
                    /root/RangeOfSingleSynMatchClauseWithMissingBody.fs (4,0--5,4) }),
              /root/RangeOfSingleSynMatchClauseWithMissingBody.fs (1,0--5,4))],
          PreXmlDocEmpty, [], None,
          /root/RangeOfSingleSynMatchClauseWithMissingBody.fs (1,0--5,7),
          { LeadingKeyword = None })], (true, false),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))