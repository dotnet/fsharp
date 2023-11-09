ImplFile
  (ParsedImplFileInput
     ("/root/Expression/DotLambda_ArgumentExpressionInInnerAppExpression.fs",
      false,
      QualifiedNameOfFile DotLambda_ArgumentExpressionInInnerAppExpression, [],
      [],
      [SynModuleOrNamespace
         ([DotLambda_ArgumentExpressionInInnerAppExpression], false, AnonModule,
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
                     App
                       (NonAtomic, false, Ident f,
                        DotLambda
                          (Ident P, (1,10--1,13),
                           { UnderscoreRange = (1,10--1,11)
                             DotRange = (1,11--1,12) }), (1,8--1,13)),
                     Const (Int32 123, (1,14--1,17)), (1,8--1,17)), (1,4--1,5),
                  Yes (1,0--1,17), { LeadingKeyword = Let (1,0--1,3)
                                     InlineKeyword = None
                                     EqualsRange = Some (1,6--1,7) })],
              (1,0--1,17))], PreXmlDocEmpty, [], None, (1,0--1,17),
          { LeadingKeyword = None })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))
