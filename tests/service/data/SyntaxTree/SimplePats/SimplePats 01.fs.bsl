ImplFile
  (ParsedImplFileInput
     ("/root/SimplePats/SimplePats 01.fs", false, QualifiedNameOfFile SimplePats,
      [], [],
      [SynModuleOrNamespace
         ([SimplePats], false, NamedModule,
          [Let
             (false,
              [SynBinding
                 (None, Normal, false, false, [],
                  PreXmlDoc ((3,0), FSharp.Compiler.Xml.XmlDocCollector),
                  SynValData
                    (None,
                     SynValInfo
                       ([[SynArgInfo ([], false, Some i)]],
                        SynArgInfo ([], false, None)), None),
                  Named (SynIdent (x, None), false, None, (3,4--3,5)), None,
                  Lambda
                    (false, false,
                     SimplePats
                       ([Typed
                           (Id (i, None, false, false, false, (3,13--3,14)),
                            LongIdent (SynLongIdent ([int], [], [None])),
                            (3,13--3,19))], [], (3,12--3,20)), Ident i,
                     Some
                       ([Paren
                           (Typed
                              (Named
                                 (SynIdent (i, None), false, None, (3,13--3,14)),
                               LongIdent (SynLongIdent ([int], [], [None])),
                               (3,13--3,19)), (3,12--3,20))], Ident i),
                     (3,8--3,25), { ArrowRange = Some (3,21--3,23) }),
                  (3,4--3,5), NoneAtLet, { LeadingKeyword = Let (3,0--3,3)
                                           InlineKeyword = None
                                           EqualsRange = Some (3,6--3,7) })],
              (3,0--3,25));
           Let
             (false,
              [SynBinding
                 (None, Normal, false, false, [],
                  PreXmlDoc ((4,0), FSharp.Compiler.Xml.XmlDocCollector),
                  SynValData
                    (None,
                     SynValInfo
                       ([[SynArgInfo ([], false, Some a);
                          SynArgInfo ([], false, Some b)]],
                        SynArgInfo ([], false, None)), None),
                  Named (SynIdent (y, None), false, None, (4,4--4,5)), None,
                  Lambda
                    (false, false,
                     SimplePats
                       ([Id (a, None, false, false, false, (4,13--4,14));
                         Id (b, None, false, false, false, (4,15--4,16))],
                        [(4,14--4,15)], (4,12--4,17)),
                     Const (Unit, (4,21--4,23)),
                     Some
                       ([Paren
                           (Tuple
                              (false,
                               [Named
                                  (SynIdent (a, None), false, None, (4,13--4,14));
                                Named
                                  (SynIdent (b, None), false, None, (4,15--4,16))],
                               [(4,14--4,15)], (4,13--4,16)), (4,12--4,17))],
                        Const (Unit, (4,21--4,23))), (4,8--4,23),
                     { ArrowRange = Some (4,18--4,20) }), (4,4--4,5), NoneAtLet,
                  { LeadingKeyword = Let (4,0--4,3)
                    InlineKeyword = None
                    EqualsRange = Some (4,6--4,7) })], (4,0--4,23));
           Let
             (false,
              [SynBinding
                 (None, Normal, false, false, [],
                  PreXmlDoc ((5,0), FSharp.Compiler.Xml.XmlDocCollector),
                  SynValData
                    (None,
                     SynValInfo
                       ([[SynArgInfo
                            ([{ Attributes =
                                 [{ TypeName = SynLongIdent ([Foo], [], [None])
                                    ArgExpr = Const (Unit, (5,15--5,18))
                                    Target = None
                                    AppliesToGetterAndSetter = false
                                    Range = (5,15--5,18) }]
                                Range = (5,13--5,20) }], false, Some bar);
                          SynArgInfo
                            ([{ Attributes =
                                 [{ TypeName = SynLongIdent ([Foo], [], [None])
                                    ArgExpr = Const (Unit, (5,28--5,31))
                                    Target = None
                                    AppliesToGetterAndSetter = false
                                    Range = (5,28--5,31) }]
                                Range = (5,26--5,33) }], false, Some v)]],
                        SynArgInfo ([], false, None)), None),
                  Named (SynIdent (z, None), false, None, (5,4--5,5)), None,
                  Lambda
                    (false, false,
                     SimplePats
                       ([Attrib
                           (Id (bar, None, false, false, false, (5,21--5,24)),
                            [{ Attributes =
                                [{ TypeName = SynLongIdent ([Foo], [], [None])
                                   ArgExpr = Const (Unit, (5,15--5,18))
                                   Target = None
                                   AppliesToGetterAndSetter = false
                                   Range = (5,15--5,18) }]
                               Range = (5,13--5,20) }], (5,13--5,24));
                         Attrib
                           (Typed
                              (Id (v, None, false, false, false, (5,34--5,35)),
                               LongIdent (SynLongIdent ([V], [], [None])),
                               (5,34--5,38)),
                            [{ Attributes =
                                [{ TypeName = SynLongIdent ([Foo], [], [None])
                                   ArgExpr = Const (Unit, (5,28--5,31))
                                   Target = None
                                   AppliesToGetterAndSetter = false
                                   Range = (5,28--5,31) }]
                               Range = (5,26--5,33) }], (5,26--5,38))],
                        [(5,24--5,25)], (5,12--5,39)),
                     Const (Unit, (5,43--5,45)),
                     Some
                       ([Paren
                           (Tuple
                              (false,
                               [Attrib
                                  (Named
                                     (SynIdent (bar, None), false, None,
                                      (5,21--5,24)),
                                   [{ Attributes =
                                       [{ TypeName =
                                           SynLongIdent ([Foo], [], [None])
                                          ArgExpr = Const (Unit, (5,15--5,18))
                                          Target = None
                                          AppliesToGetterAndSetter = false
                                          Range = (5,15--5,18) }]
                                      Range = (5,13--5,20) }], (5,13--5,24));
                                Attrib
                                  (Typed
                                     (Named
                                        (SynIdent (v, None), false, None,
                                         (5,34--5,35)),
                                      LongIdent (SynLongIdent ([V], [], [None])),
                                      (5,34--5,38)),
                                   [{ Attributes =
                                       [{ TypeName =
                                           SynLongIdent ([Foo], [], [None])
                                          ArgExpr = Const (Unit, (5,28--5,31))
                                          Target = None
                                          AppliesToGetterAndSetter = false
                                          Range = (5,28--5,31) }]
                                      Range = (5,26--5,33) }], (5,26--5,38))],
                               [(5,24--5,25)], (5,13--5,38)), (5,12--5,39))],
                        Const (Unit, (5,43--5,45))), (5,8--5,45),
                     { ArrowRange = Some (5,40--5,42) }), (5,4--5,5), NoneAtLet,
                  { LeadingKeyword = Let (5,0--5,3)
                    InlineKeyword = None
                    EqualsRange = Some (5,6--5,7) })], (5,0--5,45));
           Let
             (false,
              [SynBinding
                 (None, Normal, false, false, [],
                  PreXmlDoc ((6,0), FSharp.Compiler.Xml.XmlDocCollector),
                  SynValData
                    (None,
                     SynValInfo
                       ([[SynArgInfo ([], false, None)]],
                        SynArgInfo ([], false, None)), None),
                  Named (SynIdent (ignore, None), false, None, (6,4--6,10)),
                  None,
                  Lambda
                    (false, false,
                     SimplePats
                       ([Id (_arg1, None, true, false, false, (6,17--6,18))], [],
                        (6,17--6,18)), Const (Unit, (6,22--6,24)),
                     Some ([Wild (6,17--6,18)], Const (Unit, (6,22--6,24))),
                     (6,13--6,24), { ArrowRange = Some (6,19--6,21) }),
                  (6,4--6,10), NoneAtLet, { LeadingKeyword = Let (6,0--6,3)
                                            InlineKeyword = None
                                            EqualsRange = Some (6,11--6,12) })],
              (6,0--6,24));
           Let
             (false,
              [SynBinding
                 (None, Normal, false, false, [],
                  PreXmlDoc ((7,0), FSharp.Compiler.Xml.XmlDocCollector),
                  SynValData
                    (None, SynValInfo ([[]], SynArgInfo ([], false, None)), None),
                  Named (SynIdent (empty, None), false, None, (7,4--7,9)), None,
                  Lambda
                    (false, false, SimplePats ([], [], (7,16--7,18)),
                     Const (Unit, (7,22--7,24)),
                     Some
                       ([Paren (Const (Unit, (7,16--7,18)), (7,16--7,18))],
                        Const (Unit, (7,22--7,24))), (7,12--7,24),
                     { ArrowRange = Some (7,19--7,21) }), (7,4--7,9), NoneAtLet,
                  { LeadingKeyword = Let (7,0--7,3)
                    InlineKeyword = None
                    EqualsRange = Some (7,10--7,11) })], (7,0--7,24))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--7,24), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))
