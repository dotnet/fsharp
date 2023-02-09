SigFile
  (ParsedSigFileInput
     ("/root/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi",
      QualifiedNameOfFile
        RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl, [],
      [],
      [SynModuleOrNamespaceSig
         ([Microsoft; FSharp; Core], false, DeclaredNamespace,
          [Open
             (ModuleOrNamespace
                (SynLongIdent ([System], [], [None]),
                 /root/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (3,5--3,11)),
              /root/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (3,0--3,11));
           Open
             (ModuleOrNamespace
                (SynLongIdent
                   ([System; Collections; Generic],
                    [/root/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (4,11--4,12);
                     /root/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (4,23--4,24)],
                    [None; None; None]),
                 /root/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (4,5--4,31)),
              /root/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (4,0--4,31));
           Open
             (ModuleOrNamespace
                (SynLongIdent
                   ([Microsoft; FSharp; Core],
                    [/root/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (5,14--5,15);
                     /root/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (5,21--5,22)],
                    [None; None; None]),
                 /root/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (5,5--5,26)),
              /root/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (5,0--5,26));
           Open
             (ModuleOrNamespace
                (SynLongIdent
                   ([Microsoft; FSharp; Collections],
                    [/root/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (6,14--6,15);
                     /root/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (6,21--6,22)],
                    [None; None; None]),
                 /root/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (6,5--6,33)),
              /root/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (6,0--6,33));
           Open
             (ModuleOrNamespace
                (SynLongIdent
                   ([System; Collections],
                    [/root/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (7,11--7,12)],
                    [None; None]),
                 /root/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (7,5--7,23)),
              /root/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (7,0--7,23));
           NestedModule
             (SynComponentInfo
                ([], None, [], [Tuple],
                 PreXmlDoc ((10,0), FSharp.Compiler.Xml.XmlDocCollector), false,
                 None,
                 /root/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (10,0--10,12)),
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
                               /root/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (12,14--12,31))),
                         [], [Tuple],
                         PreXmlDoc ((12,4), FSharp.Compiler.Xml.XmlDocCollector),
                         true, None,
                         /root/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (12,9--12,14)),
                      ObjectModel
                        (Unspecified,
                         [Interface
                            (LongIdent
                               (SynLongIdent
                                  ([IStructuralEquatable], [], [None])),
                             /root/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (13,8--13,38));
                          Interface
                            (LongIdent
                               (SynLongIdent
                                  ([IStructuralComparable], [], [None])),
                             /root/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (14,8--14,39));
                          Interface
                            (LongIdent
                               (SynLongIdent ([IComparable], [], [None])),
                             /root/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (15,8--15,29));
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
                                             /root/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (16,14--16,17)));
                                       Star
                                         /root/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (16,18--16,19);
                                       Type
                                         (Var
                                            (SynTypar (T2, None, false),
                                             /root/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (16,20--16,23)));
                                       Star
                                         /root/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (16,24--16,25);
                                       Type
                                         (Var
                                            (SynTypar (T3, None, false),
                                             /root/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (16,26--16,29)));
                                       Star
                                         /root/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (16,30--16,31);
                                       Type
                                         (Var
                                            (SynTypar (T4, None, false),
                                             /root/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (16,32--16,35)))],
                                      /root/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (16,14--16,35)),
                                   App
                                     (LongIdent
                                        (SynLongIdent ([Tuple], [], [None])),
                                      Some
                                        /root/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (16,44--16,45),
                                      [Var
                                         (SynTypar (T1, None, false),
                                          /root/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (16,45--16,48));
                                       Var
                                         (SynTypar (T2, None, false),
                                          /root/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (16,49--16,52));
                                       Var
                                         (SynTypar (T3, None, false),
                                          /root/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (16,53--16,56));
                                       Var
                                         (SynTypar (T4, None, false),
                                          /root/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (16,57--16,60))],
                                      [/root/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (16,48--16,49);
                                       /root/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (16,52--16,53);
                                       /root/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (16,56--16,57)],
                                      Some
                                        /root/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (16,60--16,61),
                                      false,
                                      /root/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (16,39--16,61)),
                                   /root/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (16,14--16,61),
                                   { ArrowRange =
                                      /root/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (16,36--16,38) }),
                                SynValInfo
                                  ([[SynArgInfo ([], false, None);
                                     SynArgInfo ([], false, None);
                                     SynArgInfo ([], false, None);
                                     SynArgInfo ([], false, None)]],
                                   SynArgInfo ([], false, None)), false, false,
                                PreXmlDoc ((16,8), FSharp.Compiler.Xml.XmlDocCollector),
                                None, None,
                                /root/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (16,8--16,61),
                                { LeadingKeyword =
                                   New
                                     /root/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (16,8--16,11)
                                  InlineKeyword = None
                                  WithKeyword = None
                                  EqualsRange = None }),
                             { IsInstance = false
                               IsDispatchSlot = false
                               IsOverrideOrExplicitImpl = false
                               IsFinal = false
                               GetterOrSetterIsCompilerGenerated = false
                               MemberKind = Constructor },
                             /root/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (16,8--16,61),
                             { GetSetKeywords = None });
                          Member
                            (SynValSig
                               ([], SynIdent (Item1, None),
                                SynValTyparDecls (None, true),
                                Var
                                  (SynTypar (T1, None, false),
                                   /root/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (17,23--17,26)),
                                SynValInfo ([], SynArgInfo ([], false, None)),
                                false, false,
                                PreXmlDoc ((17,8), FSharp.Compiler.Xml.XmlDocCollector),
                                None, None,
                                /root/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (17,8--17,35),
                                { LeadingKeyword =
                                   Member
                                     /root/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (17,8--17,14)
                                  InlineKeyword = None
                                  WithKeyword =
                                   Some
                                     /root/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (17,27--17,31)
                                  EqualsRange = None }),
                             { IsInstance = true
                               IsDispatchSlot = false
                               IsOverrideOrExplicitImpl = false
                               IsFinal = false
                               GetterOrSetterIsCompilerGenerated = false
                               MemberKind = PropertyGet },
                             /root/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (17,8--17,35),
                             { GetSetKeywords =
                                Some
                                  (Get
                                     /root/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (17,32--17,35)) });
                          Member
                            (SynValSig
                               ([], SynIdent (Item2, None),
                                SynValTyparDecls (None, true),
                                Var
                                  (SynTypar (T2, None, false),
                                   /root/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (18,23--18,26)),
                                SynValInfo ([], SynArgInfo ([], false, None)),
                                false, false,
                                PreXmlDoc ((18,8), FSharp.Compiler.Xml.XmlDocCollector),
                                None, None,
                                /root/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (18,8--18,35),
                                { LeadingKeyword =
                                   Member
                                     /root/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (18,8--18,14)
                                  InlineKeyword = None
                                  WithKeyword =
                                   Some
                                     /root/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (18,27--18,31)
                                  EqualsRange = None }),
                             { IsInstance = true
                               IsDispatchSlot = false
                               IsOverrideOrExplicitImpl = false
                               IsFinal = false
                               GetterOrSetterIsCompilerGenerated = false
                               MemberKind = PropertyGet },
                             /root/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (18,8--18,35),
                             { GetSetKeywords =
                                Some
                                  (Get
                                     /root/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (18,32--18,35)) });
                          Member
                            (SynValSig
                               ([], SynIdent (Item3, None),
                                SynValTyparDecls (None, true),
                                Var
                                  (SynTypar (T3, None, false),
                                   /root/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (19,23--19,26)),
                                SynValInfo ([], SynArgInfo ([], false, None)),
                                false, false,
                                PreXmlDoc ((19,8), FSharp.Compiler.Xml.XmlDocCollector),
                                None, None,
                                /root/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (19,8--19,35),
                                { LeadingKeyword =
                                   Member
                                     /root/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (19,8--19,14)
                                  InlineKeyword = None
                                  WithKeyword =
                                   Some
                                     /root/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (19,27--19,31)
                                  EqualsRange = None }),
                             { IsInstance = true
                               IsDispatchSlot = false
                               IsOverrideOrExplicitImpl = false
                               IsFinal = false
                               GetterOrSetterIsCompilerGenerated = false
                               MemberKind = PropertyGet },
                             /root/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (19,8--19,35),
                             { GetSetKeywords =
                                Some
                                  (Get
                                     /root/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (19,32--19,35)) });
                          Member
                            (SynValSig
                               ([], SynIdent (Item4, None),
                                SynValTyparDecls (None, true),
                                Var
                                  (SynTypar (T4, None, false),
                                   /root/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (20,23--20,26)),
                                SynValInfo ([], SynArgInfo ([], false, None)),
                                false, false,
                                PreXmlDoc ((20,8), FSharp.Compiler.Xml.XmlDocCollector),
                                None, None,
                                /root/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (20,8--20,35),
                                { LeadingKeyword =
                                   Member
                                     /root/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (20,8--20,14)
                                  InlineKeyword = None
                                  WithKeyword =
                                   Some
                                     /root/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (20,27--20,31)
                                  EqualsRange = None }),
                             { IsInstance = true
                               IsDispatchSlot = false
                               IsOverrideOrExplicitImpl = false
                               IsFinal = false
                               GetterOrSetterIsCompilerGenerated = false
                               MemberKind = PropertyGet },
                             /root/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (20,8--20,35),
                             { GetSetKeywords =
                                Some
                                  (Get
                                     /root/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (20,32--20,35)) })],
                         /root/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (13,8--20,35)),
                      [],
                      /root/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (12,9--20,35),
                      { LeadingKeyword =
                         Type
                           /root/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (12,4--12,8)
                        EqualsRange =
                         Some
                           /root/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (12,32--12,33)
                        WithKeyword = None })],
                  /root/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (12,4--20,35))],
              /root/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (10,0--20,35),
              { ModuleKeyword =
                 Some
                   /root/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (10,0--10,6)
                EqualsRange =
                 Some
                   /root/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (10,13--10,14) });
           NestedModule
             (SynComponentInfo
                ([], None, [], [Choice],
                 PreXmlDoc ((23,0), FSharp.Compiler.Xml.XmlDocCollector), false,
                 None,
                 /root/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (23,0--23,13)),
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
                                    /root/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (26,6--26,24))
                                Target = None
                                AppliesToGetterAndSetter = false
                                Range =
                                 /root/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (26,6--26,24) };
                              { TypeName =
                                 SynLongIdent
                                   ([StructuralComparison], [], [None])
                                ArgExpr =
                                 Const
                                   (Unit,
                                    /root/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (26,26--26,46))
                                Target = None
                                AppliesToGetterAndSetter = false
                                Range =
                                 /root/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (26,26--26,46) }]
                            Range =
                             /root/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (26,4--26,48) };
                          { Attributes =
                             [{ TypeName =
                                 SynLongIdent ([CompiledName], [], [None])
                                ArgExpr =
                                 Paren
                                   (Const
                                      (String
                                         ("FSharpChoice`6", Regular,
                                          /root/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (27,19--27,35)),
                                       /root/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (27,19--27,35)),
                                    /root/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (27,18--27,19),
                                    Some
                                      /root/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (27,35--27,36),
                                    /root/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (27,18--27,36))
                                Target = None
                                AppliesToGetterAndSetter = false
                                Range =
                                 /root/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (27,6--27,36) }]
                            Range =
                             /root/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (27,4--27,38) }],
                         Some
                           (PostfixList
                              ([SynTyparDecl ([], SynTypar (T1, None, false));
                                SynTyparDecl ([], SynTypar (T2, None, false));
                                SynTyparDecl ([], SynTypar (T3, None, false));
                                SynTyparDecl ([], SynTypar (T4, None, false));
                                SynTyparDecl ([], SynTypar (T5, None, false));
                                SynTyparDecl ([], SynTypar (T6, None, false))],
                               [],
                               /root/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (28,15--28,40))),
                         [], [Choice],
                         PreXmlDoc ((26,4), FSharp.Compiler.Xml.XmlDocCollector),
                         true, None,
                         /root/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (28,9--28,15)),
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
                                         /root/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (30,22--30,25)),
                                      false,
                                      PreXmlDoc ((30,22), FSharp.Compiler.Xml.XmlDocCollector),
                                      None,
                                      /root/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (30,22--30,25),
                                      { LeadingKeyword = None })],
                                PreXmlDoc ((30,6), FSharp.Compiler.Xml.XmlDocCollector),
                                None,
                                /root/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (29,6--30,25),
                                { BarRange =
                                   Some
                                     /root/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (30,6--30,7) });
                             SynUnionCase
                               ([], SynIdent (Choice2Of6, None),
                                Fields
                                  [SynField
                                     ([], false, None,
                                      Var
                                        (SynTypar (T2, None, false),
                                         /root/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (32,22--32,25)),
                                      false,
                                      PreXmlDoc ((32,22), FSharp.Compiler.Xml.XmlDocCollector),
                                      None,
                                      /root/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (32,22--32,25),
                                      { LeadingKeyword = None })],
                                PreXmlDoc ((32,6), FSharp.Compiler.Xml.XmlDocCollector),
                                None,
                                /root/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (31,6--32,25),
                                { BarRange =
                                   Some
                                     /root/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (32,6--32,7) });
                             SynUnionCase
                               ([], SynIdent (Choice3Of6, None),
                                Fields
                                  [SynField
                                     ([], false, None,
                                      Var
                                        (SynTypar (T3, None, false),
                                         /root/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (34,22--34,25)),
                                      false,
                                      PreXmlDoc ((34,22), FSharp.Compiler.Xml.XmlDocCollector),
                                      None,
                                      /root/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (34,22--34,25),
                                      { LeadingKeyword = None })],
                                PreXmlDoc ((34,6), FSharp.Compiler.Xml.XmlDocCollector),
                                None,
                                /root/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (33,6--34,25),
                                { BarRange =
                                   Some
                                     /root/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (34,6--34,7) });
                             SynUnionCase
                               ([], SynIdent (Choice4Of6, None),
                                Fields
                                  [SynField
                                     ([], false, None,
                                      Var
                                        (SynTypar (T4, None, false),
                                         /root/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (36,22--36,25)),
                                      false,
                                      PreXmlDoc ((36,22), FSharp.Compiler.Xml.XmlDocCollector),
                                      None,
                                      /root/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (36,22--36,25),
                                      { LeadingKeyword = None })],
                                PreXmlDoc ((36,6), FSharp.Compiler.Xml.XmlDocCollector),
                                None,
                                /root/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (35,6--36,25),
                                { BarRange =
                                   Some
                                     /root/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (36,6--36,7) });
                             SynUnionCase
                               ([], SynIdent (Choice5Of6, None),
                                Fields
                                  [SynField
                                     ([], false, None,
                                      Var
                                        (SynTypar (T5, None, false),
                                         /root/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (38,22--38,25)),
                                      false,
                                      PreXmlDoc ((38,22), FSharp.Compiler.Xml.XmlDocCollector),
                                      None,
                                      /root/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (38,22--38,25),
                                      { LeadingKeyword = None })],
                                PreXmlDoc ((38,6), FSharp.Compiler.Xml.XmlDocCollector),
                                None,
                                /root/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (37,6--38,25),
                                { BarRange =
                                   Some
                                     /root/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (38,6--38,7) });
                             SynUnionCase
                               ([], SynIdent (Choice6Of6, None),
                                Fields
                                  [SynField
                                     ([], false, None,
                                      Var
                                        (SynTypar (T6, None, false),
                                         /root/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (40,22--40,25)),
                                      false,
                                      PreXmlDoc ((40,22), FSharp.Compiler.Xml.XmlDocCollector),
                                      None,
                                      /root/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (40,22--40,25),
                                      { LeadingKeyword = None })],
                                PreXmlDoc ((40,6), FSharp.Compiler.Xml.XmlDocCollector),
                                None,
                                /root/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (39,6--40,25),
                                { BarRange =
                                   Some
                                     /root/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (40,6--40,7) })],
                            /root/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (29,6--40,25)),
                         /root/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (29,6--40,25)),
                      [],
                      /root/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (25,4--40,25),
                      { LeadingKeyword =
                         Type
                           /root/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (28,4--28,8)
                        EqualsRange =
                         Some
                           /root/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (28,41--28,42)
                        WithKeyword = None })],
                  /root/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (25,4--40,25))],
              /root/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (23,0--40,25),
              { ModuleKeyword =
                 Some
                   /root/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (23,0--23,6)
                EqualsRange =
                 Some
                   /root/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (23,14--23,15) });
           NestedModule
             (SynComponentInfo
                ([{ Attributes =
                     [{ TypeName = SynLongIdent ([AutoOpen], [], [None])
                        ArgExpr =
                         Const
                           (Unit,
                            /root/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (45,2--45,10))
                        Target = None
                        AppliesToGetterAndSetter = false
                        Range =
                         /root/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (45,2--45,10) }]
                    Range =
                     /root/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (45,0--45,12) }],
                 None, [], [Operators],
                 PreXmlDoc ((45,0), FSharp.Compiler.Xml.XmlDocCollector), false,
                 None,
                 /root/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (46,0--46,16)),
              false,
              [Types
                 ([SynTypeDefnSig
                     (SynComponentInfo
                        ([],
                         Some
                           (PostfixList
                              ([SynTyparDecl ([], SynTypar (T, None, false))],
                               [],
                               /root/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (48,16--48,20))),
                         [], [[,]],
                         PreXmlDoc ((48,4), FSharp.Compiler.Xml.XmlDocCollector),
                         true, None,
                         /root/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (48,9--48,16)),
                      Simple
                        (None
                           /root/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (48,9--60,26),
                         /root/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (48,9--60,26)),
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
                                              /root/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (49,23--49,32)),
                                           /root/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (49,23--49,32)),
                                        /root/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (49,22--49,23),
                                        Some
                                          /root/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (49,32--49,33),
                                        /root/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (49,22--49,33))
                                    Target = None
                                    AppliesToGetterAndSetter = false
                                    Range =
                                     /root/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (49,10--49,33) }]
                                Range =
                                 /root/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (49,8--49,35) }],
                             SynIdent (Length1, None),
                             SynValTyparDecls (None, true),
                             LongIdent (SynLongIdent ([int], [], [None])),
                             SynValInfo ([], SynArgInfo ([], false, None)),
                             false, false,
                             PreXmlDoc ((49,8), FSharp.Compiler.Xml.XmlDocCollector),
                             None, None,
                             /root/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (49,8--51,28),
                             { LeadingKeyword =
                                Member
                                  /root/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (51,8--51,14)
                               InlineKeyword = None
                               WithKeyword = None
                               EqualsRange = None }),
                          { IsInstance = true
                            IsDispatchSlot = false
                            IsOverrideOrExplicitImpl = false
                            IsFinal = false
                            GetterOrSetterIsCompilerGenerated = false
                            MemberKind = PropertyGet },
                          /root/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (49,8--51,28),
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
                                              /root/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (52,23--52,32)),
                                           /root/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (52,23--52,32)),
                                        /root/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (52,22--52,23),
                                        Some
                                          /root/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (52,32--52,33),
                                        /root/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (52,22--52,33))
                                    Target = None
                                    AppliesToGetterAndSetter = false
                                    Range =
                                     /root/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (52,10--52,33) }]
                                Range =
                                 /root/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (52,8--52,35) }],
                             SynIdent (Length2, None),
                             SynValTyparDecls (None, true),
                             LongIdent (SynLongIdent ([int], [], [None])),
                             SynValInfo ([], SynArgInfo ([], false, None)),
                             false, false,
                             PreXmlDoc ((52,8), FSharp.Compiler.Xml.XmlDocCollector),
                             None, None,
                             /root/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (52,8--54,28),
                             { LeadingKeyword =
                                Member
                                  /root/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (54,8--54,14)
                               InlineKeyword = None
                               WithKeyword = None
                               EqualsRange = None }),
                          { IsInstance = true
                            IsDispatchSlot = false
                            IsOverrideOrExplicitImpl = false
                            IsFinal = false
                            GetterOrSetterIsCompilerGenerated = false
                            MemberKind = PropertyGet },
                          /root/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (52,8--54,28),
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
                                              /root/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (55,23--55,30)),
                                           /root/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (55,23--55,30)),
                                        /root/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (55,22--55,23),
                                        Some
                                          /root/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (55,30--55,31),
                                        /root/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (55,22--55,31))
                                    Target = None
                                    AppliesToGetterAndSetter = false
                                    Range =
                                     /root/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (55,10--55,31) }]
                                Range =
                                 /root/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (55,8--55,33) }],
                             SynIdent (Base1, None),
                             SynValTyparDecls (None, true),
                             LongIdent (SynLongIdent ([int], [], [None])),
                             SynValInfo ([], SynArgInfo ([], false, None)),
                             false, false,
                             PreXmlDoc ((55,8), FSharp.Compiler.Xml.XmlDocCollector),
                             None, None,
                             /root/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (55,8--57,26),
                             { LeadingKeyword =
                                Member
                                  /root/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (57,8--57,14)
                               InlineKeyword = None
                               WithKeyword = None
                               EqualsRange = None }),
                          { IsInstance = true
                            IsDispatchSlot = false
                            IsOverrideOrExplicitImpl = false
                            IsFinal = false
                            GetterOrSetterIsCompilerGenerated = false
                            MemberKind = PropertyGet },
                          /root/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (55,8--57,26),
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
                                              /root/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (58,23--58,30)),
                                           /root/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (58,23--58,30)),
                                        /root/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (58,22--58,23),
                                        Some
                                          /root/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (58,30--58,31),
                                        /root/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (58,22--58,31))
                                    Target = None
                                    AppliesToGetterAndSetter = false
                                    Range =
                                     /root/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (58,10--58,31) }]
                                Range =
                                 /root/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (58,8--58,33) }],
                             SynIdent (Base2, None),
                             SynValTyparDecls (None, true),
                             LongIdent (SynLongIdent ([int], [], [None])),
                             SynValInfo ([], SynArgInfo ([], false, None)),
                             false, false,
                             PreXmlDoc ((58,8), FSharp.Compiler.Xml.XmlDocCollector),
                             None, None,
                             /root/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (58,8--60,26),
                             { LeadingKeyword =
                                Member
                                  /root/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (60,8--60,14)
                               InlineKeyword = None
                               WithKeyword = None
                               EqualsRange = None }),
                          { IsInstance = true
                            IsDispatchSlot = false
                            IsOverrideOrExplicitImpl = false
                            IsFinal = false
                            GetterOrSetterIsCompilerGenerated = false
                            MemberKind = PropertyGet },
                          /root/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (58,8--60,26),
                          { GetSetKeywords = None })],
                      /root/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (48,9--60,26),
                      { LeadingKeyword =
                         Type
                           /root/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (48,4--48,8)
                        EqualsRange = None
                        WithKeyword =
                         Some
                           /root/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (48,21--48,25) })],
                  /root/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (48,4--60,26))],
              /root/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (44,0--60,26),
              { ModuleKeyword =
                 Some
                   /root/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (46,0--46,6)
                EqualsRange =
                 Some
                   /root/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (46,17--46,18) })],
          PreXmlDocEmpty, [], None,
          /root/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (1,0--60,26),
          { LeadingKeyword =
             Namespace
               /root/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (1,0--1,9) })],
      { ConditionalDirectives = []
        CodeComments =
         [LineComment
            /root/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (50,8--50,82);
          LineComment
            /root/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (53,8--53,84);
          LineComment
            /root/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (56,8--56,88);
          LineComment
            /root/RangeOfNestedModuleInSignatureFileShouldEndAtTheLastSynModuleSigDecl.fsi (59,8--59,89)] },
      set []))