ImplFile
  (ParsedImplFileInput
     ("/root/Type/Module Inside Type With Augmentation 01.fs", false,
      QualifiedNameOfFile Module, [],
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
                    WithKeyword = Some (6,4--6,8) })], (4,0--5,7))],
          PreXmlDoc ((2,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (2,0--5,7), { LeadingKeyword = Module (2,0--2,6) })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [LineComment (1,0--1,52)] }, set []))

(7,8)-(7,14) parse error Modules cannot be nested inside types. Define modules at module or namespace level.
(7,8)-(7,14) parse error Unexpected keyword 'module' in type definition. Expected incomplete structured construct at or before this point, 'end' or other token.
(8,0)-(8,0) parse error Incomplete structured construct at or before this point in implementation file
