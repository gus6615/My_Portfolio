// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("JwRcOwEihSQXbhFNx+meHbLXF2r3dHp1Rfd0f3f3dHR17kpXg7sbPp2qQYHCc3UJSLC6zWGXkWXjqfxaaQ9ireOftyuANn8hyripyvFF5335aKlxGfDI98xtpPmdGK7mjbEJL4S9/l58N8ObfJXWB3QIRxCp/LkKVzdG6/6VBEegwUM9DGh7DDcahGlF93RXRXhzfF/zPfOCeHR0dHB1dmweIavwMhWRekihCz/VrQ7u+1+Ux9vxKisTC/EXhwYhza2XND5KY8eTC0HsV0ORc99wbz4gcYQ58R0xCA6ckzBFLrfiYimE6cyizeUiysqYq6zC41sAZ1Zy/7GvGBDLhkXE5HvMv36dLSrAiRe0Zp+5IoTRdnBSkrO8rUDMY6gyzHd2dHV0");
        private static int[] order = new int[] { 13,1,12,8,8,12,7,13,13,12,12,11,13,13,14 };
        private static int key = 117;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
