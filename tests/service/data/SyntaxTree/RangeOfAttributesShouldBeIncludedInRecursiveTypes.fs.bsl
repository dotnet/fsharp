ImplFile
  (ParsedImplFileInput
     ("/root/RangeOfAttributesShouldBeIncludedInRecursiveTypes.fs", false,
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
                                /root/RangeOfAttributesShouldBeIncludedInRecursiveTypes.fs (1,2--1,12))
                            Target = None
                            AppliesToGetterAndSetter = false
                            Range =
                             /root/RangeOfAttributesShouldBeIncludedInRecursiveTypes.fs (1,2--1,12) };
                          { TypeName = SynLongIdent ([NoComparison], [], [None])
                            ArgExpr =
                             Const
                               (Unit,
                                /root/RangeOfAttributesShouldBeIncludedInRecursiveTypes.fs (1,15--1,27))
                            Target = None
                            AppliesToGetterAndSetter = false
                            Range =
                             /root/RangeOfAttributesShouldBeIncludedInRecursiveTypes.fs (1,15--1,27) }]
                        Range =
                         /root/RangeOfAttributesShouldBeIncludedInRecursiveTypes.fs (1,0--1,29) }],
                     Some
                       (PostfixList
                          ([SynTyparDecl ([], SynTypar (context, None, false));
                            SynTyparDecl ([], SynTypar (a, None, false))], [],
                           /root/RangeOfAttributesShouldBeIncludedInRecursiveTypes.fs (2,8--2,22))),
                     [], [Foo],
                     PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector),
                     true, None,
                     /root/RangeOfAttributesShouldBeIncludedInRecursiveTypes.fs (2,5--2,8)),
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
                                       /root/RangeOfAttributesShouldBeIncludedInRecursiveTypes.fs (3,25--3,26),
                                     [Var
                                        (SynTypar (context, None, false),
                                         /root/RangeOfAttributesShouldBeIncludedInRecursiveTypes.fs (3,26--3,34));
                                      Var
                                        (SynTypar (a, None, false),
                                         /root/RangeOfAttributesShouldBeIncludedInRecursiveTypes.fs (3,36--3,38))],
                                     [/root/RangeOfAttributesShouldBeIncludedInRecursiveTypes.fs (3,34--3,35)],
                                     Some
                                       /root/RangeOfAttributesShouldBeIncludedInRecursiveTypes.fs (3,38--3,39),
                                     false,
                                     /root/RangeOfAttributesShouldBeIncludedInRecursiveTypes.fs (3,15--3,39)),
                                  false,
                                  PreXmlDoc ((3,15), FSharp.Compiler.Xml.XmlDocCollector),
                                  None,
                                  /root/RangeOfAttributesShouldBeIncludedInRecursiveTypes.fs (3,15--3,39),
                                  { LeadingKeyword = None })],
                            PreXmlDoc ((3,4), FSharp.Compiler.Xml.XmlDocCollector),
                            None,
                            /root/RangeOfAttributesShouldBeIncludedInRecursiveTypes.fs (3,6--3,39),
                            { BarRange =
                               Some
                                 /root/RangeOfAttributesShouldBeIncludedInRecursiveTypes.fs (3,4--3,5) })],
                        /root/RangeOfAttributesShouldBeIncludedInRecursiveTypes.fs (3,4--3,39)),
                     /root/RangeOfAttributesShouldBeIncludedInRecursiveTypes.fs (3,4--3,39)),
                  [], None,
                  /root/RangeOfAttributesShouldBeIncludedInRecursiveTypes.fs (1,0--3,39),
                  { LeadingKeyword =
                     Type
                       /root/RangeOfAttributesShouldBeIncludedInRecursiveTypes.fs (2,0--2,4)
                    EqualsRange =
                     Some
                       /root/RangeOfAttributesShouldBeIncludedInRecursiveTypes.fs (2,23--2,24)
                    WithKeyword = None });
               SynTypeDefn
                 (SynComponentInfo
                    ([{ Attributes =
                         [{ TypeName =
                             SynLongIdent ([CustomEquality], [], [None])
                            ArgExpr =
                             Const
                               (Unit,
                                /root/RangeOfAttributesShouldBeIncludedInRecursiveTypes.fs (5,6--5,20))
                            Target = None
                            AppliesToGetterAndSetter = false
                            Range =
                             /root/RangeOfAttributesShouldBeIncludedInRecursiveTypes.fs (5,6--5,20) };
                          { TypeName = SynLongIdent ([NoComparison], [], [None])
                            ArgExpr =
                             Const
                               (Unit,
                                /root/RangeOfAttributesShouldBeIncludedInRecursiveTypes.fs (5,23--5,35))
                            Target = None
                            AppliesToGetterAndSetter = false
                            Range =
                             /root/RangeOfAttributesShouldBeIncludedInRecursiveTypes.fs (5,23--5,35) }]
                        Range =
                         /root/RangeOfAttributesShouldBeIncludedInRecursiveTypes.fs (5,4--5,37) }],
                     Some
                       (PostfixList
                          ([SynTyparDecl ([], SynTypar (context, None, false));
                            SynTyparDecl ([], SynTypar (a, None, false))], [],
                           /root/RangeOfAttributesShouldBeIncludedInRecursiveTypes.fs (5,41--5,55))),
                     [], [Bar],
                     PreXmlDoc ((5,4), FSharp.Compiler.Xml.XmlDocCollector),
                     true, None,
                     /root/RangeOfAttributesShouldBeIncludedInRecursiveTypes.fs (5,38--5,41)),
                  Simple
                    (Record
                       (Some
                          (Internal
                             /root/RangeOfAttributesShouldBeIncludedInRecursiveTypes.fs (6,4--6,12)),
                        [SynField
                           ([], false, Some Hash,
                            LongIdent (SynLongIdent ([int], [], [None])), false,
                            PreXmlDoc ((7,8), FSharp.Compiler.Xml.XmlDocCollector),
                            None,
                            /root/RangeOfAttributesShouldBeIncludedInRecursiveTypes.fs (7,8--7,18),
                            { LeadingKeyword = None });
                         SynField
                           ([], false, Some Foo,
                            App
                              (LongIdent (SynLongIdent ([Foo], [], [None])),
                               Some
                                 /root/RangeOfAttributesShouldBeIncludedInRecursiveTypes.fs (8,17--8,18),
                               [Var
                                  (SynTypar (a, None, false),
                                   /root/RangeOfAttributesShouldBeIncludedInRecursiveTypes.fs (8,18--8,20));
                                Var
                                  (SynTypar (b, None, false),
                                   /root/RangeOfAttributesShouldBeIncludedInRecursiveTypes.fs (8,22--8,24))],
                               [/root/RangeOfAttributesShouldBeIncludedInRecursiveTypes.fs (8,20--8,21)],
                               Some
                                 /root/RangeOfAttributesShouldBeIncludedInRecursiveTypes.fs (8,24--8,25),
                               false,
                               /root/RangeOfAttributesShouldBeIncludedInRecursiveTypes.fs (8,14--8,25)),
                            false,
                            PreXmlDoc ((8,8), FSharp.Compiler.Xml.XmlDocCollector),
                            None,
                            /root/RangeOfAttributesShouldBeIncludedInRecursiveTypes.fs (8,8--8,25),
                            { LeadingKeyword = None })],
                        /root/RangeOfAttributesShouldBeIncludedInRecursiveTypes.fs (6,4--9,5)),
                     /root/RangeOfAttributesShouldBeIncludedInRecursiveTypes.fs (6,4--9,5)),
                  [], None,
                  /root/RangeOfAttributesShouldBeIncludedInRecursiveTypes.fs (5,4--9,5),
                  { LeadingKeyword =
                     And
                       /root/RangeOfAttributesShouldBeIncludedInRecursiveTypes.fs (5,0--5,3)
                    EqualsRange =
                     Some
                       /root/RangeOfAttributesShouldBeIncludedInRecursiveTypes.fs (5,56--5,57)
                    WithKeyword = None })],
              /root/RangeOfAttributesShouldBeIncludedInRecursiveTypes.fs (1,0--9,5))],
          PreXmlDocEmpty, [], None,
          /root/RangeOfAttributesShouldBeIncludedInRecursiveTypes.fs (1,0--9,5),
          { LeadingKeyword = None })], (true, false),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))