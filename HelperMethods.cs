using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace ChaosEdition
{
    public static class HelperMethods
    {
        /// <summary>
        /// returns Vector2.Zero if none are found
        /// </summary>
        public static Vector2 NearestPlayerCenter(Vector2 targetPos)
        {
            Vector2 closest = Vector2.Zero;
            float lastDistance = 1000000;
            foreach(Player player in Main.player)
            {
                if (!player.active || player.DeadOrGhost)
                    continue;

                float distance = player.DistanceSQ(targetPos);
                if (distance < lastDistance)
                {
                    lastDistance = distance;
                    closest = player.Center;
                }
            }

            return closest;
        }

        public static void DrawStar(Vector2 position, int dustType, float pointAmount = 5, float mainSize = 1, float dustDensity = 1, float dustSize = 1f, float pointDepthMult = 1f, float pointDepthMultOffset = 0.5f, float randomAmount = 0, float rotationAmount = -1)
        {
            float rot;
            if (rotationAmount < 0) { rot = Main.rand.NextFloat(0, (float)Math.PI * 2); }
            else { rot = rotationAmount; }

            float density = 1 / dustDensity * 0.1f;

            for (float k = 0; k < 6.28f; k += density)
            {
                float rand = 0;
                if (randomAmount > 0) { rand = Main.rand.NextFloat(-0.01f, 0.01f) * randomAmount; }

                float x = (float)Math.Cos(k + rand);
                float y = (float)Math.Sin(k + rand);
                float mult = ((Math.Abs(((k * (pointAmount / 2)) % (float)Math.PI) - (float)Math.PI / 2)) * pointDepthMult) + pointDepthMultOffset;//triangle wave function
                Dust.NewDustPerfect(position, dustType, new Vector2(x, y).RotatedBy(rot) * mult * mainSize, 0, default, dustSize);
            }
        }
    }

    public static class RandomExtensions
    {
        public static void Shuffle<T>(this T[] array)
        {
            int n = array.Length;
            while (n > 1)
            {
                int k = Main.rand.Next(n--);
                T temp = array[n];
                array[n] = array[k];
                array[k] = temp;
            }
        }

        public static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = Main.rand.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }
}
