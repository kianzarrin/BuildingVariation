using System.Collections.Generic;
using System.Linq;
using ColossalFramework;
using ColossalFramework.UI;
using HarmonyLib;
using ICities;
using UnityEngine;

namespace TestProject
{
	public class TestProjectLoading : LoadingExtensionBase
	{
		public static UIDropDown m_variationDropdown;

		public static UIPanel m_variationPanel;

		public UICheckboxDropDown AE_submeshDropdown;

		public UIDropDown AE_variationDropdown;

		public UITextField AE_nameField;

		public UIButton AE_addVariationButton;

		public UIButton AE_remVariationButton;

		public UICheckBox AE_setDefaultCheckbox;

		public UIPanel AE_variationPanel;

		public static List<BuildingVariation> buildingVariations = new List<BuildingVariation>();

		public int currentVariation = 0;

		public bool m_isMinimum;

		public bool m_isMaximum;

		public static InstanceID Instance => WorldInfoPanel.GetCurrentInstanceID();

		public override void OnLevelLoaded(LoadMode mode)
		{
			Object.FindObjectOfType<ToolController>().eventEditPrefabChanged += delegate(PrefabInfo info)
			{
				if (info is BuildingInfo)
				{
					BuildingInfo buildingInfo = info as BuildingInfo;
					string[] array = TestProjectBuildingData.PotentialVariationsMap.Keys.ToArray();
					for (int i = 0; i < array.Length; i++)
					{
						if (array[i].Split('.').Length < 2)
						{
							Debug.LogWarning("[BuildingVariations] Malformed object name, aborting (at " + array[i] + ")");
						}
						if (array[i].Split('.')[1] == buildingInfo.name)
						{
							buildingVariations = TestProjectBuildingData.PotentialVariationsMap[array[i]];
						}
					}
					UpdateHelperPanel(info);
				}
			};
			if (!(UIView.library.Get<ZonedBuildingWorldInfoPanel>(typeof(ZonedBuildingWorldInfoPanel).Name) == null))
			{
				m_variationDropdown = UIView.library.Get<ZonedBuildingWorldInfoPanel>(typeof(ZonedBuildingWorldInfoPanel).Name).component.AddUIComponent<UIDropDown>();
				m_variationDropdown.name = "VariationDropdownZoned";
				m_variationDropdown.normalBgSprite = "OptionsDropbox";
				m_variationDropdown.hoveredBgSprite = "OptionsDropboxHovered";
				m_variationDropdown.focusedBgSprite = "OptionsDropboxFocused";
				m_variationDropdown.listBackground = "OptionsDropboxListbox";
				m_variationDropdown.listHeight = 200;
				m_variationDropdown.itemHeight = 24;
				m_variationDropdown.itemHover = "ListItemHover";
				m_variationDropdown.itemHighlight = "ListItemHighlight";
				m_variationDropdown.itemPadding = new RectOffset(14, 14, 0, 0);
				m_variationDropdown.isVisible = false;
				m_variationDropdown.clipChildren = true;
				m_variationDropdown.height = 25f;
				m_variationDropdown.width = 200f;
				m_variationDropdown.relativePosition = new Vector3(260f, m_variationDropdown.parent.height - 50f);
				m_variationDropdown.textScale = 0.8f;
				m_variationDropdown.listPosition = UIDropDown.PopupListPosition.Automatic;
				m_variationDropdown.listPadding = new RectOffset(4, 4, 4, 4);
				m_variationDropdown.textFieldPadding = new RectOffset(8, 0, 8, 0);
				m_variationDropdown.triggerButton = m_variationDropdown;
				m_variationDropdown.tooltip = "Select Building Variations";
				m_variationDropdown.listScrollbar = new UIScrollbar();
				m_variationDropdown.eventSelectedIndexChanged += delegate(UIComponent component, int value)
				{
					ushort building = Instance.Building;
					Building building2 = Singleton<BuildingManager>.instance.m_buildings.m_buffer[building];
					TestProjectBuildingData.IngameBuildingVariationMap[building] = (byte)(value + 1);
				};
			}
		}

		public string[] GetVariationNamesFrom(List<BuildingVariation> variations)
		{
			List<string> list = new List<string>();
			foreach (BuildingVariation variation in variations)
			{
				list.Add(variation.m_publicName);
			}
			return list.ToArray();
		}

		public void UpdateHelperPanel(PrefabInfo info)
		{
			UpdateHelperPanel(info, useToolController: false);
		}

		public void UpdateHelperPanel(PrefabInfo info, bool useToolController)
		{
			Debug.Log("[Building Variations] UpdateHelperPanel fired");
			if (useToolController)
			{
				info = Object.FindObjectOfType<ToolController>().m_editPrefabInfo;
			}
			if (info == null)
			{
				return;
			}
			BuildingInfo buildingInfo = info as BuildingInfo;
			int index = 0;
			for (int i = 0; i < buildingVariations.Count; i++)
			{
				if (buildingVariations[i].m_isDefault)
				{
					index = i;
					break;
				}
			}
			BuildingInfo.MeshInfo[] subMeshes = buildingInfo.m_subMeshes;
			for (int j = 0; j < subMeshes.Length; j++)
			{
				if (!buildingVariations[index].m_enabledSubMeshes.Contains(subMeshes[j].m_subInfo.name))
				{
					subMeshes[j].m_flagsForbidden |= Building.Flags.Created;
				}
				else
				{
					subMeshes[j].m_flagsForbidden &= ~Building.Flags.Created;
				}
			}
			if (m_variationPanel == null && info is BuildingInfo)
			{
				CreateVariationHelperPanel();
			}
			m_isMinimum = buildingVariations.Count == 0;
			m_isMaximum = buildingVariations.Count == 255;
			AE_submeshDropdown.isVisible = !m_isMinimum;
			AE_variationDropdown.isVisible = !m_isMinimum;
			AE_remVariationButton.isVisible = !m_isMinimum;
			AE_addVariationButton.isVisible = !m_isMaximum;
			AE_setDefaultCheckbox.isVisible = !m_isMinimum;
			AE_nameField.isVisible = !m_isMinimum;
			if (m_isMinimum)
			{
				return;
			}
			List<string> list = new List<string>();
			int num = 0;
			BuildingInfo.MeshInfo[] subMeshes2 = buildingInfo.m_subMeshes;
			foreach (BuildingInfo.MeshInfo meshInfo in subMeshes2)
			{
				if (!(meshInfo.m_subInfo == null))
				{
					list.Add(meshInfo.m_subInfo.name);
					num++;
				}
			}
			string[] array = list.ToArray();
			bool flag = array.Length != 0;
			if (array != AE_submeshDropdown.items)
			{
				AE_submeshDropdown.items = array;
				for (int l = 0; l < AE_submeshDropdown.items.Length; l++)
				{
					AE_submeshDropdown.SetChecked(l, buildingVariations[currentVariation].m_enabledSubMeshes.Contains(AE_submeshDropdown.items[l]));
				}
			}
			(AE_submeshDropdown.triggerButton as UIButton).text = buildingVariations[currentVariation].m_enabledSubMeshes.Join();
			AE_variationDropdown.items = GetVariationNamesFrom(buildingVariations);
			(AE_variationDropdown.triggerButton as UIButton).text = buildingVariations[currentVariation].m_publicName;
			AE_nameField.text = buildingVariations[currentVariation].m_publicName;
			AE_setDefaultCheckbox.isChecked = buildingVariations[currentVariation].m_isDefault;
		}

		public bool CreateVariationHelperPanel()
		{
			if (Object.FindObjectOfType<ToolController>().m_editPrefabInfo == null || m_variationPanel != null)
			{
				return false;
			}
			AE_variationPanel = UIView.Find("FullScreenContainer").AddUIComponent<UIPanel>();
			AE_variationPanel.width = 400f;
			AE_variationPanel.height = 160f;
			AE_variationPanel.backgroundSprite = "MenuPanel2";
			AE_variationPanel.name = "VariationPanel";
			AE_variationPanel.absolutePosition = new Vector3(200f, 250f);
			UISlicedSprite uISlicedSprite = AE_variationPanel.AddUIComponent<UISlicedSprite>();
			uISlicedSprite.width = 400f;
			uISlicedSprite.height = 40f;
			uISlicedSprite.name = "VariationPanelSub";
			uISlicedSprite.relativePosition = Vector3.zero;
			UILabel uILabel = uISlicedSprite.AddUIComponent<UILabel>();
			uILabel.anchor = UIAnchorStyle.CenterHorizontal | UIAnchorStyle.CenterVertical;
			uILabel.height = 23f;
			uILabel.text = "Building Variations Helper";
			uILabel.name = "VariationPanelLabel";
			uILabel.textScale = 1.3f;
			UIDragHandle uIDragHandle = AE_variationPanel.AddUIComponent<UIDragHandle>();
			uIDragHandle.width = 400f;
			uIDragHandle.height = 40f;
			uIDragHandle.relativePosition = Vector3.zero;
			uIDragHandle.target = AE_variationPanel;
			uIDragHandle.BringToFront();
			UISlicedSprite uISlicedSprite2 = AE_variationPanel.AddUIComponent<UISlicedSprite>();
			uISlicedSprite2.width = 400f;
			uISlicedSprite2.height = 30f;
			uISlicedSprite2.name = "VariationSelectorBase";
			uISlicedSprite2.relativePosition = new Vector3(0f, 40f);
			UILabel uILabel2 = uISlicedSprite2.AddUIComponent<UILabel>();
			uILabel2.text = "Variation";
			uILabel2.textColor = new Color32(125, 185, byte.MaxValue, byte.MaxValue);
			uILabel2.relativePosition = new Vector3(10f, 12f);
			uILabel2.name = "VariationSelectorLabel";
			AE_variationDropdown = uISlicedSprite2.AddUIComponent<UIDropDown>();
			AE_variationDropdown.relativePosition = new Vector3(246f, 12f);
			AE_variationDropdown.name = "VariationDropdown";
			AE_variationDropdown.itemHighlight = "ListItemHighlight";
			AE_variationDropdown.itemHover = "ListItemHover";
			AE_variationDropdown.itemHeight = 25;
			AE_variationDropdown.listBackground = "InfoDisplay";
			AE_variationDropdown.listHeight = 400;
			AE_variationDropdown.listWidth = 300;
			AE_variationDropdown.listPosition = UIDropDown.PopupListPosition.Automatic;
			AE_variationDropdown.normalBgSprite = "TextFieldPanel";
			AE_variationDropdown.size = new Vector2(150f, 20f);
			AE_variationDropdown.atlas = UIView.GetAView().defaultAtlas;
			AE_variationDropdown.horizontalAlignment = UIHorizontalAlignment.Right;
			AE_variationDropdown.eventSelectedIndexChanged += delegate(UIComponent component, int value)
			{
				if (AE_variationDropdown.selectedIndex != currentVariation)
				{
					currentVariation = value;
					UpdateHelperPanel(null, useToolController: true);
				}
			};
			UIButton dropdownListTriggerButton = AE_variationDropdown.AddUIComponent<UIButton>();
			dropdownListTriggerButton.normalBgSprite = "TextFieldPanel";
			dropdownListTriggerButton.normalFgSprite = (dropdownListTriggerButton.hoveredFgSprite = "IconUpArrow");
			dropdownListTriggerButton.clipChildren = true;
			dropdownListTriggerButton.text = "None";
			dropdownListTriggerButton.textColor = new Color32(0, 0, 0, byte.MaxValue);
			dropdownListTriggerButton.hoveredTextColor = (dropdownListTriggerButton.focusedTextColor = (dropdownListTriggerButton.pressedTextColor = new Color32(28, 50, 52, byte.MaxValue)));
			dropdownListTriggerButton.hoveredColor = new Color32(200, 200, 200, byte.MaxValue);
			dropdownListTriggerButton.size = AE_variationDropdown.size;
			dropdownListTriggerButton.relativePosition = Vector3.zero;
			dropdownListTriggerButton.name = "VariationListTriggerButton";
			AE_variationDropdown.triggerButton = dropdownListTriggerButton;
			AE_addVariationButton = uISlicedSprite2.AddUIComponent<UIButton>();
			AE_addVariationButton.height = 20f;
			AE_addVariationButton.width = 30f;
			AE_addVariationButton.textColor = new Color32(0, 0, 0, byte.MaxValue);
			AE_addVariationButton.hoveredColor = new Color32(200, 200, 200, byte.MaxValue);
			AE_addVariationButton.hoveredTextColor = (AE_addVariationButton.focusedTextColor = (AE_addVariationButton.pressedTextColor = new Color32(28, 50, 52, byte.MaxValue)));
			AE_addVariationButton.text = "+";
			AE_addVariationButton.relativePosition = new Vector3(160f, 12f);
			AE_addVariationButton.normalBgSprite = "TextFieldPanel";
			AE_addVariationButton.name = "AddVariation";
			AE_addVariationButton.eventClick += delegate
			{
				buildingVariations.Add(new BuildingVariation());
				currentVariation = buildingVariations.Count - 1;
				UpdateHelperPanel(null, useToolController: true);
			};
			AE_remVariationButton = uISlicedSprite2.AddUIComponent<UIButton>();
			AE_remVariationButton.height = 20f;
			AE_remVariationButton.width = 30f;
			AE_remVariationButton.textColor = new Color32(0, 0, 0, byte.MaxValue);
			AE_remVariationButton.hoveredColor = new Color32(200, 200, 200, byte.MaxValue);
			AE_remVariationButton.hoveredTextColor = (AE_remVariationButton.focusedTextColor = (AE_remVariationButton.pressedTextColor = new Color32(28, 50, 52, byte.MaxValue)));
			AE_remVariationButton.text = "-";
			AE_remVariationButton.relativePosition = new Vector3(200f, 12f);
			AE_remVariationButton.normalBgSprite = "TextFieldPanel";
			AE_remVariationButton.name = "RemVariation";
			AE_remVariationButton.eventClick += delegate
			{
				buildingVariations.RemoveAt(currentVariation);
				currentVariation--;
				if (currentVariation < 0)
				{
					currentVariation = 0;
				}
				UpdateHelperPanel(null, useToolController: true);
			};
			UIButton uIButton = uISlicedSprite2.AddUIComponent<UIButton>();
			uIButton.normalFgSprite = "IconUpArrow";
			uIButton.foregroundSpriteMode = UIForegroundSpriteMode.Scale;
			uIButton.relativePosition = (Vector2)AE_variationDropdown.relativePosition + AE_variationDropdown.size - new Vector2(20f, 20f);
			uIButton.size = new Vector2(20f, 20f);
			uIButton.eventClick += delegate
			{
				dropdownListTriggerButton.SimulateClick();
			};
			uIButton.name = "SubmeshListUpArrowFakeButton";
			uIButton.atlas = UIView.Find<UIDropDown>("CellLength").Find<UIButton>("Button").atlas;
			uIButton.BringToFront();
			UISlicedSprite uISlicedSprite3 = AE_variationPanel.AddUIComponent<UISlicedSprite>();
			uISlicedSprite3.width = 400f;
			uISlicedSprite3.height = 25f;
			uISlicedSprite3.name = "VariationNameBase";
			uISlicedSprite3.relativePosition = new Vector3(0f, 65f);
			UILabel uILabel3 = uISlicedSprite3.AddUIComponent<UILabel>();
			uILabel3.text = "Name";
			uILabel3.textColor = new Color32(125, 185, byte.MaxValue, byte.MaxValue);
			uILabel3.relativePosition = new Vector3(10f, 12f);
			uILabel3.name = "VariationNameLabel";
			AE_nameField = uISlicedSprite3.AddUIComponent<UITextField>();
			AE_nameField.size = new Vector2(150f, 20f);
			AE_nameField.normalBgSprite = "TextFieldPanel";
			AE_nameField.relativePosition = new Vector3(246f, 12f);
			AE_nameField.cursorWidth = 1;
			AE_nameField.cursorBlinkTime = 0.45f;
			AE_nameField.foregroundSpriteMode = UIForegroundSpriteMode.Stretch;
			AE_nameField.maxLength = 35;
			AE_nameField.textColor = new Color32(12, 21, 22, byte.MaxValue);
			AE_nameField.canFocus = true;
			AE_nameField.readOnly = false;
			AE_nameField.bottomColor = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);
			AE_nameField.selectionBackgroundColor = new Color32(0, 105, 210, byte.MaxValue);
			AE_nameField.builtinKeyNavigation = true;
			AE_nameField.selectionSprite = "EmptySprite";
			AE_nameField.name = "VariationName";
			AE_nameField.eventMouseEnter += delegate
			{
				AE_nameField.color = new Color32(200, 200, 200, byte.MaxValue);
			};
			AE_nameField.eventMouseLeave += delegate
			{
				AE_nameField.color = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);
			};
			AE_nameField.eventTextChanged += delegate(UIComponent component, string text)
			{
				buildingVariations[currentVariation].m_publicName = text;
				UpdateHelperPanel(null, useToolController: true);
			};
			UISlicedSprite uISlicedSprite4 = AE_variationPanel.AddUIComponent<UISlicedSprite>();
			uISlicedSprite4.width = 400f;
			uISlicedSprite4.height = 25f;
			uISlicedSprite4.name = "SubmeshSelectorBase";
			uISlicedSprite4.relativePosition = new Vector3(0f, 90f);
			UILabel uILabel4 = uISlicedSprite4.AddUIComponent<UILabel>();
			uILabel4.text = "Included Submeshes";
			uILabel4.textColor = new Color32(125, 185, byte.MaxValue, byte.MaxValue);
			uILabel4.relativePosition = new Vector3(10f, 12f);
			uILabel4.name = "SubmeshSelectorLabel";
			AE_submeshDropdown = uISlicedSprite4.AddUIComponent<UICheckboxDropDown>();
			AE_submeshDropdown.relativePosition = new Vector3(246f, 12f);
			AE_submeshDropdown.name = "SubmeshDropdown";
			AE_submeshDropdown.checkedSprite = "check-checked";
			AE_submeshDropdown.itemHighlight = "ListItemHighlight";
			AE_submeshDropdown.itemHover = "ListItemHover";
			AE_submeshDropdown.itemHeight = 25;
			AE_submeshDropdown.listBackground = "InfoDisplay";
			AE_submeshDropdown.listHeight = 400;
			AE_submeshDropdown.listWidth = 300;
			AE_submeshDropdown.listPosition = UICheckboxDropDown.PopupListPosition.Automatic;
			AE_submeshDropdown.uncheckedSprite = "check-unchecked";
			AE_submeshDropdown.normalBgSprite = "TextFieldPanel";
			AE_submeshDropdown.size = new Vector2(150f, 20f);
			AE_submeshDropdown.foregroundSpriteMode = UIForegroundSpriteMode.Scale;
			AE_submeshDropdown.horizontalAlignment = UIHorizontalAlignment.Right;
			AE_submeshDropdown.atlas = UIView.GetAView().defaultAtlas;
			AE_submeshDropdown.eventAfterDropdownClose += delegate
			{
				if (AE_submeshDropdown.items.Length != 0)
				{
					for (int i = 0; i < AE_submeshDropdown.items.Length; i++)
					{
						string item = AE_submeshDropdown.items[i];
						if (AE_submeshDropdown.GetChecked(i))
						{
							if (!buildingVariations[currentVariation].m_enabledSubMeshes.Contains(item))
							{
								buildingVariations[currentVariation].m_enabledSubMeshes.Add(item);
							}
						}
						else
						{
							buildingVariations[currentVariation].m_enabledSubMeshes.Remove(item);
						}
					}
					UpdateHelperPanel(null, useToolController: true);
				}
			};
			UIButton submeshListTriggerButton = AE_submeshDropdown.AddUIComponent<UIButton>();
			submeshListTriggerButton.normalBgSprite = "TextFieldPanel";
			submeshListTriggerButton.normalFgSprite = (submeshListTriggerButton.hoveredFgSprite = "IconUpArrow");
			submeshListTriggerButton.clipChildren = true;
			submeshListTriggerButton.text = "None";
			submeshListTriggerButton.textColor = (submeshListTriggerButton.hoveredTextColor = (submeshListTriggerButton.focusedTextColor = (submeshListTriggerButton.pressedTextColor = new Color32(0, 0, 0, byte.MaxValue))));
			submeshListTriggerButton.size = AE_submeshDropdown.size;
			submeshListTriggerButton.relativePosition = Vector3.zero;
			submeshListTriggerButton.name = "SubmeshListTriggerButton";
			submeshListTriggerButton.hoveredColor = new Color32(200, 200, 200, byte.MaxValue);
			UIButton uIButton2 = uISlicedSprite4.AddUIComponent<UIButton>();
			uIButton2.normalFgSprite = "IconUpArrow";
			uIButton2.foregroundSpriteMode = UIForegroundSpriteMode.Scale;
			uIButton2.relativePosition = (Vector2)AE_submeshDropdown.relativePosition + AE_submeshDropdown.size - new Vector2(20f, 20f);
			uIButton2.size = new Vector2(20f, 20f);
			uIButton2.eventClick += delegate
			{
				submeshListTriggerButton.SimulateClick();
			};
			uIButton2.name = "SubmeshListUpArrowFakeButton";
			uIButton2.atlas = UIView.Find<UIDropDown>("CellLength").Find<UIButton>("Button").atlas;
			uIButton2.BringToFront();
			AE_submeshDropdown.listScrollbar = UIView.Find("Scrollbar") as UIScrollbar;
			AE_submeshDropdown.triggerButton = submeshListTriggerButton;
			UISlicedSprite uISlicedSprite5 = AE_variationPanel.AddUIComponent<UISlicedSprite>();
			uISlicedSprite5.width = 400f;
			uISlicedSprite5.height = 25f;
			uISlicedSprite5.name = "SetDefaultBase";
			uISlicedSprite5.relativePosition = new Vector3(0f, 115f);
			UILabel uILabel5 = uISlicedSprite5.AddUIComponent<UILabel>();
			uILabel5.text = "Is Default?";
			uILabel5.textColor = new Color32(125, 185, byte.MaxValue, byte.MaxValue);
			uILabel5.relativePosition = new Vector3(10f, 12f);
			uILabel5.name = "SetDefaultLabel";
			AE_setDefaultCheckbox = uISlicedSprite5.AddUIComponent<UICheckBox>();
			AE_setDefaultCheckbox.relativePosition = new Vector3(375f, 12f);
			AE_setDefaultCheckbox.size = new Vector2(16f, 16f);
			AE_setDefaultCheckbox.name = "VariationDefault";
			UISprite uISprite = AE_setDefaultCheckbox.AddUIComponent<UISprite>();
			uISprite.spriteName = "check-unchecked";
			uISprite.atlas = UIView.Find<UIDropDown>("CellLength").Find<UIButton>("Button").atlas;
			uISprite.size = AE_setDefaultCheckbox.size;
			uISprite.relativePosition = Vector3.zero;
			AE_setDefaultCheckbox.checkedBoxObject = uISprite.AddUIComponent<UISprite>();
			((UISprite)AE_setDefaultCheckbox.checkedBoxObject).spriteName = "check-checked";
			((UISprite)AE_setDefaultCheckbox.checkedBoxObject).atlas = UIView.Find<UIDropDown>("CellLength").Find<UIButton>("Button").atlas;
			AE_setDefaultCheckbox.checkedBoxObject.size = AE_setDefaultCheckbox.size;
			AE_setDefaultCheckbox.checkedBoxObject.relativePosition = Vector3.zero;
			AE_setDefaultCheckbox.eventCheckChanged += delegate(UIComponent component, bool check)
			{
				buildingVariations[currentVariation].m_isDefault = check;
				UpdateHelperPanel(null, useToolController: true);
			};
			m_variationPanel = AE_variationPanel;
			Debug.Log("[Building Variations] Created asset editor variation panel.");
			return true;
		}

		public override void OnLevelUnloading()
		{
			if (m_variationDropdown != null)
			{
				m_variationDropdown.parent.RemoveUIComponent(m_variationDropdown);
			}
			AE_nameField?.parent.RemoveUIComponent(AE_nameField);
			AE_submeshDropdown?.parent.RemoveUIComponent(AE_submeshDropdown);
			AE_variationDropdown?.parent.RemoveUIComponent(AE_variationDropdown);
		}
	}
}
