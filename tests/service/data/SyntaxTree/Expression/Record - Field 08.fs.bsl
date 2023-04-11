ImplFile
  (ParsedImplFileInput
     ("/root/Expression/Record - Field 08.fs", false, QualifiedNameOfFile Foo,
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
                    ((SynLongIdent ([B], [(4,3--4,4)], [None]), true), None,
                     None, None)], (3,0--4,6)), (3,0--4,6))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--4,6), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))

(4,3)-(4,4) parse error Unexpected end of type. Expected a name after this point.
(4,2)-(4,6) parse error Field bindings must have the form 'id = expr;'
