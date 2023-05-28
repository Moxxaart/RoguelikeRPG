using RogueSharp.DiceNotation;
using RoguelikeGame.Core;

namespace RoguelikeGame.Monsters
{
    public class Kobold : Monster
    {
        public static Kobold Create(int level)
        {
            int health = Dice.Roll("1D10");
            return new Kobold
            {
                Name = "Kobold",
                Symbol = 'K',
                Color = Colors.KoboldColor,
                Awareness = 6,

                Health = health,
                MaxHealth = health,

                Attack = Dice.Roll("1D3") + level / 3,
                AttackChance = Dice.Roll("10D3"),
                Defense = Dice.Roll("1D3") + level / 3,
                DefenseChance = Dice.Roll("6D3"),

                Gold = Dice.Roll("3D5"),
                Speed = 10
            };
        }
    }
}
