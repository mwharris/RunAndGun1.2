using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementStateMachineHelper
{
    private bool _fixInitialNotGrounded = true;
    
    public void createStateTransitions(BaseStateMachine stateMachine, IStateParams stateParams, bool isWallRunning,
        bool preserveSprint, Idle idle, Walking walking, Sprinting sprinting, Jumping jumping, WallRunning wallRunning,
        Crouching crouching, Sliding sliding)
    {
        // Any -> Idle
        stateMachine.AddAnyTransition(idle, () => ToIdle(idle, jumping, crouching, sliding));
        // Any -> Jumping
        stateMachine.AddAnyTransition(jumping, () => ToJump(jumping, isWallRunning, stateParams.WallJumped));

        // Idle -> Walking
        stateMachine.AddTransition(idle, walking, () => walking.IsWalking());
        // Walking -> Sprinting
        stateMachine.AddTransition(walking, sprinting, () => PlayerInput.Instance.ShiftDown);
        // Sprinting -> Walking
        stateMachine.AddTransition(sprinting, walking, () => !sprinting.IsStillSprinting());

        // Idle -> Crouching
        stateMachine.AddTransition(idle, crouching, () => PlayerInput.Instance.CrouchDown);
        // Walking -> Crouching
        stateMachine.AddTransition(walking, crouching, () => PlayerInput.Instance.CrouchDown);
        // Crouching -> Walking
        stateMachine.AddTransition(crouching, walking, () => CrouchToWalk(crouching, walking));
        // Crouching -> Sprinting
        stateMachine.AddTransition(crouching, sprinting, () => CrouchToSprint(crouching));
        // Sprinting -> Sliding (Crouching)
        stateMachine.AddTransition(sprinting, sliding, () => PlayerInput.Instance.CrouchDown);

        // Jumping -> Sliding
        stateMachine.AddTransition(jumping, sliding, () => JumpToSlide(jumping));
        // Jumping -> Sprinting
        stateMachine.AddTransition(jumping, sprinting, () => JumpToSprint(jumping, preserveSprint));
        // Jumping -> Walking
        stateMachine.AddTransition(jumping, walking, () => JumpToWalk(jumping, walking, preserveSprint));

        // Jumping -> Wall Running
        stateMachine.AddTransition(jumping, wallRunning, () => isWallRunning);
        // Wall Running -> Sprinting
        stateMachine.AddTransition(wallRunning, jumping, () => WallRunToSprint(jumping, isWallRunning, preserveSprint));
        // Wall Running -> Walking
        stateMachine.AddTransition(wallRunning, jumping, () => WallRunToWalk(jumping, walking, isWallRunning));
    }
    
    public bool ToIdle(Idle idle, Jumping jumping, Crouching crouching, Sliding sliding)
    {
        return idle.IsIdle()
               && !jumping.IsJumping()
               && (!crouching.IsCrouching && !crouching.Rising)
               && !sliding.IsSliding;
    }
    
    public bool ToJump(Jumping jumping, bool isWallRunning, bool isWallJumping)
    {
        bool wallJumped = !isWallRunning || isWallJumping;
        return jumping.IsJumping() 
               && wallJumped
               && !FixInitialNotGrounded();
    }
    
    private bool FixInitialNotGrounded()
    {
        if (_fixInitialNotGrounded)
        {
            _fixInitialNotGrounded = false;
            return true;
        } 
        return false;
    }

    public bool JumpToSlide(Jumping jumping)
    {
        return !jumping.IsJumping() && jumping.ToSlide;
    }
    
    public bool JumpToSprint(Jumping jumping, bool preserveSprint)
    {
        return !jumping.IsJumping() && !jumping.ToSlide && preserveSprint;
    }

    public bool JumpToWalk(Jumping jumping, Walking walking, bool preserveSprint)
    {
        return !jumping.IsJumping() && !jumping.ToSlide && !preserveSprint && walking.IsWalking();
    }

    public bool WallRunToSprint(Jumping jumping, bool isWallRunning, bool preserveSprint)
    {
        return !isWallRunning 
               && !jumping.IsJumping() 
               && preserveSprint;
    }

    public bool WallRunToWalk(Jumping jumping, Walking walking, bool isWallRunning)
    {
        return !isWallRunning 
               && !jumping.IsJumping() 
               && walking.IsWalking();
    }

    public bool CrouchToWalk(Crouching crouching, Walking walking)
    {
        return walking.IsWalking() && !crouching.ToSprint && !crouching.IsCrouching && !crouching.Rising;
    }

    public bool CrouchToSprint(Crouching crouching)
    {
        return crouching.ToSprint && !crouching.IsCrouching && !crouching.Rising;
    }
}
