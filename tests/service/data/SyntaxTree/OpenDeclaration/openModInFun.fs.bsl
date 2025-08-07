ImplFile
  (ParsedImplFileInput
     ("/root/OpenDeclaration/openModInFun.fs", false,
      QualifiedNameOfFile openModInFun, [],
      [SynModuleOrNamespace
         ([openModInFun], false, NamedModule,
          [Let
             (false,
              [SynBinding
                 (None, Normal, false, false, [],
                  PreXmlDoc ((4,0), FSharp.Compiler.Xml.XmlDocCollector),
                  SynValData
                    (None,
                     SynValInfo
                       ([[SynArgInfo ([], false, Some x)];
                         [SynArgInfo ([], false, Some y)]],
                        SynArgInfo ([], false, None)), None),
                  LongIdent
                    (SynLongIdent ([f], [], [None]), None, None,
                     Pats
                       [Named (SynIdent (x, None), false, None, (4,6--4,7));
                        Named (SynIdent (y, None), false, None, (4,8--4,9))],
                     None, (4,4--4,9)), None,
                  LetOrUse
                    (false, false,
                     [SynBinding
                        (None, Normal, false, false, [],
                         PreXmlDoc ((5,4), FSharp.Compiler.Xml.XmlDocCollector),
                         SynValData
                           (None, SynValInfo ([], SynArgInfo ([], false, None)),
                            None),
                         Named
                           (SynIdent (result, None), false, None, (5,8--5,14)),
                         None,
                         App
                           (NonAtomic, false,
                            App
                              (NonAtomic, true,
                               LongIdent
                                 (false,
                                  SynLongIdent
                                    ([op_Addition], [],
                                     [Some (OriginalNotation "+")]), None,
                                  (5,19--5,20)), Ident x, (5,17--5,20)), Ident y,
                            (5,17--5,22)), (5,8--5,14), Yes (5,4--5,22),
                         { LeadingKeyword = Let (5,4--5,7)
                           InlineKeyword = None
                           EqualsRange = Some (5,15--5,16) })],
                     Open
                       (ModuleOrNamespace
                          (SynLongIdent ([System], [], [None]), (6,9--6,15)),
                        (6,4--6,15), (6,4--10,32),
                        Sequential
                          (SuppressNeither, true,
                           App
                             (Atomic, false,
                              LongIdent
                                (false,
                                 SynLongIdent
                                   ([Console; WriteLine], [(7,11--7,12)],
                                    [None; None]), None, (7,4--7,21)),
                              Paren
                                (App
                                   (Atomic, false,
                                    LongIdent
                                      (false,
                                       SynLongIdent
                                         ([result; ToString], [(7,28--7,29)],
                                          [None; None]), None, (7,22--7,37)),
                                    Const (Unit, (7,37--7,39)), (7,22--7,39)),
                                 (7,21--7,22), Some (7,39--7,40), (7,21--7,40)),
                              (7,4--7,40)),
                           Open
                             (Type
                                (LongIdent
                                   (SynLongIdent ([Console], [], [None])),
                                 (9,14--9,21)), (9,4--9,21), (9,4--10,32),
                              App
                                (Atomic, false, Ident WriteLine,
                                 Paren
                                   (App
                                      (Atomic, false,
                                       LongIdent
                                         (false,
                                          SynLongIdent
                                            ([result; ToString],
                                             [(10,20--10,21)], [None; None]),
                                          None, (10,14--10,29)),
                                       Const (Unit, (10,29--10,31)),
                                       (10,14--10,31)), (10,13--10,14),
                                    Some (10,31--10,32), (10,13--10,32)),
                                 (10,4--10,32))), (7,4--10,32),
                           { SeparatorRange = None })), (5,4--10,32),
                     { LetOrUseKeyword = (5,4--5,7)
                       InKeyword = None
                       EqualsRange = Some (5,15--5,16) }), (4,4--4,9), NoneAtLet,
                  { LeadingKeyword = Let (4,0--4,3)
                    InlineKeyword = None
                    EqualsRange = Some (4,10--4,11) })], (4,0--10,32));
           Let
             (false,
              [SynBinding
                 (None, Normal, false, false, [],
                  PreXmlDoc ((12,0), FSharp.Compiler.Xml.XmlDocCollector),
                  SynValData
                    (None,
                     SynValInfo
                       ([[SynArgInfo ([], false, Some x)];
                         [SynArgInfo ([], false, Some y)]],
                        SynArgInfo ([], false, None)), None),
                  LongIdent
                    (SynLongIdent ([top], [], [None]), None, None,
                     Pats
                       [Named (SynIdent (x, None), false, None, (12,8--12,9));
                        Named (SynIdent (y, None), false, None, (12,10--12,11))],
                     None, (12,4--12,11)), None,
                  LetOrUse
                    (false, false,
                     [SynBinding
                        (None, Normal, false, false, [],
                         PreXmlDoc ((13,4), FSharp.Compiler.Xml.XmlDocCollector),
                         SynValData
                           (None, SynValInfo ([], SynArgInfo ([], false, None)),
                            None),
                         Named (SynIdent (r1, None), false, None, (13,8--13,10)),
                         None,
                         App
                           (NonAtomic, false,
                            App
                              (NonAtomic, true,
                               LongIdent
                                 (false,
                                  SynLongIdent
                                    ([op_Addition], [],
                                     [Some (OriginalNotation "+")]), None,
                                  (13,15--13,16)), Ident x, (13,13--13,16)),
                            Ident y, (13,13--13,18)), (13,8--13,10),
                         Yes (13,4--13,18),
                         { LeadingKeyword = Let (13,4--13,7)
                           InlineKeyword = None
                           EqualsRange = Some (13,11--13,12) })],
                     LetOrUse
                       (false, false,
                        [SynBinding
                           (None, Normal, false, false, [],
                            PreXmlDoc ((14,4), FSharp.Compiler.Xml.XmlDocCollector),
                            SynValData
                              (None,
                               SynValInfo ([], SynArgInfo ([], false, None)),
                               None),
                            Named
                              (SynIdent (r2, None), false, None, (14,8--14,10)),
                            None,
                            App
                              (NonAtomic, false,
                               App
                                 (NonAtomic, true,
                                  LongIdent
                                    (false,
                                     SynLongIdent
                                       ([op_Multiply], [],
                                        [Some (OriginalNotation "*")]), None,
                                     (14,15--14,16)), Ident x, (14,13--14,16)),
                               Ident y, (14,13--14,18)), (14,8--14,10),
                            Yes (14,4--14,18),
                            { LeadingKeyword = Let (14,4--14,7)
                              InlineKeyword = None
                              EqualsRange = Some (14,11--14,12) })],
                        LetOrUse
                          (false, false,
                           [SynBinding
                              (None, Normal, false, false, [],
                               PreXmlDoc ((15,4), FSharp.Compiler.Xml.XmlDocCollector),
                               SynValData
                                 (None,
                                  SynValInfo
                                    ([[SynArgInfo ([], false, Some x)];
                                      [SynArgInfo ([], false, Some y)]],
                                     SynArgInfo ([], false, None)), None),
                               LongIdent
                                 (SynLongIdent ([nested], [], [None]), None,
                                  None,
                                  Pats
                                    [Named
                                       (SynIdent (x, None), false, None,
                                        (15,15--15,16));
                                     Named
                                       (SynIdent (y, None), false, None,
                                        (15,17--15,18))], None, (15,8--15,18)),
                               None,
                               Open
                                 (ModuleOrNamespace
                                    (SynLongIdent ([System], [], [None]),
                                     (16,13--16,19)), (16,8--16,19),
                                  (16,8--17,40),
                                  App
                                    (Atomic, false,
                                     LongIdent
                                       (false,
                                        SynLongIdent
                                          ([Console; WriteLine],
                                           [(17,15--17,16)], [None; None]), None,
                                        (17,8--17,25)),
                                     Paren
                                       (App
                                          (Atomic, false,
                                           LongIdent
                                             (false,
                                              SynLongIdent
                                                ([r1; ToString],
                                                 [(17,28--17,29)], [None; None]),
                                              None, (17,26--17,37)),
                                           Const (Unit, (17,37--17,39)),
                                           (17,26--17,39)), (17,25--17,26),
                                        Some (17,39--17,40), (17,25--17,40)),
                                     (17,8--17,40))), (15,8--15,18), NoneAtLet,
                               { LeadingKeyword = Let (15,4--15,7)
                                 InlineKeyword = None
                                 EqualsRange = Some (15,19--15,20) })],
                           App
                             (NonAtomic, false,
                              App
                                (NonAtomic, false, Ident nested, Ident r1,
                                 (18,4--18,13)), Ident r2, (18,4--18,16)),
                           (15,4--18,16), { LetOrUseKeyword = (15,4--15,7)
                                            InKeyword = None
                                            EqualsRange = Some (15,19--15,20) }),
                        (14,4--18,16), { LetOrUseKeyword = (14,4--14,7)
                                         InKeyword = None
                                         EqualsRange = Some (14,11--14,12) }),
                     (13,4--18,16), { LetOrUseKeyword = (13,4--13,7)
                                      InKeyword = None
                                      EqualsRange = Some (13,11--13,12) }),
                  (12,4--12,11), NoneAtLet,
                  { LeadingKeyword = Let (12,0--12,3)
                    InlineKeyword = None
                    EqualsRange = Some (12,12--12,13) })], (12,0--18,16));
           Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([], None, [], [Foo],
                     PreXmlDoc ((20,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (20,5--20,8)),
                  ObjectModel
                    (Unspecified,
                     [ImplicitCtor
                        (None, [], Const (Unit, (20,8--20,10)), None,
                         PreXmlDoc ((20,8), FSharp.Compiler.Xml.XmlDocCollector),
                         (20,5--20,8), { AsKeyword = None });
                      LetBindings
                        ([SynBinding
                            (None, Do, false, false, [], PreXmlDocEmpty,
                             SynValData
                               (None,
                                SynValInfo ([], SynArgInfo ([], false, None)),
                                None), Const (Unit, (21,4--24,21)), None,
                             Open
                               (ModuleOrNamespace
                                  (SynLongIdent ([System], [], [None]),
                                   (22,13--22,19)), (22,8--22,19), (22,8--24,21),
                                Open
                                  (Type
                                     (LongIdent
                                        (SynLongIdent ([Console], [], [None])),
                                      (23,18--23,25)), (23,8--23,25),
                                   (23,8--24,21),
                                   App
                                     (NonAtomic, false, Ident WriteLine,
                                      Const (Int32 123, (24,18--24,21)),
                                      (24,8--24,21)))), (21,4--24,21), NoneAtDo,
                             { LeadingKeyword = Do (21,4--21,6)
                               InlineKeyword = None
                               EqualsRange = None })], false, false,
                         (21,4--24,21));
                      Member
                        (SynBinding
                           (None, Normal, false, false, [],
                            PreXmlDoc ((25,4), FSharp.Compiler.Xml.XmlDocCollector),
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
                                 ([this; PrintHello], [(25,22--25,23)],
                                  [None; None]), None, None,
                               Pats
                                 [Paren
                                    (Const (Unit, (25,33--25,35)),
                                     (25,33--25,35))],
                               Some (Public (25,11--25,17)), (25,11--25,35)),
                            None,
                            Open
                              (ModuleOrNamespace
                                 (SynLongIdent ([System], [], [None]),
                                  (26,13--26,19)), (26,8--26,19), (26,8--27,35),
                               App
                                 (Atomic, false,
                                  LongIdent
                                    (false,
                                     SynLongIdent
                                       ([Console; WriteLine], [(27,15--27,16)],
                                        [None; None]), None, (27,8--27,25)),
                                  Paren
                                    (Const
                                       (String
                                          ("Hello!", Regular, (27,26--27,34)),
                                        (27,26--27,34)), (27,25--27,26),
                                     Some (27,34--27,35), (27,25--27,35)),
                                  (27,8--27,35))), (25,11--25,35),
                            NoneAtInvisible,
                            { LeadingKeyword = Member (25,4--25,10)
                              InlineKeyword = None
                              EqualsRange = Some (25,36--25,37) }),
                         (25,4--27,35))], (21,4--27,35)), [],
                  Some
                    (ImplicitCtor
                       (None, [], Const (Unit, (20,8--20,10)), None,
                        PreXmlDoc ((20,8), FSharp.Compiler.Xml.XmlDocCollector),
                        (20,5--20,8), { AsKeyword = None })), (20,5--27,35),
                  { LeadingKeyword = Type (20,0--20,4)
                    EqualsRange = Some (20,11--20,12)
                    WithKeyword = None })], (20,0--27,35));
           Expr
             (Paren
                (Open
                   (Type
                      (LongIdent
                         (SynLongIdent
                            ([System; Console], [(32,14--32,15)], [None; None])),
                       (32,8--32,22)), (31,4--32,22), (31,4--33,15),
                    App
                      (Atomic, false, Ident WriteLine,
                       Const (Unit, (33,13--33,15)), (33,4--33,15))),
                 (30,0--30,1), Some (34,0--34,1), (30,0--34,1)), (30,0--34,1));
           Expr
             (Match
                (Yes (38,0--38,17),
                 App
                   (NonAtomic, false, Ident Some,
                    Const (Int32 1, (38,11--38,12)), (38,6--38,12)),
                 [SynMatchClause
                    (LongIdent
                       (SynLongIdent ([Some], [], [None]), None, None,
                        Pats [Const (Int32 1, (39,7--39,8))], None, (39,2--39,8)),
                     Some
                       (Open
                          (ModuleOrNamespace
                             (SynLongIdent ([System], [], [None]),
                              (39,19--39,25)), (39,14--39,25), (39,14--39,45),
                           App
                             (NonAtomic, false,
                              App
                                (NonAtomic, true,
                                 LongIdent
                                   (false,
                                    SynLongIdent
                                      ([op_LessThan], [],
                                       [Some (OriginalNotation "<")]), None,
                                    (39,42--39,43)),
                                 LongIdent
                                   (false,
                                    SynLongIdent
                                      ([Int32; MinValue], [(39,32--39,33)],
                                       [None; None]), None, (39,27--39,41)),
                                 (39,27--39,43)),
                              Const (Int32 0, (39,44--39,45)), (39,27--39,45)))),
                     Open
                       (Type
                          (LongIdent
                             (SynLongIdent
                                ([System; Console], [(40,20--40,21)],
                                 [None; None])), (40,14--40,28)), (40,4--40,28),
                        (40,4--41,20),
                        App
                          (NonAtomic, false, Ident WriteLine,
                           Const
                             (String ("Is 1", Regular, (41,14--41,20)),
                              (41,14--41,20)), (41,4--41,20))), (39,2--41,20),
                     Yes, { ArrowRange = Some (39,46--39,48)
                            BarRange = Some (39,0--39,1) });
                  SynMatchClause
                    (Wild (42,2--42,3), None, Const (Unit, (42,7--42,9)),
                     (42,2--42,9), Yes, { ArrowRange = Some (42,4--42,6)
                                          BarRange = Some (42,0--42,1) })],
                 (38,0--42,9), { MatchKeyword = (38,0--38,5)
                                 WithKeyword = (38,13--38,17) }), (38,0--42,9));
           Expr
             (ForEach
                (Yes (45,0--45,3), Yes (45,6--45,8), SeqExprOnly false, true,
                 Wild (45,4--45,5),
                 Open
                   (ModuleOrNamespace
                      (SynLongIdent
                         ([System; Linq], [(45,20--45,21)], [None; None]),
                       (45,14--45,25)), (45,9--45,25), (45,9--45,50),
                    App
                      (Atomic, false,
                       LongIdent
                         (false,
                          SynLongIdent
                            ([Enumerable; Range], [(45,37--45,38)], [None; None]),
                          None, (45,27--45,43)),
                       Paren
                         (Tuple
                            (false,
                             [Const (Int32 0, (45,44--45,45));
                              Const (Int32 10, (45,47--45,49))],
                             [(45,45--45,46)], (45,44--45,49)), (45,43--45,44),
                          Some (45,49--45,50), (45,43--45,50)), (45,27--45,50))),
                 Open
                   (Type
                      (LongIdent
                         (SynLongIdent
                            ([System; Console], [(46,20--46,21)], [None; None])),
                       (46,14--46,28)), (46,4--46,28), (46,4--47,29),
                    App
                      (NonAtomic, false, Ident WriteLine,
                       Const
                         (String ("Hello, World!", Regular, (47,14--47,29)),
                          (47,14--47,29)), (47,4--47,29))), (45,0--47,29)),
              (45,0--47,29));
           Expr
             (While
                (Yes (50,0--52,18),
                 Paren
                   (Open
                      (Type
                         (LongIdent
                            (SynLongIdent
                               ([System; Int32], [(51,21--51,22)], [None; None])),
                          (51,15--51,27)), (51,5--51,27), (51,5--52,17),
                       App
                         (NonAtomic, false,
                          App
                            (NonAtomic, true,
                             LongIdent
                               (false,
                                SynLongIdent
                                  ([op_LessThan], [],
                                   [Some (OriginalNotation "<")]), None,
                                (52,14--52,15)), Ident MaxValue, (52,5--52,15)),
                          Const (Int32 0, (52,16--52,17)), (52,5--52,17))),
                    (51,4--51,5), Some (52,17--52,18), (51,4--52,18)),
                 Open
                   (Type
                      (LongIdent
                         (SynLongIdent
                            ([System; Console], [(53,20--53,21)], [None; None])),
                       (53,14--53,28)), (53,4--53,28), (53,4--54,36),
                    App
                      (NonAtomic, false, Ident WriteLine,
                       Const
                         (String
                            ("MaxValue is negative", Regular, (54,14--54,36)),
                          (54,14--54,36)), (54,4--54,36))), (50,0--54,36)),
              (50,0--54,36));
           Expr
             (IfThenElse
                (Paren
                   (Open
                      (Type
                         (LongIdent
                            (SynLongIdent
                               ([System; Int32], [(57,20--57,21)], [None; None])),
                          (57,14--57,26)), (57,4--57,26), (57,4--57,48),
                       App
                         (NonAtomic, false,
                          App
                            (NonAtomic, true,
                             LongIdent
                               (false,
                                SynLongIdent
                                  ([op_Inequality], [],
                                   [Some (OriginalNotation "<>")]), None,
                                (57,37--57,39)), Ident MaxValue, (57,28--57,39)),
                          Ident MinValue, (57,28--57,48))), (57,3--57,4),
                    Some (57,48--57,49), (57,3--57,49)),
                 Open
                   (Type
                      (LongIdent
                         (SynLongIdent
                            ([System; Console], [(58,20--58,21)], [None; None])),
                       (58,14--58,28)), (58,4--58,28), (58,4--59,49),
                    App
                      (NonAtomic, false, Ident WriteLine,
                       Const
                         (String
                            ("MaxValue is not equal to MinValue", Regular,
                             (59,14--59,49)), (59,14--59,49)), (59,4--59,49))),
                 Some
                   (IfThenElse
                      (Paren
                         (Open
                            (Type
                               (LongIdent
                                  (SynLongIdent
                                     ([System; Int32], [(60,22--60,23)],
                                      [None; None])), (60,16--60,28)),
                             (60,6--60,28), (60,6--60,42),
                             App
                               (NonAtomic, false,
                                App
                                  (NonAtomic, true,
                                   LongIdent
                                     (false,
                                      SynLongIdent
                                        ([op_LessThan], [],
                                         [Some (OriginalNotation "<")]), None,
                                      (60,39--60,40)), Ident MaxValue,
                                   (60,30--60,40)),
                                Const (Int32 0, (60,41--60,42)), (60,30--60,42))),
                          (60,5--60,6), Some (60,42--60,43), (60,5--60,43)),
                       Open
                         (Type
                            (LongIdent
                               (SynLongIdent
                                  ([System; Console], [(61,20--61,21)],
                                   [None; None])), (61,14--61,28)),
                          (61,4--61,28), (61,4--62,36),
                          App
                            (NonAtomic, false, Ident WriteLine,
                             Const
                               (String
                                  ("MaxValue is negative", Regular,
                                   (62,14--62,36)), (62,14--62,36)),
                             (62,4--62,36))),
                       Some
                         (Open
                            (Type
                               (LongIdent
                                  (SynLongIdent
                                     ([System; Console], [(64,20--64,21)],
                                      [None; None])), (64,14--64,28)),
                             (64,4--64,28), (64,4--65,36),
                             App
                               (NonAtomic, false, Ident WriteLine,
                                Const
                                  (String
                                     ("MaxValue is positive", Regular,
                                      (65,14--65,36)), (65,14--65,36)),
                                (65,4--65,36)))), Yes (60,0--60,48), false,
                       (60,0--65,36), { IfKeyword = (60,0--60,4)
                                        IsElif = true
                                        ThenKeyword = (60,44--60,48)
                                        ElseKeyword = Some (63,0--63,4)
                                        IfToThenRange = (60,0--60,48) })),
                 Yes (57,0--57,54), false, (57,0--65,36),
                 { IfKeyword = (57,0--57,2)
                   IsElif = false
                   ThenKeyword = (57,50--57,54)
                   ElseKeyword = None
                   IfToThenRange = (57,0--57,54) }), (57,0--65,36));
           Expr
             (TryWith
                (Open
                   (Type
                      (LongIdent
                         (SynLongIdent
                            ([System; Int32], [(69,20--69,21)], [None; None])),
                       (69,14--69,26)), (69,4--69,26), (69,4--71,24),
                    Open
                      (ModuleOrNamespace
                         (SynLongIdent ([Checked], [], [None]), (70,9--70,16)),
                       (70,4--70,16), (70,4--71,24),
                       App
                         (Atomic, false, Ident ignore,
                          Paren
                            (App
                               (NonAtomic, false,
                                App
                                  (NonAtomic, true,
                                   LongIdent
                                     (false,
                                      SynLongIdent
                                        ([op_Addition], [],
                                         [Some (OriginalNotation "+")]), None,
                                      (71,20--71,21)), Ident MaxValue,
                                   (71,11--71,21)),
                                Const (Int32 1, (71,22--71,23)), (71,11--71,23)),
                             (71,10--71,11), Some (71,23--71,24), (71,10--71,24)),
                          (71,4--71,24)))),
                 [SynMatchClause
                    (Named (SynIdent (exn, None), false, None, (72,7--72,10)),
                     None,
                     Open
                       (Type
                          (LongIdent
                             (SynLongIdent
                                ([System; Console], [(72,30--72,31)],
                                 [None; None])), (72,24--72,38)), (72,14--72,38),
                        (72,14--72,61),
                        App
                          (NonAtomic, false, Ident WriteLine,
                           LongIdent
                             (false,
                              SynLongIdent
                                ([exn; Message], [(72,53--72,54)], [None; None]),
                              None, (72,50--72,61)), (72,40--72,61))),
                     (72,7--72,61), Yes, { ArrowRange = Some (72,11--72,13)
                                           BarRange = Some (72,5--72,6) })],
                 (68,0--72,61), Yes (68,0--68,3), Yes (72,0--72,4),
                 { TryKeyword = (68,0--68,3)
                   TryToWithRange = (68,0--72,4)
                   WithKeyword = (72,0--72,4)
                   WithToEndRange = (72,0--72,61) }), (68,0--72,61));
           Let
             (false,
              [SynBinding
                 (None, Normal, false, false, [],
                  PreXmlDoc ((75,0), FSharp.Compiler.Xml.XmlDocCollector),
                  SynValData
                    (None,
                     SynValInfo
                       ([[SynArgInfo ([], false, Some x)]],
                        SynArgInfo ([], false, None)), None),
                  Named (SynIdent (fun1, None), false, None, (75,4--75,8)), None,
                  Lambda
                    (false, false,
                     SimplePats
                       ([Id (x, None, false, false, false, (75,15--75,16))], [],
                        (75,15--75,16)),
                     Open
                       (ModuleOrNamespace
                          (SynLongIdent ([System], [], [None]), (75,25--75,31)),
                        (75,20--75,31), (75,20--75,38),
                        App
                          (NonAtomic, false,
                           App
                             (NonAtomic, true,
                              LongIdent
                                (false,
                                 SynLongIdent
                                   ([op_Addition], [],
                                    [Some (OriginalNotation "+")]), None,
                                 (75,35--75,36)), Ident x, (75,33--75,36)),
                           Const (Int32 1, (75,37--75,38)), (75,33--75,38))),
                     Some
                       ([Named (SynIdent (x, None), false, None, (75,15--75,16))],
                        Open
                          (ModuleOrNamespace
                             (SynLongIdent ([System], [], [None]),
                              (75,25--75,31)), (75,20--75,31), (75,20--75,38),
                           App
                             (NonAtomic, false,
                              App
                                (NonAtomic, true,
                                 LongIdent
                                   (false,
                                    SynLongIdent
                                      ([op_Addition], [],
                                       [Some (OriginalNotation "+")]), None,
                                    (75,35--75,36)), Ident x, (75,33--75,36)),
                              Const (Int32 1, (75,37--75,38)), (75,33--75,38)))),
                     (75,11--75,38), { ArrowRange = Some (75,17--75,19) }),
                  (75,4--75,8), NoneAtLet, { LeadingKeyword = Let (75,0--75,3)
                                             InlineKeyword = None
                                             EqualsRange = Some (75,9--75,10) })],
              (75,0--75,38));
           Let
             (false,
              [SynBinding
                 (None, Normal, false, false, [],
                  PreXmlDoc ((76,0), FSharp.Compiler.Xml.XmlDocCollector),
                  SynValData
                    (None, SynValInfo ([], SynArgInfo ([], false, None)), None),
                  Named (SynIdent (fun2, None), false, None, (76,4--76,8)), None,
                  MatchLambda
                    (false, (76,11--76,19),
                     [SynMatchClause
                        (Named (SynIdent (x, None), false, None, (76,20--76,21)),
                         None,
                         Open
                           (Type
                              (LongIdent
                                 (SynLongIdent
                                    ([System; Int32], [(76,41--76,42)],
                                     [None; None])), (76,35--76,47)),
                            (76,25--76,47), (76,25--76,61),
                            App
                              (NonAtomic, false,
                               App
                                 (NonAtomic, true,
                                  LongIdent
                                    (false,
                                     SynLongIdent
                                       ([op_Addition], [],
                                        [Some (OriginalNotation "+")]), None,
                                     (76,51--76,52)), Ident x, (76,49--76,52)),
                               Ident MinValue, (76,49--76,61))), (76,20--76,61),
                         Yes, { ArrowRange = Some (76,22--76,24)
                                BarRange = None })], NoneAtInvisible,
                     (76,11--76,61)), (76,4--76,8), NoneAtLet,
                  { LeadingKeyword = Let (76,0--76,3)
                    InlineKeyword = None
                    EqualsRange = Some (76,9--76,10) })], (76,0--76,61));
           Let
             (false,
              [SynBinding
                 (None, Normal, false, false, [],
                  PreXmlDoc ((79,0), FSharp.Compiler.Xml.XmlDocCollector),
                  SynValData
                    (None, SynValInfo ([], SynArgInfo ([], false, None)), None),
                  Named (SynIdent (res, None), false, None, (79,4--79,7)), None,
                  App
                    (NonAtomic, false, Ident async,
                     ComputationExpr
                       (false,
                        Open
                          (ModuleOrNamespace
                             (SynLongIdent ([System], [], [None]), (80,9--80,15)),
                           (80,4--80,15), (80,4--83,12),
                           Sequential
                             (SuppressNeither, true,
                              App
                                (Atomic, false,
                                 LongIdent
                                   (false,
                                    SynLongIdent
                                      ([Console; WriteLine], [(81,11--81,12)],
                                       [None; None]), None, (81,4--81,21)),
                                 Paren
                                   (Const
                                      (String
                                         ("Hello, World!", Regular,
                                          (81,22--81,37)), (81,22--81,37)),
                                    (81,21--81,22), Some (81,37--81,38),
                                    (81,21--81,38)), (81,4--81,38)),
                              LetOrUseBang
                                (Yes (82,4--82,29), false, true,
                                 Named
                                   (SynIdent (x, None), false, None,
                                    (82,9--82,10)),
                                 App
                                   (NonAtomic, false,
                                    LongIdent
                                      (false,
                                       SynLongIdent
                                         ([Async; Sleep], [(82,18--82,19)],
                                          [None; None]), None, (82,13--82,24)),
                                    Const (Int32 1000, (82,25--82,29)),
                                    (82,13--82,29)), [],
                                 YieldOrReturn
                                   ((false, true), Ident x, (83,4--83,12),
                                    { YieldOrReturnKeyword = (83,4--83,10) }),
                                 (82,4--83,12),
                                 { LetOrUseKeyword = (82,4--82,8)
                                   InKeyword = None
                                   EqualsRange = Some (82,11--82,12) }),
                              (81,4--83,12), { SeparatorRange = None })),
                        (79,16--84,1)), (79,10--84,1)), (79,4--79,7), NoneAtLet,
                  { LeadingKeyword = Let (79,0--79,3)
                    InlineKeyword = None
                    EqualsRange = Some (79,8--79,9) })], (79,0--84,1));
           Let
             (false,
              [SynBinding
                 (None, Normal, false, false, [],
                  PreXmlDoc ((87,0), FSharp.Compiler.Xml.XmlDocCollector),
                  SynValData
                    (None, SynValInfo ([], SynArgInfo ([], false, None)), None),
                  Named (SynIdent (q, None), false, None, (87,4--87,5)), None,
                  App
                    (NonAtomic, false,
                     App
                       (NonAtomic, true,
                        LongIdent
                          (false,
                           SynLongIdent
                             ([op_PipeRight], [], [Some (OriginalNotation "|>")]),
                           None, (93,6--93,8)),
                        App
                          (NonAtomic, false, Ident query,
                           ComputationExpr
                             (false,
                              Open
                                (Type
                                   (LongIdent
                                      (SynLongIdent
                                         ([System; Linq; Enumerable],
                                          [(89,24--89,25); (89,29--89,30)],
                                          [None; None; None])), (89,18--89,40)),
                                 (89,8--89,40), (89,8--92,30),
                                 ForEach
                                   (Yes (90,8--90,11), Yes (90,14--90,16),
                                    SeqExprOnly false, true,
                                    Named
                                      (SynIdent (i, None), false, None,
                                       (90,12--90,13)),
                                    App
                                      (Atomic, false, Ident Range,
                                       Paren
                                         (Tuple
                                            (false,
                                             [Const (Int32 1, (90,23--90,24));
                                              Const (Int32 10, (90,26--90,28))],
                                             [(90,24--90,25)], (90,23--90,28)),
                                          (90,22--90,23), Some (90,28--90,29),
                                          (90,22--90,29)), (90,17--90,29)),
                                    Open
                                      (Type
                                         (LongIdent
                                            (SynLongIdent ([int], [], [None])),
                                          (91,22--91,25)), (91,12--91,25),
                                       (91,12--92,30),
                                       YieldOrReturn
                                         ((true, false),
                                          App
                                            (NonAtomic, false,
                                             App
                                               (NonAtomic, true,
                                                LongIdent
                                                  (false,
                                                   SynLongIdent
                                                     ([op_Addition], [],
                                                      [Some
                                                         (OriginalNotation "+")]),
                                                   None, (92,27--92,28)),
                                                Ident MinValue, (92,18--92,28)),
                                             Ident i, (92,18--92,30)),
                                          (92,12--92,30),
                                          { YieldOrReturnKeyword =
                                             (92,12--92,17) })), (90,8--92,30))),
                              (88,10--93,5)), (88,4--93,5)), (88,4--93,8)),
                     LongIdent
                       (false,
                        SynLongIdent
                          ([Seq; toArray], [(93,12--93,13)], [None; None]), None,
                        (93,9--93,20)), (88,4--93,20)), (87,4--87,5), NoneAtLet,
                  { LeadingKeyword = Let (87,0--87,3)
                    InlineKeyword = None
                    EqualsRange = Some (87,6--87,7) })], (87,0--93,20))],
          PreXmlDoc ((2,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (2,0--93,20), { LeadingKeyword = Module (2,0--2,6) })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments =
         [LineComment (1,0--1,57); LineComment (37,0--37,13);
          LineComment (44,0--44,11); LineComment (49,0--49,13);
          LineComment (56,0--56,10); LineComment (67,0--67,11);
          LineComment (74,0--74,13); LineComment (78,0--78,29);
          LineComment (86,0--86,13)] }, set []))
