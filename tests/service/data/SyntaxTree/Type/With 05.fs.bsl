ImplFile
  (ParsedImplFileInput
     ("/root/Type/With 05.fs", false, QualifiedNameOfFile Module, [],
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
                            None, (4,6--4,7), { BarRange = Some (4,4--4,5) })],
                        (4,4--4,7)), (4,4--4,7)), [], None, (3,5--4,7),
                  { LeadingKeyword = Type (3,0--3,4)
                    EqualsRange = Some (3,7--3,8)
                    WithKeyword = Some (4,8--4,12) })], (3,0--4,7))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--4,7), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [] }, set []))

(6,0)-(6,6) parse error Unexpected syntax or possible incorrect indentation: this token is offside of context started at position (3:1). Try indenting this further.
To continue using non-conforming indentation, pass the '--strict-indentation-' flag to the compiler, or set the language version to F# 7.
(6,0)-(6,6) parse error Unexpected keyword 'member' in definition. Expected incomplete structured construct at or before this point or other token.
