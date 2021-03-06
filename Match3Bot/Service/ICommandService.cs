using System.Collections.Generic;

namespace Match3Bot.Service
{
    public interface ICommandService
    {
        List<СombinationCommand> GetCommands();
    }
}