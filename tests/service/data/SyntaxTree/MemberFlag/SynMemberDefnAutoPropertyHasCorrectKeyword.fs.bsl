ImplFile
  (ParsedImplFileInput
     ("/root/MemberFlag/SynMemberDefnAutoPropertyHasCorrectKeyword.fs", false,
      QualifiedNameOfFile SynMemberDefnAutoPropertyHasCorrectKeyword, [], [],
      [SynModuleOrNamespace
         ([SynMemberDefnAutoPropertyHasCorrectKeyword], false, AnonModule,
          [Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([], None, [], [Foo],
                     PreXmlDoc ((2,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (2,5--2,8)),
                  ObjectModel
                    (Unspecified,
                     [AutoProperty
                        ([], true, W,
                         Some (LongIdent (SynLongIdent ([int], [], [None]))),
                         Member, { IsInstance = false
                                   IsDispatchSlot = false
                                   IsOverrideOrExplicitImpl = false
                                   IsFinal = false
                                   GetterOrSetterIsCompilerGenerated = false
                                   MemberKind = Member },
                         { IsInstance = false
                           IsDispatchSlot = false
                           IsOverrideOrExplicitImpl = false
                           IsFinal = false
                           GetterOrSetterIsCompilerGenerated = false
                           MemberKind = PropertySet },
                         PreXmlDoc ((3,4), FSharp.Compiler.Xml.XmlDocCollector),
                         None, Const (Int32 1, (3,32--3,33)), (3,4--3,33),
                         { LeadingKeyword =
                            StaticMemberVal
                              ((3,4--3,10), (3,11--3,17), (3,18--3,21))
                           WithKeyword = None
                           EqualsRange = Some (3,30--3,31)
                           GetSetKeywords = None });
                      AutoProperty
                        ([], false, X,
                         Some (LongIdent (SynLongIdent ([int], [], [None]))),
                         Member, { IsInstance = true
                                   IsDispatchSlot = false
                                   IsOverrideOrExplicitImpl = false
                                   IsFinal = false
                                   GetterOrSetterIsCompilerGenerated = false
                                   MemberKind = Member },
                         { IsInstance = true
                           IsDispatchSlot = false
                           IsOverrideOrExplicitImpl = false
                           IsFinal = false
                           GetterOrSetterIsCompilerGenerated = false
                           MemberKind = PropertySet },
                         PreXmlDoc ((4,4), FSharp.Compiler.Xml.XmlDocCollector),
                         None, Const (Int32 1, (4,25--4,26)), (4,4--4,26),
                         { LeadingKeyword =
                            MemberVal ((4,4--4,10), (4,11--4,14))
                           WithKeyword = None
                           EqualsRange = Some (4,23--4,24)
                           GetSetKeywords = None });
                      AutoProperty
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
                         PreXmlDoc ((5,4), FSharp.Compiler.Xml.XmlDocCollector),
                         None, Const (Int32 2, (5,27--5,28)), (5,4--5,28),
                         { LeadingKeyword =
                            OverrideVal ((5,4--5,12), (5,13--5,16))
                           WithKeyword = None
                           EqualsRange = Some (5,25--5,26)
                           GetSetKeywords = None });
                      AutoProperty
                        ([], false, Z,
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
                         PreXmlDoc ((6,4), FSharp.Compiler.Xml.XmlDocCollector),
                         None, Const (Int32 1, (6,26--6,27)), (6,4--6,27),
                         { LeadingKeyword =
                            DefaultVal ((6,4--6,11), (6,12--6,15))
                           WithKeyword = None
                           EqualsRange = Some (6,24--6,25)
                           GetSetKeywords = None })], (3,4--6,27)), [], None,
                  (2,5--6,27), { LeadingKeyword = Type (2,0--2,4)
                                 EqualsRange = Some (2,9--2,10)
                                 WithKeyword = None })], (2,0--6,27))],
          PreXmlDocEmpty, [], None, (2,0--7,0), { LeadingKeyword = None })],
      (true, true), { ConditionalDirectives = []
                      CodeComments = [] }, set []))
