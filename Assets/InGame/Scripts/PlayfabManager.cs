using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using System;
using UnityEngine.UI;
using UniRx;

namespace BlackTree
{
    #region playfab inventory�� ���̴� �͵�
    public class ItemInfo
    {
        public string instanceId;
        public int level;
        public int amount;
        public bool equip;
    }

    public class ItemSaveData
    {
        public string id;
        public string instanceId;
        public int amount;
        public bool equip;
        public int level;
    }
    #endregion
    public class PlayerData
    {
        public int item_1;
        public int item_2;
        public int item_3;
    }
    public class PlayfabManager : Monosingleton<PlayfabManager>
    {
        public PlayerData playerData;

        public Text EnergyLeftTIme;
        float secondsLeftToRefreshEnergy = 1;
        public InputField emailInput;
        public InputField passwordInput;
        public Button RegisterBtn;
        public Button loginBtn;
        public Button ResetPasswordBtn;

        public Button ExecuteBtn;

        public Button GameOverBtn;
        public Button GetleaderboardBtn;

        public Button SaveUserDatabtn;
        public Button GetUserDatabtn;

        string myplayfabId;

        public bool isLogin=false;

        public override void Init()
        {
            base.Init();

            //loginBtn.onClick.AsObservable().Subscribe(o => { Login(); });
            //RegisterBtn.onClick.AsObservable().Subscribe(o => { RegisterId(); });
            //ResetPasswordBtn.onClick.AsObservable().Subscribe(o => { ResetPassword(); });

            //GameOverBtn.onClick.AsObservable().Subscribe(o => { GameOver(); });
            //GetleaderboardBtn.onClick.AsObservable().Subscribe(o => { GetLeaderboard(); });

            //SaveUserDatabtn.onClick.AsObservable().Subscribe(o => { SaveAppearance(); });
            //GetUserDatabtn.onClick.AsObservable().Subscribe(o => { GetAppearance(); });

            //ExecuteBtn.onClick.AsObservable().Subscribe(o => { ExecuteButton(); });

            //playerData = new PlayerData();
            Login();
        }

        #region Ŭ���� ��ũ��Ʈ
        public void ExecuteButton()
        {
            var request = new ExecuteCloudScriptRequest
            {
                FunctionName = "hello",
                FunctionParameter=new { name="dds"}
            };
            PlayFabClientAPI.ExecuteCloudScript(request, OnExecuteSuccess, OnError);
        }

        private void OnExecuteSuccess(ExecuteCloudScriptResult result)
        {
        
        }

        public void SetInventoryCustomData(ItemSaveData _item,Action<ItemInfo> callback)
        {
            var request = new ExecuteCloudScriptRequest
            {
                FunctionName = "SetCustomItemdata",
                FunctionParameter = new { itemid=_item.instanceId, equip= _item.equip.ToString() }
            };
            PlayFabClientAPI.ExecuteCloudScript(request, OnExecuteSuccess, OnError);
        }
        #endregion

        #region �α���
        void Login()
        {
            var request = new LoginWithCustomIDRequest { 
            CustomId="blacktree1234",
            CreateAccount=true,
            InfoRequestParameters=new GetPlayerCombinedInfoRequestParams
            {
                GetPlayerProfile=true,
            }
            };

            PlayFabClientAPI.LoginWithCustomID(request, OnSuccess, OnError);
        }
        private void OnSuccess(LoginResult result)
        {
            myplayfabId = result.PlayFabId;
            Debug.Log($"Successful login/account create!::{myplayfabId}");

            //GetVirtualCurrencies();
            GetTitleData();
            //string name = null;
            //if(result.InfoResultPayload.PlayerProfile.DisplayName!=null)
            //    name = result.InfoResultPayload.PlayerProfile.DisplayName;
            //if(name==null)//�г��� ����
            //{
            //    SubmitNameButton();
            //}
            //else//�������� �����ֱ�
            //{
            //    GetLeaderboard();
            //}
            isLogin = true;

        }

        public void SubmitNameButton()
        {
            var request = new UpdateUserTitleDisplayNameRequest
            {
                DisplayName = UnityEngine.Random.Range(600, 1000).ToString()
            };
            PlayFabClientAPI.UpdateUserTitleDisplayName(request, OnDisplayNameUpdate, OnError);
        }

        private void OnDisplayNameUpdate(UpdateUserTitleDisplayNameResult result)
        {
            Debug.Log("Updated display name");
            GetLeaderboard();
        }

        void RegisterId()
        {
            var request = new RegisterPlayFabUserRequest
            {
                Email = emailInput.text,
                Password=passwordInput.text,
                RequireBothUsernameAndEmail=false,
                DisplayName="BlackTree"
            };

            PlayFabClientAPI.RegisterPlayFabUser(request, OnRegisterSuccess, OnError);
        }

        private void OnRegisterSuccess(RegisterPlayFabUserResult result)
        {
            Debug.Log("registered and logged in!");
        }

        void ResetPassword()
        {

        }
      
        public void OnError(PlayFabError obj)
        {
            Debug.Log(obj.ErrorMessage);
            Debug.Log(obj.GenerateErrorReport());
            
        }

        
        #endregion

        #region ��������
        public void SendLeaderboard(int score)
        {
            var request = new UpdatePlayerStatisticsRequest
            {
                Statistics = new List<StatisticUpdate>
            {
            new StatisticUpdate
                {
                    StatisticName="PlatformScore",
                    Value=score
                }
            }

            };
            PlayFabClientAPI.UpdatePlayerStatistics(request, OnLeaderboardUpdate, OnError);
        }

        private void OnLeaderboardUpdate(UpdatePlayerStatisticsResult result)
        {
            Debug.Log("Successful statistics result");
        }

        public void GameOver()
        {
            SendLeaderboard(UnityEngine.Random.Range(0, 1000000));
        }

        public void GetLeaderboard()
        {
            var request = new GetLeaderboardRequest
            {
                StatisticName = "PlatformScore",
                StartPosition = 0,
                MaxResultsCount = 10
            };
            PlayFabClientAPI.GetLeaderboard(request, OnLeaderboardGet, OnError);
        }

        private void OnLeaderboardGet(GetLeaderboardResult result)
        {
            foreach(var item in result.Leaderboard)
            {
                Debug.Log(item.Position + "/" + item.DisplayName + "/" + item.StatValue);
            }
        }

        public void GetLeaderboardAroundPlayer()
        {
            var request = new GetLeaderboardAroundPlayerRequest
            {
                StatisticName = "PlatformScore",
                MaxResultsCount = 10
            };
            PlayFabClientAPI.GetLeaderboardAroundPlayer(request, OnLeaderboardAroundPlayerGet, OnError);
        }

        private void OnLeaderboardAroundPlayerGet(GetLeaderboardAroundPlayerResult obj)
        {
            
        }
        #endregion

        #region �÷��̾����

        ///////////////////////���̺�////////////////////////
        public void SavePlayerData(Dictionary<string,string > playerdata,Action<UpdateUserDataResult> onComplete)
        {
            var request = new UpdateUserDataRequest
            {
                Data = playerdata
            };
            PlayFabClientAPI.UpdateUserData(request, onComplete, OnError);
        }
        public void SavePlayerData<T>(int keyidx, T e, Action<UpdateUserDataResult> onComplete) where T : class
        {
            Dictionary<string, string> characterdatadic = new Dictionary<string, string>();

            var _json = Newtonsoft.Json.JsonConvert.SerializeObject(e);
            characterdatadic.Add(keyidx.ToString(), _json);

            var request = new UpdateUserDataRequest
            {
                Data = characterdatadic
            };
            PlayFabClientAPI.UpdateUserData(request, onComplete, OnError);
        }

        public void SaveAppearance()
        {
            List<PlayerData> characters = new List<PlayerData>();
            for(int i=0; i<5; i++)
            {
                PlayerData _data = new PlayerData();

                _data.item_1 = UnityEngine.Random.Range(0, 100);
                _data.item_2 = UnityEngine.Random.Range(100, 200);
                _data.item_3 = UnityEngine.Random.Range(200, 300);

                characters.Add(_data);
            }

            Dictionary<string, string> characterdatadic = new Dictionary<string, string>();

            for(int i=0; i< characters.Count; i++)
            {
                var _json= Newtonsoft.Json.JsonConvert.SerializeObject(characters[i]);
                characterdatadic.Add($"item_{i}", _json);
            }

            var playerjson = Newtonsoft.Json.JsonConvert.SerializeObject(characters[0]);
            var request = new UpdateUserDataRequest
            {
                Data=new Dictionary<string, string>
                {
                    { "item_0",playerjson }
                }
            };
            PlayFabClientAPI.UpdateUserData(request, OnDataSend, OnError);
        }

        private void OnDataSend(UpdateUserDataResult obj)
        {
            var _data = obj.Request.ToJson();

            Debug.Log("success user data sended");
        }
        ///////////////////////���̺�////////////////////////
        //////
        ///

        ///////////////////////�ε�////////////////////////
        public void GetPlayerData(string keystring,Action<GetUserDataResult> onLoadComplete)
        {
            PlayFabClientAPI.GetUserData(new GetUserDataRequest()
            {
                PlayFabId = myplayfabId,
                Keys = new List<string> { keystring }
            }, onLoadComplete, OnError);
        }
        
        public void GetAppearance()
        {
            PlayFabClientAPI.GetUserData(new GetUserDataRequest() {PlayFabId=myplayfabId,
            Keys=new List<string> { "Characters" }
            }, OnDataRecieved, OnError);
        }

        private void OnDataRecieved(GetUserDataResult result)
        {
            Debug.Log("Recieved user data!");
            if(result.Data!=null)
            {
                List<PlayerData> characters = Newtonsoft.Json.JsonConvert.DeserializeObject<List<PlayerData>>(result.Data["Characters"].Value);

                foreach(var _data in characters)
                {
                    Debug.Log($"<color=cyan>{_data.item_1}</color><color=red>{_data.item_2}</color><color=green>{_data.item_3}</color>");
                }
            }
        }

        ///////////////////////�ε�////////////////////////
        #endregion

        #region Ÿ��Ʋ������
        void GetTitleData()
        {
            PlayFabClientAPI.GetTitleData(new GetTitleDataRequest(), OnTitleDataRecieved, OnError);
        }

        void OnTitleDataRecieved(GetTitleDataResult result)
        {
            if(result.Data==null)
            {
                Debug.Log("No message!");
                return;
            }
            Debug.Log(result.Data["Message"]);

            int.Parse(result.Data["Multiplier"]);
        }
        #endregion

        #region ��ȭ
        public void GetVirtualCurrencies()
        {
            PlayFabClientAPI.GetUserInventory(new GetUserInventoryRequest(), OnGetUserInventorySuccess, OnError);
        }
        void OnGetUserInventorySuccess(GetUserInventoryResult result)
        {
            int coins = result.VirtualCurrency["GD"];
            Debug.Log($"���:{coins}");

            int dia = result.VirtualCurrency["DM"];
            Debug.Log($"���̾�:{dia}");

            int energy = result.VirtualCurrency["EN"];
            Debug.Log($"������:{energy}");

            secondsLeftToRefreshEnergy = result.VirtualCurrencyRechargeTimes["EN"].SecondsToRecharge;
        }

        

        public void BuyItem()
        {
            var request = new SubtractUserVirtualCurrencyRequest
            {
                VirtualCurrency = "GD",
                Amount = 10
            };
            PlayFabClientAPI.SubtractUserVirtualCurrency(request, OnSubtractCoinsSuccess, OnError);
        }

        private void OnSubtractCoinsSuccess(ModifyUserVirtualCurrencyResult obj)
        {
            Debug.Log("bought item!!");
            GetVirtualCurrencies();
        }

        #endregion

        #region ������ ����
        public void GetItemInfo()
        {
            var request = new GetCatalogItemsRequest() {
                CatalogVersion = "CharacterItem"
            };
            PlayFabClientAPI.GetCatalogItems(request, OnGetCatalogItemResult, OnError);
            
        }

        private void OnGetCatalogItemResult(GetCatalogItemsResult result)
        {
            var items= result.Catalog;

            foreach (var item in items)
            {
                 Debug.Log(item.CustomData);
            }
        }

        public Dictionary<string, ItemSaveData> inventoryid = new Dictionary<string, ItemSaveData>();

        //���� �κ��丮 ���
        public void GetInventory(Action<Dictionary<string, ItemSaveData>> callback)
        {
            var request = new GetUserInventoryRequest();

            PlayFabClientAPI.GetUserInventory(request,o=> { OnGetUserinventory(o, callback); } , OnError);
        }

        private void OnGetUserinventory(GetUserInventoryResult obj, Action<Dictionary<string, ItemSaveData>> callback)
        {
            foreach (var _data in obj.Inventory)
            {
                inventoryid.Add( _data.ItemId,new ItemSaveData() { id = _data.ItemId, amount = (int)_data.RemainingUses, instanceId = _data.ItemInstanceId });
            }

            callback?.Invoke(inventoryid);
        }

        //������ ȹ��
        public void PrchaseItemInfo(string itemId,Action<List<ItemSaveData>> callback)
        {
            PurchaseItemRequest request = new PurchaseItemRequest {
                CatalogVersion = "CharacterItem",
                ItemId = itemId,
                Price = 0,
                VirtualCurrency="GD"
            };
            PlayFabClientAPI.PurchaseItem(request, (result)=> { OnPurchaseItem(result, callback); }, OnError);
        }

        private void OnPurchaseItem(PurchaseItemResult result, Action<List<ItemSaveData>> callback)
        {
            var _json = result.Request.ToJson();
            
            List<ItemSaveData> items = new List<ItemSaveData>();
            foreach (var item in result.Items)
            {
                if(inventoryid.ContainsKey(item.ItemId))
                {
                    inventoryid[item.ItemId].amount = (int)item.RemainingUses;
                }
                else
                {
                    inventoryid.Add(item.ItemId,new ItemSaveData() { id = item.ItemId, amount = (int)item.RemainingUses, instanceId = item.ItemInstanceId });
                }
                items.Add(new ItemSaveData() { id = item.ItemId, amount = (int)item.RemainingUses, instanceId = item.ItemInstanceId });
            }

            callback?.Invoke(items);
        }

        //������ �Һ�
        //unlock�� counsume�� �Һ� �Ǵ��� ����, ������� ���� ����,���� ������ �����ۿ� ���� ǥ�õǳ� ����
        public void ConsumeItem(ItemSaveData _data,Action<ItemInfo> callback ) 
        {
            var request = new ConsumeItemRequest {ConsumeCount=0,ItemInstanceId= _data.instanceId};
            PlayFabClientAPI.ConsumeItem(request,o=> { OnConsumeItemResult(o, callback); } , OnError);
           //PlayFabClientAPI.UnlockContainerItem(_request, o => { o.}, OnError);
        }

        private void OnConsumeItemResult(ConsumeItemResult result, Action<ItemInfo> callback)
        {
            if(result.RemainingUses<0)
            {
                //����
            }
            else
            {
                result.CustomData.ToString();
                ItemInfo _data = Newtonsoft.Json.JsonConvert.DeserializeObject<ItemInfo>(result.CustomData.ToString());
                _data.instanceId = result.ItemInstanceId;
                callback?.Invoke(_data);
            }
        }

        public void UnlockItem(ItemSaveData _data, Action<List<ItemSaveData>> callback)
        {
            var request = new UnlockContainerItemRequest { ContainerItemId=_data.id};
            PlayFabClientAPI.UnlockContainerItem(request, o => { OnUnlockItemResult(o, callback); }, OnError);
        }

        private void OnUnlockItemResult(UnlockContainerItemResult result, Action<List<ItemSaveData>> callback)
        {
            if(result.GrantedItems.Count > 0)
            {
                List<ItemSaveData> items = new List<ItemSaveData>();
                foreach (var item in result.GrantedItems)
                {
                    if (inventoryid.ContainsKey(item.ItemId))
                    {
                        inventoryid[item.ItemId].amount = (int)item.RemainingUses;
                    }
                    else
                    {
                        inventoryid.Add(item.ItemId, new ItemSaveData() { id = item.ItemId, amount = (int)item.RemainingUses, instanceId = item.ItemInstanceId });
                    }
                    items.Add(new ItemSaveData() { id = item.ItemId, amount = (int)item.RemainingUses, instanceId = item.ItemInstanceId });
                }

                callback?.Invoke(items);
            }
        }

        public void ResolveBundleItem(ItemSaveData _data, Action<List<ItemSaveData>> callback)
        {
            var request = new UnlockContainerItemRequest { ContainerItemId = _data.id };
            PlayFabClientAPI.UnlockContainerItem(request, o => { OnUnlockItemResult(o, callback); }, OnError);
        }

        private void OnResolveBundleResult(UnlockContainerItemResult result, Action<List<ItemSaveData>> callback)
        {
            if (result.GrantedItems.Count > 0)
            {
                List<ItemSaveData> items = new List<ItemSaveData>();
                foreach (var item in result.GrantedItems)
                {
                    if (inventoryid.ContainsKey(item.ItemId))
                    {
                        inventoryid[item.ItemId].amount = (int)item.RemainingUses;
                    }
                    else
                    {
                        inventoryid.Add(item.ItemId, new ItemSaveData() { id = item.ItemId, amount = (int)item.RemainingUses, instanceId = item.ItemInstanceId });
                    }
                    items.Add(new ItemSaveData() { id = item.ItemId, amount = (int)item.RemainingUses, instanceId = item.ItemInstanceId });
                }

                callback?.Invoke(items);
            }
        }

        #endregion

        private void Update()
        {
            //secondsLeftToRefreshEnergy -= Time.deltaTime;
            //TimeSpan time = TimeSpan.FromSeconds(secondsLeftToRefreshEnergy);
            //EnergyLeftTIme.text = time.ToString("mm':'ss");
            //if (secondsLeftToRefreshEnergy < 0)
            //{
            //    GetVirtualCurrencies();
            //}
        }

    }

}
