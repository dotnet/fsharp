ImplFile
  (ParsedImplFileInput
     ("/root/Binding/RangeOfAttributeShouldBeIncludedInSecondaryConstructor.fs",
      false,
      QualifiedNameOfFile RangeOfAttributeShouldBeIncludedInSecondaryConstructor,
      [], [],
      [SynModuleOrNamespace
         ([RangeOfAttributeShouldBeIncludedInSecondaryConstructor], false,
          AnonModule,
          [Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([], None, [], [T],
                     PreXmlDoc ((2,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (2,5--2,6)),
                  ObjectModel
                    (Unspecified,
                     [ImplicitCtor
                        (None, [], Const (Unit, (2,6--2,8)), None,
                         PreXmlDoc ((2,6), FSharp.Compiler.Xml.XmlDocCollector),
                         (2,5--2,6), { AsKeyword = None });
                      Member
                        (SynBinding
                           (None, Normal, false, false, [],
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
                                 [Paren (Const (Unit, (3,8--3,10)), (3,8--3,10))],
                               None, (3,4--3,7)), None,
                            App
                              (NonAtomic, false, Ident T,
                               Const (Unit, (4,10--4,12)), (4,8--4,12)),
                            (3,4--3,10), NoneAtInvisible,
                            { LeadingKeyword = New (3,4--3,7)
                              InlineKeyword = None
                              EqualsRange = Some (3,11--3,12) }), (3,4--4,12));
                      Member
                        (SynBinding
                           (None, Normal, false, false, [],
                            PreXmlDoc ((6,4), FSharp.Compiler.Xml.XmlDocCollector),
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
                                 [Paren
                                    (Const (Unit, (6,17--6,19)), (6,17--6,19))],
                               Some (Internal (6,4--6,12)), (6,13--6,16)), None,
                            App
                              (NonAtomic, false, Ident T,
                               Const (Unit, (7,10--7,12)), (7,8--7,12)),
                            (6,4--6,19), NoneAtInvisible,
                            { LeadingKeyword = New (6,13--6,16)
                              InlineKeyword = None
                              EqualsRange = Some (6,20--6,21) }), (6,4--7,12));
                      Member
                        (SynBinding
                           (None, Normal, false, false,
                            [{ Attributes =
                                [{ TypeName = SynLongIdent ([Foo], [], [None])
                                   TypeArgs = []
                                   ArgExpr = Const (Unit, (9,6--9,9))
                                   Target = None
                                   AppliesToGetterAndSetter = false
                                   Range = (9,6--9,9) }]
                               Range = (9,4--9,11) }],
                            PreXmlDoc ((9,4), FSharp.Compiler.Xml.XmlDocCollector),
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
                                 [Paren
                                    (Const (Unit, (10,8--10,10)), (10,8--10,10))],
                               None, (10,4--10,7)), None,
                            App
                              (NonAtomic, false, Ident T,
                               Const (Unit, (11,10--11,12)), (11,8--11,12)),
                            (9,4--10,10), NoneAtInvisible,
                            { LeadingKeyword = New (10,4--10,7)
                              InlineKeyword = None
                              EqualsRange = Some (10,11--10,12) }), (9,4--11,12))],
                     (3,4--11,12)), [],
                  Some
                    (ImplicitCtor
                       (None, [], Const (Unit, (2,6--2,8)), None,
                        PreXmlDoc ((2,6), FSharp.Compiler.Xml.XmlDocCollector),
                        (2,5--2,6), { AsKeyword = None })), (2,5--11,12),
                  { LeadingKeyword = Type (2,0--2,4)
                    EqualsRange = Some (2,9--2,10)
                    WithKeyword = None })], (2,0--11,12))], PreXmlDocEmpty, [],
          None, (2,0--12,0), { LeadingKeyword = None })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))
