ImplFile
  (ParsedImplFileInput
     ("/root/ModuleOrNamespace/Nested module 01.fs", false,
      QualifiedNameOfFile Module, [], [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [NestedModule
             (SynComponentInfo
                ([], None, [], [A],
                 PreXmlDoc ((3,0), FSharp.Compiler.Xml.XmlDocCollector), false,
                 None, (3,0--3,8)), false,
              [Expr (Const (Unit, (4,4--4,6)), (4,4--4,6))], false, (3,0--4,6),
              { ModuleKeyword = Some (3,0--3,6)
                EqualsRange = Some (3,9--3,10) });
           Expr (Const (Unit, (6,0--6,2)), (6,0--6,2))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--6,2), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))
