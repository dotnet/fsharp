ImplFile
  (ParsedImplFileInput
     ("/root/Expression/Record - Field 09.fs", false, QualifiedNameOfFile Foo,
      [], [],
      [SynModuleOrNamespace
         ([Foo], false, NamedModule,
          [Expr
             (Record
                (None, None,
                 [SynExprRecordField
                    ((SynLongIdent ([A], [], [None]), true), Some (3,4--3,5),
                     Some (Const (Int32 1, (3,6--3,7))), Some ((3,8--4,2), None));
                  SynExprRecordField
                    ((SynLongIdent ([B], [], [None]), true), None, None, None)],
                 (3,0--4,5)), (3,0--4,5))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--4,5), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))

(4,2)-(4,3) parse error Field bindings must have the form 'id = expr;'
