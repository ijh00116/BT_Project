using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BlackTree
{
    public class InventoryUI : MonoBehaviour
    {
        InventoryObject invenobj;
        GameObject itemSlot;
        [SerializeField] InputField iteminput;
        
        // Start is called before the first frame update
        void Start()
        {
            invenobj = new InventoryObject();
            invenobj.Init();

            StartCoroutine(GetDB());
        }

        IEnumerator GetDB()
        {
            yield return new WaitUntil(()=>PlayfabManager.instance.isLogin == true);


            PlayfabManager.instance.GetInventory(InitItemCallback);
        }
        // Update is called once per frame
        void Update()
        {
            if(Input.GetKeyDown(KeyCode.G))
            {
                PlayfabManager.instance.PrchaseItemInfo(iteminput.text, GetItemCallback);
            }

            if(Input.GetKeyDown(KeyCode.U))
            {
                PlayfabManager.instance.ConsumeItem(PlayfabManager.instance.inventoryid[iteminput.text], GetChangedItemInfo);
            }
            if (Input.GetKeyDown(KeyCode.E))
            {
                PlayfabManager.instance.inventoryid[iteminput.text].equip = !PlayfabManager.instance.inventoryid[iteminput.text].equip;
                PlayfabManager.instance.SetInventoryCustomData(PlayfabManager.instance.inventoryid[iteminput.text], GetChangedItemInfo);
            }
            if (Input.GetKeyDown(KeyCode.L))
            {
                PlayfabManager.instance.UnlockItem(PlayfabManager.instance.inventoryid[iteminput.text], GetItemCallback);
            }
        }

        void InitItemCallback(Dictionary<string,ItemSaveData> result)
        {
            foreach (var item in result)
            {
                Debug.Log($"<color=red>�ʱ�ε�</color> ���۾Ƶ�:{item.Value.id}//����:{item.Value.level}//�������:{item.Value.equip}//����:{item.Value.amount}");
            }
            if(result.Count<=0)
            {
                Debug.Log("<color=red>���� ����</color>");
            }
        }

        public void GetItemCallback(List<ItemSaveData> result)
        {
            foreach (var item in result)
            {
                Debug.Log($"<color=cyan>ȹ��</color> ���۾Ƶ�:{item.id}//����:{item.level}//�������:{item.equip}//����:{item.amount}");
            }
        }

        public void GetChangedItemInfo(ItemInfo info)
        {
            if(PlayfabManager.instance.inventoryid.ContainsKey(info.instanceId))
            {
                var item = PlayfabManager.instance.inventoryid[info.instanceId];
                item.equip= info.equip;
                item.level = info.level;
                item.amount = info.amount;
                Debug.Log($"<color=cyan>ȹ��</color> ���۾Ƶ�:{item.id}//����:{item.level}//�������:{item.equip}//����:{item.amount}");
            }
        }
    }

}
