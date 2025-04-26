ImplFile
  (ParsedImplFileInput
     ("/root/Nullness/NullAnnotatedExpression.fs", false,
      QualifiedNameOfFile NullAnnotatedExpression, [], [],
      [SynModuleOrNamespace
         ([NullAnnotatedExpression], false, AnonModule,
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
                             (LongIdent
                                (SynLongIdent ([Expression], [], [None])),
                              Some (1,18--1,19),
                              [WithNull
                                 (App
                                    (LongIdent
                                       (SynLongIdent ([Func], [], [None])),
                                     Some (1,23--1,24),
                                     [WithNull
                                        (LongIdent
                                           (SynLongIdent ([string], [], [None])),
                                         false, (1,24--1,37),
                                         { BarRange = (1,31--1,32) });
                                      WithNull
                                        (LongIdent
                                           (SynLongIdent ([T], [], [None])),
                                         false, (1,39--1,47),
                                         { BarRange = (1,41--1,42) })],
                                     [(1,37--1,38)], Some (1,47--1,48), false,
                                     (1,19--1,48)), false, (1,19--1,55),
                                  { BarRange = (1,49--1,50) })], [],
                              Some (1,55--1,56), false, (1,8--1,56)), false,
                           (1,8--1,63), { BarRange = (1,57--1,58) }),
                        (1,8--1,63), [], { ColonRange = Some (1,6--1,7) })),
                  Typed
                    (Null (1,66--1,70),
                     WithNull
                       (App
                          (LongIdent (SynLongIdent ([Expression], [], [None])),
                           Some (1,18--1,19),
                           [WithNull
                              (App
                                 (LongIdent (SynLongIdent ([Func], [], [None])),
                                  Some (1,23--1,24),
                                  [WithNull
                                     (LongIdent
                                        (SynLongIdent ([string], [], [None])),
                                      false, (1,24--1,37),
                                      { BarRange = (1,31--1,32) });
                                   WithNull
                                     (LongIdent (SynLongIdent ([T], [], [None])),
                                      false, (1,39--1,47),
                                      { BarRange = (1,41--1,42) })],
                                  [(1,37--1,38)], Some (1,47--1,48), false,
                                  (1,19--1,48)), false, (1,19--1,55),
                               { BarRange = (1,49--1,50) })], [],
                           Some (1,55--1,56), false, (1,8--1,56)), false,
                        (1,8--1,63), { BarRange = (1,57--1,58) }), (1,66--1,70)),
                  (1,4--1,5), Yes (1,0--1,70),
                  { LeadingKeyword = Let (1,0--1,3)
                    InlineKeyword = None
                    EqualsRange = Some (1,64--1,65) })], (1,0--1,70))],
          PreXmlDocEmpty, [], None, (1,0--2,0), { LeadingKeyword = None })],
      (true, true), { ConditionalDirectives = []
                      CodeComments = [] }, set []))
