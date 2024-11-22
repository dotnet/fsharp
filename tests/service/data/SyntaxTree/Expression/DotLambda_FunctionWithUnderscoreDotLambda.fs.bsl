ImplFile
  (ParsedImplFileInput
     ("/root/Expression/DotLambda_FunctionWithUnderscoreDotLambda.fs", false,
      QualifiedNameOfFile DotLambda_FunctionWithUnderscoreDotLambda, [],
      [SynModuleOrNamespace
         ([DotLambda_FunctionWithUnderscoreDotLambda], false, AnonModule,
          [Let
             (false,
              [SynBinding
                 (None, Normal, false, false, [],
                  PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector),
                  SynValData
                    (None, SynValInfo ([], SynArgInfo ([], false, None)), None),
                  Named (SynIdent (myFunc, None), false, None, (1,4--1,10)),
                  None,
                  DotLambda
                    (Ident MyProperty, (1,13--1,25),
                     { UnderscoreRange = (1,13--1,14)
                       DotRange = (1,14--1,15) }), (1,4--1,10), Yes (1,0--1,25),
                  { LeadingKeyword = Let (1,0--1,3)
                    InlineKeyword = None
                    EqualsRange = Some (1,11--1,12) })], (1,0--1,25))],
          PreXmlDocEmpty, [], None, (1,0--1,25), { LeadingKeyword = None })],
      (true, true), { ConditionalDirectives = []
                      WarnDirectives = []
                      CodeComments = [] }, set []))
