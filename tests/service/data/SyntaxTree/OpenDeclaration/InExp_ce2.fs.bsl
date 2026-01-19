ImplFile
  (ParsedImplFileInput
     ("/root/OpenDeclaration/InExp_ce2.fs", false, QualifiedNameOfFile InExp_ce2,
      [],
      [SynModuleOrNamespace
         ([InExp_ce2], false, AnonModule,
          [Let
             (false,
              [SynBinding
                 (None, Normal, false, false, [],
                  PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector),
                  SynValData
                    (None, SynValInfo ([], SynArgInfo ([], false, None)), None),
                  Named (SynIdent (q, None), false, None, (1,4--1,5)), None,
                  App
                    (NonAtomic, false,
                     App
                       (NonAtomic, true,
                        LongIdent
                          (false,
                           SynLongIdent
                             ([op_PipeRight], [], [Some (OriginalNotation "|>")]),
                           None, (7,6--7,8)),
                        App
                          (NonAtomic, false, Ident query,
                           ComputationExpr
                             (false,
                              Open
                                (Type
                                   (LongIdent
                                      (SynLongIdent
                                         ([System; Linq; Enumerable],
                                          [(3,24--3,25); (3,29--3,30)],
                                          [None; None; None])), (3,18--3,40)),
                                 (3,8--3,40), (3,8--6,30),
                                 ForEach
                                   (Yes (4,8--4,11), Yes (4,14--4,16),
                                    SeqExprOnly false, true,
                                    Named
                                      (SynIdent (i, None), false, None,
                                       (4,12--4,13)),
                                    App
                                      (Atomic, false, Ident Range,
                                       Paren
                                         (Tuple
                                            (false,
                                             [Const (Int32 1, (4,23--4,24));
                                              Const (Int32 10, (4,26--4,28))],
                                             [(4,24--4,25)], (4,23--4,28)),
                                          (4,22--4,23), Some (4,28--4,29),
                                          (4,22--4,29)), (4,17--4,29)),
                                    Open
                                      (Type
                                         (LongIdent
                                            (SynLongIdent ([int], [], [None])),
                                          (5,22--5,25)), (5,12--5,25),
                                       (5,12--6,30),
                                       YieldOrReturn
                                         ((true, false),
                                          App
                                            (NonAtomic, false,
                                             App
                                               (NonAtomic, true,
                                                LongIdent
                                                  (false,
                                                   SynLongIdent
                                                     ([op_Addition], [],
                                                      [Some
                                                         (OriginalNotation "+")]),
                                                   None, (6,27--6,28)),
                                                Ident MinValue, (6,18--6,28)),
                                             Ident i, (6,18--6,30)),
                                          (6,12--6,30),
                                          { YieldOrReturnKeyword = (6,12--6,17) })),
                                    (4,8--6,30))), (2,10--7,5)), (2,4--7,5)),
                        (2,4--7,8)),
                     LongIdent
                       (false,
                        SynLongIdent
                          ([Seq; toArray], [(7,12--7,13)], [None; None]), None,
                        (7,9--7,20)), (2,4--7,20)), (1,4--1,5), NoneAtLet,
                  { LeadingKeyword = Let (1,0--1,3)
                    InlineKeyword = None
                    EqualsRange = Some (1,6--1,7) })], (1,0--7,20))],
          PreXmlDocEmpty, [], None, (1,0--8,0), { LeadingKeyword = None })],
      (true, true), { ConditionalDirectives = []
                      WarnDirectives = []
                      CodeComments = [] }, set []))
