using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BlackTree
{
    public class InventoryUI : MonoBehaviour
    {
        InventoryObject inventory;
        [SerializeField]ItemUIDisplay itemSlot;
        [SerializeField] Transform slotparent;

        public Dictionary<ItemUIDisplay, ItemSlot> slotsOnInterface = new Dictionary<ItemUIDisplay, ItemSlot>();

        // Start is called before the first frame update
        void Start()
        {
            StartCoroutine(GetDBItemInfo());
        }

        IEnumerator GetDBItemInfo()
        {
            yield return new WaitUntil(() => InGameDataTableManager.Instance.tableLoaded== true);

            inventory = new InventoryObject();
            inventory.Init();

            yield return new WaitUntil(()=>PlayfabManager.Instance.isLogin == true);

            Debug.Log($"������ �ε�");

            foreach (var data in inventory.GetSlots)
            {
                var keyidx = data.item.idx.ToString();
                PlayfabManager.Instance.GetPlayerData(keyidx, o=> { OnDataRecieved(o, data); });
            }

            //cloudscript �Լ� ���� �κ��丮�� ��ü ������ �ε��ؾ� �Ѵ�.
            //���� �������� ���̺�� ������ ���̺�� ������ �κ��丮 ������ ������Ʈ �ϵ��� cloudscript�� �۾��ؾ� �Ѵ�.
            //���� �κ��丮 ��ü������ �޾Ƽ� �ݹ����� �κ��丮 db�� ��� �޾����� �������� �Ѿ��.
            //�׷��� ������ ��� �ӽ÷� 2�� �ڿ� �Ѿ�� �����ϵ��� �Ѵ�.
            yield return new WaitForSeconds(2.0f);

            CreateSlots();
        }

        public void CreateSlots()
        {
            slotsOnInterface = new Dictionary<ItemUIDisplay, ItemSlot>();
            for (int i = 0; i < inventory.GetSlots.Count; i++)
            {
                var obj = Instantiate(itemSlot);
                obj.transform.SetParent(slotparent.transform, false);
                obj.transform.localPosition = Vector3.zero;
                slotsOnInterface.Add(obj, inventory.GetSlots[i]);
            }

            foreach (var data in slotsOnInterface)
            {
                data.Key.GetComponent<ItemUIDisplay>().Init(data.Value);
                data.Value.display = data.Key.GetComponent<ItemUIDisplay>();
                data.Value.parent = inventory;
                data.Value.UpdateSlot();
            }

        }

        #region DB�ε�
        private void OnDataRecieved(PlayFab.ClientModels.GetUserDataResult result,ItemSlot itemdata)
        {
            Debug.Log($"Recieved user data!!{result.Data.Count}");
            if(result.Data.ContainsKey(itemdata.item.idx.ToString()))
            {
                var _itemdata = inventory.GetSlots.Find(o => o.item.idx == itemdata.item.idx);
                var dbItem = Newtonsoft.Json.JsonConvert.DeserializeObject<Item>(result.Data[itemdata.item.idx.ToString()].Value);
                _itemdata.item.Level = dbItem.Level;
                _itemdata.item.amount = dbItem.amount;
                _itemdata.item.idx = dbItem.idx;
            }
        }



        #endregion

    }

}
