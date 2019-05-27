using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DogeHelper.Commands
{
    [Group("admin")]
    [Description("Only server owner can access these commands.")]
    [Hidden]
    [RequireOwner]
    class AdministrativeCommands
    {
        [Command("getids"), Description("Get channel IDs for development")]
        public async Task GetIds(CommandContext ctx)
        {
            await ctx.TriggerTypingAsync();

            var embed = new DiscordEmbedBuilder
            {
                Title = "Channel IDs",
                Color = DiscordColor.Magenta
            };

            // Get list of channels
            IReadOnlyList<DiscordChannel> channelList = await ctx.Guild.GetChannelsAsync();

            foreach (DiscordChannel channel in channelList)
            {
                embed.AddField(channel.Name, channel.Id.ToString());
            }

            await ctx.RespondAsync(embed: embed);
        }
    }
}
