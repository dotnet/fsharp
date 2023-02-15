ImplFile
  (ParsedImplFileInput
     ("/root/LeadingKeyword/StaticMemberValKeyword.fs", false,
      QualifiedNameOfFile StaticMemberValKeyword, [], [],
      [SynModuleOrNamespace
         ([StaticMemberValKeyword], false, AnonModule,
          [Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([], None, [], [X],
                     PreXmlDoc ((2,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (2,5--2,6)),
                  ObjectModel
                    (Unspecified,
                     [AutoProperty
                        ([], true, Y,
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
                           GetSetKeywords = None })], (3,4--3,33)), [], None,
                  (2,5--3,33), { LeadingKeyword = Type (2,0--2,4)
                                 EqualsRange = Some (2,7--2,8)
                                 WithKeyword = None })], (2,0--3,33))],
          PreXmlDocEmpty, [], None, (2,0--4,0), { LeadingKeyword = None })],
      (true, false), { ConditionalDirectives = []
                       CodeComments = [] }, set []))
