using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AngleSharp;
using AngleSharp.Html.Dom;
using DogeHelper.Entities;
using DSharpPlus.Entities;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

namespace DogeHelper.Commands
{
    class GoogleCommands
    {
        internal async Task<List<FactoryImage>> LoadFactoryImages(int tableNo)
        {
            List<FactoryImage> factoryImages = new List<FactoryImage>();

            // AngleSharp, rip table contents
            var config = Configuration.Default.WithDefaultLoader();
            var source = "https://developers.google.com/android/images";
            var context = BrowsingContext.New(config);
            var document = await context.OpenAsync(source).ConfigureAwait(true);

            // Locate all tables in the site (containing the factory images)
            var tables = document.QuerySelectorAll("table");
            // Locate the desired table
            var table = (IHtmlTableElement)tables[tableNo];
            var tableContents = table.Rows;
            // Go through each row after the first one (title row)
            for (int i = 1; i < tableContents.Length; i++)
            {
                var rowChildren = tableContents[i].Children;
                // Child 1 - Version, child 2 - Link (href), child 3 - Checksum
                string version = rowChildren[0].TextContent;
                string link = ((IHtmlAnchorElement)rowChildren[1].FirstChild).Href.ToString();
                string checksum = rowChildren[2].TextContent;

                // Split up version and link by '/' and '-' to get individual elements
                string[] versionSplit = version.Split(' ');
                string[] linkSplit = link.Split(new char[] { '/', '-' });

                // Create the Factory Image and add it to the list of factory images
                factoryImages.Add(new FactoryImage(linkSplit[6], versionSplit[0], linkSplit[7].ToUpper(), linkSplit[9].Substring(0, 8), version, link, checksum));
            }

            return factoryImages;
        }

        [Group("google", CanInvokeWithoutSubcommand = false), Aliases("g")]
        [Description("Google related commands.")]
        public class Google
        {
            // TODO: Cache images website

            // Cache for Factory Images
            private Dictionary<Globals.GoogleDevice, (long, FactoryImage)> cacheFactoryImages = 
                new Dictionary<Globals.GoogleDevice, (long, FactoryImage)>();

            internal async Task<FactoryImage> GetLatestFactoryImage(Globals.GoogleDevice device)
            {
                // AngleSharp, rip table contents
                var config = Configuration.Default.WithDefaultLoader();
                var source = "https://developers.google.com/android/images";
                var context = BrowsingContext.New(config);
                var document = await context.OpenAsync(source).ConfigureAwait(true);

                // Locate all tables in the site (containing the factory images)
                var tables = document.QuerySelectorAll("table");
                // Locate the desired table
                var table = (IHtmlTableElement)tables[(int)device];
                var tableContents = table.Rows;

                // Go through the first row
                var rowChildren = tableContents[tableContents.Length - 1].Children;
                // Child 1 - Version, child 2 - Link (href), child 3 - Checksum
                string version = rowChildren[0].TextContent;
                string link = ((IHtmlAnchorElement)rowChildren[1].FirstChild).Href.ToString();
                string checksum = rowChildren[2].TextContent;

                // Split up version and link by '/' and '-' to get individual elements
                string[] versionSplit = version.Split(' ');
                string[] linkSplit = link.Split(new char[] { '/', '-' });

                // Create the Factory Image and add it to the list of factory images
                return new FactoryImage(linkSplit[6], versionSplit[0], linkSplit[7].ToUpper(), linkSplit[9].Substring(0, 8),
                    string.Format("{0} {1}", versionSplit[2], versionSplit[3]).TrimEnd(')'), link, checksum);
            }

            [Command("factory"), Aliases("f"), Description("Gets the latest factory image with for specified device.")]
            public async Task LatestDeviceFactoryImage(CommandContext context, params string[] device)
            {
                await context.TriggerTypingAsync();

                var embed = new DiscordEmbedBuilder
                {
                    Color = DiscordColor.SpringGreen,
                    Footer = Globals.Footers.DefaultFooter()
                };

                var deviceArg = string.Concat(device).ToLower();
                Globals.PrintMessage($"Factory Image query: {deviceArg} by user: {context.User.Username} " +
                    $"({context.User.Id}).");

                // Determine device
                string[] deviceNames = new String[] { "", "" };

                if (device.Length > 0)
                {
                    deviceNames = Globals.Device(device);
                    if (!string.IsNullOrEmpty(deviceNames[0]))
                    {
                        try
                        {
                            Globals.GoogleDeviceDict.TryGetValue(deviceNames[0], out Globals.GoogleDevice gDevice);
                            FactoryImage factoryImage = await GetFactoryImage(gDevice).ConfigureAwait(true);

                            // Build embed
                            embed.Title = $"**Factory Image for {deviceNames[1]} ({factoryImage.Codename})**";
                            embed.Description = $"Latest Google Factory Image for {deviceNames[1]} ({deviceNames[0]}).";
                            embed.ThumbnailUrl = "https://developers.google.com/site-assets/developers_64dp_72.png";

                            embed.AddField("Version", factoryImage.AndroidVersion);
                            embed.AddField("Build Number", factoryImage.AndroidBuild);
                            embed.AddField("Date", factoryImage.Date);
                            embed.AddField("Download", $"[Download Factory Image]({factoryImage.Link})");
                            embed.AddField("Checksum (SHA-256)", factoryImage.Checksum);
                        }
                        catch (ArgumentNullException argNullEx)
                        {
                            Console.WriteLine("Unable to determine enum via key.");
                        }
                    }
                    else
                    {
                        embed.AddField("No device found", "Please try again.");
                        embed.Color = DiscordColor.Red;
                        Globals.PrintMessage($"No device found for {deviceArg}.");
                    }
                }
                else
                {
                    embed.AddField("No device name provided", "Command usage: g[oogle] f[actory] [device name]");
                    embed.AddField("Example", $"{Globals.BotPrefix}g f pixel 4");
                    embed.Color = DiscordColor.Red;
                }

                await context.RespondAsync(embed: embed).ConfigureAwait(false);
            }

            private async Task<FactoryImage> GetFactoryImage(Globals.GoogleDevice device)
            {
                if (!FactoryImageHasCached(device))
                {
                    var factoryImage = await GetLatestFactoryImage(device);
                    var timestamp = DateTimeOffset.Now.ToUnixTimeSeconds();
                    // Add to cache
                    cacheFactoryImages.Add(device, (timestamp, factoryImage));
                    Globals.PrintMessage($"Added cached FactoryImage for {device} at {timestamp}.");

                    return factoryImage;
                }

                // Return cached FactoryImage from cache
                cacheFactoryImages.TryGetValue(device, out (long, FactoryImage) result);
                Globals.PrintMessage($"Returning cached FactoryImage for {device}. It is {result.Item1}" +
                    $" seconds old.");
                return result.Item2;
            }

            private bool FactoryImageHasCached(Globals.GoogleDevice device)
            {
                Globals.PrintMessage($"Checking for cached FactoryImage for {device}.");
                if (cacheFactoryImages.ContainsKey(device))
                {
                    cacheFactoryImages.TryGetValue(device, out (long, FactoryImage) result);
                    var age = DateTimeOffset.Now.ToUnixTimeSeconds() - result.Item1;
                    return !(age > 86400);
                }

                return false;
            }
        }
    }
}
