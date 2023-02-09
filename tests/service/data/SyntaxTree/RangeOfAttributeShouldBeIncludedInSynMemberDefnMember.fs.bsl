ImplFile
  (ParsedImplFileInput
     ("/root/RangeOfAttributeShouldBeIncludedInSynMemberDefnMember.fs", false,
      QualifiedNameOfFile RangeOfAttributeShouldBeIncludedInSynMemberDefnMember,
      [], [],
      [SynModuleOrNamespace
         ([RangeOfAttributeShouldBeIncludedInSynMemberDefnMember], false,
          AnonModule,
          [Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([], None, [], [Bar],
                     PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None,
                     /root/RangeOfAttributeShouldBeIncludedInSynMemberDefnMember.fs (1,5--1,8)),
                  ObjectModel
                    (Unspecified,
                     [Member
                        (SynBinding
                           (None, Normal, false, false,
                            [{ Attributes =
                                [{ TypeName = SynLongIdent ([Foo], [], [None])
                                   ArgExpr =
                                    Const
                                      (Unit,
                                       /root/RangeOfAttributeShouldBeIncludedInSynMemberDefnMember.fs (2,6--2,9))
                                   Target = None
                                   AppliesToGetterAndSetter = false
                                   Range =
                                    /root/RangeOfAttributeShouldBeIncludedInSynMemberDefnMember.fs (2,6--2,9) }]
                               Range =
                                /root/RangeOfAttributeShouldBeIncludedInSynMemberDefnMember.fs (2,4--2,11) }],
                            PreXmlDoc ((2,4), FSharp.Compiler.Xml.XmlDocCollector),
                            SynValData
                              (Some { IsInstance = true
                                      IsDispatchSlot = false
                                      IsOverrideOrExplicitImpl = false
                                      IsFinal = false
                                      GetterOrSetterIsCompilerGenerated = false
                                      MemberKind = Member },
                               SynValInfo
                                 ([[SynArgInfo ([], false, None)]; []],
                                  SynArgInfo ([], false, None)), None),
                            LongIdent
                              (SynLongIdent
                                 ([this; Something],
                                  [/root/RangeOfAttributeShouldBeIncludedInSynMemberDefnMember.fs (3,15--3,16)],
                                  [None; None]), None, None,
                               Pats
                                 [Paren
                                    (Const
                                       (Unit,
                                        /root/RangeOfAttributeShouldBeIncludedInSynMemberDefnMember.fs (3,26--3,28)),
                                     /root/RangeOfAttributeShouldBeIncludedInSynMemberDefnMember.fs (3,26--3,28))],
                               None,
                               /root/RangeOfAttributeShouldBeIncludedInSynMemberDefnMember.fs (3,11--3,28)),
                            None,
                            Const
                              (Unit,
                               /root/RangeOfAttributeShouldBeIncludedInSynMemberDefnMember.fs (3,31--3,33)),
                            /root/RangeOfAttributeShouldBeIncludedInSynMemberDefnMember.fs (2,4--3,28),
                            NoneAtInvisible,
                            { LeadingKeyword =
                               Member
                                 /root/RangeOfAttributeShouldBeIncludedInSynMemberDefnMember.fs (3,4--3,10)
                              InlineKeyword = None
                              EqualsRange =
                               Some
                                 /root/RangeOfAttributeShouldBeIncludedInSynMemberDefnMember.fs (3,29--3,30) }),
                         /root/RangeOfAttributeShouldBeIncludedInSynMemberDefnMember.fs (2,4--3,33))],
                     /root/RangeOfAttributeShouldBeIncludedInSynMemberDefnMember.fs (2,4--3,33)),
                  [], None,
                  /root/RangeOfAttributeShouldBeIncludedInSynMemberDefnMember.fs (1,5--3,33),
                  { LeadingKeyword =
                     Type
                       /root/RangeOfAttributeShouldBeIncludedInSynMemberDefnMember.fs (1,0--1,4)
                    EqualsRange =
                     Some
                       /root/RangeOfAttributeShouldBeIncludedInSynMemberDefnMember.fs (1,9--1,10)
                    WithKeyword = None })],
              /root/RangeOfAttributeShouldBeIncludedInSynMemberDefnMember.fs (1,0--3,33))],
          PreXmlDocEmpty, [], None,
          /root/RangeOfAttributeShouldBeIncludedInSynMemberDefnMember.fs (1,0--3,33),
          { LeadingKeyword = None })], (true, false),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))