SigFile
  (ParsedSigFileInput
     ("/root/UnionCase/Missing type 02.fsi", QualifiedNameOfFile Module, [], [],
      [SynModuleOrNamespaceSig
         ([Module], false, NamedModule,
          [Types
             ([SynTypeDefnSig
                 (SynComponentInfo
                    ([], None, [], [U],
                     PreXmlDoc ((3,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (3,5--3,6)),
                  Simple
                    (Union
                       (None,
                        [SynUnionCase
                           ([], SynIdent (A, None),
                            Fields
                              [SynField
                                 ([], false, Some i, FromParseError (4,13--4,13),
                                  false,
                                  PreXmlDoc ((4,11), FSharp.Compiler.Xml.XmlDocCollector),
                                  None, (4,11--4,13), { LeadingKeyword = None })],
                            PreXmlDoc ((4,4), FSharp.Compiler.Xml.XmlDocCollector),
                            None, (4,6--7,5), { BarRange = Some (4,4--4,5) });
                         SynUnionCase
                           ([], SynIdent (B, None), Fields [],
                            PreXmlDoc ((7,4), FSharp.Compiler.Xml.XmlDocCollector),
                            None, (6,4--7,7), { BarRange = Some (7,4--7,5) })],
                        (4,4--7,7)), (4,4--7,7)), [], (3,5--7,7),
                  { LeadingKeyword = Type (3,0--3,4)
                    EqualsRange = Some (3,7--3,8)
                    WithKeyword = None })], (3,0--7,7))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--7,7), { LeadingKeyword = Module (1,0--1,6) })],
      { ConditionalDirectives = []
        CodeComments = [] }, set []))

(7,4)-(7,5) parse error Unexpected symbol '|' in union case
