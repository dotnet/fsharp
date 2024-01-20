ImplFile
  (ParsedImplFileInput
     ("/root/Type/SynMemberDefnInterfaceContainsTheRangeOfTheWithKeyword.fs",
      false,
      QualifiedNameOfFile SynMemberDefnInterfaceContainsTheRangeOfTheWithKeyword,
      [], [],
      [SynModuleOrNamespace
         ([SynMemberDefnInterfaceContainsTheRangeOfTheWithKeyword], false,
          AnonModule,
          [Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([], None, [], [Foo],
                     PreXmlDoc ((2,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (2,5--2,8)),
                  ObjectModel
                    (Unspecified,
                     [ImplicitCtor
                        (None, [], Const (Unit, (2,8--2,10)), None,
                         PreXmlDoc ((2,8), FSharp.Compiler.Xml.XmlDocCollector),
                         (2,5--2,8), { AsKeyword = None });
                      Interface
                        (LongIdent (SynLongIdent ([Bar], [], [None])),
                         Some (3,18--3,22),
                         Some
                           [Member
                              (SynBinding
                                 (None, Normal, false, false, [],
                                  PreXmlDoc ((4,8), FSharp.Compiler.Xml.XmlDocCollector),
                                  SynValData
                                    (Some
                                       { IsInstance = true
                                         IsDispatchSlot = false
                                         IsOverrideOrExplicitImpl = true
                                         IsFinal = false
                                         GetterOrSetterIsCompilerGenerated =
                                          false
                                         MemberKind = Member },
                                     SynValInfo
                                       ([[SynArgInfo ([], false, None)]; []],
                                        SynArgInfo ([], false, None)), None),
                                  LongIdent
                                    (SynLongIdent ([Meh], [], [None]), None,
                                     None,
                                     Pats
                                       [Paren
                                          (Const (Unit, (4,19--4,21)),
                                           (4,19--4,21))], None, (4,15--4,21)),
                                  None, Const (Unit, (4,24--4,26)), (4,15--4,21),
                                  NoneAtInvisible,
                                  { LeadingKeyword = Member (4,8--4,14)
                                    InlineKeyword = None
                                    EqualsRange = Some (4,22--4,23) }),
                               (4,8--4,26))], (3,4--4,26));
                      Interface
                        (LongIdent (SynLongIdent ([Other], [], [None])), None,
                         None, (5,4--5,19))], (3,4--5,19)), [],
                  Some
                    (ImplicitCtor
                       (None, [], Const (Unit, (2,8--2,10)), None,
                        PreXmlDoc ((2,8), FSharp.Compiler.Xml.XmlDocCollector),
                        (2,5--2,8), { AsKeyword = None })), (2,5--5,19),
                  { LeadingKeyword = Type (2,0--2,4)
                    EqualsRange = Some (2,11--2,12)
                    WithKeyword = None })], (2,0--5,19))], PreXmlDocEmpty, [],
          None, (2,0--6,0), { LeadingKeyword = None })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))
