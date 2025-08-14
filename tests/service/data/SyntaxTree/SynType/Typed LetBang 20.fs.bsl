ImplFile
  (ParsedImplFileInput
     ("/root/SynType/Typed LetBang 20.fs", false, QualifiedNameOfFile Module, [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Expr
             (App
                (NonAtomic, false, Ident async,
                 ComputationExpr
                   (false,
                    LetOrUse
                      (false, false, true, false,
                       [SynBinding
                          (None, Normal, false, false, [],
                           PreXmlDoc ((4,4), FSharp.Compiler.Xml.XmlDocCollector),
                           SynValData
                             (None,
                              SynValInfo ([], SynArgInfo ([], false, None)),
                              None),
                           As
                             (LongIdent
                                (SynLongIdent ([Even], [], [None]), None, None,
                                 Pats [], None, (4,8--4,12)),
                              Named
                                (SynIdent (x, None), false, None, (4,16--4,17)),
                              (4,8--4,17)),
                           Some
                             (SynBindingReturnInfo
                                (LongIdent (SynLongIdent ([int], [], [None])),
                                 (4,19--4,22), [],
                                 { ColonRange = Some (4,17--4,18) })),
                           Typed
                             (Const (Int32 1, (4,25--4,26)),
                              LongIdent (SynLongIdent ([int], [], [None])),
                              (4,25--4,26)), (4,8--4,17), Yes (4,4--4,26),
                           { LeadingKeyword = Let (4,4--4,7)
                             InlineKeyword = None
                             EqualsRange = Some (4,23--4,24) })],
                       LetOrUse
                         (false, false, true, true,
                          [SynBinding
                             (None, Normal, false, false, [], PreXmlDocEmpty,
                              SynValData
                                (None,
                                 SynValInfo ([], SynArgInfo ([], false, None)),
                                 None),
                              Typed
                                (As
                                   (LongIdent
                                      (SynLongIdent ([Even], [], [None]), None,
                                       None, Pats [], None, (5,9--5,13)),
                                    Named
                                      (SynIdent (x, None), false, None,
                                       (5,17--5,18)), (5,9--5,18)),
                                 LongIdent (SynLongIdent ([int], [], [None])),
                                 (5,9--5,23)), None,
                              App
                                (NonAtomic, false, Ident async,
                                 ComputationExpr
                                   (false,
                                    YieldOrReturn
                                      ((false, true),
                                       Const (Int32 2, (5,41--5,42)),
                                       (5,34--5,42),
                                       { YieldOrReturnKeyword = (5,34--5,40) }),
                                    (5,32--5,44)), (5,26--5,44)), (5,4--6,12),
                              Yes (5,4--5,44),
                              { LeadingKeyword = Let (5,4--5,8)
                                InlineKeyword = None
                                EqualsRange = Some (5,24--5,25) })],
                          YieldOrReturn
                            ((false, true), Ident x, (6,4--6,12),
                             { YieldOrReturnKeyword = (6,4--6,10) }),
                          (5,4--6,12), { LetOrUseKeyword = (5,4--5,8)
                                         InKeyword = None
                                         EqualsRange = Some (5,24--5,25) }),
                       (4,4--6,12), { LetOrUseKeyword = (4,4--4,7)
                                      InKeyword = None
                                      EqualsRange = Some (4,23--4,24) }),
                    (3,6--7,1)), (3,0--7,1)), (3,0--7,1))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--7,1), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [] }, set []))
