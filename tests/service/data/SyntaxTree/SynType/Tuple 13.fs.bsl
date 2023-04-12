ImplFile
  (ParsedImplFileInput
     ("/root/SynType/Tuple 13.fs", false, QualifiedNameOfFile Module, [], [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Let
             (false,
              [SynBinding
                 (None, Normal, false, false, [],
                  PreXmlDoc ((3,0), FSharp.Compiler.Xml.XmlDocCollector),
                  SynValData
                    (None, SynValInfo ([], SynArgInfo ([], false, None)), None),
                  Wild (3,4--3,5),
                  Some
                    (SynBindingReturnInfo
                       (Fun
                          (LongIdent (SynLongIdent ([a], [], [None])),
                           Tuple
                             (false,
                              [Type
                                 (LongIdent (SynLongIdent ([b1], [], [None])));
                               Star (3,15--3,16);
                               Type (FromParseError (3,18--3,18));
                               Star (3,17--3,18);
                               Type
                                 (LongIdent (SynLongIdent ([b3], [], [None])))],
                              (3,12--3,21)), (3,7--3,21),
                           { ArrowRange = (3,9--3,11) }), (3,7--3,21), [],
                        { ColonRange = Some (3,5--3,6) })),
                  Typed
                    (Const (Unit, (3,24--3,26)),
                     Fun
                       (LongIdent (SynLongIdent ([a], [], [None])),
                        Tuple
                          (false,
                           [Type (LongIdent (SynLongIdent ([b1], [], [None])));
                            Star (3,15--3,16);
                            Type (FromParseError (3,18--3,18));
                            Star (3,17--3,18);
                            Type (LongIdent (SynLongIdent ([b3], [], [None])))],
                           (3,12--3,21)), (3,7--3,21),
                        { ArrowRange = (3,9--3,11) }), (3,24--3,26)), (3,4--3,5),
                  Yes (3,0--3,26), { LeadingKeyword = Let (3,0--3,3)
                                     InlineKeyword = None
                                     EqualsRange = Some (3,22--3,23) })],
              (3,0--3,26))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--3,26), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))

(3,17)-(3,18) parse error Expecting type
