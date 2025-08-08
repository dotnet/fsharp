ImplFile
  (ParsedImplFileInput
     ("/root/SynType/Typed LetBang AndBang 16.fs", false,
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
                             (Paren
                                (Tuple
                                   (false,
                                    [Named
                                       (SynIdent (x, None), false, None,
                                        (4,10--4,11));
                                     Named
                                       (SynIdent (y, None), false, None,
                                        (4,13--4,14))], [(4,11--4,12)],
                                    (4,10--4,14)), (4,9--4,15)),
                              Tuple
                                (false,
                                 [Type
                                    (LongIdent
                                       (SynLongIdent ([int], [], [None])));
                                  Star (4,21--4,22);
                                  Type
                                    (LongIdent
                                       (SynLongIdent ([int], [], [None])))],
                                 (4,17--4,26)), (4,9--4,26)), None,
                           App
                             (Atomic, false, Ident asyncInt,
                              Const (Unit, (4,37--4,39)), (4,29--4,39)),
                           (4,4--6,13), Yes (4,4--4,39),
                           { LeadingKeyword = Let (4,4--4,8)
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
                                 [Typed
                                    (Named
                                       (SynIdent (x, None), false, None,
                                        (5,10--5,11)),
                                     LongIdent
                                       (SynLongIdent ([int], [], [None])),
                                     (5,10--5,16));
                                  Typed
                                    (Named
                                       (SynIdent (y, None), false, None,
                                        (5,18--5,19)),
                                     LongIdent
                                       (SynLongIdent ([int], [], [None])),
                                     (5,18--5,24))], [(5,16--5,17)],
                                 (5,10--5,24)), (5,9--5,25)), None,
                           App
                             (Atomic, false, Ident asyncInt,
                              Const (Unit, (5,36--5,38)), (5,28--5,38)),
                           (5,4--5,38), Yes (5,4--5,38),
                           { LeadingKeyword = And (5,4--5,8)
                             InlineKeyword = None
                             EqualsRange = Some (5,26--5,27) })],
                       YieldOrReturn
                         ((false, true), Const (Unit, (6,11--6,13)), (6,4--6,13),
                          { YieldOrReturnKeyword = (6,4--6,10) }), (4,4--6,13),
                       { LetOrUseKeyword = (4,4--4,8)
                         InKeyword = None
                         EqualsRange = Some (4,27--4,28) }), (3,6--7,1)),
                 (3,0--7,1)), (3,0--7,1))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--7,1), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [] }, set []))
