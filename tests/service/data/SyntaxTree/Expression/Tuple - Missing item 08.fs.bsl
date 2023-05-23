ImplFile
  (ParsedImplFileInput
     ("/root/Expression/Tuple - Missing item 08.fs", false,
      QualifiedNameOfFile Module, [], [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Expr
             (Paren
                (FromParseError
                   (Tuple
                      (false,
                       [Const (Int32 1, (3,1--3,2));
                        ArbitraryAfterError ("tupleExpr5", (3,3--3,3))],
                       [(3,2--3,3)], (3,1--3,3)), (3,1--3,3)), (3,0--3,1), None,
                 (3,0--4,0)), (3,0--4,0))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--4,0), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))

(4,0)-(4,0) parse warning Possible incorrect indentation: this token is offside of context started at position (1:1). Try indenting this token further or using standard formatting conventions.
(3,2)-(3,3) parse error Expected an expression after this point
(3,0)-(3,1) parse error Unmatched '('
