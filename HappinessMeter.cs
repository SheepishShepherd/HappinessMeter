using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using Terraria.GameContent;
using Terraria.GameContent.UI;
using Terraria.ID;
using Terraria.Localization;

namespace HappinessMeter
{
	public class HappinessMeter : Mod {
		public override void Load() {
			On_Main.DrawNPCChatBubble += DrawHeartIcon;
			On_Main.GUIChatDrawInner += DrawHappynessStats;
			On_Main.DrawInventory += DrawPriceAdjustmentInShop;
		}

		/// <summary>
		/// Draws the heart icon meter for quick happiness checks.
		/// </summary>
		private void DrawHeartIcon(On_Main.orig_DrawNPCChatBubble orig, int whoAmI) {
			orig(whoAmI);

			NPC HoverNPC = Main.npc[whoAmI];

			if (NPCID.Sets.IsTownPet[HoverNPC.type])
				return;
			
			// Draw the heart here
			//Main.NewText(Main.npc[whoAmI].FullName);
		}

		private void DrawPriceAdjustmentInShop(On_Main.orig_DrawInventory orig, Main self) {
			orig(self);

			if (Main.npcShop == 0)
				return;

			float priceMultiplier = (float)Main.ShopHelper.GetShoppingSettings(Main.LocalPlayer, Main.LocalPlayer.TalkNPC).PriceAdjustment;
			float multiplierY = Main.instance.invBottom + 60f + 45f;
			Color priceColor = priceMultiplier <= 1f ? Colors.RarityGreen : Colors.RarityRed;
			string priceAdjust = Language.GetTextValue("RandomWorldName_Noun.Happiness");
			string modifierValue = $"x{priceMultiplier.ToString("0.00")} ({Helpers.GetHappinessText(Helpers.GetHappinessLevel(priceMultiplier))})";
			Utils.DrawBorderStringFourWay(Main.spriteBatch, FontAssets.MouseText.Value, priceAdjust, 504f, multiplierY, Color.White * ((float)(int)Main.mouseTextColor / 255f), Color.Black, Vector2.Zero);
			Utils.DrawBorderStringFourWay(Main.spriteBatch, FontAssets.MouseText.Value, modifierValue, 504f, multiplierY + FontAssets.MouseText.Value.MeasureString(priceAdjust).Y, priceColor * ((float)(int)Main.mouseTextColor / 255f), Color.Black, Vector2.Zero, 0.75f);
		}

		/// <summary>
		/// Draws the heart icon meter for quick happiness checks.
		/// </summary>
		private void DrawHappynessStats(On_Main.orig_GUIChatDrawInner orig, Main self) {
			orig(self);

			// If an NPC's shop is open, draw the Happiness level and price modifier just underneath the player savings
			if (Main.npcShop != 1)
				Main.spriteBatch.Draw(Helpers.GetResource("happy").Value, new Rectangle(0, Main.instance.invBottom, 10, 10), Color.White);

			// Do nothing is no NPC is being talked to or if the NPC is not talking about their happiness report
			if (Main.LocalPlayer.talkNPC == -1 || Main.npcChatText != Main.LocalPlayer.currentShoppingSettings.HappinessReport)
				return;

			int offset = 40;
			int bar_start = Main.screenWidth / 2 - TextureAssets.ChatBack.Width() / 2 + (4 * offset);
			int bar_finish = Main.screenWidth / 2 + TextureAssets.ChatBack.Width() / 2 - (4 * (offset + 2));
			float totalWidthOfBar = bar_finish - bar_start;

			float priceMultiplier = (float)Main.ShopHelper.GetShoppingSettings(Main.LocalPlayer, Main.LocalPlayer.TalkNPC).PriceAdjustment;
			float happinessPercent = (float)(priceMultiplier - ShopHelper.LowestPossiblePriceMultiplier) / (float)(ShopHelper.HighestPossiblePriceMultiplier - ShopHelper.LowestPossiblePriceMultiplier) * 100;
			float finalPoint = bar_start + 4 + (float)(totalWidthOfBar - (float)((float)((happinessPercent) / 100) * totalWidthOfBar));

			// Draw the background
			Utils.WordwrapString(Main.npcChatText, FontAssets.MouseText.Value, 460, 10, out int lineAmount); // found in PrepareCache method
			DrawHappinessCornerIcon(lineAmount);
			int HappyPanelTop = 100 + ((lineAmount + 3) * 30) + 5;
			Vector2 HappyPanelPos = new Vector2(Main.screenWidth / 2 - ((int)totalWidthOfBar + offset) / 2, HappyPanelTop);
			Utils.DrawInvBG(Main.spriteBatch, new Rectangle((int)HappyPanelPos.X, (int)HappyPanelPos.Y, (int)totalWidthOfBar + 40, 50), new Color(23, 25, 81, 255) * 0.925f);

			// Draw the ends of the bar first
			Main.spriteBatch.Draw(Helpers.GetResource("bar_left").Value, new Vector2(bar_start, HappyPanelTop + offset / 2), Color.White);
			Main.spriteBatch.Draw(Helpers.GetResource("bar_right").Value, new Vector2(bar_finish, HappyPanelTop + offset / 2), Color.White);

			// Draw the inner portion, with a minimum of 1 bar segment
			int barPos = bar_start + 4;
			while (barPos < bar_finish + 4) {
				Main.spriteBatch.Draw(Helpers.GetResource("bar").Value, new Vector2(barPos, HappyPanelTop + offset / 2), Color.White);
				if (barPos < finalPoint || barPos == bar_start + 4)
					Main.spriteBatch.Draw(Helpers.GetResource("bar_fill").Value, new Vector2(barPos, HappyPanelTop + offset / 2), Color.White);
				barPos += 4;
			}

			if (Main.MouseScreen.Between(new Vector2(bar_start, HappyPanelTop + offset / 2), new Vector2(bar_finish, HappyPanelTop + offset / 2 + 14))) {
				Helpers.DrawTooltipBackground((100 - happinessPercent).ToString("#.0") + $" (Price Multiplier: {priceMultiplier})");
			}
			//Main.NewText($"Happiness Price: {priceMultiplier} ({happinessPercent.ToString("#.0")}) -- X, Y: {bar_start} / {bar_finish} ({totalWidthOfBar}) -- {finalPoint}");
		}

		private void DrawHappinessCornerIcon(int lineCount) {
			Main.npcChatCornerItem = 0;
			Vector2 position = new Vector2(Main.screenWidth / 2 + TextureAssets.ChatBack.Width() / 2, 100 + (lineCount + 2) * 30 + 30);
			position -= Vector2.One * 8f; // position from: Main.GUIChatDrawInner
			Texture2D value = Helpers.GetResource("happy").Value;
			Main.spriteBatch.Draw(value, position, null, Color.White, 0f, new Vector2(value.Width, value.Height), 1f, SpriteEffects.None, 0f);
		}
	}
}
