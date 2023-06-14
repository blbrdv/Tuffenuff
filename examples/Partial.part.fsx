#r @"../src/Tuffenuff/bin/Release/netstandard2.0/Tuffenuff.dll"

open Tuffenuff
open Tuffenuff.DSL

let echoMaessage () =
    dockerfile
        [
            !/ "this is from 'Partial.part.fsx'"
            !> """echo 'echo "Shalom!"' > /etc/profile.d/welcome.sh"""
        ]
