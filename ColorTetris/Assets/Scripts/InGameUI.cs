using UnityEngine;
using UnityEngine.UI;

public class InGameUI : MonoBehaviour
{
    public Text count;
    public void ScoreSetup(int virusCount)
    {
        count.text = "Viruses:\n" + virusCount.ToString();
    }
    public void HideUI()
    {
        gameObject.SetActive(false);
    }
}
