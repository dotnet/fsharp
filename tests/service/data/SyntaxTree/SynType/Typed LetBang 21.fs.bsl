ImplFile
  (ParsedImplFileInput
     ("/root/SynType/Typed LetBang 21.fs", false, QualifiedNameOfFile Module, [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Expr
             (App
                (NonAtomic, false, Ident async,
                 ComputationExpr
                   (false,
                    LetOrUse
                      (false, false, true, false,
                       [SynBinding
                          (None, Normal, false, false, [],
                           PreXmlDoc ((4,4), FSharp.Compiler.Xml.XmlDocCollector),
                           SynValData
                             (None,
                              SynValInfo ([], SynArgInfo ([], false, None)),
                              None),
                           Paren
                             (Typed
                                (Named
                                   (SynIdent (x, None), false, None, (4,9--4,10)),
                                 LongIdent (SynLongIdent ([string], [], [None])),
                                 (4,9--4,18)), (4,8--4,19)),
                           Some
                             (SynBindingReturnInfo
                                (LongIdent (SynLongIdent ([int], [], [None])),
                                 (4,21--4,24), [],
                                 { ColonRange = Some (4,19--4,20) })),
                           Typed
                             (Const (Int32 2, (4,27--4,28)),
                              LongIdent (SynLongIdent ([int], [], [None])),
                              (4,27--4,28)), (4,8--4,19), Yes (4,4--4,28),
                           { LeadingKeyword = Let (4,4--4,7)
                             InlineKeyword = None
                             EqualsRange = Some (4,25--4,26) })],
                       YieldOrReturn
                         ((false, true), Ident x, (5,4--5,12),
                          { YieldOrReturnKeyword = (5,4--5,10) }), (4,4--5,12),
                       { LetOrUseKeyword = (4,4--4,7)
                         InKeyword = None
                         EqualsRange = Some (4,25--4,26) }), (3,6--6,1)),
                 (3,0--6,1)), (3,0--6,1))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--6,1), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [] }, set []))
