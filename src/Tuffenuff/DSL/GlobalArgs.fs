module Tuffenuff.DSL.GlobalArgs


/// <summary>Platform of the build result. Eg <c>linux/amd64</c>, <c>linux/arm/v7</c>,
/// <c>windows/amd64</c>.</summary>
let targetPlatform = usearg "TARGETPLATFORM"

/// <summary>OS component of <c>TARGETPLATFORM</c>.</summary>
let targetOS = usearg "TARGETOS"

/// <summary>Architecture component of <c>TARGETPLATFORM</c>.</summary>
let targetArch = usearg "TARGETARCH"

/// <summary>Variant component of <c>TARGETPLATFORM</c>.</summary>
let targetVariant = usearg "TARGETVARIANT"

/// <summary>Platform of the node performing the build.</summary>
let buildPlatform = usearg "BUILDPLATFORM"

/// <summary>OS component of <c>BUILDPLATFORM</c>.</summary>
let buildOS = usearg "BUILDOS"

/// <summary>Architecture component of <c>BUILDPLATFORM</c>.</summary>
let buildArch = usearg "BUILDARCH"

/// <summary>Variant component of <c>BUILDPLATFORM</c>.</summary>
let buildVariant = usearg "BUILDVARIANT"
