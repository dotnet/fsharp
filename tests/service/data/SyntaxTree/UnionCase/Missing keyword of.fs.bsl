ImplFile
  (ParsedImplFileInput
     ("/root/UnionCase/Missing keyword of.fs", false, QualifiedNameOfFile Module,
      [], [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([], None, [], [U],
                     PreXmlDoc ((3,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (3,5--3,6)),
                  Simple
                    (Union
                       (None,
                        [SynUnionCase
                           ([], SynIdent (Case1, None), Fields [],
                            PreXmlDoc ((4,4), FSharp.Compiler.Xml.XmlDocCollector),
                            None, (4,6--4,11), { BarRange = Some (4,4--4,5) });
                         SynUnionCase
                           ([], SynIdent (Case2, None),
                            Fields
                              [SynField
                                 ([], false, None,
                                  LongIdent
                                    (SynLongIdent ([string], [], [None])), false,
                                  PreXmlDoc ((5,12), FSharp.Compiler.Xml.XmlDocCollector),
                                  None, (5,12--5,18), { LeadingKeyword = None })],
                            PreXmlDoc ((5,4), FSharp.Compiler.Xml.XmlDocCollector),
                            None, (5,6--5,18), { BarRange = Some (5,4--5,5) });
                         SynUnionCase
                           ([], SynIdent (Case3, None),
                            Fields
                              [SynField
                                 ([], false, None,
                                  LongIdent (SynLongIdent ([int], [], [None])),
                                  false,
                                  PreXmlDoc ((6,15), FSharp.Compiler.Xml.XmlDocCollector),
                                  None, (6,15--6,18), { LeadingKeyword = None })],
                            PreXmlDoc ((6,4), FSharp.Compiler.Xml.XmlDocCollector),
                            None, (6,6--6,18), { BarRange = Some (6,4--6,5) });
                         SynUnionCase
                           ([], SynIdent (Case4, None),
                            Fields
                              [SynField
                                 ([], false, None,
                                  StaticConstant (Int32 4, (7,12--7,13)), false,
                                  PreXmlDoc ((7,12), FSharp.Compiler.Xml.XmlDocCollector),
                                  None, (7,12--7,13), { LeadingKeyword = None })],
                            PreXmlDoc ((7,4), FSharp.Compiler.Xml.XmlDocCollector),
                            None, (7,6--7,13), { BarRange = Some (7,4--7,5) })],
                        (4,4--7,13)), (4,4--7,13)), [], None, (3,5--7,13),
                  { LeadingKeyword = Type (3,0--3,4)
                    EqualsRange = Some (3,7--3,8)
                    WithKeyword = None })], (3,0--7,13))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--7,13), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))

(5,6)-(5,18) parse error Missing keyword 'of'
(7,6)-(7,13) parse error Missing keyword 'of'
