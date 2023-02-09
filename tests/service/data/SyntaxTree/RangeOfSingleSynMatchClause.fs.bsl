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
                        PreXmlDoc ((3,4), FSharp.Compiler.Xml.XmlDocCollector),
                        SynValData
                          (None, SynValInfo ([], SynArgInfo ([], false, None)),
                           None),
                        Named
                          (SynIdent (content, None), false, None,
                           /root/RangeOfSingleSynMatchClause.fs (3,8--3,15)),
                        None,
                        App
                          (NonAtomic, false, Ident tryDownloadFile, Ident url,
                           /root/RangeOfSingleSynMatchClause.fs (3,18--3,37)),
                        /root/RangeOfSingleSynMatchClause.fs (3,8--3,15),
                        Yes /root/RangeOfSingleSynMatchClause.fs (3,4--3,37),
                        { LeadingKeyword =
                           Let /root/RangeOfSingleSynMatchClause.fs (3,4--3,7)
                          InlineKeyword = None
                          EqualsRange =
                           Some
                             /root/RangeOfSingleSynMatchClause.fs (3,16--3,17) })],
                    App
                      (NonAtomic, false, Ident Some, Ident content,
                       /root/RangeOfSingleSynMatchClause.fs (4,4--4,16)),
                    /root/RangeOfSingleSynMatchClause.fs (3,4--4,16),
                    { InKeyword = None }),
                 [SynMatchClause
                    (Named
                       (SynIdent (ex, None), false, None,
                        /root/RangeOfSingleSynMatchClause.fs (5,5--5,7)), None,
                     Sequential
                       (SuppressNeither, true,
                        App
                          (NonAtomic, false,
                           LongIdent
                             (false,
                              SynLongIdent
                                ([Infrastructure; ReportWarning],
                                 [/root/RangeOfSingleSynMatchClause.fs (6,18--6,19)],
                                 [None; None]), None,
                              /root/RangeOfSingleSynMatchClause.fs (6,4--6,32)),
                           Ident ex,
                           /root/RangeOfSingleSynMatchClause.fs (6,4--6,35)),
                        Ident None,
                        /root/RangeOfSingleSynMatchClause.fs (6,4--7,8)),
                     /root/RangeOfSingleSynMatchClause.fs (5,5--7,8), Yes,
                     { ArrowRange =
                        Some /root/RangeOfSingleSynMatchClause.fs (5,8--5,10)
                       BarRange = None })],
                 /root/RangeOfSingleSynMatchClause.fs (2,0--7,8),
                 Yes /root/RangeOfSingleSynMatchClause.fs (2,0--2,3),
                 Yes /root/RangeOfSingleSynMatchClause.fs (5,0--5,4),
                 { TryKeyword = /root/RangeOfSingleSynMatchClause.fs (2,0--2,3)
                   TryToWithRange =
                    /root/RangeOfSingleSynMatchClause.fs (2,0--5,4)
                   WithKeyword = /root/RangeOfSingleSynMatchClause.fs (5,0--5,4)
                   WithToEndRange =
                    /root/RangeOfSingleSynMatchClause.fs (5,0--7,8) }),
              /root/RangeOfSingleSynMatchClause.fs (2,0--7,8))], PreXmlDocEmpty,
          [], None, /root/RangeOfSingleSynMatchClause.fs (2,0--8,0),
          { LeadingKeyword = None })], (true, false),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))