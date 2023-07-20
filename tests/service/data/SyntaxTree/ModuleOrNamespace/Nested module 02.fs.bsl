ImplFile
  (ParsedImplFileInput
     ("/root/ModuleOrNamespace/Nested module 02.fs", false,
      QualifiedNameOfFile Module, [], [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [NestedModule
             (SynComponentInfo
                ([], None, [], [A],
                 PreXmlDoc ((3,0), FSharp.Compiler.Xml.XmlDocCollector), false,
                 None, (3,0--3,8)), false, [], false, (3,0--3,10),
              { ModuleKeyword = Some (3,0--3,6)
                EqualsRange = Some (3,9--3,10) });
           Expr (Const (Unit, (5,0--5,2)), (5,0--5,2))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--5,2), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))

(5,0)-(5,1) parse error Possible incorrect indentation: this token is offside of context started at position (3:1). Try indenting this token further or using standard formatting conventions.
(5,0)-(5,1) parse error Incomplete structured construct at or before this point in definition
