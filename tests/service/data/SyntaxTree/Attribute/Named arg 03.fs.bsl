ImplFile
  (ParsedImplFileInput
     ("/root/Attribute/Named arg 03.fs", false, QualifiedNameOfFile Module, [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Attributes
             ([{ Attributes =
                  [{ TypeName = SynLongIdent ([Attr], [], [None])
                     ArgExpr =
                      Paren
                        (FromParseError
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
                            (3,7--3,17)), (3,6--3,7), None, (3,6--3,17))
                     Target = None
                     AppliesToGetterAndSetter = false
                     Range = (3,2--3,17) }]
                 Range = (3,0--3,17) }], (3,0--3,17))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--3,17), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [] }, set []))

(3,16)-(3,17) parse error Unexpected token '=' or incomplete expression
(3,6)-(3,7) parse error Unmatched '('
(3,0)-(3,2) parse error Unmatched '[<'. Expected closing '>]'
(3,0)-(3,17) parse error Incomplete structured construct at or before this point in definition
