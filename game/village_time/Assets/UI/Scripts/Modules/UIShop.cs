using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 继承UIBase，统一UI生命周期（Show/Hide/Init）
public class UIShop : UIBase
{
    [Header("商店UI配置")]
    [SerializeField] private Transform itemGrid; // 商品格子父节点（布局用）
    [SerializeField] private GameObject itemCellPrefab; // 商品格子预制体（星露风格像素格子）
    [SerializeField] private Text goldText; // 显示玩家金币的文本
    [SerializeField] private Button closeButton; // 关闭按钮
    [SerializeField] private AudioClip buySound; // 购买音效
    [SerializeField] private AudioClip sellSound; // 出售音效
    [SerializeField] private AudioClip errorSound; // 金币不足音效

    // 商店商品数据（星露风格：物品名/图标/价格/数量）
    private List<ShopItemData> shopItems = new List<ShopItemData>();
    // 玩家当前金币（实际项目中从PlayerManager获取，这里先模拟）
    private int playerGold = 1000;

    // 初始化（只调用一次）
    protected override void OnInit()
    {
        base.OnInit();
        
        // 初始化商品数据（模拟星露谷物语的商店商品）
        InitShopItems();
        
        // 更新金币显示
        UpdateGoldText();
        
        // 绑定关闭按钮
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(OnCloseButtonClick);
        }
        
        // 生成商品列表UI
        GenerateShopItemUI();
    }

    // 显示面板时的逻辑（可接收外部参数，比如不同商店的商品列表）
    protected override void OnShow(object param)
    {
        base.OnShow(param);
        
        // 如果传了自定义商品列表，更新数据
        if (param != null && param is List<ShopItemData>)
        {
            shopItems = param as List<ShopItemData>;
            GenerateShopItemUI(); // 重新生成UI
        }
        
        // 刷新金币（防止玩家在其他地方消费后数据不同步）
        UpdateGoldText();
    }

    // 隐藏面板时的逻辑
    protected override void OnHide()
    {
        base.OnHide();
        // 可选：隐藏时清空商品列表（优化性能）
        ClearShopItemUI();
    }

    // 初始化商店商品数据（模拟星露谷物语的商店商品）
    private void InitShopItems()
    {
        shopItems.Clear();
        
        // 添加星露风格的基础商品（可根据需求扩展）
        shopItems.Add(new ShopItemData("小麦种子", Resources.Load<Sprite>("UI/Resources/Atlases/WheatSeed"), 10, 999));
        shopItems.Add(new ShopItemData("防风草种子", Resources.Load<Sprite>("UI/Resources/Atlases/ParsnipSeed"), 20, 999));
        shopItems.Add(new ShopItemData("浇水壶", Resources.Load<Sprite>("UI/Resources/Atlases/WateringCan"), 200, 1));
        shopItems.Add(new ShopItemData("斧头", Resources.Load<Sprite>("UI/Resources/Atlases/Axe"), 500, 1));
    }

    // 生成商品列表UI（核心逻辑）
    private void GenerateShopItemUI()
    {
        // 先清空现有UI
        ClearShopItemUI();
        
        // 遍历商品数据，生成每个商品格子
        foreach (var item in shopItems)
        {
            // 实例化商品格子
            GameObject cellObj = Instantiate(itemCellPrefab, itemGrid);
            cellObj.name = item.ItemName;
            
            // 获取格子内的组件（需确保预制体有这些组件）
            Image itemIcon = cellObj.transform.Find("Icon").GetComponent<Image>();
            Text itemNameText = cellObj.transform.Find("Name").GetComponent<Text>();
            Text itemPriceText = cellObj.transform.Find("Price").GetComponent<Text>();
            Button buyButton = cellObj.transform.Find("BuyButton").GetComponent<Button>();
            
            // 设置商品信息
            itemIcon.sprite = item.Icon;
            itemNameText.text = item.ItemName;
            itemPriceText.text = $"${item.Price}";
            
            // 绑定购买按钮事件（使用局部变量捕获当前item，避免闭包问题）
            ShopItemData currentItem = item;
            buyButton.onClick.AddListener(() => OnBuyButtonClick(currentItem));
            
            // 像素完美适配（星露风格必加）
            cellObj.GetComponent<RectTransform>().SetPixelPerfectPosition(cellObj.GetComponent<RectTransform>().anchoredPosition);
        }
    }

    // 购买按钮点击事件
    private void OnBuyButtonClick(ShopItemData item)
    {
        // 验证金币是否足够
        if (playerGold < item.Price)
        {
            // 播放错误音效
            if (errorSound != null)
            {
                audioSource.PlayOneShot(errorSound);
            }
            Debug.Log($"金币不足！需要{item.Price}，当前只有{playerGold}");
            return;
        }
        
        // 扣减金币
        playerGold -= item.Price;
        UpdateGoldText();
        
        // 播放购买音效
        if (buySound != null)
        {
            audioSource.PlayOneShot(buySound);
        }
        
        // 分发购买事件（解耦UI和业务逻辑，由PlayerManager处理物品添加）
        UIEventManager.Instance.TriggerEvent(UIEventNames.OnBuyItem, item);
        
        Debug.Log($"购买成功：{item.ItemName}，剩余金币：{playerGold}");
    }

    // 更新金币显示文本
    private void UpdateGoldText()
    {
        if (goldText != null)
        {
            goldText.text = $"金币：{playerGold}";
            // 像素字体对齐（星露风格优化）
            goldText.rectTransform.SetPixelPerfectPosition(goldText.rectTransform.anchoredPosition);
        }
    }

    // 清空商品列表UI
    private void ClearShopItemUI()
    {
        foreach (Transform child in itemGrid)
        {
            Destroy(child.gameObject);
        }
    }

    // 关闭按钮点击（重写基类方法）
    public override void OnCloseButtonClick()
    {
        base.OnCloseButtonClick();
        // 可选：添加关闭时的额外逻辑，比如保存商店状态
    }

    // 确保销毁时清理事件（防止内存泄漏）
    protected override void OnDestroy()
    {
        base.OnDestroy();
        if (closeButton != null)
        {
            closeButton.onClick.RemoveListener(OnCloseButtonClick);
        }
    }
}

// 商店商品数据类（星露风格）
[System.Serializable]
public class ShopItemData
{
    public string ItemName; // 物品名
    public Sprite Icon; // 物品图标
    public int Price; // 价格
    public int MaxCount; // 最大购买数量

    // 构造函数
    public ShopItemData(string itemName, Sprite icon, int price, int maxCount)
    {
        ItemName = itemName;
        Icon = icon;
        Price = price;
        MaxCount = maxCount;
    }
}