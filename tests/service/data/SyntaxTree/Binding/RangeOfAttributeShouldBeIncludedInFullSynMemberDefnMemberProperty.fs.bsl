ImplFile
  (ParsedImplFileInput
     ("/root/Binding/RangeOfAttributeShouldBeIncludedInFullSynMemberDefnMemberProperty.fs",
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
                     PreXmlDoc ((2,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None,
                     /root/Binding/RangeOfAttributeShouldBeIncludedInFullSynMemberDefnMemberProperty.fs (2,5--2,9)),
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
                                          /root/Binding/RangeOfAttributeShouldBeIncludedInFullSynMemberDefnMemberProperty.fs (3,6--3,9))
                                      Target = None
                                      AppliesToGetterAndSetter = false
                                      Range =
                                       /root/Binding/RangeOfAttributeShouldBeIncludedInFullSynMemberDefnMemberProperty.fs (3,6--3,9) }]
                                  Range =
                                   /root/Binding/RangeOfAttributeShouldBeIncludedInFullSynMemberDefnMemberProperty.fs (3,4--3,11) }],
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
                                    ([this; TheWord],
                                     [/root/Binding/RangeOfAttributeShouldBeIncludedInFullSynMemberDefnMemberProperty.fs (4,15--4,16)],
                                     [None; None]), Some get, None,
                                  Pats
                                    [Paren
                                       (Const
                                          (Unit,
                                           /root/Binding/RangeOfAttributeShouldBeIncludedInFullSynMemberDefnMemberProperty.fs (5,17--5,19)),
                                        /root/Binding/RangeOfAttributeShouldBeIncludedInFullSynMemberDefnMemberProperty.fs (5,17--5,19))],
                                  None,
                                  /root/Binding/RangeOfAttributeShouldBeIncludedInFullSynMemberDefnMemberProperty.fs (5,13--5,19)),
                               None, Ident myInternalValue,
                               /root/Binding/RangeOfAttributeShouldBeIncludedInFullSynMemberDefnMemberProperty.fs (3,4--5,19),
                               NoneAtInvisible,
                               { LeadingKeyword =
                                  Member
                                    /root/Binding/RangeOfAttributeShouldBeIncludedInFullSynMemberDefnMemberProperty.fs (4,4--4,10)
                                 InlineKeyword = None
                                 EqualsRange =
                                  Some
                                    /root/Binding/RangeOfAttributeShouldBeIncludedInFullSynMemberDefnMemberProperty.fs (5,20--5,21) })),
                         Some
                           (SynBinding
                              (None, Normal, false, false,
                               [{ Attributes =
                                   [{ TypeName =
                                       SynLongIdent ([Foo], [], [None])
                                      ArgExpr =
                                       Const
                                         (Unit,
                                          /root/Binding/RangeOfAttributeShouldBeIncludedInFullSynMemberDefnMemberProperty.fs (3,6--3,9))
                                      Target = None
                                      AppliesToGetterAndSetter = false
                                      Range =
                                       /root/Binding/RangeOfAttributeShouldBeIncludedInFullSynMemberDefnMemberProperty.fs (3,6--3,9) }]
                                  Range =
                                   /root/Binding/RangeOfAttributeShouldBeIncludedInFullSynMemberDefnMemberProperty.fs (3,4--3,11) }],
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
                                    ([this; TheWord],
                                     [/root/Binding/RangeOfAttributeShouldBeIncludedInFullSynMemberDefnMemberProperty.fs (4,15--4,16)],
                                     [None; None]), Some set, None,
                                  Pats
                                    [Paren
                                       (Named
                                          (SynIdent (value, None), false, None,
                                           /root/Binding/RangeOfAttributeShouldBeIncludedInFullSynMemberDefnMemberProperty.fs (6,17--6,22)),
                                        /root/Binding/RangeOfAttributeShouldBeIncludedInFullSynMemberDefnMemberProperty.fs (6,16--6,23))],
                                  None,
                                  /root/Binding/RangeOfAttributeShouldBeIncludedInFullSynMemberDefnMemberProperty.fs (6,12--6,23)),
                               None,
                               LongIdentSet
                                 (SynLongIdent ([myInternalValue], [], [None]),
                                  Ident value,
                                  /root/Binding/RangeOfAttributeShouldBeIncludedInFullSynMemberDefnMemberProperty.fs (6,26--6,50)),
                               /root/Binding/RangeOfAttributeShouldBeIncludedInFullSynMemberDefnMemberProperty.fs (3,4--6,23),
                               NoneAtInvisible,
                               { LeadingKeyword =
                                  Member
                                    /root/Binding/RangeOfAttributeShouldBeIncludedInFullSynMemberDefnMemberProperty.fs (4,4--4,10)
                                 InlineKeyword = None
                                 EqualsRange =
                                  Some
                                    /root/Binding/RangeOfAttributeShouldBeIncludedInFullSynMemberDefnMemberProperty.fs (6,24--6,25) })),
                         /root/Binding/RangeOfAttributeShouldBeIncludedInFullSynMemberDefnMemberProperty.fs (3,4--6,50),
                         { InlineKeyword = None
                           WithKeyword =
                            /root/Binding/RangeOfAttributeShouldBeIncludedInFullSynMemberDefnMemberProperty.fs (5,8--5,12)
                           GetKeyword =
                            Some
                              /root/Binding/RangeOfAttributeShouldBeIncludedInFullSynMemberDefnMemberProperty.fs (5,13--5,16)
                           AndKeyword =
                            Some
                              /root/Binding/RangeOfAttributeShouldBeIncludedInFullSynMemberDefnMemberProperty.fs (6,8--6,11)
                           SetKeyword =
                            Some
                              /root/Binding/RangeOfAttributeShouldBeIncludedInFullSynMemberDefnMemberProperty.fs (6,12--6,15) })],
                     /root/Binding/RangeOfAttributeShouldBeIncludedInFullSynMemberDefnMemberProperty.fs (3,4--6,50)),
                  [], None,
                  /root/Binding/RangeOfAttributeShouldBeIncludedInFullSynMemberDefnMemberProperty.fs (2,5--6,50),
                  { LeadingKeyword =
                     Type
                       /root/Binding/RangeOfAttributeShouldBeIncludedInFullSynMemberDefnMemberProperty.fs (2,0--2,4)
                    EqualsRange =
                     Some
                       /root/Binding/RangeOfAttributeShouldBeIncludedInFullSynMemberDefnMemberProperty.fs (2,10--2,11)
                    WithKeyword = None })],
              /root/Binding/RangeOfAttributeShouldBeIncludedInFullSynMemberDefnMemberProperty.fs (2,0--6,50))],
          PreXmlDocEmpty, [], None,
          /root/Binding/RangeOfAttributeShouldBeIncludedInFullSynMemberDefnMemberProperty.fs (2,0--7,0),
          { LeadingKeyword = None })], (true, false),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))