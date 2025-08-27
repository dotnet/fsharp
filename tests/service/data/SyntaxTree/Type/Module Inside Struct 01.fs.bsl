ImplFile
  (ParsedImplFileInput
     ("/root/Type/Module Inside Struct 01.fs", false, QualifiedNameOfFile Module,
      [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([], None, [], [MyStruct],
                     PreXmlDoc ((4,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (4,5--4,13)),
                  ObjectModel (Struct, [], (5,4--5,10)), [], None, (4,5--5,10),
                  { LeadingKeyword = Type (4,0--4,4)
                    EqualsRange = Some (4,14--4,15)
                    WithKeyword = None })], (4,0--5,10))],
          PreXmlDoc ((2,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (2,0--5,10), { LeadingKeyword = Module (2,0--2,6) })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [LineComment (1,0--1,32)] }, set []))

(5,4)-(5,10) parse error Unmatched 'class', 'interface' or 'struct'
(6,8)-(6,14) parse error Unexpected keyword 'module' in member definition
(8,4)-(8,7) parse error Incomplete structured construct at or before this point in definition. Expected incomplete structured construct at or before this point or other token.
