ImplFile
  (ParsedImplFileInput
     ("/root/Type/Union 06.fs", false, QualifiedNameOfFile Module, [], [],
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
                                 ([], false, None, FromParseError (4,11--4,11),
                                  false, PreXmlDocEmpty, None, (4,11--4,11),
                                  { LeadingKeyword = None
                                    MutableKeyword = None });
                               SynField
                                 ([], false, None,
                                  LongIdent (SynLongIdent ([int], [], [None])),
                                  false,
                                  PreXmlDoc ((4,13), FSharp.Compiler.Xml.XmlDocCollector),
                                  None, (4,13--4,16), { LeadingKeyword = None
                                                        MutableKeyword = None });
                               SynField
                                 ([], false, None,
                                  LongIdent (SynLongIdent ([int], [], [None])),
                                  false,
                                  PreXmlDoc ((4,19), FSharp.Compiler.Xml.XmlDocCollector),
                                  None, (4,19--4,22), { LeadingKeyword = None
                                                        MutableKeyword = None })],
                            PreXmlDoc ((4,4), FSharp.Compiler.Xml.XmlDocCollector),
                            None, (4,6--4,22), { BarRange = Some (4,4--4,5) })],
                        (4,4--4,22)), (4,4--4,22)), [], None, (3,5--4,22),
                  { LeadingKeyword = Type (3,0--3,4)
                    EqualsRange = Some (3,7--3,8)
                    WithKeyword = None })], (3,0--4,22))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--4,22), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))

(4,11)-(4,12) parse error Expecting union case field
