SigFile
  (ParsedSigFileInput
     ("/root/NestedModule/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi",
      QualifiedNameOfFile
        RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl, [],
      [],
      [SynModuleOrNamespaceSig
         ([Microsoft; FSharp; Core], false, DeclaredNamespace,
          [Open
             (ModuleOrNamespace
                (SynLongIdent ([System], [], [None]), (4,5--4,11)), (4,0--4,11));
           Open
             (ModuleOrNamespace
                (SynLongIdent
                   ([System; Collections; Generic], [(5,11--5,12); (5,23--5,24)],
                    [None; None; None]), (5,5--5,31)), (5,0--5,31));
           Open
             (ModuleOrNamespace
                (SynLongIdent
                   ([Microsoft; FSharp; Core], [(6,14--6,15); (6,21--6,22)],
                    [None; None; None]), (6,5--6,26)), (6,0--6,26));
           Open
             (ModuleOrNamespace
                (SynLongIdent
                   ([Microsoft; FSharp; Collections],
                    [(7,14--7,15); (7,21--7,22)], [None; None; None]),
                 (7,5--7,33)), (7,0--7,33));
           Open
             (ModuleOrNamespace
                (SynLongIdent
                   ([System; Collections], [(8,11--8,12)], [None; None]),
                 (8,5--8,23)), (8,0--8,23));
           NestedModule
             (SynComponentInfo
                ([], None, [], [Tuple],
                 PreXmlDoc ((11,0), FSharp.Compiler.Xml.XmlDocCollector), false,
                 None, (11,0--11,12)), false,
              [Types
                 ([SynTypeDefnSig
                     (SynComponentInfo
                        ([],
                         Some
                           (PostfixList
                              ([SynTyparDecl
                                  ([], SynTypar (T1, None, false), [],
                                   { AmpersandRanges = [] });
                                SynTyparDecl
                                  ([], SynTypar (T2, None, false), [],
                                   { AmpersandRanges = [] });
                                SynTyparDecl
                                  ([], SynTypar (T3, None, false), [],
                                   { AmpersandRanges = [] });
                                SynTyparDecl
                                  ([], SynTypar (T4, None, false), [],
                                   { AmpersandRanges = [] })], [],
                               (13,14--13,31))), [], [Tuple],
                         PreXmlDoc ((13,4), FSharp.Compiler.Xml.XmlDocCollector),
                         true, None, (13,9--13,14)),
                      ObjectModel
                        (Unspecified,
                         [Interface
                            (LongIdent
                               (SynLongIdent
                                  ([IStructuralEquatable], [], [None])),
                             (14,8--14,38));
                          Interface
                            (LongIdent
                               (SynLongIdent
                                  ([IStructuralComparable], [], [None])),
                             (15,8--15,39));
                          Interface
                            (LongIdent
                               (SynLongIdent ([IComparable], [], [None])),
                             (16,8--16,29));
                          Member
                            (SynValSig
                               ([], SynIdent (new, None),
                                SynValTyparDecls (None, false),
                                Fun
                                  (Tuple
                                     (false,
                                      [Type
                                         (Var
                                            (SynTypar (T1, None, false),
                                             (17,14--17,17)));
                                       Star (17,18--17,19);
                                       Type
                                         (Var
                                            (SynTypar (T2, None, false),
                                             (17,20--17,23)));
                                       Star (17,24--17,25);
                                       Type
                                         (Var
                                            (SynTypar (T3, None, false),
                                             (17,26--17,29)));
                                       Star (17,30--17,31);
                                       Type
                                         (Var
                                            (SynTypar (T4, None, false),
                                             (17,32--17,35)))], (17,14--17,35)),
                                   App
                                     (LongIdent
                                        (SynLongIdent ([Tuple], [], [None])),
                                      Some (17,44--17,45),
                                      [Var
                                         (SynTypar (T1, None, false),
                                          (17,45--17,48));
                                       Var
                                         (SynTypar (T2, None, false),
                                          (17,49--17,52));
                                       Var
                                         (SynTypar (T3, None, false),
                                          (17,53--17,56));
                                       Var
                                         (SynTypar (T4, None, false),
                                          (17,57--17,60))],
                                      [(17,48--17,49); (17,52--17,53);
                                       (17,56--17,57)], Some (17,60--17,61),
                                      false, (17,39--17,61)), (17,14--17,61),
                                   { ArrowRange = (17,36--17,38) }),
                                SynValInfo
                                  ([[SynArgInfo ([], false, None);
                                     SynArgInfo ([], false, None);
                                     SynArgInfo ([], false, None);
                                     SynArgInfo ([], false, None)]],
                                   SynArgInfo ([], false, None)), false, false,
                                PreXmlDoc ((17,8), FSharp.Compiler.Xml.XmlDocCollector),
                                None, None, (17,8--17,61),
                                { LeadingKeyword = New (17,8--17,11)
                                  InlineKeyword = None
                                  WithKeyword = None
                                  EqualsRange = None }),
                             { IsInstance = false
                               IsDispatchSlot = false
                               IsOverrideOrExplicitImpl = false
                               IsFinal = false
                               GetterOrSetterIsCompilerGenerated = false
                               MemberKind = Constructor }, (17,8--17,61),
                             { GetSetKeywords = None });
                          Member
                            (SynValSig
                               ([], SynIdent (Item1, None),
                                SynValTyparDecls (None, true),
                                Var (SynTypar (T1, None, false), (18,23--18,26)),
                                SynValInfo ([], SynArgInfo ([], false, None)),
                                false, false,
                                PreXmlDoc ((18,8), FSharp.Compiler.Xml.XmlDocCollector),
                                None, None, (18,8--18,35),
                                { LeadingKeyword = Member (18,8--18,14)
                                  InlineKeyword = None
                                  WithKeyword = Some (18,27--18,31)
                                  EqualsRange = None }),
                             { IsInstance = true
                               IsDispatchSlot = false
                               IsOverrideOrExplicitImpl = false
                               IsFinal = false
                               GetterOrSetterIsCompilerGenerated = false
                               MemberKind = PropertyGet }, (18,8--18,35),
                             { GetSetKeywords = Some (Get (18,32--18,35)) });
                          Member
                            (SynValSig
                               ([], SynIdent (Item2, None),
                                SynValTyparDecls (None, true),
                                Var (SynTypar (T2, None, false), (19,23--19,26)),
                                SynValInfo ([], SynArgInfo ([], false, None)),
                                false, false,
                                PreXmlDoc ((19,8), FSharp.Compiler.Xml.XmlDocCollector),
                                None, None, (19,8--19,35),
                                { LeadingKeyword = Member (19,8--19,14)
                                  InlineKeyword = None
                                  WithKeyword = Some (19,27--19,31)
                                  EqualsRange = None }),
                             { IsInstance = true
                               IsDispatchSlot = false
                               IsOverrideOrExplicitImpl = false
                               IsFinal = false
                               GetterOrSetterIsCompilerGenerated = false
                               MemberKind = PropertyGet }, (19,8--19,35),
                             { GetSetKeywords = Some (Get (19,32--19,35)) });
                          Member
                            (SynValSig
                               ([], SynIdent (Item3, None),
                                SynValTyparDecls (None, true),
                                Var (SynTypar (T3, None, false), (20,23--20,26)),
                                SynValInfo ([], SynArgInfo ([], false, None)),
                                false, false,
                                PreXmlDoc ((20,8), FSharp.Compiler.Xml.XmlDocCollector),
                                None, None, (20,8--20,35),
                                { LeadingKeyword = Member (20,8--20,14)
                                  InlineKeyword = None
                                  WithKeyword = Some (20,27--20,31)
                                  EqualsRange = None }),
                             { IsInstance = true
                               IsDispatchSlot = false
                               IsOverrideOrExplicitImpl = false
                               IsFinal = false
                               GetterOrSetterIsCompilerGenerated = false
                               MemberKind = PropertyGet }, (20,8--20,35),
                             { GetSetKeywords = Some (Get (20,32--20,35)) });
                          Member
                            (SynValSig
                               ([], SynIdent (Item4, None),
                                SynValTyparDecls (None, true),
                                Var (SynTypar (T4, None, false), (21,23--21,26)),
                                SynValInfo ([], SynArgInfo ([], false, None)),
                                false, false,
                                PreXmlDoc ((21,8), FSharp.Compiler.Xml.XmlDocCollector),
                                None, None, (21,8--21,35),
                                { LeadingKeyword = Member (21,8--21,14)
                                  InlineKeyword = None
                                  WithKeyword = Some (21,27--21,31)
                                  EqualsRange = None }),
                             { IsInstance = true
                               IsDispatchSlot = false
                               IsOverrideOrExplicitImpl = false
                               IsFinal = false
                               GetterOrSetterIsCompilerGenerated = false
                               MemberKind = PropertyGet }, (21,8--21,35),
                             { GetSetKeywords = Some (Get (21,32--21,35)) })],
                         (14,8--21,35)), [], (13,9--21,35),
                      { LeadingKeyword = Type (13,4--13,8)
                        EqualsRange = Some (13,32--13,33)
                        WithKeyword = None })], (13,4--21,35))], (11,0--21,35),
              { ModuleKeyword = Some (11,0--11,6)
                EqualsRange = Some (11,13--11,14) });
           NestedModule
             (SynComponentInfo
                ([], None, [], [Choice],
                 PreXmlDoc ((24,0), FSharp.Compiler.Xml.XmlDocCollector), false,
                 None, (24,0--24,13)), false,
              [Types
                 ([SynTypeDefnSig
                     (SynComponentInfo
                        ([{ Attributes =
                             [{ TypeName =
                                 SynLongIdent ([StructuralEquality], [], [None])
                                TypeArgs = []
                                ArgExpr = Const (Unit, (27,6--27,24))
                                Target = None
                                AppliesToGetterAndSetter = false
                                Range = (27,6--27,24) };
                              { TypeName =
                                 SynLongIdent
                                   ([StructuralComparison], [], [None])
                                TypeArgs = []
                                ArgExpr = Const (Unit, (27,26--27,46))
                                Target = None
                                AppliesToGetterAndSetter = false
                                Range = (27,26--27,46) }]
                            Range = (27,4--27,48) };
                          { Attributes =
                             [{ TypeName =
                                 SynLongIdent ([CompiledName], [], [None])
                                TypeArgs = []
                                ArgExpr =
                                 Paren
                                   (Const
                                      (String
                                         ("FSharpChoice`6", Regular,
                                          (28,19--28,35)), (28,19--28,35)),
                                    (28,18--28,19), Some (28,35--28,36),
                                    (28,18--28,36))
                                Target = None
                                AppliesToGetterAndSetter = false
                                Range = (28,6--28,36) }]
                            Range = (28,4--28,38) }],
                         Some
                           (PostfixList
                              ([SynTyparDecl
                                  ([], SynTypar (T1, None, false), [],
                                   { AmpersandRanges = [] });
                                SynTyparDecl
                                  ([], SynTypar (T2, None, false), [],
                                   { AmpersandRanges = [] });
                                SynTyparDecl
                                  ([], SynTypar (T3, None, false), [],
                                   { AmpersandRanges = [] });
                                SynTyparDecl
                                  ([], SynTypar (T4, None, false), [],
                                   { AmpersandRanges = [] });
                                SynTyparDecl
                                  ([], SynTypar (T5, None, false), [],
                                   { AmpersandRanges = [] });
                                SynTyparDecl
                                  ([], SynTypar (T6, None, false), [],
                                   { AmpersandRanges = [] })], [],
                               (29,15--29,40))), [], [Choice],
                         PreXmlDoc ((27,4), FSharp.Compiler.Xml.XmlDocCollector),
                         true, None, (29,9--29,15)),
                      Simple
                        (Union
                           (None,
                            [SynUnionCase
                               ([], SynIdent (Choice1Of6, None),
                                Fields
                                  [SynField
                                     ([], false, None,
                                      Var
                                        (SynTypar (T1, None, false),
                                         (31,22--31,25)), false,
                                      PreXmlDoc ((31,22), FSharp.Compiler.Xml.XmlDocCollector),
                                      None, (31,22--31,25),
                                      { LeadingKeyword = None
                                        MutableKeyword = None })],
                                PreXmlDoc ((31,6), FSharp.Compiler.Xml.XmlDocCollector),
                                None, (30,6--31,25),
                                { BarRange = Some (31,6--31,7) });
                             SynUnionCase
                               ([], SynIdent (Choice2Of6, None),
                                Fields
                                  [SynField
                                     ([], false, None,
                                      Var
                                        (SynTypar (T2, None, false),
                                         (33,22--33,25)), false,
                                      PreXmlDoc ((33,22), FSharp.Compiler.Xml.XmlDocCollector),
                                      None, (33,22--33,25),
                                      { LeadingKeyword = None
                                        MutableKeyword = None })],
                                PreXmlDoc ((33,6), FSharp.Compiler.Xml.XmlDocCollector),
                                None, (32,6--33,25),
                                { BarRange = Some (33,6--33,7) });
                             SynUnionCase
                               ([], SynIdent (Choice3Of6, None),
                                Fields
                                  [SynField
                                     ([], false, None,
                                      Var
                                        (SynTypar (T3, None, false),
                                         (35,22--35,25)), false,
                                      PreXmlDoc ((35,22), FSharp.Compiler.Xml.XmlDocCollector),
                                      None, (35,22--35,25),
                                      { LeadingKeyword = None
                                        MutableKeyword = None })],
                                PreXmlDoc ((35,6), FSharp.Compiler.Xml.XmlDocCollector),
                                None, (34,6--35,25),
                                { BarRange = Some (35,6--35,7) });
                             SynUnionCase
                               ([], SynIdent (Choice4Of6, None),
                                Fields
                                  [SynField
                                     ([], false, None,
                                      Var
                                        (SynTypar (T4, None, false),
                                         (37,22--37,25)), false,
                                      PreXmlDoc ((37,22), FSharp.Compiler.Xml.XmlDocCollector),
                                      None, (37,22--37,25),
                                      { LeadingKeyword = None
                                        MutableKeyword = None })],
                                PreXmlDoc ((37,6), FSharp.Compiler.Xml.XmlDocCollector),
                                None, (36,6--37,25),
                                { BarRange = Some (37,6--37,7) });
                             SynUnionCase
                               ([], SynIdent (Choice5Of6, None),
                                Fields
                                  [SynField
                                     ([], false, None,
                                      Var
                                        (SynTypar (T5, None, false),
                                         (39,22--39,25)), false,
                                      PreXmlDoc ((39,22), FSharp.Compiler.Xml.XmlDocCollector),
                                      None, (39,22--39,25),
                                      { LeadingKeyword = None
                                        MutableKeyword = None })],
                                PreXmlDoc ((39,6), FSharp.Compiler.Xml.XmlDocCollector),
                                None, (38,6--39,25),
                                { BarRange = Some (39,6--39,7) });
                             SynUnionCase
                               ([], SynIdent (Choice6Of6, None),
                                Fields
                                  [SynField
                                     ([], false, None,
                                      Var
                                        (SynTypar (T6, None, false),
                                         (41,22--41,25)), false,
                                      PreXmlDoc ((41,22), FSharp.Compiler.Xml.XmlDocCollector),
                                      None, (41,22--41,25),
                                      { LeadingKeyword = None
                                        MutableKeyword = None })],
                                PreXmlDoc ((41,6), FSharp.Compiler.Xml.XmlDocCollector),
                                None, (40,6--41,25),
                                { BarRange = Some (41,6--41,7) })],
                            (30,6--41,25)), (30,6--41,25)), [], (26,4--41,25),
                      { LeadingKeyword = Type (29,4--29,8)
                        EqualsRange = Some (29,41--29,42)
                        WithKeyword = None })], (26,4--41,25))], (24,0--41,25),
              { ModuleKeyword = Some (24,0--24,6)
                EqualsRange = Some (24,14--24,15) });
           NestedModule
             (SynComponentInfo
                ([{ Attributes =
                     [{ TypeName = SynLongIdent ([AutoOpen], [], [None])
                        TypeArgs = []
                        ArgExpr = Const (Unit, (46,2--46,10))
                        Target = None
                        AppliesToGetterAndSetter = false
                        Range = (46,2--46,10) }]
                    Range = (46,0--46,12) }], None, [], [Operators],
                 PreXmlDoc ((46,0), FSharp.Compiler.Xml.XmlDocCollector), false,
                 None, (47,0--47,16)), false,
              [Types
                 ([SynTypeDefnSig
                     (SynComponentInfo
                        ([],
                         Some
                           (PostfixList
                              ([SynTyparDecl
                                  ([], SynTypar (T, None, false), [],
                                   { AmpersandRanges = [] })], [],
                               (49,16--49,20))), [], [[,]],
                         PreXmlDoc ((49,4), FSharp.Compiler.Xml.XmlDocCollector),
                         true, None, (49,9--49,16)),
                      Simple (None (49,9--61,26), (49,9--61,26)),
                      [Member
                         (SynValSig
                            ([{ Attributes =
                                 [{ TypeName =
                                     SynLongIdent ([CompiledName], [], [None])
                                    TypeArgs = []
                                    ArgExpr =
                                     Paren
                                       (Const
                                          (String
                                             ("Length1", Regular, (51,23--51,32)),
                                           (51,23--51,32)), (51,22--51,23),
                                        Some (51,32--51,33), (51,22--51,33))
                                    Target = None
                                    AppliesToGetterAndSetter = false
                                    Range = (51,10--51,33) }]
                                Range = (51,8--51,35) }],
                             SynIdent (Length1, None),
                             SynValTyparDecls (None, true),
                             LongIdent (SynLongIdent ([int], [], [None])),
                             SynValInfo ([], SynArgInfo ([], false, None)),
                             false, false,
                             PreXmlDoc ((51,8), FSharp.Compiler.Xml.XmlDocCollector),
                             None, None, (50,8--52,28),
                             { LeadingKeyword = Member (52,8--52,14)
                               InlineKeyword = None
                               WithKeyword = None
                               EqualsRange = None }),
                          { IsInstance = true
                            IsDispatchSlot = false
                            IsOverrideOrExplicitImpl = false
                            IsFinal = false
                            GetterOrSetterIsCompilerGenerated = false
                            MemberKind = PropertyGet }, (50,8--52,28),
                          { GetSetKeywords = None });
                       Member
                         (SynValSig
                            ([{ Attributes =
                                 [{ TypeName =
                                     SynLongIdent ([CompiledName], [], [None])
                                    TypeArgs = []
                                    ArgExpr =
                                     Paren
                                       (Const
                                          (String
                                             ("Length2", Regular, (54,23--54,32)),
                                           (54,23--54,32)), (54,22--54,23),
                                        Some (54,32--54,33), (54,22--54,33))
                                    Target = None
                                    AppliesToGetterAndSetter = false
                                    Range = (54,10--54,33) }]
                                Range = (54,8--54,35) }],
                             SynIdent (Length2, None),
                             SynValTyparDecls (None, true),
                             LongIdent (SynLongIdent ([int], [], [None])),
                             SynValInfo ([], SynArgInfo ([], false, None)),
                             false, false,
                             PreXmlDoc ((54,8), FSharp.Compiler.Xml.XmlDocCollector),
                             None, None, (53,8--55,28),
                             { LeadingKeyword = Member (55,8--55,14)
                               InlineKeyword = None
                               WithKeyword = None
                               EqualsRange = None }),
                          { IsInstance = true
                            IsDispatchSlot = false
                            IsOverrideOrExplicitImpl = false
                            IsFinal = false
                            GetterOrSetterIsCompilerGenerated = false
                            MemberKind = PropertyGet }, (53,8--55,28),
                          { GetSetKeywords = None });
                       Member
                         (SynValSig
                            ([{ Attributes =
                                 [{ TypeName =
                                     SynLongIdent ([CompiledName], [], [None])
                                    TypeArgs = []
                                    ArgExpr =
                                     Paren
                                       (Const
                                          (String
                                             ("Base1", Regular, (57,23--57,30)),
                                           (57,23--57,30)), (57,22--57,23),
                                        Some (57,30--57,31), (57,22--57,31))
                                    Target = None
                                    AppliesToGetterAndSetter = false
                                    Range = (57,10--57,31) }]
                                Range = (57,8--57,33) }], SynIdent (Base1, None),
                             SynValTyparDecls (None, true),
                             LongIdent (SynLongIdent ([int], [], [None])),
                             SynValInfo ([], SynArgInfo ([], false, None)),
                             false, false,
                             PreXmlDoc ((57,8), FSharp.Compiler.Xml.XmlDocCollector),
                             None, None, (56,8--58,26),
                             { LeadingKeyword = Member (58,8--58,14)
                               InlineKeyword = None
                               WithKeyword = None
                               EqualsRange = None }),
                          { IsInstance = true
                            IsDispatchSlot = false
                            IsOverrideOrExplicitImpl = false
                            IsFinal = false
                            GetterOrSetterIsCompilerGenerated = false
                            MemberKind = PropertyGet }, (56,8--58,26),
                          { GetSetKeywords = None });
                       Member
                         (SynValSig
                            ([{ Attributes =
                                 [{ TypeName =
                                     SynLongIdent ([CompiledName], [], [None])
                                    TypeArgs = []
                                    ArgExpr =
                                     Paren
                                       (Const
                                          (String
                                             ("Base2", Regular, (60,23--60,30)),
                                           (60,23--60,30)), (60,22--60,23),
                                        Some (60,30--60,31), (60,22--60,31))
                                    Target = None
                                    AppliesToGetterAndSetter = false
                                    Range = (60,10--60,31) }]
                                Range = (60,8--60,33) }], SynIdent (Base2, None),
                             SynValTyparDecls (None, true),
                             LongIdent (SynLongIdent ([int], [], [None])),
                             SynValInfo ([], SynArgInfo ([], false, None)),
                             false, false,
                             PreXmlDoc ((60,8), FSharp.Compiler.Xml.XmlDocCollector),
                             None, None, (59,8--61,26),
                             { LeadingKeyword = Member (61,8--61,14)
                               InlineKeyword = None
                               WithKeyword = None
                               EqualsRange = None }),
                          { IsInstance = true
                            IsDispatchSlot = false
                            IsOverrideOrExplicitImpl = false
                            IsFinal = false
                            GetterOrSetterIsCompilerGenerated = false
                            MemberKind = PropertyGet }, (59,8--61,26),
                          { GetSetKeywords = None })], (49,9--61,26),
                      { LeadingKeyword = Type (49,4--49,8)
                        EqualsRange = None
                        WithKeyword = Some (49,21--49,25) })], (49,4--61,26))],
              (45,0--61,26), { ModuleKeyword = Some (47,0--47,6)
                               EqualsRange = Some (47,17--47,18) })],
          PreXmlDocEmpty, [], None, (2,0--61,26),
          { LeadingKeyword = Namespace (2,0--2,9) })],
      { ConditionalDirectives = []
        CodeComments = [] }, set []))
