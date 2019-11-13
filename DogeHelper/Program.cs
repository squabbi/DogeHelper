using System;
using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;

using DogeHelper.Commands;
using DSharpPlus.Interactivity;
using DSharpPlus.CommandsNext.Exceptions;

namespace DogeHelper
{
    class Program
    {
        public DiscordClient Discord { get; set; }
        public CommandsNextModule Commands { get; set; }
        public InteractivityModule Interactivity { get; set; }


        static void Main(string[] args)
        {
            Console.WriteLine("DroidLinks (DogeHelper) v{0}\n\n", typeof(Program).Assembly.GetName().Version);

            if (args.Length < 1)
            {
                Console.WriteLine("Invalid usage.\n" +
                    "Usage: [exec name] {Discord Bot Token} [Prefix]\n" +
                    "You must supply Discord Bot Token as the first argument to the program.");
                Environment.Exit(1);
            }

            //new MainAsync(args).ConfigureAwait(false).GetAwaiter().GetResult();
            var prog = new Program();
            prog.MainAsync(args).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public async Task MainAsync(string[] args)
        {
            // Load in token from arguments.
            Globals.BotToken = args[0];
            Console.WriteLine("Token: {0}", Globals.BotToken);

            if (args.Length > 1)
            {
                // Set the bot prefix
                Globals.BotPrefix = args[1];
                Console.WriteLine("Prefix: {0}", Globals.BotPrefix);
            }

            Console.Write("\n");

            // Initalise the bot
            Discord = new DiscordClient(new DiscordConfiguration
            {
                Token = Globals.BotToken,
                TokenType = TokenType.Bot,
                UseInternalLogHandler = true,
                LogLevel = LogLevel.Info
            });

            // Initalise User Interactivity
            Discord.UseInteractivity(new InteractivityConfiguration
            {
                // default pagination behaviour to just ignore the reactions
                PaginationBehaviour = TimeoutBehaviour.Ignore,
                // default pagination timeout to 5 minutes
                PaginationTimeout = TimeSpan.FromMinutes(5),
                // default timeout for other actions to 2 minutes
                Timeout = TimeSpan.FromMinutes(2)
            });



            // Initalise commands handler
            Commands = Discord.UseCommandsNext(new CommandsNextConfiguration
            {
                StringPrefix = Globals.BotPrefix,
                EnableMentionPrefix = true
            });

            // Register commands
            Commands.RegisterCommands<Tools>();
            Commands.RegisterCommands<Nexus6PCommands>();
            Commands.RegisterCommands<MagiskCommands>();
            Commands.RegisterCommands<GoogleCommands>();
            Commands.RegisterCommands<LinkCommands>();

            // Register Commands Hooks
            Commands.CommandErrored += Commands_CommandErrored;

            // Subscribe events
            Discord.MessageCreated += Discord_MessageCreated;
            Discord.GuildMemberRemoved += Discord_GuildMemberRemoved;
            Discord.GuildAvailable += Discord_GuildAvailable;
            Discord.Ready += Discord_Ready;

            await Discord.ConnectAsync();

            // Indefinite delay
            await Task.Delay(-1);
        }

        private async Task Commands_CommandErrored(CommandErrorEventArgs e)
        {
            // let's log the error details
            e.Context.Client.DebugLogger.LogMessage(LogLevel.Error, "DogeHelper", $"{e.Context.User.Username} tried executing '{e.Command?.QualifiedName ?? "<unknown command>"}' but it errored: {e.Exception.GetType()}: {e.Exception.Message ?? "<no message>"}", DateTime.Now);

            // let's check if the error is a result of lack
            // of required permissions
            if (e.Exception is ChecksFailedException ex)
            {
                // yes, the user lacks required permissions, 
                // let them know

                var emoji = DiscordEmoji.FromName(e.Context.Client, ":no_entry:");

                // let's wrap the response into an embed
                var embed = new DiscordEmbedBuilder
                {
                    Title = "Access denied",
                    Description = $"{emoji} You do not have the permissions required to execute this command.",
                    Color = new DiscordColor(0xFF0000) // red
                };
                await e.Context.RespondAsync("", embed: embed);
            }
        }

        private async Task Discord_GuildMemberRemoved(DSharpPlus.EventArgs.GuildMemberRemoveEventArgs e)
        {
            await Discord.SendMessageAsync(Discord.GetChannelAsync(519817442881437714).Result, $"Goodbye {e.Member.Username.ToString()} {DiscordEmoji.FromName(Discord, ":wave:")} ");
        }

        private Task Discord_Ready(DSharpPlus.EventArgs.ReadyEventArgs e)
        {
            // Update game state when the bot is ready
            Discord.UpdateStatusAsync(new DiscordGame("Serving Links"), null, null);

            return Task.CompletedTask;
        }

        private static Task Discord_GuildAvailable(DSharpPlus.EventArgs.GuildCreateEventArgs e)
        {
            Console.WriteLine($"\n[INFO] {DateTimeOffset.Now.DateTime}: Connected to {e.Guild.Name}, owned by {e.Guild.Owner.Username}.");

            return Task.CompletedTask;
        }

        private async Task Discord_MessageCreated(DSharpPlus.EventArgs.MessageCreateEventArgs e)
        {
            // Crying channel only
            if (e.Channel.Id.Equals(538197625309233182))
            {
                await e.Message.CreateReactionAsync(DiscordEmoji.FromName(Discord, ":cry:"));
            }
        }
    }
}
