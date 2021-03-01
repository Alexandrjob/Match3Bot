using System.Collections.Generic;

namespace Match3Bot
{
    public interface ICommandService
    {
        List<СombinationCommand> GetCommands();
    }
}