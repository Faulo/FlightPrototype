using NUnit.Framework;
using TheCursedBroom.Extensions;
using UnityEngine;

namespace TheCursedBroom.Tests.PlayMode {
    public class Vector2ExtensionsTests {
        public class SumExample {
            public Vector2[] input;
            public Vector2 output;

            public override string ToString() {
                return $"{string.Join("+", input)} = {output}";
            }
        }
        static SumExample[] sumExamples = new[] {
            new SumExample {
                input = new[] {Vector2.zero },
                output = Vector2.zero,
            },
            new SumExample {
                input = new[] {Vector2.up , Vector2.right },
                output = Vector2.one,
            },
        };
        [Test]
        public void TestSum([ValueSource(nameof(sumExamples))] SumExample example) {
            var result = example.input.Sum();

            Assert.AreEqual(example.output, result);
        }
    }
}