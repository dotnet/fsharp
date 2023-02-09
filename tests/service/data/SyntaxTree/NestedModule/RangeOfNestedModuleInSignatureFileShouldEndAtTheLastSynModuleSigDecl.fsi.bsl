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
                (SynLongIdent ([System], [], [None]),
                 /root/NestedModule/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (4,5--4,11)),
              /root/NestedModule/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (4,0--4,11));
           Open
             (ModuleOrNamespace
                (SynLongIdent
                   ([System; Collections; Generic],
                    [/root/NestedModule/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (5,11--5,12);
                     /root/NestedModule/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (5,23--5,24)],
                    [None; None; None]),
                 /root/NestedModule/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (5,5--5,31)),
              /root/NestedModule/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (5,0--5,31));
           Open
             (ModuleOrNamespace
                (SynLongIdent
                   ([Microsoft; FSharp; Core],
                    [/root/NestedModule/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (6,14--6,15);
                     /root/NestedModule/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (6,21--6,22)],
                    [None; None; None]),
                 /root/NestedModule/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (6,5--6,26)),
              /root/NestedModule/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (6,0--6,26));
           Open
             (ModuleOrNamespace
                (SynLongIdent
                   ([Microsoft; FSharp; Collections],
                    [/root/NestedModule/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (7,14--7,15);
                     /root/NestedModule/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (7,21--7,22)],
                    [None; None; None]),
                 /root/NestedModule/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (7,5--7,33)),
              /root/NestedModule/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (7,0--7,33));
           Open
             (ModuleOrNamespace
                (SynLongIdent
                   ([System; Collections],
                    [/root/NestedModule/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (8,11--8,12)],
                    [None; None]),
                 /root/NestedModule/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (8,5--8,23)),
              /root/NestedModule/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (8,0--8,23));
           NestedModule
             (SynComponentInfo
                ([], None, [], [Tuple],
                 PreXmlDoc ((11,0), FSharp.Compiler.Xml.XmlDocCollector), false,
                 None,
                 /root/NestedModule/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (11,0--11,12)),
              false,
              [Types
                 ([SynTypeDefnSig
                     (SynComponentInfo
                        ([],
                         Some
                           (PostfixList
                              ([SynTyparDecl ([], SynTypar (T1, None, false));
                                SynTyparDecl ([], SynTypar (T2, None, false));
                                SynTyparDecl ([], SynTypar (T3, None, false));
                                SynTyparDecl ([], SynTypar (T4, None, false))],
                               [],
                               /root/NestedModule/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (13,14--13,31))),
                         [], [Tuple],
                         PreXmlDoc ((13,4), FSharp.Compiler.Xml.XmlDocCollector),
                         true, None,
                         /root/NestedModule/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (13,9--13,14)),
                      ObjectModel
                        (Unspecified,
                         [Interface
                            (LongIdent
                               (SynLongIdent
                                  ([IStructuralEquatable], [], [None])),
                             /root/NestedModule/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (14,8--14,38));
                          Interface
                            (LongIdent
                               (SynLongIdent
                                  ([IStructuralComparable], [], [None])),
                             /root/NestedModule/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (15,8--15,39));
                          Interface
                            (LongIdent
                               (SynLongIdent ([IComparable], [], [None])),
                             /root/NestedModule/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (16,8--16,29));
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
                                             /root/NestedModule/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (17,14--17,17)));
                                       Star
                                         /root/NestedModule/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (17,18--17,19);
                                       Type
                                         (Var
                                            (SynTypar (T2, None, false),
                                             /root/NestedModule/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (17,20--17,23)));
                                       Star
                                         /root/NestedModule/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (17,24--17,25);
                                       Type
                                         (Var
                                            (SynTypar (T3, None, false),
                                             /root/NestedModule/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (17,26--17,29)));
                                       Star
                                         /root/NestedModule/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (17,30--17,31);
                                       Type
                                         (Var
                                            (SynTypar (T4, None, false),
                                             /root/NestedModule/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (17,32--17,35)))],
                                      /root/NestedModule/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (17,14--17,35)),
                                   App
                                     (LongIdent
                                        (SynLongIdent ([Tuple], [], [None])),
                                      Some
                                        /root/NestedModule/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (17,44--17,45),
                                      [Var
                                         (SynTypar (T1, None, false),
                                          /root/NestedModule/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (17,45--17,48));
                                       Var
                                         (SynTypar (T2, None, false),
                                          /root/NestedModule/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (17,49--17,52));
                                       Var
                                         (SynTypar (T3, None, false),
                                          /root/NestedModule/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (17,53--17,56));
                                       Var
                                         (SynTypar (T4, None, false),
                                          /root/NestedModule/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (17,57--17,60))],
                                      [/root/NestedModule/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (17,48--17,49);
                                       /root/NestedModule/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (17,52--17,53);
                                       /root/NestedModule/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (17,56--17,57)],
                                      Some
                                        /root/NestedModule/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (17,60--17,61),
                                      false,
                                      /root/NestedModule/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (17,39--17,61)),
                                   /root/NestedModule/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (17,14--17,61),
                                   { ArrowRange =
                                      /root/NestedModule/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (17,36--17,38) }),
                                SynValInfo
                                  ([[SynArgInfo ([], false, None);
                                     SynArgInfo ([], false, None);
                                     SynArgInfo ([], false, None);
                                     SynArgInfo ([], false, None)]],
                                   SynArgInfo ([], false, None)), false, false,
                                PreXmlDoc ((17,8), FSharp.Compiler.Xml.XmlDocCollector),
                                None, None,
                                /root/NestedModule/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (17,8--17,61),
                                { LeadingKeyword =
                                   New
                                     /root/NestedModule/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (17,8--17,11)
                                  InlineKeyword = None
                                  WithKeyword = None
                                  EqualsRange = None }),
                             { IsInstance = false
                               IsDispatchSlot = false
                               IsOverrideOrExplicitImpl = false
                               IsFinal = false
                               GetterOrSetterIsCompilerGenerated = false
                               MemberKind = Constructor },
                             /root/NestedModule/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (17,8--17,61),
                             { GetSetKeywords = None });
                          Member
                            (SynValSig
                               ([], SynIdent (Item1, None),
                                SynValTyparDecls (None, true),
                                Var
                                  (SynTypar (T1, None, false),
                                   /root/NestedModule/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (18,23--18,26)),
                                SynValInfo ([], SynArgInfo ([], false, None)),
                                false, false,
                                PreXmlDoc ((18,8), FSharp.Compiler.Xml.XmlDocCollector),
                                None, None,
                                /root/NestedModule/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (18,8--18,35),
                                { LeadingKeyword =
                                   Member
                                     /root/NestedModule/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (18,8--18,14)
                                  InlineKeyword = None
                                  WithKeyword =
                                   Some
                                     /root/NestedModule/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (18,27--18,31)
                                  EqualsRange = None }),
                             { IsInstance = true
                               IsDispatchSlot = false
                               IsOverrideOrExplicitImpl = false
                               IsFinal = false
                               GetterOrSetterIsCompilerGenerated = false
                               MemberKind = PropertyGet },
                             /root/NestedModule/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (18,8--18,35),
                             { GetSetKeywords =
                                Some
                                  (Get
                                     /root/NestedModule/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (18,32--18,35)) });
                          Member
                            (SynValSig
                               ([], SynIdent (Item2, None),
                                SynValTyparDecls (None, true),
                                Var
                                  (SynTypar (T2, None, false),
                                   /root/NestedModule/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (19,23--19,26)),
                                SynValInfo ([], SynArgInfo ([], false, None)),
                                false, false,
                                PreXmlDoc ((19,8), FSharp.Compiler.Xml.XmlDocCollector),
                                None, None,
                                /root/NestedModule/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (19,8--19,35),
                                { LeadingKeyword =
                                   Member
                                     /root/NestedModule/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (19,8--19,14)
                                  InlineKeyword = None
                                  WithKeyword =
                                   Some
                                     /root/NestedModule/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (19,27--19,31)
                                  EqualsRange = None }),
                             { IsInstance = true
                               IsDispatchSlot = false
                               IsOverrideOrExplicitImpl = false
                               IsFinal = false
                               GetterOrSetterIsCompilerGenerated = false
                               MemberKind = PropertyGet },
                             /root/NestedModule/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (19,8--19,35),
                             { GetSetKeywords =
                                Some
                                  (Get
                                     /root/NestedModule/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (19,32--19,35)) });
                          Member
                            (SynValSig
                               ([], SynIdent (Item3, None),
                                SynValTyparDecls (None, true),
                                Var
                                  (SynTypar (T3, None, false),
                                   /root/NestedModule/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (20,23--20,26)),
                                SynValInfo ([], SynArgInfo ([], false, None)),
                                false, false,
                                PreXmlDoc ((20,8), FSharp.Compiler.Xml.XmlDocCollector),
                                None, None,
                                /root/NestedModule/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (20,8--20,35),
                                { LeadingKeyword =
                                   Member
                                     /root/NestedModule/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (20,8--20,14)
                                  InlineKeyword = None
                                  WithKeyword =
                                   Some
                                     /root/NestedModule/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (20,27--20,31)
                                  EqualsRange = None }),
                             { IsInstance = true
                               IsDispatchSlot = false
                               IsOverrideOrExplicitImpl = false
                               IsFinal = false
                               GetterOrSetterIsCompilerGenerated = false
                               MemberKind = PropertyGet },
                             /root/NestedModule/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (20,8--20,35),
                             { GetSetKeywords =
                                Some
                                  (Get
                                     /root/NestedModule/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (20,32--20,35)) });
                          Member
                            (SynValSig
                               ([], SynIdent (Item4, None),
                                SynValTyparDecls (None, true),
                                Var
                                  (SynTypar (T4, None, false),
                                   /root/NestedModule/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (21,23--21,26)),
                                SynValInfo ([], SynArgInfo ([], false, None)),
                                false, false,
                                PreXmlDoc ((21,8), FSharp.Compiler.Xml.XmlDocCollector),
                                None, None,
                                /root/NestedModule/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (21,8--21,35),
                                { LeadingKeyword =
                                   Member
                                     /root/NestedModule/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (21,8--21,14)
                                  InlineKeyword = None
                                  WithKeyword =
                                   Some
                                     /root/NestedModule/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (21,27--21,31)
                                  EqualsRange = None }),
                             { IsInstance = true
                               IsDispatchSlot = false
                               IsOverrideOrExplicitImpl = false
                               IsFinal = false
                               GetterOrSetterIsCompilerGenerated = false
                               MemberKind = PropertyGet },
                             /root/NestedModule/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (21,8--21,35),
                             { GetSetKeywords =
                                Some
                                  (Get
                                     /root/NestedModule/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (21,32--21,35)) })],
                         /root/NestedModule/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (14,8--21,35)),
                      [],
                      /root/NestedModule/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (13,9--21,35),
                      { LeadingKeyword =
                         Type
                           /root/NestedModule/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (13,4--13,8)
                        EqualsRange =
                         Some
                           /root/NestedModule/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (13,32--13,33)
                        WithKeyword = None })],
                  /root/NestedModule/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (13,4--21,35))],
              /root/NestedModule/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (11,0--21,35),
              { ModuleKeyword =
                 Some
                   /root/NestedModule/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (11,0--11,6)
                EqualsRange =
                 Some
                   /root/NestedModule/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (11,13--11,14) });
           NestedModule
             (SynComponentInfo
                ([], None, [], [Choice],
                 PreXmlDoc ((24,0), FSharp.Compiler.Xml.XmlDocCollector), false,
                 None,
                 /root/NestedModule/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (24,0--24,13)),
              false,
              [Types
                 ([SynTypeDefnSig
                     (SynComponentInfo
                        ([{ Attributes =
                             [{ TypeName =
                                 SynLongIdent ([StructuralEquality], [], [None])
                                ArgExpr =
                                 Const
                                   (Unit,
                                    /root/NestedModule/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (27,6--27,24))
                                Target = None
                                AppliesToGetterAndSetter = false
                                Range =
                                 /root/NestedModule/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (27,6--27,24) };
                              { TypeName =
                                 SynLongIdent
                                   ([StructuralComparison], [], [None])
                                ArgExpr =
                                 Const
                                   (Unit,
                                    /root/NestedModule/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (27,26--27,46))
                                Target = None
                                AppliesToGetterAndSetter = false
                                Range =
                                 /root/NestedModule/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (27,26--27,46) }]
                            Range =
                             /root/NestedModule/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (27,4--27,48) };
                          { Attributes =
                             [{ TypeName =
                                 SynLongIdent ([CompiledName], [], [None])
                                ArgExpr =
                                 Paren
                                   (Const
                                      (String
                                         ("FSharpChoice`6", Regular,
                                          /root/NestedModule/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (28,19--28,35)),
                                       /root/NestedModule/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (28,19--28,35)),
                                    /root/NestedModule/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (28,18--28,19),
                                    Some
                                      /root/NestedModule/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (28,35--28,36),
                                    /root/NestedModule/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (28,18--28,36))
                                Target = None
                                AppliesToGetterAndSetter = false
                                Range =
                                 /root/NestedModule/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (28,6--28,36) }]
                            Range =
                             /root/NestedModule/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (28,4--28,38) }],
                         Some
                           (PostfixList
                              ([SynTyparDecl ([], SynTypar (T1, None, false));
                                SynTyparDecl ([], SynTypar (T2, None, false));
                                SynTyparDecl ([], SynTypar (T3, None, false));
                                SynTyparDecl ([], SynTypar (T4, None, false));
                                SynTyparDecl ([], SynTypar (T5, None, false));
                                SynTyparDecl ([], SynTypar (T6, None, false))],
                               [],
                               /root/NestedModule/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (29,15--29,40))),
                         [], [Choice],
                         PreXmlDoc ((27,4), FSharp.Compiler.Xml.XmlDocCollector),
                         true, None,
                         /root/NestedModule/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (29,9--29,15)),
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
                                         /root/NestedModule/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (31,22--31,25)),
                                      false,
                                      PreXmlDoc ((31,22), FSharp.Compiler.Xml.XmlDocCollector),
                                      None,
                                      /root/NestedModule/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (31,22--31,25),
                                      { LeadingKeyword = None })],
                                PreXmlDoc ((31,6), FSharp.Compiler.Xml.XmlDocCollector),
                                None,
                                /root/NestedModule/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (30,6--31,25),
                                { BarRange =
                                   Some
                                     /root/NestedModule/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (31,6--31,7) });
                             SynUnionCase
                               ([], SynIdent (Choice2Of6, None),
                                Fields
                                  [SynField
                                     ([], false, None,
                                      Var
                                        (SynTypar (T2, None, false),
                                         /root/NestedModule/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (33,22--33,25)),
                                      false,
                                      PreXmlDoc ((33,22), FSharp.Compiler.Xml.XmlDocCollector),
                                      None,
                                      /root/NestedModule/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (33,22--33,25),
                                      { LeadingKeyword = None })],
                                PreXmlDoc ((33,6), FSharp.Compiler.Xml.XmlDocCollector),
                                None,
                                /root/NestedModule/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (32,6--33,25),
                                { BarRange =
                                   Some
                                     /root/NestedModule/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (33,6--33,7) });
                             SynUnionCase
                               ([], SynIdent (Choice3Of6, None),
                                Fields
                                  [SynField
                                     ([], false, None,
                                      Var
                                        (SynTypar (T3, None, false),
                                         /root/NestedModule/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (35,22--35,25)),
                                      false,
                                      PreXmlDoc ((35,22), FSharp.Compiler.Xml.XmlDocCollector),
                                      None,
                                      /root/NestedModule/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (35,22--35,25),
                                      { LeadingKeyword = None })],
                                PreXmlDoc ((35,6), FSharp.Compiler.Xml.XmlDocCollector),
                                None,
                                /root/NestedModule/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (34,6--35,25),
                                { BarRange =
                                   Some
                                     /root/NestedModule/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (35,6--35,7) });
                             SynUnionCase
                               ([], SynIdent (Choice4Of6, None),
                                Fields
                                  [SynField
                                     ([], false, None,
                                      Var
                                        (SynTypar (T4, None, false),
                                         /root/NestedModule/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (37,22--37,25)),
                                      false,
                                      PreXmlDoc ((37,22), FSharp.Compiler.Xml.XmlDocCollector),
                                      None,
                                      /root/NestedModule/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (37,22--37,25),
                                      { LeadingKeyword = None })],
                                PreXmlDoc ((37,6), FSharp.Compiler.Xml.XmlDocCollector),
                                None,
                                /root/NestedModule/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (36,6--37,25),
                                { BarRange =
                                   Some
                                     /root/NestedModule/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (37,6--37,7) });
                             SynUnionCase
                               ([], SynIdent (Choice5Of6, None),
                                Fields
                                  [SynField
                                     ([], false, None,
                                      Var
                                        (SynTypar (T5, None, false),
                                         /root/NestedModule/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (39,22--39,25)),
                                      false,
                                      PreXmlDoc ((39,22), FSharp.Compiler.Xml.XmlDocCollector),
                                      None,
                                      /root/NestedModule/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (39,22--39,25),
                                      { LeadingKeyword = None })],
                                PreXmlDoc ((39,6), FSharp.Compiler.Xml.XmlDocCollector),
                                None,
                                /root/NestedModule/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (38,6--39,25),
                                { BarRange =
                                   Some
                                     /root/NestedModule/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (39,6--39,7) });
                             SynUnionCase
                               ([], SynIdent (Choice6Of6, None),
                                Fields
                                  [SynField
                                     ([], false, None,
                                      Var
                                        (SynTypar (T6, None, false),
                                         /root/NestedModule/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (41,22--41,25)),
                                      false,
                                      PreXmlDoc ((41,22), FSharp.Compiler.Xml.XmlDocCollector),
                                      None,
                                      /root/NestedModule/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (41,22--41,25),
                                      { LeadingKeyword = None })],
                                PreXmlDoc ((41,6), FSharp.Compiler.Xml.XmlDocCollector),
                                None,
                                /root/NestedModule/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (40,6--41,25),
                                { BarRange =
                                   Some
                                     /root/NestedModule/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (41,6--41,7) })],
                            /root/NestedModule/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (30,6--41,25)),
                         /root/NestedModule/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (30,6--41,25)),
                      [],
                      /root/NestedModule/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (26,4--41,25),
                      { LeadingKeyword =
                         Type
                           /root/NestedModule/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (29,4--29,8)
                        EqualsRange =
                         Some
                           /root/NestedModule/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (29,41--29,42)
                        WithKeyword = None })],
                  /root/NestedModule/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (26,4--41,25))],
              /root/NestedModule/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (24,0--41,25),
              { ModuleKeyword =
                 Some
                   /root/NestedModule/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (24,0--24,6)
                EqualsRange =
                 Some
                   /root/NestedModule/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (24,14--24,15) });
           NestedModule
             (SynComponentInfo
                ([{ Attributes =
                     [{ TypeName = SynLongIdent ([AutoOpen], [], [None])
                        ArgExpr =
                         Const
                           (Unit,
                            /root/NestedModule/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (46,2--46,10))
                        Target = None
                        AppliesToGetterAndSetter = false
                        Range =
                         /root/NestedModule/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (46,2--46,10) }]
                    Range =
                     /root/NestedModule/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (46,0--46,12) }],
                 None, [], [Operators],
                 PreXmlDoc ((46,0), FSharp.Compiler.Xml.XmlDocCollector), false,
                 None,
                 /root/NestedModule/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (47,0--47,16)),
              false,
              [Types
                 ([SynTypeDefnSig
                     (SynComponentInfo
                        ([],
                         Some
                           (PostfixList
                              ([SynTyparDecl ([], SynTypar (T, None, false))],
                               [],
                               /root/NestedModule/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (49,16--49,20))),
                         [], [[,]],
                         PreXmlDoc ((49,4), FSharp.Compiler.Xml.XmlDocCollector),
                         true, None,
                         /root/NestedModule/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (49,9--49,16)),
                      Simple
                        (None
                           /root/NestedModule/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (49,9--61,26),
                         /root/NestedModule/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (49,9--61,26)),
                      [Member
                         (SynValSig
                            ([{ Attributes =
                                 [{ TypeName =
                                     SynLongIdent ([CompiledName], [], [None])
                                    ArgExpr =
                                     Paren
                                       (Const
                                          (String
                                             ("Length1", Regular,
                                              /root/NestedModule/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (50,23--50,32)),
                                           /root/NestedModule/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (50,23--50,32)),
                                        /root/NestedModule/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (50,22--50,23),
                                        Some
                                          /root/NestedModule/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (50,32--50,33),
                                        /root/NestedModule/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (50,22--50,33))
                                    Target = None
                                    AppliesToGetterAndSetter = false
                                    Range =
                                     /root/NestedModule/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (50,10--50,33) }]
                                Range =
                                 /root/NestedModule/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (50,8--50,35) }],
                             SynIdent (Length1, None),
                             SynValTyparDecls (None, true),
                             LongIdent (SynLongIdent ([int], [], [None])),
                             SynValInfo ([], SynArgInfo ([], false, None)),
                             false, false,
                             PreXmlDoc ((50,8), FSharp.Compiler.Xml.XmlDocCollector),
                             None, None,
                             /root/NestedModule/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (50,8--52,28),
                             { LeadingKeyword =
                                Member
                                  /root/NestedModule/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (52,8--52,14)
                               InlineKeyword = None
                               WithKeyword = None
                               EqualsRange = None }),
                          { IsInstance = true
                            IsDispatchSlot = false
                            IsOverrideOrExplicitImpl = false
                            IsFinal = false
                            GetterOrSetterIsCompilerGenerated = false
                            MemberKind = PropertyGet },
                          /root/NestedModule/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (50,8--52,28),
                          { GetSetKeywords = None });
                       Member
                         (SynValSig
                            ([{ Attributes =
                                 [{ TypeName =
                                     SynLongIdent ([CompiledName], [], [None])
                                    ArgExpr =
                                     Paren
                                       (Const
                                          (String
                                             ("Length2", Regular,
                                              /root/NestedModule/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (53,23--53,32)),
                                           /root/NestedModule/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (53,23--53,32)),
                                        /root/NestedModule/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (53,22--53,23),
                                        Some
                                          /root/NestedModule/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (53,32--53,33),
                                        /root/NestedModule/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (53,22--53,33))
                                    Target = None
                                    AppliesToGetterAndSetter = false
                                    Range =
                                     /root/NestedModule/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (53,10--53,33) }]
                                Range =
                                 /root/NestedModule/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (53,8--53,35) }],
                             SynIdent (Length2, None),
                             SynValTyparDecls (None, true),
                             LongIdent (SynLongIdent ([int], [], [None])),
                             SynValInfo ([], SynArgInfo ([], false, None)),
                             false, false,
                             PreXmlDoc ((53,8), FSharp.Compiler.Xml.XmlDocCollector),
                             None, None,
                             /root/NestedModule/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (53,8--55,28),
                             { LeadingKeyword =
                                Member
                                  /root/NestedModule/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (55,8--55,14)
                               InlineKeyword = None
                               WithKeyword = None
                               EqualsRange = None }),
                          { IsInstance = true
                            IsDispatchSlot = false
                            IsOverrideOrExplicitImpl = false
                            IsFinal = false
                            GetterOrSetterIsCompilerGenerated = false
                            MemberKind = PropertyGet },
                          /root/NestedModule/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (53,8--55,28),
                          { GetSetKeywords = None });
                       Member
                         (SynValSig
                            ([{ Attributes =
                                 [{ TypeName =
                                     SynLongIdent ([CompiledName], [], [None])
                                    ArgExpr =
                                     Paren
                                       (Const
                                          (String
                                             ("Base1", Regular,
                                              /root/NestedModule/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (56,23--56,30)),
                                           /root/NestedModule/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (56,23--56,30)),
                                        /root/NestedModule/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (56,22--56,23),
                                        Some
                                          /root/NestedModule/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (56,30--56,31),
                                        /root/NestedModule/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (56,22--56,31))
                                    Target = None
                                    AppliesToGetterAndSetter = false
                                    Range =
                                     /root/NestedModule/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (56,10--56,31) }]
                                Range =
                                 /root/NestedModule/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (56,8--56,33) }],
                             SynIdent (Base1, None),
                             SynValTyparDecls (None, true),
                             LongIdent (SynLongIdent ([int], [], [None])),
                             SynValInfo ([], SynArgInfo ([], false, None)),
                             false, false,
                             PreXmlDoc ((56,8), FSharp.Compiler.Xml.XmlDocCollector),
                             None, None,
                             /root/NestedModule/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (56,8--58,26),
                             { LeadingKeyword =
                                Member
                                  /root/NestedModule/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (58,8--58,14)
                               InlineKeyword = None
                               WithKeyword = None
                               EqualsRange = None }),
                          { IsInstance = true
                            IsDispatchSlot = false
                            IsOverrideOrExplicitImpl = false
                            IsFinal = false
                            GetterOrSetterIsCompilerGenerated = false
                            MemberKind = PropertyGet },
                          /root/NestedModule/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (56,8--58,26),
                          { GetSetKeywords = None });
                       Member
                         (SynValSig
                            ([{ Attributes =
                                 [{ TypeName =
                                     SynLongIdent ([CompiledName], [], [None])
                                    ArgExpr =
                                     Paren
                                       (Const
                                          (String
                                             ("Base2", Regular,
                                              /root/NestedModule/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (59,23--59,30)),
                                           /root/NestedModule/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (59,23--59,30)),
                                        /root/NestedModule/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (59,22--59,23),
                                        Some
                                          /root/NestedModule/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (59,30--59,31),
                                        /root/NestedModule/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (59,22--59,31))
                                    Target = None
                                    AppliesToGetterAndSetter = false
                                    Range =
                                     /root/NestedModule/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (59,10--59,31) }]
                                Range =
                                 /root/NestedModule/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (59,8--59,33) }],
                             SynIdent (Base2, None),
                             SynValTyparDecls (None, true),
                             LongIdent (SynLongIdent ([int], [], [None])),
                             SynValInfo ([], SynArgInfo ([], false, None)),
                             false, false,
                             PreXmlDoc ((59,8), FSharp.Compiler.Xml.XmlDocCollector),
                             None, None,
                             /root/NestedModule/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (59,8--61,26),
                             { LeadingKeyword =
                                Member
                                  /root/NestedModule/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (61,8--61,14)
                               InlineKeyword = None
                               WithKeyword = None
                               EqualsRange = None }),
                          { IsInstance = true
                            IsDispatchSlot = false
                            IsOverrideOrExplicitImpl = false
                            IsFinal = false
                            GetterOrSetterIsCompilerGenerated = false
                            MemberKind = PropertyGet },
                          /root/NestedModule/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (59,8--61,26),
                          { GetSetKeywords = None })],
                      /root/NestedModule/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (49,9--61,26),
                      { LeadingKeyword =
                         Type
                           /root/NestedModule/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (49,4--49,8)
                        EqualsRange = None
                        WithKeyword =
                         Some
                           /root/NestedModule/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (49,21--49,25) })],
                  /root/NestedModule/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (49,4--61,26))],
              /root/NestedModule/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (45,0--61,26),
              { ModuleKeyword =
                 Some
                   /root/NestedModule/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (47,0--47,6)
                EqualsRange =
                 Some
                   /root/NestedModule/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (47,17--47,18) })],
          PreXmlDocEmpty, [], None,
          /root/NestedModule/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (2,0--61,26),
          { LeadingKeyword =
             Namespace
               /root/NestedModule/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (2,0--2,9) })],
      { ConditionalDirectives = []
        CodeComments =
         [LineComment
            /root/NestedModule/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (51,8--51,82);
          LineComment
            /root/NestedModule/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (54,8--54,84);
          LineComment
            /root/NestedModule/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (57,8--57,88);
          LineComment
            /root/NestedModule/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (60,8--60,89)] },
      set []))
