using ColossalFramework;
using ColossalFramework.IO;

namespace TestProject
{
	public class TestProjectBuildingDataContainer : IDataContainer
	{
		public const string DataId = "BuildingVariations";

		public const int DataVersion = 0;

		public void Serialize(DataSerializer s)
		{
			s.WriteByteArray(TestProjectBuildingData.IngameBuildingVariationMap);
		}

		public void Deserialize(DataSerializer s)
		{
			TestProjectBuildingData.IngameBuildingVariationMap = s.ReadByteArray();
		}

		public void AfterDeserialize(DataSerializer s)
		{
			if (!Singleton<BuildingManager>.exists)
			{
				return;
			}
			Building[] buffer = Singleton<BuildingManager>.instance.m_buildings.m_buffer;
			for (int i = 0; i < TestProjectBuildingData.IngameBuildingVariationMap.Length; i++)
			{
				if (buffer[i].m_flags == Building.Flags.None)
				{
					TestProjectBuildingData.IngameBuildingVariationMap[i] = 0;
					continue;
				}
				BuildingInfo.MeshInfo[] subMeshes = buffer[i].Info.m_subMeshes;
				for (int j = 0; j < subMeshes.Length; j++)
				{
					subMeshes[j].m_flagsForbidden &= ~Building.Flags.Created;
				}
			}
		}
	}
}
