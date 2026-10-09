ImplFile
  (ParsedImplFileInput
     ("/root/Expression/Ident - Op 03.fs", false, QualifiedNameOfFile Foo, [],
      [],
      [SynModuleOrNamespace
         ([Foo], false, NamedModule,
          [Expr
             (LongIdent
                (false,
                 SynLongIdent
                   ([M; ], [(3,1--3,2)],
                    [None; Some (HasParenthesis ((3,2--3,3), (3,3--3,4)))]),
                 None, (3,0--3,4)), (3,0--3,4));
           Expr (Const (Unit, (5,0--5,2)), (5,0--5,2))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--5,2), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))

(3,2)-(3,4) parse error Attempted to parse this as an operator name, but failed
