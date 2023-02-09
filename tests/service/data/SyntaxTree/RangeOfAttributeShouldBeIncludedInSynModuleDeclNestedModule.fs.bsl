ImplFile
  (ParsedImplFileInput
     ("/root/RangeOfAttributeShouldBeIncludedInSynModuleDeclNestedModule.fs",
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
                            /root/RangeOfAttributeShouldBeIncludedInSynModuleDeclNestedModule.fs (3,2--3,5))
                        Target = None
                        AppliesToGetterAndSetter = false
                        Range =
                         /root/RangeOfAttributeShouldBeIncludedInSynModuleDeclNestedModule.fs (3,2--3,5) }]
                    Range =
                     /root/RangeOfAttributeShouldBeIncludedInSynModuleDeclNestedModule.fs (3,0--3,7) }],
                 None, [], [Nested],
                 PreXmlDoc ((3,0), FSharp.Compiler.Xml.XmlDocCollector), false,
                 None,
                 /root/RangeOfAttributeShouldBeIncludedInSynModuleDeclNestedModule.fs (4,0--4,13)),
              false,
              [Expr
                 (Const
                    (Unit,
                     /root/RangeOfAttributeShouldBeIncludedInSynModuleDeclNestedModule.fs (5,4--5,6)),
                  /root/RangeOfAttributeShouldBeIncludedInSynModuleDeclNestedModule.fs (5,4--5,6))],
              false,
              /root/RangeOfAttributeShouldBeIncludedInSynModuleDeclNestedModule.fs (3,0--5,6),
              { ModuleKeyword =
                 Some
                   /root/RangeOfAttributeShouldBeIncludedInSynModuleDeclNestedModule.fs (4,0--4,6)
                EqualsRange =
                 Some
                   /root/RangeOfAttributeShouldBeIncludedInSynModuleDeclNestedModule.fs (4,14--4,15) })],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          /root/RangeOfAttributeShouldBeIncludedInSynModuleDeclNestedModule.fs (1,0--5,6),
          { LeadingKeyword =
             Module
               /root/RangeOfAttributeShouldBeIncludedInSynModuleDeclNestedModule.fs (1,0--1,6) })],
      (true, false), { ConditionalDirectives = []
                       CodeComments = [] }, set []))