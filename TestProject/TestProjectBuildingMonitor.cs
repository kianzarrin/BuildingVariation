using ICities;

namespace TestProject
{
	public class TestProjectBuildingMonitor : BuildingExtensionBase
	{
		public override void OnBuildingReleased(ushort id)
		{
			TestProjectBuildingData.IngameBuildingVariationMap[id] = 0;
		}
	}
}
