SigFile
  (ParsedSigFileInput
     ("/root/SignatureType/RangeOfAttributesShouldBeIncludedInRecursiveTypes.fsi",
      QualifiedNameOfFile RangeOfAttributesShouldBeIncludedInRecursiveTypes, [],
      [],
      [SynModuleOrNamespaceSig
         ([SomeNamespace], false, DeclaredNamespace,
          [Types
             ([SynTypeDefnSig
                 (SynComponentInfo
                    ([], None, [], [Foo],
                     PreXmlDoc ((4,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None,
                     /root/SignatureType/RangeOfAttributesShouldBeIncludedInRecursiveTypes.fsi (4,5--4,8)),
                  Simple
                    (Union
                       (None,
                        [SynUnionCase
                           ([], SynIdent (Bar, None), Fields [],
                            PreXmlDoc ((5,4), FSharp.Compiler.Xml.XmlDocCollector),
                            None,
                            /root/SignatureType/RangeOfAttributesShouldBeIncludedInRecursiveTypes.fsi (5,6--5,9),
                            { BarRange =
                               Some
                                 /root/SignatureType/RangeOfAttributesShouldBeIncludedInRecursiveTypes.fsi (5,4--5,5) })],
                        /root/SignatureType/RangeOfAttributesShouldBeIncludedInRecursiveTypes.fsi (5,4--5,9)),
                     /root/SignatureType/RangeOfAttributesShouldBeIncludedInRecursiveTypes.fsi (5,4--5,9)),
                  [],
                  /root/SignatureType/RangeOfAttributesShouldBeIncludedInRecursiveTypes.fsi (4,5--5,9),
                  { LeadingKeyword =
                     Type
                       /root/SignatureType/RangeOfAttributesShouldBeIncludedInRecursiveTypes.fsi (4,0--4,4)
                    EqualsRange =
                     Some
                       /root/SignatureType/RangeOfAttributesShouldBeIncludedInRecursiveTypes.fsi (4,9--4,10)
                    WithKeyword = None });
               SynTypeDefnSig
                 (SynComponentInfo
                    ([{ Attributes =
                         [{ TypeName =
                             SynLongIdent ([CustomEquality], [], [None])
                            ArgExpr =
                             Const
                               (Unit,
                                /root/SignatureType/RangeOfAttributesShouldBeIncludedInRecursiveTypes.fsi (7,6--7,20))
                            Target = None
                            AppliesToGetterAndSetter = false
                            Range =
                             /root/SignatureType/RangeOfAttributesShouldBeIncludedInRecursiveTypes.fsi (7,6--7,20) }]
                        Range =
                         /root/SignatureType/RangeOfAttributesShouldBeIncludedInRecursiveTypes.fsi (7,4--7,22) }],
                     None, [], [Bang],
                     PreXmlDoc ((7,4), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None,
                     /root/SignatureType/RangeOfAttributesShouldBeIncludedInRecursiveTypes.fsi (7,23--7,27)),
                  Simple
                    (Record
                       (Some
                          (Internal
                             /root/SignatureType/RangeOfAttributesShouldBeIncludedInRecursiveTypes.fsi (8,4--8,12)),
                        [SynField
                           ([], false, Some LongNameBarBarBarBarBarBarBar,
                            LongIdent (SynLongIdent ([int], [], [None])), false,
                            PreXmlDoc ((10,12), FSharp.Compiler.Xml.XmlDocCollector),
                            None,
                            /root/SignatureType/RangeOfAttributesShouldBeIncludedInRecursiveTypes.fsi (10,12--10,46),
                            { LeadingKeyword = None })],
                        /root/SignatureType/RangeOfAttributesShouldBeIncludedInRecursiveTypes.fsi (8,4--11,9)),
                     /root/SignatureType/RangeOfAttributesShouldBeIncludedInRecursiveTypes.fsi (8,4--11,9)),
                  [Member
                     (SynValSig
                        ([], SynIdent (GetHashCode, None),
                         SynValTyparDecls (None, true),
                         Fun
                           (LongIdent (SynLongIdent ([unit], [], [None])),
                            LongIdent (SynLongIdent ([int], [], [None])),
                            /root/SignatureType/RangeOfAttributesShouldBeIncludedInRecursiveTypes.fsi (12,31--12,42),
                            { ArrowRange =
                               /root/SignatureType/RangeOfAttributesShouldBeIncludedInRecursiveTypes.fsi (12,36--12,38) }),
                         SynValInfo
                           ([[SynArgInfo ([], false, None)]],
                            SynArgInfo ([], false, None)), false, false,
                         PreXmlDoc ((12,8), FSharp.Compiler.Xml.XmlDocCollector),
                         None, None,
                         /root/SignatureType/RangeOfAttributesShouldBeIncludedInRecursiveTypes.fsi (12,8--12,42),
                         { LeadingKeyword =
                            Override
                              /root/SignatureType/RangeOfAttributesShouldBeIncludedInRecursiveTypes.fsi (12,8--12,16)
                           InlineKeyword = None
                           WithKeyword = None
                           EqualsRange = None }),
                      { IsInstance = true
                        IsDispatchSlot = false
                        IsOverrideOrExplicitImpl = true
                        IsFinal = false
                        GetterOrSetterIsCompilerGenerated = false
                        MemberKind = Member },
                      /root/SignatureType/RangeOfAttributesShouldBeIncludedInRecursiveTypes.fsi (12,8--12,42),
                      { GetSetKeywords = None })],
                  /root/SignatureType/RangeOfAttributesShouldBeIncludedInRecursiveTypes.fsi (7,4--12,42),
                  { LeadingKeyword =
                     And
                       /root/SignatureType/RangeOfAttributesShouldBeIncludedInRecursiveTypes.fsi (7,0--7,3)
                    EqualsRange =
                     Some
                       /root/SignatureType/RangeOfAttributesShouldBeIncludedInRecursiveTypes.fsi (7,28--7,29)
                    WithKeyword = None })],
              /root/SignatureType/RangeOfAttributesShouldBeIncludedInRecursiveTypes.fsi (4,0--12,42))],
          PreXmlDocEmpty, [], None,
          /root/SignatureType/RangeOfAttributesShouldBeIncludedInRecursiveTypes.fsi (2,0--12,42),
          { LeadingKeyword =
             Namespace
               /root/SignatureType/RangeOfAttributesShouldBeIncludedInRecursiveTypes.fsi (2,0--2,9) })],
      { ConditionalDirectives = []
        CodeComments = [] }, set []))
