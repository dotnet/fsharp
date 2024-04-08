ImplFile
  (ParsedImplFileInput
     ("/root/Expression/If 02.fs", false, QualifiedNameOfFile Module, [], [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Expr
             (IfThenElse
                (Const (Bool true, (3,3--3,7)), Const (Unit, (3,13--3,15)),
                 Some (Const (Unit, (3,21--3,23))), Yes (3,0--3,12), false,
                 (3,0--3,23), { IfKeyword = (3,0--3,2)
                                IsElif = false
                                ThenKeyword = (3,8--3,12)
                                ElseKeyword = Some (3,16--3,20)
                                IfToThenRange = (3,0--3,12) }), (3,0--3,23))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--3,23), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))
