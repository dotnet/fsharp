ImplFile
  (ParsedImplFileInput
     ("/root/Expression/Record - Anon 10.fs", false, QualifiedNameOfFile Module,
      [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Expr
             (AnonRecd
                (false, None,
                 [SynExprRecordField
                    ((SynLongIdent ([F1], [], [None]), true), Some (3,6--3,7),
                     Some (Const (Int32 1, (3,8--3,9))), (3,3--3,9),
                     Some (Offside ((3,10--4,3), None)));
                  SynExprRecordField
                    ((SynLongIdent ([F2], [], [None]), true), None, None,
                     (4,3--4,5), Some (Offside ((4,6--5,3), None)));
                  SynExprRecordField
                    ((SynLongIdent ([F3], [], [None]), true), Some (5,6--5,7),
                     Some (Const (Int32 3, (5,8--5,9))), (5,3--5,9), None)],
                 (3,0--5,12), { OpeningBraceRange = (3,0--3,2) }), (3,0--5,12))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--5,12), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [] }, set []))

(4,3)-(4,5) parse error Field bindings must have the form 'id = expr;'
