ImplFile
  (ParsedImplFileInput
     ("/root/Binding/RangeOfEqualSignShouldBePresentInProperty.fs", false,
      QualifiedNameOfFile RangeOfEqualSignShouldBePresentInProperty, [], [],
      [SynModuleOrNamespace
         ([RangeOfEqualSignShouldBePresentInProperty], false, AnonModule,
          [Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([], None, [], [Y],
                     PreXmlDoc ((2,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (2,5--2,6)),
                  ObjectModel
                    (Unspecified,
                     [ImplicitCtor
                        (None, [], SimplePats ([], [], (2,6--2,8)), None,
                         PreXmlDoc ((2,6), FSharp.Compiler.Xml.XmlDocCollector),
                         (2,5--2,6), { AsKeyword = None });
                      GetSetMember
                        (Some
                           (SynBinding
                              (None, Normal, false, false, [],
                               PreXmlMerge
  (PreXmlDoc ((3,4), FSharp.Compiler.Xml.XmlDocCollector), PreXmlDocEmpty),
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
                                     SynArgInfo ([], false, None)), None, None),
                               LongIdent
                                 (SynLongIdent
                                    ([this; MyReadWriteProperty], [(3,15--3,16)],
                                     [None; None]), Some get, None,
                                  Pats
                                    [Paren
                                       (Const (Unit, (4,17--4,19)), (4,17--4,19))],
                                  None, (4,13--4,19)), None,
                               Ident myInternalValue, (4,13--4,19),
                               NoneAtInvisible,
                               { LeadingKeyword = Member (3,4--3,10)
                                 InlineKeyword = None
                                 EqualsRange = Some (4,20--4,21) })),
                         Some
                           (SynBinding
                              (None, Normal, false, false, [],
                               PreXmlMerge
  (PreXmlDoc ((3,4), FSharp.Compiler.Xml.XmlDocCollector), PreXmlDocEmpty),
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
                                     SynArgInfo ([], false, None)), None, None),
                               LongIdent
                                 (SynLongIdent
                                    ([this; MyReadWriteProperty], [(3,15--3,16)],
                                     [None; None]), Some set, None,
                                  Pats
                                    [Paren
                                       (Named
                                          (SynIdent (value, None), false, None,
                                           (5,17--5,22)), (5,16--5,23))], None,
                                  (5,12--5,23)), None,
                               LongIdentSet
                                 (SynLongIdent ([myInternalValue], [], [None]),
                                  Ident value, (5,26--5,50)), (5,12--5,23),
                               NoneAtInvisible,
                               { LeadingKeyword = Member (3,4--3,10)
                                 InlineKeyword = None
                                 EqualsRange = Some (5,24--5,25) })),
                         (3,4--5,50), { InlineKeyword = None
                                        WithKeyword = (4,8--4,12)
                                        GetKeyword = Some (4,13--4,16)
                                        AndKeyword = Some (5,8--5,11)
                                        SetKeyword = Some (5,12--5,15) })],
                     (3,4--5,50)), [],
                  Some
                    (ImplicitCtor
                       (None, [], SimplePats ([], [], (2,6--2,8)), None,
                        PreXmlDoc ((2,6), FSharp.Compiler.Xml.XmlDocCollector),
                        (2,5--2,6), { AsKeyword = None })), (2,5--5,50),
                  { LeadingKeyword = Type (2,0--2,4)
                    EqualsRange = Some (2,9--2,10)
                    WithKeyword = None })], (2,0--5,50))], PreXmlDocEmpty, [],
          None, (2,0--6,0), { LeadingKeyword = None })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))
