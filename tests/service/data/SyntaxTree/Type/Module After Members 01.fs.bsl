ImplFile
  (ParsedImplFileInput
     ("/root/Type/Module After Members 01.fs", false, QualifiedNameOfFile Module,
      [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([], None, [], [ClassWithMembers],
                     PreXmlDoc ((4,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (4,5--4,21)),
                  ObjectModel
                    (Unspecified,
                     [ImplicitCtor
                        (None, [], Const (Unit, (4,21--4,23)), None,
                         PreXmlDoc ((4,21), FSharp.Compiler.Xml.XmlDocCollector),
                         (4,5--4,21), { AsKeyword = None });
                      Member
                        (SynBinding
                           (None, Normal, false, false, [],
                            PreXmlDoc ((5,4), FSharp.Compiler.Xml.XmlDocCollector),
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
                                 ([_; Method1], [(5,12--5,13)], [None; None]),
                               None, None,
                               Pats
                                 [Paren
                                    (Const (Unit, (5,20--5,22)), (5,20--5,22))],
                               None, (5,11--5,22)), None,
                            Const (Int32 1, (5,25--5,26)), (5,11--5,22),
                            NoneAtInvisible,
                            { LeadingKeyword = Member (5,4--5,10)
                              InlineKeyword = None
                              EqualsRange = Some (5,23--5,24) }), (5,4--5,26));
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
                                 ([_; Method2], [(6,12--6,13)], [None; None]),
                               None, None,
                               Pats
                                 [Paren
                                    (Const (Unit, (6,20--6,22)), (6,20--6,22))],
                               None, (6,11--6,22)), None,
                            Const (Int32 2, (6,25--6,26)), (6,11--6,22),
                            NoneAtInvisible,
                            { LeadingKeyword = Member (6,4--6,10)
                              InlineKeyword = None
                              EqualsRange = Some (6,23--6,24) }), (6,4--6,26))],
                     (5,4--6,26)), [],
                  Some
                    (ImplicitCtor
                       (None, [], Const (Unit, (4,21--4,23)), None,
                        PreXmlDoc ((4,21), FSharp.Compiler.Xml.XmlDocCollector),
                        (4,5--4,21), { AsKeyword = None })), (4,5--6,26),
                  { LeadingKeyword = Type (4,0--4,4)
                    EqualsRange = Some (4,24--4,25)
                    WithKeyword = None })], (4,0--6,26));
           NestedModule
             (SynComponentInfo
                ([], None, [], [InvalidModule],
                 PreXmlDoc ((8,4), FSharp.Compiler.Xml.XmlDocCollector), false,
                 None, (8,4--8,24)), false,
              [Let
                 (false,
                  [SynBinding
                     (None, Normal, false, false, [],
                      PreXmlDoc ((9,8), FSharp.Compiler.Xml.XmlDocCollector),
                      SynValData
                        (None, SynValInfo ([], SynArgInfo ([], false, None)),
                         None),
                      Named (SynIdent (helper, None), false, None, (9,12--9,18)),
                      None, Const (Int32 10, (9,21--9,23)), (9,12--9,18),
                      Yes (9,8--9,23), { LeadingKeyword = Let (9,8--9,11)
                                         InlineKeyword = None
                                         EqualsRange = Some (9,19--9,20) })],
                  (9,8--9,23), { InKeyword = None })], false, (8,4--9,23),
              { ModuleKeyword = Some (8,4--8,10)
                EqualsRange = Some (8,25--8,26) })],
          PreXmlDoc ((2,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (2,0--9,23), { LeadingKeyword = Module (2,0--2,6) })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [LineComment (1,0--1,45)] }, set []))

(8,4)-(8,10) parse error Modules cannot be nested inside types. Define modules at module or namespace level.
