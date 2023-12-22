using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.GameContent.UI;
using Terraria.GameContent;
using Terraria;
using Microsoft.Xna.Framework;
using ReLogic.Content;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;

namespace HappinessMeter
{
	internal class Helpers {
		internal enum HappinessLevel {
			VeryHappy,
			Happy,
			Sad,
			Mad,
		}

		internal static HappinessLevel GetHappinessLevel(float priceAdjustment) {
			if (priceAdjustment < ShopHelper.MaxHappinessAchievementPriceMultiplier) {
				return HappinessLevel.VeryHappy;
			}
			else if (priceAdjustment < 1f) {
				return HappinessLevel.Happy;
			}
			else if (priceAdjustment < 1.15f) {
				return HappinessLevel.Happy;
			}
			else if (priceAdjustment < 1.30f) {
				return HappinessLevel.Sad;
			}
			else if (priceAdjustment <= ShopHelper.HighestPossiblePriceMultiplier) {
				return HappinessLevel.Mad;
			}
			return HappinessLevel.VeryHappy;
		}

		internal static string GetHappinessText(HappinessLevel priceAdjustment) {
			return priceAdjustment switch {
				HappinessLevel.VeryHappy => Lang.GetEmojiName(EmoteID.EmoteHappiness).Value,
				HappinessLevel.Happy => Lang.GetEmojiName(EmoteID.EmoteHappiness).Value,
				HappinessLevel.Sad => Lang.GetEmojiName(EmoteID.EmoteSadness).Value,
				HappinessLevel.Mad => Lang.GetEmojiName(EmoteID.EmoteAnger).Value,
				_ => Lang.GetEmojiName(136).Value
			};
		}

		internal static Asset<Texture2D> GetResource(string path) => ModContent.Request<Texture2D>($"HappinessMeter/Resources/{path}");

		internal static void DrawTooltipBackground(string text, Color textColor = default) {
			if (text == "")
				return;

			int padd = 20;
			Vector2 stringVec = FontAssets.MouseText.Value.MeasureString(text);
			Rectangle bgPos = new Rectangle(Main.mouseX + 20, Main.mouseY + 20, (int)stringVec.X + padd, (int)stringVec.Y + padd - 5);
			bgPos.X = Utils.Clamp(bgPos.X, 0, Main.screenWidth - bgPos.Width);
			bgPos.Y = Utils.Clamp(bgPos.Y, 0, Main.screenHeight - bgPos.Height);

			Vector2 textPos = new Vector2(bgPos.X + padd / 2, bgPos.Y + padd / 2);
			if (textColor == default) {
				textColor = Main.MouseTextColorReal;
			}

			Utils.DrawInvBG(Main.spriteBatch, bgPos, new Color(23, 25, 81, 255) * 0.925f);
			Utils.DrawBorderString(Main.spriteBatch, text, textPos, textColor);
		}
	}
}
