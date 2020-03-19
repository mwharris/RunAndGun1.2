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
        
        private IEnumerator MoveToWalking(PlayerMovementStateMachine stateMachine)
        {
            PlayerInput.Instance.Vertical.Returns(1f);
            PlayerInput.Instance.VerticalHeld.Returns(true);
            yield return new WaitForSeconds(0.1f);
            Assert.AreEqual(typeof(Walking), stateMachine.CurrentStateType);
        }

        private IEnumerator MoveFromWalkingToSprinting(PlayerMovementStateMachine stateMachine)
        {
            PlayerInput.Instance.ShiftDown.Returns(true);
            yield return null;
            PlayerInput.Instance.ShiftDown.Returns(false);
            yield return new WaitForSeconds(0.2f);
            Assert.AreEqual(typeof(Sprinting), stateMachine.CurrentStateType);
        }

        private IEnumerator MoveToJumping(PlayerMovementStateMachine stateMachine)
        {
            PlayerInput.Instance.SpaceDown.Returns(true);
            yield return null;
            PlayerInput.Instance.SpaceDown.Returns(false);
            Assert.AreEqual(typeof(Jumping), stateMachine.CurrentStateType);
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

            // Idle -> Walking
            yield return MoveToWalking(stateMachine);

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

            // Idle -> Walking -> Sprinting
            yield return MoveToWalking(stateMachine);
            yield return MoveFromWalkingToSprinting(stateMachine);
            
            Assert.AreEqual(typeof(Sprinting), stateMachine.CurrentStateType);
        }
        
        [UnityTest]
        public IEnumerator switches_from_sprinting_to_walking_when_shift_pressed()
        {
            yield return TestHelper.LoadMovementTestScene();
            var player = TestHelper.GetPlayer();
            var stateMachine = TestHelper.GetPlayerMovementStateMachine(player);

            Assert.AreEqual(typeof(Idle), stateMachine.CurrentStateType);

            // Idle -> Walking -> Sprinting
            yield return MoveToWalking(stateMachine);
            yield return MoveFromWalkingToSprinting(stateMachine);
            
            // Test that we move back to Walking when shift is pressed while Sprinting
            PlayerInput.Instance.ShiftDown.Returns(true);
            yield return null;
            PlayerInput.Instance.ShiftDown.Returns(false);
            yield return new WaitForSeconds(0.2f);
            Assert.AreEqual(typeof(Walking), stateMachine.CurrentStateType);
        }

        [UnityTest]
        public IEnumerator switches_from_idle_to_jumping_when_jump_pressed()
        {
            yield return TestHelper.LoadMovementTestScene();
            var player = TestHelper.GetPlayer();
            var stateMachine = TestHelper.GetPlayerMovementStateMachine(player);

            Assert.AreEqual(typeof(Idle), stateMachine.CurrentStateType);

            yield return MoveToJumping(stateMachine);

            Assert.AreEqual(typeof(Jumping), stateMachine.CurrentStateType);
        }
        
        [UnityTest]
        public IEnumerator switches_from_walking_to_jumping_when_jump_pressed()
        {
            yield return TestHelper.LoadMovementTestScene();
            var player = TestHelper.GetPlayer();
            var stateMachine = TestHelper.GetPlayerMovementStateMachine(player);

            Assert.AreEqual(typeof(Idle), stateMachine.CurrentStateType);

            // Idle -> Walking -> Jumping
            yield return MoveToWalking(stateMachine);
            yield return MoveToJumping(stateMachine);

            Assert.AreEqual(typeof(Jumping), stateMachine.CurrentStateType);
        }
        
        [UnityTest]
        public IEnumerator switches_from_jumping_to_idle_when_landing()
        {
            yield return TestHelper.LoadMovementTestScene();
            var player = TestHelper.GetPlayer();
            var stateMachine = TestHelper.GetPlayerMovementStateMachine(player);

            Assert.AreEqual(typeof(Idle), stateMachine.CurrentStateType);
            
            // Idle -> Jumping
            yield return MoveToJumping(stateMachine);

            // Wait until we land
            float t = Time.time;
            yield return new WaitUntil(() => stateMachine.CurrentStateType == typeof(Idle) || Time.time > t + 5);

            Assert.AreEqual(typeof(Idle), stateMachine.CurrentStateType);
            Assert.AreEqual(true, stateMachine.IsGrounded);
        }
        
        [UnityTest]
        public IEnumerator switches_from_jumping_to_walking_when_landing()
        {
            yield return TestHelper.LoadMovementTestScene();
            var player = TestHelper.GetPlayer();
            var stateMachine = TestHelper.GetPlayerMovementStateMachine(player);

            Assert.AreEqual(typeof(Idle), stateMachine.CurrentStateType);

            // Idle -> Walking -> Jumping
            yield return MoveToWalking(stateMachine);
            yield return MoveToJumping(stateMachine);

            // Wait until we land
            float t = Time.time;
            yield return new WaitUntil(() => stateMachine.CurrentStateType == typeof(Walking) || Time.time > t + 5);

            Assert.AreEqual(typeof(Walking), stateMachine.CurrentStateType);
            Assert.AreEqual(true, stateMachine.IsGrounded);
        }
        
        [UnityTest]
        public IEnumerator preserves_sprinting_through_jumping()
        {
            yield return TestHelper.LoadMovementTestScene();
            var player = TestHelper.GetPlayer();
            var stateMachine = TestHelper.GetPlayerMovementStateMachine(player);

            Assert.AreEqual(typeof(Idle), stateMachine.CurrentStateType);

            // Idle -> Walking -> Sprinting -> Jumping
            yield return MoveToWalking(stateMachine);
            yield return MoveFromWalkingToSprinting(stateMachine);
            yield return MoveToJumping(stateMachine);

            // Wait until we land
            float t = Time.time;
            yield return new WaitUntil(() => stateMachine.CurrentStateType == typeof(Sprinting) || Time.time > t + 5);

            Assert.AreEqual(typeof(Sprinting), stateMachine.CurrentStateType);
            Assert.AreEqual(true, stateMachine.IsGrounded);
        }
    }
}