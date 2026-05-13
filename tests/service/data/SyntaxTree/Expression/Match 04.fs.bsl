ImplFile
  (ParsedImplFileInput
     ("/root/Expression/Match 04.fs", false, QualifiedNameOfFile Module, [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Expr
             (Do
                (Sequential
                   (SuppressNeither, true,
                    Match
                      (Yes (4,4--4,11), Const (Int32 1, (4,10--4,11)), [],
                       (4,4--4,11), { MatchKeyword = (4,4--4,9)
                                      WithKeyword = (4,11--4,11) }),
                    Const (Int32 2, (6,4--6,5)), (4,4--6,5),
                    { SeparatorRange = None }), (3,0--6,5)), (3,0--6,5))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--6,5), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [] }, set []))

(4,11)-(4,11) parse error Expecting 'with'
