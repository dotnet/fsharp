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
              [NestedModule
                 (SynComponentInfo
                    ([], None, [], [Level2],
                     PreXmlDoc ((5,4), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (5,4--5,17)), false,
                  [NestedModule
                     (SynComponentInfo
                        ([], None, [], [Level3],
                         PreXmlDoc ((6,8), FSharp.Compiler.Xml.XmlDocCollector),
                         false, None, (6,8--6,21)), false,
                      [Types
                         ([SynTypeDefn
                             (SynComponentInfo
                                ([], None, [], [TypeWithInvalidModule],
                                 PreXmlDoc ((7,12), FSharp.Compiler.Xml.XmlDocCollector),
                                 false, None, (7,17--7,38)),
                              Simple (None (7,17--7,40), (7,17--7,40)), [], None,
                              (7,17--7,40), { LeadingKeyword = Type (7,12--7,16)
                                              EqualsRange = Some (7,39--7,40)
                                              WithKeyword = None })],
                          (7,12--7,40));
                       NestedModule
                         (SynComponentInfo
                            ([], None, [], [InvalidModule],
                             PreXmlDoc ((8,16), FSharp.Compiler.Xml.XmlDocCollector),
                             false, None, (8,16--8,36)), false,
                          [Let
                             (false,
                              [SynBinding
                                 (None, Normal, false, false, [],
                                  PreXmlDoc ((9,20), FSharp.Compiler.Xml.XmlDocCollector),
                                  SynValData
                                    (None,
                                     SynValInfo
                                       ([], SynArgInfo ([], false, None)), None),
                                  Named
                                    (SynIdent (x, None), false, None,
                                     (9,24--9,25)), None,
                                  Const (Int32 1, (9,28--9,29)), (9,24--9,25),
                                  Yes (9,20--9,29),
                                  { LeadingKeyword = Let (9,20--9,23)
                                    InlineKeyword = None
                                    EqualsRange = Some (9,26--9,27) })],
                              (9,20--9,29), { InKeyword = None })], false,
                          (8,16--9,29), { ModuleKeyword = Some (8,16--8,22)
                                          EqualsRange = Some (8,37--8,38) })],
                      false, (6,8--9,29), { ModuleKeyword = Some (6,8--6,14)
                                            EqualsRange = Some (6,22--6,23) })],
                  false, (5,4--9,29), { ModuleKeyword = Some (5,4--5,10)
                                        EqualsRange = Some (5,18--5,19) })],
              false, (4,0--9,29), { ModuleKeyword = Some (4,0--4,6)
                                    EqualsRange = Some (4,14--4,15) })],
          PreXmlDoc ((2,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (2,0--9,29), { LeadingKeyword = Module (2,0--2,6) })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [LineComment (1,0--1,52)] }, set []))

(8,16)-(8,22) parse error Modules cannot be nested inside types. Define modules at module or namespace level.
(7,17)-(7,40) parse error A type definition requires one or more members or other declarations. If you intend to define an empty class, struct or interface, then use 'type ... = class end', 'interface end' or 'struct end'.
