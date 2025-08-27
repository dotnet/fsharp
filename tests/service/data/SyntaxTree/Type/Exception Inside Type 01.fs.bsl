ImplFile
  (ParsedImplFileInput
     ("/root/Type/Exception Inside Type 01.fs", false,
      QualifiedNameOfFile Module, [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([], None, [], [A],
                     PreXmlDoc ((4,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (4,5--4,6)),
                  Simple (None (4,5--4,6), (4,5--4,6)), [], None, (4,5--4,6),
                  { LeadingKeyword = Type (4,0--4,4)
                    EqualsRange = None
                    WithKeyword = None })], (4,0--4,6));
           Exception
             (SynExceptionDefn
                (SynExceptionDefnRepr
                   ([],
                    SynUnionCase
                      ([], SynIdent (MyException, None), Fields [],
                       PreXmlDocEmpty, None, (5,14--5,25), { BarRange = None }),
                    None, PreXmlDoc ((5,4), FSharp.Compiler.Xml.XmlDocCollector),
                    None, (5,4--5,25)), None, [], (5,4--5,25)), (5,4--5,25))],
          PreXmlDoc ((2,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (2,0--5,25), { LeadingKeyword = Module (2,0--2,6) })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [LineComment (1,0--1,33)] }, set []))

(5,4)-(5,13) parse error Exceptions must be defined at module level, not inside types.
(5,4)-(5,13) parse error Unexpected keyword 'exception' in type definition
(6,0)-(6,0) parse error Incomplete structured construct at or before this point in implementation file
