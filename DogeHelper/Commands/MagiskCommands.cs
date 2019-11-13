using DSharpPlus.Entities;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System.Threading.Tasks;
using System;
using Newtonsoft.Json;
using System.Net;
using DogeHelper.Entities;
using System.ComponentModel.DataAnnotations;

namespace DogeHelper.Commands
{
    public class MagiskCommands
    {
        // Magisk update fields
        private static long MagiskStableTimestamp;
        private static long MagiskBetaTimestamp;
        private static long MagiskCanaryReleaseTimestamp;
        private static long MagiskCanaryDebugTimestamp;

        // Magisk update JSON cache
        private static MagiskUpdateJson MagiskStable;
        private static MagiskUpdateJson MagiskBeta;
        private static MagiskUpdateJson MagiskCanaryRelease;
        private static MagiskUpdateJson MagiskCanaryDebug;

        // Enum for different Magisk Versions
        public enum MagiskBuild
        {
            Stable,
            Beta,
            [Display(Name = "Canary Debug")]
            CanaryDebug,
            [Display(Name = "Canary Release")]
            CanaryRelease
        }

        internal void GetMagiskUpdate(MagiskBuild build, DiscordEmbedBuilder embed)
        {
            MagiskUpdateJson update = null;
            long timestamp = 0;
            string url = null;

            switch (build)
            {
                case MagiskBuild.Stable:
                    {
                        update = MagiskStable;
                        timestamp = MagiskStableTimestamp;
                        url = Globals.Links.MagiskStableJson;
                        PrintUpdate();
                        MagiskStable = CheckMagiskUpdate();
                        MagiskStableTimestamp = timestamp;
                        break;
                    }
                case MagiskBuild.Beta:
                    {
                        update = MagiskBeta;
                        timestamp = MagiskBetaTimestamp;
                        url = Globals.Links.MagiskBetaJson;
                        PrintUpdate();
                        MagiskBeta = CheckMagiskUpdate();
                        MagiskBetaTimestamp = timestamp;
                        break;
                    }
                case MagiskBuild.CanaryDebug:
                    {
                        update = MagiskCanaryDebug;
                        timestamp = MagiskCanaryDebugTimestamp;
                        url = Globals.Links.MagiskCanaryDebugJson;
                        PrintUpdate();
                        MagiskCanaryDebug = CheckMagiskUpdate();
                        MagiskCanaryDebugTimestamp = timestamp;
                        break;
                    }
                case MagiskBuild.CanaryRelease:
                    {
                        update = MagiskCanaryRelease;
                        timestamp = MagiskCanaryReleaseTimestamp;
                        url = Globals.Links.MagiskCanaryReleaseJson;
                        PrintUpdate();
                        MagiskCanaryRelease = CheckMagiskUpdate();
                        MagiskCanaryReleaseTimestamp = timestamp;
                        break;
                    }
            }

            void PrintUpdate()
            {
                Globals.PrintMessage($"Magisk Channel: {build}, Timestamp: {timestamp}.");
            }
            
            MagiskUpdateJson CheckMagiskUpdate()
            {
                // Download new update information
                if ((DateTimeOffset.Now.ToUnixTimeSeconds() - timestamp) > 3600 || update == null)
                {
                    Console.WriteLine("[INFO] {0}: Downloading {1}. Previous version age: {2}",
                        DateTimeOffset.Now.DateTime,
                        build,
                        DateTimeOffset.Now.ToUnixTimeSeconds() - timestamp);

                    // Save new timestamp.
                    timestamp = DateTimeOffset.Now.ToUnixTimeSeconds();

                    try
                    {
                        string json;

                        using (var wc = new WebClient())
                        {
                            json = wc.DownloadString(url);
                            wc.Dispose();
                        }

                        // Convert to MagiskUpdateJson
                        return JsonConvert.DeserializeObject<MagiskUpdateJson>(json);
                    }
                    catch (WebException webEx)
                    {
                        embed.AddField("Error Downloading Update", webEx.Message);
                        embed.AddField("URL Used", url);

                        return null;
                    }
                }
                else
                {
                    Console.WriteLine("[INFO] {0}: Loaded cached values.",
                        DateTimeOffset.Now.DateTime);
                }

                return update;
            }
        }

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
                    Description = "Stable version should work for most. Newer versions of Android may require temporary use of Canary or Beta builds.",
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
                embed.AddField($"*To get links to specific versions of Magisk use:*", $"`{ Globals.BotPrefix }help magisk` to view.");

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
                await Task.Run(action: () => new MagiskCommands().GetMagiskUpdate(MagiskBuild.Stable, embed));

                if (MagiskStable != null)
                {
                    embed.AddField($"Magisk Stable (v{ MagiskStable.Magisk.Version })", MagiskStable.Magisk.Link);
                    embed.AddField($"Magisk Manager Stable (v{ MagiskStable.App.Version })", MagiskStable.App.Link);
                    embed.AddField("Magisk Uninstaller", MagiskStable.Uninstaller.Link);
                }
                else
                {
                    embed.AddField("Unable to fetch new Magisk update.", "No cached links available. Please try again later.");
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
                    Description = "Beta version of Magisk, not well tested and may have issues.",
                    Color = DiscordColor.Orange,
                    Author = new DiscordEmbedBuilder.EmbedAuthor
                    {
                        Name = "topjohnwu",
                        IconUrl = Globals.Authors.Topjohnwu.MagiskImg,
                        Url = Globals.Authors.Topjohnwu.XdaProfile
                    },
                    Footer = Globals.Footers.DefaultFooter()
                };

                // Check for latest Beta Magisk
                await Task.Run(action: () => new MagiskCommands().GetMagiskUpdate(MagiskBuild.Beta, embed));

                if (MagiskBeta != null)
                {
                    embed.AddField($"Magisk Beta (v{ MagiskBeta.Magisk.Version })", MagiskBeta.Magisk.Link);
                    embed.AddField($"Magisk Manager Beta (v{ MagiskBeta.App.Version })", MagiskBeta.App.Link);
                    embed.AddField("Magisk Uninstaller", MagiskBeta.Uninstaller.Link);
                }
                else
                {
                    embed.AddField("Unable to fetch new Magisk update.", "No cached links available. Please try again later.");
                }

                await context.RespondAsync(embed: embed);
            }

            [Group("canary", CanInvokeWithoutSubcommand = true), Aliases("c")]
            [Description("Provides links to Magisk Canary, direct download included.")]
            public class Canary
            {
                public async Task ExecuteGroupAsync(CommandContext context)
                {
                    await LatestMagiskCanaryReleaseAsync(context);
                }

                [Command("debug"), Aliases("d"), Description("Gets links for the latest canary debug build of Magisk.")]
                public async Task LatestMagiskCanaryDebugAsync(CommandContext context)
                {
                    await context.TriggerTypingAsync();

                    // Build latest canary debug
                    var embed = new DiscordEmbedBuilder
                    {
                        Title = "**Magisk Canary Debug**",
                        Description = "Unstable, bleeding edge version of Magisk.",
                        Color = DiscordColor.IndianRed,
                        Author = new DiscordEmbedBuilder.EmbedAuthor
                        {
                            Name = "topjohnwu",
                            IconUrl = Globals.Authors.Topjohnwu.MagiskImg,
                            Url = Globals.Authors.Topjohnwu.XdaProfile
                        },
                        Footer = Globals.Footers.DefaultFooter()
                    };

                    // Check for latest Magisk Canary Debug
                    await Task.Run(action: () => new MagiskCommands().GetMagiskUpdate(MagiskBuild.CanaryDebug, embed));

                    if (MagiskCanaryDebug != null)
                    {
                        embed.AddField($"Magisk Canary Debug (v{ MagiskCanaryDebug.Magisk.Version })", MagiskCanaryDebug.Magisk.Link);
                        embed.AddField($"Magisk Manager Canary Debug (v{ MagiskCanaryDebug.App.Version })", MagiskCanaryDebug.App.Link);
                        embed.AddField("Magisk Uninstaller", MagiskCanaryDebug.Uninstaller.Link);
                    }
                    else
                    {
                        embed.AddField("Unable to fetch new Magisk Canary Debug update.", "No cached links available. Please try again later.");
                    }

                    await context.RespondAsync(embed: embed);
                }

                [Command("release"), Aliases("r"), Description("Gets links for the latest canary release build of Magisk.")]
                public async Task LatestMagiskCanaryReleaseAsync(CommandContext context)
                {
                    await context.TriggerTypingAsync();

                    // Build latest canary debug
                    var embed = new DiscordEmbedBuilder
                    {
                        Title = "**Magisk Canary Release**",
                        Description = "Unstable, bleeding edge version of magisk.",
                        Color = DiscordColor.Red,
                        Author = new DiscordEmbedBuilder.EmbedAuthor
                        {
                            Name = "topjohnwu",
                            IconUrl = Globals.Authors.Topjohnwu.MagiskImg,
                            Url = Globals.Authors.Topjohnwu.XdaProfile
                        },
                        Footer = Globals.Footers.DefaultFooter()
                    };

                    // Check for latest Magisk Canary Release
                    await Task.Run(action: () => new MagiskCommands().GetMagiskUpdate(MagiskBuild.CanaryRelease, embed));

                    if (MagiskCanaryRelease != null)
                    {
                        embed.AddField($"Magisk Canary Release (v{ MagiskCanaryRelease.Magisk.Version })", MagiskCanaryRelease.Magisk.Link);
                        embed.AddField($"Magisk Manager Canary Release (v{ MagiskCanaryRelease.App.Version })", MagiskCanaryRelease.App.Link);
                        embed.AddField("Magisk Uninstaller", MagiskCanaryRelease.Uninstaller.Link);
                    }
                    else
                    {
                        embed.AddField("Unable to fetch new Magisk update.", "No cached links available. Please try again later.");
                    }

                    await context.RespondAsync(embed: embed);
                }
            }
        }
    }
}
