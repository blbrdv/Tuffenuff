#r "nuget: Tuffenuff"

open Tuffenuff
open Tuffenuff.DSL

let echoMaessage () =
    df [
        !/"this is from 'Partial.part.fsx'"
        !>"""echo 'echo "Shalom!"' > /etc/profile.d/welcome.sh"""
    ]
