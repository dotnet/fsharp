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
                                /root/RangeOfAttributeShouldBeIncludedInSynTypeDefnSig.fsi (3,2--3,6))
                            Target = None
                            AppliesToGetterAndSetter = false
                            Range =
                             /root/RangeOfAttributeShouldBeIncludedInSynTypeDefnSig.fsi (3,2--3,6) }]
                        Range =
                         /root/RangeOfAttributeShouldBeIncludedInSynTypeDefnSig.fsi (3,0--3,8) }],
                     None, [], [MyType],
                     PreXmlDoc ((3,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None,
                     /root/RangeOfAttributeShouldBeIncludedInSynTypeDefnSig.fsi (4,5--4,11)),
                  ObjectModel
                    (Class, [],
                     /root/RangeOfAttributeShouldBeIncludedInSynTypeDefnSig.fsi (5,4--6,7)),
                  [],
                  /root/RangeOfAttributeShouldBeIncludedInSynTypeDefnSig.fsi (3,0--6,7),
                  { LeadingKeyword =
                     Type
                       /root/RangeOfAttributeShouldBeIncludedInSynTypeDefnSig.fsi (4,0--4,4)
                    EqualsRange =
                     Some
                       /root/RangeOfAttributeShouldBeIncludedInSynTypeDefnSig.fsi (4,12--4,13)
                    WithKeyword = None })],
              /root/RangeOfAttributeShouldBeIncludedInSynTypeDefnSig.fsi (3,0--6,7))],
          PreXmlDocEmpty, [], None,
          /root/RangeOfAttributeShouldBeIncludedInSynTypeDefnSig.fsi (1,0--6,7),
          { LeadingKeyword =
             Namespace
               /root/RangeOfAttributeShouldBeIncludedInSynTypeDefnSig.fsi (1,0--1,9) })],
      { ConditionalDirectives = []
        CodeComments = [] }, set []))