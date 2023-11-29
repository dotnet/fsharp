ImplFile
  (ParsedImplFileInput
     ("/root/Expression/Try with - Missing expr 04.fs", false,
      QualifiedNameOfFile Module, [], [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Expr
             (TryWith
                (LetOrUse
                   (false, false,
                    [SynBinding
                       (None, Normal, false, false, [],
                        PreXmlDoc ((5,0), FSharp.Compiler.Xml.XmlDocCollector),
                        SynValData
                          (None, SynValInfo ([], SynArgInfo ([], false, None)),
                           None), Wild (5,4--5,5), None,
                        Const (Unit, (5,8--5,10)), (5,4--5,5), Yes (5,0--5,10),
                        { LeadingKeyword = Let (5,0--5,3)
                          InlineKeyword = None
                          EqualsRange = Some (5,6--5,7) })],
                    ArbitraryAfterError ("seqExpr", (5,10--5,10)), (5,0--5,10),
                    { InKeyword = None }), [], (3,0--5,10), Yes (3,0--3,3),
                 Yes (5,10--5,10), { TryKeyword = (3,0--3,3)
                                     TryToWithRange = (3,0--5,10)
                                     WithKeyword = (5,10--5,10)
                                     WithToEndRange = (3,0--5,10) }),
              (3,0--5,10))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--5,10), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))

(5,0)-(5,3) parse error The block following this 'let' is unfinished. Every code block is an expression and must have a result. 'let' cannot be the final code element in a block. Consider giving this block an explicit result.
(6,0)-(6,0) parse error Incomplete structured construct at or before this point in expression. Expected 'finally', 'with' or other token.
