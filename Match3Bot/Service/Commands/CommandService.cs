using System.Collections.Generic;

namespace Match3Bot.Service.Commands
{
    class CommandService: ICommandService
    {
        private readonly List<СombinationCommand> _commands;

        public CommandService()
        {
            _commands = new List<СombinationCommand>
            {
                //new BetweenTwoFiguresCommand(),
                new LeftLightFigure()
            };
        }

        public List<СombinationCommand> GetCommands() => _commands;
    }
}
