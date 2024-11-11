namespace BombParty.Common
{
    public class Avatar
    {
        public Avatar(int id, string fileName)
        {
            Id = id;
            FileName = fileName;
        }

        public int Id { get; }
        public string FileName { get; }
    }

    public static class Avatars
    {
        public static int AvailableAvatarCount = 8;

        public static IList<Avatar> AvailableAvatars { get; private set; } = new List<Avatar>();

        static Avatars()
        {
            foreach (var i in Enumerable.Range(1, AvailableAvatarCount))
            {
                AvailableAvatars.Add(new Avatar(i, $"Images\\avatar-{i}.jpg"));
            }
        }

        public static Avatar GetAvatar(int id) => AvailableAvatars.FirstOrDefault(a => a.Id == id) ?? AvailableAvatars[0];
    }
}
