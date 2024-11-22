ImplFile
  (ParsedImplFileInput
     ("/root/ModuleOrNamespace/Nested module 10.fs", false,
      QualifiedNameOfFile Module, [],
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
                     false, None, (4,4--6,4)), false, [], false, (4,4--6,4),
                  { ModuleKeyword = Some (4,4--4,10)
                    EqualsRange = None });
               Expr (Const (Int32 2, (6,4--6,5)), (6,4--6,5))], false,
              (3,0--6,5), { ModuleKeyword = Some (3,0--3,6)
                            EqualsRange = Some (3,9--3,10) })],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--6,5), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [] }, set []))

(4,11)-(6,4) parse error Incomplete structured construct at or before this point in definition. Expected '=' or other token.
