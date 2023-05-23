ImplFile
  (ParsedImplFileInput
     ("/root/Expression/Tuple - Missing item 10.fs", false,
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
                  Named (SynIdent (x, None), false, None, (3,4--3,5)), None,
                  Tuple
                    (false,
                     [Const (Int32 1, (3,8--3,9));
                      ArbitraryAfterError ("tupleExpr5", (3,10--3,10))],
                     [(3,9--3,10)], (3,8--3,10)), (3,4--3,5), Yes (3,0--3,10),
                  { LeadingKeyword = Let (3,0--3,3)
                    InlineKeyword = None
                    EqualsRange = Some (3,6--3,7) })], (3,0--3,10))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--3,10), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))

(4,0)-(4,0) parse warning Possible incorrect indentation: this token is offside of context started at position (3:9). Try indenting this token further or using standard formatting conventions.
(3,9)-(3,10) parse error Expected an expression after this point
