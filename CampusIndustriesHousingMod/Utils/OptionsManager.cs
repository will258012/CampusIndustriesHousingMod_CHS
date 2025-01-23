using ICities;
using ColossalFramework.UI;
using System.IO;
using System.Xml.Serialization;
using System;
using CampusIndustriesHousingMod.Managers;

namespace CampusIndustriesHousingMod.Utils
{
    public class OptionsManager
    {
        private static readonly string[] BARRACKS_INCOME_LABELS =
        [
            "全维护费用",
            "满容量时维护费用减半",
            "满容量时无维护费用",
            "满容量时维护费用变为利润",
            "维护费用和利润均为两倍",
            "两倍利润，正常维护费用"
        ];
        private static readonly string[] DORMS_INCOME_LABELS = BARRACKS_INCOME_LABELS;


        public enum IncomeValues
        {
            FULL_MAINTENANCE = 1,
            HALF_MAINTENANCE = 2,
            NO_MAINTENANCE = 3,
            NORMAL_PROFIT = 4,
            DOUBLE_DOUBLE = 5,
            DOUBLE_PROFIT = 6
        };

        private UIDropDown barracksIncomeDropDown;
        private UIDropDown dormsIncomeDropDown;
        private IncomeValues barracksIncomeValue = IncomeValues.NO_MAINTENANCE;
        private IncomeValues dormsIncomeValue = IncomeValues.NO_MAINTENANCE;

        public void Initialize(UIHelperBase helper)
        {
            Logger.LogInfo(Logger.LOG_OPTIONS, "OptionsManager.Initialize -- Initializing Menu Options");
            UIHelperBase group = helper.AddGroup("住房全局设置");
            barracksIncomeDropDown = (UIDropDown)group.AddDropdown("工人宿舍收入预设", BARRACKS_INCOME_LABELS, 2, HandleIncomeChange);
            barracksIncomeDropDown.width = 350f;
            group.AddSpace(2);
            dormsIncomeDropDown = (UIDropDown)group.AddDropdown("学生宿舍收入预设", DORMS_INCOME_LABELS, 2, HandleIncomeChange);
            dormsIncomeDropDown.width = 350f;
            group.AddSpace(5);
            group.AddButton("保存", SaveOptions);

            UIHelperBase group_clear = helper.AddGroup("清除设置相关 —— 谨慎使用，无法撤销！");
            group_clear.AddButton("清除所有建筑设置", ConfimDeleteBuildignRecords);
            group_clear.AddSpace(1);
            group_clear.AddButton("清除所有类型设置", ConfimDeletePrefabRecords);
            group_clear.AddSpace(1);
            group_clear.AddButton("清除所有全局设置", ConfimDeleteGlobalConfig);
        }

        private void ConfimDeleteBuildignRecords()
        {
            ConfirmPanel.ShowModal("清除所有建筑设置", "是否清除所有建筑设置？", (comp, ret) =>
            {
                if (ret != 1)
                    return;
                HousingManager.ClearBuildingRecords();
            });
        }

        private void ConfimDeletePrefabRecords()
        {
            ConfirmPanel.ShowModal("清除所有建筑类型设置", "是否清除所有建筑类型设置？", (comp, ret) =>
            {
                if (ret != 1)
                    return;
                HousingManager.ClearPrefabRecords();
            });
        }

        private void ConfimDeleteGlobalConfig()
        {
            ConfirmPanel.ShowModal("清除所有全局设置", "是否清除所有全局设置？", (comp, ret) =>
            {
                if (ret != 1)
                    return;
                HousingConfig.Config.ClearGlobalSettings();
            });
        }

        private void HandleIncomeChange(int newSelection)
        {
            // Do nothing until Save is pressed
        }

        public IncomeValues GetBarracksIncomeModifier()
        {
            return barracksIncomeValue;
        }

        public IncomeValues GetDormsIncomeModifier()
        {
            return dormsIncomeValue;
        }

        private void SaveOptions()
        {
            Logger.LogInfo(Logger.LOG_OPTIONS, "OptionsManager.SaveOptions -- Saving Options");
            Options options = new();

            if (barracksIncomeDropDown != null)
            {
                int barracksIncomeSelectedIndex = barracksIncomeDropDown.selectedIndex + 1;
                options.barracksIncomeModifierSelectedIndex = barracksIncomeSelectedIndex;
                if (barracksIncomeSelectedIndex >= 0)
                {
                    Logger.LogInfo(Logger.LOG_OPTIONS, "OptionsManager.SaveOptions -- Barracks Income Modifier Set to: {0}", (IncomeValues)barracksIncomeSelectedIndex);
                    barracksIncomeValue = (IncomeValues)barracksIncomeSelectedIndex;
                }
            }

            if (dormsIncomeDropDown != null)
            {
                int dormsIncomeSelectedIndex = dormsIncomeDropDown.selectedIndex + 1;
                options.dormsIncomeModifierSelectedIndex = dormsIncomeSelectedIndex;
                if (dormsIncomeSelectedIndex >= 0)
                {
                    Logger.LogInfo(Logger.LOG_OPTIONS, "OptionsManager.saveOptions -- Dorms Income Modifier Set to: {0}", (IncomeValues)dormsIncomeSelectedIndex);
                    dormsIncomeValue = (IncomeValues)dormsIncomeSelectedIndex;
                }
            }

            try
            {
                using StreamWriter streamWriter = new("CampusIndustriesHousingModOptions.xml");
                new XmlSerializer(typeof(Options)).Serialize(streamWriter, options);
            }
            catch (Exception e)
            {
                Logger.LogError(Logger.LOG_OPTIONS, "Error saving options: {0} -- {1}", e.Message, e.StackTrace);
            }

        }

        public void LoadOptions()
        {
            Logger.LogInfo(Logger.LOG_OPTIONS, "OptionsManager.LoadOptions -- Loading Options");
            Options options = new();

            try
            {
                using StreamReader streamReader = new("CampusIndustriesHousingModOptions.xml");
                options = (Options)new XmlSerializer(typeof(Options)).Deserialize(streamReader);
            }
            catch (FileNotFoundException)
            {
                // Options probably not serialized yet, just return
                return;
            }
            catch (Exception e)
            {
                Logger.LogError(Logger.LOG_OPTIONS, "Error loading options: {0} -- {1}", e.Message, e.StackTrace);
                return;
            }

            if (options.barracksIncomeModifierSelectedIndex > 0)
            {
                Logger.LogInfo(Logger.LOG_OPTIONS, "OptionsManager.LoadOptions -- Loading Barracks Income Modifier to: {0}", (IncomeValues)options.barracksIncomeModifierSelectedIndex);
                barracksIncomeDropDown.selectedIndex = options.barracksIncomeModifierSelectedIndex - 1;
                barracksIncomeValue = (IncomeValues)options.barracksIncomeModifierSelectedIndex;
            }

            if (options.dormsIncomeModifierSelectedIndex > 0)
            {
                Logger.LogInfo(Logger.LOG_OPTIONS, "OptionsManager.LoadOptions -- Loading Dorms Income Modifier to: {0}", (IncomeValues)options.dormsIncomeModifierSelectedIndex);
                dormsIncomeDropDown.selectedIndex = options.dormsIncomeModifierSelectedIndex - 1;
                dormsIncomeValue = (IncomeValues)options.dormsIncomeModifierSelectedIndex;
            }
        }

        public struct Options
        {
            public int barracksIncomeModifierSelectedIndex;
            public int dormsIncomeModifierSelectedIndex;
        }
    }
}
