ImplFile
  (ParsedImplFileInput
     ("/root/Type/Module Inside Class With Constructor 01.fs", false,
      QualifiedNameOfFile Module, [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([], None, [], [MyClass],
                     PreXmlDoc ((4,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (4,5--4,12)),
                  ObjectModel
                    (Unspecified,
                     [ImplicitCtor
                        (None, [],
                         Paren
                           (Typed
                              (Named
                                 (SynIdent (x, None), false, None, (4,13--4,14)),
                               LongIdent (SynLongIdent ([int], [], [None])),
                               (4,13--4,19)), (4,12--4,20)), None,
                         PreXmlDoc ((4,12), FSharp.Compiler.Xml.XmlDocCollector),
                         (4,5--4,12), { AsKeyword = None });
                      LetBindings
                        ([SynBinding
                            (None, Normal, false, true, [],
                             PreXmlDoc ((5,4), FSharp.Compiler.Xml.XmlDocCollector),
                             SynValData
                               (None,
                                SynValInfo ([], SynArgInfo ([], false, None)),
                                None),
                             Named
                               (SynIdent (value, None), false, None,
                                (5,16--5,21)), None, Ident x, (5,16--5,21),
                             Yes (5,4--5,25),
                             { LeadingKeyword = Let (5,4--5,7)
                               InlineKeyword = None
                               EqualsRange = Some (5,22--5,23) })], false, false,
                         (5,4--5,25));
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
                                 ([_; Value], [(6,12--6,13)], [None; None]),
                               None, None, Pats [], None, (6,11--6,18)), None,
                            Ident value, (6,11--6,18), NoneAtInvisible,
                            { LeadingKeyword = Member (6,4--6,10)
                              InlineKeyword = None
                              EqualsRange = Some (6,19--6,20) }), (6,4--6,26))],
                     (5,4--6,26)), [],
                  Some
                    (ImplicitCtor
                       (None, [],
                        Paren
                          (Typed
                             (Named
                                (SynIdent (x, None), false, None, (4,13--4,14)),
                              LongIdent (SynLongIdent ([int], [], [None])),
                              (4,13--4,19)), (4,12--4,20)), None,
                        PreXmlDoc ((4,12), FSharp.Compiler.Xml.XmlDocCollector),
                        (4,5--4,12), { AsKeyword = None })), (4,5--6,26),
                  { LeadingKeyword = Type (4,0--4,4)
                    EqualsRange = Some (4,21--4,22)
                    WithKeyword = None })], (4,0--6,26));
           NestedModule
             (SynComponentInfo
                ([], None, [], [InternalModule],
                 PreXmlDoc ((7,4), FSharp.Compiler.Xml.XmlDocCollector), false,
                 None, (7,4--7,25)), false,
              [Let
                 (false,
                  [SynBinding
                     (None, Normal, false, false, [],
                      PreXmlDoc ((8,8), FSharp.Compiler.Xml.XmlDocCollector),
                      SynValData
                        (None, SynValInfo ([[]], SynArgInfo ([], false, None)),
                         None),
                      LongIdent
                        (SynLongIdent ([helper], [], [None]), None, None,
                         Pats [Paren (Const (Unit, (8,18--8,20)), (8,18--8,20))],
                         None, (8,12--8,20)), None,
                      Const (Int32 42, (8,23--8,25)), (8,12--8,20), NoneAtLet,
                      { LeadingKeyword = Let (8,8--8,11)
                        InlineKeyword = None
                        EqualsRange = Some (8,21--8,22) })], (8,8--8,25))],
              false, (7,4--8,25), { ModuleKeyword = Some (7,4--7,10)
                                    EqualsRange = Some (7,26--7,27) })],
          PreXmlDoc ((2,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (2,0--8,25), { LeadingKeyword = Module (2,0--2,6) })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [LineComment (1,0--1,61)] }, set []))

(7,4)-(7,10) parse warning Modules cannot be nested inside types. Define modules at module or namespace level.
