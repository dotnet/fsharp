ImplFile
  (ParsedImplFileInput
     ("/root/Expression/Set 04.fs", false, QualifiedNameOfFile Module, [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Let
             (false,
              [SynBinding
                 (None, Normal, false, false, [],
                  PreXmlDoc ((3,0), FSharp.Compiler.Xml.XmlDocCollector),
                  SynValData
                    (None, SynValInfo ([], SynArgInfo ([], false, None)), None),
                  Wild (3,4--3,5), None,
                  LongIdentSet
                    (SynLongIdent ([x], [], [None]),
                     ArbitraryAfterError ("declExprBlock1", (4,8--4,8)),
                     (4,4--4,8)), (3,4--3,5), Yes (3,0--4,8),
                  { LeadingKeyword = Let (3,0--3,3)
                    InlineKeyword = None
                    EqualsRange = Some (3,6--3,7) })], (3,0--4,8));
           Expr (Const (Int32 2, (6,0--6,1)), (6,0--6,1))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--6,1), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [] }, set []))

(6,0)-(6,1) parse error Unexpected syntax or possible incorrect indentation: this token is offside of context started at position (4:5). Try indenting this further.
To continue using non-conforming indentation, pass the '--strict-indentation-' flag to the compiler, or set the language version to F# 7.
(6,0)-(6,1) parse error Expecting expression
