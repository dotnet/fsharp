module Test =
        

    type MyBuilder() =
        member x.Zero() : float = 0.0
        member x.Yield(a : float) = a
        member x.Delay(l : unit -> float) = l()
        member x.Combine(l : float, r : float) = l+r 

    let my = MyBuilder()

    let a() =
        my {
            my {
                my {
                    1
                    my {
                    
                        my {
                            3.0
                            my {
                                1.0
                                my {
                                    2.0
                                    my {
                                       1.0
                                       2.0
                                       my {
                                            my {
                                                my {
                                                    my {
                                                        3.0
                                                        my {
                                                            3.0
                                                            my {
                                                                1.0
                                                                my {
                                                                    my {
                                                                       1.0
                                                                    }   
                                                                }
                                                            }   
                                                        }
                                                    }   
                                                }
                                            }   
                                        }
                                    }   
                                }
                            }   
                        }
                    }   
                }
            }   
        }