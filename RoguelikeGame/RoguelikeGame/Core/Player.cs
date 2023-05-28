using RLNET;
namespace RoguelikeGame.Core
{
    public class Player : Actor
    {
        public Player()
        {
            Name = "Rogue";
            Awareness = 15;         
            Color = Colors.Player;
            Symbol = '@';

            Health = 200;
            MaxHealth = 200;

            Attack = 2;
            AttackChance = 50;

            Defense = 2;
            DefenseChance = 40;

            Gold = 0;
            Speed = 14;
        }

        public void DrawStats(RLConsole statConsole)
        {
            statConsole.Print(1, 1, $"Name:    {Name}", Colors.Text);
            statConsole.Print(1, 3, $"Health:  {Health}/{MaxHealth}", Colors.Text);
            statConsole.Print(1, 5, $"Attack:  {Attack} ({AttackChance}%)", Colors.Text);
            statConsole.Print(1, 7, $"Defense: {Defense} ({DefenseChance}%)", Colors.Text);
            statConsole.Print(1, 9, $"Gold:    {Gold}", Colors.Text);
        }
    }
}
