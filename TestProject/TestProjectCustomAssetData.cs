using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICities;
using UnityEngine;

namespace TestProject
{
	public class TestProjectCustomAssetData : AssetDataExtensionBase
	{
		private string kKey = "BuildingVariations";

		public override void OnAssetLoaded(string name, object asset, Dictionary<string, byte[]> userData)
		{
			if (userData.ContainsKey(kKey))
			{
				byte[] bytes = userData[kKey];
				string @string = Encoding.ASCII.GetString(bytes);
				Debug.Log("[Building Variations] Asset configuration: {" + @string + "} loaded.");
				List<string> list = @string.Split('|').ToList();
				List<BuildingVariation> list2 = new List<BuildingVariation>();
				for (int i = 0; i < list.Count; i++)
				{
					list2.Add(BuildingVariation.FromSerializedString(list[i]));
				}
				if (!(asset is BuildingInfo))
				{
					Debug.LogError("[Building Variations] Asset isn't BuildingInfo, aborting");
				}
				else
				{
					TestProjectBuildingData.PotentialVariationsMap.Add((asset as BuildingInfo).name, list2);
				}
			}
		}

		public override void OnAssetSaved(string name, object asset, out Dictionary<string, byte[]> userData)
		{
			if (asset is BuildingInfo)
			{
				bool flag = false;
				int index = 0;
				for (int i = 0; i < TestProjectLoading.buildingVariations.Count; i++)
				{
					if (TestProjectLoading.buildingVariations[i].m_isDefault)
					{
						flag = true;
						index = i;
						break;
					}
				}
				if (!flag && TestProjectLoading.buildingVariations.Count > 0)
				{
					TestProjectLoading.buildingVariations[0].m_isDefault = true;
				}
				BuildingInfo.MeshInfo[] subMeshes = (asset as BuildingInfo).m_subMeshes;
				for (int j = 0; j < subMeshes.Length; j++)
				{
					if (!TestProjectLoading.buildingVariations[index].m_enabledSubMeshes.Contains(subMeshes[j].m_subInfo.name))
					{
						subMeshes[j].m_flagsForbidden |= Building.Flags.Created;
					}
				}
				(asset as BuildingInfo).m_subMeshes = subMeshes;
			}
			userData = new Dictionary<string, byte[]>();
			if (TestProjectLoading.buildingVariations != null && TestProjectLoading.buildingVariations.Count != 0)
			{
				List<string> list = new List<string>();
				for (int k = 0; k < TestProjectLoading.buildingVariations.Count; k++)
				{
					list.Add(TestProjectLoading.buildingVariations[k].ToSerializableString());
				}
				string text = string.Join("|", list.ToArray());
				Debug.Log("[Building Variations] Asset configuration: {" + text + "} saved.");
				byte[] bytes = Encoding.ASCII.GetBytes(text);
				userData.Add(kKey, bytes);
			}
		}
	}
}
