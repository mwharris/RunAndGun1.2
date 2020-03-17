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
        public void Setup()
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
            PlayerInput.Instance.VerticalHeld.Returns(true);

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
            PlayerInput.Instance.VerticalHeld.Returns(true);

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
            PlayerInput.Instance.HorizontalHeld.Returns(true);

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
            PlayerInput.Instance.HorizontalHeld.Returns(true);

            float startXPos = player.transform.position.x;
            yield return new WaitForSeconds(0.5f);
            float endXPos = player.transform.position.x;
            
            Assert.Greater(startXPos, endXPos);
        }
    }

    public class with_positive_mouse_x : player_input_test
    {
        [UnityTest]
        public IEnumerator turns_right()
        {
            yield return TestHelper.LoadMovementTestScene();
            var player = TestHelper.GetPlayer();

            PlayerInput.Instance.MouseX.Returns(1f);

            var originalRotation = player.transform.rotation;
            yield return new WaitForSeconds(0.5f);

            float turnAmount = TestHelper.CalculateHorizontalTurn(originalRotation, player.transform.rotation);
            
            // Positive value means we rotated right
            Assert.Greater(turnAmount, 0);
        }
    }
    
    public class with_negative_mouse_x : player_input_test
    {
        [UnityTest]
        public IEnumerator turns_left()
        {
            yield return TestHelper.LoadMovementTestScene();
            var player = TestHelper.GetPlayer();

            PlayerInput.Instance.MouseX.Returns(-1f);

            var originalRotation = player.transform.rotation;
            yield return new WaitForSeconds(0.5f);

            float turnAmount = TestHelper.CalculateHorizontalTurn(originalRotation, player.transform.rotation);
            
            // Negative value means we rotated left
            Assert.Less(turnAmount, 0);
        }
    }
    
    public class with_positive_mouse_y : player_input_test
    {
        [UnityTest]
        public IEnumerator turns_upwards()
        {
            yield return TestHelper.LoadMovementTestScene();
            var player = TestHelper.GetPlayer();
            var playerCamera = TestHelper.GetPlayerCamera(player);

            PlayerInput.Instance.MouseY.Returns(1f);

            var originalRotation = player.transform.rotation;
            yield return new WaitForSeconds(0.5f);

            float turnAmount = TestHelper.CalculateVerticalTurn(originalRotation, playerCamera.transform.rotation);
            
            // Negative value means we rotated upwards
            Assert.Less(turnAmount, 0);
        }
    }
    
    public class with_negative_mouse_y : player_input_test
    {
        [UnityTest]
        public IEnumerator turns_downwards()
        {
            yield return TestHelper.LoadMovementTestScene();
            var player = TestHelper.GetPlayer();
            var playerCamera = TestHelper.GetPlayerCamera(player);

            PlayerInput.Instance.MouseY.Returns(-1f);

            var originalRotation = player.transform.rotation;
            yield return new WaitForSeconds(0.5f);

            float turnAmount = TestHelper.CalculateVerticalTurn(originalRotation, playerCamera.transform.rotation);
            
            // Positive value means we rotated downwards
            Assert.Greater(turnAmount, 0);
        }
    }
}