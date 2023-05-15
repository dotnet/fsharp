ImplFile
  (ParsedImplFileInput
     ("/root/Member/SynTypeDefnWithAutoPropertyContainsTheRangeOfTheEqualsSign.fs",
      false,
      QualifiedNameOfFile
        SynTypeDefnWithAutoPropertyContainsTheRangeOfTheEqualsSign, [], [],
      [SynModuleOrNamespace
         ([SynTypeDefnWithAutoPropertyContainsTheRangeOfTheEqualsSign], false,
          AnonModule,
          [Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([], None, [], [Person],
                     PreXmlDoc ((3,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (3,5--3,11)),
                  ObjectModel
                    (Unspecified,
                     [ImplicitCtor
                        (None, [],
                         SimplePats
                           ([Typed
                               (Id
                                  (name, None, false, false, false, (3,12--3,16)),
                                LongIdent (SynLongIdent ([string], [], [None])),
                                (3,12--3,25));
                             Typed
                               (Id
                                  (age, None, false, false, false, (3,27--3,30)),
                                LongIdent (SynLongIdent ([int], [], [None])),
                                (3,27--3,36))], [(3,25--3,26)], (3,11--3,37)),
                         None,
                         PreXmlDoc ((3,11), FSharp.Compiler.Xml.XmlDocCollector),
                         (3,5--3,11), { AsKeyword = None });
                      AutoProperty
                        ([], false, Name, None, PropertyGetSet,
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
                         PreXmlDoc ((5,4), FSharp.Compiler.Xml.XmlDocCollector),
                         None, Ident name, (4,4--5,26),
                         { LeadingKeyword =
                            MemberVal ((5,4--5,10), (5,11--5,14))
                           WithKeyword = Some (5,27--5,31)
                           EqualsRange = Some (5,20--5,21)
                           GetSetKeywords =
                            Some (GetSet ((5,32--5,35), (5,37--5,40))) })],
                     (4,4--5,26)), [],
                  Some
                    (ImplicitCtor
                       (None, [],
                        SimplePats
                          ([Typed
                              (Id
                                 (name, None, false, false, false, (3,12--3,16)),
                               LongIdent (SynLongIdent ([string], [], [None])),
                               (3,12--3,25));
                            Typed
                              (Id (age, None, false, false, false, (3,27--3,30)),
                               LongIdent (SynLongIdent ([int], [], [None])),
                               (3,27--3,36))], [(3,25--3,26)], (3,11--3,37)),
                        None,
                        PreXmlDoc ((3,11), FSharp.Compiler.Xml.XmlDocCollector),
                        (3,5--3,11), { AsKeyword = None })), (2,0--5,26),
                  { LeadingKeyword = Type (3,0--3,4)
                    EqualsRange = Some (3,38--3,39)
                    WithKeyword = None })], (2,0--5,26))], PreXmlDocEmpty, [],
          None, (3,0--6,0), { LeadingKeyword = None })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))
