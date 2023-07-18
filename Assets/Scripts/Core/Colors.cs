using UnityEngine;

namespace CoolGameClub.Core
{
    public static class Colors
    {
        public static Color RandomColor() {
            return (Color)Random.Range(0, System.Enum.GetValues(typeof(Color)).Length);
        }

        public enum Color
        {
            Red, Green, Blue, Purple
        }
    }
}
