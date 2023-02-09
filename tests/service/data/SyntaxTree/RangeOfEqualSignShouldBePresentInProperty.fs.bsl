ImplFile
  (ParsedImplFileInput
     ("/root/RangeOfEqualSignShouldBePresentInProperty.fs", false,
      QualifiedNameOfFile RangeOfEqualSignShouldBePresentInProperty, [], [],
      [SynModuleOrNamespace
         ([RangeOfEqualSignShouldBePresentInProperty], false, AnonModule,
          [Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([], None, [], [Y],
                     PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None,
                     /root/RangeOfEqualSignShouldBePresentInProperty.fs (1,5--1,6)),
                  ObjectModel
                    (Unspecified,
                     [ImplicitCtor
                        (None, [],
                         SimplePats
                           ([],
                            /root/RangeOfEqualSignShouldBePresentInProperty.fs (1,6--1,8)),
                         None,
                         PreXmlDoc ((1,6), FSharp.Compiler.Xml.XmlDocCollector),
                         /root/RangeOfEqualSignShouldBePresentInProperty.fs (1,5--1,6),
                         { AsKeyword = None });
                      GetSetMember
                        (Some
                           (SynBinding
                              (None, Normal, false, false, [],
                               PreXmlMerge
  (PreXmlDoc ((2,4), FSharp.Compiler.Xml.XmlDocCollector), PreXmlDocEmpty),
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
                                     [/root/RangeOfEqualSignShouldBePresentInProperty.fs (2,15--2,16)],
                                     [None; None]), Some get, None,
                                  Pats
                                    [Paren
                                       (Const
                                          (Unit,
                                           /root/RangeOfEqualSignShouldBePresentInProperty.fs (3,17--3,19)),
                                        /root/RangeOfEqualSignShouldBePresentInProperty.fs (3,17--3,19))],
                                  None,
                                  /root/RangeOfEqualSignShouldBePresentInProperty.fs (3,13--3,19)),
                               None, Ident myInternalValue,
                               /root/RangeOfEqualSignShouldBePresentInProperty.fs (3,13--3,19),
                               NoneAtInvisible,
                               { LeadingKeyword =
                                  Member
                                    /root/RangeOfEqualSignShouldBePresentInProperty.fs (2,4--2,10)
                                 InlineKeyword = None
                                 EqualsRange =
                                  Some
                                    /root/RangeOfEqualSignShouldBePresentInProperty.fs (3,20--3,21) })),
                         Some
                           (SynBinding
                              (None, Normal, false, false, [],
                               PreXmlMerge
  (PreXmlDoc ((2,4), FSharp.Compiler.Xml.XmlDocCollector), PreXmlDocEmpty),
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
                                     [/root/RangeOfEqualSignShouldBePresentInProperty.fs (2,15--2,16)],
                                     [None; None]), Some set, None,
                                  Pats
                                    [Paren
                                       (Named
                                          (SynIdent (value, None), false, None,
                                           /root/RangeOfEqualSignShouldBePresentInProperty.fs (4,17--4,22)),
                                        /root/RangeOfEqualSignShouldBePresentInProperty.fs (4,16--4,23))],
                                  None,
                                  /root/RangeOfEqualSignShouldBePresentInProperty.fs (4,12--4,23)),
                               None,
                               LongIdentSet
                                 (SynLongIdent ([myInternalValue], [], [None]),
                                  Ident value,
                                  /root/RangeOfEqualSignShouldBePresentInProperty.fs (4,26--4,50)),
                               /root/RangeOfEqualSignShouldBePresentInProperty.fs (4,12--4,23),
                               NoneAtInvisible,
                               { LeadingKeyword =
                                  Member
                                    /root/RangeOfEqualSignShouldBePresentInProperty.fs (2,4--2,10)
                                 InlineKeyword = None
                                 EqualsRange =
                                  Some
                                    /root/RangeOfEqualSignShouldBePresentInProperty.fs (4,24--4,25) })),
                         /root/RangeOfEqualSignShouldBePresentInProperty.fs (2,4--4,50),
                         { InlineKeyword = None
                           WithKeyword =
                            /root/RangeOfEqualSignShouldBePresentInProperty.fs (3,8--3,12)
                           GetKeyword =
                            Some
                              /root/RangeOfEqualSignShouldBePresentInProperty.fs (3,13--3,16)
                           AndKeyword =
                            Some
                              /root/RangeOfEqualSignShouldBePresentInProperty.fs (4,8--4,11)
                           SetKeyword =
                            Some
                              /root/RangeOfEqualSignShouldBePresentInProperty.fs (4,12--4,15) })],
                     /root/RangeOfEqualSignShouldBePresentInProperty.fs (2,4--4,50)),
                  [],
                  Some
                    (ImplicitCtor
                       (None, [],
                        SimplePats
                          ([],
                           /root/RangeOfEqualSignShouldBePresentInProperty.fs (1,6--1,8)),
                        None,
                        PreXmlDoc ((1,6), FSharp.Compiler.Xml.XmlDocCollector),
                        /root/RangeOfEqualSignShouldBePresentInProperty.fs (1,5--1,6),
                        { AsKeyword = None })),
                  /root/RangeOfEqualSignShouldBePresentInProperty.fs (1,5--4,50),
                  { LeadingKeyword =
                     Type
                       /root/RangeOfEqualSignShouldBePresentInProperty.fs (1,0--1,4)
                    EqualsRange =
                     Some
                       /root/RangeOfEqualSignShouldBePresentInProperty.fs (1,9--1,10)
                    WithKeyword = None })],
              /root/RangeOfEqualSignShouldBePresentInProperty.fs (1,0--4,50))],
          PreXmlDocEmpty, [], None,
          /root/RangeOfEqualSignShouldBePresentInProperty.fs (1,0--4,50),
          { LeadingKeyword = None })], (true, false),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))