ImplFile
  (ParsedImplFileInput
     ("/root/Expression/DotLambda_NestedPropertiesAfterUnderscore.fs", false,
      QualifiedNameOfFile DotLambda_NestedPropertiesAfterUnderscore, [], [],
      [SynModuleOrNamespace
         ([DotLambda_NestedPropertiesAfterUnderscore], false, AnonModule,
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
                  DotLambda
                    (LongIdent
                       (false,
                        SynLongIdent
                          ([MyProperty; MyOtherProperty], [(1,25--1,26)],
                           [None; None]), None, (1,15--1,41)), (1,13--1,41),
                     { UnderscoreRange = (1,13--1,14)
                       DotRange = (1,14--1,15) }), (1,4--1,10), Yes (1,0--1,41),
                  { LeadingKeyword = Let (1,0--1,3)
                    InlineKeyword = None
                    EqualsRange = Some (1,11--1,12) })], (1,0--1,41))],
          PreXmlDocEmpty, [], None, (1,0--1,41), { LeadingKeyword = None })],
      (true, true), { ConditionalDirectives = []
                      CodeComments = [] }, set []))
