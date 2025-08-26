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
                     [Member
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
                            PreXmlDoc ((8,4), FSharp.Compiler.Xml.XmlDocCollector),
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
                                 ([_; AnotherMethod], [(8,12--8,13)],
                                  [None; None]), None, None,
                               Pats
                                 [Paren
                                    (Const (Unit, (8,26--8,28)), (8,26--8,28))],
                               None, (8,11--8,28)), None,
                            Const (Int32 2, (8,31--8,32)), (8,11--8,28),
                            NoneAtInvisible,
                            { LeadingKeyword = Member (8,4--8,10)
                              InlineKeyword = None
                              EqualsRange = Some (8,29--8,30) }), (8,4--8,32))],
                     (6,4--8,32)), [], None, (4,5--8,32),
                  { LeadingKeyword = Type (4,0--4,4)
                    EqualsRange = Some (4,12--4,13)
                    WithKeyword = None })], (4,0--8,32))],
          PreXmlDoc ((2,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (2,0--8,32), { LeadingKeyword = Module (2,0--2,6) })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments =
         [LineComment (1,0--1,50); LineComment (5,4--5,44);
          BlockComment (7,4--7,25)] }, set []))
