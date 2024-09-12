ImplFile
  (ParsedImplFileInput
     ("/root/LeadingKeyword/OverrideValKeyword.fs", false,
      QualifiedNameOfFile OverrideValKeyword, [], [],
      [SynModuleOrNamespace
         ([OverrideValKeyword], false, AnonModule,
          [Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([], None, [], [X],
                     PreXmlDoc ((2,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (2,5--2,6)),
                  ObjectModel
                    (Unspecified,
                     [AutoProperty
                        ([], false, Y,
                         Some (LongIdent (SynLongIdent ([int], [], [None]))),
                         Member, { IsInstance = true
                                   IsDispatchSlot = false
                                   IsOverrideOrExplicitImpl = true
                                   IsFinal = false
                                   GetterOrSetterIsCompilerGenerated = false
                                   MemberKind = Member },
                         { IsInstance = true
                           IsDispatchSlot = false
                           IsOverrideOrExplicitImpl = true
                           IsFinal = false
                           GetterOrSetterIsCompilerGenerated = false
                           MemberKind = PropertySet },
                         PreXmlDoc ((3,4), FSharp.Compiler.Xml.XmlDocCollector),
                         GetSet (None, None, None),
                         Const (Int32 1, (3,27--3,28)), (3,4--3,28),
                         { LeadingKeyword =
                            OverrideVal ((3,4--3,12), (3,13--3,16))
                           WithKeyword = None
                           EqualsRange = Some (3,25--3,26)
                           GetSetKeywords = None })], (3,4--3,28)), [], None,
                  (2,5--3,28), { LeadingKeyword = Type (2,0--2,4)
                                 EqualsRange = Some (2,7--2,8)
                                 WithKeyword = None })], (2,0--3,28))],
          PreXmlDocEmpty, [], None, (2,0--4,0), { LeadingKeyword = None })],
      (true, true), { ConditionalDirectives = []
                      CodeComments = [] }, set []))
