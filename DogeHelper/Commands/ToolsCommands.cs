using DSharpPlus.Entities;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

using System.Threading.Tasks;
using System;

namespace DogeHelper.Commands
{
    [Group("tools", CanInvokeWithoutSubcommand = true)]
    [Description("General and more specific Android tools.")]
    [Aliases("t")]
    public class Tools
    {
        public async Task ExecuteGroupAsync(CommandContext context)
        {
            await context.TriggerTypingAsync();

            // Print help message.
            var embed = new DiscordEmbedBuilder
            {
                Title = "**Tools Help**",
                Description = $"Displays appropriate tools per category. Usage: `{ Globals.botPrefix }<t/tools> <category>`.",
                Color = DiscordColor.Orange,
                Footer = Globals.Footers.HelpFooter()
            };

            embed.AddField("**android**", "Outputs links to Android related tools & items. (Alias: `a`)");
            embed.AddField("**google**", "Outputs links to Google related tools & items. (Alias: `g`)");

            await context.RespondAsync(embed: embed);
        }

        [Command("android"), Aliases("a"), Description("Useful links for all, if not most Androids.")]
        public async Task Android(CommandContext context)
        {
            await context.TriggerTypingAsync();

            var embed = new DiscordEmbedBuilder
            {
                Title = "**Listing: Tools > Android**",
                Description = "Links to tools which can be used with any Android device.",
                Color = DiscordColor.SpringGreen,
                Footer = Globals.Footers.DefaultFooter()
            };

            embed.AddField("Android SDK Platform Tools (adb & fastboot)", Globals.Links.SdkPlatformTools);

            await context.RespondAsync(embed: embed);
        }

        [Command("google"), Aliases("g"), Description("Useful links for Google devices.")]
        public async Task Google(CommandContext context, [Description("Links to specified device.")] params string[] device)
        {
            await context.TriggerTypingAsync();

            var embed = new DiscordEmbedBuilder
            {
                Title = "**Listing: Tools > Google**",
                Description = "Links to tools which are used with Google devices (Nexus & Pixels).",
                Color = DiscordColor.Grayple,
                Footer = Globals.Footers.DefaultFooter()
            };

            // Determine device provided
            string[] deviceNames = new String[] { "", "" };

            if (device.Length > 0)
            {
                deviceNames = Globals.Device(device);
                if (!String.IsNullOrEmpty(deviceNames[0]))
                {
                    embed.AddField("The following links are for:", $"**{ deviceNames[1] } ({ deviceNames[0] })**");
                }
                else
                {
                    embed.AddField("No device found.", "Showing generic links.");
                }
            }

            embed.AddField("Google Factory Images for Nexus/Pixel Devices", string.Concat(Globals.Links.GoogleFactoryImages, deviceNames[0]));
            embed.AddField("Google Full OTA Images for Nexus/Pixel Devices", string.Concat(Globals.Links.GoogleFullOtaImages, deviceNames[0]));

            embed.AddField("Google USB Drivers", Globals.Links.GoogleUsbDrivers);

            await context.RespondAsync(embed: embed);
        }
    }
}
