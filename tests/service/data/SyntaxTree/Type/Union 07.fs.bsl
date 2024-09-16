ImplFile
  (ParsedImplFileInput
     ("/root/Type/Union 07.fs", false, QualifiedNameOfFile Module, [], [],
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
                           ([], SynIdent (A, None),
                            Fields
                              [SynField
                                 ([], false, None,
                                  LongIdent (SynLongIdent ([int], [], [None])),
                                  false,
                                  PreXmlDoc ((4,11), FSharp.Compiler.Xml.XmlDocCollector),
                                  None, (4,11--4,14), { LeadingKeyword = None
                                                        MutableKeyword = None });
                               SynField
                                 ([], false, None, FromParseError (4,16--4,16),
                                  false, PreXmlDocEmpty, None, (4,16--4,16),
                                  { LeadingKeyword = None
                                    MutableKeyword = None })],
                            PreXmlDoc ((4,4), FSharp.Compiler.Xml.XmlDocCollector),
                            None, (4,6--4,16), { BarRange = Some (4,4--4,5) });
                         SynUnionCase
                           ([], SynIdent (B, None), Fields [],
                            PreXmlDoc ((5,4), FSharp.Compiler.Xml.XmlDocCollector),
                            None, (5,6--5,7), { BarRange = Some (5,4--5,5) })],
                        (4,4--5,7)), (4,4--5,7)), [], None, (3,5--5,7),
                  { LeadingKeyword = Type (3,0--3,4)
                    EqualsRange = Some (3,7--3,8)
                    WithKeyword = None })], (3,0--5,7))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--5,7), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))

(5,4)-(5,5) parse error Unexpected symbol '|' in union case
