ImplFile
  (ParsedImplFileInput
     ("/root/Type/Let Inside Class 01.fs", false, QualifiedNameOfFile Module, [],
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
                        (None, [], Const (Unit, (4,12--4,14)), None,
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
                               (SynIdent (privateField, None), false, None,
                                (5,16--5,28)), None,
                             Const (Int32 10, (5,31--5,33)), (5,16--5,28),
                             Yes (5,4--5,33),
                             { LeadingKeyword = Let (5,4--5,7)
                               InlineKeyword = None
                               EqualsRange = Some (5,29--5,30) })], false, false,
                         (5,4--5,33));
                      LetBindings
                        ([SynBinding
                            (None, Normal, false, false, [],
                             PreXmlDoc ((6,4), FSharp.Compiler.Xml.XmlDocCollector),
                             SynValData
                               (None,
                                SynValInfo
                                  ([[SynArgInfo ([], false, Some x)]],
                                   SynArgInfo ([], false, None)), None),
                             LongIdent
                               (SynLongIdent ([helper], [], [None]), None, None,
                                Pats
                                  [Named
                                     (SynIdent (x, None), false, None,
                                      (6,15--6,16))], None, (6,8--6,16)), None,
                             App
                               (NonAtomic, false,
                                App
                                  (NonAtomic, true,
                                   LongIdent
                                     (false,
                                      SynLongIdent
                                        ([op_Multiply], [],
                                         [Some (OriginalNotation "*")]), None,
                                      (6,21--6,22)), Ident x, (6,19--6,22)),
                                Const (Int32 2, (6,23--6,24)), (6,19--6,24)),
                             (6,8--6,16), NoneAtLet,
                             { LeadingKeyword = Let (6,4--6,7)
                               InlineKeyword = None
                               EqualsRange = Some (6,17--6,18) })], false, false,
                         (6,4--6,24));
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
                                 ([_; GetValue], [(8,12--8,13)], [None; None]),
                               None, None,
                               Pats
                                 [Paren
                                    (Const (Unit, (8,21--8,23)), (8,21--8,23))],
                               None, (8,11--8,23)), None,
                            App
                              (NonAtomic, false, Ident helper,
                               Ident privateField, (8,26--8,45)), (8,11--8,23),
                            NoneAtInvisible,
                            { LeadingKeyword = Member (8,4--8,10)
                              InlineKeyword = None
                              EqualsRange = Some (8,24--8,25) }), (8,4--8,45));
                      Member
                        (SynBinding
                           (None, Normal, false, false, [],
                            PreXmlDoc ((9,4), FSharp.Compiler.Xml.XmlDocCollector),
                            SynValData
                              (Some { IsInstance = true
                                      IsDispatchSlot = false
                                      IsOverrideOrExplicitImpl = false
                                      IsFinal = false
                                      GetterOrSetterIsCompilerGenerated = false
                                      MemberKind = Member },
                               SynValInfo
                                 ([[SynArgInfo ([], false, None)];
                                   [SynArgInfo ([], false, Some v)]],
                                  SynArgInfo ([], false, None)), None),
                            LongIdent
                              (SynLongIdent
                                 ([_; SetValue], [(9,12--9,13)], [None; None]),
                               None, None,
                               Pats
                                 [Paren
                                    (Named
                                       (SynIdent (v, None), false, None,
                                        (9,22--9,23)), (9,21--9,24))], None,
                               (9,11--9,24)), None,
                            LongIdentSet
                              (SynLongIdent ([privateField], [], [None]),
                               Ident v, (9,27--9,44)), (9,11--9,24),
                            NoneAtInvisible,
                            { LeadingKeyword = Member (9,4--9,10)
                              InlineKeyword = None
                              EqualsRange = Some (9,25--9,26) }), (9,4--9,44))],
                     (5,4--9,44)), [],
                  Some
                    (ImplicitCtor
                       (None, [], Const (Unit, (4,12--4,14)), None,
                        PreXmlDoc ((4,12), FSharp.Compiler.Xml.XmlDocCollector),
                        (4,5--4,12), { AsKeyword = None })), (4,5--9,44),
                  { LeadingKeyword = Type (4,0--4,4)
                    EqualsRange = Some (4,15--4,16)
                    WithKeyword = None })], (4,0--9,44))],
          PreXmlDoc ((2,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (2,0--9,44), { LeadingKeyword = Module (2,0--2,6) })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [LineComment (1,0--1,59)] }, set []))
