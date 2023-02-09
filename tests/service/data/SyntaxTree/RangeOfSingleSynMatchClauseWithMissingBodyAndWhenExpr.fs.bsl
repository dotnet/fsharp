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
                        PreXmlDoc ((3,4), FSharp.Compiler.Xml.XmlDocCollector),
                        SynValData
                          (None, SynValInfo ([], SynArgInfo ([], false, None)),
                           None),
                        Named
                          (SynIdent (content, None), false, None,
                           /root/RangeOfSingleSynMatchClauseWithMissingBodyAndWhenExpr.fs (3,8--3,15)),
                        None,
                        App
                          (NonAtomic, false, Ident tryDownloadFile, Ident url,
                           /root/RangeOfSingleSynMatchClauseWithMissingBodyAndWhenExpr.fs (3,18--3,37)),
                        /root/RangeOfSingleSynMatchClauseWithMissingBodyAndWhenExpr.fs (3,8--3,15),
                        Yes
                          /root/RangeOfSingleSynMatchClauseWithMissingBodyAndWhenExpr.fs (3,4--3,37),
                        { LeadingKeyword =
                           Let
                             /root/RangeOfSingleSynMatchClauseWithMissingBodyAndWhenExpr.fs (3,4--3,7)
                          InlineKeyword = None
                          EqualsRange =
                           Some
                             /root/RangeOfSingleSynMatchClauseWithMissingBodyAndWhenExpr.fs (3,16--3,17) })],
                    App
                      (NonAtomic, false, Ident Some, Ident content,
                       /root/RangeOfSingleSynMatchClauseWithMissingBodyAndWhenExpr.fs (4,4--4,16)),
                    /root/RangeOfSingleSynMatchClauseWithMissingBodyAndWhenExpr.fs (3,4--4,16),
                    { InKeyword = None }),
                 [SynMatchClause
                    (Named
                       (SynIdent (ex, None), false, None,
                        /root/RangeOfSingleSynMatchClauseWithMissingBodyAndWhenExpr.fs (6,2--6,4)),
                     Some
                       (Paren
                          (App
                             (NonAtomic, false, Ident isNull, Ident ex,
                              /root/RangeOfSingleSynMatchClauseWithMissingBodyAndWhenExpr.fs (6,11--6,20)),
                           /root/RangeOfSingleSynMatchClauseWithMissingBodyAndWhenExpr.fs (6,10--6,11),
                           Some
                             /root/RangeOfSingleSynMatchClauseWithMissingBodyAndWhenExpr.fs (6,20--6,21),
                           /root/RangeOfSingleSynMatchClauseWithMissingBodyAndWhenExpr.fs (6,10--6,21))),
                     ArbitraryAfterError
                       ("patternClauses2",
                        /root/RangeOfSingleSynMatchClauseWithMissingBodyAndWhenExpr.fs (6,21--6,21)),
                     /root/RangeOfSingleSynMatchClauseWithMissingBodyAndWhenExpr.fs (6,2--6,21),
                     Yes,
                     { ArrowRange = None
                       BarRange =
                        Some
                          /root/RangeOfSingleSynMatchClauseWithMissingBodyAndWhenExpr.fs (6,0--6,1) })],
                 /root/RangeOfSingleSynMatchClauseWithMissingBodyAndWhenExpr.fs (2,0--6,21),
                 Yes
                   /root/RangeOfSingleSynMatchClauseWithMissingBodyAndWhenExpr.fs (2,0--2,3),
                 Yes
                   /root/RangeOfSingleSynMatchClauseWithMissingBodyAndWhenExpr.fs (5,0--5,4),
                 { TryKeyword =
                    /root/RangeOfSingleSynMatchClauseWithMissingBodyAndWhenExpr.fs (2,0--2,3)
                   TryToWithRange =
                    /root/RangeOfSingleSynMatchClauseWithMissingBodyAndWhenExpr.fs (2,0--5,4)
                   WithKeyword =
                    /root/RangeOfSingleSynMatchClauseWithMissingBodyAndWhenExpr.fs (5,0--5,4)
                   WithToEndRange =
                    /root/RangeOfSingleSynMatchClauseWithMissingBodyAndWhenExpr.fs (5,0--6,21) }),
              /root/RangeOfSingleSynMatchClauseWithMissingBodyAndWhenExpr.fs (2,0--6,21))],
          PreXmlDocEmpty, [], None,
          /root/RangeOfSingleSynMatchClauseWithMissingBodyAndWhenExpr.fs (2,0--7,0),
          { LeadingKeyword = None })], (true, false),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))