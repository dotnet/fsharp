ImplFile
  (ParsedImplFileInput
     ("/root/Expression/WhileBang 04.fs", false, QualifiedNameOfFile Module, [],
      [],
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
                  WhileBang
                    (Yes (4,4--4,32),
                     App
                       (NonAtomic, false, Ident async,
                        ComputationExpr
                          (false,
                           YieldOrReturn
                             ((false, true), Const (Bool true, (4,26--4,30)),
                              (4,19--4,30),
                              { YieldOrReturnKeyword = (4,19--4,25) }),
                           (4,17--4,32)), (4,11--4,32)),
                     ArbitraryAfterError ("whileBody1", (5,0--5,0)), (4,4--4,35)),
                  (3,4--3,5), Yes (3,0--4,35), { LeadingKeyword = Let (3,0--3,3)
                                                 InlineKeyword = None
                                                 EqualsRange = Some (3,6--3,7) })],
              (3,0--4,35))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--4,35), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))

(5,0)-(5,0) parse error Unexpected syntax or possible incorrect indentation: this token is offside of context started at position (4:5). Try indenting this further.
To continue using non-conforming indentation, pass the '--strict-indentation-' flag to the compiler, or set the language version to F# 7.
(5,0)-(5,0) parse error Incomplete structured construct at or before this point in expression
