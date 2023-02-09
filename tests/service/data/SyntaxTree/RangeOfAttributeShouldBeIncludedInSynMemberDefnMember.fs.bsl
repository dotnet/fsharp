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
                     PreXmlDoc ((2,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None,
                     /root/RangeOfAttributeShouldBeIncludedInSynMemberDefnMember.fs (2,5--2,8)),
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
                                       /root/RangeOfAttributeShouldBeIncludedInSynMemberDefnMember.fs (3,6--3,9))
                                   Target = None
                                   AppliesToGetterAndSetter = false
                                   Range =
                                    /root/RangeOfAttributeShouldBeIncludedInSynMemberDefnMember.fs (3,6--3,9) }]
                               Range =
                                /root/RangeOfAttributeShouldBeIncludedInSynMemberDefnMember.fs (3,4--3,11) }],
                            PreXmlDoc ((3,4), FSharp.Compiler.Xml.XmlDocCollector),
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
                                  [/root/RangeOfAttributeShouldBeIncludedInSynMemberDefnMember.fs (4,15--4,16)],
                                  [None; None]), None, None,
                               Pats
                                 [Paren
                                    (Const
                                       (Unit,
                                        /root/RangeOfAttributeShouldBeIncludedInSynMemberDefnMember.fs (4,26--4,28)),
                                     /root/RangeOfAttributeShouldBeIncludedInSynMemberDefnMember.fs (4,26--4,28))],
                               None,
                               /root/RangeOfAttributeShouldBeIncludedInSynMemberDefnMember.fs (4,11--4,28)),
                            None,
                            Const
                              (Unit,
                               /root/RangeOfAttributeShouldBeIncludedInSynMemberDefnMember.fs (4,31--4,33)),
                            /root/RangeOfAttributeShouldBeIncludedInSynMemberDefnMember.fs (3,4--4,28),
                            NoneAtInvisible,
                            { LeadingKeyword =
                               Member
                                 /root/RangeOfAttributeShouldBeIncludedInSynMemberDefnMember.fs (4,4--4,10)
                              InlineKeyword = None
                              EqualsRange =
                               Some
                                 /root/RangeOfAttributeShouldBeIncludedInSynMemberDefnMember.fs (4,29--4,30) }),
                         /root/RangeOfAttributeShouldBeIncludedInSynMemberDefnMember.fs (3,4--4,33))],
                     /root/RangeOfAttributeShouldBeIncludedInSynMemberDefnMember.fs (3,4--4,33)),
                  [], None,
                  /root/RangeOfAttributeShouldBeIncludedInSynMemberDefnMember.fs (2,5--4,33),
                  { LeadingKeyword =
                     Type
                       /root/RangeOfAttributeShouldBeIncludedInSynMemberDefnMember.fs (2,0--2,4)
                    EqualsRange =
                     Some
                       /root/RangeOfAttributeShouldBeIncludedInSynMemberDefnMember.fs (2,9--2,10)
                    WithKeyword = None })],
              /root/RangeOfAttributeShouldBeIncludedInSynMemberDefnMember.fs (2,0--4,33))],
          PreXmlDocEmpty, [], None,
          /root/RangeOfAttributeShouldBeIncludedInSynMemberDefnMember.fs (2,0--5,0),
          { LeadingKeyword = None })], (true, false),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))