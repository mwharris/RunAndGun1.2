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
}
