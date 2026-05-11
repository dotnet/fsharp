ImplFile
  (ParsedImplFileInput
     ("/root/Attribute/ReturnTypeAttributeOnBareTuple.fs", false,
      QualifiedNameOfFile M, [],
      [SynModuleOrNamespace
         ([M], false, NamedModule,
          [Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([], None, [], [T],
                     PreXmlDoc ((3,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (3,5--3,6)),
                  ObjectModel
                    (Unspecified,
                     [Member
                        (SynBinding
                           (None, Normal, false, false, [],
                            PreXmlDoc ((4,4), FSharp.Compiler.Xml.XmlDocCollector),
                            SynValData
                              (Some { IsInstance = false
                                      IsDispatchSlot = false
                                      IsOverrideOrExplicitImpl = false
                                      IsFinal = false
                                      GetterOrSetterIsCompilerGenerated = false
                                      MemberKind = Member },
                               SynValInfo
                                 ([[SynArgInfo ([], false, Some name)]],
                                  SynArgInfo
                                    ([{ Attributes =
                                         [{ TypeName =
                                             SynLongIdent
                                               ([ReturnDescription], [], [None])
                                            ArgExpr =
                                             Paren
                                               (Const
                                                  (String
                                                     ("first", Regular,
                                                      (5,28--5,35)),
                                                   (5,28--5,35)), (5,27--5,28),
                                                Some (5,35--5,36), (5,27--5,36))
                                            Target = None
                                            AppliesToGetterAndSetter = false
                                            Range = (5,10--5,36) }]
                                        Range = (5,8--5,38) };
                                      { Attributes =
                                         [{ TypeName =
                                             SynLongIdent
                                               ([ReturnDescription], [], [None])
                                            ArgExpr =
                                             Paren
                                               (Const
                                                  (String
                                                     ("second", Regular,
                                                      (6,28--6,36)),
                                                   (6,28--6,36)), (6,27--6,28),
                                                Some (6,36--6,37), (6,27--6,37))
                                            Target = None
                                            AppliesToGetterAndSetter = false
                                            Range = (6,10--6,37) }]
                                        Range = (6,8--6,39) }], false, None)),
                               None),
                            LongIdent
                              (SynLongIdent ([Parenthesized], [], [None]), None,
                               None,
                               Pats
                                 [Paren
                                    (Typed
                                       (Named
                                          (SynIdent (name, None), false, None,
                                           (4,32--4,36)),
                                        LongIdent
                                          (SynLongIdent ([string], [], [None])),
                                        (4,32--4,44)), (4,31--4,45))], None,
                               (4,18--4,45)),
                            Some
                              (SynBindingReturnInfo
                                 (SignatureParameter
                                    ([{ Attributes =
                                         [{ TypeName =
                                             SynLongIdent
                                               ([ReturnDescription], [], [None])
                                            ArgExpr =
                                             Paren
                                               (Const
                                                  (String
                                                     ("first", Regular,
                                                      (5,28--5,35)),
                                                   (5,28--5,35)), (5,27--5,28),
                                                Some (5,35--5,36), (5,27--5,36))
                                            Target = None
                                            AppliesToGetterAndSetter = false
                                            Range = (5,10--5,36) }]
                                        Range = (5,8--5,38) };
                                      { Attributes =
                                         [{ TypeName =
                                             SynLongIdent
                                               ([ReturnDescription], [], [None])
                                            ArgExpr =
                                             Paren
                                               (Const
                                                  (String
                                                     ("second", Regular,
                                                      (6,28--6,36)),
                                                   (6,28--6,36)), (6,27--6,28),
                                                Some (6,36--6,37), (6,27--6,37))
                                            Target = None
                                            AppliesToGetterAndSetter = false
                                            Range = (6,10--6,37) }]
                                        Range = (6,8--6,39) }], false, None,
                                     Paren
                                       (Tuple
                                          (false,
                                           [Type
                                              (LongIdent
                                                 (SynLongIdent
                                                    ([string], [], [None])));
                                            Star (7,16--7,17);
                                            Type
                                              (LongIdent
                                                 (SynLongIdent
                                                    ([string], [], [None])))],
                                           (7,9--7,24)), (7,8--7,25)),
                                     (5,8--7,25)), (5,8--7,25),
                                  [{ Attributes =
                                      [{ TypeName =
                                          SynLongIdent
                                            ([ReturnDescription], [], [None])
                                         ArgExpr =
                                          Paren
                                            (Const
                                               (String
                                                  ("first", Regular,
                                                   (5,28--5,35)), (5,28--5,35)),
                                             (5,27--5,28), Some (5,35--5,36),
                                             (5,27--5,36))
                                         Target = None
                                         AppliesToGetterAndSetter = false
                                         Range = (5,10--5,36) }]
                                     Range = (5,8--5,38) };
                                   { Attributes =
                                      [{ TypeName =
                                          SynLongIdent
                                            ([ReturnDescription], [], [None])
                                         ArgExpr =
                                          Paren
                                            (Const
                                               (String
                                                  ("second", Regular,
                                                   (6,28--6,36)), (6,28--6,36)),
                                             (6,27--6,28), Some (6,36--6,37),
                                             (6,27--6,37))
                                         Target = None
                                         AppliesToGetterAndSetter = false
                                         Range = (6,10--6,37) }]
                                     Range = (6,8--6,39) }],
                                  { ColonRange = Some (4,46--4,47) })),
                            Typed
                              (Tuple
                                 (false, [Ident name; Ident name],
                                  [(8,16--8,17)], (8,12--8,22)),
                               SignatureParameter
                                 ([{ Attributes =
                                      [{ TypeName =
                                          SynLongIdent
                                            ([ReturnDescription], [], [None])
                                         ArgExpr =
                                          Paren
                                            (Const
                                               (String
                                                  ("first", Regular,
                                                   (5,28--5,35)), (5,28--5,35)),
                                             (5,27--5,28), Some (5,35--5,36),
                                             (5,27--5,36))
                                         Target = None
                                         AppliesToGetterAndSetter = false
                                         Range = (5,10--5,36) }]
                                     Range = (5,8--5,38) };
                                   { Attributes =
                                      [{ TypeName =
                                          SynLongIdent
                                            ([ReturnDescription], [], [None])
                                         ArgExpr =
                                          Paren
                                            (Const
                                               (String
                                                  ("second", Regular,
                                                   (6,28--6,36)), (6,28--6,36)),
                                             (6,27--6,28), Some (6,36--6,37),
                                             (6,27--6,37))
                                         Target = None
                                         AppliesToGetterAndSetter = false
                                         Range = (6,10--6,37) }]
                                     Range = (6,8--6,39) }], false, None,
                                  Paren
                                    (Tuple
                                       (false,
                                        [Type
                                           (LongIdent
                                              (SynLongIdent
                                                 ([string], [], [None])));
                                         Star (7,16--7,17);
                                         Type
                                           (LongIdent
                                              (SynLongIdent
                                                 ([string], [], [None])))],
                                        (7,9--7,24)), (7,8--7,25)), (5,8--7,25)),
                               (8,12--8,22)), (4,18--4,45), NoneAtInvisible,
                            { LeadingKeyword =
                               StaticMember ((4,4--4,10), (4,11--4,17))
                              InlineKeyword = None
                              EqualsRange = Some (7,26--7,27) }), (4,4--8,22));
                      Member
                        (SynBinding
                           (None, Normal, false, false, [],
                            PreXmlDoc ((10,4), FSharp.Compiler.Xml.XmlDocCollector),
                            SynValData
                              (Some { IsInstance = false
                                      IsDispatchSlot = false
                                      IsOverrideOrExplicitImpl = false
                                      IsFinal = false
                                      GetterOrSetterIsCompilerGenerated = false
                                      MemberKind = Member },
                               SynValInfo
                                 ([[SynArgInfo ([], false, Some name)]],
                                  SynArgInfo
                                    ([{ Attributes =
                                         [{ TypeName =
                                             SynLongIdent
                                               ([ReturnDescription], [], [None])
                                            ArgExpr =
                                             Paren
                                               (Const
                                                  (String
                                                     ("first", Regular,
                                                      (11,28--11,35)),
                                                   (11,28--11,35)),
                                                (11,27--11,28),
                                                Some (11,35--11,36),
                                                (11,27--11,36))
                                            Target = None
                                            AppliesToGetterAndSetter = false
                                            Range = (11,10--11,36) }]
                                        Range = (11,8--11,38) };
                                      { Attributes =
                                         [{ TypeName =
                                             SynLongIdent
                                               ([ReturnDescription], [], [None])
                                            ArgExpr =
                                             Paren
                                               (Const
                                                  (String
                                                     ("second", Regular,
                                                      (12,28--12,36)),
                                                   (12,28--12,36)),
                                                (12,27--12,28),
                                                Some (12,36--12,37),
                                                (12,27--12,37))
                                            Target = None
                                            AppliesToGetterAndSetter = false
                                            Range = (12,10--12,37) }]
                                        Range = (12,8--12,39) }], false, None)),
                               None),
                            LongIdent
                              (SynLongIdent ([Bare], [], [None]), None, None,
                               Pats
                                 [Paren
                                    (Typed
                                       (Named
                                          (SynIdent (name, None), false, None,
                                           (10,23--10,27)),
                                        LongIdent
                                          (SynLongIdent ([string], [], [None])),
                                        (10,23--10,35)), (10,22--10,36))], None,
                               (10,18--10,36)),
                            Some
                              (SynBindingReturnInfo
                                 (Tuple
                                    (false,
                                     [Type
                                        (SignatureParameter
                                           ([{ Attributes =
                                                [{ TypeName =
                                                    SynLongIdent
                                                      ([ReturnDescription], [],
                                                       [None])
                                                   ArgExpr =
                                                    Paren
                                                      (Const
                                                         (String
                                                            ("first", Regular,
                                                             (11,28--11,35)),
                                                          (11,28--11,35)),
                                                       (11,27--11,28),
                                                       Some (11,35--11,36),
                                                       (11,27--11,36))
                                                   Target = None
                                                   AppliesToGetterAndSetter =
                                                    false
                                                   Range = (11,10--11,36) }]
                                               Range = (11,8--11,38) };
                                             { Attributes =
                                                [{ TypeName =
                                                    SynLongIdent
                                                      ([ReturnDescription], [],
                                                       [None])
                                                   ArgExpr =
                                                    Paren
                                                      (Const
                                                         (String
                                                            ("second", Regular,
                                                             (12,28--12,36)),
                                                          (12,28--12,36)),
                                                       (12,27--12,28),
                                                       Some (12,36--12,37),
                                                       (12,27--12,37))
                                                   Target = None
                                                   AppliesToGetterAndSetter =
                                                    false
                                                   Range = (12,10--12,37) }]
                                               Range = (12,8--12,39) }], false,
                                            None,
                                            LongIdent
                                              (SynLongIdent
                                                 ([string], [], [None])),
                                            (11,8--13,14))); Star (13,15--13,16);
                                      Type
                                        (LongIdent
                                           (SynLongIdent ([string], [], [None])))],
                                     (11,8--13,23)), (11,8--13,23),
                                  [{ Attributes =
                                      [{ TypeName =
                                          SynLongIdent
                                            ([ReturnDescription], [], [None])
                                         ArgExpr =
                                          Paren
                                            (Const
                                               (String
                                                  ("first", Regular,
                                                   (11,28--11,35)),
                                                (11,28--11,35)), (11,27--11,28),
                                             Some (11,35--11,36), (11,27--11,36))
                                         Target = None
                                         AppliesToGetterAndSetter = false
                                         Range = (11,10--11,36) }]
                                     Range = (11,8--11,38) };
                                   { Attributes =
                                      [{ TypeName =
                                          SynLongIdent
                                            ([ReturnDescription], [], [None])
                                         ArgExpr =
                                          Paren
                                            (Const
                                               (String
                                                  ("second", Regular,
                                                   (12,28--12,36)),
                                                (12,28--12,36)), (12,27--12,28),
                                             Some (12,36--12,37), (12,27--12,37))
                                         Target = None
                                         AppliesToGetterAndSetter = false
                                         Range = (12,10--12,37) }]
                                     Range = (12,8--12,39) }],
                                  { ColonRange = Some (10,37--10,38) })),
                            Typed
                              (Tuple
                                 (false, [Ident name; Ident name],
                                  [(14,16--14,17)], (14,12--14,22)),
                               Tuple
                                 (false,
                                  [Type
                                     (SignatureParameter
                                        ([{ Attributes =
                                             [{ TypeName =
                                                 SynLongIdent
                                                   ([ReturnDescription], [],
                                                    [None])
                                                ArgExpr =
                                                 Paren
                                                   (Const
                                                      (String
                                                         ("first", Regular,
                                                          (11,28--11,35)),
                                                       (11,28--11,35)),
                                                    (11,27--11,28),
                                                    Some (11,35--11,36),
                                                    (11,27--11,36))
                                                Target = None
                                                AppliesToGetterAndSetter = false
                                                Range = (11,10--11,36) }]
                                            Range = (11,8--11,38) };
                                          { Attributes =
                                             [{ TypeName =
                                                 SynLongIdent
                                                   ([ReturnDescription], [],
                                                    [None])
                                                ArgExpr =
                                                 Paren
                                                   (Const
                                                      (String
                                                         ("second", Regular,
                                                          (12,28--12,36)),
                                                       (12,28--12,36)),
                                                    (12,27--12,28),
                                                    Some (12,36--12,37),
                                                    (12,27--12,37))
                                                Target = None
                                                AppliesToGetterAndSetter = false
                                                Range = (12,10--12,37) }]
                                            Range = (12,8--12,39) }], false,
                                         None,
                                         LongIdent
                                           (SynLongIdent ([string], [], [None])),
                                         (11,8--13,14))); Star (13,15--13,16);
                                   Type
                                     (LongIdent
                                        (SynLongIdent ([string], [], [None])))],
                                  (11,8--13,23)), (14,12--14,22)),
                            (10,18--10,36), NoneAtInvisible,
                            { LeadingKeyword =
                               StaticMember ((10,4--10,10), (10,11--10,17))
                              InlineKeyword = None
                              EqualsRange = Some (13,24--13,25) }),
                         (10,4--14,22))], (4,4--14,22)), [], None, (3,5--14,22),
                  { LeadingKeyword = Type (3,0--3,4)
                    EqualsRange = Some (3,7--3,8)
                    WithKeyword = None })], (3,0--14,22))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--14,22), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [] }, set []))
