ImplFile
  (ParsedImplFileInput
     ("/root/Lambda/Param - Missing type 04.fs", false,
      QualifiedNameOfFile Module, [], [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Expr
             (Paren
                (Lambda
                   (false, false,
                    SimplePats
                      ([Typed
                          (Id (i, None, false, false, false, (3,6--3,7)),
                           FromParseError (3,8--3,8), (3,6--3,8));
                        Id (j, None, false, false, false, (3,10--3,11))],
                       [(3,8--3,9)], (3,5--3,12)), Const (Unit, (3,16--3,18)),
                    Some
                      ([Paren
                          (Tuple
                             (false,
                              [Typed
                                 (Named
                                    (SynIdent (i, None), false, None, (3,6--3,7)),
                                  FromParseError (3,8--3,8), (3,6--3,8));
                               Named
                                 (SynIdent (j, None), false, None, (3,10--3,11))],
                              [(3,8--3,9)], (3,6--3,11)), (3,5--3,12))],
                       Const (Unit, (3,16--3,18))), (3,1--3,18),
                    { ArrowRange = Some (3,13--3,15) }), (3,0--3,1),
                 Some (3,18--3,19), (3,0--3,19)), (3,0--3,19))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--3,19), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))

(3,8)-(3,9) parse error Unexpected symbol ',' in pattern
