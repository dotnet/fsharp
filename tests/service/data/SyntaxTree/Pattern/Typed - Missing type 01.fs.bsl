ImplFile
  (ParsedImplFileInput
     ("/root/Pattern/Typed - Missing type 01.fs", false,
      QualifiedNameOfFile Module, [], [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Let
             (false,
              [SynBinding
                 (None, Normal, false, false, [],
                  PreXmlDoc ((3,0), FSharp.Compiler.Xml.XmlDocCollector),
                  SynValData
                    (None, SynValInfo ([], SynArgInfo ([], false, None)), None),
                  Named (SynIdent (i, None), false, None, (3,4--3,5)),
                  Some
                    (SynBindingReturnInfo
                       (FromParseError (3,6--3,6), (3,6--3,6), [],
                        { ColonRange = Some (3,5--3,6) })),
                  Typed
                    (ArbitraryAfterError ("localBinding2", (5,1--5,1)),
                     FromParseError (3,6--3,6), (5,1--5,1)), (3,4--3,5),
                  Yes (3,0--5,1), { LeadingKeyword = Let (3,0--3,3)
                                    InlineKeyword = None
                                    EqualsRange = None })], (3,0--5,1))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--5,1), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))

(5,0)-(5,1) parse error Incomplete structured construct at or before this point in binding
(6,0)-(6,0) parse error Unexpected end of input in value, function or member definition
(3,0)-(3,3) parse error Incomplete value or function definition. If this is in an expression, the body of the expression must be indented to the same column as the 'let' keyword.
