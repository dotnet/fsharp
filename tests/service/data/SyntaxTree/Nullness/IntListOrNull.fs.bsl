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
                  Named (SynIdent (x, None), false, None, (1,4--1,5)),
                  Some
                    (SynBindingReturnInfo
                       (App
                          (LongIdent (SynLongIdent ([list], [], [None])), None,
                           [LongIdent (SynLongIdent ([int], [], [None]))], [],
                           None, true, (1,8--1,16)), (1,8--1,16), [],
                        { ColonRange = Some (1,6--1,7) })),
                  Typed
                    (ArbitraryAfterError ("localBinding2", (1,16--1,16)),
                     App
                       (LongIdent (SynLongIdent ([list], [], [None])), None,
                        [LongIdent (SynLongIdent ([int], [], [None]))], [], None,
                        true, (1,8--1,16)), (1,16--1,16)), (1,4--1,5),
                  Yes (1,0--1,16), { LeadingKeyword = Let (1,0--1,3)
                                     InlineKeyword = None
                                     EqualsRange = None })], (1,0--1,16))],
          PreXmlDocEmpty, [], None, (1,0--2,0), { LeadingKeyword = None })],
      (true, true), { ConditionalDirectives = []
                      CodeComments = [] }, set []))

(1,17)-(1,18) parse error Unexpected symbol '|' in binding. Expected '=' or other token.
