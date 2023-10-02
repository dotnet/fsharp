ImplFile
  (ParsedImplFileInput
     ("/root/ModuleOrNamespace/Nested module 09.fs", false,
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
                    ([], None, [], [B],
                     PreXmlDoc ((4,4), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (4,4--4,12)), false, [], false, (4,4--4,14),
                  { ModuleKeyword = Some (4,4--4,10)
                    EqualsRange = Some (4,13--4,14) });
               Expr (Const (Int32 2, (6,4--6,5)), (6,4--6,5))], false,
              (3,0--6,5), { ModuleKeyword = Some (3,0--3,6)
                            EqualsRange = Some (3,9--3,10) })],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--6,5), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))

(6,4)-(6,5) parse error Unexpected syntax or possible incorrect indentation: this token is offside of context started at position (4:5). Try indenting this further.
To continue using non-conforming indentation, pass the '--strict-indentation-' flag to the compiler, or set the language version to F# 7.
(6,4)-(6,5) parse error Incomplete structured construct at or before this point in definition
