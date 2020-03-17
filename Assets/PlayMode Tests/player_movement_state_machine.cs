using System.Collections;
using System.Collections.Generic;
using NSubstitute;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace state_machine
{
    public class player_movement_state_machine
    {
        [SetUp]
        public void Setup()
        {
            PlayerInput.Instance = Substitute.For<IPlayerInput>();
        }
        
        [UnityTest]
        public IEnumerator starts_in_idle()
        {
            yield return TestHelper.LoadMovementTestScene();
            var player = TestHelper.GetPlayer();
            var stateMachine = TestHelper.GetPlayerMovementStateMachine(player);

            Assert.AreEqual(typeof(Idle), stateMachine.CurrentStateType);
        }
        
        [UnityTest]
        public IEnumerator switches_to_walking_when_horizontal_pressed()
        {
            yield return TestHelper.LoadMovementTestScene();
            var player = TestHelper.GetPlayer();
            var stateMachine = TestHelper.GetPlayerMovementStateMachine(player);

            Assert.AreEqual(typeof(Idle), stateMachine.CurrentStateType);

            PlayerInput.Instance.Horizontal.Returns(1f);
            PlayerInput.Instance.HorizontalHeld.Returns(true);
            
            yield return new WaitForSeconds(0.5f);
            
            Assert.AreEqual(typeof(Walking), stateMachine.CurrentStateType);
        }
        
        [UnityTest]
        public IEnumerator switches_to_walking_when_vertical_pressed()
        {
            yield return TestHelper.LoadMovementTestScene();
            var player = TestHelper.GetPlayer();
            var stateMachine = TestHelper.GetPlayerMovementStateMachine(player);

            Assert.AreEqual(typeof(Idle), stateMachine.CurrentStateType);

            PlayerInput.Instance.Vertical.Returns(1f);
            PlayerInput.Instance.VerticalHeld.Returns(true);
            
            yield return new WaitForSeconds(0.5f);
            
            Assert.AreEqual(typeof(Walking), stateMachine.CurrentStateType);
        }
        
        [UnityTest]
        public IEnumerator switches_to_idle_when_nothing_pressed()
        {
            yield return TestHelper.LoadMovementTestScene();
            var player = TestHelper.GetPlayer();
            var stateMachine = TestHelper.GetPlayerMovementStateMachine(player);

            Assert.AreEqual(typeof(Idle), stateMachine.CurrentStateType);

            // Test we move to Walking state
            PlayerInput.Instance.Vertical.Returns(1f);
            PlayerInput.Instance.VerticalHeld.Returns(true);
            yield return new WaitForSeconds(0.25f);
            Assert.AreEqual(typeof(Walking), stateMachine.CurrentStateType);

            // Test we move to Idle once buttons are released
            PlayerInput.Instance.Vertical.Returns(0f);
            PlayerInput.Instance.VerticalHeld.Returns(false);
            yield return new WaitForSeconds(0.25f);
            Assert.AreEqual(typeof(Idle), stateMachine.CurrentStateType);
        }
        
        [UnityTest]
        public IEnumerator switches_to_sprinting_when_shift_pressed()
        {
            yield return TestHelper.LoadMovementTestScene();
            var player = TestHelper.GetPlayer();
            var stateMachine = TestHelper.GetPlayerMovementStateMachine(player);

            Assert.AreEqual(typeof(Idle), stateMachine.CurrentStateType);

            // Test we move to Walking state
            PlayerInput.Instance.Vertical.Returns(1f);
            PlayerInput.Instance.VerticalHeld.Returns(true);
            yield return new WaitForSeconds(0.1f);
            Assert.AreEqual(typeof(Walking), stateMachine.CurrentStateType);

            // Test we move to Sprinting once shift is pressed
            PlayerInput.Instance.ShiftDown.Returns(true);
            yield return null;
            PlayerInput.Instance.ShiftDown.Returns(false);
            yield return new WaitForSeconds(0.5f);
            Assert.AreEqual(typeof(Sprinting), stateMachine.CurrentStateType);
        }
        
        [UnityTest]
        public IEnumerator switches_from_sprinting_to_walking_when_shift_pressed()
        {
            yield return TestHelper.LoadMovementTestScene();
            var player = TestHelper.GetPlayer();
            var stateMachine = TestHelper.GetPlayerMovementStateMachine(player);

            Assert.AreEqual(typeof(Idle), stateMachine.CurrentStateType);

            // Test we move to Walking state
            PlayerInput.Instance.Vertical.Returns(1f);
            PlayerInput.Instance.VerticalHeld.Returns(true);
            yield return new WaitForSeconds(0.2f);
            Assert.AreEqual(typeof(Walking), stateMachine.CurrentStateType);

            // Test we move to Sprinting once shift is pressed
            PlayerInput.Instance.ShiftDown.Returns(true);
            yield return null;
            PlayerInput.Instance.ShiftDown.Returns(false);
            yield return new WaitForSeconds(0.2f);
            Assert.AreEqual(typeof(Sprinting), stateMachine.CurrentStateType);
            
            // Test that we move back to Walking when shift is pressed while Sprinting
            PlayerInput.Instance.ShiftDown.Returns(true);
            yield return null;
            PlayerInput.Instance.ShiftDown.Returns(false);
            yield return new WaitForSeconds(0.2f);
            Assert.AreEqual(typeof(Walking), stateMachine.CurrentStateType);
        }
    }
}