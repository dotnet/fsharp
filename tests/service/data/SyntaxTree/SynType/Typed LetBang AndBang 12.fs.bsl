ImplFile
  (ParsedImplFileInput
     ("/root/SynType/Typed LetBang AndBang 12.fs", false,
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
                           As
                             (Named
                                (SynIdent (x, None), false, None, (4,9--4,10)),
                              Named
                                (SynIdent (y, None), false, None, (4,14--4,15)),
                              (4,9--4,15)), None,
                           App
                             (Atomic, false, Ident asyncInt,
                              Const (Unit, (4,26--4,28)), (4,18--4,28)),
                           (4,4--6,16), Yes (4,4--4,28),
                           { LeadingKeyword = Let (4,4--4,8)
                             InlineKeyword = None
                             EqualsRange = Some (4,16--4,17) });
                        SynBinding
                          (None, Normal, false, false, [], PreXmlDocEmpty,
                           SynValData
                             (None,
                              SynValInfo ([], SynArgInfo ([], false, None)),
                              None),
                           As
                             (Named
                                (SynIdent (a, None), false, None, (5,9--5,10)),
                              Named
                                (SynIdent (b, None), false, None, (5,14--5,15)),
                              (5,9--5,15)), None,
                           App
                             (Atomic, false, Ident asyncString,
                              Const (Unit, (5,29--5,31)), (5,18--5,31)),
                           (5,4--5,31), Yes (5,4--5,31),
                           { LeadingKeyword = And (5,4--5,8)
                             InlineKeyword = None
                             EqualsRange = Some (5,16--5,17) })],
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
                         EqualsRange = Some (4,16--4,17) }), (3,6--7,1)),
                 (3,0--7,1)), (3,0--7,1))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--7,1), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [] }, set []))
