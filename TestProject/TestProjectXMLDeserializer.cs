using System.Collections.Generic;
using System.IO;
using System.Xml;
using ColossalFramework.Packaging;
using ICities;
using UnityEngine;

namespace TestProject
{
	public class TestProjectXMLDeserializer : LoadingExtensionBase
	{
		public static bool alreadyDeserializedXML = false;

		public static void DeserializeXML()
		{
			if (alreadyDeserializedXML)
			{
				return;
			}
			List<string> list = new List<string>();
			for (uint num = 0u; num < PrefabCollection<BuildingInfo>.LoadedCount(); num++)
			{
				BuildingInfo loaded = PrefabCollection<BuildingInfo>.GetLoaded(num);
				if (loaded == null)
				{
					continue;
				}
				Package.Asset asset = PackageManager.FindAssetByName(loaded.name);
				if (asset == null || asset.package == null)
				{
					continue;
				}
				string packagePath = asset.package.packagePath;
				string packageName = asset.package.packageName;
				if (packagePath == null || packageName == null)
				{
					continue;
				}
				string text = Path.Combine(Path.GetDirectoryName(packagePath), packageName + ".xml");
				Debug.Log("Checking: " + text);
				if (list.Contains(text))
				{
					continue;
				}
				list.Add(text);
				XmlDocument xmlDocument = new XmlDocument();
				try
				{
					xmlDocument.Load(text);
				}
				catch (XmlException ex)
				{
					Debug.LogError("There seems to be an XML error with asset " + loaded.name + ". Please give this error to the creator of that asset: " + ex.Message);
					continue;
				}
				if (xmlDocument == null)
				{
					continue;
				}
				List<BuildingVariation> list2 = new List<BuildingVariation>();
				string text2 = loaded.name.Replace("_Data", "");
				Debug.Log(text2);
				foreach (XmlNode childNode in xmlDocument.DocumentElement.ChildNodes)
				{
					if (childNode.Name == "Variation")
					{
						BuildingVariation buildingVariation = new BuildingVariation();
						buildingVariation.m_publicName = childNode.Attributes["name"]?.InnerText;
						buildingVariation.m_isDefault = childNode.Attributes["default"] != null;
						foreach (XmlNode childNode2 in childNode.ChildNodes)
						{
							if (buildingVariation.m_enabledSubMeshes == null)
							{
								buildingVariation.m_enabledSubMeshes = new List<string>();
							}
							if (childNode2.Name == "Submesh")
							{
								buildingVariation.m_enabledSubMeshes.Add(childNode2.InnerText);
							}
						}
						list2.Add(buildingVariation);
					}
					else if (childNode.Name == "OverrideName")
					{
						text2 = childNode.InnerText;
					}
				}
				TestProjectBuildingData.PotentialVariationsMap.Add(text2, list2);
			}
			alreadyDeserializedXML = true;
		}

		public override void OnLevelLoaded(LoadMode mode)
		{
			if (mode != LoadMode.NewGame && mode != LoadMode.LoadGame)
			{
			}
		}

		public override void OnLevelUnloading()
		{
			TestProjectBuildingData.PotentialVariationsMap.Clear();
			alreadyDeserializedXML = false;
		}
	}
}
