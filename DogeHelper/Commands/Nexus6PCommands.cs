using DSharpPlus.Entities;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System.Threading.Tasks;

namespace DogeHelper.Commands
{
    public class Nexus6PCommands
    {
        [Command("blod"), Description("Provides link to osm0sis' BLOD files.")]
        public async Task Blod(CommandContext context)
        {
            await context.TriggerTypingAsync();

            var embed = new DiscordEmbedBuilder
            {
                Title = "**osm0sis' Bootloop of Death (BLOD) Downloads**",
                Url = Globals.Links.OsmodsBasketBuild,
                Author = new DiscordEmbedBuilder.EmbedAuthor
                {
                    Name = "osm0sis",
                    IconUrl = Globals.Authors.Osm0sis.Img,
                    Url = Globals.Authors.Osm0sis.XdaProfile
                },
                Description = "Links to files which may temporarily fix the BLOD. Links include patched TWRP & BLOD Workaround Injector flashable ZIP.",
                Color = DiscordColor.Aquamarine,
                Footer = Globals.Footers.DefaultFooter()
            };

            embed.AddField("Patched (angler/Nexus 6P) TWRP 3.2.1-0 with File-based Encryption Compatibility", Globals.Links.OsmodsTwrpAngler);
            embed.AddField("Patched (bullhead/Nexus 5X) TWRP 3.2.1-0 with File-based Encryption Compatibility", Globals.Links.OsmodsTwrpBullhead);
            embed.AddField("Bootloop of Death Workaround Injector (Flashable ZIP)", Globals.Links.OsmodsBlodWorkaround);

            await context.RespondAsync(embed: embed);
        }
    }
}
