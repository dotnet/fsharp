ImplFile
  (ParsedImplFileInput
     ("/root/SynType/Typed LetBang AndBang 11.fs", false,
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
                             (As
                                (Named
                                   (SynIdent (x, None), false, None, (4,9--4,10)),
                                 Named
                                   (SynIdent (y, None), false, None,
                                    (4,14--4,15)), (4,9--4,15)),
                              LongIdent (SynLongIdent ([int], [], [None])),
                              (4,9--4,20)), None,
                           App
                             (Atomic, false, Ident asyncInt,
                              Const (Unit, (4,31--4,33)), (4,23--4,33)),
                           (4,4--6,16), Yes (4,4--4,33),
                           { LeadingKeyword = Let (4,4--4,8)
                             InlineKeyword = None
                             EqualsRange = Some (4,21--4,22) });
                        SynBinding
                          (None, Normal, false, false, [], PreXmlDocEmpty,
                           SynValData
                             (None,
                              SynValInfo ([], SynArgInfo ([], false, None)),
                              None),
                           Typed
                             (As
                                (Named
                                   (SynIdent (a, None), false, None, (5,9--5,10)),
                                 Named
                                   (SynIdent (b, None), false, None,
                                    (5,14--5,15)), (5,9--5,15)),
                              LongIdent (SynLongIdent ([string], [], [None])),
                              (5,9--5,23)), None,
                           App
                             (Atomic, false, Ident asyncString,
                              Const (Unit, (5,37--5,39)), (5,26--5,39)),
                           (5,4--5,39), Yes (5,4--5,39),
                           { LeadingKeyword = And (5,4--5,8)
                             InlineKeyword = None
                             EqualsRange = Some (5,24--5,25) })],
                       YieldOrReturn
                         ((false, true),
                          App
                            (NonAtomic, false,
                             App
                               (NonAtomic, true,
                                LongIdent
                                  (false,
                                   SynLongIdent
                                     ([op_Addition], [],
                                      [Some (OriginalNotation "+")]), None,
                                   (6,13--6,14)), Ident x, (6,11--6,14)),
                             Ident b, (6,11--6,16)), (6,4--6,16),
                          { YieldOrReturnKeyword = (6,4--6,10) }), (4,4--6,16),
                       { LetOrUseKeyword = (4,4--4,8)
                         InKeyword = None
                         EqualsRange = Some (4,21--4,22) }), (3,6--7,1)),
                 (3,0--7,1)), (3,0--7,1))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--7,1), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [] }, set []))
