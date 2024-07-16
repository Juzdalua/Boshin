using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "New Inventory", menuName = "Inventory System/Inventory")]
public class InventoryObject : ScriptableObject
{
    [SerializeField] private string savePath;
    [SerializeField] private ItemDatabaseObject database;
    public Inventory Container;

    //     private void OnEnable()
    //     {
    // #if UNITY_EDITOR
    //         database = (ItemDatabaseObject)AssetDatabase.LoadAssetAtPath("Assets/Resources/ItemDatabase.asset", typeof(ItemDatabaseObject));
    // #else
    //         database = Resources.Load<ItemDatabaseObject>("ItemDatabase"); 
    // #endif
    //     }

    public void AddItem(Item _item, int _amount)
    {
        for (int i = 0; i < Container.Items.Count; i++)
        {
            if (Container.Items[i].item.Id == _item.Id)
            {
                Container.Items[i].AddAmount(_amount);
                return;
            }
        }
        Container.Items.Add(new InventorySlot(_item.Id, _item, _amount));
        Container.Items = Container.Items.OrderBy(ele => ele.id).GroupBy(ele => ele.id).Select(ele => ele.First()).ToList();
    }

    public Item GetItemById(int id)
    {
        return database.items.Find(ele => ele.Id == id);
    }

    public InventorySlot GetPlayerItemById(int id)
    {
        return Container.Items.Find(ele => ele.id == id);
    }

    // [ContextMenu("Save")]
    // public void Save()
    // {
    //     IFormatter formatter = new BinaryFormatter();
    //     Stream stream = new FileStream(string.Concat(Application.persistentDataPath, savePath), FileMode.Create, FileAccess.Write);
    //     formatter.Serialize(stream, Container);
    //     stream.Close();
    // }

    // [ContextMenu("Load")]
    // public void Load()
    // {
    //     if (File.Exists(string.Concat(Application.persistentDataPath, savePath)))
    //     {
    //         IFormatter formatter = new BinaryFormatter();
    //         Stream stream = new FileStream(string.Concat(Application.persistentDataPath, savePath), FileMode.Open, FileAccess.Read);
    //         Container = (Inventory)formatter.Deserialize(stream);
    //         stream.Close();
    //     }
    // }

    // [ContextMenu("Clear")]
    // public void Clear()
    // {
    //     Container = new Inventory();
    // }

    public void SetItemInventory()
    {
        Container = new Inventory();
    }
}

[System.Serializable]
public class Inventory
{
    public List<InventorySlot> Items = new List<InventorySlot>();
}

[System.Serializable]
public class InventorySlot
{
    public int id;
    public Item item;
    public int amount;
    public InventorySlot(int _id, Item _item, int _amount)
    {
        id = _id;
        item = _item;
        amount = _amount;
    }

    public void AddAmount(int value)
    {
        amount += value;
    }
}