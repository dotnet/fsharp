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
                  Named (SynIdent (x, None), false, None, (1,4--1,5)),
                  Some
                    (SynBindingReturnInfo
                       (WithNull
                          (App
                             (LongIdent (SynLongIdent ([list], [], [None])),
                              None,
                              [LongIdent (SynLongIdent ([int], [], [None]))], [],
                              None, true, (1,8--1,16)), false, (1,8--1,23),
                           { BarRange = (1,17--1,18) }), (1,8--1,23), [],
                        { ColonRange = Some (1,6--1,7) })),
                  Typed
                    (ArbitraryAfterError ("localBinding2", (1,23--1,23)),
                     WithNull
                       (App
                          (LongIdent (SynLongIdent ([list], [], [None])), None,
                           [LongIdent (SynLongIdent ([int], [], [None]))], [],
                           None, true, (1,8--1,16)), false, (1,8--1,23),
                        { BarRange = (1,17--1,18) }), (1,23--1,23)), (1,4--1,5),
                  Yes (1,0--1,23), { LeadingKeyword = Let (1,0--1,3)
                                     InlineKeyword = None
                                     EqualsRange = None })], (1,0--1,23))],
          PreXmlDocEmpty, [], None, (1,0--2,0), { LeadingKeyword = None })],
      (true, true), { ConditionalDirectives = []
                      CodeComments = [] }, set []))

(1,24)-(1,25) parse error Unexpected symbol '|' (directly before 'null') in binding. Expected '=' or other token.
