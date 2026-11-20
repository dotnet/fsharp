ImplFile
  (ParsedImplFileInput
     ("/root/Attribute/Named arg 04.fs", false, QualifiedNameOfFile Module, [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Attributes
             ([{ Attributes =
                  [{ TypeName = SynLongIdent ([Attr], [], [None])
                     ArgExpr =
                      Paren
                        (FromParseError
                           (App
                              (NonAtomic, false,
                               App
                                 (NonAtomic, true,
                                  LongIdent
                                    (false,
                                     SynLongIdent
                                       ([op_Equality], [],
                                        [Some (OriginalNotation "=")]), None,
                                     (3,9--3,10)), Ident A, (3,7--3,10)),
                               Const (Int32 1, (3,11--3,12)), (3,7--3,12)),
                            (3,7--3,12)), (3,6--3,7), None, (3,6--3,12))
                     Target = None
                     AppliesToGetterAndSetter = false
                     Range = (3,2--3,12) }]
                 Range = (3,0--3,14) }], (3,0--3,14));
           Expr (Do (Const (Unit, (4,3--4,5)), (4,0--4,5)), (4,0--4,5))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--4,5), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [] }, set []))

(3,12)-(3,14) parse error Unexpected symbol '>]' in expression
