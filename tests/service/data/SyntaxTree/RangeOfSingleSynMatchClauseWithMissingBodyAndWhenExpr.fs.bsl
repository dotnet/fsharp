ImplFile
  (ParsedImplFileInput
     ("/root/RangeOfSingleSynMatchClauseWithMissingBodyAndWhenExpr.fs", false,
      QualifiedNameOfFile RangeOfSingleSynMatchClauseWithMissingBodyAndWhenExpr,
      [], [],
      [SynModuleOrNamespace
         ([RangeOfSingleSynMatchClauseWithMissingBodyAndWhenExpr], false,
          AnonModule,
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
                           /root/RangeOfSingleSynMatchClauseWithMissingBodyAndWhenExpr.fs (2,8--2,15)),
                        None,
                        App
                          (NonAtomic, false, Ident tryDownloadFile, Ident url,
                           /root/RangeOfSingleSynMatchClauseWithMissingBodyAndWhenExpr.fs (2,18--2,37)),
                        /root/RangeOfSingleSynMatchClauseWithMissingBodyAndWhenExpr.fs (2,8--2,15),
                        Yes
                          /root/RangeOfSingleSynMatchClauseWithMissingBodyAndWhenExpr.fs (2,4--2,37),
                        { LeadingKeyword =
                           Let
                             /root/RangeOfSingleSynMatchClauseWithMissingBodyAndWhenExpr.fs (2,4--2,7)
                          InlineKeyword = None
                          EqualsRange =
                           Some
                             /root/RangeOfSingleSynMatchClauseWithMissingBodyAndWhenExpr.fs (2,16--2,17) })],
                    App
                      (NonAtomic, false, Ident Some, Ident content,
                       /root/RangeOfSingleSynMatchClauseWithMissingBodyAndWhenExpr.fs (3,4--3,16)),
                    /root/RangeOfSingleSynMatchClauseWithMissingBodyAndWhenExpr.fs (2,4--3,16),
                    { InKeyword = None }),
                 [SynMatchClause
                    (Named
                       (SynIdent (ex, None), false, None,
                        /root/RangeOfSingleSynMatchClauseWithMissingBodyAndWhenExpr.fs (5,2--5,4)),
                     Some
                       (Paren
                          (App
                             (NonAtomic, false, Ident isNull, Ident ex,
                              /root/RangeOfSingleSynMatchClauseWithMissingBodyAndWhenExpr.fs (5,11--5,20)),
                           /root/RangeOfSingleSynMatchClauseWithMissingBodyAndWhenExpr.fs (5,10--5,11),
                           Some
                             /root/RangeOfSingleSynMatchClauseWithMissingBodyAndWhenExpr.fs (5,20--5,21),
                           /root/RangeOfSingleSynMatchClauseWithMissingBodyAndWhenExpr.fs (5,10--5,21))),
                     ArbitraryAfterError
                       ("patternClauses2",
                        /root/RangeOfSingleSynMatchClauseWithMissingBodyAndWhenExpr.fs (5,21--5,21)),
                     /root/RangeOfSingleSynMatchClauseWithMissingBodyAndWhenExpr.fs (5,2--5,21),
                     Yes,
                     { ArrowRange = None
                       BarRange =
                        Some
                          /root/RangeOfSingleSynMatchClauseWithMissingBodyAndWhenExpr.fs (5,0--5,1) })],
                 /root/RangeOfSingleSynMatchClauseWithMissingBodyAndWhenExpr.fs (1,0--5,21),
                 Yes
                   /root/RangeOfSingleSynMatchClauseWithMissingBodyAndWhenExpr.fs (1,0--1,3),
                 Yes
                   /root/RangeOfSingleSynMatchClauseWithMissingBodyAndWhenExpr.fs (4,0--4,4),
                 { TryKeyword =
                    /root/RangeOfSingleSynMatchClauseWithMissingBodyAndWhenExpr.fs (1,0--1,3)
                   TryToWithRange =
                    /root/RangeOfSingleSynMatchClauseWithMissingBodyAndWhenExpr.fs (1,0--4,4)
                   WithKeyword =
                    /root/RangeOfSingleSynMatchClauseWithMissingBodyAndWhenExpr.fs (4,0--4,4)
                   WithToEndRange =
                    /root/RangeOfSingleSynMatchClauseWithMissingBodyAndWhenExpr.fs (4,0--5,21) }),
              /root/RangeOfSingleSynMatchClauseWithMissingBodyAndWhenExpr.fs (1,0--5,21))],
          PreXmlDocEmpty, [], None,
          /root/RangeOfSingleSynMatchClauseWithMissingBodyAndWhenExpr.fs (1,0--5,24),
          { LeadingKeyword = None })], (true, false),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))