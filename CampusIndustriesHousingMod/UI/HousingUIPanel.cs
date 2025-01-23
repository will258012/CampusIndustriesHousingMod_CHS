using CampusIndustriesHousingMod.AI;
using CampusIndustriesHousingMod.Utils;
using CampusIndustriesHousingMod.Managers;
using ColossalFramework;
using ColossalFramework.UI;
using UnityEngine;

namespace CampusIndustriesHousingMod.UI
{
    public class HousingUIPanel
    {
        public readonly UIPanel m_uiMainPanel;
        
        private readonly UILabel m_settingsHeader;
        private readonly UILabel m_settingsStatus;
        private readonly UICheckBox m_settingsCheckBox;

        private readonly UIPanel ApartmentNumberPanel;

        private readonly UIButton SaveBuildingSettingsBtn;
        private readonly UIButton ReturnToDefaultBtn;

        private readonly UIButton ApplyPrefabSettingsBtn;
        private readonly UIButton ApplyGlobalSettingsBtn;

        private readonly UIButton SetPrefabSettingsBtn;
        private readonly UIButton SetGlobalSettingsBtn;

        private readonly UIButton UnlockSettingsBtn;
        private readonly UIButton LockUnlockChangesBtn;

        public HousingUIPanel(BuildingWorldInfoPanel buildingWorldInfoPanel, UIPanel uIPanel)
        {
            m_uiMainPanel = buildingWorldInfoPanel.component.AddUIComponent<UIPanel>();
            m_uiMainPanel.name = "HousingUIPanel";
            m_uiMainPanel.backgroundSprite = "SubcategoriesPanel";
            m_uiMainPanel.opacity = 0.90f;
            m_uiMainPanel.isVisible = HousingConfig.Config.ShowPanel;
            m_uiMainPanel.relativePosition = new Vector3(m_uiMainPanel.parent.width + 1f, 40f);
            m_uiMainPanel.height = 350f;
            m_uiMainPanel.width = 510f;

            m_settingsCheckBox = UiUtils.CreateCheckBox(uIPanel, "SettingsCheckBox", "设置", HousingConfig.Config.ShowPanel);
            m_settingsCheckBox.width = 80f;
            m_settingsCheckBox.label.textColor = new Color32(185, 221, 254, 255);
            m_settingsCheckBox.label.textScale = 0.8125f;
            m_settingsCheckBox.tooltip = "设置宿舍的房间数";
            m_settingsCheckBox.AlignTo(buildingWorldInfoPanel.component, UIAlignAnchor.TopLeft);
            m_settingsCheckBox.relativePosition = new Vector3(400f, 0f);
            m_settingsCheckBox.eventCheckChanged += (component, value) =>
            {
                m_uiMainPanel.isVisible = value;
                m_uiMainPanel.height = 370f;
                HousingConfig.Config.ShowPanel = value;
                if(!value)
                {
                    SaveBuildingSettingsBtn.Disable();
                    ReturnToDefaultBtn.Disable();
                    ApplyPrefabSettingsBtn.Disable();
                    ApplyGlobalSettingsBtn.Disable();
                    SetPrefabSettingsBtn.Disable();
                    SetGlobalSettingsBtn.Disable();
                    UnlockSettingsBtn.Show();
                }
                HousingConfig.Config.Serialize();
            };
            uIPanel.AttachUIComponent(m_settingsCheckBox.gameObject);

            m_settingsHeader = UiUtils.CreateLabel(m_uiMainPanel, "SettingsPanelHeader", "调节房间数", "");
            m_settingsHeader.font = UiUtils.GetUIFont("OpenSans-Regular");
            m_settingsHeader.textAlignment = UIHorizontalAlignment.Center;
            m_settingsHeader.textColor = new Color32(78, 184, 126, 255);
            m_settingsHeader.relativePosition = new Vector3(200f, 20f);
            m_settingsHeader.textScale = 1.2f;

            m_settingsStatus = UiUtils.CreateLabel(m_uiMainPanel, "SettingsStatus", "", "");
            m_settingsStatus.font = UiUtils.GetUIFont("OpenSans-Regular");
            m_settingsStatus.textAlignment = UIHorizontalAlignment.Center;
            m_settingsStatus.textColor = new Color32(240, 190, 199, 255);
            m_settingsStatus.relativePosition = new Vector3(180f, 95f);
            m_settingsStatus.textScale = 0.9f;

            ApartmentNumberPanel = UiUtils.UIServiceBar(m_uiMainPanel, "ApartmentNumber", "", "房间数: ", "房间数");
            ApartmentNumberPanel.relativePosition = new Vector3(10f, 130f);

            SaveBuildingSettingsBtn = UiUtils.AddButton(m_uiMainPanel, 10f, 190f, "SaveBuildingSettings", "保存建筑设置", "优先级最高 —— 将覆盖类型和全局设置，为此建筑创建特定设置");
            SaveBuildingSettingsBtn.eventClicked += SaveBuildingSettings;

            ReturnToDefaultBtn = UiUtils.AddButton(m_uiMainPanel, 260f, 190f, "ReturnToDefault", "重置回默认", "不会删除记录，只是为其设置默认标志，你需要清除此建筑的设置才能使用类型或全局设置");
            ReturnToDefaultBtn.eventClicked += ReturnToDefault;

            ApplyPrefabSettingsBtn = UiUtils.AddButton(m_uiMainPanel, 260f, 240f, "ApplyPrefabSettings", "应用类型设置", "将设置应用到与此建筑相同类型的所有建筑 —— 仅此存档");
            ApplyPrefabSettingsBtn.eventClicked += ApplyPrefabSettings;

            ApplyGlobalSettingsBtn = UiUtils.AddButton(m_uiMainPanel, 260f, 290f, "ApplyGlobalSettings", "应用全局设置", "将设置应用到与此建筑相同类型的所有建筑 —— 跨存档");
            ApplyGlobalSettingsBtn.eventClicked += ApplyGlobalSettings;
                
            SetPrefabSettingsBtn = UiUtils.AddButton(m_uiMainPanel, 10f, 240f, "SetPrefabSettings", "设置新类型", "这将更新此存档中所有此类型建筑的当前房间数量");
            SetPrefabSettingsBtn.eventClicked += SetPrefabSettings;

            SetGlobalSettingsBtn = UiUtils.AddButton(m_uiMainPanel, 10f, 290f, "SetGlobalSettings", "设置新全局", "这将更新所有存档中此类型建筑的当前房间数量");
            SetGlobalSettingsBtn.eventClicked += SetGlobalSettings;

            UnlockSettingsBtn = UiUtils.AddButton(m_uiMainPanel, 130f, 55f, "UnlockSettingsBtn", "解锁设置", "");
            UnlockSettingsBtn.eventClicked += UnlockSettings;

            LockUnlockChangesBtn = UiUtils.AddButton(m_uiMainPanel, 10f, 55f, "LockUnLockChanges", "", "如果锁定，类型和全局设置对该建筑没有影响", 32, 32);

            LockUnlockChangesBtn.atlas = TextureUtils.GetAtlas("LockButtonAtlas");
            LockUnlockChangesBtn.normalFgSprite = "UnLock";
            LockUnlockChangesBtn.disabledFgSprite = "UnLock";
            LockUnlockChangesBtn.focusedFgSprite = "UnLock";
            LockUnlockChangesBtn.hoveredFgSprite = "UnLock";
            LockUnlockChangesBtn.pressedFgSprite = "UnLock";

            LockUnlockChangesBtn.eventClicked += LockUnlockChanges;

            SaveBuildingSettingsBtn.Disable();
            ReturnToDefaultBtn.Disable();
            ApplyPrefabSettingsBtn.Disable();
            ApplyGlobalSettingsBtn.Disable();
            SetPrefabSettingsBtn.Disable();
            SetGlobalSettingsBtn.Disable();
        }

        public void UnlockSettings(UIComponent c, UIMouseEventParameter eventParameter)
        {
            SaveBuildingSettingsBtn.Enable();
            ReturnToDefaultBtn.Enable();
                
            ushort buildingID = WorldInfoPanel.GetCurrentInstanceID().Building;
            Building building = Singleton<BuildingManager>.instance.m_buildings.m_buffer[buildingID];

            if (HousingManager.PrefabExist(building.Info.name, building.Info.GetAI().GetType().Name))
            {
                ApplyPrefabSettingsBtn.Enable();
            }

            if (HousingConfig.Config.GetGlobalSettings(building.Info) != null)
            {
                ApplyGlobalSettingsBtn.Enable();
            }

            SetPrefabSettingsBtn.Enable();
            SetGlobalSettingsBtn.Enable();

            UnlockSettingsBtn.Hide();
        }

        public void UpdateBuildingData()
        {
            ushort buildingID = WorldInfoPanel.GetCurrentInstanceID().Building;
            var building = Singleton<BuildingManager>.instance.m_buildings.m_buffer[buildingID];
            var buildingAI = building.Info.GetAI();
            bool isAllowedCityService = buildingAI is BarracksAI || buildingAI is DormsAI;

            if (isAllowedCityService)
            {
                if(HousingManager.BuildingRecordExist(buildingID))
                {
                    var buildingRecord = HousingManager.GetBuildingRecord(buildingID);
                    RefreshData(buildingID, buildingRecord);
                }
                else
                {
                    var buildingRecord = HousingManager.CreateBuildingRecord(buildingID);
                    RefreshData(buildingID, buildingRecord);
                }
            }
            else
            {
                m_settingsCheckBox.Hide();
                m_uiMainPanel.Hide();
            }
        }

        public void RefreshData(ushort buildingID, HousingManager.BuildingRecord buildingRecord)
        {
            Building building = Singleton<BuildingManager>.instance.m_buildings.m_buffer[buildingID];
            var m_apartmentsNumTextfield = ApartmentNumberPanel.Find<UITextField>("ApartmentNumberTextField");
            var globalRecord = HousingConfig.Config.GetGlobalSettings(building.Info);

            if (!buildingRecord.IsPrefab && !buildingRecord.IsGlobal)
            {
                m_settingsStatus.text = buildingRecord.IsDefault ? "此建筑使用默认设置" : "此建筑使用自身设置";
                m_apartmentsNumTextfield.text = buildingRecord.NumOfApartments.ToString();
            }
            else if (HousingManager.PrefabExist(building.Info.name, building.Info.GetAI().GetType().Name) && buildingRecord.IsPrefab && !buildingRecord.IsLocked)
            {
                m_settingsStatus.text = "此建筑使用类型设置";
                var prefabRecord = HousingManager.GetPrefab(building.Info);
                m_apartmentsNumTextfield.text = prefabRecord.NumOfApartments.ToString();
            }
            else if(globalRecord != null && buildingRecord.IsGlobal && !buildingRecord.IsLocked)
            {
                m_settingsStatus.text = "此建筑使用全局设置";
                m_apartmentsNumTextfield.text = globalRecord.NumOfApartments.ToString();
            }

            m_settingsCheckBox.Show();
            m_settingsCheckBox.relativePosition = new Vector3(400f, 0f);

            ApartmentNumberPanel.relativePosition = new Vector3(10f, 130f);

            string spriteName = buildingRecord.IsLocked ? "Lock" : "UnLock";

            LockUnlockChangesBtn.normalFgSprite = spriteName;
            LockUnlockChangesBtn.disabledFgSprite = spriteName;
            LockUnlockChangesBtn.focusedFgSprite = spriteName;
            LockUnlockChangesBtn.hoveredFgSprite = spriteName;
            LockUnlockChangesBtn.pressedFgSprite = spriteName;

            if (m_settingsCheckBox.isChecked)
            {
                m_uiMainPanel.height = 350f;
                m_uiMainPanel.Show();
            }
			
        }

        public void LockUnlockChanges(UIComponent c, UIMouseEventParameter eventParameter)
        {
            ushort buildingID = WorldInfoPanel.GetCurrentInstanceID().Building;

            var buildingRecord = HousingManager.GetBuildingRecord(buildingID);

            string spriteName = buildingRecord.IsLocked ? "UnLock" : "Lock";

            LockUnlockChangesBtn.normalFgSprite = spriteName;
            LockUnlockChangesBtn.disabledFgSprite = spriteName;
            LockUnlockChangesBtn.focusedFgSprite = spriteName;
            LockUnlockChangesBtn.hoveredFgSprite = spriteName;
            LockUnlockChangesBtn.pressedFgSprite = spriteName;

            UpdateBuildingSettings.ChangeBuildingLockStatus(buildingID, !buildingRecord.IsLocked);
        }

        public void ReturnToDefault(UIComponent c, UIMouseEventParameter eventParameter)
        {
            ushort buildingID = WorldInfoPanel.GetCurrentInstanceID().Building;
            var buildingInfo = Singleton<BuildingManager>.instance.m_buildings.m_buffer[buildingID].Info;

            var buildingWorkTimeDefault = HousingManager.CreateBuildingRecord(buildingID);

            var m_apartmentsNumTextfield = ApartmentNumberPanel.Find<UITextField>("ApartmentNumberTextField");

            m_apartmentsNumTextfield.text = buildingWorkTimeDefault.NumOfApartments.ToString();

            UpdateBuildingSettings.UpdateBuildingToDefaultSettings(buildingID, buildingWorkTimeDefault);

            RefreshData(buildingID, buildingWorkTimeDefault);
        }

        public void SaveBuildingSettings(UIComponent c, UIMouseEventParameter eventParameter)
        {
            ushort buildingID = WorldInfoPanel.GetCurrentInstanceID().Building;

            bool is_locked = false;
            if (LockUnlockChangesBtn.normalFgSprite == "Lock")
            {
                is_locked = true;
            }
            
            var m_apartmentsNumTextfield = ApartmentNumberPanel.Find<UITextField>("ApartmentNumberTextField");

            var newBuildingSettings = new HousingManager.BuildingRecord
            {
                NumOfApartments = int.Parse(m_apartmentsNumTextfield.text),
                IsLocked = is_locked
            };

            UpdateBuildingSettings.SaveNewSettings(buildingID, newBuildingSettings);

            RefreshData(buildingID, newBuildingSettings);
        }

        public void ApplyPrefabSettings(UIComponent c, UIMouseEventParameter eventParameter)
        {
            ushort buildingID = WorldInfoPanel.GetCurrentInstanceID().Building;
            var buildingInfo = Singleton<BuildingManager>.instance.m_buildings.m_buffer[buildingID].Info;

            var buildingRecord = HousingManager.GetBuildingRecord(buildingID);

            var m_apartmentsNumTextfield = ApartmentNumberPanel.Find<UITextField>("ApartmentNumberTextField");

            if (HousingManager.PrefabExist(buildingInfo.name, buildingInfo.GetAI().GetType().Name) && !buildingRecord.IsLocked)
            {
                var prefabRecord = HousingManager.GetPrefab(buildingInfo);
                m_apartmentsNumTextfield.text = prefabRecord.NumOfApartments.ToString();

                m_settingsStatus.text = "";

                UpdateBuildingSettings.SetBuildingToPrefab(buildingID, prefabRecord);
            }
        }

        public void ApplyGlobalSettings(UIComponent c, UIMouseEventParameter eventParameter)
        {
            ushort buildingID = WorldInfoPanel.GetCurrentInstanceID().Building;
            var buildingInfo = Singleton<BuildingManager>.instance.m_buildings.m_buffer[buildingID].Info;

            var buildingRecord = HousingManager.GetBuildingRecord(buildingID);

            var buildingRecordGlobal = HousingConfig.Config.GetGlobalSettings(buildingInfo);

            var m_apartmentsNumTextfield = ApartmentNumberPanel.Find<UITextField>("ApartmentNumberTextField");

            if (buildingRecordGlobal != null && !buildingRecord.IsLocked)
            {
                m_apartmentsNumTextfield.text = buildingRecordGlobal.NumOfApartments.ToString();
                m_settingsStatus.text = "";

                UpdateBuildingSettings.SetBuildingToGlobal(buildingID, buildingRecordGlobal);
            }
        }

        public void SetPrefabSettings(UIComponent c, UIMouseEventParameter eventParameter)
        {
            ConfirmPanel.ShowModal("设置类型", "是否更新此存档中所有此类型建筑的当前房间数量？", (comp, ret) =>
            {
                if (ret != 1)
                {
                    return;
                }
                ushort buildingID = WorldInfoPanel.GetCurrentInstanceID().Building;

                var buildingRecord = HousingManager.GetBuildingRecord(buildingID);

                var m_apartmentsNumTextfield = ApartmentNumberPanel.Find<UITextField>("ApartmentNumberTextField");

                var newPrefabSettings = new HousingManager.BuildingRecord
                {
                    NumOfApartments = int.Parse(m_apartmentsNumTextfield.text)
                };

                UpdateBuildingSettings.CreatePrefabSettings(buildingID, newPrefabSettings);

                RefreshData(buildingID, newPrefabSettings);
            });
        }

        public void SetGlobalSettings(UIComponent c, UIMouseEventParameter eventParameter)
        {
                ConfirmPanel.ShowModal("设置全局", "是否更新所有存档中此类型建筑的当前房间数量？", (comp, ret) =>
            {
                if (ret != 1)
                {
                    return;
                }
                ushort buildingID = WorldInfoPanel.GetCurrentInstanceID().Building;

                var buildingRecord = HousingManager.GetBuildingRecord(buildingID);

                var m_apartmentsNumTextfield = ApartmentNumberPanel.Find<UITextField>("ApartmentNumberTextField");

                var newGlobalSettings = new HousingManager.BuildingRecord
                {
                    NumOfApartments = int.Parse(m_apartmentsNumTextfield.text)
                };

                UpdateBuildingSettings.CreateGlobalSettings(buildingID, newGlobalSettings);

                RefreshData(buildingID, newGlobalSettings);
            });
        }

    }

}
