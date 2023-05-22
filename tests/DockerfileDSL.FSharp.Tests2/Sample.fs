module Tests

open Expecto
open DockerfileDSL.FSharp

[<Tests>]
let tests =
  testList "samples" [
    testCase "Sometimes I want to ༼ノಠل͟ಠ༽ノ ︵ ┻━┻" <| fun _ ->
      let actual = render <| df [
          fresh
          cp [] "hello" "/"
          cmd [ "/hello" ]
      ]
      let expected = """FROM scratch
COPY hello /
CMD /hello"""
      Expect.equal actual expected "These should equal"
  ]
