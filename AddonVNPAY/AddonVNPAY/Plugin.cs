using CXS.Retail.Extensibility;
using System;

namespace AddonVNPAY
{
    class Plugin : BasePlugin // nếu để public class (version 6.6) khi restart MC sẽ bị lỗi
    {
        public override string Name
        {
            get { return "AddonIntegrationVNPAY"; }
        }

        public override string Description
        {
            get { return "Addon Integration VNPAY"; }
        }

        public override string CompanyName
        {
            get { return "FTI Tech"; }
        }

        public override Version VersionInfo
        {
            get { return new Version("1.0.0.0"); }
        }
        public override void Start()
        {
            base.Start();
        }
    }
}
