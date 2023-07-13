ImplFile
  (ParsedImplFileInput
     ("/root/Binding/RangeOfAttributeShouldBeIncludedInWriteOnlySynMemberDefnMemberProperty.fs",
      false,
      QualifiedNameOfFile
        RangeOfAttributeShouldBeIncludedInWriteOnlySynMemberDefnMemberProperty,
      [], [],
      [SynModuleOrNamespace
         ([RangeOfAttributeShouldBeIncludedInWriteOnlySynMemberDefnMemberProperty],
          false, AnonModule,
          [Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([], None, [], [Crane],
                     PreXmlDoc ((2,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (2,5--2,10)),
                  ObjectModel
                    (Unspecified,
                     [GetSetMember
                        (None,
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
                                    ([this; MyWriteOnlyProperty], [(4,15--4,16)],
                                     [None; None]), Some set, None,
                                  Pats
                                    [Paren
                                       (Named
                                          (SynIdent (value, None), false, None,
                                           (4,46--4,51)), (4,45--4,52))], None,
                                  (4,41--4,52)), None,
                               LongIdentSet
                                 (SynLongIdent ([myInternalValue], [], [None]),
                                  Ident value, (4,55--4,79)), (3,4--4,52),
                               NoneAtInvisible,
                               { LeadingKeyword = Member (4,4--4,10)
                                 InlineKeyword = None
                                 EqualsRange = Some (4,53--4,54) })),
                         (3,4--4,79), { InlineKeyword = None
                                        WithKeyword = (4,36--4,40)
                                        GetKeyword = None
                                        AndKeyword = None
                                        SetKeyword = Some (4,41--4,44) })],
                     (3,4--4,79)), [], None, (2,5--4,79),
                  { LeadingKeyword = Type (2,0--2,4)
                    EqualsRange = Some (2,11--2,12)
                    WithKeyword = None })], (2,0--4,79))], PreXmlDocEmpty, [],
          None, (2,0--5,0), { LeadingKeyword = None })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))
