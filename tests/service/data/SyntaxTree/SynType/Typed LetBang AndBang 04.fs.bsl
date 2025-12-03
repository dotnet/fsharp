ImplFile
  (ParsedImplFileInput
     ("/root/SynType/Typed LetBang AndBang 04.fs", false,
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
                             Record
                               ([NamePatPairField
                                   (SynLongIdent ([Name], [], [None]),
                                    Some (4,16--4,17), (4,11--4,22),
                                    Named
                                      (SynIdent (name, None), false, None,
                                       (4,18--4,22)),
                                    Some ((4,22--4,23), Some (4,23)));
                                 NamePatPairField
                                   (SynLongIdent ([Age], [], [None]),
                                    Some (4,28--4,29), (4,24--4,33),
                                    Named
                                      (SynIdent (age, None), false, None,
                                       (4,30--4,33)), None)], (4,9--4,35)),
                             Some
                               (SynBindingReturnInfo
                                  (LongIdent
                                     (SynLongIdent ([Person], [], [None])),
                                   (4,37--4,43), [],
                                   { ColonRange = Some (4,35--4,36) })),
                             App
                               (Atomic, false, Ident asyncPerson,
                                Const (Unit, (4,57--4,59)), (4,46--4,59)),
                             (4,4--4,59), Yes (4,4--4,59),
                             { LeadingKeyword = LetBang (4,4--4,8)
                               InlineKeyword = None
                               EqualsRange = Some (4,44--4,45) });
                          SynBinding
                            (None, Normal, false, false, [], PreXmlDocEmpty,
                             SynValData
                               (None,
                                SynValInfo ([], SynArgInfo ([], false, None)),
                                None),
                             Record
                               ([NamePatPairField
                                   (SynLongIdent ([Id], [], [None]),
                                    Some (5,14--5,15), (5,11--5,18),
                                    Named
                                      (SynIdent (id, None), false, None,
                                       (5,16--5,18)), None)], (5,9--5,20)),
                             Some
                               (SynBindingReturnInfo
                                  (LongIdent (SynLongIdent ([User], [], [None])),
                                   (5,22--5,26), [],
                                   { ColonRange = Some (5,20--5,21) })),
                             App
                               (Atomic, false, Ident asyncUser,
                                Const (Unit, (5,38--5,40)), (5,29--5,40)),
                             (5,4--5,40), Yes (5,4--5,40),
                             { LeadingKeyword = AndBang (5,4--5,8)
                               InlineKeyword = None
                               EqualsRange = Some (5,27--5,28) })]
                        Body =
                         YieldOrReturn
                           ((false, true), Ident name, (6,4--6,15),
                            { YieldOrReturnKeyword = (6,4--6,10) })
                        Range = (4,4--6,15)
                        Trivia = { InKeyword = None }
                        IsFromSource = true }, (3,6--7,1)), (3,0--7,1)),
              (3,0--7,1))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--7,1), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [] }, set []))
