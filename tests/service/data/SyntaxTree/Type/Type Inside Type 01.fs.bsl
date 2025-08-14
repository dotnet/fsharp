ImplFile
  (ParsedImplFileInput
     ("/root/Type/Type Inside Type 01.fs", false, QualifiedNameOfFile Module, [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([], None, [], [A],
                     PreXmlDoc ((4,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (4,5--4,6)),
                  Simple
                    (Union
                       (None,
                        [SynUnionCase
                           ([], SynIdent (A, None), Fields [],
                            PreXmlDoc ((5,4), FSharp.Compiler.Xml.XmlDocCollector),
                            None, (5,6--5,7), { BarRange = Some (5,4--5,5) })],
                        (5,4--5,7)), (5,4--5,7)), [], None, (4,5--5,7),
                  { LeadingKeyword = Type (4,0--4,4)
                    EqualsRange = Some (4,7--4,8)
                    WithKeyword = None })], (4,0--5,7));
           Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([], None, [], [NestedType],
                     PreXmlDoc ((6,4), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (6,9--6,19)),
                  Simple
                    (Union
                       (None,
                        [SynUnionCase
                           ([], SynIdent (B, None),
                            Fields
                              [SynField
                                 ([], false, None,
                                  LongIdent (SynLongIdent ([int], [], [None])),
                                  false,
                                  PreXmlDoc ((7,15), FSharp.Compiler.Xml.XmlDocCollector),
                                  None, (7,15--7,18), { LeadingKeyword = None
                                                        MutableKeyword = None })],
                            PreXmlDoc ((7,8), FSharp.Compiler.Xml.XmlDocCollector),
                            None, (7,10--7,18), { BarRange = Some (7,8--7,9) })],
                        (7,8--7,18)), (7,8--7,18)), [], None, (6,9--7,18),
                  { LeadingKeyword = Type (6,4--6,8)
                    EqualsRange = Some (6,20--6,21)
                    WithKeyword = None })], (6,4--7,18))],
          PreXmlDoc ((2,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (2,0--7,18), { LeadingKeyword = Module (2,0--2,6) })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [LineComment (1,0--1,41)] }, set []))

(6,4)-(6,8) parse error Nested type definitions are not allowed. Types must be defined at module or namespace level.
