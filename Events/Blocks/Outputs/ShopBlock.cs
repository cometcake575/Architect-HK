using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Architect.Content.Preloads;
using ItemChanger;
using UnityEngine;

namespace Architect.Events.Blocks.Outputs;

public class ShopBlock : CollectionBlock<ShopBlock.ShopItemBlock>
{
    protected override string Name => "Shop";

    public string Figurehead = "None";
    private static GameObject _shopPrefab;
    private static GameObject _shopItemPrefab;
    
    public static void Init()
    {
        PreloadManager.RegisterPreload(new BasicPreload("Room_shop", "Shop Menu",
            o =>
            {
                _shopPrefab = o;
                _shopItemPrefab = o.GetComponent<ShopMenuStock>().stock[0];
            }));
    }
    
    protected override IEnumerable<string> Inputs => ["Open"];

    protected override string ChildName => "Shop Item";
    protected override bool NeedsGap => true;

    private GameObject _shop;
    private ShopMenuStock _sms;
    private PlayMakerFSM _sFsm;

    public override void SetupReference()
    {
        _shop = Object.Instantiate(_shopPrefab);
        _sms = _shop.GetComponent<ShopMenuStock>();
        _sFsm = _shop.LocateMyFSM("shop_control");

        _sFsm.FsmVariables.FindFsmString("Figurehead Name").value = $"Figurehead {Figurehead}";

        var ccFsm = _shop.transform.Find("Confirm").Find("UI List").gameObject.LocateMyFSM("Confirm Control");
        var ci = _sFsm.FsmVariables.FindFsmInt("Current Item");
        ccFsm.GetState("Bob").AddAction(() =>
        {
            if (_sms.stock.Length <= ci.value) return;
            var si = _sms.stock[ci.value].GetComponent<ShopItemBlock.ShopItem>();
            if (!si) return;
            si.Block.Give();
            Refresh();
        }, 0);
    }
    
    protected override void Trigger(string trigger)
    {
        ArchitectPlugin.Instance.StartCoroutine(Coroutine());
    }

    private void Refresh()
    {
        _sms.stock = Children.Children.Where(c => c.GetVariable<bool>("Available", true))
            .Select(c => c.ItemPrefab).Where(i => i).ToArray();
        _sms.SpawnStock();
    }
    
    private IEnumerator Coroutine()
    {
        yield return HeroController.instance.FreeControl(_ => !GameManager.instance.isPaused);
        HeroController.instance.RelinquishControl();
        
        Refresh();

        if (!_sms.StockLeft()) yield break;
        
        _shop.gameObject.SetActive(true);
        _sFsm.SendEvent("SHOP UP");
        yield return new WaitUntil(() => _sFsm.ActiveStateName == "Idle");
        HeroController.instance.RegainControl();
    }
    
    public class ShopItemBlock : ChildBlock
    {
        public string ItemId = "Wayward_Compass";
        public string ItemName = string.Empty;
        public string ItemDesc = string.Empty;
        public int Cost = 80;
        public float ItemScale = 1;

        public GameObject ItemPrefab;

        public override void Reset()
        {
            ItemId = "Wayward_Compass";
            ItemName = string.Empty;
            ItemDesc = string.Empty;
            ItemScale = 1;
            Cost = 80;
        }

        private AbstractItem _item;

        public override void SetupReference()
        {
            ItemPrefab = null;

            var item = Finder.GetItem(ItemId);
            if (item == null) return;

            _item = item;
            
            ItemPrefab = Object.Instantiate(_shopItemPrefab);
            ItemPrefab.SetActive(false);
            var stats = ItemPrefab.GetComponent<ShopItemStats>();

            ItemPrefab.AddComponent<ShopItem>().Block = this;

            stats.priceConvo = $"ArchitectMod_{Cost}";
            stats.nameConvo = $"ArchitectMod_{ItemName}";
            stats.descConvo = $"ArchitectMod_{ItemDesc}";
            
            stats.playerDataBoolName = string.Empty;
            stats.requiredPlayerDataBool = string.Empty;
            stats.removalPlayerDataBool = string.Empty;
            stats.dungDiscount = false;
            stats.notchCostBool = string.Empty;
            
            stats.specialType = 0;
            stats.charmsRequired = 0;
            stats.relic = false;
            stats.relicNumber = 0;
            stats.relicPDInt = string.Empty;

            stats.itemSprite.transform.localScale = new Vector3(0.68f, 0.68f, 0.68f) * ItemScale;

            var sr = stats.itemSprite.GetComponent<SpriteRenderer>();
            var ud = item.GetResolvedUIDef();
            sr.sprite = ud != null ? ud.GetSprite() : item.GetPreviewSprite();
        }

        public void Give()
        {
            _item?.Give(null, new GiveInfo
            {
                MessageType = MessageType.Any
            });
            
            Event("OnPurchase");
        }
        
        public class ShopItem : MonoBehaviour
        {
            public ShopItemBlock Block;
        }

        protected override IEnumerable<(string, string)> InputVars => [("Available", "Boolean")];
        protected override IEnumerable<string> Outputs => ["OnPurchase"];
    }
}