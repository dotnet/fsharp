ImplFile
  (ParsedImplFileInput
     ("/root/ModuleOrNamespace/Nested module 07.fs", false,
      QualifiedNameOfFile Module, [], [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [NestedModule
             (SynComponentInfo
                ([], None, [], [A],
                 PreXmlDoc ((3,0), FSharp.Compiler.Xml.XmlDocCollector), false,
                 None, (3,0--3,8)), false,
              [NestedModule
                 (SynComponentInfo
                    ([], None, [], [],
                     PreXmlDoc ((4,4), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (4,4--6,0)), false, [], false, (4,4--6,0),
                  { ModuleKeyword = Some (4,4--4,10)
                    EqualsRange = None })], false, (3,0--6,0),
              { ModuleKeyword = Some (3,0--3,6)
                EqualsRange = Some (3,9--3,10) });
           Expr (Const (Int32 2, (6,0--6,1)), (6,0--6,1))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--6,1), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))

(6,0)-(6,1) parse error Incomplete structured construct at or before this point in definition. Expected '=' or other token.
