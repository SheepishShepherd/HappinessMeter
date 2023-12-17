using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Terraria.ID;

namespace HappinessMeter
{
	public class HappinessMeter : Mod {
		public override void Load() {
			On_Main.DrawNPCChatBubble += DrawHeartIcon;
			On_Main.GUIChatDrawInner += DrawHappynessStats;
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
			Main.NewText(Main.npc[whoAmI].FullName);
		}

		/// <summary>
		/// Draws the heart icon meter for quick happiness checks.
		/// </summary>
		private void DrawHappynessStats(On_Main.orig_GUIChatDrawInner orig, Main self) {
			orig(self);

			if (Main.LocalPlayer.talkNPC == -1 || Main.npcChatText != Main.LocalPlayer.currentShoppingSettings.HappinessReport)
				return;

			Texture2D bar = ModContent.Request<Texture2D>("HappinessMeter/Resources/ProofOfConcept").Value;
			Main.spriteBatch.Draw(bar, new Vector2(Main.screenWidth / 2 - bar.Width / 2, 90f - bar.Height), Color.White);

			int offset = 40;
			int bar_start = Main.screenWidth / 2 - TextureAssets.ChatBack.Width() / 2 + (4 * offset);
			int bar_finish = Main.screenWidth / 2 + TextureAssets.ChatBack.Width() / 2 - (4 * (offset + 2));
			float totalWidthOfBar = bar_finish - bar_start;

			float priceMultiplier = (float)Main.ShopHelper.GetShoppingSettings(Main.LocalPlayer, Main.LocalPlayer.TalkNPC).PriceAdjustment;
			float happinessPercent = (float)(priceMultiplier - ShopHelper.LowestPossiblePriceMultiplier) / (float)(ShopHelper.HighestPossiblePriceMultiplier - ShopHelper.LowestPossiblePriceMultiplier) * 100;
			float finalPoint = bar_start + 4 + (float)(totalWidthOfBar - (float)((float)((happinessPercent) / 100) * totalWidthOfBar));

			// Draw the ends of the bar first
			Main.spriteBatch.Draw(ModContent.Request<Texture2D>("HappinessMeter/Resources/bar_left").Value, new Vector2(bar_start, 100f - 16), Color.White);
			Main.spriteBatch.Draw(ModContent.Request<Texture2D>("HappinessMeter/Resources/bar_right").Value, new Vector2(bar_finish, 100f - 16), Color.White);

			// Draw the inner portion, with a minimum of 1 bar segment
			int barPos = bar_start + 4;
			while (barPos < bar_finish + 4) {
				Main.spriteBatch.Draw(ModContent.Request<Texture2D>("HappinessMeter/Resources/bar").Value, new Vector2(barPos, 100f - 16), Color.White);
				if (barPos < finalPoint || barPos == bar_start + 4)
					Main.spriteBatch.Draw(ModContent.Request<Texture2D>("HappinessMeter/Resources/bar_fill").Value, new Vector2(barPos, 100f - 16), Color.White);
				barPos += 4;
			}
			//Main.NewText($"Happiness Price: {priceMultiplier} ({happinessPercent.ToString("#.0")}) -- X, Y: {bar_start} / {bar_finish} ({totalWidthOfBar}) -- {finalPoint}");
		}
	}
}
