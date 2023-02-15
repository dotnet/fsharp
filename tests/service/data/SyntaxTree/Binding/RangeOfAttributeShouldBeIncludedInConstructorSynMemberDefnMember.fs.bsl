ImplFile
  (ParsedImplFileInput
     ("/root/Binding/RangeOfAttributeShouldBeIncludedInConstructorSynMemberDefnMember.fs",
      false,
      QualifiedNameOfFile
        RangeOfAttributeShouldBeIncludedInConstructorSynMemberDefnMember, [], [],
      [SynModuleOrNamespace
         ([RangeOfAttributeShouldBeIncludedInConstructorSynMemberDefnMember],
          false, AnonModule,
          [Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([], None, [], [Tiger],
                     PreXmlDoc ((2,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (2,5--2,10)),
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
                              (Some { IsInstance = false
                                      IsDispatchSlot = false
                                      IsOverrideOrExplicitImpl = false
                                      IsFinal = false
                                      GetterOrSetterIsCompilerGenerated = false
                                      MemberKind = Constructor },
                               SynValInfo ([[]], SynArgInfo ([], false, None)),
                               None),
                            LongIdent
                              (SynLongIdent ([new], [], [None]), None,
                               Some (SynValTyparDecls (None, false)),
                               Pats
                                 [Paren (Const (Unit, (4,8--4,10)), (4,8--4,10))],
                               None, (4,4--4,7)), None,
                            Const (Unit, (4,13--4,15)), (3,4--4,10),
                            NoneAtInvisible, { LeadingKeyword = New (4,4--4,7)
                                               InlineKeyword = None
                                               EqualsRange = Some (4,11--4,12) }),
                         (3,4--4,15))], (3,4--4,15)), [], None, (2,5--4,15),
                  { LeadingKeyword = Type (2,0--2,4)
                    EqualsRange = Some (2,11--2,12)
                    WithKeyword = None })], (2,0--4,15))], PreXmlDocEmpty, [],
          None, (2,0--5,0), { LeadingKeyword = None })], (true, false),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))
