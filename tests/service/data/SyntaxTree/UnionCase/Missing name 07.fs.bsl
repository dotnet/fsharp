ImplFile
  (ParsedImplFileInput
     ("/root/UnionCase/Missing name 07.fs", false, QualifiedNameOfFile Module,
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
                           ([], SynIdent (A, None), Fields [],
                            PreXmlDoc ((4,4), FSharp.Compiler.Xml.XmlDocCollector),
                            None, (4,6--4,7), { BarRange = Some (4,4--4,5) });
                         SynUnionCase
                           ([], SynIdent (, None), Fields [],
                            PreXmlDoc ((5,4), FSharp.Compiler.Xml.XmlDocCollector),
                            None, (5,4--5,5), { BarRange = Some (5,4--5,5) });
                         SynUnionCase
                           ([], SynIdent (C, None), Fields [],
                            PreXmlDoc ((6,4), FSharp.Compiler.Xml.XmlDocCollector),
                            None, (6,6--6,7), { BarRange = Some (6,4--6,5) });
                         SynUnionCase
                           ([], SynIdent (D, None), Fields [],
                            PreXmlDoc ((7,4), FSharp.Compiler.Xml.XmlDocCollector),
                            None, (7,6--7,7), { BarRange = Some (7,4--7,5) })],
                        (4,4--7,7)), (4,4--7,7)), [], None, (3,5--7,7),
                  { LeadingKeyword = Type (3,0--3,4)
                    EqualsRange = Some (3,7--3,8)
                    WithKeyword = None })], (3,0--7,7))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--7,7), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))

(6,4)-(6,5) parse error Unexpected symbol '|' in union case
