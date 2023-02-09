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
                     PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, /root/StaticMemberKeyword.fs (1,5--1,6)),
                  ObjectModel
                    (Unspecified,
                     [Member
                        (SynBinding
                           (None, Normal, false, false, [],
                            PreXmlDoc ((2,4), FSharp.Compiler.Xml.XmlDocCollector),
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
                               /root/StaticMemberKeyword.fs (2,18--2,19)),
                            Some
                              (SynBindingReturnInfo
                                 (LongIdent (SynLongIdent ([int], [], [None])),
                                  /root/StaticMemberKeyword.fs (2,22--2,25), [],
                                  { ColonRange =
                                     Some
                                       /root/StaticMemberKeyword.fs (2,20--2,21) })),
                            Typed
                              (Const
                                 (Int32 1,
                                  /root/StaticMemberKeyword.fs (2,28--2,29)),
                               LongIdent (SynLongIdent ([int], [], [None])),
                               /root/StaticMemberKeyword.fs (2,28--2,29)),
                            /root/StaticMemberKeyword.fs (2,18--2,19),
                            NoneAtInvisible,
                            { LeadingKeyword =
                               StaticMember
                                 (/root/StaticMemberKeyword.fs (2,4--2,10),
                                  /root/StaticMemberKeyword.fs (2,11--2,17))
                              InlineKeyword = None
                              EqualsRange =
                               Some /root/StaticMemberKeyword.fs (2,26--2,27) }),
                         /root/StaticMemberKeyword.fs (2,4--2,29))],
                     /root/StaticMemberKeyword.fs (2,4--2,29)), [], None,
                  /root/StaticMemberKeyword.fs (1,5--2,29),
                  { LeadingKeyword =
                     Type /root/StaticMemberKeyword.fs (1,0--1,4)
                    EqualsRange = Some /root/StaticMemberKeyword.fs (1,7--1,8)
                    WithKeyword = None })],
              /root/StaticMemberKeyword.fs (1,0--2,29))], PreXmlDocEmpty, [],
          None, /root/StaticMemberKeyword.fs (1,0--2,29),
          { LeadingKeyword = None })], (true, false),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))