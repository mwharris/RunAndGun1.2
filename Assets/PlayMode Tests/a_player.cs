using System.Collections;
using NSubstitute;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace a_player
{
    public class player_input_test
    {
        [SetUp]
        public void setup()
        {
            PlayerInput.Instance = Substitute.For<IPlayerInput>();
        }
    }
    
    public class with_positive_vertical_input : player_input_test
    {
        [UnityTest]
        public IEnumerator moves_forward()
        {
            yield return TestHelper.LoadMovementTestScene();
            var player = TestHelper.GetPlayer();

            PlayerInput.Instance.Vertical.Returns(1f);

            float startZPos = player.transform.position.z;
            yield return new WaitForSeconds(0.5f);
            float endZPos = player.transform.position.z;
            
            Assert.Greater(endZPos, startZPos);
        }
    }
    
    public class with_negative_vertical_input : player_input_test
    {
        [UnityTest]
        public IEnumerator moves_backward()
        {
            yield return TestHelper.LoadMovementTestScene();
            var player = TestHelper.GetPlayer();

            PlayerInput.Instance.Vertical.Returns(-1f);

            float startZPos = player.transform.position.z;
            yield return new WaitForSeconds(0.5f);
            float endZPos = player.transform.position.z;
            
            Assert.Greater(startZPos, endZPos);
        }
    }
    
    public class with_positive_horizontal_input : player_input_test
    {
        [UnityTest]
        public IEnumerator moves_right()
        {
            yield return TestHelper.LoadMovementTestScene();
            var player = TestHelper.GetPlayer();

            PlayerInput.Instance.Horizontal.Returns(1f);

            float startXPos = player.transform.position.x;
            yield return new WaitForSeconds(0.5f);
            float endXPos = player.transform.position.x;
            
            Assert.Greater(endXPos, startXPos);
        }
    }
    
    public class with_negative_horizontal_input : player_input_test
    {
        [UnityTest]
        public IEnumerator moves_backward()
        {
            yield return TestHelper.LoadMovementTestScene();
            var player = TestHelper.GetPlayer();

            PlayerInput.Instance.Horizontal.Returns(-1f);

            float startXPos = player.transform.position.x;
            yield return new WaitForSeconds(0.5f);
            float endXPos = player.transform.position.x;
            
            Assert.Greater(startXPos, endXPos);
        }
    }
}
