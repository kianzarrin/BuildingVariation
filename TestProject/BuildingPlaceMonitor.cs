using ColossalFramework;
using ICities;

namespace TestProject
{
	public class BuildingPlaceMonitor : BuildingExtensionBase
	{
		public override void OnBuildingCreated(ushort id)
		{
			Building building = Singleton<BuildingManager>.instance.m_buildings.m_buffer[id];
			BuildingInfo.MeshInfo[] subMeshes = building.Info.m_subMeshes;
			for (int i = 0; i < subMeshes.Length; i++)
			{
				subMeshes[i].m_flagsForbidden &= ~Building.Flags.Created;
			}
		}
	}
}
