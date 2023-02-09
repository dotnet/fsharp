ImplFile
  (ParsedImplFileInput
     ("/root/RangeOfAttributeShouldBeIncludedInWriteOnlySynMemberDefnMemberProperty.fs",
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
                     PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None,
                     /root/RangeOfAttributeShouldBeIncludedInWriteOnlySynMemberDefnMemberProperty.fs (1,5--1,10)),
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
                                      ArgExpr =
                                       Const
                                         (Unit,
                                          /root/RangeOfAttributeShouldBeIncludedInWriteOnlySynMemberDefnMemberProperty.fs (2,6--2,9))
                                      Target = None
                                      AppliesToGetterAndSetter = false
                                      Range =
                                       /root/RangeOfAttributeShouldBeIncludedInWriteOnlySynMemberDefnMemberProperty.fs (2,6--2,9) }]
                                  Range =
                                   /root/RangeOfAttributeShouldBeIncludedInWriteOnlySynMemberDefnMemberProperty.fs (2,4--2,11) }],
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
                                    ([this; MyWriteOnlyProperty],
                                     [/root/RangeOfAttributeShouldBeIncludedInWriteOnlySynMemberDefnMemberProperty.fs (3,15--3,16)],
                                     [None; None]), Some set, None,
                                  Pats
                                    [Paren
                                       (Named
                                          (SynIdent (value, None), false, None,
                                           /root/RangeOfAttributeShouldBeIncludedInWriteOnlySynMemberDefnMemberProperty.fs (3,46--3,51)),
                                        /root/RangeOfAttributeShouldBeIncludedInWriteOnlySynMemberDefnMemberProperty.fs (3,45--3,52))],
                                  None,
                                  /root/RangeOfAttributeShouldBeIncludedInWriteOnlySynMemberDefnMemberProperty.fs (3,41--3,52)),
                               None,
                               LongIdentSet
                                 (SynLongIdent ([myInternalValue], [], [None]),
                                  Ident value,
                                  /root/RangeOfAttributeShouldBeIncludedInWriteOnlySynMemberDefnMemberProperty.fs (3,55--3,79)),
                               /root/RangeOfAttributeShouldBeIncludedInWriteOnlySynMemberDefnMemberProperty.fs (2,4--3,52),
                               NoneAtInvisible,
                               { LeadingKeyword =
                                  Member
                                    /root/RangeOfAttributeShouldBeIncludedInWriteOnlySynMemberDefnMemberProperty.fs (3,4--3,10)
                                 InlineKeyword = None
                                 EqualsRange =
                                  Some
                                    /root/RangeOfAttributeShouldBeIncludedInWriteOnlySynMemberDefnMemberProperty.fs (3,53--3,54) })),
                         /root/RangeOfAttributeShouldBeIncludedInWriteOnlySynMemberDefnMemberProperty.fs (2,4--3,79),
                         { InlineKeyword = None
                           WithKeyword =
                            /root/RangeOfAttributeShouldBeIncludedInWriteOnlySynMemberDefnMemberProperty.fs (3,36--3,40)
                           GetKeyword = None
                           AndKeyword = None
                           SetKeyword =
                            Some
                              /root/RangeOfAttributeShouldBeIncludedInWriteOnlySynMemberDefnMemberProperty.fs (3,41--3,44) })],
                     /root/RangeOfAttributeShouldBeIncludedInWriteOnlySynMemberDefnMemberProperty.fs (2,4--3,79)),
                  [], None,
                  /root/RangeOfAttributeShouldBeIncludedInWriteOnlySynMemberDefnMemberProperty.fs (1,5--3,79),
                  { LeadingKeyword =
                     Type
                       /root/RangeOfAttributeShouldBeIncludedInWriteOnlySynMemberDefnMemberProperty.fs (1,0--1,4)
                    EqualsRange =
                     Some
                       /root/RangeOfAttributeShouldBeIncludedInWriteOnlySynMemberDefnMemberProperty.fs (1,11--1,12)
                    WithKeyword = None })],
              /root/RangeOfAttributeShouldBeIncludedInWriteOnlySynMemberDefnMemberProperty.fs (1,0--3,79))],
          PreXmlDocEmpty, [], None,
          /root/RangeOfAttributeShouldBeIncludedInWriteOnlySynMemberDefnMemberProperty.fs (1,0--3,79),
          { LeadingKeyword = None })], (true, false),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))