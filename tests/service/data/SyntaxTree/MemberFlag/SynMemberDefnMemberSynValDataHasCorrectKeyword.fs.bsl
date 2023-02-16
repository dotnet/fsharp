ImplFile
  (ParsedImplFileInput
     ("/root/MemberFlag/SynMemberDefnMemberSynValDataHasCorrectKeyword.fs",
      false, QualifiedNameOfFile SynMemberDefnMemberSynValDataHasCorrectKeyword,
      [], [],
      [SynModuleOrNamespace
         ([SynMemberDefnMemberSynValDataHasCorrectKeyword], false, AnonModule,
          [Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([], None, [], [Foo],
                     PreXmlDoc ((2,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (2,5--2,8)),
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
                              (SynLongIdent
                                 ([this; B], [(3,22--3,23)], [None; None]), None,
                               None,
                               Pats
                                 [Paren
                                    (Const (Unit, (3,24--3,26)), (3,24--3,26))],
                               None, (3,18--3,26)), None,
                            Const (Unit, (3,29--3,31)), (3,18--3,26),
                            NoneAtInvisible,
                            { LeadingKeyword =
                               StaticMember ((3,4--3,10), (3,11--3,17))
                              InlineKeyword = None
                              EqualsRange = Some (3,27--3,28) }), (3,4--3,31));
                      Member
                        (SynBinding
                           (None, Normal, false, false, [],
                            PreXmlDoc ((4,4), FSharp.Compiler.Xml.XmlDocCollector),
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
                                 ([this; A], [(4,15--4,16)], [None; None]), None,
                               None,
                               Pats
                                 [Paren
                                    (Const (Unit, (4,17--4,19)), (4,17--4,19))],
                               None, (4,11--4,19)), None,
                            Const (Unit, (4,22--4,24)), (4,11--4,19),
                            NoneAtInvisible,
                            { LeadingKeyword = Member (4,4--4,10)
                              InlineKeyword = None
                              EqualsRange = Some (4,20--4,21) }), (4,4--4,24));
                      Member
                        (SynBinding
                           (None, Normal, false, false, [],
                            PreXmlDoc ((5,4), FSharp.Compiler.Xml.XmlDocCollector),
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
                                 ([this; C], [(5,17--5,18)], [None; None]), None,
                               None,
                               Pats
                                 [Paren
                                    (Const (Unit, (5,19--5,21)), (5,19--5,21))],
                               None, (5,13--5,21)), None,
                            Const (Unit, (5,24--5,26)), (5,13--5,21),
                            NoneAtInvisible,
                            { LeadingKeyword = Override (5,4--5,12)
                              InlineKeyword = None
                              EqualsRange = Some (5,22--5,23) }), (5,4--5,26));
                      Member
                        (SynBinding
                           (None, Normal, false, false, [],
                            PreXmlDoc ((6,4), FSharp.Compiler.Xml.XmlDocCollector),
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
                                 ([this; D], [(6,16--6,17)], [None; None]), None,
                               None,
                               Pats
                                 [Paren
                                    (Const (Unit, (6,18--6,20)), (6,18--6,20))],
                               None, (6,12--6,20)), None,
                            Const (Unit, (6,23--6,25)), (6,12--6,20),
                            NoneAtInvisible,
                            { LeadingKeyword = Default (6,4--6,11)
                              InlineKeyword = None
                              EqualsRange = Some (6,21--6,22) }), (6,4--6,25))],
                     (3,4--6,25)), [], None, (2,5--6,25),
                  { LeadingKeyword = Type (2,0--2,4)
                    EqualsRange = Some (2,9--2,10)
                    WithKeyword = None })], (2,0--6,25))], PreXmlDocEmpty, [],
          None, (2,0--7,0), { LeadingKeyword = None })], (true, false),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))
