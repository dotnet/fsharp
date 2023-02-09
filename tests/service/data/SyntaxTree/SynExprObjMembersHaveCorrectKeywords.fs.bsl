ImplFile
  (ParsedImplFileInput
     ("/root/SynExprObjMembersHaveCorrectKeywords.fs", false,
      QualifiedNameOfFile SynExprObjMembersHaveCorrectKeywords, [], [],
      [SynModuleOrNamespace
         ([SynExprObjMembersHaveCorrectKeywords], false, AnonModule,
          [Let
             (false,
              [SynBinding
                 (None, Normal, false, false, [],
                  PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector),
                  SynValData
                    (None, SynValInfo ([], SynArgInfo ([], false, None)), None),
                  Named
                    (SynIdent (meh, None), false, None,
                     /root/SynExprObjMembersHaveCorrectKeywords.fs (1,4--1,7)),
                  None,
                  ObjExpr
                    (LongIdent (SynLongIdent ([Interface], [], [None])), None,
                     Some
                       /root/SynExprObjMembersHaveCorrectKeywords.fs (2,20--2,24),
                     [],
                     [Member
                        (SynBinding
                           (None, Normal, false, false, [],
                            PreXmlDoc ((3,8), FSharp.Compiler.Xml.XmlDocCollector),
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
                                  [/root/SynExprObjMembersHaveCorrectKeywords.fs (3,21--3,22)],
                                  [None; None]), None, None,
                               Pats
                                 [Paren
                                    (Const
                                       (Unit,
                                        /root/SynExprObjMembersHaveCorrectKeywords.fs (3,26--3,28)),
                                     /root/SynExprObjMembersHaveCorrectKeywords.fs (3,26--3,28))],
                               None,
                               /root/SynExprObjMembersHaveCorrectKeywords.fs (3,17--3,28)),
                            None,
                            Const
                              (Unit,
                               /root/SynExprObjMembersHaveCorrectKeywords.fs (3,31--3,33)),
                            /root/SynExprObjMembersHaveCorrectKeywords.fs (3,17--3,28),
                            NoneAtInvisible,
                            { LeadingKeyword =
                               Override
                                 /root/SynExprObjMembersHaveCorrectKeywords.fs (3,8--3,16)
                              InlineKeyword = None
                              EqualsRange =
                               Some
                                 /root/SynExprObjMembersHaveCorrectKeywords.fs (3,29--3,30) }),
                         /root/SynExprObjMembersHaveCorrectKeywords.fs (3,8--3,33));
                      Member
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
                                 ([this; Bar],
                                  [/root/SynExprObjMembersHaveCorrectKeywords.fs (4,19--4,20)],
                                  [None; None]), None, None,
                               Pats
                                 [Paren
                                    (Const
                                       (Unit,
                                        /root/SynExprObjMembersHaveCorrectKeywords.fs (4,24--4,26)),
                                     /root/SynExprObjMembersHaveCorrectKeywords.fs (4,24--4,26))],
                               None,
                               /root/SynExprObjMembersHaveCorrectKeywords.fs (4,15--4,26)),
                            None,
                            Const
                              (Unit,
                               /root/SynExprObjMembersHaveCorrectKeywords.fs (4,29--4,31)),
                            /root/SynExprObjMembersHaveCorrectKeywords.fs (4,15--4,26),
                            NoneAtInvisible,
                            { LeadingKeyword =
                               Member
                                 /root/SynExprObjMembersHaveCorrectKeywords.fs (4,8--4,14)
                              InlineKeyword = None
                              EqualsRange =
                               Some
                                 /root/SynExprObjMembersHaveCorrectKeywords.fs (4,27--4,28) }),
                         /root/SynExprObjMembersHaveCorrectKeywords.fs (4,8--4,31))],
                     [SynInterfaceImpl
                        (LongIdent (SynLongIdent ([SomethingElse], [], [None])),
                         Some
                           /root/SynExprObjMembersHaveCorrectKeywords.fs (5,30--5,34),
                         [],
                         [Member
                            (SynBinding
                               (None, Normal, false, false, [],
                                PreXmlDoc ((6,8), FSharp.Compiler.Xml.XmlDocCollector),
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
                                      [/root/SynExprObjMembersHaveCorrectKeywords.fs (6,19--6,20)],
                                      [None; None]), None, None,
                                   Pats
                                     [Paren
                                        (Const
                                           (Unit,
                                            /root/SynExprObjMembersHaveCorrectKeywords.fs (6,25--6,27)),
                                         /root/SynExprObjMembersHaveCorrectKeywords.fs (6,25--6,27))],
                                   None,
                                   /root/SynExprObjMembersHaveCorrectKeywords.fs (6,15--6,27)),
                                None,
                                Const
                                  (Unit,
                                   /root/SynExprObjMembersHaveCorrectKeywords.fs (6,30--6,32)),
                                /root/SynExprObjMembersHaveCorrectKeywords.fs (6,15--6,27),
                                NoneAtInvisible,
                                { LeadingKeyword =
                                   Member
                                     /root/SynExprObjMembersHaveCorrectKeywords.fs (6,8--6,14)
                                  InlineKeyword = None
                                  EqualsRange =
                                   Some
                                     /root/SynExprObjMembersHaveCorrectKeywords.fs (6,28--6,29) }),
                             /root/SynExprObjMembersHaveCorrectKeywords.fs (6,8--6,32))],
                         /root/SynExprObjMembersHaveCorrectKeywords.fs (5,6--6,34))],
                     /root/SynExprObjMembersHaveCorrectKeywords.fs (2,6--2,19),
                     /root/SynExprObjMembersHaveCorrectKeywords.fs (2,4--6,34)),
                  /root/SynExprObjMembersHaveCorrectKeywords.fs (1,4--1,7),
                  Yes /root/SynExprObjMembersHaveCorrectKeywords.fs (1,0--6,34),
                  { LeadingKeyword =
                     Let
                       /root/SynExprObjMembersHaveCorrectKeywords.fs (1,0--1,3)
                    InlineKeyword = None
                    EqualsRange =
                     Some
                       /root/SynExprObjMembersHaveCorrectKeywords.fs (1,8--1,9) })],
              /root/SynExprObjMembersHaveCorrectKeywords.fs (1,0--6,34))],
          PreXmlDocEmpty, [], None,
          /root/SynExprObjMembersHaveCorrectKeywords.fs (1,0--6,34),
          { LeadingKeyword = None })], (true, false),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))