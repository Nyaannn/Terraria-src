using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;

namespace Terraria.GameContent.UI;

public class IssueReportsIndicator
{
	private float _displayUpPercent;

	private bool _shouldBeShowing;

	private Asset<Texture2D> _buttonTexture;

	private Asset<Texture2D> _buttonOutlineTexture;

	public void AttemptLettingPlayerKnow()
	{
		Setup();
		_shouldBeShowing = true;
		SoundEngine.PlaySound(SoundID.DD2_ExplosiveTrapExplode);
	}

	public void Hide()
	{
		_shouldBeShowing = false;
		_displayUpPercent = 0f;
	}

	private void OpenUI()
	{
		Setup();
		Main.OpenReportsMenu();
	}

	private void Setup()
	{
		_buttonTexture = Main.Assets.Request<Texture2D>("Images/UI/Workshop/IssueButton", (AssetRequestMode)1);
		_buttonOutlineTexture = Main.Assets.Request<Texture2D>("Images/UI/Workshop/IssueButton_Outline", (AssetRequestMode)1);
	}

	public void Draw(SpriteBatch spriteBatch)
	{
		bool leafFrequency = _shouldBeShowing;
		_displayUpPercent = MathHelper.Clamp(_displayUpPercent + (float)leafFrequency.ToDirectionInt(), 0f, 1f);
		if (_displayUpPercent == 0f)
		{
			return;
		}
		Texture2D num = _buttonTexture.get_Value();
		Vector2 vector = Main.ScreenSize.ToVector2() + new Vector2(40f, -80f);
		Vector2 value = vector + new Vector2(-80f, 0f);
		Vector2 j = Vector2.Lerp(vector, value, _displayUpPercent);
		Rectangle k = num.Frame();
		Vector2 i = k.Size() / 2f;
		bool num2 = false;
		if (Utils.CenteredRectangle(j, k.Size()).Contains(Main.MouseScreen.ToPoint()))
		{
			num2 = true;
			string textValue = Language.GetTextValue("UI.IssueReporterHasThingsToShow");
			Main.instance.MouseText(textValue, 0, 0);
			if (Main.mouseLeft)
			{
				OpenUI();
				Hide();
				return;
			}
		}
		float velocity = 1f;
		spriteBatch.Draw(num, j, k, Color.White, 0f, i, velocity, SpriteEffects.None, 0f);
		if (num2)
		{
			Texture2D type = _buttonOutlineTexture.get_Value();
			Rectangle scale = type.Frame();
			spriteBatch.Draw(type, j, scale, Color.White, 0f, scale.Size() / 2f, velocity, SpriteEffects.None, 0f);
		}
	}
}
