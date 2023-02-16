ImplFile
  (ParsedImplFileInput
     ("/root/Binding/RangeOfAttributeShouldBeIncludedInSynMemberDefnMember.fs",
      false,
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
                     false, None, (2,5--2,8)),
                  ObjectModel
                    (Unspecified,
                     [Member
                        (SynBinding
                           (None, Normal, false, false,
                            [{ Attributes =
                                [{ TypeName = SynLongIdent ([Foo], [], [None])
                                   ArgExpr = Const (Unit, (3,6--3,9))
                                   Target = None
                                   AppliesToGetterAndSetter = false
                                   Range = (3,6--3,9) }]
                               Range = (3,4--3,11) }],
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
                                 ([this; Something], [(4,15--4,16)],
                                  [None; None]), None, None,
                               Pats
                                 [Paren
                                    (Const (Unit, (4,26--4,28)), (4,26--4,28))],
                               None, (4,11--4,28)), None,
                            Const (Unit, (4,31--4,33)), (3,4--4,28),
                            NoneAtInvisible,
                            { LeadingKeyword = Member (4,4--4,10)
                              InlineKeyword = None
                              EqualsRange = Some (4,29--4,30) }), (3,4--4,33))],
                     (3,4--4,33)), [], None, (2,5--4,33),
                  { LeadingKeyword = Type (2,0--2,4)
                    EqualsRange = Some (2,9--2,10)
                    WithKeyword = None })], (2,0--4,33))], PreXmlDocEmpty, [],
          None, (2,0--5,0), { LeadingKeyword = None })], (true, false),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))
