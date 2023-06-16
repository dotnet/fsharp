ImplFile
  (ParsedImplFileInput
     ("/root/Pattern/Tuple - Recover 02.fs", false, QualifiedNameOfFile Tuple,
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
                      Wild (3,6--3,6); Wild (3,7--3,7)],
                     [(3,5--3,6); (3,6--3,7)], (3,4--4,0)), None,
                  ArbitraryAfterError ("localBinding2", (4,0--4,0)), (3,4--4,0),
                  Yes (3,0--4,0), { LeadingKeyword = Let (3,0--3,3)
                                    InlineKeyword = None
                                    EqualsRange = None })], (3,0--4,0))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--4,0), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))

(4,0)-(4,0) parse error Possible incorrect indentation: this token is offside of context started at position (3:1). Try indenting this token further or using standard formatting conventions.
(4,0)-(4,0) parse error Incomplete structured construct at or before this point in binding
(4,0)-(4,0) parse error Unexpected end of input in value, function or member definition
(3,0)-(3,3) parse error Incomplete value or function definition. If this is in an expression, the body of the expression must be indented to the same column as the 'let' keyword.
