using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;

namespace Tests
{
    public class GunTestScript
    {
        // A Test behaves as an ordinary method
        [Test]
        public void GunTestScriptSimplePasses()
        {
            // Use the Assert class to test conditions
        }

        // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
        // `yield return null;` to skip a frame.
        [UnityTest]
        public IEnumerator GunTestScriptWithEnumeratorPasses()
        {
            // Use the Assert class to test conditions.
            // Use yield to skip a frame.

            SceneManager.LoadScene("NetworkItemTest");

            GameObject networkGb = GameObject.Find("NETWORK");

            Assert.NotNull(networkGb);

            yield return null;
        }
    }
}
