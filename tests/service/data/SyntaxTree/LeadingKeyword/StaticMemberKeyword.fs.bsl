ImplFile
  (ParsedImplFileInput
     ("/root/LeadingKeyword/StaticMemberKeyword.fs", false,
      QualifiedNameOfFile StaticMemberKeyword, [], [],
      [SynModuleOrNamespace
         ([StaticMemberKeyword], false, AnonModule,
          [Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([], None, [], [X],
                     PreXmlDoc ((2,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (2,5--2,6)),
                  ObjectModel
                    (Unspecified,
                     [Member
                        (SynBinding
                           (None, Normal, false, false, [],
                            PreXmlDoc ((3,4), FSharp.Compiler.Xml.XmlDocCollector),
                            SynValData
                              (Some { IsInstance = false
                                      IsDispatchSlot = false
                                      IsOverrideOrExplicitImpl = false
                                      IsFinal = false
                                      GetterOrSetterIsCompilerGenerated = false
                                      MemberKind = Member },
                               SynValInfo ([[]], SynArgInfo ([], false, None)),
                               None),
                            LongIdent
                              (SynLongIdent ([Y], [], [None]), None, None,
                               Pats [], None, (3,18--3,19)),
                            Some
                              (SynBindingReturnInfo
                                 (LongIdent (SynLongIdent ([int], [], [None])),
                                  (3,22--3,25), [],
                                  { ColonRange = Some (3,20--3,21) })),
                            Typed
                              (Const (Int32 1, (3,28--3,29)),
                               LongIdent (SynLongIdent ([int], [], [None])),
                               (3,28--3,29)), (3,18--3,19), NoneAtInvisible,
                            { LeadingKeyword =
                               StaticMember ((3,4--3,10), (3,11--3,17))
                              InlineKeyword = None
                              EqualsRange = Some (3,26--3,27) }), (3,4--3,29))],
                     (3,4--3,29)), [], None, (2,5--3,29),
                  { LeadingKeyword = Type (2,0--2,4)
                    EqualsRange = Some (2,7--2,8)
                    WithKeyword = None })], (2,0--3,29))], PreXmlDocEmpty, [],
          None, (2,0--4,0), { LeadingKeyword = None })], (true, false),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))
