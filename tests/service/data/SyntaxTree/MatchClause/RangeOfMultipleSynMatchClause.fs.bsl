ImplFile
  (ParsedImplFileInput
     ("/root/MatchClause/RangeOfMultipleSynMatchClause.fs", false,
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
                    (3,4--4,16), { LetOrUseKeyword = (3,4--3,7)
                                   InKeyword = None }),
                 [SynMatchClause
                    (Named (SynIdent (ex, None), false, None, (6,2--6,4)), None,
                     Sequential
                       (SuppressNeither, true,
                        App
                          (NonAtomic, false,
                           LongIdent
                             (false,
                              SynLongIdent
                                ([Infrastructure; ReportWarning], [(7,18--7,19)],
                                 [None; None]), None, (7,4--7,32)), Ident ex,
                           (7,4--7,35)), Ident None, (7,4--8,8),
                        { SeparatorRange = None }), (6,2--8,8), Yes,
                     { ArrowRange = Some (6,5--6,7)
                       BarRange = Some (6,0--6,1) });
                  SynMatchClause
                    (Named (SynIdent (exx, None), false, None, (9,2--9,5)), None,
                     Ident None, (9,2--10,8), Yes,
                     { ArrowRange = Some (9,6--9,8)
                       BarRange = Some (9,0--9,1) })], (2,0--10,8),
                 Yes (2,0--2,3), Yes (5,0--5,4),
                 { TryKeyword = (2,0--2,3)
                   TryToWithRange = (2,0--5,4)
                   WithKeyword = (5,0--5,4)
                   WithToEndRange = (5,0--10,8) }), (2,0--10,8))],
          PreXmlDocEmpty, [], None, (2,0--11,0), { LeadingKeyword = None })],
      (true, true), { ConditionalDirectives = []
                      CodeComments = [] }, set []))
