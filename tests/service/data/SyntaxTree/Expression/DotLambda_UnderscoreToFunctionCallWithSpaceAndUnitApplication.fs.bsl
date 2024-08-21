ImplFile
  (ParsedImplFileInput
     ("/root/Expression/DotLambda_UnderscoreToFunctionCallWithSpaceAndUnitApplication.fs",
      false,
      QualifiedNameOfFile
        DotLambda_UnderscoreToFunctionCallWithSpaceAndUnitApplication, [], [],
      [SynModuleOrNamespace
         ([DotLambda_UnderscoreToFunctionCallWithSpaceAndUnitApplication], false,
          AnonModule,
          [Let
             (false,
              [SynBinding
                 (None, Normal, false, false, [],
                  PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector),
                  SynValData
                    (None, SynValInfo ([], SynArgInfo ([], false, None)), None),
                  Named (SynIdent (myFunc, None), false, None, (1,4--1,10)),
                  None,
                  App
                    (NonAtomic, false,
                     DotLambda
                       (Ident MyMethodCall, (1,13--1,27),
                        { UnderscoreRange = (1,13--1,14)
                          DotRange = (1,14--1,15) }), Const (Unit, (1,28--1,30)),
                     (1,13--1,30)), (1,4--1,10), Yes (1,0--1,30),
                  { LeadingKeyword = Let (1,0--1,3)
                    InlineKeyword = None
                    EqualsRange = Some (1,11--1,12) })], (1,0--1,30))],
          PreXmlDocEmpty, [], None, (1,0--1,30), { LeadingKeyword = None })],
      (true, true), { ConditionalDirectives = []
                      CodeComments = [] }, set []))
