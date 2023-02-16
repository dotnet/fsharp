ImplFile
  (ParsedImplFileInput
     ("/root/LeadingKeyword/StaticAbstractMemberKeyword.fs", false,
      QualifiedNameOfFile StaticAbstractMemberKeyword, [], [],
      [SynModuleOrNamespace
         ([StaticAbstractMemberKeyword], false, AnonModule,
          [Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([], None, [], [X],
                     PreXmlDoc ((2,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (2,5--2,6)),
                  ObjectModel
                    (Unspecified,
                     [AbstractSlot
                        (SynValSig
                           ([], SynIdent (Y, None),
                            SynValTyparDecls (None, true),
                            Fun
                              (LongIdent (SynLongIdent ([int], [], [None])),
                               LongIdent (SynLongIdent ([int], [], [None])),
                               (3,31--3,41), { ArrowRange = (3,35--3,37) }),
                            SynValInfo
                              ([[SynArgInfo ([], false, None)]],
                               SynArgInfo ([], false, None)), false, false,
                            PreXmlDoc ((3,4), FSharp.Compiler.Xml.XmlDocCollector),
                            None, None, (3,4--3,41),
                            { LeadingKeyword =
                               StaticAbstractMember
                                 ((3,4--3,10), (3,11--3,19), (3,20--3,26))
                              InlineKeyword = None
                              WithKeyword = None
                              EqualsRange = None }),
                         { IsInstance = false
                           IsDispatchSlot = true
                           IsOverrideOrExplicitImpl = false
                           IsFinal = false
                           GetterOrSetterIsCompilerGenerated = false
                           MemberKind = Member }, (3,4--3,41),
                         { GetSetKeywords = None })], (3,4--3,41)), [], None,
                  (2,5--3,41), { LeadingKeyword = Type (2,0--2,4)
                                 EqualsRange = Some (2,7--2,8)
                                 WithKeyword = None })], (2,0--3,41))],
          PreXmlDocEmpty, [], None, (2,0--4,0), { LeadingKeyword = None })],
      (true, false), { ConditionalDirectives = []
                       CodeComments = [] }, set []))
