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
                            ArgExpr =
                             Const
                               (Unit,
                                /root/Type/RangeOfAttributesShouldBeIncludedInRecursiveTypes.fs (2,2--2,12))
                            Target = None
                            AppliesToGetterAndSetter = false
                            Range =
                             /root/Type/RangeOfAttributesShouldBeIncludedInRecursiveTypes.fs (2,2--2,12) };
                          { TypeName = SynLongIdent ([NoComparison], [], [None])
                            ArgExpr =
                             Const
                               (Unit,
                                /root/Type/RangeOfAttributesShouldBeIncludedInRecursiveTypes.fs (2,15--2,27))
                            Target = None
                            AppliesToGetterAndSetter = false
                            Range =
                             /root/Type/RangeOfAttributesShouldBeIncludedInRecursiveTypes.fs (2,15--2,27) }]
                        Range =
                         /root/Type/RangeOfAttributesShouldBeIncludedInRecursiveTypes.fs (2,0--2,29) }],
                     Some
                       (PostfixList
                          ([SynTyparDecl ([], SynTypar (context, None, false));
                            SynTyparDecl ([], SynTypar (a, None, false))], [],
                           /root/Type/RangeOfAttributesShouldBeIncludedInRecursiveTypes.fs (3,8--3,22))),
                     [], [Foo],
                     PreXmlDoc ((2,0), FSharp.Compiler.Xml.XmlDocCollector),
                     true, None,
                     /root/Type/RangeOfAttributesShouldBeIncludedInRecursiveTypes.fs (3,5--3,8)),
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
                                     Some
                                       /root/Type/RangeOfAttributesShouldBeIncludedInRecursiveTypes.fs (4,25--4,26),
                                     [Var
                                        (SynTypar (context, None, false),
                                         /root/Type/RangeOfAttributesShouldBeIncludedInRecursiveTypes.fs (4,26--4,34));
                                      Var
                                        (SynTypar (a, None, false),
                                         /root/Type/RangeOfAttributesShouldBeIncludedInRecursiveTypes.fs (4,36--4,38))],
                                     [/root/Type/RangeOfAttributesShouldBeIncludedInRecursiveTypes.fs (4,34--4,35)],
                                     Some
                                       /root/Type/RangeOfAttributesShouldBeIncludedInRecursiveTypes.fs (4,38--4,39),
                                     false,
                                     /root/Type/RangeOfAttributesShouldBeIncludedInRecursiveTypes.fs (4,15--4,39)),
                                  false,
                                  PreXmlDoc ((4,15), FSharp.Compiler.Xml.XmlDocCollector),
                                  None,
                                  /root/Type/RangeOfAttributesShouldBeIncludedInRecursiveTypes.fs (4,15--4,39),
                                  { LeadingKeyword = None })],
                            PreXmlDoc ((4,4), FSharp.Compiler.Xml.XmlDocCollector),
                            None,
                            /root/Type/RangeOfAttributesShouldBeIncludedInRecursiveTypes.fs (4,6--4,39),
                            { BarRange =
                               Some
                                 /root/Type/RangeOfAttributesShouldBeIncludedInRecursiveTypes.fs (4,4--4,5) })],
                        /root/Type/RangeOfAttributesShouldBeIncludedInRecursiveTypes.fs (4,4--4,39)),
                     /root/Type/RangeOfAttributesShouldBeIncludedInRecursiveTypes.fs (4,4--4,39)),
                  [], None,
                  /root/Type/RangeOfAttributesShouldBeIncludedInRecursiveTypes.fs (2,0--4,39),
                  { LeadingKeyword =
                     Type
                       /root/Type/RangeOfAttributesShouldBeIncludedInRecursiveTypes.fs (3,0--3,4)
                    EqualsRange =
                     Some
                       /root/Type/RangeOfAttributesShouldBeIncludedInRecursiveTypes.fs (3,23--3,24)
                    WithKeyword = None });
               SynTypeDefn
                 (SynComponentInfo
                    ([{ Attributes =
                         [{ TypeName =
                             SynLongIdent ([CustomEquality], [], [None])
                            ArgExpr =
                             Const
                               (Unit,
                                /root/Type/RangeOfAttributesShouldBeIncludedInRecursiveTypes.fs (6,6--6,20))
                            Target = None
                            AppliesToGetterAndSetter = false
                            Range =
                             /root/Type/RangeOfAttributesShouldBeIncludedInRecursiveTypes.fs (6,6--6,20) };
                          { TypeName = SynLongIdent ([NoComparison], [], [None])
                            ArgExpr =
                             Const
                               (Unit,
                                /root/Type/RangeOfAttributesShouldBeIncludedInRecursiveTypes.fs (6,23--6,35))
                            Target = None
                            AppliesToGetterAndSetter = false
                            Range =
                             /root/Type/RangeOfAttributesShouldBeIncludedInRecursiveTypes.fs (6,23--6,35) }]
                        Range =
                         /root/Type/RangeOfAttributesShouldBeIncludedInRecursiveTypes.fs (6,4--6,37) }],
                     Some
                       (PostfixList
                          ([SynTyparDecl ([], SynTypar (context, None, false));
                            SynTyparDecl ([], SynTypar (a, None, false))], [],
                           /root/Type/RangeOfAttributesShouldBeIncludedInRecursiveTypes.fs (6,41--6,55))),
                     [], [Bar],
                     PreXmlDoc ((6,4), FSharp.Compiler.Xml.XmlDocCollector),
                     true, None,
                     /root/Type/RangeOfAttributesShouldBeIncludedInRecursiveTypes.fs (6,38--6,41)),
                  Simple
                    (Record
                       (Some
                          (Internal
                             /root/Type/RangeOfAttributesShouldBeIncludedInRecursiveTypes.fs (7,4--7,12)),
                        [SynField
                           ([], false, Some Hash,
                            LongIdent (SynLongIdent ([int], [], [None])), false,
                            PreXmlDoc ((8,8), FSharp.Compiler.Xml.XmlDocCollector),
                            None,
                            /root/Type/RangeOfAttributesShouldBeIncludedInRecursiveTypes.fs (8,8--8,18),
                            { LeadingKeyword = None });
                         SynField
                           ([], false, Some Foo,
                            App
                              (LongIdent (SynLongIdent ([Foo], [], [None])),
                               Some
                                 /root/Type/RangeOfAttributesShouldBeIncludedInRecursiveTypes.fs (9,17--9,18),
                               [Var
                                  (SynTypar (a, None, false),
                                   /root/Type/RangeOfAttributesShouldBeIncludedInRecursiveTypes.fs (9,18--9,20));
                                Var
                                  (SynTypar (b, None, false),
                                   /root/Type/RangeOfAttributesShouldBeIncludedInRecursiveTypes.fs (9,22--9,24))],
                               [/root/Type/RangeOfAttributesShouldBeIncludedInRecursiveTypes.fs (9,20--9,21)],
                               Some
                                 /root/Type/RangeOfAttributesShouldBeIncludedInRecursiveTypes.fs (9,24--9,25),
                               false,
                               /root/Type/RangeOfAttributesShouldBeIncludedInRecursiveTypes.fs (9,14--9,25)),
                            false,
                            PreXmlDoc ((9,8), FSharp.Compiler.Xml.XmlDocCollector),
                            None,
                            /root/Type/RangeOfAttributesShouldBeIncludedInRecursiveTypes.fs (9,8--9,25),
                            { LeadingKeyword = None })],
                        /root/Type/RangeOfAttributesShouldBeIncludedInRecursiveTypes.fs (7,4--10,5)),
                     /root/Type/RangeOfAttributesShouldBeIncludedInRecursiveTypes.fs (7,4--10,5)),
                  [], None,
                  /root/Type/RangeOfAttributesShouldBeIncludedInRecursiveTypes.fs (6,4--10,5),
                  { LeadingKeyword =
                     And
                       /root/Type/RangeOfAttributesShouldBeIncludedInRecursiveTypes.fs (6,0--6,3)
                    EqualsRange =
                     Some
                       /root/Type/RangeOfAttributesShouldBeIncludedInRecursiveTypes.fs (6,56--6,57)
                    WithKeyword = None })],
              /root/Type/RangeOfAttributesShouldBeIncludedInRecursiveTypes.fs (2,0--10,5))],
          PreXmlDocEmpty, [], None,
          /root/Type/RangeOfAttributesShouldBeIncludedInRecursiveTypes.fs (2,0--11,0),
          { LeadingKeyword = None })], (true, false),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))
