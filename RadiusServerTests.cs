using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text;
using Flexinets.Radius;
using System.Net;
using System.IO;

namespace RadiusServerTests
{
    [TestClass]
    public class RadiusServerTests
    {
        /// <summary>
        /// Example from https://tools.ietf.org/html/rfc2865
        /// </summary>
        [TestMethod]
        public void TestResponsePacket()
        {
            var request = "010000380f403f9473978057bd83d5cb98f4227a01066e656d6f02120dbe708d93d413ce3196e43f782a0aee0406c0a80110050600000003";
            var expected = "0200002686fe220e7624ba2a1005f6bf9b55e0b20606000000010f06000000000e06c0a80103";
            var secret = "xyzzy5461";

            var path = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\dictionary";  // todo hurgh
            var dictionary = new RadiusDictionary(path);

            var rs = new RadiusServer(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 1812), dictionary);
            var response = rs.GetResponsePacket(new MockPacketHandler(), secret, StringToByteArray(request), new IPEndPoint(IPAddress.Parse("127.0.0.1"), 1813));
            var responseBytes = rs.GetBytes(response);

            Assert.AreEqual(expected, ByteArrayToString(responseBytes));
        }



        public byte[] StringToByteArray(String hex)
        {
            int NumberChars = hex.Length;
            byte[] bytes = new byte[NumberChars / 2];
            for (int i = 0; i < NumberChars; i += 2)
            {
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            }
            return bytes;
        }

        public string ByteArrayToString(byte[] bytes)
        {
            var hex = new StringBuilder(bytes.Length * 2);
            foreach (byte b in bytes)
            {
                hex.AppendFormat("{0:x2}", b);
            }
            return hex.ToString();
        }
    }
}