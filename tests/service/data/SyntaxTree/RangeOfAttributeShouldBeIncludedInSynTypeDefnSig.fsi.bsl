SigFile
  (ParsedSigFileInput
     ("/root/RangeOfAttributeShouldBeIncludedInSynTypeDefnSig.fsi",
      QualifiedNameOfFile RangeOfAttributeShouldBeIncludedInSynTypeDefnSig, [],
      [],
      [SynModuleOrNamespaceSig
         ([SomeNamespace], false, DeclaredNamespace,
          [Types
             ([SynTypeDefnSig
                 (SynComponentInfo
                    ([{ Attributes =
                         [{ TypeName = SynLongIdent ([Foo1], [], [None])
                            ArgExpr =
                             Const
                               (Unit,
                                /root/RangeOfAttributeShouldBeIncludedInSynTypeDefnSig.fsi (4,2--4,6))
                            Target = None
                            AppliesToGetterAndSetter = false
                            Range =
                             /root/RangeOfAttributeShouldBeIncludedInSynTypeDefnSig.fsi (4,2--4,6) }]
                        Range =
                         /root/RangeOfAttributeShouldBeIncludedInSynTypeDefnSig.fsi (4,0--4,8) }],
                     None, [], [MyType],
                     PreXmlDoc ((4,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None,
                     /root/RangeOfAttributeShouldBeIncludedInSynTypeDefnSig.fsi (5,5--5,11)),
                  ObjectModel
                    (Class, [],
                     /root/RangeOfAttributeShouldBeIncludedInSynTypeDefnSig.fsi (6,4--7,7)),
                  [],
                  /root/RangeOfAttributeShouldBeIncludedInSynTypeDefnSig.fsi (4,0--7,7),
                  { LeadingKeyword =
                     Type
                       /root/RangeOfAttributeShouldBeIncludedInSynTypeDefnSig.fsi (5,0--5,4)
                    EqualsRange =
                     Some
                       /root/RangeOfAttributeShouldBeIncludedInSynTypeDefnSig.fsi (5,12--5,13)
                    WithKeyword = None })],
              /root/RangeOfAttributeShouldBeIncludedInSynTypeDefnSig.fsi (4,0--7,7))],
          PreXmlDocEmpty, [], None,
          /root/RangeOfAttributeShouldBeIncludedInSynTypeDefnSig.fsi (2,0--7,7),
          { LeadingKeyword =
             Namespace
               /root/RangeOfAttributeShouldBeIncludedInSynTypeDefnSig.fsi (2,0--2,9) })],
      { ConditionalDirectives = []
        CodeComments = [] }, set []))