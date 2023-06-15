ImplFile
  (ParsedImplFileInput
     ("/root/ModuleMember/Do 02.fs", false, QualifiedNameOfFile Module, [], [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Expr
             (Do
                (ArbitraryAfterError ("hardwhiteDoBinding1", (3,2--3,2)),
                 (3,0--3,2)), (3,0--3,2));
           Expr (Const (Int32 2, (6,0--6,1)), (6,0--6,1))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--6,1), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))

(4,0)-(4,4) parse error Possible incorrect indentation: this token is offside of context started at position (3:1). Try indenting this token further or using standard formatting conventions.
(4,0)-(4,4) parse error Expecting expression
