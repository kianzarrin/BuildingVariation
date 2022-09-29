using System.Collections.Generic;
using System.Linq;
using ColossalFramework;

namespace TestProject
{
	public static class TestProjectBuildingData
	{
		public static Dictionary<string, List<BuildingVariation>> PotentialVariationsMap = new Dictionary<string, List<BuildingVariation>>();

		public static byte[] IngameBuildingVariationMap = new byte[49152];

		public static bool IsSubmeshEnabled(ushort building, BuildingInfo.MeshInfo submesh)
		{
			string name = Singleton<BuildingManager>.instance.m_buildings.m_buffer[building].Info.name;
			if (!PotentialVariationsMap.ContainsKey(name))
			{
				return true;
			}
			bool flag = true;
			for (int j = 0; j < PotentialVariationsMap[name].Count; j++)
			{
				if (PotentialVariationsMap[name][j].m_enabledSubMeshes.Contains(submesh.m_subInfo.name))
				{
					flag = false;
				}
			}
			if (flag)
			{
				return true;
			}
			if (IngameBuildingVariationMap[building] == 0)
			{
				BuildingVariation[] array = PotentialVariationsMap[name].Where((BuildingVariation i) => i.m_isDefault).ToArray();
				if (array == null || array.Length == 0)
				{
					return false;
				}
				BuildingVariation buildingVariation = array[0];
				if (buildingVariation == null)
				{
					return false;
				}
				if (buildingVariation.m_enabledSubMeshes == null || buildingVariation.m_enabledSubMeshes.Count == 0)
				{
					return false;
				}
				return buildingVariation.m_enabledSubMeshes.Contains(submesh.m_subInfo.name);
			}
			if (PotentialVariationsMap[name][IngameBuildingVariationMap[building] - 1] == null)
			{
				return false;
			}
			if (PotentialVariationsMap[name][IngameBuildingVariationMap[building] - 1].m_enabledSubMeshes == null || PotentialVariationsMap[name][IngameBuildingVariationMap[building] - 1].m_enabledSubMeshes.Count == 0)
			{
				return false;
			}
			return PotentialVariationsMap[name][IngameBuildingVariationMap[building] - 1].m_enabledSubMeshes.Contains(submesh.m_subInfo.name);
		}
	}
}
