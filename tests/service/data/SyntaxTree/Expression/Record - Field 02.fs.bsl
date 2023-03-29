ImplFile
  (ParsedImplFileInput
     ("/root/Expression/Record - Field 02.fs", false, QualifiedNameOfFile Foo,
      [], [],
      [SynModuleOrNamespace
         ([Foo], false, NamedModule,
          [Expr
             (ComputationExpr
                (false,
                 DiscardAfterMissingQualificationAfterDot
                   (LongIdent
                      (false, SynLongIdent ([A; B], [(3,3--3,4)], [None; None]),
                       None, (3,2--3,5)), (3,5--3,6), (3,2--3,6)), (3,0--3,8)),
              (3,0--3,8))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--3,8), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))

(3,5)-(3,6) parse error Missing qualification after '.'
