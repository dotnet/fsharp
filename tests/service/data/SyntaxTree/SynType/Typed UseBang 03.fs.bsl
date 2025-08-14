ImplFile
  (ParsedImplFileInput
     ("/root/SynType/Typed UseBang 03.fs", false, QualifiedNameOfFile Module, [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Expr
             (LetOrUse
                (false, true, true, true,
                 [SynBinding
                    (None, Normal, false, false, [], PreXmlDocEmpty,
                     SynValData
                       (None, SynValInfo ([], SynArgInfo ([], false, None)),
                        None),
                     Typed
                       (Named (SynIdent (x, None), false, None, (3,5--3,6)),
                        LongIdent (SynLongIdent ([int], [], [None])),
                        (3,5--3,10)), None,
                     App
                       (NonAtomic, false, Ident async,
                        ComputationExpr
                          (false,
                           YieldOrReturn
                             ((false, true), Const (Int32 1, (3,28--3,29)),
                              (3,21--3,29),
                              { YieldOrReturnKeyword = (3,21--3,27) }),
                           (3,19--3,31)), (3,13--3,31)), (3,0--4,33),
                     Yes (3,0--3,31), { LeadingKeyword = Use (3,0--3,4)
                                        InlineKeyword = None
                                        EqualsRange = Some (3,11--3,12) })],
                 LetOrUse
                   (false, true, true, true,
                    [SynBinding
                       (None, Normal, false, false, [], PreXmlDocEmpty,
                        SynValData
                          (None, SynValInfo ([], SynArgInfo ([], false, None)),
                           None),
                        Paren
                          (Typed
                             (Named
                                (SynIdent (y, None), false, None, (4,6--4,7)),
                              LongIdent (SynLongIdent ([int], [], [None])),
                              (4,6--4,11)), (4,5--4,12)), None,
                        App
                          (NonAtomic, false, Ident async,
                           ComputationExpr
                             (false,
                              YieldOrReturn
                                ((false, true), Const (Int32 2, (4,30--4,31)),
                                 (4,23--4,31),
                                 { YieldOrReturnKeyword = (4,23--4,29) }),
                              (4,21--4,33)), (4,15--4,33)), (4,0--4,33),
                        Yes (4,0--4,33), { LeadingKeyword = Use (4,0--4,4)
                                           InlineKeyword = None
                                           EqualsRange = Some (4,13--4,14) })],
                    ImplicitZero (4,33--4,33), (4,0--4,33),
                    { LetOrUseKeyword = (4,0--4,4)
                      InKeyword = None
                      EqualsRange = Some (4,13--4,14) }), (3,0--4,33),
                 { LetOrUseKeyword = (3,0--3,4)
                   InKeyword = None
                   EqualsRange = Some (3,11--3,12) }), (3,0--4,33))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--4,33), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [] }, set []))

(4,0)-(4,33) parse error Incomplete structured construct at or before this point in expression
