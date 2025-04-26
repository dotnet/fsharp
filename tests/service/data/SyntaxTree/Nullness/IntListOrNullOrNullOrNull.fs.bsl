ImplFile
  (ParsedImplFileInput
     ("/root/Nullness/IntListOrNullOrNullOrNull.fs", false,
      QualifiedNameOfFile IntListOrNullOrNullOrNull, [], [],
      [SynModuleOrNamespace
         ([IntListOrNullOrNullOrNull], false, AnonModule,
          [Let
             (false,
              [SynBinding
                 (None, Normal, false, false, [],
                  PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector),
                  SynValData
                    (None, SynValInfo ([], SynArgInfo ([], false, None)), None),
                  Or
                    (Or
                       (Typed
                          (Named (SynIdent (x, None), false, None, (1,4--1,5)),
                           WithNull
                             (App
                                (LongIdent (SynLongIdent ([list], [], [None])),
                                 None,
                                 [LongIdent (SynLongIdent ([int], [], [None]))],
                                 [], None, true, (1,8--1,16)), false,
                              (1,8--1,23), { BarRange = (1,17--1,18) }),
                           (1,4--1,23)), Null (1,26--1,30), (1,4--1,30),
                        { BarRange = (1,24--1,25) }), Null (1,33--1,37),
                     (1,4--1,37), { BarRange = (1,31--1,32) }), None,
                  ArrayOrList (false, [], (1,40--1,42)), (1,4--1,37),
                  Yes (1,0--1,42), { LeadingKeyword = Let (1,0--1,3)
                                     InlineKeyword = None
                                     EqualsRange = Some (1,38--1,39) })],
              (1,0--1,42))], PreXmlDocEmpty, [], None, (1,0--2,0),
          { LeadingKeyword = None })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))
