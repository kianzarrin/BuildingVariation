using System.Collections.Generic;
using ColossalFramework;
using HarmonyLib;

namespace TestProject
{
	[HarmonyPatch(typeof(ZonedBuildingWorldInfoPanel), "OnSetTarget")]
	public static class TestProjectPatchZonedBuildingWorldInfoPanel
	{
		public static void Postfix()
		{
			ushort building = TestProjectLoading.Instance.Building;
			Building building2 = Singleton<BuildingManager>.instance.m_buildings.m_buffer[building];
			string name = building2.Info.name;
			if (TestProjectBuildingData.PotentialVariationsMap.ContainsKey(name))
			{
				List<BuildingVariation> list = TestProjectBuildingData.PotentialVariationsMap[name];
				TestProjectLoading.m_variationDropdown.isVisible = true;
				List<string> list2 = new List<string>();
				for (int i = 0; i < list.Count; i++)
				{
					list2.Add(list[i].m_publicName);
				}
				TestProjectLoading.m_variationDropdown.items = list2.ToArray();
				if (TestProjectBuildingData.IngameBuildingVariationMap[building] != 0)
				{
					TestProjectLoading.m_variationDropdown.selectedIndex = TestProjectBuildingData.IngameBuildingVariationMap[building] - 1;
					return;
				}
				TestProjectLoading.m_variationDropdown.selectedIndex = list.FindIndex((BuildingVariation r) => r.m_isDefault);
			}
			else
			{
				TestProjectLoading.m_variationDropdown.isVisible = false;
			}
		}
	}
}
