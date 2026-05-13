ImplFile
  (ParsedImplFileInput
     ("/root/Expression/Match 08.fs", false, QualifiedNameOfFile Module, [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Expr
             (Do
                (Match
                   (Yes (4,4--4,16), Const (Int32 1, (4,10--4,11)), [],
                    (4,4--4,16), { MatchKeyword = (4,4--4,9)
                                   WithKeyword = (4,12--4,16) }), (3,0--4,16)),
              (3,0--4,16))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--4,16), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [] }, set []))

(6,4)-(6,7) parse error Unexpected keyword 'let' or 'use' in expression
(7,4)-(7,5) parse error Unexpected symbol '(' in definition. Expected incomplete structured construct at or before this point or other token.
