namespace BombParty.Common
{
    public class Player
    {
        public string Id { get; set; }
        public PlayerSettings Settings { get; set; }

        public string DisplayName => Settings.UserName ?? Id;

        public string Input { get; set; } = string.Empty;

        public int HealthPoints { get; set; }
        public bool Alive => HealthPoints > 0;

        public Player(string id)
        {
            Id = id;
        }

        public void Damage()
        {
            if (HealthPoints > 0)
                HealthPoints--;
        }
    }
}
