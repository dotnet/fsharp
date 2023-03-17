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
                  Named (SynIdent (meh, None), false, None, (2,4--2,7)), None,
                  ObjExpr
                    (LongIdent (SynLongIdent ([Interface], [], [None])), None,
                     Some (3,20--3,24), [],
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
                                 ([this; Foo], [(4,21--4,22)], [None; None]),
                               None, None,
                               Pats
                                 [Paren
                                    (Const (Unit, (4,26--4,28)), (4,26--4,28))],
                               None, (4,17--4,28)), None,
                            Const (Unit, (4,31--4,33)), (4,17--4,28),
                            NoneAtInvisible,
                            { LeadingKeyword = Override (4,8--4,16)
                              InlineKeyword = None
                              EqualsRange = Some (4,29--4,30) }), (4,8--4,33));
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
                                 ([this; Bar], [(5,19--5,20)], [None; None]),
                               None, None,
                               Pats
                                 [Paren
                                    (Const (Unit, (5,24--5,26)), (5,24--5,26))],
                               None, (5,15--5,26)), None,
                            Const (Unit, (5,29--5,31)), (5,15--5,26),
                            NoneAtInvisible,
                            { LeadingKeyword = Member (5,8--5,14)
                              InlineKeyword = None
                              EqualsRange = Some (5,27--5,28) }), (5,8--5,31))],
                     [SynInterfaceImpl
                        (LongIdent (SynLongIdent ([SomethingElse], [], [None])),
                         Some (6,30--6,34), [],
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
                                     ([this; Blah], [(7,19--7,20)], [None; None]),
                                   None, None,
                                   Pats
                                     [Paren
                                        (Const (Unit, (7,25--7,27)),
                                         (7,25--7,27))], None, (7,15--7,27)),
                                None, Const (Unit, (7,30--7,32)), (7,15--7,27),
                                NoneAtInvisible,
                                { LeadingKeyword = Member (7,8--7,14)
                                  InlineKeyword = None
                                  EqualsRange = Some (7,28--7,29) }),
                             (7,8--7,32))], (6,6--7,34))], (3,6--3,19),
                     (3,4--7,34)), (2,4--2,7), Yes (2,0--7,34),
                  { LeadingKeyword = Let (2,0--2,3)
                    InlineKeyword = None
                    EqualsRange = Some (2,8--2,9) })], (2,0--7,34))],
          PreXmlDocEmpty, [], None, (2,0--8,0), { LeadingKeyword = None })],
      (true, false), { ConditionalDirectives = []
                       CodeComments = [] }, set []))
