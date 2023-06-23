ImplFile
  (ParsedImplFileInput
     ("/root/Pattern/Tuple - Recover 01.fs", false, QualifiedNameOfFile Tuple,
      [], [],
      [SynModuleOrNamespace
         ([Tuple], false, NamedModule,
          [Let
             (false,
              [SynBinding
                 (None, Normal, false, false, [],
                  PreXmlDoc ((3,0), FSharp.Compiler.Xml.XmlDocCollector),
                  SynValData
                    (None, SynValInfo ([], SynArgInfo ([], false, None)), None),
                  Tuple
                    (false,
                     [Named (SynIdent (a, None), false, None, (3,4--3,5));
                      Wild (4,9--4,9)], [(4,8--4,9)], (3,4--5,0)), None,
                  ArbitraryAfterError ("localBinding2", (5,0--5,0)), (3,4--5,0),
                  Yes (3,0--5,0), { LeadingKeyword = Let (3,0--3,3)
                                    InlineKeyword = None
                                    EqualsRange = None })], (3,0--5,0))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--5,0), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))

(5,0)-(5,0) parse error Possible incorrect indentation: this token is offside of context started at position (3:1). Try indenting this token further or using standard formatting conventions.
(5,0)-(5,0) parse error Incomplete structured construct at or before this point in binding
(4,8)-(4,9) parse error Expecting pattern
(5,0)-(5,0) parse error Unexpected end of input in value, function or member definition
(3,0)-(3,3) parse error Incomplete value or function definition. If this is in an expression, the body of the expression must be indented to the same column as the 'let' keyword.
