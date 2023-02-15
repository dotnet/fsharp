ImplFile
  (ParsedImplFileInput
     ("/root/Member/Read-onlyPropertyInSynMemberDefnMemberContainsTheRangeOfTheWithKeyword.fs",
      false,
      QualifiedNameOfFile
        Read-onlyPropertyInSynMemberDefnMemberContainsTheRangeOfTheWithKeyword,
      [], [],
      [SynModuleOrNamespace
         ([Read-onlyPropertyInSynMemberDefnMemberContainsTheRangeOfTheWithKeyword],
          false, AnonModule,
          [Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([], None, [], [Foo],
                     PreXmlDoc ((2,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (2,5--2,8)),
                  ObjectModel
                    (Unspecified,
                     [ImplicitCtor
                        (None, [], SimplePats ([], (2,8--2,10)), None,
                         PreXmlDoc ((2,8), FSharp.Compiler.Xml.XmlDocCollector),
                         (2,5--2,8), { AsKeyword = None });
                      GetSetMember
                        (Some
                           (SynBinding
                              (None, Normal, false, false, [],
                               PreXmlMerge
  (PreXmlDoc ((4,4), FSharp.Compiler.Xml.XmlDocCollector), PreXmlDocEmpty),
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
                                    ([this; MyReadProperty], [(4,15--4,16)],
                                     [None; None]), Some get, None,
                                  Pats
                                    [Paren
                                       (Const (Unit, (4,40--4,42)), (4,40--4,42))],
                                  None, (4,36--4,42)), None,
                               Ident myInternalValue, (4,36--4,42),
                               NoneAtInvisible,
                               { LeadingKeyword = Member (4,4--4,10)
                                 InlineKeyword = None
                                 EqualsRange = Some (4,43--4,44) })), None,
                         (4,4--4,60), { InlineKeyword = None
                                        WithKeyword = (4,31--4,35)
                                        GetKeyword = Some (4,36--4,39)
                                        AndKeyword = None
                                        SetKeyword = None })], (4,4--4,60)), [],
                  Some
                    (ImplicitCtor
                       (None, [], SimplePats ([], (2,8--2,10)), None,
                        PreXmlDoc ((2,8), FSharp.Compiler.Xml.XmlDocCollector),
                        (2,5--2,8), { AsKeyword = None })), (2,5--4,60),
                  { LeadingKeyword = Type (2,0--2,4)
                    EqualsRange = Some (2,11--2,12)
                    WithKeyword = None })], (2,0--4,60))], PreXmlDocEmpty, [],
          None, (2,0--5,0), { LeadingKeyword = None })], (true, false),
      { ConditionalDirectives = []
        CodeComments = [LineComment (3,4--3,28)] }, set []))
