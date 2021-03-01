using Match3Bot.Service.Commands;
using System.Collections.Generic;

namespace Match3Bot
{
    class CommandService: ICommandService
    {
        private readonly List<СombinationCommand> _commands;

        public CommandService()
        {
            _commands = new List<СombinationCommand>
            {
                new BetweenTwoFiguresCommand(),
            };
        }

        public List<СombinationCommand> GetCommands() => _commands;
    }
}
