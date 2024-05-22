using RoR2;
using System;
using System.Collections.Generic;
using System.Text;

namespace LookingGlass.LookingGlassLanguage
{
    public static class LookingGlassLanguageAPI //calling this an API is really stretching it...
    {
        public static void SetupToken(Language language, string token, string value)
        {
            //Log.Debug($"{token}   {value}");
            language.stringsByToken.Add("LG_TOKEN_" + token, value);
        }
    }
}
