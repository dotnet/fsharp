ImplFile
  (ParsedImplFileInput
     ("/root/Expression/Try 02.fs", false, QualifiedNameOfFile Module, [], [],
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
                  TryWith
                    (ArbitraryAfterError ("try1", (4,7--4,7)), [], (4,4--4,7),
                     Yes (4,4--4,7), Yes (4,7--4,7),
                     { TryKeyword = (4,4--4,7)
                       TryToWithRange = (4,4--4,7)
                       WithKeyword = (4,7--4,7)
                       WithToEndRange = (4,7--4,7) }), (3,4--3,5), NoneAtLet,
                  { LeadingKeyword = Let (3,0--3,3)
                    InlineKeyword = None
                    EqualsRange = Some (3,6--3,7) })], (3,0--4,7));
           Expr (Const (Int32 1, (6,0--6,1)), (6,0--6,1))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--6,1), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))

(6,0)-(6,1) parse error Possible incorrect indentation: this token is offside of context started at position (4:5). Try indenting this token further or using standard formatting conventions.
(6,0)-(6,1) parse error Expecting expression
