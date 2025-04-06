module Tests.Run

open Expecto
open Tuffenuff.DSL
open Tuffenuff.Domain.Types
open Tuffenuff.Domain.Collections

[<Tests>]
let tests =
    testList "RUN instruction tests" [
        let errorMessage = "Records must be equals"


        testCase "short syntax test"
        <| fun _ ->
            let expected =
                {
                    Mounts = Collection.empty
                    Network = None
                    Security = None
                    Arguments = Arguments [ "exit 0" ]
                }
                |> Run
                |> Instruction

            let actual = !> "exit 0"

            Expect.equal actual expected errorMessage


        testCase "multiple commands test"
        <| fun _ ->
            let expected =
                {
                    Mounts = Collection.empty
                    Network = None
                    Security = None
                    Arguments = Arguments [ "make test" ; "exit 0" ]
                }
                |> Run
                |> Instruction

            let actual =
                run {
                    cmd "make test"
                    cmd "exit 0"
                }

            Expect.equal actual expected errorMessage


        testCase "seq commands test"
        <| fun _ ->
            let expected =
                {
                    Mounts = Collection.empty
                    Network = None
                    Security = None
                    Arguments = Arguments [ "apt-get install wget" ; "exit 0" ]
                }
                |> Run
                |> Instruction

            let actual = run { cmds [ "apt-get install wget" ; "exit 0" ] }

            Expect.equal actual expected errorMessage


        testCase "bind default mount test"
        <| fun _ ->
            let expected =
                {
                    Name = Bind
                    Params = Dict [ "target", "/" ]
                }

            let actual = bind "/"

            Expect.equal actual expected errorMessage


        testCase "bind full mount test"
        <| fun _ ->
            let expected =
                {
                    Name = Bind
                    Params =
                        Dict [
                            "target", "/"
                            "source", "/"
                            "from", "/etc"
                            "rw", "true"
                        ]
                }

            let actual =
                bindParams "/" {
                    source "/"
                    from "/etc"
                    rw true
                }

            Expect.equal actual expected errorMessage


        testCase "cache default mount test"
        <| fun _ ->
            let expected =
                {
                    Name = Cache
                    Params = Dict [ "target", "/" ]
                }

            let actual = cache "/"

            Expect.equal actual expected errorMessage


        testCase "cache full mount test"
        <| fun _ ->
            let expected =
                {
                    Name = Cache
                    Params =
                        Dict [
                            "target", "/"
                            "id", "app"
                            "ro", "true"
                            "sharing", "private"
                            "source", "/"
                            "from", "/etc/.cache"
                            "UID", "1"
                            "mode", "0755"
                            "GID", "2"
                        ]
                }

            let actual =
                cacheParams "/" {
                    source "/"
                    id "app"
                    ro true
                    sharing Private
                    source "/"
                    from "/etc/.cache"
                    mode "0755"
                    UID 1
                    GID 2
                }

            Expect.equal actual expected errorMessage


        testCase "tmpfs default mount test"
        <| fun _ ->
            let expected =
                {
                    Name = Tmpfs
                    Params = Dict [ "target", "/" ]
                }

            let actual = tmpfs "/"

            Expect.equal actual expected errorMessage


        testCase "tmpfs full mount test"
        <| fun _ ->
            let expected =
                {
                    Name = Tmpfs
                    Params = Dict [ "target", "/" ; "size", "1" ]
                }

            let actual = tmpfsParams "/" { size 1 }

            Expect.equal actual expected errorMessage


        testCase "secret mount test"
        <| fun _ ->
            let expected =
                {
                    Name = Secret
                    Params =
                        Dict [
                            "target", "/"
                            "id", "app"
                            "required", "true"
                            "mode", "0400"
                            "UID", "1"
                            "GID", "2"
                        ]
                }

            let actual =
                secret {
                    id "app"
                    target "/"
                    required true
                    mode "0400"
                    UID 1
                    GID 2
                }

            Expect.equal actual expected errorMessage


        testCase "ssh mount test"
        <| fun _ ->
            let expected =
                {
                    Name = Ssh
                    Params =
                        Dict [
                            "target", "/"
                            "id", "app"
                            "required", "true"
                            "mode", "0600"
                            "UID", "1"
                            "GID", "2"
                        ]
                }

            let actual =
                ssh {
                    id "app"
                    target "/"
                    required true
                    mode "0600"
                    UID 1
                    GID 2
                }

            Expect.equal actual expected errorMessage


        testCase "network test"
        <| fun _ ->
            let expected =
                {
                    Mounts = Collection.empty
                    Network = Some DefaultNetwork
                    Security = None
                    Arguments = Arguments [ "exit 0" ]
                }
                |> Run
                |> Instruction

            let actual =
                run {
                    network DefaultNetwork
                    cmd "exit 0"
                }

            Expect.equal actual expected errorMessage


        testCase "security test"
        <| fun _ ->
            let expected =
                {
                    Mounts = Collection.empty
                    Network = None
                    Security = Some Sandbox
                    Arguments = Arguments [ "exit 0" ]
                }
                |> Run
                |> Instruction

            let actual =
                run {
                    security Sandbox
                    cmd "exit 0"
                }

            Expect.equal actual expected errorMessage


        testCase "averything at once test"
        <| fun _ ->
            let expected =
                {
                    Mounts =
                        Collection [
                            {
                                Name = Cache
                                Params =
                                    Dict [
                                        "target", "/var/cache/apt"
                                        "sharing", "locked"
                                    ]
                            }
                            {
                                Name = Cache
                                Params =
                                    Dict [
                                        "target", "/var/lib/apt"
                                        "sharing", "locked"
                                    ]
                            }
                        ]
                    Network = Some NoneNetwork
                    Security = Some Insecure
                    Arguments =
                        Arguments [ "make tests" ; "apt-get install wget" ; "exit 0" ]
                }
                |> Run
                |> Instruction

            let actual =
                run {
                    mount (cacheParams "/var/cache/apt" { sharing Locked })
                    mount (cacheParams "/var/lib/apt" { sharing Locked })
                    network NoneNetwork
                    security Insecure
                    cmd "make tests"
                    cmds [ "apt-get install wget" ; "exit 0" ]
                }

            Expect.equal actual expected errorMessage
    ]
