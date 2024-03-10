ImplFile
  (ParsedImplFileInput
     ("/root/SynType/Tuple 10.fs", false, QualifiedNameOfFile Module, [], [],
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
                              [Type (LongIdent (SynLongIdent ([a], [], [None])));
                               Star (3,9--3,10);
                               Type (FromParseError (3,10--3,10))], (3,7--3,10)),
                           LongIdent (SynLongIdent ([b], [], [None])),
                           (3,7--3,15), { ArrowRange = (3,11--3,13) }),
                        (3,7--3,15), [], { ColonRange = Some (3,5--3,6) })),
                  Typed
                    (Const (Unit, (3,18--3,20)),
                     Fun
                       (Tuple
                          (false,
                           [Type (LongIdent (SynLongIdent ([a], [], [None])));
                            Star (3,9--3,10); Type (FromParseError (3,10--3,10))],
                           (3,7--3,10)),
                        LongIdent (SynLongIdent ([b], [], [None])), (3,7--3,15),
                        { ArrowRange = (3,11--3,13) }), (3,18--3,20)),
                  (3,4--3,5), Yes (3,0--3,20),
                  { LeadingKeyword = Let (3,0--3,3)
                    InlineKeyword = None
                    EqualsRange = Some (3,16--3,17) })], (3,0--3,20))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--3,20), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))

(3,11)-(3,13) parse error Unexpected symbol '->' in binding
