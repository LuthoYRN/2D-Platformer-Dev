using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
public class WeaponWheelController : MonoBehaviour
{
    private Animator anim;
    private bool weaponWheelSelected = false;
    public static bool isWeaponWheelOpen = false;
    [SerializeField] WeaponWheelButtonController[] buttons;
    public static WeaponWheelController instance;
    public Image selectedItem;
    public static int weaponID = 1;
    [SerializeField]private int lastSelectedWeaponID = 1;
    [SerializeField]private Sprite arrowIconSprite;
    [SerializeField]private Sprite fireIconSprite;
    [SerializeField]private Sprite iceIconSprite;
    [SerializeField]private Sprite gravityIconSprite;
    public GameObject player;

    private void Awake()
    {   
        instance = this;
        anim = GetComponent<Animator>();   
        LoadUnlockedPowers();
        weaponID = PlayerPrefs.GetInt("LastWeaponID", 1);
        lastSelectedWeaponID = weaponID;
    }
    private void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            weaponWheelSelected = !weaponWheelSelected;
            isWeaponWheelOpen = weaponWheelSelected;
        }
        if (weaponWheelSelected)
        {
            
            anim.SetBool("OpenWeaponWheel",true);
        }else{
            
            anim.SetBool("OpenWeaponWheel",false);
        }
        switch (weaponID)
        {
            case 1:            
                player.GetComponent<Animator>().SetBool("power",false);
                break;
            case 2:
                player.GetComponent<Animator>().SetBool("power",true);
                break;
            case 3:
                player.GetComponent<Animator>().SetBool("power",true);
                break;
            case 4:
                player.GetComponent<Animator>().SetBool("power",true);
                break;
        }
        if (weaponID != lastSelectedWeaponID)
        {
            lastSelectedWeaponID = weaponID;
            PlayerPrefs.SetInt("LastWeaponID", weaponID);
            PlayerPrefs.Save();
        }
        UpdateSelectedItem();
    }
    private void UpdateSelectedItem()
    {
        switch (lastSelectedWeaponID)
        {
            case 1:
                selectedItem.sprite = arrowIconSprite;
                break;
            case 2:
                selectedItem.sprite = fireIconSprite;
                break;
            case 3:
                selectedItem.sprite = iceIconSprite;
                break;
            case 4:
                selectedItem.sprite = gravityIconSprite;
                break;
            default:
                selectedItem.sprite = null;
                break;
        }
    }
    public void UnlockButton(string attackType){
        foreach (var btn in buttons)
        {
            if ((attackType=="F" && btn.ID==2) || (attackType=="I" && btn.ID==3) || (attackType=="G" && btn.ID==4))
            {
                btn.Unlock();
            }
        }
    }

    private void LoadUnlockedPowers()
{
    if (PlayerPrefs.GetInt("PowerUnlocked_F", 0) == 1)
        UnlockButton("F");
    if (PlayerPrefs.GetInt("PowerUnlocked_I", 0) == 1)
        UnlockButton("I");
    if (PlayerPrefs.GetInt("PowerUnlocked_G", 0) == 1)
        UnlockButton("G");
}
}
