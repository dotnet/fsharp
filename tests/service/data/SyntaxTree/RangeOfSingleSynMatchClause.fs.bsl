ImplFile
  (ParsedImplFileInput
     ("/root/RangeOfSingleSynMatchClause.fs", false,
      QualifiedNameOfFile RangeOfSingleSynMatchClause, [], [],
      [SynModuleOrNamespace
         ([RangeOfSingleSynMatchClause], false, AnonModule,
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
                           /root/RangeOfSingleSynMatchClause.fs (2,8--2,15)),
                        None,
                        App
                          (NonAtomic, false, Ident tryDownloadFile, Ident url,
                           /root/RangeOfSingleSynMatchClause.fs (2,18--2,37)),
                        /root/RangeOfSingleSynMatchClause.fs (2,8--2,15),
                        Yes /root/RangeOfSingleSynMatchClause.fs (2,4--2,37),
                        { LeadingKeyword =
                           Let /root/RangeOfSingleSynMatchClause.fs (2,4--2,7)
                          InlineKeyword = None
                          EqualsRange =
                           Some
                             /root/RangeOfSingleSynMatchClause.fs (2,16--2,17) })],
                    App
                      (NonAtomic, false, Ident Some, Ident content,
                       /root/RangeOfSingleSynMatchClause.fs (3,4--3,16)),
                    /root/RangeOfSingleSynMatchClause.fs (2,4--3,16),
                    { InKeyword = None }),
                 [SynMatchClause
                    (Named
                       (SynIdent (ex, None), false, None,
                        /root/RangeOfSingleSynMatchClause.fs (4,5--4,7)), None,
                     Sequential
                       (SuppressNeither, true,
                        App
                          (NonAtomic, false,
                           LongIdent
                             (false,
                              SynLongIdent
                                ([Infrastructure; ReportWarning],
                                 [/root/RangeOfSingleSynMatchClause.fs (5,18--5,19)],
                                 [None; None]), None,
                              /root/RangeOfSingleSynMatchClause.fs (5,4--5,32)),
                           Ident ex,
                           /root/RangeOfSingleSynMatchClause.fs (5,4--5,35)),
                        Ident None,
                        /root/RangeOfSingleSynMatchClause.fs (5,4--6,8)),
                     /root/RangeOfSingleSynMatchClause.fs (4,5--6,8), Yes,
                     { ArrowRange =
                        Some /root/RangeOfSingleSynMatchClause.fs (4,8--4,10)
                       BarRange = None })],
                 /root/RangeOfSingleSynMatchClause.fs (1,0--6,8),
                 Yes /root/RangeOfSingleSynMatchClause.fs (1,0--1,3),
                 Yes /root/RangeOfSingleSynMatchClause.fs (4,0--4,4),
                 { TryKeyword = /root/RangeOfSingleSynMatchClause.fs (1,0--1,3)
                   TryToWithRange =
                    /root/RangeOfSingleSynMatchClause.fs (1,0--4,4)
                   WithKeyword = /root/RangeOfSingleSynMatchClause.fs (4,0--4,4)
                   WithToEndRange =
                    /root/RangeOfSingleSynMatchClause.fs (4,0--6,8) }),
              /root/RangeOfSingleSynMatchClause.fs (1,0--6,8))], PreXmlDocEmpty,
          [], None, /root/RangeOfSingleSynMatchClause.fs (1,0--6,8),
          { LeadingKeyword = None })], (true, false),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))