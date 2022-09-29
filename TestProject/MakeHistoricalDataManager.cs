using System.IO;
using ColossalFramework.IO;
using ICities;

namespace TestProject
{
	public class MakeHistoricalDataManager : SerializableDataExtensionBase
	{
		private TestProjectBuildingDataContainer _data;

		public override void OnLoadData()
		{
			byte[] array = base.serializableDataManager.LoadData("BuildingVariations");
			if (array != null)
			{
				using MemoryStream stream = new MemoryStream(array);
				_data = DataSerializer.Deserialize<TestProjectBuildingDataContainer>(stream, DataSerializer.Mode.Memory);
			}
			else
			{
				_data = new TestProjectBuildingDataContainer();
			}
		}

		public override void OnSaveData()
		{
			byte[] data;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				DataSerializer.Serialize(memoryStream, DataSerializer.Mode.Memory, 0u, _data);
				data = memoryStream.ToArray();
			}
			base.serializableDataManager.SaveData("BuildingVariations", data);
		}
	}
}
