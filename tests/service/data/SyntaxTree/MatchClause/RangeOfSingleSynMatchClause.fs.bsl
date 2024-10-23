ImplFile
  (ParsedImplFileInput
     ("/root/MatchClause/RangeOfSingleSynMatchClause.fs", false,
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
                    (Named (SynIdent (ex, None), false, None, (5,5--5,7)), None,
                     Sequential
                       (SuppressNeither, true,
                        App
                          (NonAtomic, false,
                           LongIdent
                             (false,
                              SynLongIdent
                                ([Infrastructure; ReportWarning], [(6,18--6,19)],
                                 [None; None]), None, (6,4--6,32)), Ident ex,
                           (6,4--6,35)), Ident None, (6,4--7,8),
                        { SeparatorRange = None }), (5,5--7,8), Yes,
                     { ArrowRange = Some (5,8--5,10)
                       BarRange = None })], (2,0--7,8), Yes (2,0--2,3),
                 Yes (5,0--5,4), { TryKeyword = (2,0--2,3)
                                   TryToWithRange = (2,0--5,4)
                                   WithKeyword = (5,0--5,4)
                                   WithToEndRange = (5,0--7,8) }), (2,0--7,8))],
          PreXmlDocEmpty, [], None, (2,0--8,0), { LeadingKeyword = None })],
      (true, true), { ConditionalDirectives = []
                      CodeComments = [] }, set []))
