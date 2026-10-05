ImplFile
  (ParsedImplFileInput
     ("/root/UnionCase/SynUnionCaseShouldContainTheRangeOfTheOfKeyword.fs",
      false, QualifiedNameOfFile X, [],
      [SynModuleOrNamespace
         ([X], false, NamedModule,
          [Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([], None, [],
                     Some (LongIdent (SynLongIdent ([Foo], [], [None]))),
                     PreXmlDoc ((3,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (3,5--3,8)),
                  Simple
                    (Union
                       (None,
                        [SynUnionCase
                           ([], SynIdent (Bar, None),
                            Fields
                              [SynField
                                 ([], false, None,
                                  LongIdent (SynLongIdent ([int], [], [None])),
                                  false,
                                  PreXmlDoc ((4,29), FSharp.Compiler.Xml.XmlDocCollector),
                                  None, (4,29--4,32), { LeadingKeyword = None
                                                        MutableKeyword = None })],
                            PreXmlDoc ((4,4), FSharp.Compiler.Xml.XmlDocCollector),
                            None, (4,6--4,32), { BarRange = Some (4,4--4,5)
                                                 OfKeyword = Some (4,10--4,12) });
                         SynUnionCase
                           ([], SynIdent (Baz, None),
                            Fields
                              [SynField
                                 ([], false, None,
                                  LongIdent
                                    (SynLongIdent ([string], [], [None])), false,
                                  PreXmlDoc ((5,13), FSharp.Compiler.Xml.XmlDocCollector),
                                  None, (5,13--5,19), { LeadingKeyword = None
                                                        MutableKeyword = None });
                               SynField
                                 ([], false, None,
                                  LongIdent (SynLongIdent ([int], [], [None])),
                                  false,
                                  PreXmlDoc ((5,22), FSharp.Compiler.Xml.XmlDocCollector),
                                  None, (5,22--5,25), { LeadingKeyword = None
                                                        MutableKeyword = None })],
                            PreXmlDoc ((5,4), FSharp.Compiler.Xml.XmlDocCollector),
                            None, (5,6--5,25), { BarRange = Some (5,4--5,5)
                                                 OfKeyword = Some (5,10--5,12) });
                         SynUnionCase
                           ([], SynIdent (Qux, None), Fields [],
                            PreXmlDoc ((6,4), FSharp.Compiler.Xml.XmlDocCollector),
                            None, (6,6--6,9), { BarRange = Some (6,4--6,5)
                                                OfKeyword = None })], (4,4--6,9)),
                     (4,4--6,9)), [], None, (3,5--6,9),
                  { LeadingKeyword = Type (3,0--3,4)
                    EqualsRange = Some (3,9--3,10)
                    WithKeyword = None })], (3,0--6,9));
           Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([], None, [],
                     Some (LongIdent (SynLongIdent ([Single], [], [None]))),
                     PreXmlDoc ((8,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (8,5--8,11)),
                  Simple
                    (Union
                       (None,
                        [SynUnionCase
                           ([], SynIdent (Single, None),
                            Fields
                              [SynField
                                 ([], false, None,
                                  LongIdent (SynLongIdent ([int], [], [None])),
                                  false,
                                  PreXmlDoc ((8,24), FSharp.Compiler.Xml.XmlDocCollector),
                                  None, (8,24--8,27), { LeadingKeyword = None
                                                        MutableKeyword = None })],
                            PreXmlDoc ((8,14), FSharp.Compiler.Xml.XmlDocCollector),
                            None, (8,14--8,27),
                            { BarRange = None
                              OfKeyword = Some (8,21--8,23) })], (8,14--8,27)),
                     (8,14--8,27)), [], None, (8,5--8,27),
                  { LeadingKeyword = Type (8,0--8,4)
                    EqualsRange = Some (8,12--8,13)
                    WithKeyword = None })], (8,0--8,27))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--8,27), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [BlockComment (4,13--4,28)] }, set []))
