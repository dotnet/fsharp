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
                          (WithNull
                             (WithNull
                                (App
                                   (LongIdent
                                      (SynLongIdent ([list], [], [None])), None,
                                    [LongIdent
                                       (SynLongIdent ([int], [], [None]))], [],
                                    None, true, (1,8--1,16)), false, (1,8--1,23)),
                              false, (1,8--1,30)), false, (1,8--1,37)),
                        (1,8--1,37), [], { ColonRange = Some (1,6--1,7) })),
                  Typed
                    (ArrayOrList (false, [], (1,40--1,42)),
                     WithNull
                       (WithNull
                          (WithNull
                             (App
                                (LongIdent (SynLongIdent ([list], [], [None])),
                                 None,
                                 [LongIdent (SynLongIdent ([int], [], [None]))],
                                 [], None, true, (1,8--1,16)), false,
                              (1,8--1,23)), false, (1,8--1,30)), false,
                        (1,8--1,37)), (1,40--1,42)), (1,4--1,5), Yes (1,0--1,42),
                  { LeadingKeyword = Let (1,0--1,3)
                    InlineKeyword = None
                    EqualsRange = Some (1,38--1,39) })], (1,0--1,42))],
          PreXmlDocEmpty, [], None, (1,0--2,0), { LeadingKeyword = None })],
      (true, true), { ConditionalDirectives = []
                      CodeComments = [] }, set []))
