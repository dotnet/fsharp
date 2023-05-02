ImplFile
  (ParsedImplFileInput
     ("/root/Expression/Record - Field 01.fs", false, QualifiedNameOfFile Foo,
      [], [],
      [SynModuleOrNamespace
         ([Foo], false, NamedModule,
          [Expr
             (ComputationExpr
                (false,
                 DiscardAfterMissingQualificationAfterDot
                   (Ident A, (3,3--3,4), (3,2--3,4)), (3,0--3,6)), (3,0--3,6))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--3,6), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))

(3,3)-(3,4) parse error Missing qualification after '.'
