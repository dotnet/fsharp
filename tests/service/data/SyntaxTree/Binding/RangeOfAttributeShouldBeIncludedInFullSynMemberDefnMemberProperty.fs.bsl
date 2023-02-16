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
                     false, None, (2,5--2,9)),
                  ObjectModel
                    (Unspecified,
                     [GetSetMember
                        (Some
                           (SynBinding
                              (None, Normal, false, false,
                               [{ Attributes =
                                   [{ TypeName =
                                       SynLongIdent ([Foo], [], [None])
                                      ArgExpr = Const (Unit, (3,6--3,9))
                                      Target = None
                                      AppliesToGetterAndSetter = false
                                      Range = (3,6--3,9) }]
                                  Range = (3,4--3,11) }],
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
                                    ([this; TheWord], [(4,15--4,16)],
                                     [None; None]), Some get, None,
                                  Pats
                                    [Paren
                                       (Const (Unit, (5,17--5,19)), (5,17--5,19))],
                                  None, (5,13--5,19)), None,
                               Ident myInternalValue, (3,4--5,19),
                               NoneAtInvisible,
                               { LeadingKeyword = Member (4,4--4,10)
                                 InlineKeyword = None
                                 EqualsRange = Some (5,20--5,21) })),
                         Some
                           (SynBinding
                              (None, Normal, false, false,
                               [{ Attributes =
                                   [{ TypeName =
                                       SynLongIdent ([Foo], [], [None])
                                      ArgExpr = Const (Unit, (3,6--3,9))
                                      Target = None
                                      AppliesToGetterAndSetter = false
                                      Range = (3,6--3,9) }]
                                  Range = (3,4--3,11) }],
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
                                    ([this; TheWord], [(4,15--4,16)],
                                     [None; None]), Some set, None,
                                  Pats
                                    [Paren
                                       (Named
                                          (SynIdent (value, None), false, None,
                                           (6,17--6,22)), (6,16--6,23))], None,
                                  (6,12--6,23)), None,
                               LongIdentSet
                                 (SynLongIdent ([myInternalValue], [], [None]),
                                  Ident value, (6,26--6,50)), (3,4--6,23),
                               NoneAtInvisible,
                               { LeadingKeyword = Member (4,4--4,10)
                                 InlineKeyword = None
                                 EqualsRange = Some (6,24--6,25) })),
                         (3,4--6,50), { InlineKeyword = None
                                        WithKeyword = (5,8--5,12)
                                        GetKeyword = Some (5,13--5,16)
                                        AndKeyword = Some (6,8--6,11)
                                        SetKeyword = Some (6,12--6,15) })],
                     (3,4--6,50)), [], None, (2,5--6,50),
                  { LeadingKeyword = Type (2,0--2,4)
                    EqualsRange = Some (2,10--2,11)
                    WithKeyword = None })], (2,0--6,50))], PreXmlDocEmpty, [],
          None, (2,0--7,0), { LeadingKeyword = None })], (true, false),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))
