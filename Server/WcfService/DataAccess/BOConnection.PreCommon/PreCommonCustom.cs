using LSOmni.Common.Util;
using LSOmni.DataAccess.BOConnection.PreCommon.SaveTermsAcceptance;
using System;

namespace LSOmni.DataAccess.BOConnection.PreCommon
{
    public partial class PreCommonBase
    {
        public virtual string MyCustomFunction(string data)
        {
            // TODO: Here you put the code to access BC or NAV WS
            // Data Mapping is done under Mapping folder
            return "My return data + Incoming data: " + data;
        }

        #region terms

        public bool AcceptTerms(string accountId, string deviceId, string termsAndConditionsVersion, string privacyPolicyVersion, Statistics stat)
        {
            logger.StatisticStartSub(true, ref stat, out int index);

            string respCode = string.Empty;
            string errorText = string.Empty;

            SaveTermsAcceptance.CallSaveTermsAcceptance(ref respCode, ref errorText, new RootSaveTermsAcceptance()
            {
                SaveTermsAcceptance = new[] { new SaveTermsAcceptance.SaveTermsAcceptance1() { accountId = accountId, deviceId = deviceId, privacyPolicyVersion = privacyPolicyVersion, termsAndConditionsVersion = termsAndConditionsVersion } },
            });

            HandleWS2ResponseCode("SPGProfileGet", respCode, errorText, ref stat, index);

            logger.StatisticEndSub(ref stat, index);

            return true;
        }

        #endregion
    }
}
