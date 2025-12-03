ImplFile
  (ParsedImplFileInput
     ("/root/SynType/Typed LetBang AndBang 17.fs", false,
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
                             Paren
                               (Tuple
                                  (false,
                                   [Named
                                      (SynIdent (x, None), false, None,
                                       (4,10--4,11));
                                    Named
                                      (SynIdent (y, None), false, None,
                                       (4,13--4,14))], [(4,11--4,12)],
                                   (4,10--4,14)), (4,9--4,15)),
                             Some
                               (SynBindingReturnInfo
                                  (Tuple
                                     (false,
                                      [Type
                                         (LongIdent
                                            (SynLongIdent ([int], [], [None])));
                                       Star (4,21--4,22);
                                       Type
                                         (LongIdent
                                            (SynLongIdent ([int], [], [None])))],
                                      (4,17--4,26)), (4,17--4,26), [],
                                   { ColonRange = Some (4,15--4,16) })),
                             App
                               (Atomic, false, Ident asyncInt,
                                Const (Unit, (4,37--4,39)), (4,29--4,39)),
                             (4,4--4,39), Yes (4,4--4,39),
                             { LeadingKeyword = LetBang (4,4--4,8)
                               InlineKeyword = None
                               EqualsRange = Some (4,27--4,28) });
                          SynBinding
                            (None, Normal, false, false, [], PreXmlDocEmpty,
                             SynValData
                               (None,
                                SynValInfo ([], SynArgInfo ([], false, None)),
                                None),
                             Paren
                               (Tuple
                                  (false,
                                   [Named
                                      (SynIdent (x, None), false, None,
                                       (5,10--5,11));
                                    Named
                                      (SynIdent (y, None), false, None,
                                       (5,13--5,14))], [(5,11--5,12)],
                                   (5,10--5,14)), (5,9--5,15)),
                             Some
                               (SynBindingReturnInfo
                                  (Tuple
                                     (false,
                                      [Type
                                         (LongIdent
                                            (SynLongIdent ([int], [], [None])));
                                       Star (5,21--5,22);
                                       Type
                                         (LongIdent
                                            (SynLongIdent ([int], [], [None])))],
                                      (5,17--5,26)), (5,17--5,26), [],
                                   { ColonRange = Some (5,15--5,16) })),
                             App
                               (Atomic, false, Ident asyncInt,
                                Const (Unit, (5,37--5,39)), (5,29--5,39)),
                             (5,4--5,39), Yes (5,4--5,39),
                             { LeadingKeyword = AndBang (5,4--5,8)
                               InlineKeyword = None
                               EqualsRange = Some (5,27--5,28) })]
                        Body =
                         YieldOrReturn
                           ((false, true), Const (Unit, (6,11--6,13)),
                            (6,4--6,13), { YieldOrReturnKeyword = (6,4--6,10) })
                        Range = (4,4--6,13)
                        Trivia = { InKeyword = None }
                        IsFromSource = true }, (3,6--7,1)), (3,0--7,1)),
              (3,0--7,1))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--7,1), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [] }, set []))
