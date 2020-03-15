using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class TestHelper
{
    public static IEnumerator LoadMovementTestScene()
    {
        var operation = SceneManager.LoadSceneAsync("MovementTests");
        while (!operation.isDone)
        {
            yield return null;
        }
    }

    public static Player GetPlayer()
    {
        return GameObject.FindObjectOfType<Player>();
    }

    public static Camera GetPlayerCamera(Player player)
    {
        var playerLook = player.GetComponent<PlayerLook>();
        return playerLook.PlayerCamera;
    }

    public static float CalculateHorizontalTurn(Quaternion originalRotation, Quaternion transformRotation)
    {
        Vector3 cross = CalculateTurn(originalRotation, transformRotation);
        return cross.y;
    }

    public static float CalculateVerticalTurn(Quaternion originalRotation, Quaternion transformRotation)
    {
        Vector3 cross = CalculateTurn(originalRotation, transformRotation);
        return cross.x;
    }

    private static Vector3 CalculateTurn(Quaternion originalRotation, Quaternion transformRotation)
    {
        Vector3 cross = Vector3.Cross(originalRotation * Vector3.forward, transformRotation * Vector3.forward);
        return cross;
    }
}
