ImplFile
  (ParsedImplFileInput
     ("/root/Expression/DotLambda_NotAllowedFunctionExpressionWithArg.fs", false,
      QualifiedNameOfFile DotLambda_NotAllowedFunctionExpressionWithArg, [], [],
      [SynModuleOrNamespace
         ([DotLambda_NotAllowedFunctionExpressionWithArg], false, AnonModule,
          [Let
             (false,
              [SynBinding
                 (None, Normal, false, false, [],
                  PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector),
                  SynValData
                    (None, SynValInfo ([], SynArgInfo ([], false, None)), None,
                     None), Wild (1,4--1,5), None,
                  App
                    (NonAtomic, false,
                     DotLambda
                       (Ident P, (1,8--1,11), { UnderscoreRange = (1,8--1,9)
                                                DotRange = (1,9--1,10) }),
                     Const (Int32 123, (1,12--1,15)), (1,8--1,15)), (1,4--1,5),
                  Yes (1,0--1,15), { LeadingKeyword = Let (1,0--1,3)
                                     InlineKeyword = None
                                     EqualsRange = Some (1,6--1,7) })],
              (1,0--1,15))], PreXmlDocEmpty, [], None, (1,0--1,15),
          { LeadingKeyword = None })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))
