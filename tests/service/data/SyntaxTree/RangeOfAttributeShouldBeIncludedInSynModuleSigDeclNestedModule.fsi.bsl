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
                            /root/RangeOfAttributeShouldBeIncludedInSynModuleSigDeclNestedModule.fsi (4,2--4,5))
                        Target = None
                        AppliesToGetterAndSetter = false
                        Range =
                         /root/RangeOfAttributeShouldBeIncludedInSynModuleSigDeclNestedModule.fsi (4,2--4,5) }]
                    Range =
                     /root/RangeOfAttributeShouldBeIncludedInSynModuleSigDeclNestedModule.fsi (4,0--4,7) }],
                 None, [], [Nested],
                 PreXmlDoc ((4,0), FSharp.Compiler.Xml.XmlDocCollector), false,
                 None,
                 /root/RangeOfAttributeShouldBeIncludedInSynModuleSigDeclNestedModule.fsi (5,0--5,13)),
              false,
              [Val
                 (SynValSig
                    ([], SynIdent (x, None), SynValTyparDecls (None, true),
                     LongIdent (SynLongIdent ([int], [], [None])),
                     SynValInfo ([], SynArgInfo ([], false, None)), false, false,
                     PreXmlDoc ((6,4), FSharp.Compiler.Xml.XmlDocCollector),
                     None, None,
                     /root/RangeOfAttributeShouldBeIncludedInSynModuleSigDeclNestedModule.fsi (6,4--6,15),
                     { LeadingKeyword =
                        Val
                          /root/RangeOfAttributeShouldBeIncludedInSynModuleSigDeclNestedModule.fsi (6,4--6,7)
                       InlineKeyword = None
                       WithKeyword = None
                       EqualsRange = None }),
                  /root/RangeOfAttributeShouldBeIncludedInSynModuleSigDeclNestedModule.fsi (6,4--6,15))],
              /root/RangeOfAttributeShouldBeIncludedInSynModuleSigDeclNestedModule.fsi (4,0--6,15),
              { ModuleKeyword =
                 Some
                   /root/RangeOfAttributeShouldBeIncludedInSynModuleSigDeclNestedModule.fsi (5,0--5,6)
                EqualsRange =
                 Some
                   /root/RangeOfAttributeShouldBeIncludedInSynModuleSigDeclNestedModule.fsi (5,14--5,15) })],
          PreXmlDocEmpty, [], None,
          /root/RangeOfAttributeShouldBeIncludedInSynModuleSigDeclNestedModule.fsi (2,0--6,15),
          { LeadingKeyword =
             Namespace
               /root/RangeOfAttributeShouldBeIncludedInSynModuleSigDeclNestedModule.fsi (2,0--2,9) })],
      { ConditionalDirectives = []
        CodeComments = [] }, set []))