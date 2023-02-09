ImplFile
  (ParsedImplFileInput
     ("/root/GetSetMemberWithInlineKeyword.fs", false,
      QualifiedNameOfFile GetSetMemberWithInlineKeyword, [], [],
      [SynModuleOrNamespace
         ([GetSetMemberWithInlineKeyword], false, AnonModule,
          [Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([], None, [], [X],
                     PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None,
                     /root/GetSetMemberWithInlineKeyword.fs (1,5--1,6)),
                  ObjectModel
                    (Unspecified,
                     [GetSetMember
                        (Some
                           (SynBinding
                              (None, Normal, true, false, [],
                               PreXmlMerge
  (PreXmlDoc ((2,4), FSharp.Compiler.Xml.XmlDocCollector), PreXmlDocEmpty),
                               SynValData
                                 (Some
                                    { IsInstance = true
                                      IsDispatchSlot = false
                                      IsOverrideOrExplicitImpl = false
                                      IsFinal = false
                                      GetterOrSetterIsCompilerGenerated = false
                                      MemberKind = PropertyGet },
                                  SynValInfo
                                    ([[SynArgInfo ([], false, None)]; []],
                                     SynArgInfo ([], false, None)), None),
                               LongIdent
                                 (SynLongIdent
                                    ([x; Y],
                                     [/root/GetSetMemberWithInlineKeyword.fs (2,19--2,20)],
                                     [None; None]), Some get, None,
                                  Pats
                                    [Paren
                                       (Const
                                          (Unit,
                                           /root/GetSetMemberWithInlineKeyword.fs (3,24--3,26)),
                                        /root/GetSetMemberWithInlineKeyword.fs (3,24--3,26))],
                                  None,
                                  /root/GetSetMemberWithInlineKeyword.fs (3,20--3,26)),
                               None,
                               Const
                                 (Int32 4,
                                  /root/GetSetMemberWithInlineKeyword.fs (3,29--3,30)),
                               /root/GetSetMemberWithInlineKeyword.fs (3,20--3,26),
                               NoneAtInvisible,
                               { LeadingKeyword =
                                  Member
                                    /root/GetSetMemberWithInlineKeyword.fs (2,4--2,10)
                                 InlineKeyword =
                                  Some
                                    /root/GetSetMemberWithInlineKeyword.fs (3,13--3,19)
                                 EqualsRange =
                                  Some
                                    /root/GetSetMemberWithInlineKeyword.fs (3,27--3,28) })),
                         Some
                           (SynBinding
                              (None, Normal, true, false, [],
                               PreXmlMerge
  (PreXmlDoc ((2,4), FSharp.Compiler.Xml.XmlDocCollector), PreXmlDocEmpty),
                               SynValData
                                 (Some
                                    { IsInstance = true
                                      IsDispatchSlot = false
                                      IsOverrideOrExplicitImpl = false
                                      IsFinal = false
                                      GetterOrSetterIsCompilerGenerated = false
                                      MemberKind = PropertySet },
                                  SynValInfo
                                    ([[SynArgInfo ([], false, None)];
                                      [SynArgInfo ([], false, Some y)]],
                                     SynArgInfo ([], false, None)), None),
                               LongIdent
                                 (SynLongIdent
                                    ([x; Y],
                                     [/root/GetSetMemberWithInlineKeyword.fs (2,19--2,20)],
                                     [None; None]), Some set, None,
                                  Pats
                                    [Named
                                       (SynIdent (y, None), false, None,
                                        /root/GetSetMemberWithInlineKeyword.fs (4,23--4,24))],
                                  None,
                                  /root/GetSetMemberWithInlineKeyword.fs (4,19--4,24)),
                               None,
                               Const
                                 (Unit,
                                  /root/GetSetMemberWithInlineKeyword.fs (4,27--4,29)),
                               /root/GetSetMemberWithInlineKeyword.fs (4,19--4,24),
                               NoneAtInvisible,
                               { LeadingKeyword =
                                  Member
                                    /root/GetSetMemberWithInlineKeyword.fs (2,4--2,10)
                                 InlineKeyword =
                                  Some
                                    /root/GetSetMemberWithInlineKeyword.fs (4,12--4,18)
                                 EqualsRange =
                                  Some
                                    /root/GetSetMemberWithInlineKeyword.fs (4,25--4,26) })),
                         /root/GetSetMemberWithInlineKeyword.fs (2,4--4,29),
                         { InlineKeyword =
                            Some
                              /root/GetSetMemberWithInlineKeyword.fs (2,11--2,17)
                           WithKeyword =
                            /root/GetSetMemberWithInlineKeyword.fs (3,8--3,12)
                           GetKeyword =
                            Some
                              /root/GetSetMemberWithInlineKeyword.fs (3,20--3,23)
                           AndKeyword =
                            Some
                              /root/GetSetMemberWithInlineKeyword.fs (4,8--4,11)
                           SetKeyword =
                            Some
                              /root/GetSetMemberWithInlineKeyword.fs (4,19--4,22) })],
                     /root/GetSetMemberWithInlineKeyword.fs (2,4--4,29)), [],
                  None, /root/GetSetMemberWithInlineKeyword.fs (1,5--4,29),
                  { LeadingKeyword =
                     Type /root/GetSetMemberWithInlineKeyword.fs (1,0--1,4)
                    EqualsRange =
                     Some /root/GetSetMemberWithInlineKeyword.fs (1,7--1,8)
                    WithKeyword = None })],
              /root/GetSetMemberWithInlineKeyword.fs (1,0--4,29))],
          PreXmlDocEmpty, [], None,
          /root/GetSetMemberWithInlineKeyword.fs (1,0--4,29),
          { LeadingKeyword = None })], (true, false),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))