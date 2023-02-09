SigFile
  (ParsedSigFileInput
     ("/root/RangeOfAttributeShouldBeIncludedInSynModuleSigDeclNestedModule.fsi",
      QualifiedNameOfFile
        RangeOfAttributeShouldBeIncludedInSynModuleSigDeclNestedModule, [], [],
      [SynModuleOrNamespaceSig
         ([SomeNamespace], false, DeclaredNamespace,
          [NestedModule
             (SynComponentInfo
                ([{ Attributes =
                     [{ TypeName = SynLongIdent ([Foo], [], [None])
                        ArgExpr =
                         Const
                           (Unit,
                            /root/RangeOfAttributeShouldBeIncludedInSynModuleSigDeclNestedModule.fsi (3,2--3,5))
                        Target = None
                        AppliesToGetterAndSetter = false
                        Range =
                         /root/RangeOfAttributeShouldBeIncludedInSynModuleSigDeclNestedModule.fsi (3,2--3,5) }]
                    Range =
                     /root/RangeOfAttributeShouldBeIncludedInSynModuleSigDeclNestedModule.fsi (3,0--3,7) }],
                 None, [], [Nested],
                 PreXmlDoc ((3,0), FSharp.Compiler.Xml.XmlDocCollector), false,
                 None,
                 /root/RangeOfAttributeShouldBeIncludedInSynModuleSigDeclNestedModule.fsi (4,0--4,13)),
              false,
              [Val
                 (SynValSig
                    ([], SynIdent (x, None), SynValTyparDecls (None, true),
                     LongIdent (SynLongIdent ([int], [], [None])),
                     SynValInfo ([], SynArgInfo ([], false, None)), false, false,
                     PreXmlDoc ((5,4), FSharp.Compiler.Xml.XmlDocCollector),
                     None, None,
                     /root/RangeOfAttributeShouldBeIncludedInSynModuleSigDeclNestedModule.fsi (5,4--5,15),
                     { LeadingKeyword =
                        Val
                          /root/RangeOfAttributeShouldBeIncludedInSynModuleSigDeclNestedModule.fsi (5,4--5,7)
                       InlineKeyword = None
                       WithKeyword = None
                       EqualsRange = None }),
                  /root/RangeOfAttributeShouldBeIncludedInSynModuleSigDeclNestedModule.fsi (5,4--5,15))],
              /root/RangeOfAttributeShouldBeIncludedInSynModuleSigDeclNestedModule.fsi (3,0--5,15),
              { ModuleKeyword =
                 Some
                   /root/RangeOfAttributeShouldBeIncludedInSynModuleSigDeclNestedModule.fsi (4,0--4,6)
                EqualsRange =
                 Some
                   /root/RangeOfAttributeShouldBeIncludedInSynModuleSigDeclNestedModule.fsi (4,14--4,15) })],
          PreXmlDocEmpty, [], None,
          /root/RangeOfAttributeShouldBeIncludedInSynModuleSigDeclNestedModule.fsi (1,0--5,15),
          { LeadingKeyword =
             Namespace
               /root/RangeOfAttributeShouldBeIncludedInSynModuleSigDeclNestedModule.fsi (1,0--1,9) })],
      { ConditionalDirectives = []
        CodeComments = [] }, set []))