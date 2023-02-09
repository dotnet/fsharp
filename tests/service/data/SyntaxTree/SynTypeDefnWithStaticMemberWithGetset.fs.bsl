ImplFile
  (ParsedImplFileInput
     ("/root/SynTypeDefnWithStaticMemberWithGetset.fs", false,
      QualifiedNameOfFile SynTypeDefnWithStaticMemberWithGetset, [], [],
      [SynModuleOrNamespace
         ([SynTypeDefnWithStaticMemberWithGetset], false, AnonModule,
          [Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([], None, [], [Foo],
                     PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None,
                     /root/SynTypeDefnWithStaticMemberWithGetset.fs (1,5--1,8)),
                  ObjectModel
                    (Unspecified,
                     [GetSetMember
                        (Some
                           (SynBinding
                              (None, Normal, false, false, [],
                               PreXmlMerge
  (PreXmlDoc ((2,4), FSharp.Compiler.Xml.XmlDocCollector), PreXmlDocEmpty),
                               SynValData
                                 (Some
                                    { IsInstance = false
                                      IsDispatchSlot = false
                                      IsOverrideOrExplicitImpl = false
                                      IsFinal = false
                                      GetterOrSetterIsCompilerGenerated = false
                                      MemberKind = PropertyGet },
                                  SynValInfo
                                    ([[]], SynArgInfo ([], false, None)), None),
                               LongIdent
                                 (SynLongIdent ([ReadWrite2], [], [None]),
                                  Some get, None,
                                  Pats
                                    [Paren
                                       (Const
                                          (Unit,
                                           /root/SynTypeDefnWithStaticMemberWithGetset.fs (4,17--4,19)),
                                        /root/SynTypeDefnWithStaticMemberWithGetset.fs (4,17--4,19))],
                                  None,
                                  /root/SynTypeDefnWithStaticMemberWithGetset.fs (4,13--4,19)),
                               None,
                               Sequential
                                 (SuppressNeither, true,
                                  LongIdentSet
                                    (SynLongIdent ([lastUsed], [], [None]),
                                     Paren
                                       (Tuple
                                          (false,
                                           [Const
                                              (String
                                                 ("ReadWrite2", Regular,
                                                  /root/SynTypeDefnWithStaticMemberWithGetset.fs (4,35--4,47)),
                                               /root/SynTypeDefnWithStaticMemberWithGetset.fs (4,35--4,47));
                                            Const
                                              (Int32 0,
                                               /root/SynTypeDefnWithStaticMemberWithGetset.fs (4,49--4,50))],
                                           [/root/SynTypeDefnWithStaticMemberWithGetset.fs (4,47--4,48)],
                                           /root/SynTypeDefnWithStaticMemberWithGetset.fs (4,35--4,50)),
                                        /root/SynTypeDefnWithStaticMemberWithGetset.fs (4,34--4,35),
                                        Some
                                          /root/SynTypeDefnWithStaticMemberWithGetset.fs (4,50--4,51),
                                        /root/SynTypeDefnWithStaticMemberWithGetset.fs (4,34--4,51)),
                                     /root/SynTypeDefnWithStaticMemberWithGetset.fs (4,22--4,51)),
                                  Const
                                    (Int32 4,
                                     /root/SynTypeDefnWithStaticMemberWithGetset.fs (4,53--4,54)),
                                  /root/SynTypeDefnWithStaticMemberWithGetset.fs (4,22--4,54)),
                               /root/SynTypeDefnWithStaticMemberWithGetset.fs (4,13--4,19),
                               NoneAtInvisible,
                               { LeadingKeyword =
                                  StaticMember
                                    (/root/SynTypeDefnWithStaticMemberWithGetset.fs (2,4--2,10),
                                     /root/SynTypeDefnWithStaticMemberWithGetset.fs (2,11--2,17))
                                 InlineKeyword = None
                                 EqualsRange =
                                  Some
                                    /root/SynTypeDefnWithStaticMemberWithGetset.fs (4,20--4,21) })),
                         Some
                           (SynBinding
                              (None, Normal, false, false, [],
                               PreXmlMerge
  (PreXmlDoc ((2,4), FSharp.Compiler.Xml.XmlDocCollector), PreXmlDocEmpty),
                               SynValData
                                 (Some
                                    { IsInstance = false
                                      IsDispatchSlot = false
                                      IsOverrideOrExplicitImpl = false
                                      IsFinal = false
                                      GetterOrSetterIsCompilerGenerated = false
                                      MemberKind = PropertySet },
                                  SynValInfo
                                    ([[SynArgInfo ([], false, Some x)]],
                                     SynArgInfo ([], false, None)), None),
                               LongIdent
                                 (SynLongIdent ([ReadWrite2], [], [None]),
                                  Some set, None,
                                  Pats
                                    [Named
                                       (SynIdent (x, None), false, None,
                                        /root/SynTypeDefnWithStaticMemberWithGetset.fs (3,18--3,19))],
                                  None,
                                  /root/SynTypeDefnWithStaticMemberWithGetset.fs (3,13--3,19)),
                               None,
                               LongIdentSet
                                 (SynLongIdent ([lastUsed], [], [None]),
                                  Paren
                                    (Tuple
                                       (false,
                                        [Const
                                           (String
                                              ("ReadWrite2", Regular,
                                               /root/SynTypeDefnWithStaticMemberWithGetset.fs (3,35--3,47)),
                                            /root/SynTypeDefnWithStaticMemberWithGetset.fs (3,35--3,47));
                                         Ident x],
                                        [/root/SynTypeDefnWithStaticMemberWithGetset.fs (3,47--3,48)],
                                        /root/SynTypeDefnWithStaticMemberWithGetset.fs (3,35--3,50)),
                                     /root/SynTypeDefnWithStaticMemberWithGetset.fs (3,34--3,35),
                                     Some
                                       /root/SynTypeDefnWithStaticMemberWithGetset.fs (3,50--3,51),
                                     /root/SynTypeDefnWithStaticMemberWithGetset.fs (3,34--3,51)),
                                  /root/SynTypeDefnWithStaticMemberWithGetset.fs (3,22--3,51)),
                               /root/SynTypeDefnWithStaticMemberWithGetset.fs (3,13--3,19),
                               NoneAtInvisible,
                               { LeadingKeyword =
                                  StaticMember
                                    (/root/SynTypeDefnWithStaticMemberWithGetset.fs (2,4--2,10),
                                     /root/SynTypeDefnWithStaticMemberWithGetset.fs (2,11--2,17))
                                 InlineKeyword = None
                                 EqualsRange =
                                  Some
                                    /root/SynTypeDefnWithStaticMemberWithGetset.fs (3,20--3,21) })),
                         /root/SynTypeDefnWithStaticMemberWithGetset.fs (2,4--4,54),
                         { InlineKeyword = None
                           WithKeyword =
                            /root/SynTypeDefnWithStaticMemberWithGetset.fs (3,8--3,12)
                           GetKeyword =
                            Some
                              /root/SynTypeDefnWithStaticMemberWithGetset.fs (4,13--4,16)
                           AndKeyword =
                            Some
                              /root/SynTypeDefnWithStaticMemberWithGetset.fs (4,8--4,11)
                           SetKeyword =
                            Some
                              /root/SynTypeDefnWithStaticMemberWithGetset.fs (3,13--3,16) })],
                     /root/SynTypeDefnWithStaticMemberWithGetset.fs (2,4--4,54)),
                  [], None,
                  /root/SynTypeDefnWithStaticMemberWithGetset.fs (1,5--4,54),
                  { LeadingKeyword =
                     Type
                       /root/SynTypeDefnWithStaticMemberWithGetset.fs (1,0--1,4)
                    EqualsRange =
                     Some
                       /root/SynTypeDefnWithStaticMemberWithGetset.fs (1,9--1,10)
                    WithKeyword = None })],
              /root/SynTypeDefnWithStaticMemberWithGetset.fs (1,0--4,54))],
          PreXmlDocEmpty, [], None,
          /root/SynTypeDefnWithStaticMemberWithGetset.fs (1,0--4,54),
          { LeadingKeyword = None })], (true, false),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))