SigFile
  (ParsedSigFileInput
     ("/root/RangeOfAttributeShouldBeIncludedInSynValSpfnAndMember.fsi",
      QualifiedNameOfFile RangeOfAttributeShouldBeIncludedInSynValSpfnAndMember,
      [], [],
      [SynModuleOrNamespaceSig
         ([SomeNamespace], false, DeclaredNamespace,
          [Types
             ([SynTypeDefnSig
                 (SynComponentInfo
                    ([], None, [], [FooType],
                     PreXmlDoc ((4,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None,
                     /root/RangeOfAttributeShouldBeIncludedInSynValSpfnAndMember.fsi (4,5--4,12)),
                  ObjectModel
                    (Unspecified,
                     [Member
                        (SynValSig
                           ([{ Attributes =
                                [{ TypeName = SynLongIdent ([Foo2], [], [None])
                                   ArgExpr =
                                    Const
                                      (Unit,
                                       /root/RangeOfAttributeShouldBeIncludedInSynValSpfnAndMember.fsi (5,6--5,10))
                                   Target = None
                                   AppliesToGetterAndSetter = false
                                   Range =
                                    /root/RangeOfAttributeShouldBeIncludedInSynValSpfnAndMember.fsi (5,6--5,10) }]
                               Range =
                                /root/RangeOfAttributeShouldBeIncludedInSynValSpfnAndMember.fsi (5,4--5,12) }],
                            SynIdent (x, None), SynValTyparDecls (None, true),
                            LongIdent (SynLongIdent ([int], [], [None])),
                            SynValInfo ([], SynArgInfo ([], false, None)), false,
                            false,
                            PreXmlDoc ((5,4), FSharp.Compiler.Xml.XmlDocCollector),
                            None, None,
                            /root/RangeOfAttributeShouldBeIncludedInSynValSpfnAndMember.fsi (5,4--6,20),
                            { LeadingKeyword =
                               Abstract
                                 /root/RangeOfAttributeShouldBeIncludedInSynValSpfnAndMember.fsi (6,4--6,12)
                              InlineKeyword = None
                              WithKeyword = None
                              EqualsRange = None }),
                         { IsInstance = true
                           IsDispatchSlot = true
                           IsOverrideOrExplicitImpl = false
                           IsFinal = false
                           GetterOrSetterIsCompilerGenerated = false
                           MemberKind = PropertyGet },
                         /root/RangeOfAttributeShouldBeIncludedInSynValSpfnAndMember.fsi (5,4--6,20),
                         { GetSetKeywords = None })],
                     /root/RangeOfAttributeShouldBeIncludedInSynValSpfnAndMember.fsi (5,4--6,20)),
                  [],
                  /root/RangeOfAttributeShouldBeIncludedInSynValSpfnAndMember.fsi (4,5--6,20),
                  { LeadingKeyword =
                     Type
                       /root/RangeOfAttributeShouldBeIncludedInSynValSpfnAndMember.fsi (4,0--4,4)
                    EqualsRange =
                     Some
                       /root/RangeOfAttributeShouldBeIncludedInSynValSpfnAndMember.fsi (4,13--4,14)
                    WithKeyword = None })],
              /root/RangeOfAttributeShouldBeIncludedInSynValSpfnAndMember.fsi (4,0--6,20))],
          PreXmlDocEmpty, [], None,
          /root/RangeOfAttributeShouldBeIncludedInSynValSpfnAndMember.fsi (2,0--6,20),
          { LeadingKeyword =
             Namespace
               /root/RangeOfAttributeShouldBeIncludedInSynValSpfnAndMember.fsi (2,0--2,9) })],
      { ConditionalDirectives = []
        CodeComments =
         [LineComment
            /root/RangeOfAttributeShouldBeIncludedInSynValSpfnAndMember.fsi (5,13--5,23)] },
      set []))