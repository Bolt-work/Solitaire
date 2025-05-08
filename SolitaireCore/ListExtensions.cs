namespace SolitaireCore
{
    public static class ListExtensions
    {
        // Fisher-Yates shuffle algorithm
        public static void Shuffle<T>(this List<T> list)
        {
            Random rand = new Random();
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rand.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }
}
