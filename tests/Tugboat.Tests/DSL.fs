module Tests.DSL

open Expecto
open Dockerfile.Tugboat

[<Tests>]
let tests =
    testList "DSL tests" [
        let errorMessage = "Generated text should be equal to raw dockerfile"

        testCase "hello-world test" <| fun _ ->
            let expected = """FROM scratch
COPY hello /
CMD /hello"""
            let actual = render <| df [
                fresh
                cp [] "hello" "/"
                cmd [ "/hello" ]
            ]

            Expect.equal expected actual errorMessage

        testCase "custom syntax test" <| fun _ ->
            let incl path = 
                plain (sprintf "INCLUDE+ %s" path)

            let expected = """# syntax=edrevo/dockerfile-plus

FROM alpine:latest
INCLUDE+ Dockerfile.common
ENTRYPOINT [ "mybin" ]"""
            let actual = render <| df [
                syntax "edrevo/dockerfile-plus"
                from "alpine:latest" []
                incl "Dockerfile.common"
                entry [| "mybin" |]
            ]

            Expect.equal expected actual errorMessage

        testCase "multi-stage test" <| fun _ ->
            let img = % "IMAGE"
            let externalDf = df [
                from img [ as_ "build" ]
                !> """apt-get install \
        wget \
        somebloatware;"""
                workdir "/etc"
                !@ [ keep ] "https://git.example.com/some/thing.git" "."
                ~~ (run [ "make install" ])
            ]

            let expected = """# multi-stage text
ARG USERNAME="nonroot"
ARG IMAGE="ubuntu:14.04"
FROM ${IMAGE} AS build
RUN apt-get install \
        wget \
        somebloatware;
WORKDIR "/etc"
ADD --keep-git-dir=true https://git.example.com/some/thing.git .
ONBUILD RUN make install

FROM ${IMAGE}
COPY --from=build /etc /app
ENV CONFIG="/app/config"
ARG USERNAME
VOLUME "/app/data"
EXPOSE 5000
LABEL foo="bar"
USER ${USERNAME}
HEALTCHECK --interval=3s \
    CMD hc.bash
ENTRYPOINT [ "/bin/foobar" ]
CMD server"""
            let actual = render <| df [
                !/ "multi-stage text"
                arg "USERNAME" "nonroot"
                arg "IMAGE" "ubuntu:14.04"
                !& externalDf
                from img []
                cp [ from_ "build" ] "/etc" "/app"
                env "CONFIG" "/app/config"
                usearg "USERNAME"
                vol "/app/data"
                exp "5000"
                label "foo" "bar"
                user (% "USERNAME")
                hc [ interval "3s" ] [ "hc.bash" ]
                entry [| "/bin/foobar" |]
                cmd [ "server" ]
            ]

            Expect.equal expected actual errorMessage
    ]
