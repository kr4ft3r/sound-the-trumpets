using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UpgradeOption : MonoBehaviour
{
    public System.Action<UpgradeOption, IRegimentUpgrade> RegimentUpgradeSelected;

    public int Key;
    public string Name;
    public string Description;
    public IRegimentUpgrade RegimentUpgrade;
    public KeyCode KeyCode;
    TextMeshPro NameText;
    TextMeshPro DescriptionText;
    TextMeshPro KeyText;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(this.KeyCode))
        {

        }
    }

    public void Configure(int key, IRegimentUpgrade upgrade)
    {
        NameText = transform.Find("Name").GetComponent<TextMeshPro>();
        DescriptionText = transform.Find("Description").GetComponent<TextMeshPro>();
        KeyText = transform.Find("Key").GetComponent<TextMeshPro>();

        Key = key;
        Name = upgrade.GetName();
        Description = upgrade.GetDescription(true);
        this.KeyCode = KeyCode.Alpha0 + Key;

        NameText.text = upgrade.GetName();
        DescriptionText.text = upgrade.GetDescription(true);
        KeyText.text = Key.ToString();
    }
}
