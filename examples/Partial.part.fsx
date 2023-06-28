#r @"../src/Tuffenuff/bin/Release/netstandard2.0/Tuffenuff.dll"

open Tuffenuff
open Tuffenuff.Domain.ImageCE

let echoMaessage () =
    stage {
        cmt "this is from 'Partial.part.fsx'"
        RUN ["""echo 'echo "Shalom!"' > /etc/profile.d/welcome.sh"""]
    }
