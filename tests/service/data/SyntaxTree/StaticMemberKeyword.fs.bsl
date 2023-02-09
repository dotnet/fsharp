ImplFile
  (ParsedImplFileInput
     ("/root/StaticMemberKeyword.fs", false,
      QualifiedNameOfFile StaticMemberKeyword, [], [],
      [SynModuleOrNamespace
         ([StaticMemberKeyword], false, AnonModule,
          [Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([], None, [], [X],
                     PreXmlDoc ((2,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, /root/StaticMemberKeyword.fs (2,5--2,6)),
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
                               Pats [], None,
                               /root/StaticMemberKeyword.fs (3,18--3,19)),
                            Some
                              (SynBindingReturnInfo
                                 (LongIdent (SynLongIdent ([int], [], [None])),
                                  /root/StaticMemberKeyword.fs (3,22--3,25), [],
                                  { ColonRange =
                                     Some
                                       /root/StaticMemberKeyword.fs (3,20--3,21) })),
                            Typed
                              (Const
                                 (Int32 1,
                                  /root/StaticMemberKeyword.fs (3,28--3,29)),
                               LongIdent (SynLongIdent ([int], [], [None])),
                               /root/StaticMemberKeyword.fs (3,28--3,29)),
                            /root/StaticMemberKeyword.fs (3,18--3,19),
                            NoneAtInvisible,
                            { LeadingKeyword =
                               StaticMember
                                 (/root/StaticMemberKeyword.fs (3,4--3,10),
                                  /root/StaticMemberKeyword.fs (3,11--3,17))
                              InlineKeyword = None
                              EqualsRange =
                               Some /root/StaticMemberKeyword.fs (3,26--3,27) }),
                         /root/StaticMemberKeyword.fs (3,4--3,29))],
                     /root/StaticMemberKeyword.fs (3,4--3,29)), [], None,
                  /root/StaticMemberKeyword.fs (2,5--3,29),
                  { LeadingKeyword =
                     Type /root/StaticMemberKeyword.fs (2,0--2,4)
                    EqualsRange = Some /root/StaticMemberKeyword.fs (2,7--2,8)
                    WithKeyword = None })],
              /root/StaticMemberKeyword.fs (2,0--3,29))], PreXmlDocEmpty, [],
          None, /root/StaticMemberKeyword.fs (2,0--4,0),
          { LeadingKeyword = None })], (true, false),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))