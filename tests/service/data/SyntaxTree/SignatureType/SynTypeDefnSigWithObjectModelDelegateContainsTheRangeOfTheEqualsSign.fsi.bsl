SigFile
  (ParsedSigFileInput
     ("/root/SignatureType/SynTypeDefnSigWithObjectModelDelegateContainsTheRangeOfTheEqualsSign.fsi",
      QualifiedNameOfFile
        SynTypeDefnSigWithObjectModelDelegateContainsTheRangeOfTheEqualsSign, [],
      [],
      [SynModuleOrNamespaceSig
         ([Foo], false, DeclaredNamespace,
          [Types
             ([SynTypeDefnSig
                 (SynComponentInfo
                    ([], None, [], [X],
                     PreXmlDoc ((4,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (4,5--4,6)),
                  ObjectModel
                    (Delegate
                       (Fun
                          (LongIdent (SynLongIdent ([string], [], [None])),
                           LongIdent (SynLongIdent ([string], [], [None])),
                           (4,21--4,37), { ArrowRange = (4,28--4,30) }),
                        SynValInfo
                          ([[SynArgInfo ([], false, None)]],
                           SynArgInfo ([], false, None))),
                     [Member
                        (SynValSig
                           ([], SynIdent (Invoke, None),
                            SynValTyparDecls (None, true),
                            Fun
                              (LongIdent (SynLongIdent ([string], [], [None])),
                               LongIdent (SynLongIdent ([string], [], [None])),
                               (4,21--4,37), { ArrowRange = (4,28--4,30) }),
                            SynValInfo
                              ([[SynArgInfo ([], false, None)]],
                               SynArgInfo ([], false, None)), false, false,
                            PreXmlDocEmpty, Single None, None, (4,9--4,37),
                            { LeadingKeyword = Synthetic
                              InlineKeyword = None
                              WithKeyword = None
                              EqualsRange = None }),
                         { IsInstance = true
                           IsDispatchSlot = true
                           IsOverrideOrExplicitImpl = false
                           IsFinal = false
                           GetterOrSetterIsCompilerGenerated = false
                           MemberKind = Member }, (4,9--4,37),
                         { GetSetKeywords = None })], (4,9--4,37)), [],
                  (4,5--4,37), { LeadingKeyword = Type (4,0--4,4)
                                 EqualsRange = Some (4,7--4,8)
                                 WithKeyword = None })], (4,0--4,37))],
          PreXmlDocEmpty, [], None, (2,0--4,37),
          { LeadingKeyword = Namespace (2,0--2,9) })],
      { ConditionalDirectives = []
        CodeComments = [] }, set []))
