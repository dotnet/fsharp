ImplFile
  (ParsedImplFileInput
     ("/root/MemberFlag/SynExprObjMembersHaveCorrectKeywords.fs", false,
      QualifiedNameOfFile SynExprObjMembersHaveCorrectKeywords, [], [],
      [SynModuleOrNamespace
         ([SynExprObjMembersHaveCorrectKeywords], false, AnonModule,
          [Let
             (false,
              [SynBinding
                 (None, Normal, false, false, [],
                  PreXmlDoc ((2,0), FSharp.Compiler.Xml.XmlDocCollector),
                  SynValData
                    (None, SynValInfo ([], SynArgInfo ([], false, None)), None),
                  Named
                    (SynIdent (meh, None), false, None,
                     /root/MemberFlag/SynExprObjMembersHaveCorrectKeywords.fs (2,4--2,7)),
                  None,
                  ObjExpr
                    (LongIdent (SynLongIdent ([Interface], [], [None])), None,
                     Some
                       /root/MemberFlag/SynExprObjMembersHaveCorrectKeywords.fs (3,20--3,24),
                     [],
                     [Member
                        (SynBinding
                           (None, Normal, false, false, [],
                            PreXmlDoc ((4,8), FSharp.Compiler.Xml.XmlDocCollector),
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
                              (SynLongIdent
                                 ([this; Foo],
                                  [/root/MemberFlag/SynExprObjMembersHaveCorrectKeywords.fs (4,21--4,22)],
                                  [None; None]), None, None,
                               Pats
                                 [Paren
                                    (Const
                                       (Unit,
                                        /root/MemberFlag/SynExprObjMembersHaveCorrectKeywords.fs (4,26--4,28)),
                                     /root/MemberFlag/SynExprObjMembersHaveCorrectKeywords.fs (4,26--4,28))],
                               None,
                               /root/MemberFlag/SynExprObjMembersHaveCorrectKeywords.fs (4,17--4,28)),
                            None,
                            Const
                              (Unit,
                               /root/MemberFlag/SynExprObjMembersHaveCorrectKeywords.fs (4,31--4,33)),
                            /root/MemberFlag/SynExprObjMembersHaveCorrectKeywords.fs (4,17--4,28),
                            NoneAtInvisible,
                            { LeadingKeyword =
                               Override
                                 /root/MemberFlag/SynExprObjMembersHaveCorrectKeywords.fs (4,8--4,16)
                              InlineKeyword = None
                              EqualsRange =
                               Some
                                 /root/MemberFlag/SynExprObjMembersHaveCorrectKeywords.fs (4,29--4,30) }),
                         /root/MemberFlag/SynExprObjMembersHaveCorrectKeywords.fs (4,8--4,33));
                      Member
                        (SynBinding
                           (None, Normal, false, false, [],
                            PreXmlDoc ((5,8), FSharp.Compiler.Xml.XmlDocCollector),
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
                              (SynLongIdent
                                 ([this; Bar],
                                  [/root/MemberFlag/SynExprObjMembersHaveCorrectKeywords.fs (5,19--5,20)],
                                  [None; None]), None, None,
                               Pats
                                 [Paren
                                    (Const
                                       (Unit,
                                        /root/MemberFlag/SynExprObjMembersHaveCorrectKeywords.fs (5,24--5,26)),
                                     /root/MemberFlag/SynExprObjMembersHaveCorrectKeywords.fs (5,24--5,26))],
                               None,
                               /root/MemberFlag/SynExprObjMembersHaveCorrectKeywords.fs (5,15--5,26)),
                            None,
                            Const
                              (Unit,
                               /root/MemberFlag/SynExprObjMembersHaveCorrectKeywords.fs (5,29--5,31)),
                            /root/MemberFlag/SynExprObjMembersHaveCorrectKeywords.fs (5,15--5,26),
                            NoneAtInvisible,
                            { LeadingKeyword =
                               Member
                                 /root/MemberFlag/SynExprObjMembersHaveCorrectKeywords.fs (5,8--5,14)
                              InlineKeyword = None
                              EqualsRange =
                               Some
                                 /root/MemberFlag/SynExprObjMembersHaveCorrectKeywords.fs (5,27--5,28) }),
                         /root/MemberFlag/SynExprObjMembersHaveCorrectKeywords.fs (5,8--5,31))],
                     [SynInterfaceImpl
                        (LongIdent (SynLongIdent ([SomethingElse], [], [None])),
                         Some
                           /root/MemberFlag/SynExprObjMembersHaveCorrectKeywords.fs (6,30--6,34),
                         [],
                         [Member
                            (SynBinding
                               (None, Normal, false, false, [],
                                PreXmlDoc ((7,8), FSharp.Compiler.Xml.XmlDocCollector),
                                SynValData
                                  (Some
                                     { IsInstance = true
                                       IsDispatchSlot = false
                                       IsOverrideOrExplicitImpl = true
                                       IsFinal = false
                                       GetterOrSetterIsCompilerGenerated = false
                                       MemberKind = Member },
                                   SynValInfo
                                     ([[SynArgInfo ([], false, None)]; []],
                                      SynArgInfo ([], false, None)), None),
                                LongIdent
                                  (SynLongIdent
                                     ([this; Blah],
                                      [/root/MemberFlag/SynExprObjMembersHaveCorrectKeywords.fs (7,19--7,20)],
                                      [None; None]), None, None,
                                   Pats
                                     [Paren
                                        (Const
                                           (Unit,
                                            /root/MemberFlag/SynExprObjMembersHaveCorrectKeywords.fs (7,25--7,27)),
                                         /root/MemberFlag/SynExprObjMembersHaveCorrectKeywords.fs (7,25--7,27))],
                                   None,
                                   /root/MemberFlag/SynExprObjMembersHaveCorrectKeywords.fs (7,15--7,27)),
                                None,
                                Const
                                  (Unit,
                                   /root/MemberFlag/SynExprObjMembersHaveCorrectKeywords.fs (7,30--7,32)),
                                /root/MemberFlag/SynExprObjMembersHaveCorrectKeywords.fs (7,15--7,27),
                                NoneAtInvisible,
                                { LeadingKeyword =
                                   Member
                                     /root/MemberFlag/SynExprObjMembersHaveCorrectKeywords.fs (7,8--7,14)
                                  InlineKeyword = None
                                  EqualsRange =
                                   Some
                                     /root/MemberFlag/SynExprObjMembersHaveCorrectKeywords.fs (7,28--7,29) }),
                             /root/MemberFlag/SynExprObjMembersHaveCorrectKeywords.fs (7,8--7,32))],
                         /root/MemberFlag/SynExprObjMembersHaveCorrectKeywords.fs (6,6--7,34))],
                     /root/MemberFlag/SynExprObjMembersHaveCorrectKeywords.fs (3,6--3,19),
                     /root/MemberFlag/SynExprObjMembersHaveCorrectKeywords.fs (3,4--7,34)),
                  /root/MemberFlag/SynExprObjMembersHaveCorrectKeywords.fs (2,4--2,7),
                  Yes
                    /root/MemberFlag/SynExprObjMembersHaveCorrectKeywords.fs (2,0--7,34),
                  { LeadingKeyword =
                     Let
                       /root/MemberFlag/SynExprObjMembersHaveCorrectKeywords.fs (2,0--2,3)
                    InlineKeyword = None
                    EqualsRange =
                     Some
                       /root/MemberFlag/SynExprObjMembersHaveCorrectKeywords.fs (2,8--2,9) })],
              /root/MemberFlag/SynExprObjMembersHaveCorrectKeywords.fs (2,0--7,34))],
          PreXmlDocEmpty, [], None,
          /root/MemberFlag/SynExprObjMembersHaveCorrectKeywords.fs (2,0--8,0),
          { LeadingKeyword = None })], (true, false),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))
