ImplFile
  (ParsedImplFileInput
     ("/root/Type/RangeOfAttributesShouldBeIncludedInRecursiveTypes.fs", false,
      QualifiedNameOfFile RangeOfAttributesShouldBeIncludedInRecursiveTypes, [],
      [],
      [SynModuleOrNamespace
         ([RangeOfAttributesShouldBeIncludedInRecursiveTypes], false, AnonModule,
          [Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([{ Attributes =
                         [{ TypeName = SynLongIdent ([NoEquality], [], [None])
                            ArgExpr = Const (Unit, (2,2--2,12))
                            Target = None
                            AppliesToGetterAndSetter = false
                            Range = (2,2--2,12) };
                          { TypeName = SynLongIdent ([NoComparison], [], [None])
                            ArgExpr = Const (Unit, (2,15--2,27))
                            Target = None
                            AppliesToGetterAndSetter = false
                            Range = (2,15--2,27) }]
                        Range = (2,0--2,29) }],
                     Some
                       (PostfixList
                          ([SynTyparDecl ([], SynTypar (context, None, false));
                            SynTyparDecl ([], SynTypar (a, None, false))], [],
                           (3,8--3,22))), [], [Foo],
                     PreXmlDoc ((2,0), FSharp.Compiler.Xml.XmlDocCollector),
                     true, None, (3,5--3,8)),
                  Simple
                    (Union
                       (None,
                        [SynUnionCase
                           ([], SynIdent (Apply, None),
                            Fields
                              [SynField
                                 ([], false, None,
                                  App
                                    (LongIdent
                                       (SynLongIdent ([ApplyCrate], [], [None])),
                                     Some (4,25--4,26),
                                     [Var
                                        (SynTypar (context, None, false),
                                         (4,26--4,34));
                                      Var
                                        (SynTypar (a, None, false), (4,36--4,38))],
                                     [(4,34--4,35)], Some (4,38--4,39), false,
                                     (4,15--4,39)), false,
                                  PreXmlDoc ((4,15), FSharp.Compiler.Xml.XmlDocCollector),
                                  None, (4,15--4,39), { LeadingKeyword = None })],
                            PreXmlDoc ((4,4), FSharp.Compiler.Xml.XmlDocCollector),
                            None, (4,6--4,39), { BarRange = Some (4,4--4,5) })],
                        (4,4--4,39)), (4,4--4,39)), [], None, (2,0--4,39),
                  { LeadingKeyword = Type (3,0--3,4)
                    EqualsRange = Some (3,23--3,24)
                    WithKeyword = None });
               SynTypeDefn
                 (SynComponentInfo
                    ([{ Attributes =
                         [{ TypeName =
                             SynLongIdent ([CustomEquality], [], [None])
                            ArgExpr = Const (Unit, (6,6--6,20))
                            Target = None
                            AppliesToGetterAndSetter = false
                            Range = (6,6--6,20) };
                          { TypeName = SynLongIdent ([NoComparison], [], [None])
                            ArgExpr = Const (Unit, (6,23--6,35))
                            Target = None
                            AppliesToGetterAndSetter = false
                            Range = (6,23--6,35) }]
                        Range = (6,4--6,37) }],
                     Some
                       (PostfixList
                          ([SynTyparDecl ([], SynTypar (context, None, false));
                            SynTyparDecl ([], SynTypar (a, None, false))], [],
                           (6,41--6,55))), [], [Bar],
                     PreXmlDoc ((6,4), FSharp.Compiler.Xml.XmlDocCollector),
                     true, None, (6,38--6,41)),
                  Simple
                    (Record
                       (Some (Internal (7,4--7,12)),
                        [SynField
                           ([], false, Some Hash,
                            LongIdent (SynLongIdent ([int], [], [None])), false,
                            PreXmlDoc ((8,8), FSharp.Compiler.Xml.XmlDocCollector),
                            None, (8,8--8,18), { LeadingKeyword = None });
                         SynField
                           ([], false, Some Foo,
                            App
                              (LongIdent (SynLongIdent ([Foo], [], [None])),
                               Some (9,17--9,18),
                               [Var (SynTypar (a, None, false), (9,18--9,20));
                                Var (SynTypar (b, None, false), (9,22--9,24))],
                               [(9,20--9,21)], Some (9,24--9,25), false,
                               (9,14--9,25)), false,
                            PreXmlDoc ((9,8), FSharp.Compiler.Xml.XmlDocCollector),
                            None, (9,8--9,25), { LeadingKeyword = None })],
                        (7,4--10,5)), (7,4--10,5)), [], None, (6,4--10,5),
                  { LeadingKeyword = And (6,0--6,3)
                    EqualsRange = Some (6,56--6,57)
                    WithKeyword = None })], (2,0--10,5))], PreXmlDocEmpty, [],
          None, (2,0--11,0), { LeadingKeyword = None })], (true, false),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))
