ImplFile
  (ParsedImplFileInput
     ("/root/Expression/YieldBang 05.fs", false, QualifiedNameOfFile Module, [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Expr
             (App
                (NonAtomic, false, Ident seq,
                 ComputationExpr
                   (false,
                    YieldOrReturnFrom
                      ((true, false),
                       Typed
                         (ArrayOrListComputed
                            (false,
                             IndexRange
                               (Some (Const (Int32 1, (4,13--4,14))),
                                (4,15--4,17),
                                Some (Const (Int32 10, (4,18--4,20))),
                                (4,13--4,14), (4,18--4,20), (4,13--4,20)),
                             (4,11--4,22)), FromParseError (4,24--4,24),
                          (4,11--4,24)), (4,4--4,22),
                       { YieldOrReturnFromKeyword = (4,4--4,10) }), (3,4--5,1)),
                 (3,0--5,1)), (3,0--5,1))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--5,1), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [] }, set []))

(5,0)-(5,1) parse error Unexpected symbol '}' in expression
