using System;
using DSharpPlus.Entities;

namespace DogeHelper
{
    public static class Globals
    {
        private static string botToken;
        public static readonly string botPrefix = ">";

        public static string BotToken { get => botToken; set => botToken = value; }

        /* Methods */

        public static string[] Device(string[] device)
        {
            string _device = string.Concat(device).ToLower();

            switch (_device)
            {
                case "3axl":
                case "pixel3axl":
                case "bonito":
                    return new string[] { "bonito", "Pixel 3a XL" };

                case "3a":
                case "pixel3a":
                case "sargo":
                    return new string[] { "sargo", "Pixel 3a" };

                case "3xl":
                case "pixel3xl":
                case "crosshatch":
                    return new string[] { "crosshatch", "Pixel 3 XL" };

                case "3":
                case "pixel3":
                case "blueline":
                    return new string[] { "blueline", "Pixel 3" };

                case "2xl":
                case "pixel2xl":
                case "taimen":
                    return new string[] { "taimen", "Pixel 2 XL" };

                case "2":
                case "pixel2":
                case "walleye":
                    return new string[] { "walleye", "Pixel 2" };

                case "xl":
                case "pixelxl":
                case "marlin":
                    return new string[] { "marlin", "Pixel XL" };

                case "pixel":
                case "sailfish":
                    return new string[] { "sailfish", "Pixel" };

                case "c":
                case "pixelc":
                case "ryu":
                    return new string[] { "ryu", "Pixel C" };

                case "6p":
                case "nexus6p":
                case "angler":
                    return new string[] { "angler", "Nexus 6P" };

                case "5x":
                case "nexus5x":
                case "bullhead":
                    return new string[] { "bullhead", "Nexus 5X" };
            }
            
            // Return empty string array.
            return new String[] { "", "" };
        }

        /* Footers */
        public static class Footers
        {
            public static DiscordEmbedBuilder.EmbedFooter DefaultFooter()
            {
                return new DiscordEmbedBuilder.EmbedFooter
                {
                    Text = Strings.DefaultFooter
                };
            }

            public static DiscordEmbedBuilder.EmbedFooter HelpFooter()
            {
                return new DiscordEmbedBuilder.EmbedFooter
                {
                    Text = Strings.HelpFooter
                };
            }
        }

        /* Strings */
        public static class Strings
        {
            public static readonly string DefaultFooter = "DroidLinkr - Links for Android";
            public static readonly string HelpFooter = "help | DroidLinkr - Links for Android.";
        }

        /* Links */
        public static class Links
        {
            public static readonly string GoogleFactoryImages = "https://developers.google.com/android/images#";
            public static readonly string GoogleFullOtaImages = "https://developers.google.com/android/ota#";

            public static readonly string SdkPlatformTools = "https://developer.android.com/studio/releases/platform-tools";
            public static readonly string GoogleUsbDrivers = "https://developer.android.com/studio/run/win-usb";

            public static readonly string OsmodsBasketBuild = "https://basketbuild.com/devs/osm0sis/osmods";
            public static readonly string OsmodsTwrpAngler = "https://basketbuild.com/filedl/devs?dev=osm0sis&dl=osm0sis/osmods/twrp-3.2.1-0-fbe-4core-angler.img";
            public static readonly string OsmodsTwrpBullhead = "https://basketbuild.com/filedl/devs?dev=osm0sis&dl=osm0sis/osmods/twrp-3.2.1-0-fbe-4core-bullhead.img";
            public static readonly string OsmodsBlodWorkaround = "https://basketbuild.com/filedl/devs?dev=osm0sis&dl=osm0sis/osmods/N5X-6P_BLOD_Workaround_Injector_Addon-AK2-signed.zip";

            public static readonly string MagiskXdaThread = "https://forum.xda-developers.com/apps/magisk/official-magisk-v7-universal-systemless-t3473445";
            public static readonly string MagiskCanaryXdaThread = "https://forum.xda-developers.com/apps/magisk/dev-magisk-canary-channel-bleeding-edge-t3839337";

            // Magisk Update URLs
            public readonly static string MagiskStableJson = "https://raw.githubusercontent.com/topjohnwu/magisk_files/master/stable.json";
            public readonly static string MagiskBetaJson = "https://raw.githubusercontent.com/topjohnwu/magisk_files/master/beta.json";
            public static readonly string MagiskCanaryJson = "https://raw.githubusercontent.com/topjohnwu/magisk_files/master/canary_builds/canary.json";
        }

        public static class Authors
        {
            public static class Osm0sis
            {
                public static readonly string Img = "https://cdn-cf-1.xda-developers.com/customavatars/thumbs/avatar4544860_6.gif";
                public static readonly string XdaProfile = "https://forum.xda-developers.com/member.php?u=4544860";
            }

            public static class Google
            {
                public static readonly string GoogleImg = "https://www.google.com/s2/favicons?domain=www.google.com";
            }

            public static class Topjohnwu
            {
                public static readonly string MagiskImg = "https://www.didgeridoohan.com/magisk/images/magisk/logo192x192.png";
                public static readonly string XdaProfile = "https://forum.xda-developers.com/member.php?u=4470081";
            }
        }

        public static string GetDescription<T>(this T enumerationValue)
        where T : struct
        {
            Type type = enumerationValue.GetType();
            if (!type.IsEnum)
            {
                throw new ArgumentException("EnumerationValue must be of Enum type", "enumerationValue");
            }

            //Tries to find a DescriptionAttribute for a potential friendly name
            //for the enum
            MemberInfo[] memberInfo = type.GetMember(enumerationValue.ToString());
            if (memberInfo != null && memberInfo.Length > 0)
            {
                object[] attrs = memberInfo[0].GetCustomAttributes(typeof(DisplayNameAttribute), false);

                if (attrs != null && attrs.Length > 0)
                {
                    //Pull out the description value
                    return ((DisplayNameAttribute)attrs[0]).DisplayName;
                }
            }
            //If we have no description attribute, just return the ToString of the enum
            return enumerationValue.ToString();
        }
    }
}
