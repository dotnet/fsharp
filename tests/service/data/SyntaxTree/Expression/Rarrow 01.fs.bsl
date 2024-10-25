ImplFile
  (ParsedImplFileInput
     ("/root/Expression/Rarrow 01.fs", false, QualifiedNameOfFile Module, [], [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Expr
             (YieldOrReturn
                ((true, true), Const (Int32 1, (3,3--3,4)), (3,0--3,4),
                 { YieldOrReturnKeyword = (3,0--3,2) }), (3,0--3,4))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--3,4), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))

(3,0)-(3,4) parse error The use of '->' in sequence and computation expressions is limited to the form 'for pat in expr -> expr'. Use the syntax 'for ... in ... do ... yield...' to generate elements in more complex sequence expressions.
