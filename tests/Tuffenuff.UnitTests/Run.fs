module Tests.Run

open Expecto
open Tuffenuff.Domain.Types
open Tuffenuff.DSL

[<Tests>]
let tests =
    testList "RUN instruction tests" [

        testCase "run command"
        <| fun _ ->
            let expected = [ "RUN \\" ; "    apt-get dist-upgrade -y" ] |> toMultiline
            let actual = run { cmd "apt-get dist-upgrade -y" } |> render

            Expect.equal actual expected
            <| toErrorMessage [
                "String must contain"
                "  1. 'RUN' keyword"
                "  2. whitespace"
                "  3. backslash symbol"
                "  4. EOL"
                "  5. 4 whitespaces"
                "  6. command line from argument of 'cmd' parameter"
                "  7. EOL"
            ]

        testCase "run short syntax command"
        <| fun _ ->
            let expected = [ "RUN \\" ; "    apt-get dist-upgrade -y" ] |> toMultiline
            let actual = !> "apt-get dist-upgrade -y" |> render

            Expect.equal actual expected
            <| toErrorMessage [
                "String must contain"
                "  1. 'RUN' keyword"
                "  2. whitespace"
                "  3. backslash symbol"
                "  4. EOL"
                "  5. 4 whitespaces"
                "  6. command line from argument of 'cmd' parameter"
                "  7. EOL"
            ]

        testCase "run multiple commands"
        <| fun _ ->
            let expected =
                [ "RUN \\" ; "    echo \"command 1\" \\" ; "    exit 0" ] |> toMultiline

            let actual =
                run {
                    cmd "echo \"command 1\""
                    cmd "exit 0"
                }
                |> render

            Expect.equal actual expected
            <| toErrorMessage [
                "String must contain"
                "  1. 'RUN' keyword"
                "  2. whitespace"
                "  3. backslash symbol"
                "  4. EOL"
                "  5. 4 whitespaces"
                "  6. command line from argument of first 'cmd' parameter"
                "  7. whitespace"
                "  8. backslash symbol"
                "  9. EOL"
                "  10. 4 whitespaces"
                "  11. command line from argument of second 'cmd' parameter"
                "  12. EOL"
            ]

        testCase "run multiple commands from seq"
        <| fun _ ->
            let expected =
                [ "RUN \\" ; "    echo \"command 1\" \\" ; "    exit 0" ] |> toMultiline

            let actual = run { cmds [ "echo \"command 1\"" ; "exit 0" ] } |> render

            Expect.equal actual expected
            <| toErrorMessage [
                "String must contain"
                "  1. 'RUN' keyword"
                "  2. whitespace"
                "  3. backslash symbol"
                "  4. EOL"
                "  5. 4 whitespaces"
                "  6. command line from argument of first 'cmd' parameter"
                "  7. whitespace"
                "  8. backslash symbol"
                "  9. EOL"
                "  10. 4 whitespaces"
                "  11. command line from argument of second 'cmd' parameter"
                "  12. EOL"
            ]

        testCase "run command with set networking environment"
        <| fun _ ->
            let expected =
                [
                    "RUN \\"
                    "    --network=host \\"
                    "    echo \"command 1\""
                ]
                |> toMultiline

            let actual =
                run {
                    cmd "echo \"command 1\""
                    network Host
                }
                |> render

            Expect.equal actual expected
            <| toErrorMessage [
                "String must contain"
                "  1. 'RUN' keyword"
                "  2. whitespace"
                "  3. backslash symbol"
                "  4. EOL"
                "  5. 4 whitespaces"
                "  6. '--network' flag"
                "  7. equal symbol"
                "  8. network env type from argument of 'network' parameter"
                "  9. whitespace"
                "  10. backslash symbol"
                "  11. EOL"
                "  12. 4 whitespaces"
                "  13. command line from argument of second 'cmd' parameter"
                "  14. EOL"
            ]

        testCase "run command with set security mode"
        <| fun _ ->
            let expected =
                [
                    "RUN \\"
                    "    --security=insecure \\"
                    "    echo \"command 1\""
                ]
                |> toMultiline

            let actual =
                run {
                    cmd "echo \"command 1\""
                    security Insecure
                }
                |> render

            Expect.equal actual expected
            <| toErrorMessage [
                "String must contain"
                "  1. 'RUN' keyword"
                "  2. whitespace"
                "  3. backslash symbol"
                "  4. EOL"
                "  5. 4 whitespaces"
                "  6. '--security' flag"
                "  7. equal symbol"
                "  8. security mode from argument of 'security' parameter"
                "  9. whitespace"
                "  10. backslash symbol"
                "  11. EOL"
                "  12. 4 whitespaces"
                "  13. command line from argument of second 'cmd' parameter"
                "  14. EOL"
            ]

        testCase "run command with ssh mount mode"
        <| fun _ ->
            let expected =
                [
                    "RUN \\"
                    "    --mount=type=ssh \\"
                    "    ssh -q -T git@gitlab.com 2>&1 | tee /hello"
                ]
                |> toMultiline

            let actual =
                run {
                    ssh
                    cmd "ssh -q -T git@gitlab.com 2>&1 | tee /hello"
                }
                |> render

            Expect.equal actual expected "huh"
    ]
