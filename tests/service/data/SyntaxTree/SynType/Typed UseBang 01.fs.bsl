ImplFile
  (ParsedImplFileInput
     ("/root/SynType/Typed UseBang 01.fs", false, QualifiedNameOfFile Module, [],
      [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Expr
             (App
                (NonAtomic, false, Ident async,
                 ComputationExpr
                   (false,
                    LetOrUseBang
                      (Yes (4,4--4,29), true, true,
                       Typed
                         (LongIdent
                            (SynLongIdent ([res], [], [None]), None, None,
                             Pats [], None, (4,9--4,12)),
                          App
                            (LongIdent (SynLongIdent ([Async], [], [None])),
                             Some (4,19--4,20),
                             [LongIdent (SynLongIdent ([int], [], [None]))], [],
                             Some (4,23--4,24), false, (4,14--4,24)),
                          (4,9--4,24)), Const (Unit, (4,27--4,29)), [],
                       YieldOrReturn
                         ((false, true), Const (Unit, (5,11--5,13)), (5,4--5,13),
                          { YieldOrReturnKeyword = (5,4--5,10) }), (4,4--5,13),
                       { LetOrUseBangKeyword = (4,4--4,8)
                         EqualsRange = Some (4,25--4,26) }), (3,6--6,1)),
                 (3,0--6,1)), (3,0--6,1))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--6,1), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))
