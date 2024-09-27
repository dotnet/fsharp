ImplFile
  (ParsedImplFileInput
     ("/root/Type/Union 03.fs", false, QualifiedNameOfFile Module, [], [],
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
                            None, (4,6--4,16), { BarRange = Some (4,4--4,5) })],
                        (4,4--4,16)), (4,4--4,16)), [], None, (3,5--4,16),
                  { LeadingKeyword = Type (3,0--3,4)
                    EqualsRange = Some (3,7--3,8)
                    WithKeyword = None })], (3,0--4,16))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--4,16), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))

(5,0)-(5,0) parse error Unexpected syntax or possible incorrect indentation: this token is offside of context started at position (4:5). Try indenting this further.
To continue using non-conforming indentation, pass the '--strict-indentation-' flag to the compiler, or set the language version to F# 7.
(5,0)-(5,0) parse error Incomplete structured construct at or before this point in union case
