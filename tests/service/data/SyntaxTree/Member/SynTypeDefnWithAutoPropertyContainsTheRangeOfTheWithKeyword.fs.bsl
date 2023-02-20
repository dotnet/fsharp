ImplFile
  (ParsedImplFileInput
     ("/root/Member/SynTypeDefnWithAutoPropertyContainsTheRangeOfTheWithKeyword.fs",
      false,
      QualifiedNameOfFile
        SynTypeDefnWithAutoPropertyContainsTheRangeOfTheWithKeyword, [], [],
      [SynModuleOrNamespace
         ([SynTypeDefnWithAutoPropertyContainsTheRangeOfTheWithKeyword], false,
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
                        (None, [], SimplePats ([], (2,8--2,10)), None,
                         PreXmlDoc ((2,8), FSharp.Compiler.Xml.XmlDocCollector),
                         (2,5--2,8), { AsKeyword = None });
                      AutoProperty
                        ([], false, AutoProperty, None, PropertyGetSet,
                         { IsInstance = true
                           IsDispatchSlot = false
                           IsOverrideOrExplicitImpl = false
                           IsFinal = false
                           GetterOrSetterIsCompilerGenerated = false
                           MemberKind = Member },
                         { IsInstance = true
                           IsDispatchSlot = false
                           IsOverrideOrExplicitImpl = false
                           IsFinal = false
                           GetterOrSetterIsCompilerGenerated = false
                           MemberKind = PropertySet },
                         PreXmlDoc ((3,4), FSharp.Compiler.Xml.XmlDocCollector),
                         None, Ident autoProp, (3,4--3,38),
                         { LeadingKeyword =
                            MemberVal ((3,4--3,10), (3,11--3,14))
                           WithKeyword = Some (3,39--3,43)
                           EqualsRange = Some (3,28--3,29)
                           GetSetKeywords =
                            Some (GetSet ((3,44--3,47), (3,49--3,52))) });
                      AutoProperty
                        ([], false, AutoProperty2, None, Member,
                         { IsInstance = true
                           IsDispatchSlot = false
                           IsOverrideOrExplicitImpl = false
                           IsFinal = false
                           GetterOrSetterIsCompilerGenerated = false
                           MemberKind = Member },
                         { IsInstance = true
                           IsDispatchSlot = false
                           IsOverrideOrExplicitImpl = false
                           IsFinal = false
                           GetterOrSetterIsCompilerGenerated = false
                           MemberKind = PropertySet },
                         PreXmlDoc ((4,4), FSharp.Compiler.Xml.XmlDocCollector),
                         None, Ident autoProp, (4,4--4,39),
                         { LeadingKeyword =
                            MemberVal ((4,4--4,10), (4,11--4,14))
                           WithKeyword = None
                           EqualsRange = Some (4,29--4,30)
                           GetSetKeywords = None })], (3,4--4,39)), [],
                  Some
                    (ImplicitCtor
                       (None, [], SimplePats ([], (2,8--2,10)), None,
                        PreXmlDoc ((2,8), FSharp.Compiler.Xml.XmlDocCollector),
                        (2,5--2,8), { AsKeyword = None })), (2,5--4,39),
                  { LeadingKeyword = Type (2,0--2,4)
                    EqualsRange = Some (2,11--2,12)
                    WithKeyword = None })], (2,0--4,39))], PreXmlDocEmpty, [],
          None, (2,0--5,0), { LeadingKeyword = None })], (true, false),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))
