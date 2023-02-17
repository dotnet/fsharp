SigFile
  (ParsedSigFileInput
     ("/root/SignatureType/RangeOfAttributeShouldBeIncludedInSynTypeDefnSig.fsi",
      QualifiedNameOfFile RangeOfAttributeShouldBeIncludedInSynTypeDefnSig, [],
      [],
      [SynModuleOrNamespaceSig
         ([SomeNamespace], false, DeclaredNamespace,
          [Types
             ([SynTypeDefnSig
                 (SynComponentInfo
                    ([{ Attributes =
                         [{ TypeName = SynLongIdent ([Foo1], [], [None])
                            ArgExpr = Const (Unit, (4,2--4,6))
                            Target = None
                            AppliesToGetterAndSetter = false
                            Range = (4,2--4,6) }]
                        Range = (4,0--4,8) }], None, [], [MyType],
                     PreXmlDoc ((4,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (5,5--5,11)),
                  ObjectModel (Class, [], (6,4--7,7)), [], (4,0--7,7),
                  { LeadingKeyword = Type (5,0--5,4)
                    EqualsRange = Some (5,12--5,13)
                    WithKeyword = None })], (4,0--7,7))], PreXmlDocEmpty, [],
          None, (2,0--7,7), { LeadingKeyword = Namespace (2,0--2,9) })],
      { ConditionalDirectives = []
        CodeComments = [] }, set []))
