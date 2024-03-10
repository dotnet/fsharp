ImplFile
  (ParsedImplFileInput
     ("/root/Member/GetSetMember 01.fs", false, QualifiedNameOfFile Foo, [], [],
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
                        (None, [], Const (Unit, (3,8--3,10)), None,
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
                                    ([f; X], [(4,12--4,13)], [None; None]),
                                  Some get, None,
                                  Pats
                                    [Paren
                                       (Tuple
                                          (false,
                                           [Named
                                              (SynIdent (key1, None), false,
                                               None, (4,34--4,38));
                                            Named
                                              (SynIdent (key2, None), false,
                                               None, (4,40--4,44))],
                                           [(4,38--4,39)], (4,34--4,44)),
                                        (4,33--4,45))],
                                  Some (Internal (4,20--4,28)), (4,20--4,45)),
                               None, Const (Bool true, (4,48--4,52)),
                               (4,20--4,45), NoneAtInvisible,
                               { LeadingKeyword = Member (4,4--4,10)
                                 InlineKeyword = None
                                 EqualsRange = Some (4,46--4,47) })),
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
                                    ([f; X], [(4,12--4,13)], [None; None]),
                                  Some set, None,
                                  Pats
                                    [Tuple
                                       (false,
                                        [Named
                                           (SynIdent (key1, None), false, None,
                                            (4,70--4,74));
                                         Named
                                           (SynIdent (key2, None), false, None,
                                            (4,76--4,80));
                                         Named
                                           (SynIdent (value, None), false, None,
                                            (4,82--4,87))], [(4,74--4,75)],
                                        (4,69--4,87))],
                                  Some (Private (4,57--4,64)), (4,57--4,87)),
                               None, Const (Unit, (4,90--4,92)), (4,57--4,87),
                               NoneAtInvisible,
                               { LeadingKeyword = Member (4,4--4,10)
                                 InlineKeyword = None
                                 EqualsRange = Some (4,88--4,89) })),
                         (4,4--4,92), { InlineKeyword = None
                                        WithKeyword = (4,15--4,19)
                                        GetKeyword = Some (4,29--4,32)
                                        AndKeyword = Some (4,53--4,56)
                                        SetKeyword = Some (4,65--4,68) });
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
                                    ([f; Y], [(5,21--5,22)], [None; None]),
                                  Some get, None,
                                  Pats
                                    [Paren
                                       (Tuple
                                          (false,
                                           [Named
                                              (SynIdent (key1, None), false,
                                               None, (5,34--5,38));
                                            Named
                                              (SynIdent (key2, None), false,
                                               None, (5,40--5,44))],
                                           [(5,38--5,39)], (5,34--5,44)),
                                        (5,33--5,45))],
                                  Some (Internal (5,11--5,19)), (5,29--5,45)),
                               None, Const (Bool true, (5,48--5,52)),
                               (5,29--5,45), NoneAtInvisible,
                               { LeadingKeyword = Member (5,4--5,10)
                                 InlineKeyword = None
                                 EqualsRange = Some (5,46--5,47) })),
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
                                    ([f; Y], [(5,21--5,22)], [None; None]),
                                  Some set, None,
                                  Pats
                                    [Tuple
                                       (false,
                                        [Named
                                           (SynIdent (key1, None), false, None,
                                            (5,70--5,74));
                                         Named
                                           (SynIdent (key2, None), false, None,
                                            (5,76--5,80));
                                         Named
                                           (SynIdent (value, None), false, None,
                                            (5,82--5,87))], [(5,74--5,75)],
                                        (5,69--5,87))],
                                  Some (Private (5,57--5,64)), (5,57--5,87)),
                               None, Const (Unit, (5,90--5,92)), (5,57--5,87),
                               NoneAtInvisible,
                               { LeadingKeyword = Member (5,4--5,10)
                                 InlineKeyword = None
                                 EqualsRange = Some (5,88--5,89) })),
                         (5,4--5,92), { InlineKeyword = None
                                        WithKeyword = (5,24--5,28)
                                        GetKeyword = Some (5,29--5,32)
                                        AndKeyword = Some (5,53--5,56)
                                        SetKeyword = Some (5,65--5,68) })],
                     (4,4--5,92)), [],
                  Some
                    (ImplicitCtor
                       (None, [], Const (Unit, (3,8--3,10)), None,
                        PreXmlDoc ((3,8), FSharp.Compiler.Xml.XmlDocCollector),
                        (3,5--3,8), { AsKeyword = None })), (3,5--5,92),
                  { LeadingKeyword = Type (3,0--3,4)
                    EqualsRange = Some (3,11--3,12)
                    WithKeyword = None })], (3,0--5,92))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--5,92), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))

(5,57)-(5,87) parse error When the visibility for a property is specified, setting the visibility of the set or get method is not allowed.
