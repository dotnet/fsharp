ImplFile
  (ParsedImplFileInput
     ("/root/Type/Comments With Keywords 01.fs", false,
      QualifiedNameOfFile Module, [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([], None, [], [MyType],
                     PreXmlDoc ((4,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (4,5--4,11)),
                  ObjectModel
                    (Unspecified,
                     [ImplicitCtor
                        (None, [], Const (Unit, (4,11--4,13)), None,
                         PreXmlDoc ((4,11), FSharp.Compiler.Xml.XmlDocCollector),
                         (4,5--4,11), { AsKeyword = None });
                      Member
                        (SynBinding
                           (None, Normal, false, false, [],
                            PreXmlDoc ((6,4), FSharp.Compiler.Xml.XmlDocCollector),
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
                                 ([_; Method], [(6,12--6,13)], [None; None]),
                               None, None,
                               Pats
                                 [Paren
                                    (Const (Unit, (6,19--6,21)), (6,19--6,21))],
                               None, (6,11--6,21)), None,
                            Const (Int32 1, (6,24--6,25)), (6,11--6,21),
                            NoneAtInvisible,
                            { LeadingKeyword = Member (6,4--6,10)
                              InlineKeyword = None
                              EqualsRange = Some (6,22--6,23) }), (6,4--6,25));
                      Member
                        (SynBinding
                           (None, Normal, false, false, [],
                            PreXmlDoc ((14,4), FSharp.Compiler.Xml.XmlDocCollector),
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
                                 ([_; AnotherMethod], [(14,12--14,13)],
                                  [None; None]), None, None,
                               Pats
                                 [Paren
                                    (Const (Unit, (14,26--14,28)),
                                     (14,26--14,28))], None, (14,11--14,28)),
                            None, Const (Int32 2, (14,31--14,32)),
                            (14,11--14,28), NoneAtInvisible,
                            { LeadingKeyword = Member (14,4--14,10)
                              InlineKeyword = None
                              EqualsRange = Some (14,29--14,30) }),
                         (14,4--14,32));
                      Member
                        (SynBinding
                           (None, Normal, false, false, [],
                            PreXmlDoc ((17,4), FSharp.Compiler.Xml.XmlDocCollector),
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
                                 ([_; DocumentedMethod], [(17,12--17,13)],
                                  [None; None]), None, None,
                               Pats
                                 [Paren
                                    (Const (Unit, (17,29--17,31)),
                                     (17,29--17,31))], None, (17,11--17,31)),
                            None, Const (Int32 3, (17,34--17,35)), (16,4--17,31),
                            NoneAtInvisible,
                            { LeadingKeyword = Member (17,4--17,10)
                              InlineKeyword = None
                              EqualsRange = Some (17,32--17,33) }),
                         (16,4--17,35))], (6,4--17,35)), [],
                  Some
                    (ImplicitCtor
                       (None, [], Const (Unit, (4,11--4,13)), None,
                        PreXmlDoc ((4,11), FSharp.Compiler.Xml.XmlDocCollector),
                        (4,5--4,11), { AsKeyword = None })), (4,5--17,35),
                  { LeadingKeyword = Type (4,0--4,4)
                    EqualsRange = Some (4,14--4,15)
                    WithKeyword = None })], (4,0--17,35))],
          PreXmlDoc ((2,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (2,0--17,35), { LeadingKeyword = Module (2,0--2,6) })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments =
         [LineComment (1,0--1,58); LineComment (5,4--5,44);
          BlockComment (8,4--13,6)] }, set []))
