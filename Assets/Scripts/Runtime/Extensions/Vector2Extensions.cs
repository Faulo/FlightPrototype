using System.Collections.Generic;
using UnityEngine;

namespace TheCursedBroom.Extensions {
    public static class Vector2Extensions {
        public static Vector2 Sum(this IEnumerable<Vector2> summands) {
            var result = Vector2.zero;
            foreach (var summand in summands) {
                result += summand;
            }
            return result;
        }
    }
}