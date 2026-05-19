using ItemChanger;
using ItemChanger.Internal;
using ItemChanger.Placements;
using ItemChanger.Util;
using UnityEngine;

namespace Architect.Behaviour.Utility;

public class CustomPickup : MonoBehaviour
{
    public string itemId = string.Empty;
    
    public static void Init()
    {
        
    }

    private void Start()
    {
        var item = Finder.GetItem(itemId);
        if (item == null) return;
        
        var shiny = Instantiate(ObjectCache.ShinyItem, transform);
        shiny.transform.localPosition = Vector3.zero;
        shiny.name = name + " Shiny";
        
        var fsm = shiny.LocateMyFSM("Shiny Control");
        ShinyUtility.DontFlingShiny(fsm);
        ShinyUtility.ModifyMultiShiny(fsm, FlingType.DirectDeposit, null!, [item]);

        shiny.GetComponent<Rigidbody2D>().gravityScale = 1;
        
        shiny.SetActive(true);
    }
}