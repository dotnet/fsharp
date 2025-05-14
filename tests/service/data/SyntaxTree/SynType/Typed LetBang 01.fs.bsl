ImplFile
  (ParsedImplFileInput
     ("/root/SynType/Typed LetBang 01.fs", false, QualifiedNameOfFile Module, [],
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
                             (4,26--4,38)), (4,20--4,38)), [],
                       YieldOrReturn
                         ((false, true), Ident res, (5,4--5,14),
                          { YieldOrReturnKeyword = (5,4--5,10) }), (4,4--5,14),
                       { LetOrUseBangKeyword = (4,4--4,8)
                         EqualsRange = Some (4,18--4,19) }), (3,6--6,1)),
                 (3,0--6,1)), (3,0--6,1))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--6,1), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [] }, set []))
