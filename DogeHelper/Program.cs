﻿using System;
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
            Console.WriteLine(">>> DroidLinkr (DogeHelper) v{0} <<<\n\n", typeof(Program).Assembly.GetName().Version);

            if (args.Length < 1)
            {
                Console.WriteLine("Invalid usage.\n" +
                    "Usage: [exec name] {Discord Bot Token} [Prefix]\n" +
                    "You must supply Discord Bot Token as the first argument to the program.");
                Environment.Exit(1);
            }

            MainAsync(args).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        static async Task MainAsync(string[] args)
        {
           // Load in token from arguments.
            Globals.BotToken = args[0];
            Console.WriteLine("Token: {0}", args[0]);

            // Initalise the bot
            discord = new DiscordClient(new DiscordConfiguration
            {
                Token = Globals.BotToken,
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
            discord.UpdateStatusAsync(new DiscordGame("Serving Links"), null, null);

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
