module CompilerService.TaskAgent
open Internal.Utilities.TaskAgent

open Xunit
open System
open System.Threading.Tasks
open System.Runtime.CompilerServices
open System.Threading


[<Fact>]
let ``TaskAgent should process messages that expect a reply`` () =
    // Arrange
    let agent = new TaskAgent<_,_,_>(
        processMessage = (fun _ msg -> msg + 1),
        processMessageNoReply = (fun _ _ -> ()))

    // Act
    let replyTask = agent.PostAndAwaitReply(1)

    // Assert
    Assert.Equal(2, replyTask.Result)

[<Fact>]
let ``TaskAgent should process messages that do not expect a reply`` () =
    // Arrange
    let mutable messageProcessed = false

    let agent = new TaskAgent<_,_,_>(
        processMessage = (fun _ msg -> msg + 1),
        processMessageNoReply = (fun _ _ -> messageProcessed <- true))

    // Act
    agent.Post(1)
    agent.PostAndAwaitReply(1).Wait()

    // Assert
    Assert.True(messageProcessed)

[<Fact>]
let ``TaskAgent should publish exceptions that occur while processing messages that do not expect a reply`` () =
    // Arrange
    let mutable exceptionPublished = false

    let agent = new TaskAgent<_,_,_>(
        processMessage = (fun _ msg -> msg + 1),
        processMessageNoReply = (fun _ _ -> failwith "Test exception"))

    use _ = agent.NoReplyExceptions.Subscribe(fun _ -> exceptionPublished <- true)

    // Act
    agent.Post(1)
    agent.PostAndAwaitReply(1).Wait()

    // Assert
    Assert.True(exceptionPublished)

[<Fact>]
let ``TaskAgent should not deadlock under heavy use`` () =
    // Arrange
    use messagesProcessed = new ThreadLocal<_>((fun () -> ResizeArray<int>()), true)
    let agent = new TaskAgent<_,_,_>(
        processMessage = (fun post msg ->
            if msg > 0 then
                post (msg + 1)
            msg + 1),
        processMessageNoReply = (fun _ msg ->
            messagesProcessed.Value.Add(msg)))
    let numMessages = 100000

    // Act
    let replyTasks =
        [ for i in 1..numMessages do
            yield agent.PostAndAwaitReply(i) ]
    let replies = (replyTasks |> Task.WhenAll).Result

    // Assert
    Assert.Equal(numMessages, replies.Length)
    Assert.True(replies |> Seq.forall (fun r -> r > 0))
    agent.PostAndAwaitReply(0).Wait()
    Assert.Equal(numMessages, messagesProcessed.Values |> Seq.sumBy (fun x -> x.Count))
