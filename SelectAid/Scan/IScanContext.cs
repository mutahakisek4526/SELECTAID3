using System.Collections.Generic;

namespace SelectAid.Scan;

public interface IScanContext
{
    IReadOnlyList<IScanTarget> GetScanTargets();
}
