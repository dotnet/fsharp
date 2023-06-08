ImplFile
  (ParsedImplFileInput
     ("/root/MemberFlag/SynMemberDefnAbstractSlotHasCorrectKeyword.fs", false,
      QualifiedNameOfFile SynMemberDefnAbstractSlotHasCorrectKeyword, [], [],
      [SynModuleOrNamespace
         ([SynMemberDefnAbstractSlotHasCorrectKeyword], false, AnonModule,
          [Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([], None, [], [Foo],
                     PreXmlDoc ((2,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (2,5--2,8)),
                  ObjectModel
                    (Unspecified,
                     [AbstractSlot
                        (SynValSig
                           ([], SynIdent (X, None),
                            SynValTyparDecls (None, true),
                            LongIdent (SynLongIdent ([int], [], [None])),
                            SynValInfo ([], SynArgInfo ([], false, None)), false,
                            false,
                            PreXmlDoc ((3,4), FSharp.Compiler.Xml.XmlDocCollector),
                            None, None, (3,4--3,20),
                            { LeadingKeyword = Abstract (3,4--3,12)
                              InlineKeyword = None
                              WithKeyword = None
                              EqualsRange = None }),
                         { IsInstance = true
                           IsDispatchSlot = true
                           IsOverrideOrExplicitImpl = false
                           IsFinal = false
                           GetterOrSetterIsCompilerGenerated = false
                           MemberKind = PropertyGet }, (3,4--3,20),
                         { GetSetKeywords = None });
                      AbstractSlot
                        (SynValSig
                           ([], SynIdent (Y, None),
                            SynValTyparDecls (None, true),
                            LongIdent (SynLongIdent ([int], [], [None])),
                            SynValInfo ([], SynArgInfo ([], false, None)), false,
                            false,
                            PreXmlDoc ((4,4), FSharp.Compiler.Xml.XmlDocCollector),
                            None, None, (4,4--4,26),
                            { LeadingKeyword =
                               AbstractMember ((4,4--4,12), (4,13--4,19))
                              InlineKeyword = None
                              WithKeyword = None
                              EqualsRange = None }),
                         { IsInstance = true
                           IsDispatchSlot = true
                           IsOverrideOrExplicitImpl = false
                           IsFinal = false
                           GetterOrSetterIsCompilerGenerated = false
                           MemberKind = PropertyGet }, (4,4--4,26),
                         { GetSetKeywords = None })], (3,4--4,26)), [], None,
                  (2,5--4,26), { LeadingKeyword = Type (2,0--2,4)
                                 EqualsRange = Some (2,9--2,10)
                                 WithKeyword = None })], (2,0--4,26))],
          PreXmlDocEmpty, [], None, (2,0--5,0), { LeadingKeyword = None })],
      (true, true), { ConditionalDirectives = []
                      CodeComments = [] }, set []))
