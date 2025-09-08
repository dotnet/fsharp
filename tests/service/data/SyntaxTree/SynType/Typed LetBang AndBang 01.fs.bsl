ImplFile
  (ParsedImplFileInput
     ("/root/SynType/Typed LetBang AndBang 01.fs", false,
      QualifiedNameOfFile Module, [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Expr
             (App
                (NonAtomic, false, Ident async,
                 ComputationExpr
                   (false,
                    LetOrUse
                      (false, false, true, true,
                       [SynBinding
                          (None, Normal, false, false, [], PreXmlDocEmpty,
                           SynValData
                             (None,
                              SynValInfo ([], SynArgInfo ([], false, None)),
                              None),
                           Typed
                             (Named
                                (SynIdent (res, None), false, None, (4,9--4,12)),
                              LongIdent (SynLongIdent ([int], [], [None])),
                              (4,9--4,17)), None,
                           App
                             (NonAtomic, false, Ident async,
                              ComputationExpr
                                (false,
                                 YieldOrReturn
                                   ((false, true), Const (Int32 1, (4,35--4,36)),
                                    (4,28--4,36),
                                    { YieldOrReturnKeyword = (4,28--4,34) }),
                                 (4,26--4,38)), (4,20--4,38)), (4,4--6,14),
                           Yes (4,4--4,38), { LeadingKeyword = Let (4,4--4,8)
                                              InlineKeyword = None
                                              EqualsRange = Some (4,18--4,19) });
                        SynBinding
                          (None, Normal, false, false, [], PreXmlDocEmpty,
                           SynValData
                             (None,
                              SynValInfo ([], SynArgInfo ([], false, None)),
                              None),
                           Typed
                             (Named
                                (SynIdent (res2, None), false, None, (5,9--5,13)),
                              LongIdent (SynLongIdent ([int], [], [None])),
                              (5,9--5,18)), None,
                           App
                             (NonAtomic, false, Ident async,
                              ComputationExpr
                                (false,
                                 YieldOrReturn
                                   ((false, true), Const (Int32 2, (5,36--5,37)),
                                    (5,29--5,37),
                                    { YieldOrReturnKeyword = (5,29--5,35) }),
                                 (5,27--5,39)), (5,21--5,39)), (5,4--5,39),
                           Yes (5,4--5,39), { LeadingKeyword = And (5,4--5,8)
                                              InlineKeyword = None
                                              EqualsRange = Some (5,19--5,20) })],
                       YieldOrReturn
                         ((false, true), Ident res, (6,4--6,14),
                          { YieldOrReturnKeyword = (6,4--6,10) }), (4,4--6,14),
                       { LetOrUseKeyword = (4,4--4,8)
                         InKeyword = None
                         EqualsRange = Some (4,18--4,19) }), (3,6--7,1)),
                 (3,0--7,1)), (3,0--7,1))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--7,1), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [] }, set []))
