ImplFile
  (ParsedImplFileInput
     ("/root/Expression/Record - Field 13.fs", false, QualifiedNameOfFile Foo,
      [], [],
      [SynModuleOrNamespace
         ([Foo], false, NamedModule,
          [Expr
             (Record
                (None, None,
                 [SynExprRecordField
                    ((SynLongIdent ([F1], [], [None]), true), Some (3,5--3,6),
                     Some (Const (Int32 1, (3,7--3,8))), Some ((3,9--4,2), None));
                  SynExprRecordField
                    ((SynLongIdent ([F2], [], [None]), true), Some (4,5--4,6),
                     None, None)], (3,0--4,8)), (3,0--4,8))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--4,8), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))

(4,2)-(4,4) parse error Field bindings must have the form 'id = expr;'
