ImplFile
  (ParsedImplFileInput
     ("/root/MemberWithInlineKeyword.fs", false,
      QualifiedNameOfFile MemberWithInlineKeyword, [], [],
      [SynModuleOrNamespace
         ([MemberWithInlineKeyword], false, AnonModule,
          [Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([], None, [], [X],
                     PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, /root/MemberWithInlineKeyword.fs (1,5--1,6)),
                  ObjectModel
                    (Unspecified,
                     [Member
                        (SynBinding
                           (None, Normal, true, false, [],
                            PreXmlDoc ((2,4), FSharp.Compiler.Xml.XmlDocCollector),
                            SynValData
                              (Some { IsInstance = true
                                      IsDispatchSlot = false
                                      IsOverrideOrExplicitImpl = false
                                      IsFinal = false
                                      GetterOrSetterIsCompilerGenerated = false
                                      MemberKind = Member },
                               SynValInfo
                                 ([[SynArgInfo ([], false, None)]; []],
                                  SynArgInfo ([], false, None)), None),
                            LongIdent
                              (SynLongIdent
                                 ([x; Y],
                                  [/root/MemberWithInlineKeyword.fs (2,19--2,20)],
                                  [None; None]), None, None,
                               Pats
                                 [Paren
                                    (Const
                                       (Unit,
                                        /root/MemberWithInlineKeyword.fs (2,22--2,24)),
                                     /root/MemberWithInlineKeyword.fs (2,22--2,24))],
                               None,
                               /root/MemberWithInlineKeyword.fs (2,18--2,24)),
                            None,
                            Const
                              (Unit,
                               /root/MemberWithInlineKeyword.fs (2,27--2,29)),
                            /root/MemberWithInlineKeyword.fs (2,18--2,24),
                            NoneAtInvisible,
                            { LeadingKeyword =
                               Member
                                 /root/MemberWithInlineKeyword.fs (2,4--2,10)
                              InlineKeyword =
                               Some
                                 /root/MemberWithInlineKeyword.fs (2,11--2,17)
                              EqualsRange =
                               Some
                                 /root/MemberWithInlineKeyword.fs (2,25--2,26) }),
                         /root/MemberWithInlineKeyword.fs (2,4--2,29))],
                     /root/MemberWithInlineKeyword.fs (2,4--2,29)), [], None,
                  /root/MemberWithInlineKeyword.fs (1,5--2,29),
                  { LeadingKeyword =
                     Type /root/MemberWithInlineKeyword.fs (1,0--1,4)
                    EqualsRange =
                     Some /root/MemberWithInlineKeyword.fs (1,7--1,8)
                    WithKeyword = None })],
              /root/MemberWithInlineKeyword.fs (1,0--2,29))], PreXmlDocEmpty, [],
          None, /root/MemberWithInlineKeyword.fs (1,0--2,29),
          { LeadingKeyword = None })], (true, false),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))