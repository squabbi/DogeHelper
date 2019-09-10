using System;
using DSharpPlus.Entities;

namespace DogeHelper
{
    public static class Globals
    {
        public static readonly string botToken = "NTI0MTM5MDM0MjE4NDYzMjMz.Dvj8-w.hX2q976Po95_TbNJFoYszPxu1n4";
        public static readonly string botPrefix = ";;";

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
                    return new String[] { "crosshatch", "Pixel 3 XL" };

                case "3":
                case "pixel3":
                case "blueline":
                    return new String[] { "blueline", "Pixel 3" };

                case "2xl":
                case "pixel2xl":
                case "taimen":
                    return new String[] { "taimen", "Pixel 2 XL" };

                case "2":
                case "pixel2":
                case "walleye":
                    return new String[] { "walleye", "Pixel 2" };

                case "xl":
                case "pixelxl":
                case "marlin":
                    return new String[] { "marlin", "Pixel XL" };

                case "pixel":
                case "sailfish":
                    return new String[] { "sailfish", "Pixel" };

                case "c":
                case "pixelc":
                case "ryu":
                    return new String[] { "ryu", "Pixel C" };

                case "6p":
                case "nexus6p":
                case "angler":
                    return new String[] { "angler", "Nexus 6P" };

                case "5x":
                case "nexus5x":
                case "bullhead":
                    return new String[] { "bullhead", "Nexus 5X" };

                case "6":
                case "nexus6":
                case "shamu":
                    return new String[] { "shamu", "Nexus 6" };
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
            public static readonly string DefaultFooter = "c0yns the Doge, helper bot for MaowDroid.";
            public static readonly string HelpFooter = "help | c0yns the Doge, helper bot for MaowDroid.";
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
    }
}
