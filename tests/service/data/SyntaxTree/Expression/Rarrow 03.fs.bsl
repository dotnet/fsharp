ImplFile
  (ParsedImplFileInput
     ("/root/Expression/Rarrow 03.fs", false, QualifiedNameOfFile Module, [], [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Expr
             (YieldOrReturn
                ((true, true),
                 YieldOrReturn
                   ((true, true),
                    ArbitraryAfterError
                      ("typedSequentialExprBlockR", (3,3--3,5)), (3,3--3,5)),
                 (3,0--3,5)), (3,0--3,5))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--3,5), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))

(4,0)-(4,0) parse error Incomplete structured construct at or before this point in expression
(3,3)-(4,0) parse error The use of '->' in sequence and computation expressions is limited to the form 'for pat in expr -> expr'. Use the syntax 'for ... in ... do ... yield...' to generate elements in more complex sequence expressions.
(3,0)-(4,0) parse error The use of '->' in sequence and computation expressions is limited to the form 'for pat in expr -> expr'. Use the syntax 'for ... in ... do ... yield...' to generate elements in more complex sequence expressions.
