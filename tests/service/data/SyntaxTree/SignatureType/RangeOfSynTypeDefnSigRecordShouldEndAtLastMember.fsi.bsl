SigFile
  (ParsedSigFileInput
     ("/root/SignatureType/RangeOfSynTypeDefnSigRecordShouldEndAtLastMember.fsi",
      QualifiedNameOfFile RangeOfSynTypeDefnSigRecordShouldEndAtLastMember, [],
      [],
      [SynModuleOrNamespaceSig
         ([X], false, DeclaredNamespace,
          [Types
             ([SynTypeDefnSig
                 (SynComponentInfo
                    ([], None, [], [MyRecord],
                     PreXmlDoc ((3,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (3,5--3,13)),
                  Simple
                    (Record
                       (None,
                        [SynField
                           ([], false, Some Level,
                            LongIdent (SynLongIdent ([int], [], [None])), false,
                            PreXmlDoc ((4,6), FSharp.Compiler.Xml.XmlDocCollector),
                            None, (4,6--4,16), { LeadingKeyword = None })],
                        (4,4--4,18)), (4,4--4,18)),
                  [Member
                     (SynValSig
                        ([], SynIdent (Score, None),
                         SynValTyparDecls (None, true),
                         Fun
                           (LongIdent (SynLongIdent ([unit], [], [None])),
                            LongIdent (SynLongIdent ([int], [], [None])),
                            (5,19--5,30), { ArrowRange = (5,24--5,26) }),
                         SynValInfo
                           ([[SynArgInfo ([], false, None)]],
                            SynArgInfo ([], false, None)), false, false,
                         PreXmlDoc ((5,4), FSharp.Compiler.Xml.XmlDocCollector),
                         None, None, (5,4--5,30),
                         { LeadingKeyword = Member (5,4--5,10)
                           InlineKeyword = None
                           WithKeyword = None
                           EqualsRange = None }),
                      { IsInstance = true
                        IsDispatchSlot = false
                        IsOverrideOrExplicitImpl = false
                        IsFinal = false
                        GetterOrSetterIsCompilerGenerated = false
                        MemberKind = Member }, (5,4--5,30),
                      { GetSetKeywords = None })], (3,5--5,30),
                  { LeadingKeyword = Type (3,0--3,4)
                    EqualsRange = Some (3,14--3,15)
                    WithKeyword = None })], (3,0--5,30))], PreXmlDocEmpty, [],
          None, (2,0--5,30), { LeadingKeyword = Namespace (2,0--2,9) })],
      { ConditionalDirectives = []
        CodeComments = [] }, set []))
