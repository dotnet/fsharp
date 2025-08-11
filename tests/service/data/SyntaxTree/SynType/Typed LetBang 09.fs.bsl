ImplFile
  (ParsedImplFileInput
     ("/root/SynType/Typed LetBang 09.fs", false, QualifiedNameOfFile Module, [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Expr
             (App
                (NonAtomic, false, Ident async,
                 ComputationExpr
                   (false,
                    LetOrUseBang
                      (Yes (3,4--3,38), false, true,
                       Paren
                         (LongIdent
                            (SynLongIdent ([Union], [], [None]), None, None,
                             Pats
                               [Named
                                  (SynIdent (value, None), false, None,
                                   (3,16--3,21))], None, (3,10--3,21)),
                          (3,9--3,22)),
                       App
                         (Atomic, false, Ident asyncOption,
                          Const (Unit, (3,36--3,38)), (3,25--3,38)), [],
                       YieldOrReturn
                         ((false, true), Ident value, (4,4--4,16),
                          { YieldOrReturnKeyword = (4,4--4,10) }), (3,4--4,16),
                       { LetOrUseKeyword = (3,4--3,8)
                         InKeyword = None
                         EqualsRange = Some (3,23--3,24) }), (2,6--5,1)),
                 (2,0--5,1)), (2,0--5,1))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--5,1), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [] }, set []))
