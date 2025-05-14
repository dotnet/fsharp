ImplFile
  (ParsedImplFileInput
     ("/root/SynType/Typed UseBang 02.fs", false, QualifiedNameOfFile Module, [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Expr
             (App
                (NonAtomic, false, Ident async,
                 ComputationExpr
                   (false,
                    LetOrUseBang
                      (Yes (4,4--4,40), true, true,
                       Paren
                         (Typed
                            (Named
                               (SynIdent (res, None), false, None, (4,10--4,13)),
                             LongIdent (SynLongIdent ([int], [], [None])),
                             (4,10--4,18)), (4,9--4,19)),
                       App
                         (NonAtomic, false, Ident async,
                          ComputationExpr
                            (false,
                             YieldOrReturn
                               ((false, true), Const (Int32 1, (4,37--4,38)),
                                (4,30--4,38),
                                { YieldOrReturnKeyword = (4,30--4,36) }),
                             (4,28--4,40)), (4,22--4,40)), [],
                       YieldOrReturn
                         ((false, true), Ident res, (5,4--5,14),
                          { YieldOrReturnKeyword = (5,4--5,10) }), (4,4--5,14),
                       { LetOrUseBangKeyword = (4,4--4,8)
                         EqualsRange = Some (4,20--4,21) }), (3,6--6,1)),
                 (3,0--6,1)), (3,0--6,1))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--6,1), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [] }, set []))
