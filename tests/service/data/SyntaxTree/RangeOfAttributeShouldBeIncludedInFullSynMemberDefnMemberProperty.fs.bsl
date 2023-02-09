ImplFile
  (ParsedImplFileInput
     ("/root/RangeOfAttributeShouldBeIncludedInFullSynMemberDefnMemberProperty.fs",
      false,
      QualifiedNameOfFile
        RangeOfAttributeShouldBeIncludedInFullSynMemberDefnMemberProperty, [],
      [],
      [SynModuleOrNamespace
         ([RangeOfAttributeShouldBeIncludedInFullSynMemberDefnMemberProperty],
          false, AnonModule,
          [Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([], None, [], [Bird],
                     PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None,
                     /root/RangeOfAttributeShouldBeIncludedInFullSynMemberDefnMemberProperty.fs (1,5--1,9)),
                  ObjectModel
                    (Unspecified,
                     [GetSetMember
                        (Some
                           (SynBinding
                              (None, Normal, false, false,
                               [{ Attributes =
                                   [{ TypeName =
                                       SynLongIdent ([Foo], [], [None])
                                      ArgExpr =
                                       Const
                                         (Unit,
                                          /root/RangeOfAttributeShouldBeIncludedInFullSynMemberDefnMemberProperty.fs (2,6--2,9))
                                      Target = None
                                      AppliesToGetterAndSetter = false
                                      Range =
                                       /root/RangeOfAttributeShouldBeIncludedInFullSynMemberDefnMemberProperty.fs (2,6--2,9) }]
                                  Range =
                                   /root/RangeOfAttributeShouldBeIncludedInFullSynMemberDefnMemberProperty.fs (2,4--2,11) }],
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
                                    ([this; TheWord],
                                     [/root/RangeOfAttributeShouldBeIncludedInFullSynMemberDefnMemberProperty.fs (3,15--3,16)],
                                     [None; None]), Some get, None,
                                  Pats
                                    [Paren
                                       (Const
                                          (Unit,
                                           /root/RangeOfAttributeShouldBeIncludedInFullSynMemberDefnMemberProperty.fs (4,17--4,19)),
                                        /root/RangeOfAttributeShouldBeIncludedInFullSynMemberDefnMemberProperty.fs (4,17--4,19))],
                                  None,
                                  /root/RangeOfAttributeShouldBeIncludedInFullSynMemberDefnMemberProperty.fs (4,13--4,19)),
                               None, Ident myInternalValue,
                               /root/RangeOfAttributeShouldBeIncludedInFullSynMemberDefnMemberProperty.fs (2,4--4,19),
                               NoneAtInvisible,
                               { LeadingKeyword =
                                  Member
                                    /root/RangeOfAttributeShouldBeIncludedInFullSynMemberDefnMemberProperty.fs (3,4--3,10)
                                 InlineKeyword = None
                                 EqualsRange =
                                  Some
                                    /root/RangeOfAttributeShouldBeIncludedInFullSynMemberDefnMemberProperty.fs (4,20--4,21) })),
                         Some
                           (SynBinding
                              (None, Normal, false, false,
                               [{ Attributes =
                                   [{ TypeName =
                                       SynLongIdent ([Foo], [], [None])
                                      ArgExpr =
                                       Const
                                         (Unit,
                                          /root/RangeOfAttributeShouldBeIncludedInFullSynMemberDefnMemberProperty.fs (2,6--2,9))
                                      Target = None
                                      AppliesToGetterAndSetter = false
                                      Range =
                                       /root/RangeOfAttributeShouldBeIncludedInFullSynMemberDefnMemberProperty.fs (2,6--2,9) }]
                                  Range =
                                   /root/RangeOfAttributeShouldBeIncludedInFullSynMemberDefnMemberProperty.fs (2,4--2,11) }],
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
                                    ([this; TheWord],
                                     [/root/RangeOfAttributeShouldBeIncludedInFullSynMemberDefnMemberProperty.fs (3,15--3,16)],
                                     [None; None]), Some set, None,
                                  Pats
                                    [Paren
                                       (Named
                                          (SynIdent (value, None), false, None,
                                           /root/RangeOfAttributeShouldBeIncludedInFullSynMemberDefnMemberProperty.fs (5,17--5,22)),
                                        /root/RangeOfAttributeShouldBeIncludedInFullSynMemberDefnMemberProperty.fs (5,16--5,23))],
                                  None,
                                  /root/RangeOfAttributeShouldBeIncludedInFullSynMemberDefnMemberProperty.fs (5,12--5,23)),
                               None,
                               LongIdentSet
                                 (SynLongIdent ([myInternalValue], [], [None]),
                                  Ident value,
                                  /root/RangeOfAttributeShouldBeIncludedInFullSynMemberDefnMemberProperty.fs (5,26--5,50)),
                               /root/RangeOfAttributeShouldBeIncludedInFullSynMemberDefnMemberProperty.fs (2,4--5,23),
                               NoneAtInvisible,
                               { LeadingKeyword =
                                  Member
                                    /root/RangeOfAttributeShouldBeIncludedInFullSynMemberDefnMemberProperty.fs (3,4--3,10)
                                 InlineKeyword = None
                                 EqualsRange =
                                  Some
                                    /root/RangeOfAttributeShouldBeIncludedInFullSynMemberDefnMemberProperty.fs (5,24--5,25) })),
                         /root/RangeOfAttributeShouldBeIncludedInFullSynMemberDefnMemberProperty.fs (2,4--5,50),
                         { InlineKeyword = None
                           WithKeyword =
                            /root/RangeOfAttributeShouldBeIncludedInFullSynMemberDefnMemberProperty.fs (4,8--4,12)
                           GetKeyword =
                            Some
                              /root/RangeOfAttributeShouldBeIncludedInFullSynMemberDefnMemberProperty.fs (4,13--4,16)
                           AndKeyword =
                            Some
                              /root/RangeOfAttributeShouldBeIncludedInFullSynMemberDefnMemberProperty.fs (5,8--5,11)
                           SetKeyword =
                            Some
                              /root/RangeOfAttributeShouldBeIncludedInFullSynMemberDefnMemberProperty.fs (5,12--5,15) })],
                     /root/RangeOfAttributeShouldBeIncludedInFullSynMemberDefnMemberProperty.fs (2,4--5,50)),
                  [], None,
                  /root/RangeOfAttributeShouldBeIncludedInFullSynMemberDefnMemberProperty.fs (1,5--5,50),
                  { LeadingKeyword =
                     Type
                       /root/RangeOfAttributeShouldBeIncludedInFullSynMemberDefnMemberProperty.fs (1,0--1,4)
                    EqualsRange =
                     Some
                       /root/RangeOfAttributeShouldBeIncludedInFullSynMemberDefnMemberProperty.fs (1,10--1,11)
                    WithKeyword = None })],
              /root/RangeOfAttributeShouldBeIncludedInFullSynMemberDefnMemberProperty.fs (1,0--5,50))],
          PreXmlDocEmpty, [], None,
          /root/RangeOfAttributeShouldBeIncludedInFullSynMemberDefnMemberProperty.fs (1,0--5,50),
          { LeadingKeyword = None })], (true, false),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))