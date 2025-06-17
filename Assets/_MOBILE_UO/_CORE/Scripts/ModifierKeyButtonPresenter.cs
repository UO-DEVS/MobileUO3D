using UnityEngine;
using UnityEngine.UI;

public class ModifierKeyButtonPresenter : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private Image image;

	private readonly Color activeColor = new Color(0.25f, 0.5f, 0f, 1f);
	private readonly Color inactiveColor = Color.black;
    
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