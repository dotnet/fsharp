ImplFile
  (ParsedImplFileInput
     ("/root/Type/SynTypeTupleWithStructRecovery.fs", false,
      QualifiedNameOfFile SynTypeTupleWithStructRecovery, [], [],
      [SynModuleOrNamespace
         ([SynTypeTupleWithStructRecovery], false, AnonModule,
          [Let
             (false,
              [SynBinding
                 (None, Normal, false, false, [],
                  PreXmlDoc ((2,0), FSharp.Compiler.Xml.XmlDocCollector),
                  SynValData
                    (None, SynValInfo ([], SynArgInfo ([], false, None)), None),
                  Wild (2,4--2,5),
                  Some
                    (SynBindingReturnInfo
                       (Tuple
                          (true,
                           [Type (LongIdent (SynLongIdent ([int], [], [None])));
                            Star (2,19--2,20);
                            Type (LongIdent (SynLongIdent ([int], [], [None])))],
                           (2,7--2,24)), (2,7--2,26), [],
                        { ColonRange = Some (2,5--2,6) })),
                  Typed
                    (Const (Unit, (2,27--2,29)),
                     Tuple
                       (true,
                        [Type (LongIdent (SynLongIdent ([int], [], [None])));
                         Star (2,19--2,20);
                         Type (LongIdent (SynLongIdent ([int], [], [None])))],
                        (2,7--2,24)), (2,27--2,29)), (2,4--2,5), Yes (2,0--2,29),
                  { LeadingKeyword = Let (2,0--2,3)
                    InlineKeyword = None
                    EqualsRange = Some (2,25--2,26) })], (2,0--2,29))],
          PreXmlDocEmpty, [], None, (2,0--3,0), { LeadingKeyword = None })],
      (true, false), { ConditionalDirectives = []
                       CodeComments = [] }, set []))
