ImplFile
  (ParsedImplFileInput
     ("/root/ModuleOrNamespace/Nested module 04.fs", false,
      QualifiedNameOfFile Module, [], [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [NestedModule
             (SynComponentInfo
                ([], None, [], [],
                 PreXmlDoc ((3,0), FSharp.Compiler.Xml.XmlDocCollector), false,
                 None, (3,0--5,0)), false, [], false, (3,0--5,0),
              { ModuleKeyword = Some (3,0--3,6)
                EqualsRange = None });
           Expr (Const (Unit, (5,0--5,2)), (5,0--5,2))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--5,2), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))

(3,7)-(5,0) parse error Incomplete structured construct at or before this point in definition. Expected '=' or other token.
