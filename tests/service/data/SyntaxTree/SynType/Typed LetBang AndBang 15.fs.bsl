ImplFile
  (ParsedImplFileInput
     ("/root/SynType/Typed LetBang AndBang 15.fs", false,
      QualifiedNameOfFile Module, [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Expr
             (App
                (NonAtomic, false, Ident async,
                 ComputationExpr
                   (false,
                    App
                      (NonAtomic, false,
                       App
                         (NonAtomic, true,
                          LongIdent
                            (false,
                             SynLongIdent
                               ([op_Equality], [], [Some (OriginalNotation "=")]),
                             None, (5,24--5,25)),
                          LetOrUseBang
                            (Yes (4,4--4,39), false, true,
                             Typed
                               (Paren
                                  (Tuple
                                     (false,
                                      [Named
                                         (SynIdent (x, None), false, None,
                                          (4,10--4,11));
                                       Named
                                         (SynIdent (y, None), false, None,
                                          (4,13--4,14))], [(4,11--4,12)],
                                      (4,10--4,14)), (4,9--4,15)),
                                Tuple
                                  (false,
                                   [Type
                                      (LongIdent
                                         (SynLongIdent ([int], [], [None])));
                                    Star (4,21--4,22);
                                    Type
                                      (LongIdent
                                         (SynLongIdent ([int], [], [None])))],
                                   (4,17--4,26)), (4,9--4,26)),
                             App
                               (Atomic, false, Ident asyncInt,
                                Const (Unit, (4,37--4,39)), (4,29--4,39)), [],
                             Typed
                               (Ident y,
                                LongIdent (SynLongIdent ([int], [], [None])),
                                (5,17--5,23)), (4,4--5,23),
                             { LetOrUseBangKeyword = (4,4--4,8)
                               EqualsRange = Some (4,27--4,28) }), (4,4--5,25)),
                       ArbitraryAfterError ("declExprInfixEquals", (5,25--5,25)),
                       (4,4--5,25)), (3,6--7,1)), (3,0--7,1)), (3,0--7,1))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--7,1), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [] }, set []))

(5,15)-(5,16) parse error Unexpected symbol ',' in expression. Expected '=' or other token.
(4,4)-(4,8) parse error Incomplete value definition. If this is in an expression, the body of the expression must be indented to the same column as the 'let!' keyword.
(5,26)-(5,34) parse error Unexpected start of structured construct in expression
(5,24)-(5,25) parse error Unexpected token '=' or incomplete expression
