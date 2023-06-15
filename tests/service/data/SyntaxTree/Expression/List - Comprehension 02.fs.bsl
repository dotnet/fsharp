ImplFile
  (ParsedImplFileInput
     ("/root/Expression/List - Comprehension 02.fs", false,
      QualifiedNameOfFile Module, [], [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Expr
             (ArrayOrListComputed
                (false,
                 ForEach
                   (Yes (3,2--3,5), Yes (3,8--3,10), SeqExprOnly true, true,
                    Named (SynIdent (x, None), false, None, (3,6--3,7)),
                    ArrayOrList (false, [], (3,11--3,13)),
                    YieldOrReturn
                      ((true, false),
                       ArbitraryAfterError
                         ("typedSequentialExprBlockR1", (3,16--3,16)),
                       (3,14--3,16)), (3,2--3,16)), (3,0--3,18)), (3,0--3,18))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--3,18), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))

(3,17)-(3,18) parse error Incomplete structured construct at or before this point in expression
