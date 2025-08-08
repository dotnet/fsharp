ImplFile
  (ParsedImplFileInput
     ("/root/SynType/Typed LetBang 18.fs", false, QualifiedNameOfFile Module, [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Let
             (false,
              [SynBinding
                 (None, Normal, false, false, [],
                  PreXmlDoc ((3,0), FSharp.Compiler.Xml.XmlDocCollector),
                  SynValData
                    (None, SynValInfo ([], SynArgInfo ([], false, None)), None),
                  As
                    (LongIdent
                       (SynLongIdent ([Even], [], [None]), None, None, Pats [],
                        None, (3,4--3,8)),
                     Named (SynIdent (x, None), false, None, (3,12--3,13)),
                     (3,4--3,13)),
                  Some
                    (SynBindingReturnInfo
                       (LongIdent (SynLongIdent ([int], [], [None])),
                        (3,15--3,18), [], { ColonRange = Some (3,13--3,14) })),
                  Typed
                    (Const (Int32 1, (3,21--3,22)),
                     LongIdent (SynLongIdent ([int], [], [None])), (3,21--3,22)),
                  (3,4--3,13), Yes (3,0--3,22),
                  { LeadingKeyword = Let (3,0--3,3)
                    InlineKeyword = None
                    EqualsRange = Some (3,19--3,20) })], (3,0--3,22));
           Expr
             (LetOrUse
                (false, false, true, true,
                 [SynBinding
                    (None, Normal, false, false, [], PreXmlDocEmpty,
                     SynValData
                       (None, SynValInfo ([], SynArgInfo ([], false, None)),
                        None),
                     Typed
                       (As
                          (LongIdent
                             (SynLongIdent ([Even], [], [None]), None, None,
                              Pats [], None, (5,5--5,9)),
                           Named (SynIdent (x, None), false, None, (5,13--5,14)),
                           (5,5--5,14)),
                        LongIdent (SynLongIdent ([int], [], [None])),
                        (5,5--5,19)), None,
                     App
                       (NonAtomic, false, Ident async,
                        ComputationExpr
                          (false,
                           YieldOrReturn
                             ((false, true), Const (Int32 2, (5,37--5,38)),
                              (5,30--5,38),
                              { YieldOrReturnKeyword = (5,30--5,36) }),
                           (5,28--5,40)), (5,22--5,40)), (5,0--5,40),
                     Yes (5,0--5,40), { LeadingKeyword = Let (5,0--5,4)
                                        InlineKeyword = None
                                        EqualsRange = Some (5,20--5,21) })],
                 ImplicitZero (5,40--5,40), (5,0--5,40),
                 { LetOrUseKeyword = (5,0--5,4)
                   InKeyword = None
                   EqualsRange = Some (5,20--5,21) }), (5,0--5,40))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--5,40), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [] }, set []))

(5,0)-(5,40) parse error Incomplete structured construct at or before this point in expression
