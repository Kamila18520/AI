using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public BotController[] botController;

    public void MoveBot()
    {
        foreach (var bot in botController)
        {
            bot.MoveToNextCheckpoint();

        }
    }
}

