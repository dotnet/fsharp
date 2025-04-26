ImplFile
  (ParsedImplFileInput
     ("/root/SynType/Typed LetBang 02.fs", false, QualifiedNameOfFile Module, [],
      [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Expr
             (App
                (NonAtomic, false, Ident async,
                 ComputationExpr
                   (false,
                    LetOrUseBang
                      (Yes (4,4--4,31), false, true,
                       Paren
                         (Typed
                            (Named
                               (SynIdent (res, None), false, None, (4,10--4,13)),
                             App
                               (LongIdent (SynLongIdent ([Async], [], [None])),
                                Some (4,20--4,21),
                                [LongIdent (SynLongIdent ([int], [], [None]))],
                                [], Some (4,24--4,25), false, (4,15--4,25)),
                             (4,10--4,25)), (4,9--4,26)),
                       Const (Unit, (4,29--4,31)), [],
                       YieldOrReturn
                         ((false, true), Const (Unit, (5,11--5,13)), (5,4--5,13),
                          { YieldOrReturnKeyword = (5,4--5,10) }), (4,4--5,13),
                       { LetOrUseBangKeyword = (4,4--4,8)
                         EqualsRange = Some (4,27--4,28) }), (3,6--6,1)),
                 (3,0--6,1)), (3,0--6,1))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--6,1), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))
