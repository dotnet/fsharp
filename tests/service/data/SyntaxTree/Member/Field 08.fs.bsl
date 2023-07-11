ImplFile
  (ParsedImplFileInput
     ("/root/Member/Field 08.fs", false, QualifiedNameOfFile Module, [], [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([], None, [], [T],
                     PreXmlDoc ((3,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (3,5--3,6)),
                  ObjectModel
                    (Struct,
                     [ValField
                        (SynField
                           ([], false, Some , FromParseError (1,13--1,13), false,
                            PreXmlDoc ((5,8), FSharp.Compiler.Xml.XmlDocCollector),
                            None, (6,4--134217728,0),
                            { LeadingKeyword = Some (Val (5,8--5,11)) }),
                         (1,13--5,11))], (4,4--6,7)), [], None, (3,5--6,7),
                  { LeadingKeyword = Type (3,0--3,4)
                    EqualsRange = Some (3,7--3,8)
                    WithKeyword = None })], (3,0--6,7));
           Expr (Const (Unit, (8,0--8,2)), (8,0--8,2))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--8,2), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))

(6,4)-(6,7) parse error Unexpected keyword 'end' in member definition. Expected identifier or other token.
