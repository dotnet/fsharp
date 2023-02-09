SigFile
  (ParsedSigFileInput
     ("/root/RangeOfAttributesShouldBeIncludedInRecursiveTypes.fsi",
      QualifiedNameOfFile RangeOfAttributesShouldBeIncludedInRecursiveTypes, [],
      [],
      [SynModuleOrNamespaceSig
         ([SomeNamespace], false, DeclaredNamespace,
          [Types
             ([SynTypeDefnSig
                 (SynComponentInfo
                    ([], None, [], [Foo],
                     PreXmlDoc ((3,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None,
                     /root/RangeOfAttributesShouldBeIncludedInRecursiveTypes.fsi (3,5--3,8)),
                  Simple
                    (Union
                       (None,
                        [SynUnionCase
                           ([], SynIdent (Bar, None), Fields [],
                            PreXmlDoc ((4,4), FSharp.Compiler.Xml.XmlDocCollector),
                            None,
                            /root/RangeOfAttributesShouldBeIncludedInRecursiveTypes.fsi (4,6--4,9),
                            { BarRange =
                               Some
                                 /root/RangeOfAttributesShouldBeIncludedInRecursiveTypes.fsi (4,4--4,5) })],
                        /root/RangeOfAttributesShouldBeIncludedInRecursiveTypes.fsi (4,4--4,9)),
                     /root/RangeOfAttributesShouldBeIncludedInRecursiveTypes.fsi (4,4--4,9)),
                  [],
                  /root/RangeOfAttributesShouldBeIncludedInRecursiveTypes.fsi (3,5--4,9),
                  { LeadingKeyword =
                     Type
                       /root/RangeOfAttributesShouldBeIncludedInRecursiveTypes.fsi (3,0--3,4)
                    EqualsRange =
                     Some
                       /root/RangeOfAttributesShouldBeIncludedInRecursiveTypes.fsi (3,9--3,10)
                    WithKeyword = None });
               SynTypeDefnSig
                 (SynComponentInfo
                    ([{ Attributes =
                         [{ TypeName =
                             SynLongIdent ([CustomEquality], [], [None])
                            ArgExpr =
                             Const
                               (Unit,
                                /root/RangeOfAttributesShouldBeIncludedInRecursiveTypes.fsi (6,6--6,20))
                            Target = None
                            AppliesToGetterAndSetter = false
                            Range =
                             /root/RangeOfAttributesShouldBeIncludedInRecursiveTypes.fsi (6,6--6,20) }]
                        Range =
                         /root/RangeOfAttributesShouldBeIncludedInRecursiveTypes.fsi (6,4--6,22) }],
                     None, [], [Bang],
                     PreXmlDoc ((6,4), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None,
                     /root/RangeOfAttributesShouldBeIncludedInRecursiveTypes.fsi (6,23--6,27)),
                  Simple
                    (Record
                       (Some
                          (Internal
                             /root/RangeOfAttributesShouldBeIncludedInRecursiveTypes.fsi (7,4--7,12)),
                        [SynField
                           ([], false, Some LongNameBarBarBarBarBarBarBar,
                            LongIdent (SynLongIdent ([int], [], [None])), false,
                            PreXmlDoc ((9,12), FSharp.Compiler.Xml.XmlDocCollector),
                            None,
                            /root/RangeOfAttributesShouldBeIncludedInRecursiveTypes.fsi (9,12--9,46),
                            { LeadingKeyword = None })],
                        /root/RangeOfAttributesShouldBeIncludedInRecursiveTypes.fsi (7,4--10,9)),
                     /root/RangeOfAttributesShouldBeIncludedInRecursiveTypes.fsi (7,4--10,9)),
                  [Member
                     (SynValSig
                        ([], SynIdent (GetHashCode, None),
                         SynValTyparDecls (None, true),
                         Fun
                           (LongIdent (SynLongIdent ([unit], [], [None])),
                            LongIdent (SynLongIdent ([int], [], [None])),
                            /root/RangeOfAttributesShouldBeIncludedInRecursiveTypes.fsi (11,31--11,42),
                            { ArrowRange =
                               /root/RangeOfAttributesShouldBeIncludedInRecursiveTypes.fsi (11,36--11,38) }),
                         SynValInfo
                           ([[SynArgInfo ([], false, None)]],
                            SynArgInfo ([], false, None)), false, false,
                         PreXmlDoc ((11,8), FSharp.Compiler.Xml.XmlDocCollector),
                         None, None,
                         /root/RangeOfAttributesShouldBeIncludedInRecursiveTypes.fsi (11,8--11,42),
                         { LeadingKeyword =
                            Override
                              /root/RangeOfAttributesShouldBeIncludedInRecursiveTypes.fsi (11,8--11,16)
                           InlineKeyword = None
                           WithKeyword = None
                           EqualsRange = None }),
                      { IsInstance = true
                        IsDispatchSlot = false
                        IsOverrideOrExplicitImpl = true
                        IsFinal = false
                        GetterOrSetterIsCompilerGenerated = false
                        MemberKind = Member },
                      /root/RangeOfAttributesShouldBeIncludedInRecursiveTypes.fsi (11,8--11,42),
                      { GetSetKeywords = None })],
                  /root/RangeOfAttributesShouldBeIncludedInRecursiveTypes.fsi (6,4--11,42),
                  { LeadingKeyword =
                     And
                       /root/RangeOfAttributesShouldBeIncludedInRecursiveTypes.fsi (6,0--6,3)
                    EqualsRange =
                     Some
                       /root/RangeOfAttributesShouldBeIncludedInRecursiveTypes.fsi (6,28--6,29)
                    WithKeyword = None })],
              /root/RangeOfAttributesShouldBeIncludedInRecursiveTypes.fsi (3,0--11,42))],
          PreXmlDocEmpty, [], None,
          /root/RangeOfAttributesShouldBeIncludedInRecursiveTypes.fsi (1,0--11,42),
          { LeadingKeyword =
             Namespace
               /root/RangeOfAttributesShouldBeIncludedInRecursiveTypes.fsi (1,0--1,9) })],
      { ConditionalDirectives = []
        CodeComments = [] }, set []))