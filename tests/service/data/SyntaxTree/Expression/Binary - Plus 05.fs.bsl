ImplFile
  (ParsedImplFileInput
     ("/root/Expression/Binary - Plus 05.fs", false, QualifiedNameOfFile Module,
      [], [],
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
                  App
                    (NonAtomic, false,
                     App
                       (NonAtomic, true,
                        LongIdent
                          (false,
                           SynLongIdent
                             ([op_Addition], [], [Some (OriginalNotation "+")]),
                           None, (4,6--4,7)), Ident a, (4,4--4,7)),
                     ArbitraryAfterError ("declExprInfixPlusMinus", (4,7--4,7)),
                     (4,4--4,7)), (3,4--3,5), Yes (3,0--4,7),
                  { LeadingKeyword = Let (3,0--3,3)
                    InlineKeyword = None
                    EqualsRange = Some (3,6--3,7) })], (3,0--4,7));
           Expr (Const (Unit, (6,0--6,2)), (6,0--6,2))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--6,2), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))

(6,0)-(6,1) parse error Possible incorrect indentation: this token is offside of context started at position (4:5). Try indenting this token further or using standard formatting conventions.
(4,6)-(4,7) parse error Unexpected token '+' or incomplete expression
