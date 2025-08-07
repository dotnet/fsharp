ImplFile
  (ParsedImplFileInput
     ("/root/OpenDeclaration/E_openInTypeDecl.fs", false,
      QualifiedNameOfFile openInTypeDecl, [],
      [SynModuleOrNamespace
         ([openInTypeDecl], false, NamedModule,
          [Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([], None, [], [Foo],
                     PreXmlDoc ((4,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (4,5--4,8)),
                  ObjectModel
                    (Unspecified,
                     [ImplicitCtor
                        (None, [], Const (Unit, (4,8--4,10)), None,
                         PreXmlDoc ((4,8), FSharp.Compiler.Xml.XmlDocCollector),
                         (4,5--4,8), { AsKeyword = None });
                      Open
                        (Type
                           (LongIdent
                              (SynLongIdent
                                 ([System; DateTime], [(5,20--5,21)],
                                  [None; None])), (5,14--5,29)), (5,4--5,29));
                      ImplicitInherit
                        (LongIdent (SynLongIdent ([Object], [], [None])),
                         Const (Unit, (7,18--7,20)), None, (7,4--7,20),
                         { InheritKeyword = (7,4--7,11) });
                      ValField
                        (SynField
                           ([{ Attributes =
                                [{ TypeName =
                                    SynLongIdent ([DefaultValue], [], [None])
                                   ArgExpr = Const (Unit, (9,6--9,18))
                                   Target = None
                                   AppliesToGetterAndSetter = false
                                   Range = (9,6--9,18) }]
                               Range = (9,4--9,20) }], false, Some x,
                            LongIdent (SynLongIdent ([Int64], [], [None])), true,
                            PreXmlDoc ((9,4), FSharp.Compiler.Xml.XmlDocCollector),
                            None, (9,4--9,41),
                            { LeadingKeyword = Some (Val (9,21--9,24))
                              MutableKeyword = Some (9,25--9,32) }), (9,4--9,41));
                      LetBindings
                        ([SynBinding
                            (None, Normal, false, false, [],
                             PreXmlDoc ((10,4), FSharp.Compiler.Xml.XmlDocCollector),
                             SynValData
                               (None,
                                SynValInfo ([], SynArgInfo ([], false, None)),
                                None),
                             Named
                               (SynIdent (x, None), false, None, (10,8--10,9)),
                             None, Const (Int32 42, (10,12--10,14)),
                             (10,8--10,9), Yes (10,4--10,14),
                             { LeadingKeyword = Let (10,4--10,7)
                               InlineKeyword = None
                               EqualsRange = Some (10,10--10,11) })], false,
                         false, (10,4--10,14));
                      LetBindings
                        ([SynBinding
                            (None, Normal, false, false, [],
                             PreXmlDoc ((11,4), FSharp.Compiler.Xml.XmlDocCollector),
                             SynValData
                               (None,
                                SynValInfo ([], SynArgInfo ([], false, None)),
                                None),
                             Named
                               (SynIdent (timeConstructed, None), false, None,
                                (11,8--11,23)), None,
                             LongIdent
                               (false,
                                SynLongIdent
                                  ([Now; Ticks], [(11,29--11,30)], [None; None]),
                                None, (11,26--11,35)), (11,8--11,23),
                             Yes (11,4--11,35),
                             { LeadingKeyword = Let (11,4--11,7)
                               InlineKeyword = None
                               EqualsRange = Some (11,24--11,25) })], false,
                         false, (11,4--11,35));
                      LetBindings
                        ([SynBinding
                            (None, Do, false, false, [], PreXmlDocEmpty,
                             SynValData
                               (None,
                                SynValInfo ([], SynArgInfo ([], false, None)),
                                None), Const (Unit, (12,4--12,34)), None,
                             App
                               (NonAtomic, false,
                                App
                                  (NonAtomic, false, Ident printfn,
                                   Const
                                     (String ("%d", Regular, (12,15--12,19)),
                                      (12,15--12,19)), (12,7--12,19)),
                                LongIdent
                                  (false,
                                   SynLongIdent
                                     ([Int32; MaxValue], [(12,25--12,26)],
                                      [None; None]), None, (12,20--12,34)),
                                (12,7--12,34)), (12,4--12,34), NoneAtDo,
                             { LeadingKeyword = Do (12,4--12,6)
                               InlineKeyword = None
                               EqualsRange = None })], false, false,
                         (12,4--12,34));
                      Member
                        (SynBinding
                           (None, Normal, false, false, [],
                            PreXmlDoc ((13,4), FSharp.Compiler.Xml.XmlDocCollector),
                            SynValData
                              (Some { IsInstance = false
                                      IsDispatchSlot = false
                                      IsOverrideOrExplicitImpl = false
                                      IsFinal = false
                                      GetterOrSetterIsCompilerGenerated = false
                                      MemberKind = Member },
                               SynValInfo ([[]], SynArgInfo ([], false, None)),
                               None),
                            LongIdent
                              (SynLongIdent ([Now], [], [None]), None, None,
                               Pats
                                 [Paren
                                    (Const (Unit, (13,22--13,24)),
                                     (13,22--13,24))], None, (13,18--13,24)),
                            None,
                            LongIdent
                              (false,
                               SynLongIdent
                                 ([DateTime; Now], [(13,35--13,36)],
                                  [None; None]), None, (13,27--13,39)),
                            (13,18--13,24), NoneAtInvisible,
                            { LeadingKeyword =
                               StaticMember ((13,4--13,10), (13,11--13,17))
                              InlineKeyword = None
                              EqualsRange = Some (13,25--13,26) }),
                         (13,4--13,39));
                      AutoProperty
                        ([], false, TimeConstructed, None, PropertyGetSet,
                         { IsInstance = true
                           IsDispatchSlot = false
                           IsOverrideOrExplicitImpl = false
                           IsFinal = false
                           GetterOrSetterIsCompilerGenerated = false
                           MemberKind = Member },
                         { IsInstance = true
                           IsDispatchSlot = false
                           IsOverrideOrExplicitImpl = false
                           IsFinal = false
                           GetterOrSetterIsCompilerGenerated = false
                           MemberKind = PropertySet },
                         PreXmlDoc ((14,4), FSharp.Compiler.Xml.XmlDocCollector),
                         GetSet (None, None, None), Ident timeConstructed,
                         (14,4--14,62),
                         { LeadingKeyword =
                            MemberVal ((14,4--14,10), (14,11--14,14))
                           WithKeyword = Some (14,49--14,53)
                           EqualsRange = Some (14,31--14,32)
                           GetSetKeywords =
                            Some (GetSet ((14,54--14,57), (14,59--14,62))) });
                      Member
                        (SynBinding
                           (None, Normal, false, false, [],
                            PreXmlDoc ((16,4), FSharp.Compiler.Xml.XmlDocCollector),
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
                                 ([_; M], [(16,12--16,13)], [None; None]), None,
                               None,
                               Pats
                                 [Paren
                                    (Const (Unit, (16,14--16,16)),
                                     (16,14--16,16))], None, (16,11--16,16)),
                            None,
                            Open
                              (Type
                                 (LongIdent
                                    (SynLongIdent
                                       ([System; ArgumentException],
                                        [(17,24--17,25)], [None; None])),
                                  (17,18--17,42)), (17,8--17,42), (17,8--18,22),
                               LongIdent
                                 (false,
                                  SynLongIdent
                                    ([Int32; MaxValue], [(18,13--18,14)],
                                     [None; None]), None, (18,8--18,22))),
                            (16,11--16,16), NoneAtInvisible,
                            { LeadingKeyword = Member (16,4--16,10)
                              InlineKeyword = None
                              EqualsRange = Some (16,17--16,18) }),
                         (16,4--18,22));
                      Interface
                        (LongIdent (SynLongIdent ([IDisposable], [], [None])),
                         Some (20,26--20,30),
                         Some
                           [Member
                              (SynBinding
                                 (None, Normal, false, false, [],
                                  PreXmlDoc ((21,8), FSharp.Compiler.Xml.XmlDocCollector),
                                  SynValData
                                    (Some
                                       { IsInstance = true
                                         IsDispatchSlot = false
                                         IsOverrideOrExplicitImpl = true
                                         IsFinal = false
                                         GetterOrSetterIsCompilerGenerated =
                                          false
                                         MemberKind = Member },
                                     SynValInfo
                                       ([[SynArgInfo ([], false, None)]; []],
                                        SynArgInfo ([], false, None)), None),
                                  LongIdent
                                    (SynLongIdent
                                       ([this; Dispose], [(21,19--21,20)],
                                        [None; None]), None, None,
                                     Pats
                                       [Paren
                                          (Const (Unit, (21,28--21,30)),
                                           (21,28--21,30))], None,
                                     (21,15--21,30)),
                                  Some
                                    (SynBindingReturnInfo
                                       (LongIdent
                                          (SynLongIdent ([unit], [], [None])),
                                        (21,32--21,36), [],
                                        { ColonRange = Some (21,30--21,31) })),
                                  Typed
                                    (App
                                       (NonAtomic, false, Ident raise,
                                        Paren
                                          (App
                                             (Atomic, false,
                                              Ident NotImplementedException,
                                              Const (Unit, (22,42--22,44)),
                                              (22,19--22,44)), (22,18--22,19),
                                           Some (22,44--22,45), (22,18--22,45)),
                                        (22,12--22,45)),
                                     LongIdent
                                       (SynLongIdent ([unit], [], [None])),
                                     (22,12--22,45)), (21,15--21,30),
                                  NoneAtInvisible,
                                  { LeadingKeyword = Member (21,8--21,14)
                                    InlineKeyword = None
                                    EqualsRange = Some (21,37--21,38) }),
                               (21,8--22,45))], (20,4--22,45));
                      Open
                        (ModuleOrNamespace
                           (SynLongIdent
                              ([`global`; System], [(23,15--23,16)],
                               [Some (OriginalNotation "global"); None]),
                            (23,9--23,22)), (23,4--23,22))], (5,4--23,22)), [],
                  Some
                    (ImplicitCtor
                       (None, [], Const (Unit, (4,8--4,10)), None,
                        PreXmlDoc ((4,8), FSharp.Compiler.Xml.XmlDocCollector),
                        (4,5--4,8), { AsKeyword = None })), (4,5--23,22),
                  { LeadingKeyword = Type (4,0--4,4)
                    EqualsRange = Some (4,11--4,12)
                    WithKeyword = None })], (4,0--23,22));
           Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([], None, [], [A],
                     PreXmlDoc ((25,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (25,5--25,6)),
                  Simple
                    (Union
                       (None,
                        [SynUnionCase
                           ([], SynIdent (A, None),
                            Fields
                              [SynField
                                 ([], false, None,
                                  LongIdent (SynLongIdent ([int], [], [None])),
                                  false,
                                  PreXmlDoc ((25,14), FSharp.Compiler.Xml.XmlDocCollector),
                                  None, (25,14--25,17),
                                  { LeadingKeyword = None
                                    MutableKeyword = None })],
                            PreXmlDoc ((25,9), FSharp.Compiler.Xml.XmlDocCollector),
                            None, (25,9--25,17), { BarRange = None })],
                        (25,9--25,17)), (25,9--25,17)),
                  [GetSetMember
                     (Some
                        (SynBinding
                           (None, Normal, false, false, [],
                            PreXmlMerge
  (PreXmlDoc ((27,8), FSharp.Compiler.Xml.XmlDocCollector), PreXmlDocEmpty),
                            SynValData
                              (Some { IsInstance = true
                                      IsDispatchSlot = false
                                      IsOverrideOrExplicitImpl = false
                                      IsFinal = false
                                      GetterOrSetterIsCompilerGenerated = false
                                      MemberKind = PropertyGet },
                               SynValInfo
                                 ([[SynArgInfo ([], false, None)]; []],
                                  SynArgInfo ([], false, None)), None),
                            LongIdent
                              (SynLongIdent
                                 ([_; RandomNumber], [(27,16--27,17)],
                                  [None; None]), Some get, None,
                               Pats
                                 [Paren
                                    (Const (Unit, (27,38--27,40)),
                                     (27,38--27,40))], None, (27,35--27,40)),
                            None,
                            App
                              (Atomic, false,
                               DotGet
                                 (App
                                    (Atomic, false, Ident Random,
                                     Const (Unit, (27,49--27,51)),
                                     (27,43--27,51)), (27,51--27,52),
                                  SynLongIdent ([Next], [], [None]),
                                  (27,43--27,56)), Const (Unit, (27,56--27,58)),
                               (27,43--27,58)), (27,35--27,40), NoneAtInvisible,
                            { LeadingKeyword = Member (27,8--27,14)
                              InlineKeyword = None
                              EqualsRange = Some (27,41--27,42) })), None,
                      (27,8--27,58), { InlineKeyword = None
                                       WithKeyword = (27,30--27,34)
                                       GetKeyword = Some (27,35--27,38)
                                       AndKeyword = None
                                       SetKeyword = None });
                   Open
                     (ModuleOrNamespace
                        (SynLongIdent ([System], [], [None]), (28,13--28,19)),
                      (28,8--28,19))], None, (25,5--28,19),
                  { LeadingKeyword = Type (25,0--25,4)
                    EqualsRange = Some (25,7--25,8)
                    WithKeyword = Some (26,4--26,8) })], (25,0--28,19));
           Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([], None, [], [ARecord],
                     PreXmlDoc ((30,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (30,5--30,12)),
                  Simple
                    (Record
                       (None,
                        [SynField
                           ([], false, Some A,
                            LongIdent (SynLongIdent ([int], [], [None])), false,
                            PreXmlDoc ((30,17), FSharp.Compiler.Xml.XmlDocCollector),
                            None, (30,17--30,24), { LeadingKeyword = None
                                                    MutableKeyword = None })],
                        (30,15--30,26)), (30,15--30,26)),
                  [GetSetMember
                     (Some
                        (SynBinding
                           (None, Normal, false, false, [],
                            PreXmlMerge
  (PreXmlDoc ((32,8), FSharp.Compiler.Xml.XmlDocCollector), PreXmlDocEmpty),
                            SynValData
                              (Some { IsInstance = true
                                      IsDispatchSlot = false
                                      IsOverrideOrExplicitImpl = false
                                      IsFinal = false
                                      GetterOrSetterIsCompilerGenerated = false
                                      MemberKind = PropertyGet },
                               SynValInfo
                                 ([[SynArgInfo ([], false, None)]; []],
                                  SynArgInfo ([], false, None)), None),
                            LongIdent
                              (SynLongIdent
                                 ([_; RandomNumber], [(32,16--32,17)],
                                  [None; None]), Some get, None,
                               Pats
                                 [Paren
                                    (Const (Unit, (32,38--32,40)),
                                     (32,38--32,40))], None, (32,35--32,40)),
                            None,
                            App
                              (Atomic, false,
                               DotGet
                                 (App
                                    (Atomic, false, Ident Random,
                                     Const (Unit, (32,49--32,51)),
                                     (32,43--32,51)), (32,51--32,52),
                                  SynLongIdent ([Next], [], [None]),
                                  (32,43--32,56)), Const (Unit, (32,56--32,58)),
                               (32,43--32,58)), (32,35--32,40), NoneAtInvisible,
                            { LeadingKeyword = Member (32,8--32,14)
                              InlineKeyword = None
                              EqualsRange = Some (32,41--32,42) })), None,
                      (32,8--32,58), { InlineKeyword = None
                                       WithKeyword = (32,30--32,34)
                                       GetKeyword = Some (32,35--32,38)
                                       AndKeyword = None
                                       SetKeyword = None });
                   Open
                     (ModuleOrNamespace
                        (SynLongIdent ([System], [], [None]), (33,13--33,19)),
                      (33,8--33,19))], None, (30,5--33,19),
                  { LeadingKeyword = Type (30,0--30,4)
                    EqualsRange = Some (30,13--30,14)
                    WithKeyword = Some (31,4--31,8) })], (30,0--33,19));
           Exception
             (SynExceptionDefn
                (SynExceptionDefnRepr
                   ([],
                    SynUnionCase
                      ([], SynIdent (AException, None),
                       Fields
                         [SynField
                            ([], false, None,
                             LongIdent (SynLongIdent ([int], [], [None])), false,
                             PreXmlDoc ((35,24), FSharp.Compiler.Xml.XmlDocCollector),
                             None, (35,24--35,27), { LeadingKeyword = None
                                                     MutableKeyword = None })],
                       PreXmlDocEmpty, None, (35,10--35,27), { BarRange = None }),
                    None,
                    PreXmlDoc ((35,0), FSharp.Compiler.Xml.XmlDocCollector),
                    None, (35,0--35,27)), Some (36,4--36,8),
                 [GetSetMember
                    (Some
                       (SynBinding
                          (None, Normal, false, false, [],
                           PreXmlMerge
  (PreXmlDoc ((37,8), FSharp.Compiler.Xml.XmlDocCollector), PreXmlDocEmpty),
                           SynValData
                             (Some { IsInstance = true
                                     IsDispatchSlot = false
                                     IsOverrideOrExplicitImpl = false
                                     IsFinal = false
                                     GetterOrSetterIsCompilerGenerated = false
                                     MemberKind = PropertyGet },
                              SynValInfo
                                ([[SynArgInfo ([], false, None)]; []],
                                 SynArgInfo ([], false, None)), None),
                           LongIdent
                             (SynLongIdent
                                ([_; RandomNumber], [(37,16--37,17)],
                                 [None; None]), Some get, None,
                              Pats
                                [Paren
                                   (Const (Unit, (37,38--37,40)), (37,38--37,40))],
                              None, (37,35--37,40)), None,
                           App
                             (Atomic, false,
                              DotGet
                                (App
                                   (Atomic, false, Ident Random,
                                    Const (Unit, (37,49--37,51)), (37,43--37,51)),
                                 (37,51--37,52),
                                 SynLongIdent ([Next], [], [None]),
                                 (37,43--37,56)), Const (Unit, (37,56--37,58)),
                              (37,43--37,58)), (37,35--37,40), NoneAtInvisible,
                           { LeadingKeyword = Member (37,8--37,14)
                             InlineKeyword = None
                             EqualsRange = Some (37,41--37,42) })), None,
                     (37,8--37,58), { InlineKeyword = None
                                      WithKeyword = (37,30--37,34)
                                      GetKeyword = Some (37,35--37,38)
                                      AndKeyword = None
                                      SetKeyword = None });
                  Open
                    (ModuleOrNamespace
                       (SynLongIdent ([System], [], [None]), (38,13--38,19)),
                     (38,8--38,19))], (35,0--38,19)), (35,0--38,19));
           Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([], None, [], [B],
                     PreXmlDoc ((40,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (40,5--40,6)),
                  Simple
                    (Union
                       (None,
                        [SynUnionCase
                           ([], SynIdent (A, None),
                            Fields
                              [SynField
                                 ([], false, None,
                                  LongIdent (SynLongIdent ([Int32], [], [None])),
                                  false,
                                  PreXmlDoc ((40,14), FSharp.Compiler.Xml.XmlDocCollector),
                                  None, (40,14--40,19),
                                  { LeadingKeyword = None
                                    MutableKeyword = None })],
                            PreXmlDoc ((40,9), FSharp.Compiler.Xml.XmlDocCollector),
                            None, (40,9--40,19), { BarRange = None })],
                        (40,9--40,19)), (40,9--40,19)),
                  [Open
                     (ModuleOrNamespace
                        (SynLongIdent ([System], [], [None]), (42,13--42,19)),
                      (42,8--42,19))], None, (40,5--42,19),
                  { LeadingKeyword = Type (40,0--40,4)
                    EqualsRange = Some (40,7--40,8)
                    WithKeyword = Some (41,4--41,8) })], (40,0--42,19));
           Exception
             (SynExceptionDefn
                (SynExceptionDefnRepr
                   ([],
                    SynUnionCase
                      ([], SynIdent (BException, None),
                       Fields
                         [SynField
                            ([], false, None,
                             LongIdent (SynLongIdent ([Int32], [], [None])),
                             false,
                             PreXmlDoc ((44,24), FSharp.Compiler.Xml.XmlDocCollector),
                             None, (44,24--44,29), { LeadingKeyword = None
                                                     MutableKeyword = None })],
                       PreXmlDocEmpty, None, (44,10--44,29), { BarRange = None }),
                    None,
                    PreXmlDoc ((44,0), FSharp.Compiler.Xml.XmlDocCollector),
                    None, (44,0--44,29)), Some (45,4--45,8),
                 [Open
                    (ModuleOrNamespace
                       (SynLongIdent ([System], [], [None]), (46,13--46,19)),
                     (46,8--46,19))], (44,0--46,19)), (44,0--46,19));
           Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([], None, [], [BRecord],
                     PreXmlDoc ((48,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (48,5--48,12)),
                  Simple
                    (Record
                       (None,
                        [SynField
                           ([], false, Some A,
                            LongIdent (SynLongIdent ([Int32], [], [None])),
                            false,
                            PreXmlDoc ((48,17), FSharp.Compiler.Xml.XmlDocCollector),
                            None, (48,17--48,26), { LeadingKeyword = None
                                                    MutableKeyword = None })],
                        (48,15--48,28)), (48,15--48,28)),
                  [Open
                     (ModuleOrNamespace
                        (SynLongIdent ([System], [], [None]), (50,13--50,19)),
                      (50,8--50,19))], None, (48,5--50,19),
                  { LeadingKeyword = Type (48,0--48,4)
                    EqualsRange = Some (48,13--48,14)
                    WithKeyword = Some (49,4--49,8) })], (48,0--50,19));
           Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([{ Attributes =
                         [{ TypeName = SynLongIdent ([Struct], [], [None])
                            ArgExpr = Const (Unit, (52,2--52,8))
                            Target = None
                            AppliesToGetterAndSetter = false
                            Range = (52,2--52,8) }]
                        Range = (52,0--52,10) }], None, [], [ABC],
                     PreXmlDoc ((52,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (53,5--53,8)),
                  ObjectModel
                    (Unspecified,
                     [ValField
                        (SynField
                           ([], false, Some a,
                            LongIdent (SynLongIdent ([Int32], [], [None])),
                            false,
                            PreXmlDoc ((54,4), FSharp.Compiler.Xml.XmlDocCollector),
                            None, (54,4--54,16),
                            { LeadingKeyword = Some (Val (54,4--54,7))
                              MutableKeyword = None }), (54,4--54,16));
                      ValField
                        (SynField
                           ([], false, Some b,
                            LongIdent (SynLongIdent ([Int32], [], [None])),
                            false,
                            PreXmlDoc ((55,4), FSharp.Compiler.Xml.XmlDocCollector),
                            None, (55,4--55,16),
                            { LeadingKeyword = Some (Val (55,4--55,7))
                              MutableKeyword = None }), (55,4--55,16));
                      Member
                        (SynBinding
                           (None, Normal, false, false, [],
                            PreXmlDoc ((56,4), FSharp.Compiler.Xml.XmlDocCollector),
                            SynValData
                              (Some { IsInstance = false
                                      IsDispatchSlot = false
                                      IsOverrideOrExplicitImpl = false
                                      IsFinal = false
                                      GetterOrSetterIsCompilerGenerated = false
                                      MemberKind = Constructor },
                               SynValInfo
                                 ([[SynArgInfo ([], false, Some a)]],
                                  SynArgInfo ([], false, None)), None),
                            LongIdent
                              (SynLongIdent ([new], [], [None]), None,
                               Some (SynValTyparDecls (None, false)),
                               Pats
                                 [Paren
                                    (Named
                                       (SynIdent (a, None), false, None,
                                        (56,9--56,10)), (56,8--56,11))], None,
                               (56,4--56,7)), None,
                            Open
                              (Type
                                 (LongIdent
                                    (SynLongIdent
                                       ([System; Int32], [(57,24--57,25)],
                                        [None; None])), (57,18--57,30)),
                               (57,8--57,30), (57,8--58,31),
                               Record
                                 (None, None,
                                  [SynExprRecordField
                                     ((SynLongIdent ([a], [], [None]), true),
                                      Some (58,12--58,13), Some (Ident a),
                                      (58,10--58,15),
                                      Some ((58,15--58,16), Some (58,16)));
                                   SynExprRecordField
                                     ((SynLongIdent ([b], [], [None]), true),
                                      Some (58,19--58,20), Some (Ident MinValue),
                                      (58,17--58,29), None)], (58,8--58,31))),
                            (56,4--56,11), NoneAtInvisible,
                            { LeadingKeyword = New (56,4--56,7)
                              InlineKeyword = None
                              EqualsRange = Some (56,12--56,13) }),
                         (56,4--58,31));
                      Open
                        (ModuleOrNamespace
                           (SynLongIdent ([System], [], [None]), (59,9--59,15)),
                         (59,4--59,15))], (54,4--59,15)), [], None,
                  (52,0--59,15), { LeadingKeyword = Type (53,0--53,4)
                                   EqualsRange = Some (53,9--53,10)
                                   WithKeyword = None })], (52,0--59,15));
           Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([], None, [], [System; Int32],
                     PreXmlDoc ((61,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (61,5--61,17)),
                  ObjectModel (Augmentation (61,18--61,22), [], (61,5--63,25)),
                  [Member
                     (SynBinding
                        (None, Normal, false, false, [],
                         PreXmlDoc ((62,4), FSharp.Compiler.Xml.XmlDocCollector),
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
                              ([this; Abs111], [(62,15--62,16)], [None; None]),
                            None, None, Pats [], None, (62,11--62,22)), None,
                         App
                           (Atomic, false, Ident Abs,
                            Paren
                              (Ident this, (62,28--62,29), Some (62,33--62,34),
                               (62,28--62,34)), (62,25--62,34)), (62,11--62,22),
                         NoneAtInvisible,
                         { LeadingKeyword = Member (62,4--62,10)
                           InlineKeyword = None
                           EqualsRange = Some (62,23--62,24) }), (62,4--62,34));
                   Open
                     (Type
                        (LongIdent
                           (SynLongIdent
                              ([System; Math], [(63,20--63,21)], [None; None])),
                         (63,14--63,25)), (63,4--63,25))], None, (61,5--63,25),
                  { LeadingKeyword = Type (61,0--61,4)
                    EqualsRange = None
                    WithKeyword = None })], (61,0--63,25))],
          PreXmlDoc ((2,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (2,0--63,25), { LeadingKeyword = Module (2,0--2,6) })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [LineComment (1,0--1,57)] }, set []))
