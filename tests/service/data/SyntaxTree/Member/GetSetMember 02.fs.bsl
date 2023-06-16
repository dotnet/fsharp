ImplFile
  (ParsedImplFileInput
     ("/root/Member/GetSetMember 02.fs", false, QualifiedNameOfFile Foo, [], [],
      [SynModuleOrNamespace
         ([Foo], false, NamedModule,
          [Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([], None, [], [Foo],
                     PreXmlDoc ((3,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (3,5--3,8)),
                  ObjectModel
                    (Unspecified,
                     [ImplicitCtor
                        (None, [], SimplePats ([], [], (3,8--3,10)), None,
                         PreXmlDoc ((3,8), FSharp.Compiler.Xml.XmlDocCollector),
                         (3,5--3,8), { AsKeyword = None });
                      GetSetMember
                        (Some
                           (SynBinding
                              (None, Normal, false, false, [],
                               PreXmlMerge
  (PreXmlDoc ((4,4), FSharp.Compiler.Xml.XmlDocCollector), PreXmlDocEmpty),
                               SynValData
                                 (Some
                                    { IsInstance = true
                                      IsDispatchSlot = false
                                      IsOverrideOrExplicitImpl = false
                                      IsFinal = false
                                      GetterOrSetterIsCompilerGenerated = false
                                      MemberKind = PropertyGet },
                                  SynValInfo
                                    ([[SynArgInfo ([], false, None)];
                                      [SynArgInfo ([], false, Some key1);
                                       SynArgInfo ([], false, Some key2)]],
                                     SynArgInfo ([], false, None)), None),
                               LongIdent
                                 (SynLongIdent
                                    ([f; W], [(4,12--4,13)], [None; None]),
                                  Some get, None,
                                  Pats
                                    [Paren
                                       (Tuple
                                          (false,
                                           [Named
                                              (SynIdent (key1, None), false,
                                               None, (4,65--4,69));
                                            Named
                                              (SynIdent (key2, None), false,
                                               None, (4,71--4,75))],
                                           [(4,69--4,70)], (4,65--4,75)),
                                        (4,64--4,76))], None, (4,60--4,76)),
                               None, Const (Bool true, (4,79--4,83)),
                               (4,60--4,76), NoneAtInvisible,
                               { LeadingKeyword = Member (4,4--4,10)
                                 InlineKeyword = None
                                 EqualsRange = Some (4,77--4,78) })),
                         Some
                           (SynBinding
                              (None, Normal, false, false, [],
                               PreXmlMerge
  (PreXmlDoc ((4,4), FSharp.Compiler.Xml.XmlDocCollector), PreXmlDocEmpty),
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
                                      [SynArgInfo ([], false, Some key1);
                                       SynArgInfo ([], false, Some key2);
                                       SynArgInfo ([], false, Some value)]],
                                     SynArgInfo ([], false, None)), None),
                               LongIdent
                                 (SynLongIdent
                                    ([f; W], [(4,12--4,13)], [None; None]),
                                  Some set, None,
                                  Pats
                                    [Tuple
                                       (false,
                                        [Named
                                           (SynIdent (key1, None), false, None,
                                            (4,33--4,37));
                                         Named
                                           (SynIdent (key2, None), false, None,
                                            (4,39--4,43));
                                         Named
                                           (SynIdent (value, None), false, None,
                                            (4,45--4,50))], [(4,37--4,38)],
                                        (4,32--4,50))],
                                  Some (Private (4,20--4,27)), (4,20--4,50)),
                               None, Const (Unit, (4,53--4,55)), (4,20--4,50),
                               NoneAtInvisible,
                               { LeadingKeyword = Member (4,4--4,10)
                                 InlineKeyword = None
                                 EqualsRange = Some (4,51--4,52) })),
                         (4,4--4,83), { InlineKeyword = None
                                        WithKeyword = (4,15--4,19)
                                        GetKeyword = Some (4,60--4,63)
                                        AndKeyword = Some (4,56--4,59)
                                        SetKeyword = Some (4,28--4,31) });
                      GetSetMember
                        (Some
                           (SynBinding
                              (None, Normal, false, false, [],
                               PreXmlMerge
  (PreXmlDoc ((5,4), FSharp.Compiler.Xml.XmlDocCollector), PreXmlDocEmpty),
                               SynValData
                                 (Some
                                    { IsInstance = true
                                      IsDispatchSlot = false
                                      IsOverrideOrExplicitImpl = false
                                      IsFinal = false
                                      GetterOrSetterIsCompilerGenerated = false
                                      MemberKind = PropertyGet },
                                  SynValInfo
                                    ([[SynArgInfo ([], false, None)];
                                      [SynArgInfo ([], false, Some key1);
                                       SynArgInfo ([], false, Some key2)]],
                                     SynArgInfo ([], false, None)), None),
                               LongIdent
                                 (SynLongIdent
                                    ([f; X], [(5,12--5,13)], [None; None]),
                                  Some get, None,
                                  Pats
                                    [Paren
                                       (Tuple
                                          (false,
                                           [Named
                                              (SynIdent (key1, None), false,
                                               None, (5,25--5,29));
                                            Named
                                              (SynIdent (key2, None), false,
                                               None, (5,31--5,35))],
                                           [(5,29--5,30)], (5,25--5,35)),
                                        (5,24--5,36))], None, (5,20--5,36)),
                               None, Const (Bool true, (5,39--5,43)),
                               (5,20--5,36), NoneAtInvisible,
                               { LeadingKeyword = Member (5,4--5,10)
                                 InlineKeyword = None
                                 EqualsRange = Some (5,37--5,38) })),
                         Some
                           (SynBinding
                              (None, Normal, false, false, [],
                               PreXmlMerge
  (PreXmlDoc ((5,4), FSharp.Compiler.Xml.XmlDocCollector), PreXmlDocEmpty),
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
                                      [SynArgInfo ([], false, Some key1);
                                       SynArgInfo ([], false, Some key2);
                                       SynArgInfo ([], false, Some value)]],
                                     SynArgInfo ([], false, None)), None),
                               LongIdent
                                 (SynLongIdent
                                    ([f; X], [(5,12--5,13)], [None; None]),
                                  Some set, None,
                                  Pats
                                    [Tuple
                                       (false,
                                        [Named
                                           (SynIdent (key1, None), false, None,
                                            (5,61--5,65));
                                         Named
                                           (SynIdent (key2, None), false, None,
                                            (5,67--5,71));
                                         Named
                                           (SynIdent (value, None), false, None,
                                            (5,73--5,78))], [(5,65--5,66)],
                                        (5,60--5,78))],
                                  Some (Private (5,48--5,55)), (5,48--5,78)),
                               None, Const (Unit, (5,81--5,83)), (5,48--5,78),
                               NoneAtInvisible,
                               { LeadingKeyword = Member (5,4--5,10)
                                 InlineKeyword = None
                                 EqualsRange = Some (5,79--5,80) })),
                         (5,4--5,83), { InlineKeyword = None
                                        WithKeyword = (5,15--5,19)
                                        GetKeyword = Some (5,20--5,23)
                                        AndKeyword = Some (5,44--5,47)
                                        SetKeyword = Some (5,56--5,59) });
                      GetSetMember
                        (Some
                           (SynBinding
                              (None, Normal, false, false, [],
                               PreXmlMerge
  (PreXmlDoc ((6,4), FSharp.Compiler.Xml.XmlDocCollector), PreXmlDocEmpty),
                               SynValData
                                 (Some
                                    { IsInstance = true
                                      IsDispatchSlot = false
                                      IsOverrideOrExplicitImpl = false
                                      IsFinal = false
                                      GetterOrSetterIsCompilerGenerated = false
                                      MemberKind = PropertyGet },
                                  SynValInfo
                                    ([[SynArgInfo ([], false, None)];
                                      [SynArgInfo ([], false, Some key1);
                                       SynArgInfo ([], false, Some key2)]],
                                     SynArgInfo ([], false, None)), None),
                               LongIdent
                                 (SynLongIdent
                                    ([f; Y], [(6,12--6,13)], [None; None]),
                                  Some get, None,
                                  Pats
                                    [Paren
                                       (Tuple
                                          (false,
                                           [Named
                                              (SynIdent (key1, None), false,
                                               None, (6,33--6,37));
                                            Named
                                              (SynIdent (key2, None), false,
                                               None, (6,39--6,43))],
                                           [(6,37--6,38)], (6,33--6,43)),
                                        (6,32--6,44))],
                                  Some (Private (6,20--6,27)), (6,20--6,44)),
                               None, Const (Bool true, (6,47--6,51)),
                               (6,20--6,44), NoneAtInvisible,
                               { LeadingKeyword = Member (6,4--6,10)
                                 InlineKeyword = None
                                 EqualsRange = Some (6,45--6,46) })),
                         Some
                           (SynBinding
                              (None, Normal, false, false, [],
                               PreXmlMerge
  (PreXmlDoc ((6,4), FSharp.Compiler.Xml.XmlDocCollector), PreXmlDocEmpty),
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
                                      [SynArgInfo ([], false, Some key1);
                                       SynArgInfo ([], false, Some key2);
                                       SynArgInfo ([], false, Some value)]],
                                     SynArgInfo ([], false, None)), None),
                               LongIdent
                                 (SynLongIdent
                                    ([f; Y], [(6,12--6,13)], [None; None]),
                                  Some set, None,
                                  Pats
                                    [Tuple
                                       (false,
                                        [Named
                                           (SynIdent (key1, None), false, None,
                                            (6,68--6,72));
                                         Named
                                           (SynIdent (key2, None), false, None,
                                            (6,74--6,78));
                                         Named
                                           (SynIdent (value, None), false, None,
                                            (6,80--6,85))], [(6,72--6,73)],
                                        (6,67--6,85))],
                                  Some (Public (6,56--6,62)), (6,56--6,85)),
                               None, Const (Unit, (6,88--6,90)), (6,56--6,85),
                               NoneAtInvisible,
                               { LeadingKeyword = Member (6,4--6,10)
                                 InlineKeyword = None
                                 EqualsRange = Some (6,86--6,87) })),
                         (6,4--6,90), { InlineKeyword = None
                                        WithKeyword = (6,15--6,19)
                                        GetKeyword = Some (6,28--6,31)
                                        AndKeyword = Some (6,52--6,55)
                                        SetKeyword = Some (6,63--6,66) });
                      GetSetMember
                        (Some
                           (SynBinding
                              (None, Normal, false, false, [],
                               PreXmlMerge
  (PreXmlDoc ((7,4), FSharp.Compiler.Xml.XmlDocCollector), PreXmlDocEmpty),
                               SynValData
                                 (Some
                                    { IsInstance = true
                                      IsDispatchSlot = false
                                      IsOverrideOrExplicitImpl = false
                                      IsFinal = false
                                      GetterOrSetterIsCompilerGenerated = false
                                      MemberKind = PropertyGet },
                                  SynValInfo
                                    ([[SynArgInfo ([], false, None)];
                                      [SynArgInfo ([], false, Some key1);
                                       SynArgInfo ([], false, Some key2)]],
                                     SynArgInfo ([], false, None)), None),
                               LongIdent
                                 (SynLongIdent
                                    ([f; Z], [(7,12--7,13)], [None; None]),
                                  Some get, None,
                                  Pats
                                    [Paren
                                       (Tuple
                                          (false,
                                           [Named
                                              (SynIdent (key1, None), false,
                                               None, (7,73--7,77));
                                            Named
                                              (SynIdent (key2, None), false,
                                               None, (7,79--7,83))],
                                           [(7,77--7,78)], (7,73--7,83)),
                                        (7,72--7,84))],
                                  Some (Internal (7,59--7,67)), (7,59--7,84)),
                               None, Const (Bool true, (7,87--7,91)),
                               (7,59--7,84), NoneAtInvisible,
                               { LeadingKeyword = Member (7,4--7,10)
                                 InlineKeyword = None
                                 EqualsRange = Some (7,85--7,86) })),
                         Some
                           (SynBinding
                              (None, Normal, false, false, [],
                               PreXmlMerge
  (PreXmlDoc ((7,4), FSharp.Compiler.Xml.XmlDocCollector), PreXmlDocEmpty),
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
                                      [SynArgInfo ([], false, Some key1);
                                       SynArgInfo ([], false, Some key2);
                                       SynArgInfo ([], false, Some value)]],
                                     SynArgInfo ([], false, None)), None),
                               LongIdent
                                 (SynLongIdent
                                    ([f; Z], [(7,12--7,13)], [None; None]),
                                  Some set, None,
                                  Pats
                                    [Tuple
                                       (false,
                                        [Named
                                           (SynIdent (key1, None), false, None,
                                            (7,32--7,36));
                                         Named
                                           (SynIdent (key2, None), false, None,
                                            (7,38--7,42));
                                         Named
                                           (SynIdent (value, None), false, None,
                                            (7,44--7,49))], [(7,36--7,37)],
                                        (7,31--7,49))],
                                  Some (Public (7,20--7,26)), (7,20--7,49)),
                               None, Const (Unit, (7,52--7,54)), (7,20--7,49),
                               NoneAtInvisible,
                               { LeadingKeyword = Member (7,4--7,10)
                                 InlineKeyword = None
                                 EqualsRange = Some (7,50--7,51) })),
                         (7,4--7,91), { InlineKeyword = None
                                        WithKeyword = (7,15--7,19)
                                        GetKeyword = Some (7,68--7,71)
                                        AndKeyword = Some (7,55--7,58)
                                        SetKeyword = Some (7,27--7,30) })],
                     (4,4--7,91)), [],
                  Some
                    (ImplicitCtor
                       (None, [], SimplePats ([], [], (3,8--3,10)), None,
                        PreXmlDoc ((3,8), FSharp.Compiler.Xml.XmlDocCollector),
                        (3,5--3,8), { AsKeyword = None })), (3,5--7,91),
                  { LeadingKeyword = Type (3,0--3,4)
                    EqualsRange = Some (3,11--3,12)
                    WithKeyword = None })], (3,0--7,91))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--7,91), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))

(4,20)-(4,50) parse error Multiple accessibilities given for property getter or setter
(5,48)-(5,78) parse error Multiple accessibilities given for property getter or setter
(6,56)-(6,85) parse error Multiple accessibilities given for property getter or setter
(7,20)-(7,49) parse error Multiple accessibilities given for property getter or setter
