ImplFile
  (ParsedImplFileInput
     ("/root/SynType/Tuple 14.fs", false, QualifiedNameOfFile Module, [], [],
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
                          (Tuple
                             (false,
                              [Type
                                 (LongIdent (SynLongIdent ([a1], [], [None])));
                               Star (3,10--3,11);
                               Type (FromParseError (3,13--3,13));
                               Star (3,12--3,13);
                               Type
                                 (LongIdent (SynLongIdent ([a3], [], [None])))],
                              (3,7--3,16)),
                           LongIdent (SynLongIdent ([b], [], [None])),
                           (3,7--3,21), { ArrowRange = (3,17--3,19) }),
                        (3,7--3,21), [], { ColonRange = Some (3,5--3,6) })),
                  Typed
                    (Const (Unit, (3,24--3,26)),
                     Fun
                       (Tuple
                          (false,
                           [Type (LongIdent (SynLongIdent ([a1], [], [None])));
                            Star (3,10--3,11);
                            Type (FromParseError (3,13--3,13));
                            Star (3,12--3,13);
                            Type (LongIdent (SynLongIdent ([a3], [], [None])))],
                           (3,7--3,16)),
                        LongIdent (SynLongIdent ([b], [], [None])), (3,7--3,21),
                        { ArrowRange = (3,17--3,19) }), (3,24--3,26)),
                  (3,4--3,5), Yes (3,0--3,26),
                  { LeadingKeyword = Let (3,0--3,3)
                    InlineKeyword = None
                    EqualsRange = Some (3,22--3,23) })], (3,0--3,26))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--3,26), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))

(3,12)-(3,13) parse error Expecting type
