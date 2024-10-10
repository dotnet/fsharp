ImplFile
  (ParsedImplFileInput
     ("/root/Pattern/Typed - Missing type 05.fs", false,
      QualifiedNameOfFile Module, [], [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Expr
             (Do
                (FromParseError
                   (LetOrUse
                      (false, false,
                       [SynBinding
                          (None, Normal, false, false, [],
                           PreXmlDoc ((4,4), FSharp.Compiler.Xml.XmlDocCollector),
                           SynValData
                             (None,
                              SynValInfo ([], SynArgInfo ([], false, None)),
                              None),
                           Named (SynIdent (i, None), false, None, (4,8--4,9)),
                           Some
                             (SynBindingReturnInfo
                                (FromParseError (4,10--4,10), (4,10--4,10), [],
                                 { ColonRange = Some (4,9--4,10) })),
                           Typed
                             (ArbitraryAfterError
                                ("localBinding2", (4,10--4,10)),
                              FromParseError (4,10--4,10), (4,10--4,10)),
                           (4,8--4,9), Yes (4,4--4,10),
                           { LeadingKeyword = Let (4,4--4,7)
                             InlineKeyword = None
                             EqualsRange = None })],
                       ArbitraryAfterError ("seqExpr", (4,10--4,10)),
                       (4,4--4,10), { LetOrUseKeyword = (4,4--4,7)
                                      InKeyword = None }), (4,4--4,10)),
                 (3,0--4,10)), (3,0--4,10))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--4,10), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))

(6,4)-(6,5) parse error Incomplete structured construct at or before this point in binding
(7,0)-(7,0) parse error Unexpected end of input in value, function or member definition
(4,4)-(4,7) parse error Incomplete value or function definition. If this is in an expression, the body of the expression must be indented to the same column as the 'let' keyword.
(4,4)-(4,7) parse error The block following this 'let' is unfinished. Every code block is an expression and must have a result. 'let' cannot be the final code element in a block. Consider giving this block an explicit result.
(7,0)-(7,0) parse error Unexpected end of input in expression
