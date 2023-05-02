using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using DataAccess.BOConnection.Bonus.SaveTermsAcceptance;
using LSOmni.Common.Util;
using LSOmni.DataAccess.BOConnection.PreCommon;
using LSRetail.Omni.Domain.DataModel.Base;

namespace DataAccess.BOConnection.Bonus.Terms
{
    public class SpgAdditions : PreCommonBase
    {
        protected static LSLogger logger = new LSLogger();

        public SpgAdditions(BOConfiguration configuration, bool ping = false) : base(configuration, ping)
        {
            var saveTermsWs = new SaveTermsAcceptance.SaveTermsAcceptance_PortClient();
            
            saveTermsWs.Url = url.Replace("RetailWebServices", "OmniWrapper");
            saveTermsWs.Timeout = config.SettingsIntGetByKey(ConfigKey.BOTimeout) * 1000;  //millisecs,  60 seconds
            saveTermsWs.PreAuthenticate = true;
            saveTermsWs.AllowAutoRedirect = true;
        }

        
    }
}
