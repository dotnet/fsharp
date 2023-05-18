ImplFile
  (ParsedImplFileInput
     ("/root/SimplePats/SimplePats - Recover 01.fs", false,
      QualifiedNameOfFile SimplePats, [], [],
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
                       ([[SynArgInfo ([], false, Some i);
                          SynArgInfo ([], false, None)]],
                        SynArgInfo ([], false, None)), None),
                  Named (SynIdent (x, None), false, None, (3,4--3,5)), None,
                  Lambda
                    (false, false,
                     SimplePats
                       ([Typed
                           (Id (i, None, false, false, false, (3,13--3,14)),
                            LongIdent (SynLongIdent ([int], [], [None])),
                            (3,13--3,19));
                         Id (_arg1, None, true, false, false, (3,20--3,20))],
                        [(3,19--3,20)], (3,12--3,21)), Ident i,
                     Some
                       ([Paren
                           (Tuple
                              (false,
                               [Typed
                                  (Named
                                     (SynIdent (i, None), false, None,
                                      (3,13--3,14)),
                                   LongIdent (SynLongIdent ([int], [], [None])),
                                   (3,13--3,19)); Wild (3,20--3,20)],
                               [(3,19--3,20)], (3,13--3,21)), (3,12--3,21))],
                        Ident i), (3,8--3,26),
                     { ArrowRange = Some (3,22--3,24) }), (3,4--3,5), NoneAtLet,
                  { LeadingKeyword = Let (3,0--3,3)
                    InlineKeyword = None
                    EqualsRange = Some (3,6--3,7) })], (3,0--3,26));
           Let
             (false,
              [SynBinding
                 (None, Normal, false, false, [],
                  PreXmlDoc ((4,0), FSharp.Compiler.Xml.XmlDocCollector),
                  SynValData
                    (None,
                     SynValInfo
                       ([[SynArgInfo ([], false, Some a);
                          SynArgInfo ([], false, Some b);
                          SynArgInfo ([], false, None)]],
                        SynArgInfo ([], false, None)), None),
                  Named (SynIdent (y, None), false, None, (4,4--4,5)), None,
                  Lambda
                    (false, false,
                     SimplePats
                       ([Id (a, None, false, false, false, (4,13--4,14));
                         Id (b, None, false, false, false, (4,15--4,16));
                         Id (_arg1, None, true, false, false, (4,17--4,17))],
                        [(4,14--4,15); (4,16--4,17)], (4,12--4,18)),
                     Const (Unit, (4,22--4,24)),
                     Some
                       ([Paren
                           (Tuple
                              (false,
                               [Named
                                  (SynIdent (a, None), false, None, (4,13--4,14));
                                Named
                                  (SynIdent (b, None), false, None, (4,15--4,16));
                                Wild (4,17--4,17)], [(4,14--4,15); (4,16--4,17)],
                               (4,13--4,18)), (4,12--4,18))],
                        Const (Unit, (4,22--4,24))), (4,8--4,24),
                     { ArrowRange = Some (4,19--4,21) }), (4,4--4,5), NoneAtLet,
                  { LeadingKeyword = Let (4,0--4,3)
                    InlineKeyword = None
                    EqualsRange = Some (4,6--4,7) })], (4,0--4,24));
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
                                Range = (5,26--5,33) }], false, Some v);
                          SynArgInfo ([], false, None)]],
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
                               Range = (5,26--5,33) }], (5,26--5,38));
                         Id (_arg1, None, true, false, false, (5,39--5,39))],
                        [(5,24--5,25); (5,38--5,39)], (5,12--5,40)),
                     Const (Unit, (5,44--5,46)),
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
                                      Range = (5,26--5,33) }], (5,26--5,38));
                                Wild (5,39--5,39)], [(5,24--5,25); (5,38--5,39)],
                               (5,13--5,40)), (5,12--5,40))],
                        Const (Unit, (5,44--5,46))), (5,8--5,46),
                     { ArrowRange = Some (5,41--5,43) }), (5,4--5,5), NoneAtLet,
                  { LeadingKeyword = Let (5,0--5,3)
                    InlineKeyword = None
                    EqualsRange = Some (5,6--5,7) })], (5,0--5,46));
           Let
             (false,
              [SynBinding
                 (None, Normal, false, false, [],
                  PreXmlDoc ((6,0), FSharp.Compiler.Xml.XmlDocCollector),
                  SynValData
                    (None,
                     SynValInfo
                       ([[SynArgInfo ([], false, None);
                          SynArgInfo ([], false, None)]],
                        SynArgInfo ([], false, None)), None),
                  Named (SynIdent (ignore, None), false, None, (6,4--6,10)),
                  None,
                  Lambda
                    (false, false,
                     SimplePats
                       ([Id (_arg1, None, true, false, false, (6,18--6,19));
                         Id (_arg2, None, true, false, false, (6,20--6,20))],
                        [(6,19--6,20)], (6,17--6,21)),
                     Const (Unit, (6,25--6,27)),
                     Some
                       ([Paren
                           (Tuple
                              (false, [Wild (6,18--6,19); Wild (6,20--6,20)],
                               [(6,19--6,20)], (6,18--6,21)), (6,17--6,21))],
                        Const (Unit, (6,25--6,27))), (6,13--6,27),
                     { ArrowRange = Some (6,22--6,24) }), (6,4--6,10), NoneAtLet,
                  { LeadingKeyword = Let (6,0--6,3)
                    InlineKeyword = None
                    EqualsRange = Some (6,11--6,12) })], (6,0--6,27))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--6,27), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))

(3,19)-(3,20) parse error Expecting pattern
(4,16)-(4,17) parse error Expecting pattern
(5,38)-(5,39) parse error Expecting pattern
(6,19)-(6,20) parse error Expecting pattern
