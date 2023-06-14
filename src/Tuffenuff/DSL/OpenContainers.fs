[<RequireQualifiedAccess>]
module Tuffenuff.DSL.OpenContainers


/// <summary>Date and time on which the image was built</summary>
/// <param name="value">SHOULD be formatted according to <c>RFC 3339</c></param>
let created (value : string) =
    label "org.opencontainers.image.created" value

/// <summary>Contact details of the people or organization responsible for the
/// image.</summary>
let authors (value : string) =
    label "org.opencontainers.image.authors" value

/// <summary>URL to find more information on the image.</summary>
let url (value : string) =
    label "org.opencontainers.image.url" value

/// <summary>URL to get documentation on the image.</summary>
let documentation (value : string) =
    label "org.opencontainers.image.documentation" value

/// <summary>URL to get source code for building the image.</summary>
let source (value : string) =
    label "org.opencontainers.image.source" value

/// <summary>Version of the packaged software.</remarks>
/// <remarks>The version MAY match a label or tag in the source code repository version
/// MAY be Semantic versioning-compatible </remarks>
let version (value : string) =
    label "org.opencontainers.image.version" value

/// <summary>Source control revision identifier for the packaged software.</summary>
let revision (value : string) =
    label "org.opencontainers.image.revision" value

/// <summary>Name of the distributing entity, organization or individual.</summary>
let vendor (value : string) =
    label "org.opencontainers.image.vendor" value

/// <summary>License(s) under which contained software is distributed.</summary>
/// <param name="value">SPDX License Expression</param>
let licenses (value : string) =
    label "org.opencontainers.image.licenses" value

/// <summary>Name of the reference for a target.</summary>
/// <remarks>
/// <para>SHOULD only be considered valid when on descriptors on <c>index.json</c>
///  within image layout.</para>
/// <para>Character set of the value SHOULD conform to alphanum of <c>A-Za-z0-9</c> and
/// separator set of <c>-._:@/+</c></para>
/// <para>The reference must match the following grammar:
/// <code>
///    ref       ::= component ("/" component)*
///    component ::= alphanum (separator alphanum)*
///    alphanum  ::= [A-Za-z0-9]+
///    separator ::= [-._:@+] | "--"
/// </code>
/// </para>
/// </remarks>
let refName (value : string) =
    label "org.opencontainers.image.ref.name" value

/// <summary>Human-readable title of the image.</summary>
let title (value : string) =
    label "org.opencontainers.image.title" value

/// <summary>Human-readable description of the software packaged in the image.</summary>
let description (value : string) =
    label "rg.opencontainers.image.description" value

/// <summary> Digest of the image this image is based on.</summary>
/// <remarks>
/// <para>This SHOULD be the immediate image sharing zero-indexed layers with the
/// image, such as from a Dockerfile <c>FROM</c> statement.</para>
/// <para>This SHOULD NOT reference any other images used to generate the contents of
/// the image (e.g., multi-stage Dockerfile builds).</para>
/// </remarks>
let baseDigest (value : string) =
    label "org.opencontainers.image.base.digest" value

/// <summary>Image reference of the image this image is based on.</summary>
/// <remarks>
/// <para>This SHOULD be image references in the format defined by
/// distribution/distribution.</para>
/// <para>This SHOULD be a fully qualified reference name, without any assumed default
/// registry. (e.g., <c>registry.example.com/my-org/my-image:tag</c> instead of
/// <c>my-org/my-image:tag</c>).</para>
/// <para>This SHOULD be the immediate image sharing zero-indexed layers with the image,
/// such as from a Dockerfile <c>FROM</c> statement.</para>
/// <para>This SHOULD NOT reference any other images used to generate the contents of
/// the image (e.g., multi-stage Dockerfile builds).</para>
/// <para>If the <c>image.base.name</c> annotation is specified, the
/// <c>image.base.digest</c> annotation SHOULD be the digest of the manifest referenced
/// by the <c>image.ref.name</c> annotation.</para>
/// </remarks>
let baseName (value : string) =
    label "org.opencontainers.image.base.name" value
