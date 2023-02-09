ImplFile
  (ParsedImplFileInput
     ("/root/RangeOfMultipleSynMatchClause.fs", false,
      QualifiedNameOfFile RangeOfMultipleSynMatchClause, [], [],
      [SynModuleOrNamespace
         ([RangeOfMultipleSynMatchClause], false, AnonModule,
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
                           /root/RangeOfMultipleSynMatchClause.fs (2,8--2,15)),
                        None,
                        App
                          (NonAtomic, false, Ident tryDownloadFile, Ident url,
                           /root/RangeOfMultipleSynMatchClause.fs (2,18--2,37)),
                        /root/RangeOfMultipleSynMatchClause.fs (2,8--2,15),
                        Yes /root/RangeOfMultipleSynMatchClause.fs (2,4--2,37),
                        { LeadingKeyword =
                           Let /root/RangeOfMultipleSynMatchClause.fs (2,4--2,7)
                          InlineKeyword = None
                          EqualsRange =
                           Some
                             /root/RangeOfMultipleSynMatchClause.fs (2,16--2,17) })],
                    App
                      (NonAtomic, false, Ident Some, Ident content,
                       /root/RangeOfMultipleSynMatchClause.fs (3,4--3,16)),
                    /root/RangeOfMultipleSynMatchClause.fs (2,4--3,16),
                    { InKeyword = None }),
                 [SynMatchClause
                    (Named
                       (SynIdent (ex, None), false, None,
                        /root/RangeOfMultipleSynMatchClause.fs (5,2--5,4)), None,
                     Sequential
                       (SuppressNeither, true,
                        App
                          (NonAtomic, false,
                           LongIdent
                             (false,
                              SynLongIdent
                                ([Infrastructure; ReportWarning],
                                 [/root/RangeOfMultipleSynMatchClause.fs (6,18--6,19)],
                                 [None; None]), None,
                              /root/RangeOfMultipleSynMatchClause.fs (6,4--6,32)),
                           Ident ex,
                           /root/RangeOfMultipleSynMatchClause.fs (6,4--6,35)),
                        Ident None,
                        /root/RangeOfMultipleSynMatchClause.fs (6,4--7,8)),
                     /root/RangeOfMultipleSynMatchClause.fs (5,2--7,8), Yes,
                     { ArrowRange =
                        Some /root/RangeOfMultipleSynMatchClause.fs (5,5--5,7)
                       BarRange =
                        Some /root/RangeOfMultipleSynMatchClause.fs (5,0--5,1) });
                  SynMatchClause
                    (Named
                       (SynIdent (exx, None), false, None,
                        /root/RangeOfMultipleSynMatchClause.fs (8,2--8,5)), None,
                     Ident None,
                     /root/RangeOfMultipleSynMatchClause.fs (8,2--9,8), Yes,
                     { ArrowRange =
                        Some /root/RangeOfMultipleSynMatchClause.fs (8,6--8,8)
                       BarRange =
                        Some /root/RangeOfMultipleSynMatchClause.fs (8,0--8,1) })],
                 /root/RangeOfMultipleSynMatchClause.fs (1,0--9,8),
                 Yes /root/RangeOfMultipleSynMatchClause.fs (1,0--1,3),
                 Yes /root/RangeOfMultipleSynMatchClause.fs (4,0--4,4),
                 { TryKeyword =
                    /root/RangeOfMultipleSynMatchClause.fs (1,0--1,3)
                   TryToWithRange =
                    /root/RangeOfMultipleSynMatchClause.fs (1,0--4,4)
                   WithKeyword =
                    /root/RangeOfMultipleSynMatchClause.fs (4,0--4,4)
                   WithToEndRange =
                    /root/RangeOfMultipleSynMatchClause.fs (4,0--9,8) }),
              /root/RangeOfMultipleSynMatchClause.fs (1,0--9,8))],
          PreXmlDocEmpty, [], None,
          /root/RangeOfMultipleSynMatchClause.fs (1,0--9,8),
          { LeadingKeyword = None })], (true, false),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))