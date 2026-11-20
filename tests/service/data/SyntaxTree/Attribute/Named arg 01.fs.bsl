ImplFile
  (ParsedImplFileInput
     ("/root/Attribute/Named arg 01.fs", false, QualifiedNameOfFile Module, [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Attributes
             ([{ Attributes =
                  [{ TypeName = SynLongIdent ([Attr], [], [None])
                     ArgExpr =
                      Paren
                        (Tuple
                           (false,
                            [App
                               (NonAtomic, false,
                                App
                                  (NonAtomic, true,
                                   LongIdent
                                     (false,
                                      SynLongIdent
                                        ([op_Equality], [],
                                         [Some (OriginalNotation "=")]), None,
                                      (3,9--3,10)), Ident A, (3,7--3,10)),
                                Const (Int32 1, (3,11--3,12)), (3,7--3,12));
                             App
                               (NonAtomic, false,
                                App
                                  (NonAtomic, true,
                                   LongIdent
                                     (false,
                                      SynLongIdent
                                        ([op_Equality], [],
                                         [Some (OriginalNotation "=")]), None,
                                      (3,16--3,17)), Ident B, (3,14--3,17)),
                                ArbitraryAfterError
                                  ("declExprInfixEquals", (3,17--3,17)),
                                (3,14--3,17))], [(3,12--3,13)], (3,7--3,17)),
                         (3,6--3,7), Some (3,17--3,18), (3,6--3,18))
                     Target = None
                     AppliesToGetterAndSetter = false
                     Range = (3,2--3,18) }]
                 Range = (3,0--3,20) }], (3,0--3,20));
           Expr (Do (Const (Unit, (4,3--4,5)), (4,0--4,5)), (4,0--4,5))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--4,5), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [] }, set []))

(3,16)-(3,17) parse error Unexpected token '=' or incomplete expression
