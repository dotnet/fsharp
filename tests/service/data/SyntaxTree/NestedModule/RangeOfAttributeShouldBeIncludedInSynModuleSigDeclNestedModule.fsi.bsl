SigFile
  (ParsedSigFileInput
     ("/root/NestedModule/RangeOfAttributeShouldBeIncludedInSynModuleSigDeclNestedModule.fsi",
      QualifiedNameOfFile
        RangeOfAttributeShouldBeIncludedInSynModuleSigDeclNestedModule, [], [],
      [SynModuleOrNamespaceSig
         ([SomeNamespace], false, DeclaredNamespace,
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
              [Val
                 (SynValSig
                    ([], SynIdent (x, None), SynValTyparDecls (None, true),
                     LongIdent (SynLongIdent ([int], [], [None])),
                     SynValInfo ([], SynArgInfo ([], false, None)), false, false,
                     PreXmlDoc ((6,4), FSharp.Compiler.Xml.XmlDocCollector),
                     None, None, (6,4--6,15), { LeadingKeyword = Val (6,4--6,7)
                                                InlineKeyword = None
                                                WithKeyword = None
                                                EqualsRange = None }),
                  (6,4--6,15))], (4,0--6,15),
              { ModuleKeyword = Some (5,0--5,6)
                EqualsRange = Some (5,14--5,15) })], PreXmlDocEmpty, [], None,
          (2,0--6,15), { LeadingKeyword = Namespace (2,0--2,9) })],
      { ConditionalDirectives = []
        CodeComments = [] }, set []))
