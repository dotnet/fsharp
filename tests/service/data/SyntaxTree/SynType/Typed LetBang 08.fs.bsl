ImplFile
  (ParsedImplFileInput
     ("/root/SynType/Typed LetBang 08.fs", false, QualifiedNameOfFile Module, [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Expr
             (App
                (NonAtomic, false, Ident async,
                 ComputationExpr
                   (false,
                    LetOrUseBang
                      (Yes (3,4--3,48), false, true,
                       Typed
                         (Record
                            ([(([], Name), Some (3,16--3,17),
                               Named
                                 (SynIdent (name, None), false, None,
                                  (3,18--3,22)))], (3,9--3,24)),
                          LongIdent (SynLongIdent ([Person], [], [None])),
                          (3,9--3,32)),
                       App
                         (Atomic, false, Ident asyncPerson,
                          Const (Unit, (3,46--3,48)), (3,35--3,48)), [],
                       YieldOrReturn
                         ((false, true), Ident name, (4,4--4,15),
                          { YieldOrReturnKeyword = (4,4--4,10) }), (3,4--4,15),
                       { LetOrUseKeyword = (3,4--3,8)
                         InKeyword = None
                         EqualsRange = Some (3,33--3,34) }), (2,6--6,1)),
                 (2,0--6,1)), (2,0--6,1))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--6,1), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [] }, set []))
