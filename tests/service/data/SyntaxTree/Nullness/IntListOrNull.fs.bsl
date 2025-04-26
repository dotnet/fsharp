ImplFile
  (ParsedImplFileInput
     ("/root/Nullness/IntListOrNull.fs", false,
      QualifiedNameOfFile IntListOrNull, [], [],
      [SynModuleOrNamespace
         ([IntListOrNull], false, AnonModule,
          [Let
             (false,
              [SynBinding
                 (None, Normal, false, false, [],
                  PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector),
                  SynValData
                    (None, SynValInfo ([], SynArgInfo ([], false, None)), None),
                  Typed
                    (Named (SynIdent (x, None), false, None, (1,4--1,5)),
                     WithNull
                       (App
                          (LongIdent (SynLongIdent ([list], [], [None])), None,
                           [LongIdent (SynLongIdent ([int], [], [None]))], [],
                           None, true, (1,8--1,16)), false, (1,8--1,23),
                        { BarRange = (1,17--1,18) }), (1,4--1,23)), None,
                  ArrayOrList (false, [], (1,26--1,28)), (1,4--1,23),
                  Yes (1,0--1,28), { LeadingKeyword = Let (1,0--1,3)
                                     InlineKeyword = None
                                     EqualsRange = Some (1,24--1,25) })],
              (1,0--1,28))], PreXmlDocEmpty, [], None, (1,0--2,0),
          { LeadingKeyword = None })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))
