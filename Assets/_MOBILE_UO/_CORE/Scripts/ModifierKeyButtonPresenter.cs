//MODIFIED BY DX4D
using UnityEngine;
using UnityEngine.UI;

public class ModifierKeyButtonPresenter : MonoBehaviour
{
	[Header("COMPONENT LINKS")]
    [SerializeField] private Button button;
    [SerializeField] private Image image;

	[Header("IMAGE COLOR")]
	[SerializeField] Color activeColor = new Color(0.25f, 0.5f, 0f, 1f);
	[SerializeField] Color inactiveColor = Color.black;
    
    public bool ToggledOn { get; private set; }

    private void Awake()
    {
        button.onClick.AddListener(OnButtonClicked);
    }
    
    private void OnButtonClicked()
    {
        ToggledOn = !ToggledOn;
        image.color = ToggledOn ? activeColor : inactiveColor;
    }
}
