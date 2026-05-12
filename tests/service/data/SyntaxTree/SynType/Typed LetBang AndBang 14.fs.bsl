ImplFile
  (ParsedImplFileInput
     ("/root/SynType/Typed LetBang AndBang 14.fs", false,
      QualifiedNameOfFile Module, [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Expr
             (App
                (NonAtomic, false, Ident async,
                 ComputationExpr
                   (false,
                    LetOrUse
                      { IsRecursive = false
                        Bindings =
                         [SynBinding
                            (None, Normal, false, false, [], PreXmlDocEmpty,
                             SynValData
                               (None,
                                SynValInfo ([], SynArgInfo ([], false, None)),
                                None),
                             ArrayOrList
                               (true,
                                [Named
                                   (SynIdent (first, None), false, None,
                                    (4,12--4,17));
                                 Named
                                   (SynIdent (second, None), false, None,
                                    (4,19--4,25))], (4,9--4,28)),
                             Some
                               (SynBindingReturnInfo
                                  (App
                                     (LongIdent
                                        (SynLongIdent ([array], [], [None])),
                                      None,
                                      [LongIdent
                                         (SynLongIdent ([int], [], [None]))], [],
                                      None, true, (4,30--4,39)), (4,30--4,39),
                                   [], { ColonRange = Some (4,28--4,29) })),
                             App
                               (Atomic, false, Ident asyncArray,
                                Const (Unit, (4,52--4,54)), (4,42--4,54)),
                             (4,4--4,54), Yes (4,4--4,54),
                             { LeadingKeyword = LetBang (4,4--4,8)
                               InlineKeyword = None
                               EqualsRange = Some (4,40--4,41) });
                          SynBinding
                            (None, Normal, false, false, [], PreXmlDocEmpty,
                             SynValData
                               (None,
                                SynValInfo ([], SynArgInfo ([], false, None)),
                                None),
                             ListCons
                               (Named
                                  (SynIdent (head, None), false, None,
                                   (5,9--5,13)),
                                Named
                                  (SynIdent (tail, None), false, None,
                                   (5,17--5,21)), (5,9--5,21),
                                { ColonColonRange = (5,14--5,16) }),
                             Some
                               (SynBindingReturnInfo
                                  (App
                                     (LongIdent
                                        (SynLongIdent ([list], [], [None])),
                                      None,
                                      [LongIdent
                                         (SynLongIdent ([string], [], [None]))],
                                      [], None, true, (5,23--5,34)),
                                   (5,23--5,34), [],
                                   { ColonRange = Some (5,21--5,22) })),
                             App
                               (Atomic, false, Ident asyncList,
                                Const (Unit, (5,46--5,48)), (5,37--5,48)),
                             (5,4--5,48), Yes (5,4--5,48),
                             { LeadingKeyword = AndBang (5,4--5,8)
                               InlineKeyword = None
                               EqualsRange = Some (5,35--5,36) })]
                        Body =
                         YieldOrReturn
                           ((false, true), Ident first, (6,4--6,16),
                            { YieldOrReturnKeyword = (6,4--6,10) })
                        Range = (4,4--6,16)
                        Trivia = { InKeyword = None }
                        IsFromSource = true }, (3,6--7,1)), (3,0--7,1)),
              (3,0--7,1))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--7,1), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [] }, set []))
