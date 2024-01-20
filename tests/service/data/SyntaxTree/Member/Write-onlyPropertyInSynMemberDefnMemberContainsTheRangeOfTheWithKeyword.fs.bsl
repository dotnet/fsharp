ImplFile
  (ParsedImplFileInput
     ("/root/Member/Write-onlyPropertyInSynMemberDefnMemberContainsTheRangeOfTheWithKeyword.fs",
      false,
      QualifiedNameOfFile
        Write-onlyPropertyInSynMemberDefnMemberContainsTheRangeOfTheWithKeyword,
      [], [],
      [SynModuleOrNamespace
         ([Foo], false, DeclaredNamespace,
          [Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([], None, [], [Foo],
                     PreXmlDoc ((3,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (3,5--3,8)),
                  ObjectModel
                    (Unspecified,
                     [ImplicitCtor
                        (None, [], Const (Unit, (3,8--3,10)), None,
                         PreXmlDoc ((3,8), FSharp.Compiler.Xml.XmlDocCollector),
                         (3,5--3,8), { AsKeyword = None });
                      GetSetMember
                        (None,
                         Some
                           (SynBinding
                              (None, Normal, false, false, [],
                               PreXmlMerge
  (PreXmlDoc ((5,4), FSharp.Compiler.Xml.XmlDocCollector), PreXmlDocEmpty),
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
                                    ([this; MyWriteOnlyProperty], [(5,15--5,16)],
                                     [None; None]), Some set, None,
                                  Pats
                                    [Paren
                                       (Named
                                          (SynIdent (value, None), false, None,
                                           (5,46--5,51)), (5,45--5,52))], None,
                                  (5,41--5,52)), None,
                               LongIdentSet
                                 (SynLongIdent ([myInternalValue], [], [None]),
                                  Ident value, (5,55--5,79)), (5,41--5,52),
                               NoneAtInvisible,
                               { LeadingKeyword = Member (5,4--5,10)
                                 InlineKeyword = None
                                 EqualsRange = Some (5,53--5,54) })),
                         (5,4--5,79), { InlineKeyword = None
                                        WithKeyword = (5,36--5,40)
                                        GetKeyword = None
                                        AndKeyword = None
                                        SetKeyword = Some (5,41--5,44) })],
                     (5,4--5,79)), [],
                  Some
                    (ImplicitCtor
                       (None, [], Const (Unit, (3,8--3,10)), None,
                        PreXmlDoc ((3,8), FSharp.Compiler.Xml.XmlDocCollector),
                        (3,5--3,8), { AsKeyword = None })), (3,5--5,79),
                  { LeadingKeyword = Type (3,0--3,4)
                    EqualsRange = Some (3,11--3,12)
                    WithKeyword = None })], (3,0--5,79))], PreXmlDocEmpty, [],
          None, (1,0--5,79), { LeadingKeyword = Namespace (1,0--1,9) })],
      (true, true), { ConditionalDirectives = []
                      CodeComments = [LineComment (4,4--4,29)] }, set []))
