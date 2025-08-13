ImplFile
  (ParsedImplFileInput
     ("/root/Type/Deeply Indented Type 01.fs", false,
      QualifiedNameOfFile OuterModule, [],
      [SynModuleOrNamespace
         ([OuterModule], false, NamedModule,
          [NestedModule
             (SynComponentInfo
                ([], None, [], [InnerModule],
                 PreXmlDoc ((4,0), FSharp.Compiler.Xml.XmlDocCollector), false,
                 None, (4,0--4,18)), false,
              [NestedModule
                 (SynComponentInfo
                    ([], None, [], [DeeplyNested],
                     PreXmlDoc ((5,4), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (5,4--5,23)), false,
                  [Types
                     ([SynTypeDefn
                         (SynComponentInfo
                            ([], None, [], [IndentedType],
                             PreXmlDoc ((6,8), FSharp.Compiler.Xml.XmlDocCollector),
                             false, None, (6,13--6,25)),
                          Simple
                            (Union
                               (None,
                                [SynUnionCase
                                   ([], SynIdent (Case1, None), Fields [],
                                    PreXmlDoc ((7,12), FSharp.Compiler.Xml.XmlDocCollector),
                                    None, (7,14--7,19),
                                    { BarRange = Some (7,12--7,13) });
                                 SynUnionCase
                                   ([], SynIdent (Case2, None), Fields [],
                                    PreXmlDoc ((8,12), FSharp.Compiler.Xml.XmlDocCollector),
                                    None, (8,14--8,19),
                                    { BarRange = Some (8,12--8,13) })],
                                (7,12--8,19)), (7,12--8,19)), [], None,
                          (6,13--8,19), { LeadingKeyword = Type (6,8--6,12)
                                          EqualsRange = Some (6,26--6,27)
                                          WithKeyword = None })], (6,8--8,19));
                   Types
                     ([SynTypeDefn
                         (SynComponentInfo
                            ([], None, [], [NestedType],
                             PreXmlDoc ((9,12), FSharp.Compiler.Xml.XmlDocCollector),
                             false, None, (9,17--9,27)),
                          Simple
                            (TypeAbbrev
                               (Ok, LongIdent (SynLongIdent ([int], [], [None])),
                                (9,30--9,33)), (9,30--9,33)), [], None,
                          (9,17--9,33), { LeadingKeyword = Type (9,12--9,16)
                                          EqualsRange = Some (9,28--9,29)
                                          WithKeyword = None })], (9,12--9,33));
                   NestedModule
                     (SynComponentInfo
                        ([], None, [], [NestedModule],
                         PreXmlDoc ((10,12), FSharp.Compiler.Xml.XmlDocCollector),
                         false, None, (10,12--10,31)), false,
                      [Let
                         (false,
                          [SynBinding
                             (None, Normal, false, false, [],
                              PreXmlDoc ((11,16), FSharp.Compiler.Xml.XmlDocCollector),
                              SynValData
                                (None,
                                 SynValInfo ([], SynArgInfo ([], false, None)),
                                 None),
                              Named
                                (SynIdent (x, None), false, None, (11,20--11,21)),
                              None, Const (Int32 1, (11,24--11,25)),
                              (11,20--11,21), Yes (11,16--11,25),
                              { LeadingKeyword = Let (11,16--11,19)
                                InlineKeyword = None
                                EqualsRange = Some (11,22--11,23) })],
                          (11,16--11,25))], false, (10,12--11,25),
                      { ModuleKeyword = Some (10,12--10,18)
                        EqualsRange = Some (10,32--10,33) });
                   Exception
                     (SynExceptionDefn
                        (SynExceptionDefnRepr
                           ([],
                            SynUnionCase
                              ([], SynIdent (NestedExc, None),
                               Fields
                                 [SynField
                                    ([], false, None,
                                     LongIdent
                                       (SynLongIdent ([string], [], [None])),
                                     false,
                                     PreXmlDoc ((12,35), FSharp.Compiler.Xml.XmlDocCollector),
                                     None, (12,35--12,41),
                                     { LeadingKeyword = None
                                       MutableKeyword = None })], PreXmlDocEmpty,
                               None, (12,22--12,41), { BarRange = None }), None,
                            PreXmlDoc ((12,12), FSharp.Compiler.Xml.XmlDocCollector),
                            None, (12,12--12,41)), None, [], (12,12--12,41)),
                      (12,12--12,41))], false, (5,4--12,41),
                  { ModuleKeyword = Some (5,4--5,10)
                    EqualsRange = Some (5,24--5,25) })], false, (4,0--12,41),
              { ModuleKeyword = Some (4,0--4,6)
                EqualsRange = Some (4,19--4,20) })],
          PreXmlDoc ((2,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (2,0--12,41), { LeadingKeyword = Module (2,0--2,6) })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments =
         [LineComment (1,0--1,60); LineComment (9,35--9,49);
          LineComment (10,35--10,49); LineComment (12,43--12,57)] }, set []))

(9,12)-(9,16) parse warning Nested type definitions are not allowed. Types must be defined at module or namespace level.
(10,12)-(10,18) parse warning Modules cannot be nested inside types. Define modules at module or namespace level.
(12,12)-(12,21) parse warning Exceptions must be defined at module level, not inside types.
