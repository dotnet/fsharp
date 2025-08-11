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
                    LetOrUse
                      (false, false, true, true,
                       [SynBinding
                          (None, Normal, false, false, [], PreXmlDocEmpty,
                           SynValData
                             (None,
                              SynValInfo ([], SynArgInfo ([], false, None)),
                              None),
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
                                   (LongIdent
                                      (SynLongIdent ([array], [], [None])), None,
                                    [LongIdent
                                       (SynLongIdent ([int], [], [None]))], [],
                                    None, true, (4,31--4,40)), (4,10--4,40)),
                              (4,9--4,41)), None,
                           App
                             (Atomic, false, Ident asyncArray,
                              Const (Unit, (4,54--4,56)), (4,44--4,56)),
                           (4,4--6,16), Yes (4,4--4,56),
                           { LeadingKeyword = Let (4,4--4,8)
                             InlineKeyword = None
                             EqualsRange = Some (4,42--4,43) });
                        SynBinding
                          (None, Normal, false, false, [], PreXmlDocEmpty,
                           SynValData
                             (None,
                              SynValInfo ([], SynArgInfo ([], false, None)),
                              None),
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
                              (5,9--5,36)), None,
                           App
                             (Atomic, false, Ident asyncList,
                              Const (Unit, (5,48--5,50)), (5,39--5,50)),
                           (5,4--5,50), Yes (5,4--5,50),
                           { LeadingKeyword = And (5,4--5,8)
                             InlineKeyword = None
                             EqualsRange = Some (5,37--5,38) })],
                       YieldOrReturn
                         ((false, true), Ident first, (6,4--6,16),
                          { YieldOrReturnKeyword = (6,4--6,10) }), (4,4--6,16),
                       { LetOrUseKeyword = (4,4--4,8)
                         InKeyword = None
                         EqualsRange = Some (4,42--4,43) }), (3,6--7,1)),
                 (3,0--7,1)), (3,0--7,1))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--7,1), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [] }, set []))
