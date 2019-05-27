using DSharpPlus.Entities;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System.Threading.Tasks;
using System;
using Newtonsoft.Json;
using System.Net;
using DogeHelper.Entities;

namespace DogeHelper.Commands
{
    public class MagiskCommands
    {
        // Magisk update JSON links
        private readonly static string MagiskStableJson = "https://raw.githubusercontent.com/topjohnwu/magisk_files/master/stable.json";
        private readonly static string MagiskBetaJson = "https://raw.githubusercontent.com/topjohnwu/magisk_files/master/beta.json";

        // Magisk update fields
        private static long MagiskStableTimestamp;
        private static long MagiskBetaTimestamp;
        private static long MagiskCanaryTimestamp;

        // Magisk update JSON cache
        private static MagiskUpdateJson MagiskStable;
        private static MagiskUpdateJson MagiskBeta;
        private static MagiskUpdateJson MagiskCanary;

        [Group("magisk", CanInvokeWithoutSubcommand = true)]
        [Description("Provides links to Magisk, direct download included.")]
        public class Magisk
        {
            public async Task ExecuteGroupAsync(CommandContext context)
            {
                await context.TriggerTypingAsync();

                // Print default Magisk message
                var embed = new DiscordEmbedBuilder
                {
                    Title = "**Magisk**",
                    Description = "A Magic Mask to Alter System Systemless-ly.",
                    Color = DiscordColor.DarkGreen,
                    Author = new DiscordEmbedBuilder.EmbedAuthor
                    {
                        Name = "topjohnwu",
                        IconUrl = Globals.Authors.Topjohnwu.MagiskImg,
                        Url = Globals.Authors.Topjohnwu.XdaProfile
                    },
                    Footer = Globals.Footers.DefaultFooter()
                };

                embed.AddField("Magisk XDA Thread", Globals.Links.MagiskXdaThread);
                embed.AddField("Magisk Canary XDA Thread", Globals.Links.MagiskCanaryXdaThread);
                embed.AddField($"*To get links to specific versions of Magisk use:*", $"`{ Globals.botPrefix }help magisk` to view.");

                await context.RespondAsync(embed: embed);
            }

            [Command("stable"), Aliases("s"), Description("Gets links for the latest stable version of Magisk.")]
            public async Task LatestMagiskStableAsync(CommandContext context)
            {
                await context.TriggerTypingAsync();

                // Build latest stable Magisk message
                var embed = new DiscordEmbedBuilder
                {
                    Title = "**Magisk Stable**",
                    Description = "A Magic Mask to Alter System Systemless-ly.",
                    Color = DiscordColor.DarkGreen,
                    Author = new DiscordEmbedBuilder.EmbedAuthor
                    {
                        Name = "topjohnwu",
                        IconUrl = Globals.Authors.Topjohnwu.MagiskImg,
                        Url = Globals.Authors.Topjohnwu.XdaProfile
                    },
                    Footer = Globals.Footers.DefaultFooter()
                };

                // Check for latest Stable Magisk
                try
                {
                    await Task.Run(action: () => getMagiskStable());
                }
                catch (WebException ex)
                {
                    embed.AddField("Error downloading updates...", ex.Message);

                    // Catch web exception, load cache if possible.
                    if (MagiskStable != null)
                    {
                        embed.AddField("Unable to fetch new Magisk update.", $"Find cached links below for Magisk ({ MagiskStable.Magisk.Version }).");
                    }
                    else
                    {
                        embed.AddField("Unable to fetch new Magisk update.", "No cached links available. Please try again later.");
                    }
                }

                if (MagiskStable != null)
                {
                    embed.AddField($"Magisk Stable ({ MagiskStable.Magisk.Version })", MagiskStable.Magisk.Link);
                    embed.AddField($"Magisk Manager Stable ({ MagiskStable.App.Version })", MagiskStable.App.Link);
                    embed.AddField("Magisk Uninstaller", MagiskStable.Uninstaller.Link);
                }

                await context.RespondAsync(embed: embed);
            }

            [Command("beta"), Aliases("b"), Description("Gets links for the latest beta version of Magisk.")]
            public async Task LatestMagiskBetaAsync(CommandContext context)
            {
                await context.TriggerTypingAsync();

                // Build latest stable Magisk message
                var embed = new DiscordEmbedBuilder
                {
                    Title = "**Magisk Beta**",
                    Description = "A Magic Mask to Alter System Systemless-ly.",
                    Color = DiscordColor.Orange,
                    Author = new DiscordEmbedBuilder.EmbedAuthor
                    {
                        Name = "topjohnwu",
                        IconUrl = Globals.Authors.Topjohnwu.MagiskImg,
                        Url = Globals.Authors.Topjohnwu.XdaProfile
                    },
                    Footer = Globals.Footers.DefaultFooter()
                };

                // Check for latest Stable Magisk
                try
                {
                    await Task.Run(action: () => getMagiskBeta());
                }
                catch (WebException ex)
                {
                    embed.AddField("Error downloading updates...", ex.Message);

                    // Catch web exception, load cache if possible.
                    if (MagiskBeta != null)
                    {
                        embed.AddField("Unable to fetch new Magisk update.", $"Find cached links below for Magisk ({ MagiskBeta.Magisk.Version }).");
                    }
                    else
                    {
                        embed.AddField("Unable to fetch new Magisk update.", "No cached links available. Please try again later.");
                    }
                }

                if (MagiskBeta != null)
                {
                    embed.AddField($"Magisk Beta ({ MagiskBeta.Magisk.Version })", MagiskBeta.Magisk.Link);
                    embed.AddField($"Magisk Manager Beta ({ MagiskBeta.App.Version })", MagiskBeta.App.Link);
                    embed.AddField("Magisk Uninstaller", MagiskBeta.Uninstaller.Link);
                }

                await context.RespondAsync(embed: embed);
            }

            private MagiskUpdateJson getMagiskStable()
            {
                // Check if cached, only redownload if more than 1 hour old or MagiskStable has not been checked.
                if ((DateTimeOffset.Now.ToUnixTimeSeconds() - MagiskStableTimestamp) > 3600 || MagiskStable == null)
                {
                    Console.WriteLine("[INFO]: Downloading Magisk stable update JSON... Previous version age: " + (DateTimeOffset.Now.ToUnixTimeSeconds() - MagiskStableTimestamp));

                    // Save new timestamp.
                    MagiskStableTimestamp = DateTimeOffset.Now.ToUnixTimeSeconds();

                    // If greater than 1 hour, fetch new.
                    string jsonContents;
                    using (var wc = new WebClient())
                    {
                        jsonContents = wc.DownloadString(MagiskStableJson);
                        wc.Dispose();
                    }

                    // Convert to MagiskUpdateJson
                    MagiskStable = JsonConvert.DeserializeObject<MagiskUpdateJson>(jsonContents);
                }
                else
                {
                    Console.WriteLine("[INFO]: Accessed cache values for Magisk Stable");
                }

                return MagiskStable;
            }

            private MagiskUpdateJson getMagiskBeta()
            {
                // Check if cached, only redownload if more than 1 hour old or MagiskStable has not been checked.
                if ((DateTimeOffset.Now.ToUnixTimeSeconds() - MagiskBetaTimestamp) > 3600 || MagiskBeta == null)
                {
                    Console.WriteLine("[INFO]: Downloading Magisk beta update JSON... Previous version age: " + (DateTimeOffset.Now.ToUnixTimeSeconds() - MagiskBetaTimestamp));

                    // Save new timestamp.
                    MagiskBetaTimestamp = DateTimeOffset.Now.ToUnixTimeSeconds();

                    // If greater than 1 hour, fetch new.
                    string jsonContents;
                    using (var wc = new WebClient())
                    {
                        jsonContents = wc.DownloadString(MagiskBetaJson);
                        wc.Dispose();
                    }

                    // Convert to MagiskUpdateJson
                    MagiskBeta = JsonConvert.DeserializeObject<MagiskUpdateJson>(jsonContents);
                }
                else
                {
                    Console.WriteLine("[INFO]: Accessed cache values for Magisk Beta");
                }

                return MagiskBeta;
            }

            // TODO: Magisk Canary builds
            private MagiskUpdateJson getMagiskCanary()
            {
                return MagiskCanary;
            }
        }
    }
}
