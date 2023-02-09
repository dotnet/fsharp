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
                     PreXmlDoc ((2,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None,
                     /root/SynTypeDefnWithStaticMemberWithGetset.fs (2,5--2,8)),
                  ObjectModel
                    (Unspecified,
                     [GetSetMember
                        (Some
                           (SynBinding
                              (None, Normal, false, false, [],
                               PreXmlMerge
  (PreXmlDoc ((3,4), FSharp.Compiler.Xml.XmlDocCollector), PreXmlDocEmpty),
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
                                           /root/SynTypeDefnWithStaticMemberWithGetset.fs (5,17--5,19)),
                                        /root/SynTypeDefnWithStaticMemberWithGetset.fs (5,17--5,19))],
                                  None,
                                  /root/SynTypeDefnWithStaticMemberWithGetset.fs (5,13--5,19)),
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
                                                  /root/SynTypeDefnWithStaticMemberWithGetset.fs (5,35--5,47)),
                                               /root/SynTypeDefnWithStaticMemberWithGetset.fs (5,35--5,47));
                                            Const
                                              (Int32 0,
                                               /root/SynTypeDefnWithStaticMemberWithGetset.fs (5,49--5,50))],
                                           [/root/SynTypeDefnWithStaticMemberWithGetset.fs (5,47--5,48)],
                                           /root/SynTypeDefnWithStaticMemberWithGetset.fs (5,35--5,50)),
                                        /root/SynTypeDefnWithStaticMemberWithGetset.fs (5,34--5,35),
                                        Some
                                          /root/SynTypeDefnWithStaticMemberWithGetset.fs (5,50--5,51),
                                        /root/SynTypeDefnWithStaticMemberWithGetset.fs (5,34--5,51)),
                                     /root/SynTypeDefnWithStaticMemberWithGetset.fs (5,22--5,51)),
                                  Const
                                    (Int32 4,
                                     /root/SynTypeDefnWithStaticMemberWithGetset.fs (5,53--5,54)),
                                  /root/SynTypeDefnWithStaticMemberWithGetset.fs (5,22--5,54)),
                               /root/SynTypeDefnWithStaticMemberWithGetset.fs (5,13--5,19),
                               NoneAtInvisible,
                               { LeadingKeyword =
                                  StaticMember
                                    (/root/SynTypeDefnWithStaticMemberWithGetset.fs (3,4--3,10),
                                     /root/SynTypeDefnWithStaticMemberWithGetset.fs (3,11--3,17))
                                 InlineKeyword = None
                                 EqualsRange =
                                  Some
                                    /root/SynTypeDefnWithStaticMemberWithGetset.fs (5,20--5,21) })),
                         Some
                           (SynBinding
                              (None, Normal, false, false, [],
                               PreXmlMerge
  (PreXmlDoc ((3,4), FSharp.Compiler.Xml.XmlDocCollector), PreXmlDocEmpty),
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
                                        /root/SynTypeDefnWithStaticMemberWithGetset.fs (4,18--4,19))],
                                  None,
                                  /root/SynTypeDefnWithStaticMemberWithGetset.fs (4,13--4,19)),
                               None,
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
                                         Ident x],
                                        [/root/SynTypeDefnWithStaticMemberWithGetset.fs (4,47--4,48)],
                                        /root/SynTypeDefnWithStaticMemberWithGetset.fs (4,35--4,50)),
                                     /root/SynTypeDefnWithStaticMemberWithGetset.fs (4,34--4,35),
                                     Some
                                       /root/SynTypeDefnWithStaticMemberWithGetset.fs (4,50--4,51),
                                     /root/SynTypeDefnWithStaticMemberWithGetset.fs (4,34--4,51)),
                                  /root/SynTypeDefnWithStaticMemberWithGetset.fs (4,22--4,51)),
                               /root/SynTypeDefnWithStaticMemberWithGetset.fs (4,13--4,19),
                               NoneAtInvisible,
                               { LeadingKeyword =
                                  StaticMember
                                    (/root/SynTypeDefnWithStaticMemberWithGetset.fs (3,4--3,10),
                                     /root/SynTypeDefnWithStaticMemberWithGetset.fs (3,11--3,17))
                                 InlineKeyword = None
                                 EqualsRange =
                                  Some
                                    /root/SynTypeDefnWithStaticMemberWithGetset.fs (4,20--4,21) })),
                         /root/SynTypeDefnWithStaticMemberWithGetset.fs (3,4--5,54),
                         { InlineKeyword = None
                           WithKeyword =
                            /root/SynTypeDefnWithStaticMemberWithGetset.fs (4,8--4,12)
                           GetKeyword =
                            Some
                              /root/SynTypeDefnWithStaticMemberWithGetset.fs (5,13--5,16)
                           AndKeyword =
                            Some
                              /root/SynTypeDefnWithStaticMemberWithGetset.fs (5,8--5,11)
                           SetKeyword =
                            Some
                              /root/SynTypeDefnWithStaticMemberWithGetset.fs (4,13--4,16) })],
                     /root/SynTypeDefnWithStaticMemberWithGetset.fs (3,4--5,54)),
                  [], None,
                  /root/SynTypeDefnWithStaticMemberWithGetset.fs (2,5--5,54),
                  { LeadingKeyword =
                     Type
                       /root/SynTypeDefnWithStaticMemberWithGetset.fs (2,0--2,4)
                    EqualsRange =
                     Some
                       /root/SynTypeDefnWithStaticMemberWithGetset.fs (2,9--2,10)
                    WithKeyword = None })],
              /root/SynTypeDefnWithStaticMemberWithGetset.fs (2,0--5,54))],
          PreXmlDocEmpty, [], None,
          /root/SynTypeDefnWithStaticMemberWithGetset.fs (2,0--6,0),
          { LeadingKeyword = None })], (true, false),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))