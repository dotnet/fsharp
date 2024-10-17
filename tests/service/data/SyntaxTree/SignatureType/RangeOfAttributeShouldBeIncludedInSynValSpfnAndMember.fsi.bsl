SigFile
  (ParsedSigFileInput
     ("/root/SignatureType/RangeOfAttributeShouldBeIncludedInSynValSpfnAndMember.fsi",
      QualifiedNameOfFile RangeOfAttributeShouldBeIncludedInSynValSpfnAndMember,
      [], [],
      [SynModuleOrNamespaceSig
         ([SomeNamespace], false, DeclaredNamespace,
          [Types
             ([SynTypeDefnSig
                 (SynComponentInfo
                    ([], None, [], [FooType],
                     PreXmlDoc ((4,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (4,5--4,12)),
                  ObjectModel
                    (Unspecified,
                     [Member
                        (SynValSig
                           ([{ Attributes =
                                [{ TypeName = SynLongIdent ([Foo2], [], [None])
                                   ArgExpr = Const (Unit, (5,6--5,10))
                                   Target = None
                                   AppliesToGetterAndSetter = false
                                   Range = (5,6--5,10) }]
                               Range = (5,4--5,12) }], SynIdent (x, None),
                            SynValTyparDecls (None, true),
                            LongIdent (SynLongIdent ([int], [], [None])),
                            SynValInfo ([], SynArgInfo ([], false, None)), false,
                            false,
                            PreXmlDoc ((5,4), FSharp.Compiler.Xml.XmlDocCollector),
                            Single None, None, (5,4--6,20),
                            { LeadingKeyword = Abstract (6,4--6,12)
                              InlineKeyword = None
                              WithKeyword = None
                              EqualsRange = None }),
                         { IsInstance = true
                           IsDispatchSlot = true
                           IsOverrideOrExplicitImpl = false
                           IsFinal = false
                           GetterOrSetterIsCompilerGenerated = false
                           MemberKind = PropertyGet }, (5,4--6,20),
                         { GetSetKeywords = None })], (5,4--6,20)), [],
                  (4,5--6,20), { LeadingKeyword = Type (4,0--4,4)
                                 EqualsRange = Some (4,13--4,14)
                                 WithKeyword = None })], (4,0--6,20))],
          PreXmlDocEmpty, [], None, (2,0--6,20),
          { LeadingKeyword = Namespace (2,0--2,9) })],
      { ConditionalDirectives = []
        CodeComments = [LineComment (5,13--5,23)] }, set []))
