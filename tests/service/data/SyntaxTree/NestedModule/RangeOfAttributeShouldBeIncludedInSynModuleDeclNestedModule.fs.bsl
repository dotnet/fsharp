ImplFile
  (ParsedImplFileInput
     ("/root/NestedModule/RangeOfAttributeShouldBeIncludedInSynModuleDeclNestedModule.fs",
      false, QualifiedNameOfFile TopLevel, [], [],
      [SynModuleOrNamespace
         ([TopLevel], false, NamedModule,
          [NestedModule
             (SynComponentInfo
                ([{ Attributes = [{ TypeName = SynLongIdent ([Foo], [], [None])
                                    ArgExpr = Const (Unit, (4,2--4,5))
                                    Target = None
                                    AppliesToGetterAndSetter = false
                                    Range = (4,2--4,5) }]
                    Range = (4,0--4,7) }], None, [], [Nested],
                 PreXmlDoc ((4,0), FSharp.Compiler.Xml.XmlDocCollector), false,
                 None, (5,0--5,13)), false,
              [Expr (Const (Unit, (6,4--6,6)), (6,4--6,6))], false, (4,0--6,6),
              { ModuleKeyword = Some (5,0--5,6)
                EqualsRange = Some (5,14--5,15) })],
          PreXmlDoc ((2,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (2,0--6,6), { LeadingKeyword = Module (2,0--2,6) })], (true, false),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))
