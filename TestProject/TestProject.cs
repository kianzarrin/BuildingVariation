using System.Reflection;
using HarmonyLib;
using CitiesHarmony;
using ICities;
using CitiesHarmony.API;

namespace TestProject
{
	public class TestProject : IUserMod
	{
		private readonly string harmonyId = "elektrix.buildingvariations";

		public string Name => "Building Variations";

		public string Description => "Submeshes can now be configured as a variation on the original building.";

		public void OnEnabled()
		{
			HarmonyHelper.DoOnHarmonyReady(
				() => new Harmony(harmonyId).PatchAll());
		}

		public void OnDisabled()
		{
            HarmonyHelper.DoOnHarmonyReady(
				() => new Harmony(harmonyId).UnpatchAll(harmonyId));
        }
    }
}
