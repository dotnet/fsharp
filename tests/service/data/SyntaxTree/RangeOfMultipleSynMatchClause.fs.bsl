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
                        PreXmlDoc ((3,4), FSharp.Compiler.Xml.XmlDocCollector),
                        SynValData
                          (None, SynValInfo ([], SynArgInfo ([], false, None)),
                           None),
                        Named
                          (SynIdent (content, None), false, None,
                           /root/RangeOfMultipleSynMatchClause.fs (3,8--3,15)),
                        None,
                        App
                          (NonAtomic, false, Ident tryDownloadFile, Ident url,
                           /root/RangeOfMultipleSynMatchClause.fs (3,18--3,37)),
                        /root/RangeOfMultipleSynMatchClause.fs (3,8--3,15),
                        Yes /root/RangeOfMultipleSynMatchClause.fs (3,4--3,37),
                        { LeadingKeyword =
                           Let /root/RangeOfMultipleSynMatchClause.fs (3,4--3,7)
                          InlineKeyword = None
                          EqualsRange =
                           Some
                             /root/RangeOfMultipleSynMatchClause.fs (3,16--3,17) })],
                    App
                      (NonAtomic, false, Ident Some, Ident content,
                       /root/RangeOfMultipleSynMatchClause.fs (4,4--4,16)),
                    /root/RangeOfMultipleSynMatchClause.fs (3,4--4,16),
                    { InKeyword = None }),
                 [SynMatchClause
                    (Named
                       (SynIdent (ex, None), false, None,
                        /root/RangeOfMultipleSynMatchClause.fs (6,2--6,4)), None,
                     Sequential
                       (SuppressNeither, true,
                        App
                          (NonAtomic, false,
                           LongIdent
                             (false,
                              SynLongIdent
                                ([Infrastructure; ReportWarning],
                                 [/root/RangeOfMultipleSynMatchClause.fs (7,18--7,19)],
                                 [None; None]), None,
                              /root/RangeOfMultipleSynMatchClause.fs (7,4--7,32)),
                           Ident ex,
                           /root/RangeOfMultipleSynMatchClause.fs (7,4--7,35)),
                        Ident None,
                        /root/RangeOfMultipleSynMatchClause.fs (7,4--8,8)),
                     /root/RangeOfMultipleSynMatchClause.fs (6,2--8,8), Yes,
                     { ArrowRange =
                        Some /root/RangeOfMultipleSynMatchClause.fs (6,5--6,7)
                       BarRange =
                        Some /root/RangeOfMultipleSynMatchClause.fs (6,0--6,1) });
                  SynMatchClause
                    (Named
                       (SynIdent (exx, None), false, None,
                        /root/RangeOfMultipleSynMatchClause.fs (9,2--9,5)), None,
                     Ident None,
                     /root/RangeOfMultipleSynMatchClause.fs (9,2--10,8), Yes,
                     { ArrowRange =
                        Some /root/RangeOfMultipleSynMatchClause.fs (9,6--9,8)
                       BarRange =
                        Some /root/RangeOfMultipleSynMatchClause.fs (9,0--9,1) })],
                 /root/RangeOfMultipleSynMatchClause.fs (2,0--10,8),
                 Yes /root/RangeOfMultipleSynMatchClause.fs (2,0--2,3),
                 Yes /root/RangeOfMultipleSynMatchClause.fs (5,0--5,4),
                 { TryKeyword =
                    /root/RangeOfMultipleSynMatchClause.fs (2,0--2,3)
                   TryToWithRange =
                    /root/RangeOfMultipleSynMatchClause.fs (2,0--5,4)
                   WithKeyword =
                    /root/RangeOfMultipleSynMatchClause.fs (5,0--5,4)
                   WithToEndRange =
                    /root/RangeOfMultipleSynMatchClause.fs (5,0--10,8) }),
              /root/RangeOfMultipleSynMatchClause.fs (2,0--10,8))],
          PreXmlDocEmpty, [], None,
          /root/RangeOfMultipleSynMatchClause.fs (2,0--11,0),
          { LeadingKeyword = None })], (true, false),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))