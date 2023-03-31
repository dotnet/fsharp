ImplFile
  (ParsedImplFileInput
     ("/root/Expression/Record - Field 04.fs", false, QualifiedNameOfFile Foo,
      [], [],
      [SynModuleOrNamespace
         ([Foo], false, NamedModule,
          [Expr
             (Record
                (None, None,
                 [SynExprRecordField
                    ((SynLongIdent
                        ([A; B], [(3,3--3,4); (3,5--3,6)], [None; None]), true),
                     Some (3,7--3,8), Some (Const (Int32 1, (3,9--3,10))), None)],
                 (3,0--3,12)), (3,0--3,12))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--3,12), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))

(3,5)-(3,6) parse error Missing qualification after '.'
