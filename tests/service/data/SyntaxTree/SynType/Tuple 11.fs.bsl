ImplFile
  (ParsedImplFileInput
     ("/root/SynType/Tuple 11.fs", false, QualifiedNameOfFile Module, [], [],
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
                               Type
                                 (LongIdent (SynLongIdent ([b2], [], [None])));
                               Star (3,20--3,21);
                               Type (FromParseError (3,21--3,21))], (3,12--3,21)),
                           (3,7--3,23), { ArrowRange = (3,9--3,11) }),
                        (3,7--3,23), [], { ColonRange = Some (3,5--3,6) })),
                  Typed
                    (Const (Unit, (3,24--3,26)),
                     Fun
                       (LongIdent (SynLongIdent ([a], [], [None])),
                        Tuple
                          (false,
                           [Type (LongIdent (SynLongIdent ([b1], [], [None])));
                            Star (3,15--3,16);
                            Type (LongIdent (SynLongIdent ([b2], [], [None])));
                            Star (3,20--3,21);
                            Type (FromParseError (3,21--3,21))], (3,12--3,21)),
                        (3,7--3,23), { ArrowRange = (3,9--3,11) }), (3,24--3,26)),
                  (3,4--3,5), Yes (3,0--3,26),
                  { LeadingKeyword = Let (3,0--3,3)
                    InlineKeyword = None
                    EqualsRange = Some (3,22--3,23) })], (3,0--3,26))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--3,26), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))

(3,22)-(3,23) parse error Unexpected symbol '=' in binding
