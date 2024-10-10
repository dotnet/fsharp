ImplFile
  (ParsedImplFileInput
     ("/root/Expression/Rarrow 02.fs", false, QualifiedNameOfFile Module, [], [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Expr
             (YieldOrReturn
                ((true, true),
                 ArbitraryAfterError ("typedSequentialExprBlockR1", (3,2--3,2)),
                 (3,0--3,2), { YieldOrReturnKeyword = (3,0--3,2) }), (3,0--3,2))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--3,2), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))

(4,0)-(4,0) parse error Incomplete structured construct at or before this point in expression
(3,0)-(4,0) parse error The use of '->' in sequence and computation expressions is limited to the form 'for pat in expr -> expr'. Use the syntax 'for ... in ... do ... yield...' to generate elements in more complex sequence expressions.
