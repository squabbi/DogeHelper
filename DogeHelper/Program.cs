using System;
using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;

using DogeHelper.Commands;
using System.Reflection;

namespace DogeHelper
{
    class Program
    {
        static DiscordClient discord;
        static CommandsNextModule commands;

        static void Main(string[] args)
        {
            MainAsync(args).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        static async Task MainAsync(string[] args)
        {
            // Print bot version
            Console.WriteLine($">>> DogeHelper v{Assembly.GetEntryAssembly().GetName().Version}. Now loading... <<<");

            // Initalise the bot
            discord = new DiscordClient(new DiscordConfiguration
            {
                Token = Globals.botToken,
                TokenType = TokenType.Bot,
                UseInternalLogHandler = true,
                LogLevel = LogLevel.Info
            });

            // Initalise commands handler
            commands = discord.UseCommandsNext(new CommandsNextConfiguration
            {
                StringPrefix = Globals.botPrefix
            });

            // Register commands
            commands.RegisterCommands<Tools>();
            commands.RegisterCommands<Nexus6PCommands>();
            commands.RegisterCommands<MagiskCommands>();

            commands.RegisterCommands<AdministrativeCommands>();

            // Subscribe events
            discord.MessageCreated += Discord_MessageCreated;
            discord.GuildAvailable += Discord_GuildAvailable;
            discord.Ready += Discord_Ready;

            await discord.ConnectAsync();

            // Indefinite delay
            await Task.Delay(-1);
        }

        private static Task Discord_Ready(DSharpPlus.EventArgs.ReadyEventArgs e)
        {
            // Update game state when the bot is ready
            discord.UpdateStatusAsync(new DiscordGame("with Android Q"), null, null);

            return Task.CompletedTask;
        }

        private static Task Discord_GuildAvailable(DSharpPlus.EventArgs.GuildCreateEventArgs e)
        {
            Console.WriteLine($"[INFO]: Connected to {e.Guild.Name}, owned by {e.Guild.Owner.Username}.");

            return Task.CompletedTask;
        }

        private static async Task Discord_MessageCreated(DSharpPlus.EventArgs.MessageCreateEventArgs e)
        {
            // Crying channel only
            if (e.Channel.Id.Equals(538197625309233182))
            {
                await e.Message.CreateReactionAsync(DiscordEmoji.FromName(discord, ":cry:"));
            }
        }
    }
}
