ImplFile
  (ParsedImplFileInput
     ("/root/Record/Field - Missing type 03.fs", false,
      QualifiedNameOfFile Module, [], [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([], None, [], [R],
                     PreXmlDoc ((3,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (3,5--3,6)),
                  Simple
                    (Record
                       (None,
                        [SynField
                           ([], false, Some F1,
                            LongIdent (SynLongIdent ([int], [], [None])), false,
                            PreXmlDoc ((4,6), FSharp.Compiler.Xml.XmlDocCollector),
                            None, (4,6--4,13), { LeadingKeyword = None });
                         SynField
                           ([], false, Some F2, FromParseError (5,8--5,8), false,
                            PreXmlDoc ((5,6), FSharp.Compiler.Xml.XmlDocCollector),
                            None, (5,6--5,8), { LeadingKeyword = None })],
                        (4,4--5,10)), (4,4--5,10)), [], None, (3,5--5,10),
                  { LeadingKeyword = Type (3,0--3,4)
                    EqualsRange = Some (3,7--3,8)
                    WithKeyword = None })], (3,0--5,10))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--5,10), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))

(5,9)-(5,10) parse error Unexpected symbol '}' in field declaration. Expected ':' or other token.
