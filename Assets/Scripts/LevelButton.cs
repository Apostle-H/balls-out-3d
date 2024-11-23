using UnityEngine;
using UnityEngine.UI;

namespace DefaultNamespace
{
    public class LevelButton : MonoBehaviour
    {
        [field: SerializeField] public Image Image { get; private set; }
        [field: SerializeField] public Text Text { get; private set; }
        [field: SerializeField] public Button Button { get; private set; }
    }
}