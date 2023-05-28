using RoguelikeGame.Systems;
using RoguelikeGame.Core;

namespace RoguelikeGame.Interfaces
{
    public interface IBehavior
    {
        bool Act(Monster monster, CommandSystem commandSystem);
    }
}
