ImplFile
  (ParsedImplFileInput
     ("/root/OpenDeclaration/openInTypeDecl.fs", false,
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
                        (ModuleOrNamespace
                           (SynLongIdent
                              ([`global`; System], [(5,15--5,16)],
                               [Some (OriginalNotation "global"); None]),
                            (5,9--5,22)), (5,4--5,22));
                      Open
                        (Type
                           (LongIdent (SynLongIdent ([DateTime], [], [None])),
                            (6,14--6,22)), (6,4--6,22));
                      ImplicitInherit
                        (LongIdent (SynLongIdent ([Object], [], [None])),
                         Const (Unit, (8,18--8,20)), None, (8,4--8,20),
                         { InheritKeyword = (8,4--8,11) });
                      ValField
                        (SynField
                           ([{ Attributes =
                                [{ TypeName =
                                    SynLongIdent ([DefaultValue], [], [None])
                                   ArgExpr = Const (Unit, (10,6--10,18))
                                   Target = None
                                   AppliesToGetterAndSetter = false
                                   Range = (10,6--10,18) }]
                               Range = (10,4--10,20) }], false, Some x,
                            LongIdent (SynLongIdent ([Int64], [], [None])), true,
                            PreXmlDoc ((10,4), FSharp.Compiler.Xml.XmlDocCollector),
                            None, (10,4--10,41),
                            { LeadingKeyword = Some (Val (10,21--10,24))
                              MutableKeyword = Some (10,25--10,32) }),
                         (10,4--10,41));
                      LetBindings
                        ([SynBinding
                            (None, Normal, false, false, [],
                             PreXmlDoc ((11,4), FSharp.Compiler.Xml.XmlDocCollector),
                             SynValData
                               (None,
                                SynValInfo ([], SynArgInfo ([], false, None)),
                                None),
                             Named
                               (SynIdent (x, None), false, None, (11,8--11,9)),
                             None, Const (Int32 42, (11,12--11,14)),
                             (11,8--11,9), Yes (11,4--11,14),
                             { LeadingKeyword = Let (11,4--11,7)
                               InlineKeyword = None
                               EqualsRange = Some (11,10--11,11) })], false,
                         false, (11,4--11,14));
                      LetBindings
                        ([SynBinding
                            (None, Normal, false, false, [],
                             PreXmlDoc ((12,4), FSharp.Compiler.Xml.XmlDocCollector),
                             SynValData
                               (None,
                                SynValInfo ([], SynArgInfo ([], false, None)),
                                None),
                             Named
                               (SynIdent (timeConstructed, None), false, None,
                                (12,8--12,23)), None,
                             LongIdent
                               (false,
                                SynLongIdent
                                  ([Now; Ticks], [(12,29--12,30)], [None; None]),
                                None, (12,26--12,35)), (12,8--12,23),
                             Yes (12,4--12,35),
                             { LeadingKeyword = Let (12,4--12,7)
                               InlineKeyword = None
                               EqualsRange = Some (12,24--12,25) })], false,
                         false, (12,4--12,35));
                      LetBindings
                        ([SynBinding
                            (None, Do, false, false, [], PreXmlDocEmpty,
                             SynValData
                               (None,
                                SynValInfo ([], SynArgInfo ([], false, None)),
                                None), Const (Unit, (13,4--13,34)), None,
                             App
                               (NonAtomic, false,
                                App
                                  (NonAtomic, false, Ident printfn,
                                   Const
                                     (String ("%d", Regular, (13,15--13,19)),
                                      (13,15--13,19)), (13,7--13,19)),
                                LongIdent
                                  (false,
                                   SynLongIdent
                                     ([Int32; MaxValue], [(13,25--13,26)],
                                      [None; None]), None, (13,20--13,34)),
                                (13,7--13,34)), (13,4--13,34), NoneAtDo,
                             { LeadingKeyword = Do (13,4--13,6)
                               InlineKeyword = None
                               EqualsRange = None })], false, false,
                         (13,4--13,34));
                      Member
                        (SynBinding
                           (None, Normal, false, false, [],
                            PreXmlDoc ((14,4), FSharp.Compiler.Xml.XmlDocCollector),
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
                                    (Const (Unit, (14,22--14,24)),
                                     (14,22--14,24))], None, (14,18--14,24)),
                            None,
                            LongIdent
                              (false,
                               SynLongIdent
                                 ([DateTime; Now], [(14,35--14,36)],
                                  [None; None]), None, (14,27--14,39)),
                            (14,18--14,24), NoneAtInvisible,
                            { LeadingKeyword =
                               StaticMember ((14,4--14,10), (14,11--14,17))
                              InlineKeyword = None
                              EqualsRange = Some (14,25--14,26) }),
                         (14,4--14,39));
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
                         PreXmlDoc ((15,4), FSharp.Compiler.Xml.XmlDocCollector),
                         GetSet (None, None, None), Ident timeConstructed,
                         (15,4--15,62),
                         { LeadingKeyword =
                            MemberVal ((15,4--15,10), (15,11--15,14))
                           WithKeyword = Some (15,49--15,53)
                           EqualsRange = Some (15,31--15,32)
                           GetSetKeywords =
                            Some (GetSet ((15,54--15,57), (15,59--15,62))) });
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
                                 ([_; M], [(17,12--17,13)], [None; None]), None,
                               None,
                               Pats
                                 [Paren
                                    (Const (Unit, (17,14--17,16)),
                                     (17,14--17,16))], None, (17,11--17,16)),
                            None,
                            Open
                              (Type
                                 (LongIdent
                                    (SynLongIdent
                                       ([System; ArgumentException],
                                        [(18,24--18,25)], [None; None])),
                                  (18,18--18,42)), (18,8--18,42),
                               LongIdent
                                 (false,
                                  SynLongIdent
                                    ([Int32; MaxValue], [(19,13--19,14)],
                                     [None; None]), None, (19,8--19,22))),
                            (17,11--17,16), NoneAtInvisible,
                            { LeadingKeyword = Member (17,4--17,10)
                              InlineKeyword = None
                              EqualsRange = Some (17,17--17,18) }),
                         (17,4--18,42));
                      Interface
                        (LongIdent (SynLongIdent ([IDisposable], [], [None])),
                         Some (21,26--21,30),
                         Some
                           [Member
                              (SynBinding
                                 (None, Normal, false, false, [],
                                  PreXmlDoc ((22,8), FSharp.Compiler.Xml.XmlDocCollector),
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
                                       ([this; Dispose], [(22,19--22,20)],
                                        [None; None]), None, None,
                                     Pats
                                       [Paren
                                          (Const (Unit, (22,28--22,30)),
                                           (22,28--22,30))], None,
                                     (22,15--22,30)),
                                  Some
                                    (SynBindingReturnInfo
                                       (LongIdent
                                          (SynLongIdent ([unit], [], [None])),
                                        (22,32--22,36), [],
                                        { ColonRange = Some (22,30--22,31) })),
                                  Typed
                                    (App
                                       (NonAtomic, false, Ident raise,
                                        Paren
                                          (App
                                             (Atomic, false,
                                              Ident NotImplementedException,
                                              Const (Unit, (23,42--23,44)),
                                              (23,19--23,44)), (23,18--23,19),
                                           Some (23,44--23,45), (23,18--23,45)),
                                        (23,12--23,45)),
                                     LongIdent
                                       (SynLongIdent ([unit], [], [None])),
                                     (23,12--23,45)), (22,15--22,30),
                                  NoneAtInvisible,
                                  { LeadingKeyword = Member (22,8--22,14)
                                    InlineKeyword = None
                                    EqualsRange = Some (22,37--22,38) }),
                               (22,8--23,45))], (21,4--23,45))], (5,4--23,45)),
                  [],
                  Some
                    (ImplicitCtor
                       (None, [], Const (Unit, (4,8--4,10)), None,
                        PreXmlDoc ((4,8), FSharp.Compiler.Xml.XmlDocCollector),
                        (4,5--4,8), { AsKeyword = None })), (4,5--23,45),
                  { LeadingKeyword = Type (4,0--4,4)
                    EqualsRange = Some (4,11--4,12)
                    WithKeyword = None })], (4,0--23,45));
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
                  [Open
                     (ModuleOrNamespace
                        (SynLongIdent ([System], [], [None]), (27,13--27,19)),
                      (27,8--27,19));
                   GetSetMember
                     (Some
                        (SynBinding
                           (None, Normal, false, false, [],
                            PreXmlMerge
  (PreXmlDoc ((28,8), FSharp.Compiler.Xml.XmlDocCollector), PreXmlDocEmpty),
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
                                 ([_; RandomNumber], [(28,16--28,17)],
                                  [None; None]), Some get, None,
                               Pats
                                 [Paren
                                    (Const (Unit, (28,38--28,40)),
                                     (28,38--28,40))], None, (28,35--28,40)),
                            None,
                            App
                              (Atomic, false,
                               DotGet
                                 (App
                                    (Atomic, false, Ident Random,
                                     Const (Unit, (28,49--28,51)),
                                     (28,43--28,51)), (28,51--28,52),
                                  SynLongIdent ([Next], [], [None]),
                                  (28,43--28,56)), Const (Unit, (28,56--28,58)),
                               (28,43--28,58)), (28,35--28,40), NoneAtInvisible,
                            { LeadingKeyword = Member (28,8--28,14)
                              InlineKeyword = None
                              EqualsRange = Some (28,41--28,42) })), None,
                      (28,8--28,58), { InlineKeyword = None
                                       WithKeyword = (28,30--28,34)
                                       GetKeyword = Some (28,35--28,38)
                                       AndKeyword = None
                                       SetKeyword = None })], None,
                  (25,5--28,58), { LeadingKeyword = Type (25,0--25,4)
                                   EqualsRange = Some (25,7--25,8)
                                   WithKeyword = Some (26,4--26,8) })],
              (25,0--28,58));
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
                  [Open
                     (ModuleOrNamespace
                        (SynLongIdent ([System], [], [None]), (32,13--32,19)),
                      (32,8--32,19));
                   GetSetMember
                     (Some
                        (SynBinding
                           (None, Normal, false, false, [],
                            PreXmlMerge
  (PreXmlDoc ((33,8), FSharp.Compiler.Xml.XmlDocCollector), PreXmlDocEmpty),
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
                                 ([_; RandomNumber], [(33,16--33,17)],
                                  [None; None]), Some get, None,
                               Pats
                                 [Paren
                                    (Const (Unit, (33,38--33,40)),
                                     (33,38--33,40))], None, (33,35--33,40)),
                            None,
                            App
                              (Atomic, false,
                               DotGet
                                 (App
                                    (Atomic, false, Ident Random,
                                     Const (Unit, (33,49--33,51)),
                                     (33,43--33,51)), (33,51--33,52),
                                  SynLongIdent ([Next], [], [None]),
                                  (33,43--33,56)), Const (Unit, (33,56--33,58)),
                               (33,43--33,58)), (33,35--33,40), NoneAtInvisible,
                            { LeadingKeyword = Member (33,8--33,14)
                              InlineKeyword = None
                              EqualsRange = Some (33,41--33,42) })), None,
                      (33,8--33,58), { InlineKeyword = None
                                       WithKeyword = (33,30--33,34)
                                       GetKeyword = Some (33,35--33,38)
                                       AndKeyword = None
                                       SetKeyword = None })], None,
                  (30,5--33,58), { LeadingKeyword = Type (30,0--30,4)
                                   EqualsRange = Some (30,13--30,14)
                                   WithKeyword = Some (31,4--31,8) })],
              (30,0--33,58));
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
                 [Open
                    (ModuleOrNamespace
                       (SynLongIdent ([System], [], [None]), (37,13--37,19)),
                     (37,8--37,19));
                  GetSetMember
                    (Some
                       (SynBinding
                          (None, Normal, false, false, [],
                           PreXmlMerge
  (PreXmlDoc ((38,8), FSharp.Compiler.Xml.XmlDocCollector), PreXmlDocEmpty),
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
                                ([_; RandomNumber], [(38,16--38,17)],
                                 [None; None]), Some get, None,
                              Pats
                                [Paren
                                   (Const (Unit, (38,38--38,40)), (38,38--38,40))],
                              None, (38,35--38,40)), None,
                           App
                             (Atomic, false,
                              DotGet
                                (App
                                   (Atomic, false, Ident Random,
                                    Const (Unit, (38,49--38,51)), (38,43--38,51)),
                                 (38,51--38,52),
                                 SynLongIdent ([Next], [], [None]),
                                 (38,43--38,56)), Const (Unit, (38,56--38,58)),
                              (38,43--38,58)), (38,35--38,40), NoneAtInvisible,
                           { LeadingKeyword = Member (38,8--38,14)
                             InlineKeyword = None
                             EqualsRange = Some (38,41--38,42) })), None,
                     (38,8--38,58), { InlineKeyword = None
                                      WithKeyword = (38,30--38,34)
                                      GetKeyword = Some (38,35--38,38)
                                      AndKeyword = None
                                      SetKeyword = None })], (35,0--38,58)),
              (35,0--38,58));
           Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([{ Attributes =
                         [{ TypeName = SynLongIdent ([Struct], [], [None])
                            ArgExpr = Const (Unit, (40,2--40,8))
                            Target = None
                            AppliesToGetterAndSetter = false
                            Range = (40,2--40,8) }]
                        Range = (40,0--40,10) }], None, [], [ABC],
                     PreXmlDoc ((40,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (41,5--41,8)),
                  ObjectModel
                    (Unspecified,
                     [Open
                        (ModuleOrNamespace
                           (SynLongIdent ([System], [], [None]), (42,9--42,15)),
                         (42,4--42,15));
                      ValField
                        (SynField
                           ([], false, Some a,
                            LongIdent (SynLongIdent ([Int32], [], [None])),
                            false,
                            PreXmlDoc ((43,4), FSharp.Compiler.Xml.XmlDocCollector),
                            None, (43,4--43,16),
                            { LeadingKeyword = Some (Val (43,4--43,7))
                              MutableKeyword = None }), (43,4--43,16));
                      ValField
                        (SynField
                           ([], false, Some b,
                            LongIdent (SynLongIdent ([Int32], [], [None])),
                            false,
                            PreXmlDoc ((44,4), FSharp.Compiler.Xml.XmlDocCollector),
                            None, (44,4--44,16),
                            { LeadingKeyword = Some (Val (44,4--44,7))
                              MutableKeyword = None }), (44,4--44,16));
                      Member
                        (SynBinding
                           (None, Normal, false, false, [],
                            PreXmlDoc ((45,4), FSharp.Compiler.Xml.XmlDocCollector),
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
                                        (45,9--45,10)), (45,8--45,11))], None,
                               (45,4--45,7)), None,
                            Open
                              (Type
                                 (LongIdent
                                    (SynLongIdent
                                       ([System; Int32], [(46,24--46,25)],
                                        [None; None])), (46,18--46,30)),
                               (46,8--46,30),
                               Record
                                 (None, None,
                                  [SynExprRecordField
                                     ((SynLongIdent ([a], [], [None]), true),
                                      Some (47,12--47,13), Some (Ident a),
                                      (47,10--47,15),
                                      Some ((47,15--47,16), Some (47,16)));
                                   SynExprRecordField
                                     ((SynLongIdent ([b], [], [None]), true),
                                      Some (47,19--47,20), Some (Ident MinValue),
                                      (47,17--47,29), None)], (47,8--47,31))),
                            (45,4--45,11), NoneAtInvisible,
                            { LeadingKeyword = New (45,4--45,7)
                              InlineKeyword = None
                              EqualsRange = Some (45,12--45,13) }),
                         (45,4--46,30))], (42,4--46,30)), [], None,
                  (40,0--46,30), { LeadingKeyword = Type (41,0--41,4)
                                   EqualsRange = Some (41,9--41,10)
                                   WithKeyword = None })], (40,0--46,30));
           Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([], None, [], [System; Int32],
                     PreXmlDoc ((49,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (49,5--49,17)),
                  ObjectModel (Augmentation (49,18--49,22), [], (49,5--51,34)),
                  [Open
                     (Type
                        (LongIdent
                           (SynLongIdent
                              ([System; Math], [(50,20--50,21)], [None; None])),
                         (50,14--50,25)), (50,4--50,25));
                   Member
                     (SynBinding
                        (None, Normal, false, false, [],
                         PreXmlDoc ((51,4), FSharp.Compiler.Xml.XmlDocCollector),
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
                              ([this; Abs111], [(51,15--51,16)], [None; None]),
                            None, None, Pats [], None, (51,11--51,22)), None,
                         App
                           (Atomic, false, Ident Abs,
                            Paren
                              (Ident this, (51,28--51,29), Some (51,33--51,34),
                               (51,28--51,34)), (51,25--51,34)), (51,11--51,22),
                         NoneAtInvisible,
                         { LeadingKeyword = Member (51,4--51,10)
                           InlineKeyword = None
                           EqualsRange = Some (51,23--51,24) }), (51,4--51,34))],
                  None, (49,5--51,34), { LeadingKeyword = Type (49,0--49,4)
                                         EqualsRange = None
                                         WithKeyword = None })], (49,0--51,34))],
          PreXmlDoc ((2,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (2,0--51,34), { LeadingKeyword = Module (2,0--2,6) })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [LineComment (1,0--1,57)] }, set []))
