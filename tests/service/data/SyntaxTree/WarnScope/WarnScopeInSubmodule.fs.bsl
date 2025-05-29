ImplFile
  (ParsedImplFileInput
     ("/root/WarnScope/WarnScopeInSubmodule.fs", false, QualifiedNameOfFile M,
      [],
      [SynModuleOrNamespace
         ([M], false, NamedModule,
          [NestedModule
             (SynComponentInfo
                ([], None, [], [N],
                 PreXmlDoc ((2,0), FSharp.Compiler.Xml.XmlDocCollector), false,
                 None, (2,0--2,8)), false,
              [Expr (Const (Unit, (4,4--4,6)), (4,4--4,6));
               Expr (Const (Unit, (6,4--6,6)), (6,4--6,6))], false, (2,0--6,6),
              { ModuleKeyword = Some (2,0--2,6)
                EqualsRange = Some (2,9--2,10) });
           Expr (Const (Unit, (7,0--7,2)), (7,0--7,2))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--7,2), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives =
         [Nowarn ([20; 25], (3,4--3,17)); Warnon ([20; 25], (5,4--5,17))]
        CodeComments = [LineComment (5,18--5,30)] }, set []))
