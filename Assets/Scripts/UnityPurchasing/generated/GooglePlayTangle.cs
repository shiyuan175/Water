// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security
{
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("YeLs49Nh4unhYeLi40fkRWszhrTQFrKVI8dvlLBrWJximc8qTmTvTFm/q1rXV3aMAiJYXHMIa694yLp1GuNQrsPnC+a2l6KFYU4iGN8s55gE1rYC/uTca5kYUGTPlpo81sWpPoNFcsanEbukxOYK5FHyQqpYqEhR02HiwdPu5erJZatlFO7i4uLm4+BURi/smTW94wviE0/OFTVU35LgJ/3UL3f0fwzk1XSI0dBWX4YvLvO3CpdVut5vDFvIXqBwYvdrKCw2c4GSIrSRQ/Y38jc3q0FMse7zBqs4qvTWF0+bR0VsQyCuAGPAYtKkBi1oSkA02v7egVoc9qIdsm37HSppddIHfgtQYcut6pa0bigbLkOkTHlzXd6Ln8PQ+6qcJuHg4uPi");
        private static int[] order = new int[] { 1, 8, 2, 9, 13, 10, 8, 12, 10, 10, 10, 13, 12, 13, 14 };
        private static int key = 227;

        public static readonly bool IsPopulated = true;

        public static byte[] Data()
        {
            if (IsPopulated == false)
                return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
