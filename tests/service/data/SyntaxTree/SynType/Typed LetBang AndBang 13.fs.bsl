ImplFile
  (ParsedImplFileInput
     ("/root/SynType/Typed LetBang AndBang 13.fs", false,
      QualifiedNameOfFile Module, [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Expr
             (App
                (NonAtomic, false, Ident async,
                 ComputationExpr
                   (false,
                    LetOrUseBang
                      (Yes (4,4--4,56), false, true,
                       Paren
                         (Typed
                            (ArrayOrList
                               (true,
                                [Named
                                   (SynIdent (first, None), false, None,
                                    (4,13--4,18));
                                 Named
                                   (SynIdent (second, None), false, None,
                                    (4,20--4,26))], (4,10--4,29)),
                             App
                               (LongIdent (SynLongIdent ([array], [], [None])),
                                None,
                                [LongIdent (SynLongIdent ([int], [], [None]))],
                                [], None, true, (4,31--4,40)), (4,10--4,40)),
                          (4,9--4,41)),
                       App
                         (Atomic, false, Ident asyncArray,
                          Const (Unit, (4,54--4,56)), (4,44--4,56)),
                       [SynExprAndBang
                          (Yes (5,4--5,50), false, true,
                           Paren
                             (ListCons
                                (Named
                                   (SynIdent (head, None), false, None,
                                    (5,10--5,14)),
                                 Typed
                                   (Named
                                      (SynIdent (tail, None), false, None,
                                       (5,18--5,22)),
                                    App
                                      (LongIdent
                                         (SynLongIdent ([list], [], [None])),
                                       None,
                                       [LongIdent
                                          (SynLongIdent ([string], [], [None]))],
                                       [], None, true, (5,24--5,35)),
                                    (5,18--5,35)), (5,10--5,35),
                                 { ColonColonRange = (5,15--5,17) }),
                              (5,9--5,36)),
                           App
                             (Atomic, false, Ident asyncList,
                              Const (Unit, (5,48--5,50)), (5,39--5,50)),
                           (5,4--5,50), { AndBangKeyword = (5,4--5,8)
                                          EqualsRange = (5,37--5,38)
                                          InKeyword = None })],
                       YieldOrReturn
                         ((false, true), Ident first, (6,4--6,16),
                          { YieldOrReturnKeyword = (6,4--6,10) }), (4,4--6,16),
                       { LetOrUseBangKeyword = (4,4--4,8)
                         EqualsRange = Some (4,42--4,43) }), (3,6--7,1)),
                 (3,0--7,1)), (3,0--7,1))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--7,1), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [] }, set []))
