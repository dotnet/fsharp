ImplFile
  (ParsedImplFileInput
     ("/root/Member/Write-onlyPropertyInSynMemberDefnMemberContainsTheRangeOfTheWithKeyword.fs",
      false,
      QualifiedNameOfFile
        Write-onlyPropertyInSynMemberDefnMemberContainsTheRangeOfTheWithKeyword,
      [], [],
      [SynModuleOrNamespace
         ([Write-onlyPropertyInSynMemberDefnMemberContainsTheRangeOfTheWithKeyword],
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
                        (None,
                         Some
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
                                  Ident value, (4,55--4,79)), (4,41--4,52),
                               NoneAtInvisible,
                               { LeadingKeyword = Member (4,4--4,10)
                                 InlineKeyword = None
                                 EqualsRange = Some (4,53--4,54) })),
                         (4,4--4,79), { InlineKeyword = None
                                        WithKeyword = (4,36--4,40)
                                        GetKeyword = None
                                        AndKeyword = None
                                        SetKeyword = Some (4,41--4,44) })],
                     (4,4--4,79)), [],
                  Some
                    (ImplicitCtor
                       (None, [], SimplePats ([], (2,8--2,10)), None,
                        PreXmlDoc ((2,8), FSharp.Compiler.Xml.XmlDocCollector),
                        (2,5--2,8), { AsKeyword = None })), (2,5--4,79),
                  { LeadingKeyword = Type (2,0--2,4)
                    EqualsRange = Some (2,11--2,12)
                    WithKeyword = None })], (2,0--4,79))], PreXmlDocEmpty, [],
          None, (2,0--5,0), { LeadingKeyword = None })], (true, false),
      { ConditionalDirectives = []
        CodeComments = [LineComment (3,4--3,29)] }, set []))
