ImplFile
  (ParsedImplFileInput
     ("/root/Expression/Lambda 01.fs", false, QualifiedNameOfFile Module, [], [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Expr
             (Paren
                (Lambda
                   (false, false,
                    SimplePats
                      ([Id (_arg1, None, true, false, false, (3,5--3,6))],
                       (3,5--3,6)), Const (Unit, (3,10--3,12)),
                    Some ([Wild (3,5--3,6)], Const (Unit, (3,10--3,12))),
                    (3,1--3,12), { ArrowRange = Some (3,7--3,9) }), (3,0--3,1),
                 Some (3,12--3,13), (3,0--3,13)), (3,0--3,13))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--3,13), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))
