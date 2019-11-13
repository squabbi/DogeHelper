using DogeHelper.Db;
using DogeHelper.Entities;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using System;
using System.Threading.Tasks;

namespace DogeHelper.Commands
{
    [Group("link", CanInvokeWithoutSubcommand = true), Aliases("l"),
        Description("Provides shortcuts to links and other common info.")]
    public class LinkCommands
    {
        public async Task ExecuteGroupAsync(CommandContext context, params string[] keys)
        {
            await context.TriggerTypingAsync();

            // Check if key is provided, else return default message
            if (keys.Length > 0)
            {
                var link = await GetLinkAsync(keys[0]);
                if (link == null)
                {
                    var embed = new DiscordEmbedBuilder
                    {
                        Title = "No link found",
                        Description = $"{DiscordEmoji.FromName(context.Client, ":x:")} " +
                    $"No such key: `{keys[0]}` found in database. Please try again.",
                        Color = DiscordColor.Red,
                        Footer = Globals.Footers.DefaultFooter()
                    };

                    await context.RespondAsync(embed: embed);
                }
                else
                {
                    await context.RespondAsync(link.Value);
                }
            }
            else
            {
                await context.RespondAsync(embed: DefaultEmbed());
            }
        }

        [Command("add"), Aliases("a"), Description("Adds a link with a specified key. Key must be unique.")]
        [RequireRolesAttribute("Helpers 🌟")]
        public async Task Add(CommandContext context, string key, string value)
        {
            await context.TriggerTypingAsync();

            // Create Link object
            var link = new Link {
                Key = key, 
                Value = value,
                LastChangedBy = context.Member.Username,
                LastChanged = DateTime.Now
            };

            if (await AddLinkAsync(link))
            {
                var embed = new DiscordEmbedBuilder
                {
                    Title = "Added link",
                    Description = $"{DiscordEmoji.FromName(context.Client, ":tada:")} " +
                    $"New link added under key: `{key}`.",
                    Color = DiscordColor.SpringGreen,
                    Footer = Globals.Footers.DefaultFooter()
                };

                embed.AddField(key, value);

                await context.RespondAsync(embed: embed);
            }
            else
            {
                var embed = new DiscordEmbedBuilder
                {
                    Title = "Failed to add link",
                    Description = $"{DiscordEmoji.FromName(context.Client, ":x:")} " +
                    $"Cannot add new link. Key: `{key}` already exists.",
                    Color = DiscordColor.Red,
                    Footer = Globals.Footers.DefaultFooter()    
                };

                var existingLink = await GetLinkAsync(key);

                embed.AddField(existingLink.Key, existingLink.Value);
                embed.AddField("Last updated by:", existingLink.LastChangedBy, true);
                embed.AddField("Last updated:", existingLink.LastChanged.ToLocalTime().ToShortDateString(), true);

                await context.RespondAsync(embed: embed);
            }
        }

        [Command("update"), Aliases("u"), Description("Remove link with specified key.")]
        [RequireRolesAttribute("Helpers 🌟")]
        public async Task Update(CommandContext context, string key, string value)
        {
            await context.TriggerTypingAsync();

            // Update link with key, value
            if (await UpdateLinkAsync(key, value, context.Member.Username))
            {
                // Successfully updated
                var embed = new DiscordEmbedBuilder
                {
                    Title = "Update success",
                    Description = $"{DiscordEmoji.FromName(context.Client, ":white_check_mark:")} " +
                    $"Updated Link with key: `{key}` successfully.",
                    Color = DiscordColor.SpringGreen,
                    Footer = Globals.Footers.DefaultFooter()
                };

                embed.AddField(key, value);

                await context.RespondAsync(embed: embed);
            }
            else
            {
                // Successfully updated
                var embed = new DiscordEmbedBuilder
                {
                    Title = "Update failed",
                    Description = $"{DiscordEmoji.FromName(context.Client, ":x:")} " +
                    $"Unable to find Link with key: `{key}`.",
                    Color = DiscordColor.Red,
                    Footer = Globals.Footers.DefaultFooter()
                };

                embed.AddField(key, value);

                await context.RespondAsync(embed: embed);
            }
        }

        [Command("remove"), Aliases("r"), Description("Remove link with specified key.")]
        [RequireRolesAttribute("Helpers 🌟")]
        public async Task Remove(CommandContext context, string key)
        {
            await context.TriggerTypingAsync();

            // Locate and remove link.
            if (await RemoveLinkAsync(key) > 0)
            {
                // Located and removed at least 1 Link
                var embed = new DiscordEmbedBuilder
                {
                    Title = "Removal success",
                    Description = $"{DiscordEmoji.FromName(context.Client, ":white_check_mark:")} " +
                    $"Removed Link with key: `{key}` successfully.",
                    Color = DiscordColor.SpringGreen,
                    Footer = Globals.Footers.DefaultFooter()
                };

                await context.RespondAsync(embed: embed);
            }
            else
            {
                var embed = new DiscordEmbedBuilder
                {
                    Title = "Removal failed",
                    Description = $"{DiscordEmoji.FromName(context.Client, ":x:")} " +
                    $"No such key: `{key}` was found.",
                    Color = DiscordColor.Red,
                    Footer = Globals.Footers.DefaultFooter()
                };

                await context.RespondAsync(embed: embed);
            }
        }

        [Command("removeall"), Description("Removes all existing links in database. This action is irreversible.")]
        [RequireOwner]
        public async Task RemoveAll(CommandContext context)
        {
            await context.TriggerTypingAsync();

            var interactivity = context.Client.GetInteractivityModule();

            var embed = new DiscordEmbedBuilder
            {
                Title = "Remove all links",
                Description = $"{DiscordEmoji.FromName(context.Client, ":warning:")} " +
                    $"This will remove all links in database. Are you sure? Type `yes` to confirm within 5 seconds.",
                Color = DiscordColor.Orange,
                Footer = Globals.Footers.DefaultFooter()
            };

            await context.RespondAsync(embed: embed);

            var msg = await interactivity.WaitForMessageAsync(m => ((m.Author == context.Member) && (m.Content.Equals("yes"))),
                TimeSpan.FromSeconds(5));
            if (msg != null)
            {
                // Delete all messages
                await RemoveAllLinks();
                var removedEmbed = new DiscordEmbedBuilder
                {
                    Title = "Remove all links success",
                    Description = $"{DiscordEmoji.FromName(context.Client, ":sparkles:")} " +
                    $"Removed all links from database successfully.",
                    Color = DiscordColor.SpringGreen,
                    Footer = Globals.Footers.DefaultFooter()
                };

                await context.RespondAsync(embed: removedEmbed);
            }
            else
            {
                var failedEmbed = new DiscordEmbedBuilder
                {
                    Title = "Response timeout",
                    Description = $"{DiscordEmoji.FromName(context.Client, ":hourglass:")} " +
                    $"Response for confirmation timed out.",
                    Color = DiscordColor.Yellow,
                    Footer = Globals.Footers.DefaultFooter()
                };

                await context.RespondAsync(embed: failedEmbed);
            }
        }

        private static DiscordEmbed DefaultEmbed()
        {
            var embed = new DiscordEmbedBuilder
            {
                Title = "Usage",
                Color = DiscordColor.Orange,
                Description = $"To view links, say `{Globals.BotPrefix}l[ink] [key]`.",
                Footer = Globals.Footers.DefaultFooter()
            };

            return embed;
        }

        private async Task<Link> GetLinkAsync(string key)
        {
            return await DbHelper.Instance.GetMessage(key);
        }

        private async Task<bool> AddLinkAsync(Link link)
        {
            return await DbHelper.Instance.AddMessage(link);
        }

        private async Task<bool> UpdateLinkAsync(string key, string value, string username)
        {
            return await DbHelper.Instance.UpdateMessage(key, value, username);
        }

        private async Task<int> RemoveLinkAsync(string key)
        {
            return await DbHelper.Instance.RemoveMessage(key);
        }

        private Task RemoveAllLinks()
        {
            DbHelper.Instance.RemoveAllMessages();

            return Task.CompletedTask;
        }
    }
}
