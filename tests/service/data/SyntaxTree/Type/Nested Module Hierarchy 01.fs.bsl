ImplFile
  (ParsedImplFileInput
     ("/root/Type/Nested Module Hierarchy 01.fs", false,
      QualifiedNameOfFile Root, [],
      [SynModuleOrNamespace
         ([Root], false, NamedModule,
          [NestedModule
             (SynComponentInfo
                ([], None, [], [Level1],
                 PreXmlDoc ((4,0), FSharp.Compiler.Xml.XmlDocCollector), false,
                 None, (4,0--4,13)), false,
              [Types
                 ([SynTypeDefn
                     (SynComponentInfo
                        ([], None, [], [ValidType1],
                         PreXmlDoc ((5,4), FSharp.Compiler.Xml.XmlDocCollector),
                         false, None, (5,9--5,19)),
                      Simple
                        (TypeAbbrev
                           (Ok, LongIdent (SynLongIdent ([int], [], [None])),
                            (5,22--5,25)), (5,22--5,25)), [], None, (5,9--5,25),
                      { LeadingKeyword = Type (5,4--5,8)
                        EqualsRange = Some (5,20--5,21)
                        WithKeyword = None })], (5,4--5,25));
               NestedModule
                 (SynComponentInfo
                    ([], None, [], [Level2],
                     PreXmlDoc ((7,4), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (7,4--7,17)), false,
                  [Types
                     ([SynTypeDefn
                         (SynComponentInfo
                            ([], None, [], [ValidType2],
                             PreXmlDoc ((8,8), FSharp.Compiler.Xml.XmlDocCollector),
                             false, None, (8,13--8,23)),
                          Simple
                            (TypeAbbrev
                               (Ok,
                                LongIdent (SynLongIdent ([string], [], [None])),
                                (8,26--8,32)), (8,26--8,32)), [], None,
                          (8,13--8,32), { LeadingKeyword = Type (8,8--8,12)
                                          EqualsRange = Some (8,24--8,25)
                                          WithKeyword = None })], (8,8--8,32));
                   NestedModule
                     (SynComponentInfo
                        ([], None, [], [Level3],
                         PreXmlDoc ((10,8), FSharp.Compiler.Xml.XmlDocCollector),
                         false, None, (10,8--10,21)), false,
                      [Types
                         ([SynTypeDefn
                             (SynComponentInfo
                                ([], None, [], [TypeWithInvalidModule],
                                 PreXmlDoc ((11,12), FSharp.Compiler.Xml.XmlDocCollector),
                                 false, None, (11,17--11,38)),
                              Simple
                                (Union
                                   (None,
                                    [SynUnionCase
                                       ([], SynIdent (A, None), Fields [],
                                        PreXmlDoc ((12,16), FSharp.Compiler.Xml.XmlDocCollector),
                                        None, (12,18--12,19),
                                        { BarRange = Some (12,16--12,17) });
                                     SynUnionCase
                                       ([], SynIdent (B, None), Fields [],
                                        PreXmlDoc ((13,16), FSharp.Compiler.Xml.XmlDocCollector),
                                        None, (13,18--13,19),
                                        { BarRange = Some (13,16--13,17) })],
                                    (12,16--13,19)), (12,16--13,19)), [], None,
                              (11,17--13,19),
                              { LeadingKeyword = Type (11,12--11,16)
                                EqualsRange = Some (11,39--11,40)
                                WithKeyword = None })], (11,12--13,19));
                       NestedModule
                         (SynComponentInfo
                            ([], None, [], [InvalidModule],
                             PreXmlDoc ((14,16), FSharp.Compiler.Xml.XmlDocCollector),
                             false, None, (14,16--14,36)), false,
                          [Let
                             (false,
                              [SynBinding
                                 (None, Normal, false, false, [],
                                  PreXmlDoc ((15,20), FSharp.Compiler.Xml.XmlDocCollector),
                                  SynValData
                                    (None,
                                     SynValInfo
                                       ([], SynArgInfo ([], false, None)), None),
                                  Named
                                    (SynIdent (x, None), false, None,
                                     (15,24--15,25)), None,
                                  Const (Int32 1, (15,28--15,29)),
                                  (15,24--15,25), Yes (15,20--15,29),
                                  { LeadingKeyword = Let (15,20--15,23)
                                    InlineKeyword = None
                                    EqualsRange = Some (15,26--15,27) })],
                              (15,20--15,29))], false, (14,16--15,29),
                          { ModuleKeyword = Some (14,16--14,22)
                            EqualsRange = Some (14,37--14,38) });
                       Types
                         ([SynTypeDefn
                             (SynComponentInfo
                                ([], None, [], [ValidType3],
                                 PreXmlDoc ((17,12), FSharp.Compiler.Xml.XmlDocCollector),
                                 false, None, (17,17--17,27)),
                              Simple
                                (TypeAbbrev
                                   (Ok,
                                    LongIdent
                                      (SynLongIdent ([float], [], [None])),
                                    (17,30--17,35)), (17,30--17,35)), [], None,
                              (17,17--17,35),
                              { LeadingKeyword = Type (17,12--17,16)
                                EqualsRange = Some (17,28--17,29)
                                WithKeyword = None })], (17,12--17,35))], false,
                      (10,8--17,35), { ModuleKeyword = Some (10,8--10,14)
                                       EqualsRange = Some (10,22--10,23) })],
                  false, (7,4--17,35), { ModuleKeyword = Some (7,4--7,10)
                                         EqualsRange = Some (7,18--7,19) })],
              false, (4,0--17,35), { ModuleKeyword = Some (4,0--4,6)
                                     EqualsRange = Some (4,14--4,15) })],
          PreXmlDoc ((2,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (2,0--17,35), { LeadingKeyword = Module (2,0--2,6) })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [LineComment (1,0--1,65)] }, set []))

(14,16)-(14,22) parse error Modules cannot be nested inside types. Define modules at module or namespace level.
