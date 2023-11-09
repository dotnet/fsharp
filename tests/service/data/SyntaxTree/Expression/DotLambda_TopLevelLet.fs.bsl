ImplFile
  (ParsedImplFileInput
     ("/root/Expression/DotLambda_TopLevelLet.fs", false,
      QualifiedNameOfFile DotLambda_TopLevelLet, [], [],
      [SynModuleOrNamespace
         ([DotLambda_TopLevelLet], false, AnonModule,
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
                       (NonAtomic, true,
                        LongIdent
                          (false,
                           SynLongIdent
                             ([op_PipeRight], [], [Some (OriginalNotation "|>")]),
                           None, (1,10--1,12)), Const (Int32 1, (1,8--1,9)),
                        (1,8--1,12)),
                     DotLambda
                       (App
                          (Atomic, false, Ident ToString,
                           Const (Unit, (1,23--1,25)), (1,15--1,25)),
                        (1,13--1,25), { UnderscoreRange = (1,13--1,14)
                                        DotRange = (1,14--1,15) }), (1,8--1,25)),
                  (1,4--1,5), NoneAtLet, { LeadingKeyword = Let (1,0--1,3)
                                           InlineKeyword = None
                                           EqualsRange = Some (1,6--1,7) })],
              (1,0--1,25))], PreXmlDocEmpty, [], None, (1,0--1,25),
          { LeadingKeyword = None })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))
