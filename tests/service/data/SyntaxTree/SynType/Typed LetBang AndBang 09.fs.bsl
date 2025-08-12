ImplFile
  (ParsedImplFileInput
     ("/root/SynType/Typed LetBang AndBang 09.fs", false,
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
                             (LongIdent
                                (SynLongIdent ([Union], [], [None]), None, None,
                                 Pats
                                   [Named
                                      (SynIdent (value, None), false, None,
                                       (4,15--4,20))], None, (4,9--4,20)),
                              App
                                (LongIdent (SynLongIdent ([option], [], [None])),
                                 None,
                                 [LongIdent (SynLongIdent ([int], [], [None]))],
                                 [], None, true, (4,22--4,32)), (4,9--4,32)),
                           None,
                           App
                             (Atomic, false, Ident asyncOption,
                              Const (Unit, (4,46--4,48)), (4,35--4,48)),
                           (4,4--6,25), Yes (4,4--4,48),
                           { LeadingKeyword = Let (4,4--4,8)
                             InlineKeyword = None
                             EqualsRange = Some (4,33--4,34) });
                        SynBinding
                          (None, Normal, false, false, [], PreXmlDocEmpty,
                           SynValData
                             (None,
                              SynValInfo ([], SynArgInfo ([], false, None)),
                              None),
                           Typed
                             (LongIdent
                                (SynLongIdent ([Union], [], [None]), None, None,
                                 Pats
                                   [Named
                                      (SynIdent (value2, None), false, None,
                                       (5,15--5,21))], None, (5,9--5,21)),
                              App
                                (LongIdent (SynLongIdent ([option], [], [None])),
                                 None,
                                 [LongIdent (SynLongIdent ([int], [], [None]))],
                                 [], None, true, (5,23--5,33)), (5,9--5,33)),
                           None,
                           App
                             (Atomic, false, Ident asyncOption,
                              Const (Unit, (5,47--5,49)), (5,36--5,49)),
                           (5,4--5,49), Yes (5,4--5,49),
                           { LeadingKeyword = And (5,4--5,8)
                             InlineKeyword = None
                             EqualsRange = Some (5,34--5,35) })],
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
                                   (6,17--6,18)), Ident value, (6,11--6,18)),
                             Ident value2, (6,11--6,25)), (6,4--6,25),
                          { YieldOrReturnKeyword = (6,4--6,10) }), (4,4--6,25),
                       { LetOrUseKeyword = (4,4--4,8)
                         InKeyword = None
                         EqualsRange = Some (4,33--4,34) }), (3,6--7,1)),
                 (3,0--7,1)), (3,0--7,1))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--7,1), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [] }, set []))
