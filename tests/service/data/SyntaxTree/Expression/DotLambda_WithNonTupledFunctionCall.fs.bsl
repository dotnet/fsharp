ImplFile
  (ParsedImplFileInput
     ("/root/Expression/DotLambda_WithNonTupledFunctionCall.fs", false,
      QualifiedNameOfFile DotLambda_WithNonTupledFunctionCall, [], [],
      [SynModuleOrNamespace
         ([DotLambda_WithNonTupledFunctionCall], false, AnonModule,
          [Let
             (false,
              [SynBinding
                 (None, Normal, false, false, [],
                  PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector),
                  SynValData
                    (None, SynValInfo ([], SynArgInfo ([], false, None)), None,
                     None),
                  Named (SynIdent (myFunc, None), false, None, (1,4--1,10)),
                  None,
                  App
                    (NonAtomic, false,
                     App
                       (NonAtomic, false,
                        App
                          (NonAtomic, false,
                           DotLambda
                             (Ident ThisIsMyFunction, (1,13--1,31),
                              { UnderscoreRange = (1,13--1,14)
                                DotRange = (1,14--1,15) }), Ident a,
                           (1,13--1,33)), Ident b, (1,13--1,35)), Ident c,
                     (1,13--1,37)), (1,4--1,10), Yes (1,0--1,37),
                  { LeadingKeyword = Let (1,0--1,3)
                    InlineKeyword = None
                    EqualsRange = Some (1,11--1,12) })], (1,0--1,37))],
          PreXmlDocEmpty, [], None, (1,0--1,37), { LeadingKeyword = None })],
      (true, true), { ConditionalDirectives = []
                      CodeComments = [] }, set []))
