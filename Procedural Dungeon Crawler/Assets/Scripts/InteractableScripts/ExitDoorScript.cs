using UnityEngine;
using GameManagerScripts;

namespace InteractableScripts
{
    public class ExitDoorScript : MonoBehaviour
    {
        public void Interact()
        {
            ScreenFadeManager.Instance.FadeNextFloor();
        }
    }
}