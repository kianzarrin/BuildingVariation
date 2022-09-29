using System.Collections.Generic;
using System.Reflection;
using ColossalFramework;
using ColossalFramework.UI;
using HarmonyLib;

namespace TestProject
{
	[HarmonyPatch(typeof(CityServiceWorldInfoPanel), "OnSetTarget")]
	public class TestProjectPatchPanelVisibility
	{
		private static void Postfix(CityServiceWorldInfoPanel __instance)
		{
			InstanceID? instanceID = __instance.GetType().GetField("m_InstanceID", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(__instance) as InstanceID?;
			if (!instanceID.HasValue)
			{
				return;
			}
			ushort building = instanceID.Value.Building;
			Building building2 = Singleton<BuildingManager>.instance.m_buildings.m_buffer[building];
			string name = building2.Info.name;
			if (!TestProjectBuildingData.PotentialVariationsMap.ContainsKey(name))
			{
				return;
			}
			List<BuildingVariation> list = new List<BuildingVariation>();
			list = TestProjectBuildingData.PotentialVariationsMap[name];
			if (list.Count == 0)
			{
				return;
			}
			(__instance.GetType().GetField("m_VariationPanel", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(__instance) as UIPanel).isVisible = true;
			__instance.component.height += 80f;
			UIDropDown uIDropDown = __instance.GetType().GetField("m_VariationDropdown", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(__instance) as UIDropDown;
			uIDropDown.zOrder = 3;
			List<string> list2 = new List<string>();
			for (int i = 0; i < list.Count; i++)
			{
				list2.Add(list[i].m_publicName);
			}
			uIDropDown.items = list2.ToArray();
			uIDropDown.selectedIndex = TestProjectBuildingData.IngameBuildingVariationMap[building] - 1;
			if (uIDropDown.selectedIndex == -1)
			{
				int num = list.FindIndex((BuildingVariation r) => r.m_isDefault);
				uIDropDown.selectedIndex = ((num != -1) ? num : 0);
			}
		}
	}
}
