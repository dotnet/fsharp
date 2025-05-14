ImplFile
  (ParsedImplFileInput
     ("/root/SynType/Typed LetBang 03.fs", false, QualifiedNameOfFile Module, [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Expr
             (App
                (NonAtomic, false, Ident async,
                 ComputationExpr
                   (false,
                    LetOrUse
                      (false, false,
                       [SynBinding
                          (None, Normal, false, false, [],
                           PreXmlDoc ((4,4), FSharp.Compiler.Xml.XmlDocCollector),
                           SynValData
                             (None,
                              SynValInfo ([], SynArgInfo ([], false, None)),
                              None),
                           Paren
                             (Tuple
                                (false,
                                 [Typed
                                    (Named
                                       (SynIdent (a, None), false, None,
                                        (4,9--4,10)),
                                     LongIdent
                                       (SynLongIdent ([int], [], [None])),
                                     (4,9--4,15));
                                  Typed
                                    (Named
                                       (SynIdent (b, None), false, None,
                                        (4,17--4,18)),
                                     LongIdent
                                       (SynLongIdent ([int], [], [None])),
                                     (4,17--4,23))], [(4,15--4,16)], (4,9--4,23)),
                              (4,8--4,24)), None,
                           Tuple
                             (false,
                              [Const (Int32 1, (4,27--4,28));
                               Const (Int32 3, (4,30--4,31))], [(4,28--4,29)],
                              (4,27--4,31)), (4,8--4,24), Yes (4,4--4,31),
                           { LeadingKeyword = Let (4,4--4,7)
                             InlineKeyword = None
                             EqualsRange = Some (4,25--4,26) })],
                       LetOrUseBang
                         (Yes (5,4--5,49), false, true,
                          Paren
                            (Tuple
                               (false,
                                [Typed
                                   (Named
                                      (SynIdent (c, None), false, None,
                                       (5,10--5,11)),
                                    LongIdent (SynLongIdent ([int], [], [None])),
                                    (5,10--5,16));
                                 Typed
                                   (Named
                                      (SynIdent (d, None), false, None,
                                       (5,18--5,19)),
                                    LongIdent (SynLongIdent ([int], [], [None])),
                                    (5,18--5,24))], [(5,16--5,17)], (5,10--5,24)),
                             (5,9--5,25)),
                          App
                            (NonAtomic, false, Ident async,
                             ComputationExpr
                               (false,
                                YieldOrReturn
                                  ((false, true),
                                   Tuple
                                     (false,
                                      [Const (Int32 1, (5,43--5,44));
                                       Const (Int32 3, (5,46--5,47))],
                                      [(5,44--5,45)], (5,43--5,47)),
                                   (5,36--5,47),
                                   { YieldOrReturnKeyword = (5,36--5,42) }),
                                (5,34--5,49)), (5,28--5,49)), [],
                          YieldOrReturn
                            ((false, true),
                             App
                               (NonAtomic, false,
                                App
                                  (NonAtomic, true,
                                   LongIdent
                                     (false,
                                      SynLongIdent
                                        ([op_Addition], [],
                                         [Some (OriginalNotation "+")]), None,
                                      (6,21--6,22)),
                                   App
                                     (NonAtomic, false,
                                      App
                                        (NonAtomic, true,
                                         LongIdent
                                           (false,
                                            SynLongIdent
                                              ([op_Addition], [],
                                               [Some (OriginalNotation "+")]),
                                            None, (6,17--6,18)),
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
                                                  None, (6,13--6,14)), Ident a,
                                               (6,11--6,14)), Ident b,
                                            (6,11--6,16)), (6,11--6,18)),
                                      Ident c, (6,11--6,20)), (6,11--6,22)),
                                Ident d, (6,11--6,24)), (6,4--6,24),
                             { YieldOrReturnKeyword = (6,4--6,10) }),
                          (5,4--6,24), { LetOrUseBangKeyword = (5,4--5,8)
                                         EqualsRange = Some (5,26--5,27) }),
                       (4,4--6,24), { LetOrUseKeyword = (4,4--4,7)
                                      InKeyword = None }), (3,6--7,1)),
                 (3,0--7,1)), (3,0--7,1))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--7,1), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [] }, set []))
