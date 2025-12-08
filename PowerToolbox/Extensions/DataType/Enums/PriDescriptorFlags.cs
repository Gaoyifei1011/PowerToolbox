using System;

namespace PowerToolbox.Extensions.DataType.Enums
{
    [Flags]
    public enum PriDescriptorFlags : ushort
    {
        AutoMerge = 1,
        IsDeploymentMergeable = 2,
        IsDeploymentMergeResult = 4,
        IsAutoMergeResult = 8
    }
}
