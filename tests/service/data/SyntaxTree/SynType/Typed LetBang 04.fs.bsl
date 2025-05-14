ImplFile
  (ParsedImplFileInput
     ("/root/SynType/Typed LetBang 04.fs", false, QualifiedNameOfFile Module, [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Expr
             (App
                (NonAtomic, false, Ident async,
                 ComputationExpr
                   (false,
                    Tuple
                      (false,
                       [LetOrUse
                          (false, false,
                           [SynBinding
                              (None, Normal, false, false, [],
                               PreXmlDoc ((4,4), FSharp.Compiler.Xml.XmlDocCollector),
                               SynValData
                                 (None,
                                  SynValInfo ([], SynArgInfo ([], false, None)),
                                  None),
                               Named
                                 (SynIdent (a, None), false, None, (4,8--4,9)),
                               Some
                                 (SynBindingReturnInfo
                                    (LongIdent
                                       (SynLongIdent ([int], [], [None])),
                                     (4,11--4,14), [],
                                     { ColonRange = Some (4,9--4,10) })),
                               Typed
                                 (ArbitraryAfterError
                                    ("localBinding2", (4,14--4,14)),
                                  LongIdent (SynLongIdent ([int], [], [None])),
                                  (4,14--4,14)), (4,8--4,9), Yes (4,4--4,14),
                               { LeadingKeyword = Let (4,4--4,7)
                                 InlineKeyword = None
                                 EqualsRange = None })],
                           ArbitraryAfterError ("declExpr3", (5,15--5,16)),
                           (4,4--5,16), { LetOrUseKeyword = (4,4--4,7)
                                          InKeyword = None }); Ident d],
                       [(5,15--5,16)], (4,4--5,18)), (3,6--5,47)), (3,0--5,47)),
              (3,0--5,47))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--5,47), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [] }, set []))

(4,14)-(4,15) parse error Unexpected symbol ',' in binding. Expected '=' or other token.
(5,15)-(5,16) parse error Unexpected symbol ',' in expression. Expected '=' or other token.
(6,4)-(6,10) parse error Incomplete structured construct at or before this point in implementation file
