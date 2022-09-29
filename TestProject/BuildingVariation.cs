using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TestProject
{
	public class BuildingVariation
	{
		public string m_publicName;

		public bool m_isDefault;

		public List<string> m_enabledSubMeshes;

		public BuildingVariation()
		{
			m_publicName = "Public Name";
			m_isDefault = false;
			m_enabledSubMeshes = new List<string>();
		}

		public override string ToString()
		{
			return m_publicName + (m_isDefault ? "[default] " : " ") + m_enabledSubMeshes.Count + " enabled submeshes";
		}

		public string ToSerializableString()
		{
			string text = "";
			for (int i = 0; i < m_enabledSubMeshes.Count; i++)
			{
				text = text + m_enabledSubMeshes[i] + ((i == m_enabledSubMeshes.Count - 1) ? "," : "");
			}
			return m_publicName + ":" + m_isDefault + ":" + text;
		}

		public static BuildingVariation FromSerializedString(string s)
		{
			BuildingVariation buildingVariation = new BuildingVariation();
			List<string> list = s.Split(':').ToList();
			buildingVariation.m_publicName = list[0];
			Debug.LogError("[Building Variations] " + list[1]);
			buildingVariation.m_isDefault = list[1].ToLower() == "true";
			buildingVariation.m_enabledSubMeshes = list[2].Split(',').ToList();
			return buildingVariation;
		}
	}
}
