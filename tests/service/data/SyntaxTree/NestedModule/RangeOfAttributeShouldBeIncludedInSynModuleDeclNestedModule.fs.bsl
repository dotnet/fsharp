ImplFile
  (ParsedImplFileInput
     ("/root/NestedModule/RangeOfAttributeShouldBeIncludedInSynModuleDeclNestedModule.fs",
      false, QualifiedNameOfFile TopLevel, [], [],
      [SynModuleOrNamespace
         ([TopLevel], false, NamedModule,
          [NestedModule
             (SynComponentInfo
                ([{ Attributes =
                     [{ TypeName = SynLongIdent ([Foo], [], [None])
                        ArgExpr =
                         Const
                           (Unit,
                            /root/NestedModule/RangeOfAttributeShouldBeIncludedInSynModuleDeclNestedModule.fs (4,2--4,5))
                        Target = None
                        AppliesToGetterAndSetter = false
                        Range =
                         /root/NestedModule/RangeOfAttributeShouldBeIncludedInSynModuleDeclNestedModule.fs (4,2--4,5) }]
                    Range =
                     /root/NestedModule/RangeOfAttributeShouldBeIncludedInSynModuleDeclNestedModule.fs (4,0--4,7) }],
                 None, [], [Nested],
                 PreXmlDoc ((4,0), FSharp.Compiler.Xml.XmlDocCollector), false,
                 None,
                 /root/NestedModule/RangeOfAttributeShouldBeIncludedInSynModuleDeclNestedModule.fs (5,0--5,13)),
              false,
              [Expr
                 (Const
                    (Unit,
                     /root/NestedModule/RangeOfAttributeShouldBeIncludedInSynModuleDeclNestedModule.fs (6,4--6,6)),
                  /root/NestedModule/RangeOfAttributeShouldBeIncludedInSynModuleDeclNestedModule.fs (6,4--6,6))],
              false,
              /root/NestedModule/RangeOfAttributeShouldBeIncludedInSynModuleDeclNestedModule.fs (4,0--6,6),
              { ModuleKeyword =
                 Some
                   /root/NestedModule/RangeOfAttributeShouldBeIncludedInSynModuleDeclNestedModule.fs (5,0--5,6)
                EqualsRange =
                 Some
                   /root/NestedModule/RangeOfAttributeShouldBeIncludedInSynModuleDeclNestedModule.fs (5,14--5,15) })],
          PreXmlDoc ((2,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          /root/NestedModule/RangeOfAttributeShouldBeIncludedInSynModuleDeclNestedModule.fs (2,0--6,6),
          { LeadingKeyword =
             Module
               /root/NestedModule/RangeOfAttributeShouldBeIncludedInSynModuleDeclNestedModule.fs (2,0--2,6) })],
      (true, false), { ConditionalDirectives = []
                       CodeComments = [] }, set []))