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
                    LetOrUseBang
                      (Yes (4,4--4,38), false, true,
                       Typed
                         (Named (SynIdent (res, None), false, None, (4,9--4,12)),
                          LongIdent (SynLongIdent ([int], [], [None])),
                          (4,9--4,17)),
                       App
                         (NonAtomic, false, Ident async,
                          ComputationExpr
                            (false,
                             YieldOrReturn
                               ((false, true), Const (Int32 1, (4,35--4,36)),
                                (4,28--4,36),
                                { YieldOrReturnKeyword = (4,28--4,34) }),
                             (4,26--4,38)), (4,20--4,38)),
                       [SynExprAndBang
                          (Yes (5,4--5,39), false, true,
                           Typed
                             (Named
                                (SynIdent (res2, None), false, None, (5,9--5,13)),
                              LongIdent (SynLongIdent ([int], [], [None])),
                              (5,9--5,18)),
                           App
                             (NonAtomic, false, Ident async,
                              ComputationExpr
                                (false,
                                 YieldOrReturn
                                   ((false, true), Const (Int32 2, (5,36--5,37)),
                                    (5,29--5,37),
                                    { YieldOrReturnKeyword = (5,29--5,35) }),
                                 (5,27--5,39)), (5,21--5,39)), (5,4--5,39),
                           { AndBangKeyword = (5,4--5,8)
                             EqualsRange = (5,19--5,20)
                             InKeyword = None })],
                       YieldOrReturn
                         ((false, true), Ident res, (6,4--6,14),
                          { YieldOrReturnKeyword = (6,4--6,10) }), (4,4--6,14),
                       { LetOrUseBangKeyword = (4,4--4,8)
                         EqualsRange = Some (4,18--4,19) }), (3,6--7,1)),
                 (3,0--7,1)), (3,0--7,1))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--7,1), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [] }, set []))
