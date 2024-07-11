// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// [CreateAssetMenu(fileName = "New Item Database", menuName = "Inventory System/Items/Database")]
// public class ItemDatabaseObject : ScriptableObject, ISerializationCallbackReceiver
// {
//     public ItemObject[] Items;
//     public Dictionary<int, ItemObject> GetItem = new Dictionary<int, ItemObject>();

//     public void OnAfterDeserialize()
//     {
//         for (int i = 0; i < Items.Length; i++)
//         {
//             Items[i].Id = i+1;
//             GetItem.Add(i, Items[i]);
//         }
//     }

//     public void OnBeforeSerialize()
//     {
//         GetItem = new Dictionary<int, ItemObject>();
//     }
// }

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item Database", menuName = "Inventory System/Items/Database")]
public class ItemDatabaseObject : ScriptableObject
{
    public ItemObject[] itemObjects;
    public List<Item> items = new List<Item>();

    void OnEnable()
    {
        SetItemDatabase();
    }

    public void SetItemDatabase()
    {
        items = new List<Item>();
        for (int i = 0; i < itemObjects.Length; i++)
        {
            items.Add(new Item(itemObjects[i]));
        }
        items = items.GroupBy(ele => ele.id).Select(ele => ele.First()).ToList();
    }
}
