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
                     PreXmlDoc ((2,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None,
                     /root/ReadwritePropertyInSynMemberDefnMemberContainsTheRangeOfTheWithKeyword.fs (2,5--2,8)),
                  ObjectModel
                    (Unspecified,
                     [ImplicitCtor
                        (None, [],
                         SimplePats
                           ([],
                            /root/ReadwritePropertyInSynMemberDefnMemberContainsTheRangeOfTheWithKeyword.fs (2,8--2,10)),
                         None,
                         PreXmlDoc ((2,8), FSharp.Compiler.Xml.XmlDocCollector),
                         /root/ReadwritePropertyInSynMemberDefnMemberContainsTheRangeOfTheWithKeyword.fs (2,5--2,8),
                         { AsKeyword = None });
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
                                    ([[SynArgInfo ([], false, None)]; []],
                                     SynArgInfo ([], false, None)), None),
                               LongIdent
                                 (SynLongIdent
                                    ([this; MyReadWriteProperty],
                                     [/root/ReadwritePropertyInSynMemberDefnMemberContainsTheRangeOfTheWithKeyword.fs (4,15--4,16)],
                                     [None; None]), Some get, None,
                                  Pats
                                    [Paren
                                       (Const
                                          (Unit,
                                           /root/ReadwritePropertyInSynMemberDefnMemberContainsTheRangeOfTheWithKeyword.fs (5,17--5,19)),
                                        /root/ReadwritePropertyInSynMemberDefnMemberContainsTheRangeOfTheWithKeyword.fs (5,17--5,19))],
                                  None,
                                  /root/ReadwritePropertyInSynMemberDefnMemberContainsTheRangeOfTheWithKeyword.fs (5,13--5,19)),
                               None, Ident myInternalValue,
                               /root/ReadwritePropertyInSynMemberDefnMemberContainsTheRangeOfTheWithKeyword.fs (5,13--5,19),
                               NoneAtInvisible,
                               { LeadingKeyword =
                                  Member
                                    /root/ReadwritePropertyInSynMemberDefnMemberContainsTheRangeOfTheWithKeyword.fs (4,4--4,10)
                                 InlineKeyword = None
                                 EqualsRange =
                                  Some
                                    /root/ReadwritePropertyInSynMemberDefnMemberContainsTheRangeOfTheWithKeyword.fs (5,20--5,21) })),
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
                                      [SynArgInfo ([], false, Some value)]],
                                     SynArgInfo ([], false, None)), None),
                               LongIdent
                                 (SynLongIdent
                                    ([this; MyReadWriteProperty],
                                     [/root/ReadwritePropertyInSynMemberDefnMemberContainsTheRangeOfTheWithKeyword.fs (4,15--4,16)],
                                     [None; None]), Some set, None,
                                  Pats
                                    [Paren
                                       (Named
                                          (SynIdent (value, None), false, None,
                                           /root/ReadwritePropertyInSynMemberDefnMemberContainsTheRangeOfTheWithKeyword.fs (6,17--6,22)),
                                        /root/ReadwritePropertyInSynMemberDefnMemberContainsTheRangeOfTheWithKeyword.fs (6,16--6,23))],
                                  None,
                                  /root/ReadwritePropertyInSynMemberDefnMemberContainsTheRangeOfTheWithKeyword.fs (6,12--6,23)),
                               None,
                               LongIdentSet
                                 (SynLongIdent ([myInternalValue], [], [None]),
                                  Ident value,
                                  /root/ReadwritePropertyInSynMemberDefnMemberContainsTheRangeOfTheWithKeyword.fs (6,26--6,50)),
                               /root/ReadwritePropertyInSynMemberDefnMemberContainsTheRangeOfTheWithKeyword.fs (6,12--6,23),
                               NoneAtInvisible,
                               { LeadingKeyword =
                                  Member
                                    /root/ReadwritePropertyInSynMemberDefnMemberContainsTheRangeOfTheWithKeyword.fs (4,4--4,10)
                                 InlineKeyword = None
                                 EqualsRange =
                                  Some
                                    /root/ReadwritePropertyInSynMemberDefnMemberContainsTheRangeOfTheWithKeyword.fs (6,24--6,25) })),
                         /root/ReadwritePropertyInSynMemberDefnMemberContainsTheRangeOfTheWithKeyword.fs (4,4--6,50),
                         { InlineKeyword = None
                           WithKeyword =
                            /root/ReadwritePropertyInSynMemberDefnMemberContainsTheRangeOfTheWithKeyword.fs (5,8--5,12)
                           GetKeyword =
                            Some
                              /root/ReadwritePropertyInSynMemberDefnMemberContainsTheRangeOfTheWithKeyword.fs (5,13--5,16)
                           AndKeyword =
                            Some
                              /root/ReadwritePropertyInSynMemberDefnMemberContainsTheRangeOfTheWithKeyword.fs (6,8--6,11)
                           SetKeyword =
                            Some
                              /root/ReadwritePropertyInSynMemberDefnMemberContainsTheRangeOfTheWithKeyword.fs (6,12--6,15) })],
                     /root/ReadwritePropertyInSynMemberDefnMemberContainsTheRangeOfTheWithKeyword.fs (4,4--6,50)),
                  [],
                  Some
                    (ImplicitCtor
                       (None, [],
                        SimplePats
                          ([],
                           /root/ReadwritePropertyInSynMemberDefnMemberContainsTheRangeOfTheWithKeyword.fs (2,8--2,10)),
                        None,
                        PreXmlDoc ((2,8), FSharp.Compiler.Xml.XmlDocCollector),
                        /root/ReadwritePropertyInSynMemberDefnMemberContainsTheRangeOfTheWithKeyword.fs (2,5--2,8),
                        { AsKeyword = None })),
                  /root/ReadwritePropertyInSynMemberDefnMemberContainsTheRangeOfTheWithKeyword.fs (2,5--6,50),
                  { LeadingKeyword =
                     Type
                       /root/ReadwritePropertyInSynMemberDefnMemberContainsTheRangeOfTheWithKeyword.fs (2,0--2,4)
                    EqualsRange =
                     Some
                       /root/ReadwritePropertyInSynMemberDefnMemberContainsTheRangeOfTheWithKeyword.fs (2,11--2,12)
                    WithKeyword = None })],
              /root/ReadwritePropertyInSynMemberDefnMemberContainsTheRangeOfTheWithKeyword.fs (2,0--6,50))],
          PreXmlDocEmpty, [], None,
          /root/ReadwritePropertyInSynMemberDefnMemberContainsTheRangeOfTheWithKeyword.fs (2,0--7,0),
          { LeadingKeyword = None })], (true, false),
      { ConditionalDirectives = []
        CodeComments =
         [LineComment
            /root/ReadwritePropertyInSynMemberDefnMemberContainsTheRangeOfTheWithKeyword.fs (3,4--3,29)] },
      set []))