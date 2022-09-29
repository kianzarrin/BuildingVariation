using System.Collections.Generic;
using System.Reflection;
using ColossalFramework;
using ColossalFramework.UI;
using HarmonyLib;

namespace TestProject
{
	[HarmonyPatch(typeof(CityServiceWorldInfoPanel), "OnVariationDropdownChanged")]
	public class TestProjectPatchPanelVariations
	{
		private static bool Prefix(CityServiceWorldInfoPanel __instance, UIComponent component, int value)
		{
			if (__instance.GetType().GetField("m_IndustryBuildingAI", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(__instance) as IndustryBuildingAI == null)
			{
				UIDropDown uIDropDown = __instance.GetType().GetField("m_VariationDropdown", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(__instance) as UIDropDown;
				InstanceID? instanceID = __instance.GetType().GetField("m_InstanceID", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(__instance) as InstanceID?;
				if (!instanceID.HasValue)
				{
					return false;
				}
				ushort building = instanceID.Value.Building;
				Building building2 = Singleton<BuildingManager>.instance.m_buildings.m_buffer[building];
				string name = building2.Info.name;
				List<BuildingVariation> list = TestProjectBuildingData.PotentialVariationsMap[name];
				TestProjectBuildingData.IngameBuildingVariationMap[building] = (byte)(value + 1);
				return false;
			}
			return true;
		}
	}
}
