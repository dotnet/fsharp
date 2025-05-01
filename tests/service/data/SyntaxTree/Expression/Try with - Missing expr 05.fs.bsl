ImplFile
  (ParsedImplFileInput
     ("/root/Expression/Try with - Missing expr 05.fs", false,
      QualifiedNameOfFile Module, [], [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Expr
             (TryWith
                (ArbitraryAfterError ("try1", (3,3--3,3)), [], (3,0--3,3),
                 Yes (3,0--3,3), Yes (3,3--3,3), { TryKeyword = (3,0--3,3)
                                                   TryToWithRange = (3,0--3,3)
                                                   WithKeyword = (3,3--3,3)
                                                   WithToEndRange = (3,3--3,3) }),
              (3,0--3,3))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--3,3), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))

(5,0)-(5,6) parse error Expecting expression
(5,0)-(5,6) parse error Unexpected keyword 'module' in definition. Expected incomplete structured construct at or before this point or other token.
