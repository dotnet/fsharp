ImplFile
  (ParsedImplFileInput
     ("/root/ReadwritePropertyInSynMemberDefnMemberContainsTheRangeOfTheWithKeyword.fs",
      false,
      QualifiedNameOfFile
        ReadwritePropertyInSynMemberDefnMemberContainsTheRangeOfTheWithKeyword,
      [], [],
      [SynModuleOrNamespace
         ([ReadwritePropertyInSynMemberDefnMemberContainsTheRangeOfTheWithKeyword],
          false, AnonModule,
          [Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([], None, [], [Foo],
                     PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None,
                     /root/ReadwritePropertyInSynMemberDefnMemberContainsTheRangeOfTheWithKeyword.fs (1,5--1,8)),
                  ObjectModel
                    (Unspecified,
                     [ImplicitCtor
                        (None, [],
                         SimplePats
                           ([],
                            /root/ReadwritePropertyInSynMemberDefnMemberContainsTheRangeOfTheWithKeyword.fs (1,8--1,10)),
                         None,
                         PreXmlDoc ((1,8), FSharp.Compiler.Xml.XmlDocCollector),
                         /root/ReadwritePropertyInSynMemberDefnMemberContainsTheRangeOfTheWithKeyword.fs (1,5--1,8),
                         { AsKeyword = None });
                      GetSetMember
                        (Some
                           (SynBinding
                              (None, Normal, false, false, [],
                               PreXmlMerge
  (PreXmlDoc ((3,4), FSharp.Compiler.Xml.XmlDocCollector), PreXmlDocEmpty),
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
                                    ([this; MyReadWriteProperty],
                                     [/root/ReadwritePropertyInSynMemberDefnMemberContainsTheRangeOfTheWithKeyword.fs (3,15--3,16)],
                                     [None; None]), Some get, None,
                                  Pats
                                    [Paren
                                       (Const
                                          (Unit,
                                           /root/ReadwritePropertyInSynMemberDefnMemberContainsTheRangeOfTheWithKeyword.fs (4,17--4,19)),
                                        /root/ReadwritePropertyInSynMemberDefnMemberContainsTheRangeOfTheWithKeyword.fs (4,17--4,19))],
                                  None,
                                  /root/ReadwritePropertyInSynMemberDefnMemberContainsTheRangeOfTheWithKeyword.fs (4,13--4,19)),
                               None, Ident myInternalValue,
                               /root/ReadwritePropertyInSynMemberDefnMemberContainsTheRangeOfTheWithKeyword.fs (4,13--4,19),
                               NoneAtInvisible,
                               { LeadingKeyword =
                                  Member
                                    /root/ReadwritePropertyInSynMemberDefnMemberContainsTheRangeOfTheWithKeyword.fs (3,4--3,10)
                                 InlineKeyword = None
                                 EqualsRange =
                                  Some
                                    /root/ReadwritePropertyInSynMemberDefnMemberContainsTheRangeOfTheWithKeyword.fs (4,20--4,21) })),
                         Some
                           (SynBinding
                              (None, Normal, false, false, [],
                               PreXmlMerge
  (PreXmlDoc ((3,4), FSharp.Compiler.Xml.XmlDocCollector), PreXmlDocEmpty),
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
                                      [SynArgInfo ([], false, Some value)]],
                                     SynArgInfo ([], false, None)), None),
                               LongIdent
                                 (SynLongIdent
                                    ([this; MyReadWriteProperty],
                                     [/root/ReadwritePropertyInSynMemberDefnMemberContainsTheRangeOfTheWithKeyword.fs (3,15--3,16)],
                                     [None; None]), Some set, None,
                                  Pats
                                    [Paren
                                       (Named
                                          (SynIdent (value, None), false, None,
                                           /root/ReadwritePropertyInSynMemberDefnMemberContainsTheRangeOfTheWithKeyword.fs (5,17--5,22)),
                                        /root/ReadwritePropertyInSynMemberDefnMemberContainsTheRangeOfTheWithKeyword.fs (5,16--5,23))],
                                  None,
                                  /root/ReadwritePropertyInSynMemberDefnMemberContainsTheRangeOfTheWithKeyword.fs (5,12--5,23)),
                               None,
                               LongIdentSet
                                 (SynLongIdent ([myInternalValue], [], [None]),
                                  Ident value,
                                  /root/ReadwritePropertyInSynMemberDefnMemberContainsTheRangeOfTheWithKeyword.fs (5,26--5,50)),
                               /root/ReadwritePropertyInSynMemberDefnMemberContainsTheRangeOfTheWithKeyword.fs (5,12--5,23),
                               NoneAtInvisible,
                               { LeadingKeyword =
                                  Member
                                    /root/ReadwritePropertyInSynMemberDefnMemberContainsTheRangeOfTheWithKeyword.fs (3,4--3,10)
                                 InlineKeyword = None
                                 EqualsRange =
                                  Some
                                    /root/ReadwritePropertyInSynMemberDefnMemberContainsTheRangeOfTheWithKeyword.fs (5,24--5,25) })),
                         /root/ReadwritePropertyInSynMemberDefnMemberContainsTheRangeOfTheWithKeyword.fs (3,4--5,50),
                         { InlineKeyword = None
                           WithKeyword =
                            /root/ReadwritePropertyInSynMemberDefnMemberContainsTheRangeOfTheWithKeyword.fs (4,8--4,12)
                           GetKeyword =
                            Some
                              /root/ReadwritePropertyInSynMemberDefnMemberContainsTheRangeOfTheWithKeyword.fs (4,13--4,16)
                           AndKeyword =
                            Some
                              /root/ReadwritePropertyInSynMemberDefnMemberContainsTheRangeOfTheWithKeyword.fs (5,8--5,11)
                           SetKeyword =
                            Some
                              /root/ReadwritePropertyInSynMemberDefnMemberContainsTheRangeOfTheWithKeyword.fs (5,12--5,15) })],
                     /root/ReadwritePropertyInSynMemberDefnMemberContainsTheRangeOfTheWithKeyword.fs (3,4--5,50)),
                  [],
                  Some
                    (ImplicitCtor
                       (None, [],
                        SimplePats
                          ([],
                           /root/ReadwritePropertyInSynMemberDefnMemberContainsTheRangeOfTheWithKeyword.fs (1,8--1,10)),
                        None,
                        PreXmlDoc ((1,8), FSharp.Compiler.Xml.XmlDocCollector),
                        /root/ReadwritePropertyInSynMemberDefnMemberContainsTheRangeOfTheWithKeyword.fs (1,5--1,8),
                        { AsKeyword = None })),
                  /root/ReadwritePropertyInSynMemberDefnMemberContainsTheRangeOfTheWithKeyword.fs (1,5--5,50),
                  { LeadingKeyword =
                     Type
                       /root/ReadwritePropertyInSynMemberDefnMemberContainsTheRangeOfTheWithKeyword.fs (1,0--1,4)
                    EqualsRange =
                     Some
                       /root/ReadwritePropertyInSynMemberDefnMemberContainsTheRangeOfTheWithKeyword.fs (1,11--1,12)
                    WithKeyword = None })],
              /root/ReadwritePropertyInSynMemberDefnMemberContainsTheRangeOfTheWithKeyword.fs (1,0--5,50))],
          PreXmlDocEmpty, [], None,
          /root/ReadwritePropertyInSynMemberDefnMemberContainsTheRangeOfTheWithKeyword.fs (1,0--5,50),
          { LeadingKeyword = None })], (true, false),
      { ConditionalDirectives = []
        CodeComments =
         [LineComment
            /root/ReadwritePropertyInSynMemberDefnMemberContainsTheRangeOfTheWithKeyword.fs (2,4--2,29)] },
      set []))