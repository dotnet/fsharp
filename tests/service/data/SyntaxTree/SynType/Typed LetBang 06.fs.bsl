ImplFile
  (ParsedImplFileInput
     ("/root/SynType/Typed LetBang 06.fs", false, QualifiedNameOfFile Module, [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Expr
             (LetOrUseBang
                (Yes (2,0--2,31), false, true,
                 Typed
                   (Wild (2,5--2,6),
                    LongIdent (SynLongIdent ([int], [], [None])), (2,5--2,10)),
                 App
                   (NonAtomic, false, Ident async,
                    ComputationExpr
                      (false,
                       YieldOrReturn
                         ((false, true), Const (Int32 1, (2,28--2,29)),
                          (2,21--2,29), { YieldOrReturnKeyword = (2,21--2,27) }),
                       (2,19--2,31)), (2,13--2,31)), [],
                 LetOrUseBang
                   (Yes (3,0--3,33), false, true,
                    Paren
                      (Typed
                         (Wild (3,6--3,7),
                          LongIdent (SynLongIdent ([int], [], [None])),
                          (3,6--3,11)), (3,5--3,12)),
                    App
                      (NonAtomic, false, Ident async,
                       ComputationExpr
                         (false,
                          YieldOrReturn
                            ((false, true), Const (Int32 2, (3,30--3,31)),
                             (3,23--3,31),
                             { YieldOrReturnKeyword = (3,23--3,29) }),
                          (3,21--3,33)), (3,15--3,33)), [],
                    ImplicitZero (3,33--3,33), (3,0--3,33),
                    { LetOrUseBangKeyword = (3,0--3,4)
                      EqualsRange = Some (3,13--3,14) }), (2,0--3,33),
                 { LetOrUseBangKeyword = (2,0--2,4)
                   EqualsRange = Some (2,11--2,12) }), (2,0--3,33))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--3,33), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [] }, set []))

(3,0)-(3,33) parse error Incomplete structured construct at or before this point in expression
