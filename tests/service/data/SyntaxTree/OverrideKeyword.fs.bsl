ImplFile
  (ParsedImplFileInput
     ("/root/OverrideKeyword.fs", false, QualifiedNameOfFile OverrideKeyword, [],
      [],
      [SynModuleOrNamespace
         ([OverrideKeyword], false, AnonModule,
          [Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([], None, [], [D],
                     PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, /root/OverrideKeyword.fs (1,5--1,6)),
                  ObjectModel
                    (Unspecified,
                     [Member
                        (SynBinding
                           (None, Normal, false, false, [],
                            PreXmlDoc ((2,4), FSharp.Compiler.Xml.XmlDocCollector),
                            SynValData
                              (Some { IsInstance = true
                                      IsDispatchSlot = false
                                      IsOverrideOrExplicitImpl = true
                                      IsFinal = false
                                      GetterOrSetterIsCompilerGenerated = false
                                      MemberKind = Member },
                               SynValInfo
                                 ([[SynArgInfo ([], false, None)]; []],
                                  SynArgInfo ([], false, None)), None),
                            LongIdent
                              (SynLongIdent ([E], [], [None]), None, None,
                               Pats [], None,
                               /root/OverrideKeyword.fs (2,13--2,14)),
                            Some
                              (SynBindingReturnInfo
                                 (LongIdent
                                    (SynLongIdent ([string], [], [None])),
                                  /root/OverrideKeyword.fs (2,17--2,23), [],
                                  { ColonRange =
                                     Some /root/OverrideKeyword.fs (2,15--2,16) })),
                            Typed
                              (Const
                                 (String
                                    ("", Regular,
                                     /root/OverrideKeyword.fs (2,26--2,28)),
                                  /root/OverrideKeyword.fs (2,26--2,28)),
                               LongIdent (SynLongIdent ([string], [], [None])),
                               /root/OverrideKeyword.fs (2,26--2,28)),
                            /root/OverrideKeyword.fs (2,13--2,14),
                            NoneAtInvisible,
                            { LeadingKeyword =
                               Override /root/OverrideKeyword.fs (2,4--2,12)
                              InlineKeyword = None
                              EqualsRange =
                               Some /root/OverrideKeyword.fs (2,24--2,25) }),
                         /root/OverrideKeyword.fs (2,4--2,28))],
                     /root/OverrideKeyword.fs (2,4--2,28)), [], None,
                  /root/OverrideKeyword.fs (1,5--2,28),
                  { LeadingKeyword = Type /root/OverrideKeyword.fs (1,0--1,4)
                    EqualsRange = Some /root/OverrideKeyword.fs (1,7--1,8)
                    WithKeyword = None })], /root/OverrideKeyword.fs (1,0--2,28))],
          PreXmlDocEmpty, [], None, /root/OverrideKeyword.fs (1,0--2,28),
          { LeadingKeyword = None })], (true, false),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))