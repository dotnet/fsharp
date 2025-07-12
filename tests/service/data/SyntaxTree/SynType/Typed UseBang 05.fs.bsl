ImplFile
  (ParsedImplFileInput
     ("/root/SynType/Typed UseBang 05.fs", false, QualifiedNameOfFile Module, [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Expr
             (App
                (NonAtomic, false, Ident async,
                 ComputationExpr
                   (false,
                    LetOrUseBang
                      (Yes (4,4--4,34), true, true,
                       Typed
                         (Named (SynIdent (res, None), false, None, (4,9--4,12)),
                          FromParseError (4,13--4,13), (4,9--4,13)),
                       App
                         (NonAtomic, false, Ident async,
                          ComputationExpr
                            (false,
                             YieldOrReturn
                               ((false, true), Const (Int32 1, (4,31--4,32)),
                                (4,24--4,32),
                                { YieldOrReturnKeyword = (4,24--4,30) }),
                             (4,22--4,34)), (4,16--4,34)), [],
                       YieldOrReturn
                         ((false, true), Ident res, (5,4--5,14),
                          { YieldOrReturnKeyword = (5,4--5,10) }), (4,4--5,14),
                       { LetOrUseBangKeyword = (4,4--4,8)
                         EqualsRange = Some (4,14--4,15) }), (3,6--6,1)),
                 (3,0--6,1)), (3,0--6,1))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--6,1), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [] }, set []))

(4,14)-(4,15) parse error Unexpected symbol '=' in expression
(4,13)-(4,13) parse error Expecting type
