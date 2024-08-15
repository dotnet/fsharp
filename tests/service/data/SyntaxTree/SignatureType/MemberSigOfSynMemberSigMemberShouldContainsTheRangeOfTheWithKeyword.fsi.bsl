SigFile
  (ParsedSigFileInput
     ("/root/SignatureType/MemberSigOfSynMemberSigMemberShouldContainsTheRangeOfTheWithKeyword.fsi",
      QualifiedNameOfFile
        MemberSigOfSynMemberSigMemberShouldContainsTheRangeOfTheWithKeyword, [],
      [],
      [SynModuleOrNamespaceSig
         ([X], false, DeclaredNamespace,
          [Types
             ([SynTypeDefnSig
                 (SynComponentInfo
                    ([], None, [], [Foo],
                     PreXmlDoc ((4,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (4,5--4,8)),
                  ObjectModel
                    (Unspecified,
                     [Member
                        (SynValSig
                           ([], SynIdent (Bar, None),
                            SynValTyparDecls (None, true),
                            LongIdent (SynLongIdent ([int], [], [None])),
                            SynValInfo ([], SynArgInfo ([], false, None)), false,
                            false,
                            PreXmlDoc ((5,4), FSharp.Compiler.Xml.XmlDocCollector),
                            Single None, None, (5,4--5,42),
                            { LeadingKeyword =
                               AbstractMember ((5,4--5,12), (5,13--5,19))
                              InlineKeyword = None
                              WithKeyword = Some (5,30--5,34)
                              EqualsRange = None }),
                         { IsInstance = true
                           IsDispatchSlot = true
                           IsOverrideOrExplicitImpl = false
                           IsFinal = false
                           GetterOrSetterIsCompilerGenerated = false
                           MemberKind = PropertyGetSet }, (5,4--5,42),
                         { GetSetKeywords =
                            Some (GetSet ((5,35--5,38), (5,39--5,42))) })],
                     (5,4--5,42)), [], (4,5--5,42),
                  { LeadingKeyword = Type (4,0--4,4)
                    EqualsRange = Some (4,9--4,10)
                    WithKeyword = None })], (4,0--5,42))], PreXmlDocEmpty, [],
          None, (2,0--5,42), { LeadingKeyword = Namespace (2,0--2,9) })],
      { ConditionalDirectives = []
        CodeComments = [] }, set []))
