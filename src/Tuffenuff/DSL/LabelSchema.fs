[<RequireQualifiedAccess>]
module Tuffenuff.DSL.LabelSchema

open System


/// <summary>This label contains the Date/Time the image was built.</summary>
/// <param name="value">SHOULD be formatted according to RFC 3339</param>
[<Obsolete("Label Schema Convention is depricated in favor of OCI IMAGE SPEC")>]
let buildDate (value : string) =
    label "org.label-schema.build-date" value

/// <summary>A human friendly name for the image.</summary>
/// <remarks>For example, this could be the name of a microservice in a microservice
/// architecture.</remarks>
[<Obsolete("Label Schema Convention is depricated in favor of OCI IMAGE SPEC")>]
let name (value : string) = label "org.label-schema.name" value

/// <summary>Text description of the image.</summary>
/// <param name="value">May contain up to 300 characters.</param>
[<Obsolete("Label Schema Convention is depricated in favor of OCI IMAGE SPEC")>]
let description (value : string) =
    label "org.label-schema.description" value

/// <summary>Link to a file in the container or alternatively a URL that provides usage
/// instructions.</summary>
/// <param name="value">If a URL is given it SHOULD be specific to this version of the
/// image e.g. http://docs.example.com/v1.2/usage rather than
/// http://docs.example.com/usage</param>
[<Obsolete("Label Schema Convention is depricated in favor of OCI IMAGE SPEC")>]
let usage (value : string) = label "org.label-schema.usage" value

/// <summary>URL of website with more information about the product or service provided
/// by the container.</summary>
[<Obsolete("Label Schema Convention is depricated in favor of OCI IMAGE SPEC")>]
let url (value : string) = label "org.label-schema.url" value

/// <summary>URL for the source code under version control from which this container
/// image was built.</summary>
[<Obsolete("Label Schema Convention is depricated in favor of OCI IMAGE SPEC")>]
let vcsUrl (value : string) = label "org.label-schema.vcs-url" value

/// <summary>Identifier for the version of the source code from which this image was
/// built.</summary>
/// <remarks>For example if the version control system is git this is the SHA.</remarks>
[<Obsolete("Label Schema Convention is depricated in favor of OCI IMAGE SPEC")>]
let vcsRef (value : string) = label "org.label-schema.vcs-ref" value

/// <summary>The organization that produces this image.</summary>
[<Obsolete("Label Schema Convention is depricated in favor of OCI IMAGE SPEC")>]
let vendor (value : string) = label "org.label-schema.vendor" value

/// <summary>Release identifier for the contents of the image. This is entirely up to
/// the user and could be a numeric version number like 1.2.3, or a text label.</summary>
/// <remarks>
/// <para>The version MAY match a label or tag in the source code
/// repository.</para>
/// <para>You should make sure that only images that exactly reflect a version of code
/// shouldhave that version label. If Julia finds a version 0.7.1 in a repository she
/// SHOULD be able to infer this matches version 0.7.1 of the associated code (and in
/// particular, not 0.7.1 plus some later commits).</para>
/// <para>You SHOULD omit the version label, or use a marker like “dirty” or “test” to
/// indicate when a container image does not match a labelled / tagged version of the
/// code.</para>
/// </remarks>
[<Obsolete("Label Schema Convention is depricated in favor of OCI IMAGE SPEC")>]
let version (value : string) = label "org.label-schema.version" value

/// <summary>This label SHOULD be present to indicate the version of Label Schema in
/// use.</summary>
[<Obsolete("Label Schema Convention is depricated in favor of OCI IMAGE SPEC")>]
let schemaVersion (value : string) =
    label "org.label-schema.schema-version" value

/// <summary>How to run a container based on the image under the Docker
/// runtime.</summary>
[<Obsolete("Label Schema Convention is depricated in favor of OCI IMAGE SPEC")>]
let dockerCmd (value : string) =
    label "org.label-schema.docker.cmd" value

/// <summary>How to run the container in development mode under the Docker runtime e.g.
/// with debug tooling or more verbose output.</summary>
[<Obsolete("Label Schema Convention is depricated in favor of OCI IMAGE SPEC")>]
let dockerCmdDevel (value : string) =
    label "org.label-schema.docker.cmd.devel" value

/// <summary>How to run the bundled test-suite for the image under the Docker
/// runtime.</summary>
/// <remarks>MUST execute tests then exit, returning output on stdout and exit with a
/// non-zero exit code on failure.</remarks>
[<Obsolete("Label Schema Convention is depricated in favor of OCI IMAGE SPEC")>]
let dockerCmdTest (value : string) =
    label "org.label-schema.docker.cmd.test" value

/// <summary>How to get an appropriate interactive shell for debugging on the container
/// under Docker.</summary>
[<Obsolete("Label Schema Convention is depricated in favor of OCI IMAGE SPEC")>]
let dockerCmdDebug (value : string) =
    label "org.label-schema.docker.cmd.debug" value

/// <summary>How to output help from the image under the docker runtime.</summary>
/// <remarks>The container MUST print this information to stdout and then exit.</remarks>
[<Obsolete("Label Schema Convention is depricated in favor of OCI IMAGE SPEC")>]
let dockerCmdHelp (value : string) =
    label "org.label-schema.docker.cmd.help" value

/// <summary>Applicable environment variables for the Docker runtime.</summary>
/// <param name="value">Multiple environment
/// variables can be specified by separating with commas.</param>
[<Obsolete("Label Schema Convention is depricated in favor of OCI IMAGE SPEC")>]
let dockerParams (value : string) =
    label "org.label-schema.docker.params" value

/// <summary>How to run a container based on the image under the rkt
/// runtime.</summary>
[<Obsolete("Label Schema Convention is depricated in favor of OCI IMAGE SPEC")>]
let rktCmd (value : string) = label "org.label-schema.rkt.cmd" value

/// <summary>How to run the container in development mode under the rkt runtime e.g.
/// with debug tooling or more verbose output.</summary>
[<Obsolete("Label Schema Convention is depricated in favor of OCI IMAGE SPEC")>]
let rktCmdDevel (value : string) =
    label "org.label-schema.rkt.cmd.devel" value

/// <summary>How to run the bundled test-suite for the image under the rkt
/// runtime.</summary>
/// <remarks>MUST execute tests then exit, returning output on stdout and exit with a
/// non-zero exit code on failure.</remarks>
[<Obsolete("Label Schema Convention is depricated in favor of OCI IMAGE SPEC")>]
let rktCmdTest (value : string) =
    label "org.label-schema.rkt.cmd.test" value

/// <summary>How to get an appropriate interactive shell for debugging on the
/// container under rkt.</summary>
[<Obsolete("Label Schema Convention is depricated in favor of OCI IMAGE SPEC")>]
let rktCmdDebug (value : string) =
    label "org.label-schema.rkt.cmd.debug" value

/// <summary>How to output help from the image under the rkt runtime.</summary>
/// <remarks>The container MUST print
/// this information to stdout and then exit.</remarks>
[<Obsolete("Label Schema Convention is depricated in favor of OCI IMAGE SPEC")>]
let rktCmdHelp (value : string) =
    label "org.label-schema.rkt.cmd.help" value

/// <summary>Applicable environment variables for the rkt runtime.</summary>
/// <param name="value">Multiple environment variables
/// can be specified by separating with commas.</param>
[<Obsolete("Label Schema Convention is depricated in favor of OCI IMAGE SPEC")>]
let rktParams (value : string) =
    label "org.label-schema.rkt.params" value
