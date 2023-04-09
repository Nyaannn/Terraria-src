using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.DataStructures;
using Terraria.GameContent.Animations;
using Terraria.ID;
using Terraria.Localization;

namespace Terraria.GameContent.Skies.CreditsRoll;

public class CreditsRollComposer
{
	private struct SimplifiedNPCInfo
	{
		private Vector2 _simplifiedPosition;

		private int _npcType;

		public SimplifiedNPCInfo(int npcType, Vector2 simplifiedPosition)
		{
			_simplifiedPosition = simplifiedPosition;
			_npcType = npcType;
		}

		public void SpawnNPC(AddNPCMethod methodToUse, Vector2 baseAnchor, int startTime, int totalSceneTime)
		{
			Vector2 properPosition = GetProperPosition(baseAnchor);
			int lookDirection = ((!(_simplifiedPosition.X > 0f)) ? 1 : (-1));
			int num = 240;
			int timeToJumpAt = (totalSceneTime - num) / 2 - 20 + (int)(_simplifiedPosition.X * -8f);
			methodToUse(_npcType, properPosition, lookDirection, startTime, totalSceneTime, timeToJumpAt);
		}

		private Vector2 GetProperPosition(Vector2 baseAnchor)
		{
			return baseAnchor + _simplifiedPosition * new Vector2(26f, 24f);
		}
	}

	private delegate void AddNPCMethod(int npcType, Vector2 sceneAnchoePosition, int lookDirection, int fromTime, int duration, int timeToJumpAt);

	private Vector2 _originAtBottom = new Vector2(0.5f, 1f);

	private Vector2 _emoteBubbleOffsetWhenOnLeft = new Vector2(-14f, -38f);

	private Vector2 _emoteBubbleOffsetWhenOnRight = new Vector2(14f, -38f);

	private Vector2 _backgroundOffset = new Vector2(76f, 166f);

	private int _endTime;

	private List<IAnimationSegment> _segments;

	public void FillSegments_Test(List<IAnimationSegment> segmentsList, out int endTime)
	{
		_segments = segmentsList;
		int num = 0;
		int num2 = 80;
		Vector2 sceneAnchorPosition = Vector2.UnitY * -1f * num2;
		num += PlaySegment_PrincessAndEveryoneThanksPlayer(num, sceneAnchorPosition).totalTime;
		_endTime = num + 20;
		endTime = _endTime;
	}

	public void FillSegments(List<IAnimationSegment> segmentsList, out int endTime, bool inGame)
	{
		_segments = segmentsList;
		int i = 0;
		int num = 80;
		Vector2 vector = Vector2.UnitY * -1f * num;
		int num2 = 210;
		Vector2 vector2 = vector + Vector2.UnitX * 200f;
		Vector2 vector3 = vector2;
		if (!inGame)
		{
			vector3 = (vector2 = Vector2.UnitY * 80f);
		}
		int num3 = num2 * 3;
		int num4 = num2 * 3;
		int num5 = num3 - num4;
		if (!inGame)
		{
			num4 = 180;
			num5 = num3 - num4;
		}
		i += num4;
		i += PlaySegment_TextRoll(i, "CreditsRollCategory_Creator", vector2).totalTime;
		i += num2;
		i += PlaySegment_Grox_GuideRunningFromZombie(i, vector2).totalTime;
		i += num2;
		vector2.X *= -1f;
		i += PlaySegment_TextRoll(i, "CreditsRollCategory_ExecutiveProducer", vector2).totalTime;
		i += num2;
		i += PlaySegment_Grox_MerchantAndTravelingMerchantTryingToSellJunk(i, vector2).totalTime;
		i += num2;
		vector2.X *= -1f;
		i += PlaySegment_TextRoll(i, "CreditsRollCategory_Designer", vector2).totalTime;
		i += num2;
		i += PlaySegment_Grox_DemolitionistAndArmsDealerArguingThenNurseComes(i, vector2).totalTime;
		i += num2;
		vector2.X *= -1f;
		i += PlaySegment_TextRoll(i, "CreditsRollCategory_Programming", vector2).totalTime;
		i += num2;
		i += PlaySegment_TinkererAndMechanic(i, vector2).totalTime;
		i += num2;
		vector2.X *= 0f;
		i += PlaySegment_TextRoll(i, "CreditsRollCategory_Graphics", vector2).totalTime;
		i += num2;
		i += PlaySegment_Grox_DryadSayingByeToTavernKeep(i, vector2).totalTime;
		i += num2;
		vector2 = vector3;
		vector2.X *= -1f;
		i += PlaySegment_TextRoll(i, "CreditsRollCategory_Music", vector2).totalTime;
		i += num2;
		i += PlaySegment_Grox_WizardPartyGirlDyeTraderAndPainterPartyWithBunnies(i, vector2).totalTime;
		i += num2;
		vector2.X *= -1f;
		i += PlaySegment_TextRoll(i, "CreditsRollCategory_Sound", vector2).totalTime;
		i += num2;
		i += PlaySegment_ClothierChasingTruffle(i, vector2).totalTime;
		i += num2;
		vector2.X *= -1f;
		i += PlaySegment_TextRoll(i, "CreditsRollCategory_Dialog", vector2).totalTime;
		i += num2;
		i += PlaySegment_Grox_AnglerAndPirateTalkAboutFish(i, vector2).totalTime;
		i += num2;
		vector2.X *= 0f;
		i += PlaySegment_TextRoll(i, "CreditsRollCategory_QualityAssurance", vector2).totalTime;
		i += num2;
		i += PlaySegment_Grox_ZoologistAndPetsAnnoyGolfer(i, vector2).totalTime;
		i += num2;
		vector2 = vector3;
		vector2.X *= -1f;
		i += PlaySegment_TextRoll(i, "CreditsRollCategory_BusinessDevelopment", vector2).totalTime;
		i += num2;
		i += PlaySegment_Grox_SkeletonMerchantSearchesThroughBones(i, vector2).totalTime;
		i += num2;
		vector2.X *= -1f;
		i += PlaySegment_TextRoll(i, "CreditsRollCategory_Marketing", vector2).totalTime;
		i += num2;
		i += PlaySegment_DryadTurningToTree(i, vector2).totalTime;
		i += num2;
		vector2.X *= -1f;
		i += PlaySegment_TextRoll(i, "CreditsRollCategory_PublicRelations", vector2).totalTime;
		i += num2;
		i += PlaySegment_Grox_SteampunkerRepairingCyborg(i, vector2).totalTime;
		i += num2;
		vector2.X *= 0f;
		i += PlaySegment_TextRoll(i, "CreditsRollCategory_Webmaster", vector2).totalTime;
		i += num2;
		i += PlaySegment_Grox_SantaAndTaxCollectorThrowingPresents(i, vector2).totalTime;
		i += num2;
		i += PlaySegment_TextRoll(i, "CreditsRollCategory_Playtesting", vector2).totalTime;
		i += num2;
		i += PlaySegment_Grox_WitchDoctorGoingToHisPeople(i, vector2).totalTime;
		i += num2;
		i += PlaySegment_TextRoll(i, "CreditsRollCategory_SpecialThanksto", vector2).totalTime;
		i += num2;
		i += PlaySegment_PrincessAndEveryoneThanksPlayer(i, vector2).totalTime;
		i += num2;
		i += PlaySegment_TextRoll(i, "CreditsRollCategory_EndingNotes", vector2).totalTime;
		i += num5;
		_endTime = i + 10;
		endTime = _endTime;
	}

	private SegmentInforReport PlaySegment_PrincessAndEveryoneThanksPlayer(int startTime, Vector2 sceneAnchorPosition)
	{
		sceneAnchorPosition.Y += 40f;
		int unscaledPosition = -2;
		int value = 2;
		List<int> firstTileX = new List<int> { 228, 178, 550, 208, 160, 209 };
		List<int> lastTileX = new List<int> { 353, 633, 207, 588, 227, 368 };
		List<int> firstTileY = new List<int> { 22, 19, 18, 17, 38, 54, 108 };
		List<int> lastTileY = new List<int> { 663, 20, 441, 107, 124, 229, 369 };
		List<SimplifiedNPCInfo> list = new List<SimplifiedNPCInfo>();
		for (int i = 0; i < firstTileX.Count; i++)
		{
			int npcType = firstTileX[i];
			list.Add(new SimplifiedNPCInfo(npcType, new Vector2(unscaledPosition - i, -1f)));
		}
		for (int j = 0; j < firstTileY.Count; j++)
		{
			int npcType2 = firstTileY[j];
			list.Add(new SimplifiedNPCInfo(npcType2, new Vector2((float)(unscaledPosition - j) + 0.5f, 0f)));
		}
		for (int k = 0; k < lastTileX.Count; k++)
		{
			int npcType3 = lastTileX[k];
			list.Add(new SimplifiedNPCInfo(npcType3, new Vector2(value + k, -1f)));
		}
		for (int l = 0; l < lastTileY.Count; l++)
		{
			int npcType4 = lastTileY[l];
			list.Add(new SimplifiedNPCInfo(npcType4, new Vector2((float)(value + l) - 0.5f, 0f)));
		}
		int num = 240;
		int num2 = 400;
		int num3 = num2 + num;
		Asset<Texture2D> val = TextureAssets.Extra[241];
		Rectangle rectangle = val.Frame();
		DrawData data = new DrawData(val.get_Value(), Vector2.Zero, rectangle, Color.White, 0f, rectangle.Size() * new Vector2(0.5f, 1f) + new Vector2(0f, -92f), 1f, SpriteEffects.None);
		Segments.AnimationSegmentWithActions<Segments.LooseSprite> item = new Segments.SpriteSegment(val, startTime, data, sceneAnchorPosition).Then(new Actions.Sprites.Fade(0f)).With(new Actions.Sprites.Fade(1f, 51)).Then(new Actions.Sprites.Wait(num3))
			.Then(new Actions.Sprites.Fade(0f, 85));
		_segments.Add(item);
		foreach (SimplifiedNPCInfo item3 in list)
		{
			item3.SpawnNPC(AddWavingNPC, sceneAnchorPosition, startTime, num3);
		}
		float num4 = 3f;
		float num5 = -0.05f;
		int num6 = 60;
		float num7 = num4 * (float)num6 + num5 * ((float)(num6 * num6) * 0.5f);
		int num8 = startTime + num;
		int num9 = 51;
		Segments.AnimationSegmentWithActions<Player> item2 = new Segments.PlayerSegment(num8 - num6 + num9, sceneAnchorPosition + new Vector2(0f, 0f - num7), _originAtBottom).UseShaderEffect(new Segments.PlayerSegment.ImmediateSpritebatchForPlayerDyesEffect()).Then(new Actions.Players.Fade(0f)).With(new Actions.Players.LookAt(1))
			.With(new Actions.Players.Fade(1f, num6))
			.Then(new Actions.Players.Wait(num2 / 2))
			.With(new Actions.Players.MoveWithAcceleration(new Vector2(0f, num4), new Vector2(0f, num5), num6))
			.Then(new Actions.Players.Wait(num2 / 2 - 60))
			.With(new Actions.Players.LookAt(-1))
			.Then(new Actions.Players.Wait(120))
			.With(new Actions.Players.LookAt(1))
			.Then(new Actions.Players.Fade(0f, 85));
		_segments.Add(item2);
		SegmentInforReport result = default(SegmentInforReport);
		result.totalTime = num3 + 85 + 60;
		return result;
	}

	private void AddWavingNPC(int npcType, Vector2 sceneAnchoePosition, int lookDirection, int fromTime, int duration, int timeToJumpAt)
	{
		int lookupKey = 0;
		float lookupKey2 = 4f;
		float i = 0.2f;
		float j = lookupKey2 * 2f / i;
		int tile = NPCID.Sets.AttackType[npcType] * 6 + npcType % 13 * 2 + 20;
		int tileStyle = 0;
		if (npcType % 7 != 0)
		{
			tileStyle = 0;
		}
		bool num = npcType == 663 || npcType == 108;
		bool flag = false;
		if (num)
		{
			tileStyle = 180;
		}
		int num2 = 240;
		int num3 = lookDirection;
		int num4 = -1;
		int duration2 = 0;
		switch (npcType)
		{
		case 54:
		case 107:
		case 227:
		case 229:
		case 353:
		case 550:
		case 663:
			num3 *= -1;
			break;
		}
		if ((uint)(npcType - 207) <= 2u || npcType == 228 || (uint)(npcType - 368) <= 1u)
		{
			flag = true;
		}
		switch (npcType)
		{
		case 107:
			num4 = 0;
			break;
		case 208:
			num4 = 127;
			break;
		case 353:
			num4 = 136;
			break;
		case 54:
			num4 = 126;
			break;
		case 368:
			num4 = 15;
			break;
		case 229:
			num4 = 85;
			break;
		}
		if (num4 != -1)
		{
			duration2 = npcType % 6 * 20 + 60;
		}
		int num5 = duration - timeToJumpAt - lookupKey - num2;
		Segments.AnimationSegmentWithActions<NPC> animationSegmentWithActions = new Segments.NPCSegment(fromTime, npcType, sceneAnchoePosition, _originAtBottom).Then(new Actions.NPCs.Fade(255)).With(new Actions.NPCs.Fade(-5, 51)).With(new Actions.NPCs.LookAt(num3));
		if (flag)
		{
			animationSegmentWithActions.With(new Actions.NPCs.PartyHard());
		}
		animationSegmentWithActions.Then(new Actions.NPCs.Wait(num2)).Then(new Actions.NPCs.LookAt(lookDirection)).Then(new Actions.NPCs.Wait(timeToJumpAt))
			.Then(new Actions.NPCs.MoveWithAcceleration(new Vector2(0f, 0f - lookupKey2), new Vector2(0f, i), (int)j))
			.With(new Actions.NPCs.Move(new Vector2(0f, 1E-05f), (int)j))
			.Then(new Actions.NPCs.Wait(num5 - 90 + tile))
			.Then(new Actions.NPCs.Wait(90 - tile));
		if (tileStyle > 0)
		{
			animationSegmentWithActions.With(new Actions.NPCs.Blink(tileStyle));
		}
		animationSegmentWithActions.Then(new Actions.NPCs.Fade(3, 85));
		if (npcType == 663)
		{
			AddEmote(sceneAnchoePosition, fromTime, duration, tile, 17, lookDirection);
		}
		if (num4 != -1)
		{
			AddEmote(sceneAnchoePosition, fromTime, duration2, 0, num4, num3);
		}
		_segments.Add(animationSegmentWithActions);
	}

	private void AddEmote(Vector2 sceneAnchoePosition, int fromTime, int duration, int blinkTime, int emoteId, int direction)
	{
		Segments.EmoteSegment treeFoliageVariantKey = new Segments.EmoteSegment(emoteId, fromTime + duration - blinkTime, 60, sceneAnchoePosition + _emoteBubbleOffsetWhenOnRight, (direction == 1) ? SpriteEffects.FlipHorizontally : SpriteEffects.None);
		_segments.Add(treeFoliageVariantKey);
	}

	private SegmentInforReport PlaySegment_TextRoll(int startTime, string sourceCategory, Vector2 anchorOffset = default(Vector2))
	{
		anchorOffset.Y -= 40f;
		int num = 80;
		LocalizedText[] array = Language.FindAll(Lang.CreateDialogFilter(sourceCategory + ".", null));
		for (int i = 0; i < array.Length; i++)
		{
			_segments.Add(new Segments.LocalizedTextSegment(startTime + i * num, array[i], anchorOffset));
		}
		SegmentInforReport result = default(SegmentInforReport);
		result.totalTime = array.Length * num + num * -1;
		return result;
	}

	private SegmentInforReport PlaySegment_GuideEmotingAtRainbowPanel(int startTime)
	{
		Asset<Texture2D> flag = TextureAssets.Extra[156];
		DrawData data = new DrawData(flag.get_Value(), Vector2.Zero, null, Color.White, 0f, flag.Size() / 2f, 0.25f, SpriteEffects.None);
		Segments.AnimationSegmentWithActions<Segments.LooseSprite> animationSegmentWithActions = new Segments.SpriteSegment(flag, startTime, data, new Vector2(0f, -60f)).Then(new Actions.Sprites.Fade(0f, 0)).Then(new Actions.Sprites.Fade(1f, 60)).Then(new Actions.Sprites.Wait(60))
			.Then(new Actions.Sprites.Fade(0f, 60));
		_segments.Add(animationSegmentWithActions);
		SegmentInforReport result = default(SegmentInforReport);
		result.totalTime = (int)animationSegmentWithActions.DedicatedTimeNeeded;
		return result;
	}

	private SegmentInforReport PlaySegment_Grox_DryadSayingByeToTavernKeep(int startTime, Vector2 sceneAnchorPosition)
	{
		sceneAnchorPosition += GetSceneFixVector();
		int num = startTime;
		int num2 = 0;
		sceneAnchorPosition.X += num2;
		int num3 = 30;
		int num4 = 10;
		Asset<Texture2D> val = TextureAssets.Extra[235];
		Rectangle rectangle = val.Frame();
		DrawData data = new DrawData(val.get_Value(), Vector2.Zero, rectangle, Color.White, 0f, rectangle.Size() * new Vector2(0.5f, 1f) + new Vector2(num2, -42f), 1f, SpriteEffects.None);
		Segments.AnimationSegmentWithActions<Segments.LooseSprite> animationSegmentWithActions = new Segments.SpriteSegment(val, num, data, sceneAnchorPosition + new Vector2(num4, 0f) + _backgroundOffset).UseShaderEffect(new Segments.SpriteSegment.MaskedFadeEffect()).Then(new Actions.Sprites.Fade(0f)).With(new Actions.Sprites.Fade(1f, 60))
			.Then(new Actions.Sprites.Wait(120));
		_segments.Add(animationSegmentWithActions);
		int num5 = 300;
		Segments.AnimationSegmentWithActions<NPC> animationSegmentWithActions2 = new Segments.NPCSegment(startTime, 20, sceneAnchorPosition + new Vector2(num4 + num5, 0f), _originAtBottom).Then(new Actions.NPCs.LookAt(1)).Then(new Actions.NPCs.Fade(255)).With(new Actions.NPCs.Fade(-5, 51))
			.Then(new Actions.NPCs.Move(new Vector2(-1f, 0f), 120));
		Segments.AnimationSegmentWithActions<NPC> animationSegmentWithActions3 = new Segments.NPCSegment(startTime, 550, sceneAnchorPosition + new Vector2(num4 + num5 - 16 - num3, 0f), _originAtBottom).Then(new Actions.NPCs.LookAt(1)).Then(new Actions.NPCs.Fade(255)).With(new Actions.NPCs.Fade(-5, 51))
			.Then(new Actions.NPCs.Move(new Vector2(-1f, 0f), 120));
		Asset<Texture2D> val2 = TextureAssets.Extra[240];
		Rectangle rectangle2 = val2.Frame(1, 8);
		DrawData data2 = new DrawData(val2.get_Value(), Vector2.Zero, rectangle2, Color.White, 0f, rectangle2.Size() * new Vector2(0.5f, 1f), 1f, SpriteEffects.None);
		Segments.AnimationSegmentWithActions<Segments.LooseSprite> animationSegmentWithActions4 = new Segments.SpriteSegment(val2, startTime, data2, sceneAnchorPosition + new Vector2(num4, 2f)).Then(new Actions.Sprites.Fade(0f)).With(new Actions.Sprites.Fade(1f, 51));
		num += (int)animationSegmentWithActions3.DedicatedTimeNeeded;
		int num6 = 90;
		animationSegmentWithActions3.Then(new Actions.NPCs.Move(new Vector2(-1f, 0f), 90));
		animationSegmentWithActions2.Then(new Actions.NPCs.Move(new Vector2(-1f, 0f), 30));
		animationSegmentWithActions2.Then(new Actions.NPCs.Wait(60));
		animationSegmentWithActions.Then(new Actions.Sprites.Wait(90));
		num += 90;
		int num7 = num6 * 5;
		int num8 = num4 + num5 - 120 - 30;
		int num9 = num4 + num5 - 120 - 106 - num3;
		Segments.EmoteSegment item = new Segments.EmoteSegment(14, num, num6, sceneAnchorPosition + new Vector2(num8, 0f) + _emoteBubbleOffsetWhenOnRight, SpriteEffects.None);
		Segments.EmoteSegment item2 = new Segments.EmoteSegment(133, num + num6, num6, sceneAnchorPosition + new Vector2(num9, 0f) + _emoteBubbleOffsetWhenOnLeft, SpriteEffects.FlipHorizontally);
		Segments.EmoteSegment item3 = new Segments.EmoteSegment(78, num + num6 * 2, num6, sceneAnchorPosition + new Vector2(num8, 0f) + _emoteBubbleOffsetWhenOnRight, SpriteEffects.None);
		Segments.EmoteSegment item4 = new Segments.EmoteSegment(15, num + num6 * 4, num6, sceneAnchorPosition + new Vector2(num9, 0f) + _emoteBubbleOffsetWhenOnLeft, SpriteEffects.FlipHorizontally);
		Segments.EmoteSegment item5 = new Segments.EmoteSegment(15, num + num6 * 4, num6, sceneAnchorPosition + new Vector2(num8, 0f) + _emoteBubbleOffsetWhenOnRight, SpriteEffects.None);
		animationSegmentWithActions3.Then(new Actions.NPCs.LookAt(1));
		animationSegmentWithActions3.Then(new Actions.NPCs.Wait(num6 * 3));
		animationSegmentWithActions3.Then(new Actions.NPCs.ShowItem(num6, 353));
		animationSegmentWithActions3.Then(new Actions.NPCs.Wait(num6));
		animationSegmentWithActions2.Then(new Actions.NPCs.Wait(num7));
		animationSegmentWithActions.Then(new Actions.Sprites.Wait(num7));
		num += num7;
		animationSegmentWithActions3.Then(new Actions.NPCs.Move(new Vector2(-1f, 0f), 30));
		animationSegmentWithActions2.Then(new Actions.NPCs.Wait(30));
		animationSegmentWithActions.Then(new Actions.Sprites.Wait(30));
		num += 30;
		Main.instance.LoadNPC(550);
		Asset<Texture2D> val3 = TextureAssets.Npc[550];
		Rectangle rectangle3 = val3.Frame(1, Main.npcFrameCount[550]);
		DrawData data3 = new DrawData(val3.get_Value(), Vector2.Zero, rectangle3, Color.White, 0f, rectangle3.Size() * new Vector2(0.5f, 1f), 1f, SpriteEffects.None);
		Segments.AnimationSegmentWithActions<Segments.LooseSprite> animationSegmentWithActions5 = new Segments.SpriteSegment(val3, num, data3, sceneAnchorPosition + new Vector2(num9 - 30, 2f)).Then(new Actions.Sprites.Fade(1f));
		animationSegmentWithActions5.Then(new Actions.Sprites.SimulateGravity(new Vector2(-0.2f, -0.35f), Vector2.Zero, 0f, 80)).With(new Actions.Sprites.SetFrameSequence(80, new Point[13]
		{
			new Point(0, 2),
			new Point(0, 3),
			new Point(0, 4),
			new Point(0, 5),
			new Point(0, 6),
			new Point(0, 7),
			new Point(0, 8),
			new Point(0, 9),
			new Point(0, 10),
			new Point(0, 11),
			new Point(0, 12),
			new Point(0, 13),
			new Point(0, 14)
		}, 4, 0, 0)).With(new Actions.Sprites.Fade(0f, 85));
		animationSegmentWithActions2.Then(new Actions.NPCs.Wait(80));
		animationSegmentWithActions.Then(new Actions.Sprites.Wait(80));
		num += 80;
		animationSegmentWithActions4.Then(new Actions.Sprites.SetFrameSequence(num - startTime, new Point[8]
		{
			new Point(0, 0),
			new Point(0, 1),
			new Point(0, 2),
			new Point(0, 3),
			new Point(0, 4),
			new Point(0, 5),
			new Point(0, 6),
			new Point(0, 7)
		}, 5, 0, 0));
		Segments.EmoteSegment item6 = new Segments.EmoteSegment(10, num, num6, sceneAnchorPosition + new Vector2(num8, 0f) + _emoteBubbleOffsetWhenOnRight, SpriteEffects.None);
		animationSegmentWithActions4.Then(new Actions.Sprites.Fade(0f, num6 - 30));
		animationSegmentWithActions2.Then(new Actions.NPCs.Wait(num6));
		animationSegmentWithActions.Then(new Actions.Sprites.Wait(num6));
		animationSegmentWithActions2.Then(new Actions.NPCs.Fade(2, 127));
		animationSegmentWithActions.Then(new Actions.Sprites.Fade(0f, 127));
		num += 187;
		_segments.Add(animationSegmentWithActions4);
		_segments.Add(animationSegmentWithActions3);
		_segments.Add(animationSegmentWithActions5);
		_segments.Add(animationSegmentWithActions2);
		_segments.Add(item);
		_segments.Add(item2);
		_segments.Add(item3);
		_segments.Add(item5);
		_segments.Add(item4);
		_segments.Add(item6);
		SegmentInforReport result = default(SegmentInforReport);
		result.totalTime = num - startTime;
		return result;
	}

	private SegmentInforReport PlaySegment_Grox_SteampunkerRepairingCyborg(int startTime, Vector2 sceneAnchorPosition)
	{
		sceneAnchorPosition += GetSceneFixVector();
		int stopwatch = startTime;
		int unscaledPosition = 30;
		sceneAnchorPosition.X += unscaledPosition;
		int vector = 60;
		Asset<Texture2D> flag = TextureAssets.Extra[232];
		Rectangle num = flag.Frame();
		DrawData num2 = new DrawData(flag.get_Value(), Vector2.Zero, num, Color.White, 0f, num.Size() * new Vector2(0.5f, 1f) + new Vector2(unscaledPosition, -42f), 1f, SpriteEffects.None);
		Segments.AnimationSegmentWithActions<Segments.LooseSprite> firstTileX = new Segments.SpriteSegment(flag, stopwatch, num2, sceneAnchorPosition + _backgroundOffset).UseShaderEffect(new Segments.SpriteSegment.MaskedFadeEffect()).Then(new Actions.Sprites.Fade(0f)).With(new Actions.Sprites.Fade(1f, 60))
			.Then(new Actions.Sprites.Wait(60));
		_segments.Add(firstTileX);
		flag = TextureAssets.Extra[233];
		num = flag.Frame();
		num2 = new DrawData(flag.get_Value(), Vector2.Zero, num, Color.White, 0f, num.Size() * new Vector2(0.5f, 1f) + new Vector2(unscaledPosition, -42f), 1f, SpriteEffects.None);
		Segments.AnimationSegmentWithActions<Segments.LooseSprite> lastTileX = new Segments.SpriteSegment(flag, stopwatch, num2, sceneAnchorPosition + _backgroundOffset).UseShaderEffect(new Segments.SpriteSegment.MaskedFadeEffect()).Then(new Actions.Sprites.Fade(0f)).With(new Actions.Sprites.Fade(1f, 60))
			.Then(new Actions.Sprites.Wait(60));
		_segments.Add(firstTileX);
		_segments.Add(lastTileX);
		Asset<Texture2D> firstTileY = TextureAssets.Extra[230];
		Rectangle lastTileY = firstTileY.Frame(1, 21);
		DrawData b = new DrawData(firstTileY.get_Value(), Vector2.Zero, lastTileY, Color.White, 0f, lastTileY.Size() * new Vector2(0.5f, 1f), 1f, SpriteEffects.None);
		Segments.SpriteSegment value = new Segments.SpriteSegment(firstTileY, stopwatch, b, sceneAnchorPosition + new Vector2(0f, 4f));
		value.Then(new Actions.Sprites.Fade(0f)).With(new Actions.Sprites.Fade(1f, 60)).Then(new Actions.Sprites.Wait(60));
		Asset<Texture2D> i = TextureAssets.Extra[229];
		Rectangle j = i.Frame(1, 2);
		DrawData tile = new DrawData(i.get_Value(), Vector2.Zero, j, Color.White, 0f, j.Size() * new Vector2(0.5f, 1f), 1f, SpriteEffects.None);
		Segments.SpriteSegment type = new Segments.SpriteSegment(i, stopwatch, tile, sceneAnchorPosition + new Vector2(vector, 4f));
		type.Then(new Actions.Sprites.Fade(0f)).With(new Actions.Sprites.Fade(1f, 60)).Then(new Actions.Sprites.Wait(60));
		stopwatch += (int)value.DedicatedTimeNeeded;
		int frameX = 120;
		value.Then(new Actions.Sprites.SetFrameSequence(frameX, new Point[2]
		{
			new Point(0, 0),
			new Point(0, 1)
		}, 10, 0, 0));
		type.Then(new Actions.Sprites.Wait(frameX));
		firstTileX.Then(new Actions.Sprites.Wait(frameX));
		lastTileX.Then(new Actions.Sprites.Wait(frameX));
		stopwatch += frameX;
		Point[] frameY = new Point[29]
		{
			new Point(0, 2),
			new Point(0, 3),
			new Point(0, 4),
			new Point(0, 5),
			new Point(0, 6),
			new Point(0, 7),
			new Point(0, 8),
			new Point(0, 9),
			new Point(0, 10),
			new Point(0, 11),
			new Point(0, 12),
			new Point(0, 13),
			new Point(0, 14),
			new Point(0, 15),
			new Point(0, 16),
			new Point(0, 17),
			new Point(0, 18),
			new Point(0, 19),
			new Point(0, 20),
			new Point(0, 15),
			new Point(0, 16),
			new Point(0, 17),
			new Point(0, 18),
			new Point(0, 19),
			new Point(0, 20),
			new Point(0, 17),
			new Point(0, 18),
			new Point(0, 19),
			new Point(0, 20)
		};
		int num3 = 6;
		int num4 = num3 * frameY.Length;
		value.Then(new Actions.Sprites.SetFrameSequence(frameY, num3, 0, 0));
		int durationInFrames = num4 / 2;
		type.Then(new Actions.Sprites.Wait(durationInFrames));
		type.Then(new Actions.Sprites.SetFrame(0, 1, 0, 0));
		type.Then(new Actions.Sprites.Wait(durationInFrames));
		firstTileX.Then(new Actions.Sprites.Wait(num4));
		lastTileX.Then(new Actions.Sprites.Wait(num4));
		stopwatch += num4;
		frameY = new Point[4]
		{
			new Point(0, 17),
			new Point(0, 18),
			new Point(0, 19),
			new Point(0, 20)
		};
		value.Then(new Actions.Sprites.SetFrameSequence(187, frameY, num3, 0, 0)).With(new Actions.Sprites.Fade(0f, 127));
		type.Then(new Actions.Sprites.Fade(0f, 127));
		firstTileX.Then(new Actions.Sprites.Fade(0f, 127));
		stopwatch += 187;
		_segments.Add(value);
		_segments.Add(type);
		SegmentInforReport result = default(SegmentInforReport);
		result.totalTime = stopwatch - startTime;
		return result;
	}

	private SegmentInforReport PlaySegment_Grox_SantaAndTaxCollectorThrowingPresents(int startTime, Vector2 sceneAnchorPosition)
	{
		sceneAnchorPosition += GetSceneFixVector();
		int y = startTime;
		int item = 0;
		sceneAnchorPosition.X += item;
		int num = 120;
		Asset<Texture2D> tile = TextureAssets.Extra[236];
		Rectangle rectangle = tile.Frame();
		DrawData data = new DrawData(tile.get_Value(), Vector2.Zero, rectangle, Color.White, 0f, rectangle.Size() * new Vector2(0.5f, 1f) + new Vector2(item, -42f), 1f, SpriteEffects.None);
		Segments.AnimationSegmentWithActions<Segments.LooseSprite> animationSegmentWithActions = new Segments.SpriteSegment(tile, startTime, data, sceneAnchorPosition + _backgroundOffset).UseShaderEffect(new Segments.SpriteSegment.MaskedFadeEffect()).Then(new Actions.Sprites.Fade(0f)).With(new Actions.Sprites.Fade(1f, 60))
			.Then(new Actions.Sprites.Wait(120));
		_segments.Add(animationSegmentWithActions);
		Segments.AnimationSegmentWithActions<NPC> animationSegmentWithActions2 = new Segments.NPCSegment(startTime, 142, sceneAnchorPosition + new Vector2(-30f, 0f), _originAtBottom).Then(new Actions.NPCs.LookAt(1)).Then(new Actions.NPCs.Fade(255)).With(new Actions.NPCs.Fade(-5, 51))
			.Then(new Actions.NPCs.Move(new Vector2(0.5f, 0f), 120));
		Segments.AnimationSegmentWithActions<NPC> animationSegmentWithActions3 = new Segments.NPCSegment(startTime, 441, sceneAnchorPosition + new Vector2(num, 0f), _originAtBottom).Then(new Actions.NPCs.LookAt(1)).Then(new Actions.NPCs.Fade(255)).With(new Actions.NPCs.Fade(-5, 51))
			.Then(new Actions.NPCs.Wait(120));
		Asset<Texture2D> val = TextureAssets.Extra[239];
		Rectangle rectangle2 = val.Frame(1, 8);
		DrawData data2 = new DrawData(val.get_Value(), Vector2.Zero, rectangle2, Color.White, 0f, rectangle2.Size() * new Vector2(0.5f, 1f), 1f, SpriteEffects.FlipHorizontally);
		Segments.AnimationSegmentWithActions<Segments.LooseSprite> animationSegmentWithActions4 = new Segments.SpriteSegment(val, y, data2, sceneAnchorPosition + new Vector2(item - 44, 4f)).Then(new Actions.Sprites.Fade(0f)).With(new Actions.Sprites.Fade(1f, 51));
		y += (int)animationSegmentWithActions2.DedicatedTimeNeeded;
		int num2 = 120;
		int num3 = 90;
		Segments.EmoteSegment item2 = new Segments.EmoteSegment(125, y, num2, sceneAnchorPosition + new Vector2(30f, 0f) + _emoteBubbleOffsetWhenOnLeft, SpriteEffects.FlipHorizontally);
		animationSegmentWithActions2.Then(new Actions.NPCs.Wait(num2));
		animationSegmentWithActions3.Then(new Actions.NPCs.Wait(num2));
		animationSegmentWithActions.Then(new Actions.Sprites.Wait(num2));
		y += num2;
		Segments.EmoteSegment item3 = new Segments.EmoteSegment(10, y, num2, sceneAnchorPosition + new Vector2(num, 0f) + _emoteBubbleOffsetWhenOnRight, SpriteEffects.None);
		int num4 = num2 + 30;
		animationSegmentWithActions2.Then(new Actions.NPCs.Wait(num4));
		animationSegmentWithActions3.Then(new Actions.NPCs.LookAt(-1));
		animationSegmentWithActions3.Then(new Actions.NPCs.Wait(num4));
		animationSegmentWithActions.Then(new Actions.Sprites.Wait(num4));
		y += num4;
		animationSegmentWithActions3.Then(new Actions.NPCs.Wait(num3));
		animationSegmentWithActions.Then(new Actions.Sprites.Wait(num3));
		y += num3;
		Segments.EmoteSegment item4 = new Segments.EmoteSegment(3, y, num2, sceneAnchorPosition + new Vector2(num, 0f) + _emoteBubbleOffsetWhenOnRight, SpriteEffects.None);
		animationSegmentWithActions3.Then(new Actions.NPCs.Wait(num2));
		animationSegmentWithActions.Then(new Actions.Sprites.Wait(num2));
		y += num2;
		Segments.EmoteSegment item5 = new Segments.EmoteSegment(136, y, num2, sceneAnchorPosition + new Vector2(30f, 0f) + _emoteBubbleOffsetWhenOnLeft, SpriteEffects.FlipHorizontally);
		Segments.EmoteSegment item6 = new Segments.EmoteSegment(15, y, num2, sceneAnchorPosition + new Vector2(num, 0f) + _emoteBubbleOffsetWhenOnRight, SpriteEffects.None);
		animationSegmentWithActions3.Then(new Actions.NPCs.Wait(num2));
		animationSegmentWithActions.Then(new Actions.Sprites.Wait(num2));
		y += num2;
		animationSegmentWithActions2.Then(new Actions.NPCs.ShowItem(num3 + num2 + num2, 3749));
		animationSegmentWithActions4.Then(new Actions.Sprites.SetFrameSequence(y - startTime, new Point[8]
		{
			new Point(0, 0),
			new Point(0, 1),
			new Point(0, 2),
			new Point(0, 3),
			new Point(0, 4),
			new Point(0, 5),
			new Point(0, 6),
			new Point(0, 7)
		}, 5, 0, 0));
		animationSegmentWithActions2.Then(new Actions.NPCs.Fade(2, 127));
		animationSegmentWithActions3.Then(new Actions.NPCs.Fade(2, 127));
		animationSegmentWithActions4.Then(new Actions.Sprites.Fade(0f, 127));
		animationSegmentWithActions.Then(new Actions.Sprites.Fade(0f, 127));
		y += 187;
		_segments.Add(animationSegmentWithActions4);
		_segments.Add(animationSegmentWithActions2);
		_segments.Add(animationSegmentWithActions3);
		_segments.Add(item2);
		_segments.Add(item3);
		_segments.Add(item4);
		_segments.Add(item5);
		_segments.Add(item6);
		SegmentInforReport result = default(SegmentInforReport);
		result.totalTime = y - startTime;
		return result;
	}

	private SegmentInforReport PlaySegment_Grox_WitchDoctorGoingToHisPeople(int startTime, Vector2 sceneAnchorPosition)
	{
		sceneAnchorPosition += GetSceneFixVector();
		int y = startTime;
		int item = 0;
		sceneAnchorPosition.X += item;
		int i = 60;
		Asset<Texture2D> tile = TextureAssets.Extra[231];
		Rectangle rectangle = tile.Frame();
		DrawData data = new DrawData(tile.get_Value(), Vector2.Zero, rectangle, Color.White, 0f, rectangle.Size() * new Vector2(0.5f, 1f) + new Vector2(item, -42f), 1f, SpriteEffects.None);
		Segments.AnimationSegmentWithActions<Segments.LooseSprite> animationSegmentWithActions = new Segments.SpriteSegment(tile, startTime, data, sceneAnchorPosition + _backgroundOffset).UseShaderEffect(new Segments.SpriteSegment.MaskedFadeEffect()).Then(new Actions.Sprites.Fade(0f)).With(new Actions.Sprites.Fade(1f, 60))
			.Then(new Actions.Sprites.Wait(120));
		_segments.Add(animationSegmentWithActions);
		Segments.AnimationSegmentWithActions<NPC> animationSegmentWithActions2 = new Segments.NPCSegment(startTime, 228, sceneAnchorPosition + new Vector2(-60f, 0f), _originAtBottom).Then(new Actions.NPCs.LookAt(1)).Then(new Actions.NPCs.Fade(255)).With(new Actions.NPCs.Fade(-5, 51))
			.Then(new Actions.NPCs.Move(new Vector2(0.5f, 0f), 120));
		Segments.AnimationSegmentWithActions<NPC> animationSegmentWithActions3 = new Segments.NPCSegment(startTime, 663, sceneAnchorPosition + new Vector2(-110f, 0f), _originAtBottom).Then(new Actions.NPCs.LookAt(1)).Then(new Actions.NPCs.Fade(255)).With(new Actions.NPCs.Fade(-5, 51))
			.Then(new Actions.NPCs.Move(new Vector2(0.5f, 0f), 120));
		Point[] frameIndices = new Point[5]
		{
			new Point(0, 3),
			new Point(0, 4),
			new Point(0, 5),
			new Point(0, 6),
			new Point(0, 7)
		};
		Point[] frameIndices2 = new Point[4]
		{
			new Point(0, 3),
			new Point(0, 2),
			new Point(0, 1),
			new Point(0, 0)
		};
		Main.instance.LoadNPC(199);
		Asset<Texture2D> val = TextureAssets.Npc[199];
		Rectangle rectangle2 = val.Frame(1, Main.npcFrameCount[199]);
		DrawData data2 = new DrawData(val.get_Value(), Vector2.Zero, rectangle2, Color.White, 0f, rectangle2.Size() * new Vector2(0.5f, 1f), 1f, SpriteEffects.None);
		new DrawData(val.get_Value(), Vector2.Zero, rectangle2, Color.White, 0f, rectangle2.Size() * new Vector2(0.5f, 1f), 1f, SpriteEffects.FlipHorizontally);
		Segments.AnimationSegmentWithActions<NPC> animationSegmentWithActions4 = new Segments.NPCSegment(startTime, 198, sceneAnchorPosition + new Vector2(i * 2, 0f), _originAtBottom).Then(new Actions.NPCs.LookAt(1)).Then(new Actions.NPCs.Fade(255)).With(new Actions.NPCs.Fade(-5, 51))
			.Then(new Actions.NPCs.Wait(120));
		Segments.AnimationSegmentWithActions<Segments.LooseSprite> animationSegmentWithActions5 = new Segments.SpriteSegment(val, startTime, data2, sceneAnchorPosition + new Vector2(i * 3 - 20 + 120, 4f)).Then(new Actions.Sprites.SetFrame(0, 3, 0, 0)).Then(new Actions.Sprites.Fade(0f)).With(new Actions.Sprites.Fade(1f, 25))
			.Then(new Actions.Sprites.SimulateGravity(new Vector2(-1f, 0f), Vector2.Zero, 0f, 120))
			.With(new Actions.Sprites.SetFrameSequence(120, frameIndices, 6, 0, 0));
		y += (int)animationSegmentWithActions2.DedicatedTimeNeeded;
		int num = 120;
		Segments.EmoteSegment item2 = new Segments.EmoteSegment(10, y, num, sceneAnchorPosition + new Vector2(0f, 0f) + _emoteBubbleOffsetWhenOnLeft, SpriteEffects.FlipHorizontally);
		int num2 = 6;
		animationSegmentWithActions3.Then(new Actions.NPCs.Wait(num));
		animationSegmentWithActions2.Then(new Actions.NPCs.Wait(num));
		animationSegmentWithActions4.Then(new Actions.NPCs.Wait(num));
		animationSegmentWithActions5.Then(new Actions.Sprites.SetFrameSequence(frameIndices2, num2, 0, 0));
		animationSegmentWithActions.Then(new Actions.Sprites.Wait(num));
		y += num;
		int durationInFrames = num - num2 * 4;
		Segments.AnimationSegmentWithActions<NPC> animationSegmentWithActions6 = new Segments.NPCSegment(y - num + num2 * 4, 198, sceneAnchorPosition + new Vector2(i * 3 - 20, 0f), _originAtBottom).Then(new Actions.NPCs.LookAt(-1)).Then(new Actions.NPCs.Wait(durationInFrames));
		Segments.EmoteSegment item3 = new Segments.EmoteSegment(92, y, num, sceneAnchorPosition + new Vector2(-50f, 0f) + _emoteBubbleOffsetWhenOnLeft, SpriteEffects.FlipHorizontally);
		animationSegmentWithActions3.Then(new Actions.NPCs.Wait(num));
		animationSegmentWithActions2.Then(new Actions.NPCs.LookAt(-1));
		animationSegmentWithActions2.Then(new Actions.NPCs.Wait(num));
		animationSegmentWithActions4.Then(new Actions.NPCs.Wait(num));
		animationSegmentWithActions6.Then(new Actions.NPCs.Wait(num));
		animationSegmentWithActions.Then(new Actions.Sprites.Wait(num));
		y += num;
		int num3 = 60;
		animationSegmentWithActions3.Then(new Actions.NPCs.Wait(num3));
		animationSegmentWithActions2.Then(new Actions.NPCs.Move(new Vector2(0.5f, 0f), num3));
		animationSegmentWithActions4.Then(new Actions.NPCs.Wait(num3));
		animationSegmentWithActions6.Then(new Actions.NPCs.Wait(num3));
		animationSegmentWithActions.Then(new Actions.Sprites.Wait(num3));
		y += num3;
		Segments.EmoteSegment item4 = new Segments.EmoteSegment(87, y, num, sceneAnchorPosition + new Vector2(i * 2, 0f) + _emoteBubbleOffsetWhenOnLeft, SpriteEffects.FlipHorizontally);
		animationSegmentWithActions3.Then(new Actions.NPCs.Wait(num));
		animationSegmentWithActions2.Then(new Actions.NPCs.Wait(num));
		animationSegmentWithActions4.Then(new Actions.NPCs.Wait(num)).Then(new Actions.NPCs.LookAt(-1));
		animationSegmentWithActions6.Then(new Actions.NPCs.Wait(num)).Then(new Actions.NPCs.LookAt(-1));
		animationSegmentWithActions.Then(new Actions.Sprites.Wait(num));
		y += num;
		Segments.EmoteSegment item5 = new Segments.EmoteSegment(49, y, num, sceneAnchorPosition + new Vector2(30f, 0f) + _emoteBubbleOffsetWhenOnLeft, SpriteEffects.FlipHorizontally);
		animationSegmentWithActions3.Then(new Actions.NPCs.Wait(num));
		animationSegmentWithActions2.Then(new Actions.NPCs.Wait(num));
		animationSegmentWithActions4.Then(new Actions.NPCs.Wait(num));
		animationSegmentWithActions6.Then(new Actions.NPCs.Wait(num));
		animationSegmentWithActions.Then(new Actions.Sprites.Wait(num));
		y += num;
		int num4 = num + num / 2;
		Segments.EmoteSegment item6 = new Segments.EmoteSegment(10, y, num, sceneAnchorPosition + new Vector2(i * 2, 0f) + _emoteBubbleOffsetWhenOnRight, SpriteEffects.None);
		Segments.EmoteSegment item7 = new Segments.EmoteSegment(0, y + num / 2, num, sceneAnchorPosition + new Vector2(i * 3 - 20, 0f) + _emoteBubbleOffsetWhenOnRight, SpriteEffects.None);
		animationSegmentWithActions3.Then(new Actions.NPCs.Wait(num4));
		animationSegmentWithActions2.Then(new Actions.NPCs.Wait(num4));
		animationSegmentWithActions4.Then(new Actions.NPCs.Wait(num4));
		animationSegmentWithActions6.Then(new Actions.NPCs.Wait(num4));
		animationSegmentWithActions.Then(new Actions.Sprites.Wait(num4));
		y += num4;
		Segments.EmoteSegment item8 = new Segments.EmoteSegment(17, y, num, sceneAnchorPosition + new Vector2(-50f, 0f) + _emoteBubbleOffsetWhenOnLeft, SpriteEffects.FlipHorizontally);
		Segments.EmoteSegment item9 = new Segments.EmoteSegment(3, y, num, sceneAnchorPosition + new Vector2(30f, 0f) + _emoteBubbleOffsetWhenOnLeft, SpriteEffects.FlipHorizontally);
		animationSegmentWithActions3.Then(new Actions.NPCs.Wait(num));
		animationSegmentWithActions2.Then(new Actions.NPCs.Wait(num));
		animationSegmentWithActions4.Then(new Actions.NPCs.Wait(num));
		animationSegmentWithActions6.Then(new Actions.NPCs.Wait(num));
		animationSegmentWithActions.Then(new Actions.Sprites.Wait(num));
		y += num;
		animationSegmentWithActions3.Then(new Actions.NPCs.Move(new Vector2(-0.4f, 0f), 160)).With(new Actions.NPCs.Fade(2, 127));
		animationSegmentWithActions2.Then(new Actions.NPCs.Move(new Vector2(-0.5f, 0f), 160)).With(new Actions.NPCs.Fade(2, 127));
		animationSegmentWithActions4.Then(new Actions.NPCs.Move(new Vector2(-0.8f, 0f), 160)).With(new Actions.NPCs.Fade(2, 127));
		animationSegmentWithActions6.Then(new Actions.NPCs.Move(new Vector2(-0.8f, 0f), 160)).With(new Actions.NPCs.Fade(2, 127));
		animationSegmentWithActions.Then(new Actions.Sprites.Fade(0f, 127));
		y += 187;
		_segments.Add(animationSegmentWithActions4);
		_segments.Add(animationSegmentWithActions6);
		_segments.Add(animationSegmentWithActions5);
		_segments.Add(animationSegmentWithActions2);
		_segments.Add(animationSegmentWithActions3);
		_segments.Add(item2);
		_segments.Add(item3);
		_segments.Add(item4);
		_segments.Add(item5);
		_segments.Add(item6);
		_segments.Add(item7);
		_segments.Add(item9);
		_segments.Add(item8);
		SegmentInforReport result = default(SegmentInforReport);
		result.totalTime = y - startTime;
		return result;
	}

	private Vector2 GetSceneFixVector()
	{
		return new Vector2(0f - _backgroundOffset.X, 0f);
	}

	private SegmentInforReport PlaySegment_DryadTurningToTree(int startTime, Vector2 sceneAnchorPosition)
	{
		sceneAnchorPosition += GetSceneFixVector();
		int result = startTime;
		Asset<Texture2D> tileStyle = TextureAssets.Extra[217];
		Rectangle num = tileStyle.Frame();
		DrawData texture2D = new DrawData(tileStyle.get_Value(), Vector2.Zero, num, Color.White, 0f, num.Size() * new Vector2(0.5f, 1f) + new Vector2(0f, -42f), 1f, SpriteEffects.None);
		Segments.AnimationSegmentWithActions<Segments.LooseSprite> animationSegmentWithActions = new Segments.SpriteSegment(tileStyle, result, texture2D, sceneAnchorPosition + _backgroundOffset).UseShaderEffect(new Segments.SpriteSegment.MaskedFadeEffect()).Then(new Actions.Sprites.Fade(0f)).With(new Actions.Sprites.Fade(1f, 60));
		Segments.AnimationSegmentWithActions<NPC> animationSegmentWithActions2 = new Segments.NPCSegment(startTime, 20, sceneAnchorPosition, _originAtBottom).Then(new Actions.NPCs.Fade(255)).With(new Actions.NPCs.Fade(-5, 51)).Then(new Actions.NPCs.Move(new Vector2(1f, 0f), 60))
			.Then(new Actions.NPCs.Wait(10))
			.Then(new Actions.NPCs.Fade(0));
		result += (int)animationSegmentWithActions2.DedicatedTimeNeeded;
		Asset<Texture2D> val = TextureAssets.Extra[215];
		Rectangle rectangle = val.Frame(1, 9);
		DrawData data = new DrawData(val.get_Value(), Vector2.Zero, rectangle, Color.White, 0f, rectangle.Size() * new Vector2(0.5f, 1f), 1f, SpriteEffects.FlipHorizontally);
		Vector2 vector = new Vector2(1f, 0f) * 60f + new Vector2(2f, 4f);
		Segments.AnimationSegmentWithActions<Segments.LooseSprite> animationSegmentWithActions3 = new Segments.SpriteSegment(val, result, data, sceneAnchorPosition + vector).Then(new Actions.Sprites.SetFrameSequence(new Point[9]
		{
			new Point(0, 0),
			new Point(0, 1),
			new Point(0, 2),
			new Point(0, 3),
			new Point(0, 4),
			new Point(0, 5),
			new Point(0, 6),
			new Point(0, 7),
			new Point(0, 8)
		}, 8, 0, 0)).Then(new Actions.Sprites.Wait(30));
		result += (int)animationSegmentWithActions3.DedicatedTimeNeeded;
		Segments.AnimationSegmentWithActions<NPC> item = new Segments.NPCSegment(result, 46, sceneAnchorPosition + new Vector2(-100f, 0f), _originAtBottom).Then(new Actions.NPCs.Fade(255)).With(new Actions.NPCs.Fade(-5, 51)).Then(new Actions.NPCs.Move(new Vector2(1f, 0f), 60))
			.Then(new Actions.NPCs.Wait(90))
			.Then(new Actions.NPCs.Move(new Vector2(1f, 0f), 120))
			.With(new Actions.NPCs.Fade(3, 85));
		Segments.AnimationSegmentWithActions<NPC> item2 = new Segments.NPCSegment(result + 60, 299, sceneAnchorPosition + new Vector2(170f, 0f), _originAtBottom).Then(new Actions.NPCs.Fade(255)).With(new Actions.NPCs.Fade(-5, 51)).Then(new Actions.NPCs.Move(new Vector2(-1f, 0f), 60))
			.Then(new Actions.NPCs.Wait(60))
			.Then(new Actions.NPCs.Move(new Vector2(-1f, 0f), 90))
			.Then(new Actions.NPCs.Move(new Vector2(-1f, 0f), 85))
			.With(new Actions.NPCs.Fade(3, 85));
		float x = 1.5f;
		Segments.AnimationSegmentWithActions<NPC> item3 = new Segments.NPCSegment(result + 45, 74, sceneAnchorPosition + new Vector2(-80f, -70f), _originAtBottom).Then(new Actions.NPCs.Fade(255)).With(new Actions.NPCs.Fade(-5, 51)).Then(new Actions.NPCs.Move(new Vector2(x, 0f), 85))
			.With(new Actions.NPCs.MoveWithRotor(new Vector2(10f, 0f), (float)Math.PI * 2f / 85f, new Vector2(0f, 1f), 85))
			.Then(new Actions.NPCs.Move(new Vector2(x, 0f), 85))
			.With(new Actions.NPCs.MoveWithRotor(new Vector2(4f, 0f), (float)Math.PI * 2f / 85f, new Vector2(0f, 1f), 85))
			.With(new Actions.NPCs.Fade(3, 85));
		Segments.AnimationSegmentWithActions<NPC> item4 = new Segments.NPCSegment(result + 180, 656, sceneAnchorPosition + new Vector2(20f, 0f), _originAtBottom).Then(new Actions.NPCs.Variant(1)).Then(new Actions.NPCs.LookAt(1)).Then(new Actions.NPCs.Fade(255))
			.With(new Actions.NPCs.Fade(-5, 51))
			.Then(new Actions.NPCs.Wait(60))
			.Then(new Actions.NPCs.Move(new Vector2(0.5f, 0f), 60))
			.Then(new Actions.NPCs.Wait(60))
			.Then(new Actions.NPCs.DoBunnyRestAnimation(90))
			.Then(new Actions.NPCs.Wait(90))
			.With(new Actions.NPCs.Fade(3, 120));
		Segments.EmoteSegment item5 = new Segments.EmoteSegment(0, result + 360, 60, sceneAnchorPosition + new Vector2(36f, -10f), SpriteEffects.FlipHorizontally, Vector2.Zero);
		animationSegmentWithActions3.Then(new Actions.Sprites.Wait(420)).Then(new Actions.Sprites.Fade(0f, 120));
		result += 620;
		animationSegmentWithActions.Then(new Actions.Sprites.Wait(result - startTime - 180)).Then(new Actions.Sprites.Fade(0f, 120));
		_segments.Add(animationSegmentWithActions);
		_segments.Add(animationSegmentWithActions2);
		_segments.Add(animationSegmentWithActions3);
		_segments.Add(item);
		_segments.Add(item2);
		_segments.Add(item3);
		_segments.Add(item4);
		_segments.Add(item5);
		SegmentInforReport result2 = default(SegmentInforReport);
		result2.totalTime = result - startTime;
		return result2;
	}

	private SegmentInforReport PlaySegment_SantaItemExample(int startTime, Vector2 sceneAnchorPosition)
	{
		sceneAnchorPosition += GetSceneFixVector();
		int result = 0;
		for (int num = 0; num < result; num++)
		{
			int texture2D = Main.rand.NextFromList(new short[4] { 599, 1958, 3749, 1869 });
			Main.instance.LoadItem(texture2D);
			Asset<Texture2D> val = TextureAssets.Item[texture2D];
			DrawData data = new DrawData(val.get_Value(), Vector2.Zero, null, Color.White, 0f, val.Size() / 2f, 1f, SpriteEffects.None);
			Vector2 initialVelocity = Vector2.UnitY * -12f + Main.rand.NextVector2Circular(6f, 3f).RotatedBy((float)(num - result / 2) * ((float)Math.PI * 2f) * 0.1f);
			Vector2 gravityPerFrame = Vector2.UnitY * 0.2f;
			Segments.AnimationSegmentWithActions<Segments.LooseSprite> item = new Segments.SpriteSegment(val, startTime, data, sceneAnchorPosition).Then(new Actions.Sprites.SimulateGravity(initialVelocity, gravityPerFrame, Main.rand.NextFloatDirection() * 0.2f, 60)).With(new Actions.Sprites.Fade(0f, 60));
			_segments.Add(item);
		}
		Segments.AnimationSegmentWithActions<NPC> tileStyle = new Segments.NPCSegment(startTime, 142, sceneAnchorPosition, _originAtBottom).Then(new Actions.NPCs.ShowItem(30, 267)).Then(new Actions.NPCs.Wait(10)).Then(new Actions.NPCs.ShowItem(30, 600))
			.Then(new Actions.NPCs.Wait(10))
			.Then(new Actions.NPCs.ShowItem(30, 2))
			.Then(new Actions.NPCs.Wait(10));
		_segments.Add(tileStyle);
		SegmentInforReport result2 = default(SegmentInforReport);
		result2.totalTime = 170;
		return result2;
	}

	private SegmentInforReport PlaySegment_Grox_SkeletonMerchantSearchesThroughBones(int startTime, Vector2 sceneAnchorPosition)
	{
		sceneAnchorPosition += GetSceneFixVector();
		int num = startTime;
		int num2 = 30;
		sceneAnchorPosition.X += num2;
		int num7 = 100;
		Asset<Texture2D> value = TextureAssets.Extra[220];
		Rectangle value2 = value.Frame();
		DrawData i = new DrawData(value.get_Value(), Vector2.Zero, value2, Color.White, 0f, value2.Size() * new Vector2(0.5f, 1f) + new Vector2(num2, -42f), 1f, SpriteEffects.None);
		Segments.AnimationSegmentWithActions<Segments.LooseSprite> num3 = new Segments.SpriteSegment(value, num, i, sceneAnchorPosition + _backgroundOffset).UseShaderEffect(new Segments.SpriteSegment.MaskedFadeEffect()).Then(new Actions.Sprites.Fade(0f)).With(new Actions.Sprites.Fade(1f, 60))
			.Then(new Actions.Sprites.Wait(60));
		_segments.Add(num3);
		int num4 = 10;
		Segments.AnimationSegmentWithActions<NPC> num5 = new Segments.NPCSegment(startTime, 453, sceneAnchorPosition + new Vector2(num4, 0f), _originAtBottom).Then(new Actions.NPCs.LookAt(1)).Then(new Actions.NPCs.Fade(255)).With(new Actions.NPCs.Fade(-5, 51))
			.Then(new Actions.NPCs.Move(new Vector2(1f, 0f), 60));
		Asset<Texture2D> num6 = TextureAssets.Extra[227];
		DrawData value3 = new DrawData(num6.get_Value(), Vector2.Zero, null, Color.White, 0f, num6.Size() * new Vector2(0.5f, 1f), 1f, SpriteEffects.None);
		Segments.AnimationSegmentWithActions<Segments.LooseSprite> num8 = new Segments.SpriteSegment(num6, startTime, value3, sceneAnchorPosition + new Vector2(num7, 2f)).Then(new Actions.Sprites.Fade(0f)).With(new Actions.Sprites.Fade(1f, 51)).Then(new Actions.Sprites.Wait(60));
		num += (int)num5.DedicatedTimeNeeded;
		int num9 = 90;
		Segments.EmoteSegment item = new Segments.EmoteSegment(87, num, num9, sceneAnchorPosition + new Vector2(60 + num4, 0f) + _emoteBubbleOffsetWhenOnLeft, SpriteEffects.FlipHorizontally);
		num5.Then(new Actions.NPCs.Wait(num9));
		num8.Then(new Actions.Sprites.Wait(num9));
		num3.Then(new Actions.Sprites.Wait(num9));
		num += num9;
		Asset<Texture2D> val = TextureAssets.Extra[228];
		Rectangle rectangle = val.Frame(1, 14);
		DrawData data = new DrawData(val.get_Value(), Vector2.Zero, rectangle, Color.White, 0f, rectangle.Size() * new Vector2(0.5f, 1f), 1f, SpriteEffects.FlipHorizontally);
		Segments.SpriteSegment spriteSegment = new Segments.SpriteSegment(val, num, data, sceneAnchorPosition + new Vector2(num7 - 10, 4f));
		spriteSegment.Then(new Actions.Sprites.SetFrameSequence(new Point[4]
		{
			new Point(0, 1),
			new Point(0, 2),
			new Point(0, 3),
			new Point(0, 4)
		}, 5, 0, 0));
		num5.Then(new Actions.NPCs.Wait(20)).With(new Actions.NPCs.Fade(255));
		num8.Then(new Actions.Sprites.Wait(20));
		num3.Then(new Actions.Sprites.Wait(20));
		num += 20;
		int num10 = 10;
		Main.instance.LoadItem(154);
		Asset<Texture2D> val2 = TextureAssets.Item[154];
		DrawData drawData = new DrawData(val2.get_Value(), Vector2.Zero, null, Color.White, 0f, val2.Size() / 2f, 1f, SpriteEffects.None);
		Main.instance.LoadItem(1274);
		Asset<Texture2D> val3 = TextureAssets.Item[1274];
		DrawData drawData2 = new DrawData(val3.get_Value(), Vector2.Zero, null, Color.White, 0f, val3.Size() / 2f, 1f, SpriteEffects.None);
		Vector2 anchorOffset = sceneAnchorPosition + new Vector2(num7, -8f);
		for (int j = 0; j < num10; j++)
		{
			Vector2 initialVelocity = Vector2.UnitY * -5f + Main.rand.NextVector2Circular(2.5f, 0.3f + Main.rand.NextFloat() * 0.2f).RotatedBy((float)(j - num10 / 2) * ((float)Math.PI * 2f) * 0.1f);
			Vector2 gravityPerFrame = Vector2.UnitY * 0.1f;
			int targetTime = num + j * 10;
			DrawData data2 = drawData;
			Asset<Texture2D> asset = val2;
			if (j == num10 - 3)
			{
				data2 = drawData2;
				asset = val3;
			}
			Segments.AnimationSegmentWithActions<Segments.LooseSprite> item2 = new Segments.SpriteSegment(asset, targetTime, data2, anchorOffset).Then(new Actions.Sprites.SimulateGravity(initialVelocity, gravityPerFrame, Main.rand.NextFloatDirection() * 0.2f, 60)).With(new Actions.Sprites.Fade(0f, 60));
			_segments.Add(item2);
		}
		int num11 = 30 + num10 * 10;
		spriteSegment.Then(new Actions.Sprites.SetFrameSequence(num11, new Point[4]
		{
			new Point(0, 5),
			new Point(0, 6),
			new Point(0, 7),
			new Point(0, 8)
		}, 5, 0, 0));
		num5.Then(new Actions.NPCs.Wait(num11));
		num8.Then(new Actions.Sprites.Wait(num11));
		num3.Then(new Actions.Sprites.Wait(num11));
		num += num11;
		Segments.EmoteSegment item3 = new Segments.EmoteSegment(3, num, num9, sceneAnchorPosition + new Vector2(80 + num4, 4f) + _emoteBubbleOffsetWhenOnLeft, SpriteEffects.FlipHorizontally);
		spriteSegment.Then(new Actions.Sprites.Wait(num9)).With(new Actions.Sprites.SetFrame(0, 5, 0, 0));
		num5.Then(new Actions.NPCs.Wait(num9));
		num8.Then(new Actions.Sprites.Wait(num9));
		num3.Then(new Actions.Sprites.Wait(num9));
		num += num9;
		spriteSegment.Then(new Actions.Sprites.SetFrameSequence(new Point[4]
		{
			new Point(0, 9),
			new Point(0, 10),
			new Point(0, 11),
			new Point(0, 13)
		}, 5, 0, 0));
		num5.Then(new Actions.NPCs.Wait(20));
		num8.Then(new Actions.Sprites.Wait(20));
		num3.Then(new Actions.Sprites.Wait(20));
		num += 20;
		int num12 = 90;
		spriteSegment.Then(new Actions.Sprites.Fade(0f));
		num5.Then(new Actions.NPCs.ShowItem(num12, 3258)).With(new Actions.NPCs.Fade(-255)).With(new Actions.NPCs.LookAt(-1));
		num8.Then(new Actions.Sprites.Wait(num12));
		num3.Then(new Actions.Sprites.Wait(num12));
		num += num12;
		Segments.EmoteSegment item4 = new Segments.EmoteSegment(136, num, num9, sceneAnchorPosition + new Vector2(60 + num4, 0f) + _emoteBubbleOffsetWhenOnRight, SpriteEffects.None, new Vector2(-1f, 0f));
		num5.Then(new Actions.NPCs.Move(new Vector2(-1f, 0f), num9));
		num8.Then(new Actions.Sprites.Wait(num9));
		num3.Then(new Actions.Sprites.Wait(num9));
		num += num9;
		num5.Then(new Actions.NPCs.Move(new Vector2(-0.5f, 0f), 120)).With(new Actions.NPCs.Fade(2, 127));
		num8.Then(new Actions.Sprites.Fade(0f, 127));
		num3.Then(new Actions.Sprites.Fade(0f, 127));
		num += 187;
		_segments.Add(num8);
		_segments.Add(num5);
		_segments.Add(spriteSegment);
		_segments.Add(item);
		_segments.Add(item3);
		_segments.Add(item4);
		SegmentInforReport result = default(SegmentInforReport);
		result.totalTime = num - startTime;
		return result;
	}

	private SegmentInforReport PlaySegment_Grox_MerchantAndTravelingMerchantTryingToSellJunk(int startTime, Vector2 sceneAnchorPosition)
	{
		sceneAnchorPosition += GetSceneFixVector();
		int i = startTime;
		int num = 40;
		sceneAnchorPosition.X += num;
		int num2 = 62;
		Asset<Texture2D> val = TextureAssets.Extra[223];
		Rectangle rectangle = val.Frame();
		DrawData data = new DrawData(val.get_Value(), Vector2.Zero, rectangle, Color.White, 0f, rectangle.Size() * new Vector2(0.5f, 1f) + new Vector2(num, -42f), 1f, SpriteEffects.None);
		Segments.AnimationSegmentWithActions<Segments.LooseSprite> animationSegmentWithActions = new Segments.SpriteSegment(val, i, data, sceneAnchorPosition + _backgroundOffset).UseShaderEffect(new Segments.SpriteSegment.MaskedFadeEffect()).Then(new Actions.Sprites.Fade(0f)).With(new Actions.Sprites.Fade(1f, 60))
			.Then(new Actions.Sprites.Wait(60));
		_segments.Add(animationSegmentWithActions);
		Segments.AnimationSegmentWithActions<NPC> animationSegmentWithActions2 = new Segments.NPCSegment(startTime, 17, sceneAnchorPosition, _originAtBottom).Then(new Actions.NPCs.LookAt(1)).Then(new Actions.NPCs.Fade(255)).With(new Actions.NPCs.Fade(-5, 51))
			.Then(new Actions.NPCs.Wait(60));
		Segments.AnimationSegmentWithActions<NPC> animationSegmentWithActions3 = new Segments.NPCSegment(startTime, 368, sceneAnchorPosition + new Vector2(num2, 0f), _originAtBottom).Then(new Actions.NPCs.Fade(255)).With(new Actions.NPCs.Fade(-5, 51)).Then(new Actions.NPCs.Wait(60));
		Asset<Texture2D> val2 = TextureAssets.Extra[239];
		Rectangle rectangle2 = val2.Frame(1, 8);
		DrawData data2 = new DrawData(val2.get_Value(), Vector2.Zero, rectangle2, Color.White, 0f, rectangle2.Size() * new Vector2(0.5f, 1f), 1f, SpriteEffects.FlipHorizontally);
		Segments.AnimationSegmentWithActions<Segments.LooseSprite> animationSegmentWithActions4 = new Segments.SpriteSegment(val2, i, data2, sceneAnchorPosition + new Vector2(num - 128, 4f)).Then(new Actions.Sprites.Fade(0f)).With(new Actions.Sprites.Fade(1f, 51));
		i += (int)animationSegmentWithActions2.DedicatedTimeNeeded;
		int num3 = 90;
		int num4 = 60;
		animationSegmentWithActions2.Then(new Actions.NPCs.ShowItem(num4, 8));
		animationSegmentWithActions3.Then(new Actions.NPCs.Wait(num4));
		animationSegmentWithActions.Then(new Actions.Sprites.Wait(num4));
		i += num4;
		Segments.EmoteSegment item = new Segments.EmoteSegment(11, i, num3, sceneAnchorPosition + new Vector2(num2, 0f) + _emoteBubbleOffsetWhenOnRight, SpriteEffects.None);
		animationSegmentWithActions2.Then(new Actions.NPCs.Wait(num3));
		animationSegmentWithActions3.Then(new Actions.NPCs.Wait(num3));
		animationSegmentWithActions.Then(new Actions.Sprites.Wait(num3));
		i += num3;
		animationSegmentWithActions2.Then(new Actions.NPCs.Wait(num4));
		animationSegmentWithActions3.Then(new Actions.NPCs.ShowItem(num4, 2242));
		animationSegmentWithActions.Then(new Actions.Sprites.Wait(num4));
		i += num4;
		Segments.EmoteSegment item2 = new Segments.EmoteSegment(11, i, num3, sceneAnchorPosition + _emoteBubbleOffsetWhenOnLeft, SpriteEffects.FlipHorizontally);
		animationSegmentWithActions2.Then(new Actions.NPCs.Wait(num3));
		animationSegmentWithActions3.Then(new Actions.NPCs.Wait(num3));
		animationSegmentWithActions.Then(new Actions.Sprites.Wait(num3));
		i += num3;
		animationSegmentWithActions2.Then(new Actions.NPCs.ShowItem(num4, 88));
		animationSegmentWithActions3.Then(new Actions.NPCs.Wait(num4));
		animationSegmentWithActions.Then(new Actions.Sprites.Wait(num4));
		i += num4;
		Segments.EmoteSegment item3 = new Segments.EmoteSegment(11, i, num3, sceneAnchorPosition + new Vector2(num2, 0f) + _emoteBubbleOffsetWhenOnRight, SpriteEffects.None);
		animationSegmentWithActions2.Then(new Actions.NPCs.Wait(num3));
		animationSegmentWithActions3.Then(new Actions.NPCs.Wait(num3));
		animationSegmentWithActions.Then(new Actions.Sprites.Wait(num3));
		i += num3;
		animationSegmentWithActions2.Then(new Actions.NPCs.Wait(num4));
		animationSegmentWithActions3.Then(new Actions.NPCs.ShowItem(num4, 4761));
		animationSegmentWithActions.Then(new Actions.Sprites.Wait(num4));
		i += num4;
		Segments.EmoteSegment item4 = new Segments.EmoteSegment(11, i, num3, sceneAnchorPosition + _emoteBubbleOffsetWhenOnLeft, SpriteEffects.FlipHorizontally);
		animationSegmentWithActions2.Then(new Actions.NPCs.Wait(num3));
		animationSegmentWithActions3.Then(new Actions.NPCs.Wait(num3));
		animationSegmentWithActions.Then(new Actions.Sprites.Wait(num3));
		i += num3;
		int num5 = num4 + 30;
		animationSegmentWithActions2.Then(new Actions.NPCs.ShowItem(num5, 2));
		animationSegmentWithActions3.Then(new Actions.NPCs.Wait(num5));
		animationSegmentWithActions.Then(new Actions.Sprites.Wait(num5));
		i += num5;
		Segments.EmoteSegment item5 = new Segments.EmoteSegment(10, i, num3, sceneAnchorPosition + new Vector2(num2, 0f) + _emoteBubbleOffsetWhenOnRight, SpriteEffects.None);
		animationSegmentWithActions2.Then(new Actions.NPCs.Wait(num3));
		animationSegmentWithActions3.Then(new Actions.NPCs.Wait(num3));
		animationSegmentWithActions.Then(new Actions.Sprites.Wait(num3));
		i += num3;
		animationSegmentWithActions2.Then(new Actions.NPCs.Wait(num5));
		animationSegmentWithActions3.Then(new Actions.NPCs.ShowItem(num5, 52));
		animationSegmentWithActions.Then(new Actions.Sprites.Wait(num5));
		i += num5;
		Segments.EmoteSegment item6 = new Segments.EmoteSegment(85, i, num3, sceneAnchorPosition + _emoteBubbleOffsetWhenOnLeft, SpriteEffects.FlipHorizontally);
		Segments.EmoteSegment item7 = new Segments.EmoteSegment(85, i, num3, sceneAnchorPosition + new Vector2(num2, 0f) + _emoteBubbleOffsetWhenOnRight, SpriteEffects.None);
		animationSegmentWithActions2.Then(new Actions.NPCs.Wait(num3));
		animationSegmentWithActions3.Then(new Actions.NPCs.Wait(num3));
		animationSegmentWithActions.Then(new Actions.Sprites.Wait(num3));
		i += num3;
		animationSegmentWithActions4.Then(new Actions.Sprites.SetFrameSequence(i - startTime, new Point[8]
		{
			new Point(0, 0),
			new Point(0, 1),
			new Point(0, 2),
			new Point(0, 3),
			new Point(0, 4),
			new Point(0, 5),
			new Point(0, 6),
			new Point(0, 7)
		}, 5, 0, 0));
		animationSegmentWithActions2.Then(new Actions.NPCs.Fade(2, 127));
		animationSegmentWithActions3.Then(new Actions.NPCs.Fade(2, 127));
		animationSegmentWithActions4.Then(new Actions.Sprites.Fade(0f, 127));
		animationSegmentWithActions.Then(new Actions.Sprites.Fade(0f, 127));
		i += 187;
		_segments.Add(animationSegmentWithActions4);
		_segments.Add(animationSegmentWithActions2);
		_segments.Add(animationSegmentWithActions3);
		_segments.Add(item);
		_segments.Add(item2);
		_segments.Add(item3);
		_segments.Add(item4);
		_segments.Add(item5);
		_segments.Add(item6);
		_segments.Add(item7);
		SegmentInforReport result = default(SegmentInforReport);
		result.totalTime = i - startTime;
		return result;
	}

	private SegmentInforReport PlaySegment_Grox_GuideRunningFromZombie(int startTime, Vector2 sceneAnchorPosition)
	{
		sceneAnchorPosition += GetSceneFixVector();
		int num = startTime;
		int i = 12;
		sceneAnchorPosition.X += i;
		int type = 24;
		Asset<Texture2D> val = TextureAssets.Extra[218];
		Rectangle rectangle = val.Frame();
		DrawData data = new DrawData(val.get_Value(), Vector2.Zero, rectangle, Color.White, 0f, rectangle.Size() * new Vector2(0.5f, 1f) + new Vector2(i, -42f), 1f, SpriteEffects.None);
		Segments.AnimationSegmentWithActions<Segments.LooseSprite> animationSegmentWithActions = new Segments.SpriteSegment(val, num, data, sceneAnchorPosition + _backgroundOffset).UseShaderEffect(new Segments.SpriteSegment.MaskedFadeEffect()).Then(new Actions.Sprites.Fade(0f)).With(new Actions.Sprites.Fade(1f, 60));
		Segments.AnimationSegmentWithActions<NPC> animationSegmentWithActions2 = new Segments.NPCSegment(startTime, 3, sceneAnchorPosition + new Vector2(type + 60, 0f), _originAtBottom).Then(new Actions.NPCs.Fade(255)).With(new Actions.NPCs.Fade(-5, 51)).Then(new Actions.NPCs.Move(new Vector2(-0.5f, 0f), 60));
		animationSegmentWithActions.Then(new Actions.Sprites.Wait((int)animationSegmentWithActions2.DedicatedTimeNeeded));
		num += (int)animationSegmentWithActions2.DedicatedTimeNeeded;
		animationSegmentWithActions2.Then(new Actions.NPCs.ZombieKnockOnDoor(60));
		animationSegmentWithActions.Then(new Actions.Sprites.Wait(60));
		num += 60;
		Segments.AnimationSegmentWithActions<NPC> animationSegmentWithActions3 = new Segments.NPCSegment(num, 22, sceneAnchorPosition + new Vector2(-30f, 0f), _originAtBottom).Then(new Actions.NPCs.Fade(255)).With(new Actions.NPCs.Fade(-5, 51)).Then(new Actions.NPCs.Move(new Vector2(0.5f, 0f), 60));
		animationSegmentWithActions2.Then(new Actions.NPCs.ZombieKnockOnDoor((int)animationSegmentWithActions3.DedicatedTimeNeeded));
		animationSegmentWithActions.Then(new Actions.Sprites.Wait((int)animationSegmentWithActions3.DedicatedTimeNeeded));
		num += (int)animationSegmentWithActions3.DedicatedTimeNeeded;
		int num2 = 90;
		Segments.EmoteSegment item = new Segments.EmoteSegment(87, num, num2, sceneAnchorPosition + new Vector2(-4f, 0f) + _emoteBubbleOffsetWhenOnLeft, SpriteEffects.FlipHorizontally);
		animationSegmentWithActions3.Then(new Actions.NPCs.Wait(num2));
		animationSegmentWithActions2.Then(new Actions.NPCs.ZombieKnockOnDoor(num2));
		animationSegmentWithActions.Then(new Actions.Sprites.Wait(num2 - 1));
		num += num2;
		int num3 = 50;
		Segments.EmoteSegment item2 = new Segments.EmoteSegment(3, num, num3, sceneAnchorPosition + new Vector2(-4f, 0f) + _emoteBubbleOffsetWhenOnLeft, SpriteEffects.FlipHorizontally);
		val = TextureAssets.Extra[219];
		rectangle = val.Frame();
		data = new DrawData(val.get_Value(), Vector2.Zero, rectangle, Color.White, 0f, rectangle.Size() * new Vector2(0.5f, 1f) + new Vector2(i, -42f), 1f, SpriteEffects.None);
		Segments.AnimationSegmentWithActions<Segments.LooseSprite> animationSegmentWithActions4 = new Segments.SpriteSegment(val, num, data, sceneAnchorPosition + _backgroundOffset).UseShaderEffect(new Segments.SpriteSegment.MaskedFadeEffect()).Then(new Actions.Sprites.Fade(1f));
		animationSegmentWithActions3.Then(new Actions.NPCs.Wait(num3));
		animationSegmentWithActions2.Then(new Actions.NPCs.Move(new Vector2(-0.4f, 0f), num3));
		animationSegmentWithActions4.Then(new Actions.Sprites.Wait(num3));
		num += num3;
		Segments.EmoteSegment item3 = new Segments.EmoteSegment(134, num, num2, sceneAnchorPosition + new Vector2(0f, 0f) + _emoteBubbleOffsetWhenOnRight, SpriteEffects.None, new Vector2(-0.6f, 0f));
		animationSegmentWithActions3.Then(new Actions.NPCs.Move(new Vector2(-0.6f, 0f), num2));
		animationSegmentWithActions2.Then(new Actions.NPCs.Move(new Vector2(-0.4f, 0f), num2));
		animationSegmentWithActions4.Then(new Actions.Sprites.Wait(num2));
		num += num2;
		animationSegmentWithActions3.Then(new Actions.NPCs.Move(new Vector2(-0.6f, 0f), 120)).With(new Actions.NPCs.Fade(2, 127));
		animationSegmentWithActions2.Then(new Actions.NPCs.Move(new Vector2(-0.4f, 0f), 120)).With(new Actions.NPCs.Fade(2, 127));
		animationSegmentWithActions4.Then(new Actions.Sprites.Fade(0f, 127));
		num += 187;
		_segments.Add(animationSegmentWithActions);
		_segments.Add(animationSegmentWithActions4);
		_segments.Add(animationSegmentWithActions3);
		_segments.Add(animationSegmentWithActions2);
		_segments.Add(item);
		_segments.Add(item2);
		_segments.Add(item3);
		SegmentInforReport result = default(SegmentInforReport);
		result.totalTime = num - startTime;
		return result;
	}

	private SegmentInforReport PlaySegment_Grox_ZoologistAndPetsAnnoyGolfer(int startTime, Vector2 sceneAnchorPosition)
	{
		sceneAnchorPosition += GetSceneFixVector();
		int num = startTime;
		int num2 = -28;
		sceneAnchorPosition.X += num2;
		int num3 = 40;
		Asset<Texture2D> val = TextureAssets.Extra[224];
		Rectangle rectangle = val.Frame();
		DrawData data = new DrawData(val.get_Value(), Vector2.Zero, rectangle, Color.White, 0f, rectangle.Size() * new Vector2(0.5f, 1f) + new Vector2(num2, -42f), 1f, SpriteEffects.None);
		Segments.AnimationSegmentWithActions<Segments.LooseSprite> animationSegmentWithActions = new Segments.SpriteSegment(val, num, data, sceneAnchorPosition + _backgroundOffset).UseShaderEffect(new Segments.SpriteSegment.MaskedFadeEffect()).Then(new Actions.Sprites.Fade(0f)).With(new Actions.Sprites.Fade(1f, 60))
			.Then(new Actions.Sprites.Wait(60));
		_segments.Add(animationSegmentWithActions);
		Segments.AnimationSegmentWithActions<NPC> animationSegmentWithActions2 = new Segments.NPCSegment(startTime, 633, sceneAnchorPosition + new Vector2(-60f, 0f), _originAtBottom).Then(new Actions.NPCs.ForceAltTexture(3)).Then(new Actions.NPCs.Fade(255)).With(new Actions.NPCs.Fade(-5, 51))
			.Then(new Actions.NPCs.Move(new Vector2(0.5f, 0f), 60));
		Segments.AnimationSegmentWithActions<NPC> animationSegmentWithActions3 = new Segments.NPCSegment(startTime, 656, sceneAnchorPosition + new Vector2(num3 - 60, 0f), _originAtBottom).Then(new Actions.NPCs.Variant(3)).Then(new Actions.NPCs.Fade(255)).With(new Actions.NPCs.Fade(-5, 51))
			.Then(new Actions.NPCs.Move(new Vector2(0.5f, 0f), 60));
		Segments.AnimationSegmentWithActions<NPC> animationSegmentWithActions4 = new Segments.NPCSegment(startTime, 638, sceneAnchorPosition + new Vector2(num3 * 2 - 60, 0f), _originAtBottom).Then(new Actions.NPCs.Variant(2)).Then(new Actions.NPCs.Fade(255)).With(new Actions.NPCs.Fade(-5, 51))
			.Then(new Actions.NPCs.Move(new Vector2(0.5f, 0f), 60));
		Segments.AnimationSegmentWithActions<NPC> animationSegmentWithActions5 = new Segments.NPCSegment(startTime, 637, sceneAnchorPosition + new Vector2(num3 * 3 - 60, 0f), _originAtBottom).Then(new Actions.NPCs.Variant(4)).Then(new Actions.NPCs.Fade(255)).With(new Actions.NPCs.Fade(-5, 51))
			.Then(new Actions.NPCs.Move(new Vector2(0.5f, 0f), 60));
		Main.instance.LoadProjectile(748);
		Asset<Texture2D> val2 = TextureAssets.Projectile[748];
		DrawData data2 = new DrawData(val2.get_Value(), Vector2.Zero, null, Color.White, 0f, val2.Size() / 2f, 1f, SpriteEffects.None);
		Segments.AnimationSegmentWithActions<Segments.LooseSprite> animationSegmentWithActions6 = new Segments.SpriteSegment(val2, startTime, data2, sceneAnchorPosition + new Vector2(num3 * 3 - 20, 0f)).Then(new Actions.Sprites.Fade(0f)).With(new Actions.Sprites.Fade(1f, 51)).Then(new Actions.Sprites.Wait(60));
		num += (int)animationSegmentWithActions2.DedicatedTimeNeeded;
		int num4 = 90;
		float num5 = 0.5f;
		animationSegmentWithActions2.Then(new Actions.NPCs.Move(new Vector2(1f * num5, 0f), num4));
		animationSegmentWithActions3.Then(new Actions.NPCs.Move(new Vector2(0.5f * num5, 0f), num4));
		animationSegmentWithActions4.Then(new Actions.NPCs.Move(new Vector2(0.6f * num5, 0f), num4));
		animationSegmentWithActions5.Then(new Actions.NPCs.Move(new Vector2(0.8f * num5, 0f), num4));
		animationSegmentWithActions6.Then(new Actions.Sprites.SimulateGravity(new Vector2(0.82f * num5, 0f), Vector2.Zero, 0.07f, num4));
		animationSegmentWithActions.Then(new Actions.Sprites.Wait(num4));
		num += num4;
		Segments.AnimationSegmentWithActions<NPC> animationSegmentWithActions7 = new Segments.NPCSegment(num, 588, sceneAnchorPosition + new Vector2(-70f, 0f), _originAtBottom).Then(new Actions.NPCs.Fade(255)).With(new Actions.NPCs.Fade(-5, 51)).Then(new Actions.NPCs.Move(new Vector2(0.7f * num5, 0f), 60));
		int num6 = (int)animationSegmentWithActions7.DedicatedTimeNeeded;
		animationSegmentWithActions2.Then(new Actions.NPCs.Move(new Vector2(1f * num5, 0f), num6));
		animationSegmentWithActions3.Then(new Actions.NPCs.Move(new Vector2(0.85f * num5, 0f), num6));
		animationSegmentWithActions4.Then(new Actions.NPCs.Move(new Vector2(0.7f * num5, 0f), num6));
		animationSegmentWithActions5.Then(new Actions.NPCs.Move(new Vector2(0.65f * num5, 0f), num6));
		animationSegmentWithActions6.Then(new Actions.Sprites.SimulateGravity(new Vector2(1f * num5, 0f), Vector2.Zero, 0.07f, num6));
		animationSegmentWithActions.Then(new Actions.Sprites.Wait(num6));
		num += num6;
		int num7 = 90;
		int num8 = num7 * 2 + num7 / 2;
		Segments.EmoteSegment item = new Segments.EmoteSegment(1, num, num7, sceneAnchorPosition + new Vector2(-70f + 42f * num5, 0f) + _emoteBubbleOffsetWhenOnLeft, SpriteEffects.FlipHorizontally, new Vector2(1f * num5, 0f));
		Segments.EmoteSegment item2 = new Segments.EmoteSegment(15, num + num7 / 2, num7, sceneAnchorPosition + new Vector2((float)(80 + num6) * num5, 0f) + _emoteBubbleOffsetWhenOnLeft, SpriteEffects.FlipHorizontally, new Vector2(1f * num5, 0f));
		animationSegmentWithActions7.Then(new Actions.NPCs.Move(new Vector2(1f * num5, 0f), num8));
		animationSegmentWithActions2.Then(new Actions.NPCs.Move(new Vector2(1f * num5, 0f), num8));
		animationSegmentWithActions3.Then(new Actions.NPCs.Move(new Vector2(0.72f * num5, 0f), num8));
		animationSegmentWithActions4.Then(new Actions.NPCs.Move(new Vector2(0.7f * num5, 0f), num8));
		animationSegmentWithActions5.Then(new Actions.NPCs.Move(new Vector2(0.8f * num5, 0f), num8));
		animationSegmentWithActions6.Then(new Actions.Sprites.SimulateGravity(new Vector2(0.85f * num5, 0f), Vector2.Zero, 0.07f, num8));
		animationSegmentWithActions.Then(new Actions.Sprites.Wait(num8));
		num += num8;
		animationSegmentWithActions2.Then(new Actions.NPCs.Move(new Vector2(0.5f * num5, 0f), 120)).With(new Actions.NPCs.Fade(2, 127));
		animationSegmentWithActions7.Then(new Actions.NPCs.Move(new Vector2(0.5f * num5, 0f), 120)).With(new Actions.NPCs.Fade(2, 127));
		animationSegmentWithActions3.Then(new Actions.NPCs.Move(new Vector2(0.6f * num5, 0f), 120)).With(new Actions.NPCs.Fade(2, 127));
		animationSegmentWithActions4.Then(new Actions.NPCs.Move(new Vector2(0.7f * num5, 0f), 120)).With(new Actions.NPCs.Fade(2, 127));
		animationSegmentWithActions5.Then(new Actions.NPCs.Move(new Vector2(0.6f * num5, 0f), 120)).With(new Actions.NPCs.Fade(2, 127));
		animationSegmentWithActions6.Then(new Actions.Sprites.SimulateGravity(new Vector2(0.5f * num5, 0f), Vector2.Zero, 0.05f, 120)).With(new Actions.Sprites.Fade(0f, 127));
		animationSegmentWithActions.Then(new Actions.Sprites.Fade(0f, 127));
		num += 187;
		_segments.Add(animationSegmentWithActions2);
		_segments.Add(animationSegmentWithActions7);
		_segments.Add(animationSegmentWithActions3);
		_segments.Add(animationSegmentWithActions4);
		_segments.Add(animationSegmentWithActions5);
		_segments.Add(animationSegmentWithActions6);
		_segments.Add(item2);
		_segments.Add(item);
		SegmentInforReport result = default(SegmentInforReport);
		result.totalTime = num - startTime;
		return result;
	}

	private SegmentInforReport PlaySegment_Grox_AnglerAndPirateTalkAboutFish(int startTime, Vector2 sceneAnchorPosition)
	{
		sceneAnchorPosition += GetSceneFixVector();
		int tileFlameData = startTime;
		int flameSeed = 30;
		sceneAnchorPosition.X += flameSeed;
		int num = 90;
		Asset<Texture2D> result = TextureAssets.Extra[222];
		Rectangle rectangle = result.Frame();
		DrawData data = new DrawData(result.get_Value(), Vector2.Zero, rectangle, Color.White, 0f, rectangle.Size() * new Vector2(0.5f, 1f) + new Vector2(flameSeed, -42f), 1f, SpriteEffects.None);
		Segments.AnimationSegmentWithActions<Segments.LooseSprite> animationSegmentWithActions = new Segments.SpriteSegment(result, tileFlameData, data, sceneAnchorPosition + _backgroundOffset).UseShaderEffect(new Segments.SpriteSegment.MaskedFadeEffect()).Then(new Actions.Sprites.Fade(0f)).With(new Actions.Sprites.Fade(1f, 60))
			.Then(new Actions.Sprites.Wait(60));
		_segments.Add(animationSegmentWithActions);
		Segments.AnimationSegmentWithActions<NPC> animationSegmentWithActions2 = new Segments.NPCSegment(startTime, 369, sceneAnchorPosition, _originAtBottom).Then(new Actions.NPCs.LookAt(1)).Then(new Actions.NPCs.Fade(255)).With(new Actions.NPCs.Fade(-5, 51))
			.Then(new Actions.NPCs.Wait(60));
		Segments.AnimationSegmentWithActions<NPC> animationSegmentWithActions3 = new Segments.NPCSegment(startTime, 229, sceneAnchorPosition + new Vector2(num + 60, 0f), _originAtBottom).Then(new Actions.NPCs.LookAt(-1)).Then(new Actions.NPCs.Fade(255)).With(new Actions.NPCs.Fade(-5, 51))
			.Then(new Actions.NPCs.Move(new Vector2(-1f, 0f), 60));
		Asset<Texture2D> val = TextureAssets.Extra[226];
		Rectangle rectangle2 = val.Frame(1, 8);
		DrawData data2 = new DrawData(val.get_Value(), Vector2.Zero, rectangle2, Color.White, 0f, rectangle2.Size() * new Vector2(0.5f, 1f), 1f, SpriteEffects.FlipHorizontally);
		Segments.AnimationSegmentWithActions<Segments.LooseSprite> animationSegmentWithActions4 = new Segments.SpriteSegment(val, tileFlameData, data2, sceneAnchorPosition + new Vector2(num / 2, 4f)).Then(new Actions.Sprites.Fade(0f)).With(new Actions.Sprites.Fade(1f, 51));
		tileFlameData += (int)animationSegmentWithActions2.DedicatedTimeNeeded;
		int num2 = 90;
		int num3 = num2 * 8;
		Segments.EmoteSegment item = new Segments.EmoteSegment(79, tileFlameData, num2, sceneAnchorPosition + _emoteBubbleOffsetWhenOnLeft, SpriteEffects.FlipHorizontally);
		Segments.EmoteSegment item2 = new Segments.EmoteSegment(65, tileFlameData + num2, num2, sceneAnchorPosition + new Vector2(num, 0f) + _emoteBubbleOffsetWhenOnRight, SpriteEffects.None);
		Segments.EmoteSegment item3 = new Segments.EmoteSegment(136, tileFlameData + num2 * 3, num2, sceneAnchorPosition + _emoteBubbleOffsetWhenOnLeft, SpriteEffects.FlipHorizontally);
		Segments.EmoteSegment item4 = new Segments.EmoteSegment(3, tileFlameData + num2 * 5, num2, sceneAnchorPosition + new Vector2(num, 0f) + _emoteBubbleOffsetWhenOnRight, SpriteEffects.None);
		Segments.EmoteSegment item5 = new Segments.EmoteSegment(50, tileFlameData + num2 * 6, num2, sceneAnchorPosition + new Vector2(num, 0f) + _emoteBubbleOffsetWhenOnRight, SpriteEffects.None);
		Segments.EmoteSegment item6 = new Segments.EmoteSegment(15, tileFlameData + num2 * 6, num2, sceneAnchorPosition + _emoteBubbleOffsetWhenOnRight, SpriteEffects.None, new Vector2(-1f, 0f));
		Segments.EmoteSegment item7 = new Segments.EmoteSegment(2, tileFlameData + num2 * 7, num2, sceneAnchorPosition + new Vector2(num, 0f) + _emoteBubbleOffsetWhenOnRight, SpriteEffects.None, new Vector2(-1.25f, 0f));
		animationSegmentWithActions2.Then(new Actions.NPCs.Wait(num2 * 4)).Then(new Actions.NPCs.ShowItem(num2, 2673)).Then(new Actions.NPCs.Wait(num2))
			.Then(new Actions.NPCs.Move(new Vector2(-1f, 0f), num2));
		animationSegmentWithActions3.Then(new Actions.NPCs.Wait(num2 * 2)).Then(new Actions.NPCs.ShowItem(num2, 2480)).Then(new Actions.NPCs.Wait(num2 * 4))
			.Then(new Actions.NPCs.Move(new Vector2(-1.25f, 0f), num2));
		animationSegmentWithActions.Then(new Actions.Sprites.Wait(num3));
		animationSegmentWithActions4.Then(new Actions.Sprites.SetFrameSequence(num3 + 60, new Point[8]
		{
			new Point(0, 0),
			new Point(0, 1),
			new Point(0, 2),
			new Point(0, 3),
			new Point(0, 4),
			new Point(0, 5),
			new Point(0, 6),
			new Point(0, 7)
		}, 5, 0, 0));
		tileFlameData += num3;
		animationSegmentWithActions2.Then(new Actions.NPCs.Move(new Vector2(-0.4f, 0f), 120)).With(new Actions.NPCs.Fade(2, 127));
		animationSegmentWithActions3.Then(new Actions.NPCs.Move(new Vector2(-0.75f, 0f), 120)).With(new Actions.NPCs.Fade(2, 127));
		animationSegmentWithActions4.Then(new Actions.Sprites.Fade(0f, 127));
		animationSegmentWithActions.Then(new Actions.Sprites.Fade(0f, 127));
		tileFlameData += 187;
		_segments.Add(animationSegmentWithActions4);
		_segments.Add(animationSegmentWithActions2);
		_segments.Add(animationSegmentWithActions3);
		_segments.Add(item);
		_segments.Add(item2);
		_segments.Add(item3);
		_segments.Add(item4);
		_segments.Add(item5);
		_segments.Add(item6);
		_segments.Add(item7);
		SegmentInforReport result2 = default(SegmentInforReport);
		result2.totalTime = tileFlameData - startTime;
		return result2;
	}

	private SegmentInforReport PlaySegment_Grox_WizardPartyGirlDyeTraderAndPainterPartyWithBunnies(int startTime, Vector2 sceneAnchorPosition)
	{
		sceneAnchorPosition += GetSceneFixVector();
		int num = startTime;
		int color = -35;
		sceneAnchorPosition.X += color;
		int graveyardVisualIntensity = 34;
		Asset<Texture2D> num4 = TextureAssets.Extra[221];
		Rectangle tileFlameData = num4.Frame();
		DrawData vector = new DrawData(num4.get_Value(), Vector2.Zero, tileFlameData, Color.White, 0f, tileFlameData.Size() * new Vector2(0.5f, 1f) + new Vector2(color, -42f), 1f, SpriteEffects.None);
		Segments.AnimationSegmentWithActions<Segments.LooseSprite> value = new Segments.SpriteSegment(num4, num, vector, sceneAnchorPosition + _backgroundOffset).UseShaderEffect(new Segments.SpriteSegment.MaskedFadeEffect()).Then(new Actions.Sprites.Fade(0f)).With(new Actions.Sprites.Fade(1f, 60))
			.Then(new Actions.Sprites.Wait(60));
		_segments.Add(value);
		Segments.AnimationSegmentWithActions<NPC> i = new Segments.NPCSegment(startTime, 227, sceneAnchorPosition, _originAtBottom).Then(new Actions.NPCs.PartyHard()).Then(new Actions.NPCs.LookAt(1)).Then(new Actions.NPCs.Fade(255))
			.With(new Actions.NPCs.Fade(-5, 51))
			.Then(new Actions.NPCs.Wait(60));
		Segments.AnimationSegmentWithActions<NPC> color2 = new Segments.NPCSegment(startTime, 108, sceneAnchorPosition + new Vector2(graveyardVisualIntensity, 0f), _originAtBottom).Then(new Actions.NPCs.PartyHard()).Then(new Actions.NPCs.LookAt(-1)).Then(new Actions.NPCs.Fade(255))
			.With(new Actions.NPCs.Fade(-5, 51))
			.Then(new Actions.NPCs.Wait(60));
		Segments.AnimationSegmentWithActions<NPC> x = new Segments.NPCSegment(startTime, 207, sceneAnchorPosition + new Vector2(graveyardVisualIntensity * 2 + 60, 0f), _originAtBottom).Then(new Actions.NPCs.PartyHard()).Then(new Actions.NPCs.LookAt(-1)).Then(new Actions.NPCs.Fade(255))
			.With(new Actions.NPCs.Fade(-5, 51))
			.Then(new Actions.NPCs.Move(new Vector2(-1f, 0f), 60));
		Segments.AnimationSegmentWithActions<NPC> y = new Segments.NPCSegment(startTime, 656, sceneAnchorPosition + new Vector2(graveyardVisualIntensity * 2, 0f), _originAtBottom).Then(new Actions.NPCs.Variant(1)).Then(new Actions.NPCs.PartyHard()).Then(new Actions.NPCs.LookAt(1))
			.Then(new Actions.NPCs.Fade(255))
			.With(new Actions.NPCs.Fade(-5, 51))
			.Then(new Actions.NPCs.Move(new Vector2(1f, 0f), 60));
		Segments.AnimationSegmentWithActions<NPC> num5 = new Segments.NPCSegment(startTime, 540, sceneAnchorPosition + new Vector2(graveyardVisualIntensity * 4 + 100, 0f), _originAtBottom).Then(new Actions.NPCs.LookAt(-1)).Then(new Actions.NPCs.Fade(255)).With(new Actions.NPCs.Fade(-5, 51))
			.Then(new Actions.NPCs.Move(new Vector2(-1f, 0f), 60));
		Asset<Texture2D> val = TextureAssets.Extra[238];
		Rectangle num6 = val.Frame(1, 4);
		DrawData num7 = new DrawData(val.get_Value(), Vector2.Zero, num6, Color.White, 0f, num6.Size() * new Vector2(0.5f, 1f), 1f, SpriteEffects.FlipHorizontally);
		Segments.AnimationSegmentWithActions<Segments.LooseSprite> num8 = new Segments.SpriteSegment(val, num, num7, sceneAnchorPosition + new Vector2(60f, 2f)).Then(new Actions.Sprites.Fade(0f)).With(new Actions.Sprites.Fade(1f, 51));
		Segments.AnimationSegmentWithActions<Segments.LooseSprite> seed = new Segments.SpriteSegment(val, num, num7, sceneAnchorPosition + new Vector2(150f, 2f)).Then(new Actions.Sprites.Fade(0f)).With(new Actions.Sprites.Fade(1f, 51));
		num += (int)i.DedicatedTimeNeeded;
		int typeCache = 90;
		int num9 = typeCache * 4;
		Segments.EmoteSegment j = new Segments.EmoteSegment(127, num, typeCache, sceneAnchorPosition + _emoteBubbleOffsetWhenOnLeft, SpriteEffects.FlipHorizontally);
		Segments.EmoteSegment num2 = new Segments.EmoteSegment(6, num + typeCache, typeCache, sceneAnchorPosition + new Vector2(graveyardVisualIntensity, 0f) + _emoteBubbleOffsetWhenOnRight, SpriteEffects.None);
		Segments.EmoteSegment num3 = new Segments.EmoteSegment(136, num + typeCache * 2, typeCache, sceneAnchorPosition + new Vector2(graveyardVisualIntensity * 2, 0f) + _emoteBubbleOffsetWhenOnRight, SpriteEffects.None);
		Segments.EmoteSegment item = new Segments.EmoteSegment(129, num + typeCache * 3, typeCache, sceneAnchorPosition + _emoteBubbleOffsetWhenOnLeft, SpriteEffects.FlipHorizontally);
		i.Then(new Actions.NPCs.Wait(num9));
		color2.Then(new Actions.NPCs.Wait(typeCache * 2)).Then(new Actions.NPCs.LookAt(1)).Then(new Actions.NPCs.Wait(typeCache))
			.Then(new Actions.NPCs.LookAt(-1))
			.Then(new Actions.NPCs.Wait(typeCache));
		x.Then(new Actions.NPCs.Wait(num9));
		value.Then(new Actions.Sprites.Wait(num9));
		num5.Then(new Actions.NPCs.Move(new Vector2(-0.5f, 0f), num9 / 3)).Then(new Actions.NPCs.Wait(num9 / 3)).Then(new Actions.NPCs.Move(new Vector2(0.4f, 0f), num9 / 3));
		y.Then(new Actions.NPCs.Move(new Vector2(-0.6f, 0f), num9 / 3)).Then(new Actions.NPCs.Move(new Vector2(0.5f, 0f), num9 / 3)).Then(new Actions.NPCs.Wait(num9 / 3));
		num += num9;
		Segments.AnimationSegmentWithActions<NPC> num12 = new Segments.NPCSegment(num - 60, 208, sceneAnchorPosition + new Vector2(graveyardVisualIntensity * 5 + 100, 0f), _originAtBottom).Then(new Actions.NPCs.PartyHard()).Then(new Actions.NPCs.LookAt(-1)).Then(new Actions.NPCs.Fade(255))
			.With(new Actions.NPCs.Fade(-5, 51))
			.Then(new Actions.NPCs.Move(new Vector2(-1f, 0f), 60));
		int num13 = (int)num12.DedicatedTimeNeeded - 60;
		num += num13;
		i.Then(new Actions.NPCs.Wait(num13));
		color2.Then(new Actions.NPCs.Wait(num13));
		x.Then(new Actions.NPCs.Wait(num13));
		num5.Then(new Actions.NPCs.Wait(num13));
		y.Then(new Actions.NPCs.Wait(num13));
		value.Then(new Actions.Sprites.Wait(num13));
		Segments.EmoteSegment num14 = new Segments.EmoteSegment(128, num, typeCache, sceneAnchorPosition + new Vector2(graveyardVisualIntensity * 5 + 40, 0f) + _emoteBubbleOffsetWhenOnRight, SpriteEffects.None);
		num += typeCache;
		i.Then(new Actions.NPCs.Wait(typeCache));
		color2.Then(new Actions.NPCs.LookAt(1)).Then(new Actions.NPCs.Wait(typeCache));
		x.Then(new Actions.NPCs.LookAt(1)).Then(new Actions.NPCs.Wait(typeCache));
		num5.Then(new Actions.NPCs.Wait(typeCache));
		y.Then(new Actions.NPCs.Wait(typeCache));
		num12.Then(new Actions.NPCs.Wait(typeCache));
		value.Then(new Actions.Sprites.Wait(typeCache));
		Segments.EmoteSegment num16 = new Segments.EmoteSegment(128, num, typeCache, sceneAnchorPosition + _emoteBubbleOffsetWhenOnLeft, SpriteEffects.FlipHorizontally);
		Segments.EmoteSegment num17 = new Segments.EmoteSegment(128, num, typeCache, sceneAnchorPosition + new Vector2(graveyardVisualIntensity, 0f) + _emoteBubbleOffsetWhenOnLeft, SpriteEffects.FlipHorizontally);
		Segments.EmoteSegment num18 = new Segments.EmoteSegment(128, num, typeCache, sceneAnchorPosition + new Vector2(graveyardVisualIntensity * 2, 0f) + _emoteBubbleOffsetWhenOnLeft, SpriteEffects.FlipHorizontally);
		Segments.EmoteSegment num10 = new Segments.EmoteSegment(3, num, typeCache, sceneAnchorPosition + new Vector2(graveyardVisualIntensity * 5 - 10, 24f) + _emoteBubbleOffsetWhenOnLeft, SpriteEffects.FlipHorizontally);
		Segments.EmoteSegment num11 = new Segments.EmoteSegment(0, num, typeCache, sceneAnchorPosition + new Vector2(graveyardVisualIntensity * 4 - 20, 24f) + _emoteBubbleOffsetWhenOnLeft, SpriteEffects.FlipHorizontally);
		i.Then(new Actions.NPCs.Wait(typeCache));
		color2.Then(new Actions.NPCs.Wait(typeCache));
		x.Then(new Actions.NPCs.Wait(typeCache));
		num5.Then(new Actions.NPCs.Wait(typeCache));
		y.Then(new Actions.NPCs.Wait(typeCache));
		num12.Then(new Actions.NPCs.Wait(typeCache));
		value.Then(new Actions.Sprites.Wait(typeCache));
		num += typeCache;
		num8.Then(new Actions.Sprites.SetFrameSequence(num - startTime, new Point[4]
		{
			new Point(0, 0),
			new Point(0, 1),
			new Point(0, 2),
			new Point(0, 3)
		}, 10, 0, 0));
		seed.Then(new Actions.Sprites.SetFrameSequence(num - startTime, new Point[4]
		{
			new Point(0, 2),
			new Point(0, 3),
			new Point(0, 0),
			new Point(0, 1)
		}, 10, 0, 0));
		i.Then(new Actions.NPCs.Move(new Vector2(1f, 0f), 120)).With(new Actions.NPCs.Fade(2, 127));
		color2.Then(new Actions.NPCs.Move(new Vector2(1f, 0f), 120)).With(new Actions.NPCs.Fade(2, 127));
		x.Then(new Actions.NPCs.Move(new Vector2(1f, 0f), 120)).With(new Actions.NPCs.Fade(2, 127));
		num5.Then(new Actions.NPCs.Move(new Vector2(0.5f, 0f), 120)).With(new Actions.NPCs.Fade(2, 127));
		y.Then(new Actions.NPCs.Move(new Vector2(0.75f, 0f), 120)).With(new Actions.NPCs.Fade(2, 127));
		num12.Then(new Actions.NPCs.Move(new Vector2(0.5f, 0f), 120)).With(new Actions.NPCs.Fade(2, 127));
		num8.Then(new Actions.Sprites.Fade(0f, 127));
		seed.Then(new Actions.Sprites.Fade(0f, 127));
		value.Then(new Actions.Sprites.Fade(0f, 127));
		num += 187;
		_segments.Add(num8);
		_segments.Add(seed);
		_segments.Add(i);
		_segments.Add(color2);
		_segments.Add(x);
		_segments.Add(num5);
		_segments.Add(y);
		_segments.Add(num12);
		_segments.Add(j);
		_segments.Add(num2);
		_segments.Add(num3);
		_segments.Add(item);
		_segments.Add(num14);
		_segments.Add(num16);
		_segments.Add(num17);
		_segments.Add(num18);
		_segments.Add(num10);
		_segments.Add(num11);
		SegmentInforReport num15 = default(SegmentInforReport);
		num15.totalTime = num - startTime;
		return num15;
	}

	private SegmentInforReport PlaySegment_Grox_DemolitionistAndArmsDealerArguingThenNurseComes(int startTime, Vector2 sceneAnchorPosition)
	{
		sceneAnchorPosition += GetSceneFixVector();
		int num = startTime;
		int num2 = -30;
		sceneAnchorPosition.X += num2;
		Asset<Texture2D> val = TextureAssets.Extra[234];
		Rectangle rectangle = val.Frame();
		DrawData data = new DrawData(val.get_Value(), Vector2.Zero, rectangle, Color.White, 0f, rectangle.Size() * new Vector2(0.5f, 1f) + new Vector2(num2, -42f), 1f, SpriteEffects.None);
		Segments.AnimationSegmentWithActions<Segments.LooseSprite> animationSegmentWithActions = new Segments.SpriteSegment(val, num, data, sceneAnchorPosition + _backgroundOffset).UseShaderEffect(new Segments.SpriteSegment.MaskedFadeEffect()).Then(new Actions.Sprites.Fade(0f)).With(new Actions.Sprites.Fade(1f, 60))
			.Then(new Actions.Sprites.Wait(120));
		_segments.Add(animationSegmentWithActions);
		Segments.AnimationSegmentWithActions<NPC> animationSegmentWithActions2 = new Segments.NPCSegment(startTime, 38, sceneAnchorPosition, _originAtBottom).Then(new Actions.NPCs.Fade(255)).With(new Actions.NPCs.Fade(-5, 51)).Then(new Actions.NPCs.Move(new Vector2(1f, 0f), 60))
			.Then(new Actions.NPCs.Wait(60));
		int num3 = 90;
		Segments.AnimationSegmentWithActions<NPC> animationSegmentWithActions3 = new Segments.NPCSegment(startTime, 19, sceneAnchorPosition + new Vector2(120 + num3, 0f), _originAtBottom).Then(new Actions.NPCs.Fade(255)).With(new Actions.NPCs.Fade(-5, 51)).Then(new Actions.NPCs.Move(new Vector2(-1f, 0f), 60))
			.Then(new Actions.NPCs.Wait(60));
		num += (int)animationSegmentWithActions2.DedicatedTimeNeeded;
		int num4 = 90 * 4;
		int num5 = 90;
		int num6 = 90 / 2;
		int num7 = 90 + 90 / 2;
		int num8 = 90 * 2;
		Segments.EmoteSegment item = new Segments.EmoteSegment(81, num, num5, sceneAnchorPosition + new Vector2(60f, 0f) + _emoteBubbleOffsetWhenOnLeft, SpriteEffects.FlipHorizontally);
		Segments.EmoteSegment item2 = new Segments.EmoteSegment(82, num + num6, num5, sceneAnchorPosition + new Vector2(60 + num3, 0f) + _emoteBubbleOffsetWhenOnRight, SpriteEffects.None);
		Segments.EmoteSegment item3 = new Segments.EmoteSegment(135, num + num7, num5, sceneAnchorPosition + new Vector2(60f, 0f) + _emoteBubbleOffsetWhenOnLeft, SpriteEffects.FlipHorizontally);
		Segments.EmoteSegment item4 = new Segments.EmoteSegment(135, num + num8, num5, sceneAnchorPosition + new Vector2(60 + num3, 0f) + _emoteBubbleOffsetWhenOnRight, SpriteEffects.None);
		animationSegmentWithActions2.Then(new Actions.NPCs.Wait(num4));
		animationSegmentWithActions3.Then(new Actions.NPCs.Wait(num4));
		animationSegmentWithActions.Then(new Actions.Sprites.Wait(num4));
		num += num4;
		int num9 = num3 - 30;
		int num10 = 120;
		Segments.AnimationSegmentWithActions<NPC> animationSegmentWithActions4 = new Segments.NPCSegment(num - num10, 18, sceneAnchorPosition + new Vector2(120 + num9, 0f), _originAtBottom).Then(new Actions.NPCs.Fade(255)).With(new Actions.NPCs.Fade(-5, 51)).Then(new Actions.NPCs.Move(new Vector2(-1f, 0f), 60))
			.Then(new Actions.NPCs.Wait(20));
		int num11 = (int)animationSegmentWithActions4.DedicatedTimeNeeded - num10;
		num += num11;
		animationSegmentWithActions2.Then(new Actions.NPCs.Wait(num11));
		animationSegmentWithActions3.Then(new Actions.NPCs.Wait(num11));
		animationSegmentWithActions.Then(new Actions.Sprites.Wait(num11));
		animationSegmentWithActions4.Then(new Actions.NPCs.LookAt(-1));
		Segments.EmoteSegment item5 = new Segments.EmoteSegment(77, num, num5, sceneAnchorPosition + new Vector2(60 + num9, 0f) + _emoteBubbleOffsetWhenOnLeft, SpriteEffects.FlipHorizontally);
		Segments.EmoteSegment item6 = new Segments.EmoteSegment(15, num + num5, num5, sceneAnchorPosition + new Vector2(60 + num9, 0f) + _emoteBubbleOffsetWhenOnRight, SpriteEffects.None);
		animationSegmentWithActions2.Then(new Actions.NPCs.Wait(num5));
		animationSegmentWithActions3.Then(new Actions.NPCs.Wait(num5));
		animationSegmentWithActions4.Then(new Actions.NPCs.Wait(num5));
		animationSegmentWithActions.Then(new Actions.Sprites.Wait(num5));
		num += num5;
		animationSegmentWithActions4.Then(new Actions.NPCs.LookAt(1));
		animationSegmentWithActions2.Then(new Actions.NPCs.Wait(num5));
		animationSegmentWithActions3.Then(new Actions.NPCs.Wait(num5));
		animationSegmentWithActions4.Then(new Actions.NPCs.Wait(num5));
		animationSegmentWithActions.Then(new Actions.Sprites.Wait(num5));
		num += num5;
		Segments.EmoteSegment item7 = new Segments.EmoteSegment(10, num, num5, sceneAnchorPosition + new Vector2(60f, 0f) + _emoteBubbleOffsetWhenOnLeft, SpriteEffects.FlipHorizontally);
		Segments.EmoteSegment item8 = new Segments.EmoteSegment(10, num, num5, sceneAnchorPosition + new Vector2(60 + num3, 0f) + _emoteBubbleOffsetWhenOnRight, SpriteEffects.None);
		animationSegmentWithActions2.Then(new Actions.NPCs.Wait(num5));
		animationSegmentWithActions3.Then(new Actions.NPCs.Wait(num5));
		animationSegmentWithActions4.Then(new Actions.NPCs.Wait(num5));
		animationSegmentWithActions.Then(new Actions.Sprites.Wait(num5));
		num += num5;
		Vector2 vector = new Vector2(-1f, 0f);
		Segments.EmoteSegment item9 = new Segments.EmoteSegment(77, num, num5, sceneAnchorPosition + new Vector2(60 + num3, 0f) + _emoteBubbleOffsetWhenOnRight, SpriteEffects.None, vector);
		Segments.EmoteSegment item10 = new Segments.EmoteSegment(77, num + num5 / 2, num5, sceneAnchorPosition + new Vector2(60f, 0f) + _emoteBubbleOffsetWhenOnRight, SpriteEffects.None, vector);
		int num12 = num5 + num5 / 2;
		animationSegmentWithActions3.Then(new Actions.NPCs.Move(vector, num12));
		animationSegmentWithActions2.Then(new Actions.NPCs.Wait(num5 / 2)).Then(new Actions.NPCs.Move(vector, num5));
		animationSegmentWithActions4.Then(new Actions.NPCs.Wait(num5 / 2 + 20)).Then(new Actions.NPCs.Move(vector, num5 - 20));
		animationSegmentWithActions.Then(new Actions.Sprites.Wait(num12));
		num += num12;
		animationSegmentWithActions2.Then(new Actions.NPCs.Move(new Vector2(-0.5f, 0f), 120)).With(new Actions.NPCs.Fade(2, 127));
		animationSegmentWithActions3.Then(new Actions.NPCs.Move(new Vector2(-0.5f, 0f), 120)).With(new Actions.NPCs.Fade(2, 127));
		animationSegmentWithActions4.Then(new Actions.NPCs.Move(new Vector2(-0.5f, 0f), 120)).With(new Actions.NPCs.Fade(2, 127));
		animationSegmentWithActions.Then(new Actions.Sprites.Fade(0f, 127));
		num += 187;
		_segments.Add(animationSegmentWithActions4);
		_segments.Add(animationSegmentWithActions2);
		_segments.Add(animationSegmentWithActions3);
		_segments.Add(item);
		_segments.Add(item2);
		_segments.Add(item3);
		_segments.Add(item4);
		_segments.Add(item5);
		_segments.Add(item6);
		_segments.Add(item7);
		_segments.Add(item8);
		_segments.Add(item10);
		_segments.Add(item9);
		SegmentInforReport result = default(SegmentInforReport);
		result.totalTime = num - startTime;
		return result;
	}

	private SegmentInforReport PlaySegment_TinkererAndMechanic(int startTime, Vector2 sceneAnchorPosition)
	{
		sceneAnchorPosition += GetSceneFixVector();
		int color = startTime;
		Asset<Texture2D> origin = TextureAssets.Extra[237];
		Rectangle value = origin.Frame();
		DrawData tileLight = new DrawData(origin.get_Value(), Vector2.Zero, value, Color.White, 0f, value.Size() * new Vector2(0.5f, 1f) + new Vector2(0f, -42f), 1f, SpriteEffects.None);
		Segments.AnimationSegmentWithActions<Segments.LooseSprite> position = new Segments.SpriteSegment(origin, color, tileLight, sceneAnchorPosition + _backgroundOffset).UseShaderEffect(new Segments.SpriteSegment.MaskedFadeEffect()).Then(new Actions.Sprites.Fade(0f)).With(new Actions.Sprites.Fade(1f, 60));
		_segments.Add(position);
		Segments.AnimationSegmentWithActions<NPC> slices = new Segments.NPCSegment(startTime, 107, sceneAnchorPosition, _originAtBottom).Then(new Actions.NPCs.Fade(255)).With(new Actions.NPCs.Fade(-5, 51)).Then(new Actions.NPCs.Move(new Vector2(1f, 0f), 60))
			.Then(new Actions.NPCs.Wait(60));
		position.Then(new Actions.Sprites.Wait((int)slices.DedicatedTimeNeeded));
		color += (int)slices.DedicatedTimeNeeded;
		int vector = 24;
		Segments.AnimationSegmentWithActions<NPC> tint = new Segments.NPCSegment(color, 124, sceneAnchorPosition + new Vector2(120 + vector, 0f), _originAtBottom).Then(new Actions.NPCs.LookAt(-1)).Then(new Actions.NPCs.Fade(255)).With(new Actions.NPCs.Fade(-5, 51))
			.Then(new Actions.NPCs.Move(new Vector2(-1f, 0f), 60));
		color += (int)tint.DedicatedTimeNeeded;
		slices.Then(new Actions.NPCs.Wait((int)tint.DedicatedTimeNeeded));
		position.Then(new Actions.Sprites.Wait((int)tint.DedicatedTimeNeeded));
		int i = 120;
		Segments.EmoteSegment num = new Segments.EmoteSegment(0, color, i, sceneAnchorPosition + new Vector2(60f, 0f) + _emoteBubbleOffsetWhenOnLeft, SpriteEffects.FlipHorizontally);
		Segments.EmoteSegment num2 = new Segments.EmoteSegment(0, color, i, sceneAnchorPosition + new Vector2(60 + vector, 0f) + _emoteBubbleOffsetWhenOnRight, SpriteEffects.None);
		slices.Then(new Actions.NPCs.Wait(i));
		tint.Then(new Actions.NPCs.Wait(i));
		position.Then(new Actions.Sprites.Wait(i));
		color += i;
		slices.Then(new Actions.NPCs.Move(new Vector2(-0.5f, 0f), 120)).With(new Actions.NPCs.Fade(2, 127));
		tint.Then(new Actions.NPCs.Move(new Vector2(-0.5f, 0f), 120)).With(new Actions.NPCs.Fade(2, 127));
		position.Then(new Actions.Sprites.Fade(0f, 127));
		color += 187;
		_segments.Add(slices);
		_segments.Add(tint);
		_segments.Add(num);
		_segments.Add(num2);
		SegmentInforReport slices2 = default(SegmentInforReport);
		slices2.totalTime = color - startTime;
		return slices2;
	}

	private SegmentInforReport PlaySegment_ClothierChasingTruffle(int startTime, Vector2 sceneAnchorPosition)
	{
		sceneAnchorPosition += GetSceneFixVector();
		int num = startTime;
		int num2 = 10;
		sceneAnchorPosition.X += num2;
		Asset<Texture2D> flag = TextureAssets.Extra[225];
		Rectangle rectangle = flag.Frame();
		DrawData data = new DrawData(flag.get_Value(), Vector2.Zero, rectangle, Color.White, 0f, rectangle.Size() * new Vector2(0.5f, 1f) + new Vector2(num2, -42f), 1f, SpriteEffects.None);
		Segments.AnimationSegmentWithActions<Segments.LooseSprite> animationSegmentWithActions = new Segments.SpriteSegment(flag, num, data, sceneAnchorPosition + _backgroundOffset).UseShaderEffect(new Segments.SpriteSegment.MaskedFadeEffect()).Then(new Actions.Sprites.Fade(0f)).With(new Actions.Sprites.Fade(1f, 60));
		_segments.Add(animationSegmentWithActions);
		Segments.AnimationSegmentWithActions<NPC> animationSegmentWithActions2 = new Segments.NPCSegment(startTime, 160, sceneAnchorPosition + new Vector2(20f, 0f), _originAtBottom).Then(new Actions.NPCs.Fade(255)).With(new Actions.NPCs.Fade(-5, 51)).Then(new Actions.NPCs.LookAt(1))
			.Then(new Actions.NPCs.Wait(60))
			.Then(new Actions.NPCs.LookAt(-1));
		animationSegmentWithActions.Then(new Actions.Sprites.Wait((int)animationSegmentWithActions2.DedicatedTimeNeeded));
		num += (int)animationSegmentWithActions2.DedicatedTimeNeeded;
		int num3 = 60;
		Segments.EmoteSegment item = new Segments.EmoteSegment(10, num, num3, sceneAnchorPosition + new Vector2(20f, 0f) + _emoteBubbleOffsetWhenOnRight, SpriteEffects.None);
		animationSegmentWithActions2.Then(new Actions.NPCs.Wait(num3));
		animationSegmentWithActions.Then(new Actions.Sprites.Wait(num3));
		num += num3;
		Segments.EmoteSegment item2 = new Segments.EmoteSegment(3, num, num3, sceneAnchorPosition + new Vector2(20f, 0f) + _emoteBubbleOffsetWhenOnRight, SpriteEffects.None);
		animationSegmentWithActions2.Then(new Actions.NPCs.Wait(num3));
		animationSegmentWithActions.Then(new Actions.Sprites.Wait(num3));
		num += num3;
		Vector2 vector = new Vector2(1.2f, 0f);
		Vector2 vector2 = new Vector2(1f, 0f);
		Segments.AnimationSegmentWithActions<NPC> item3 = new Segments.NPCSegment(num, 54, sceneAnchorPosition + new Vector2(-100f, 0f), _originAtBottom).Then(new Actions.NPCs.Fade(255)).With(new Actions.NPCs.Fade(-5, 51)).Then(new Actions.NPCs.Move(vector, 60))
			.Then(new Actions.NPCs.Move(vector, 130))
			.With(new Actions.NPCs.Fade(2, 127));
		animationSegmentWithActions2.Then(new Actions.NPCs.Move(vector2, 60)).Then(new Actions.NPCs.Move(vector2, 130)).With(new Actions.NPCs.Fade(2, 127));
		animationSegmentWithActions.Then(new Actions.Sprites.Wait(60)).Then(new Actions.Sprites.Wait(130)).With(new Actions.Sprites.Fade(0f, 127));
		int num4 = 10;
		int num5 = 40;
		int timeToPlay = 70;
		Segments.EmoteSegment item4 = new Segments.EmoteSegment(134, num + num4, timeToPlay, sceneAnchorPosition + new Vector2(20f, 0f) + _emoteBubbleOffsetWhenOnLeft + vector2 * num4, SpriteEffects.FlipHorizontally, vector2);
		Segments.EmoteSegment item5 = new Segments.EmoteSegment(15, num + num5, timeToPlay, sceneAnchorPosition + new Vector2(-100f, 0f) + _emoteBubbleOffsetWhenOnLeft + vector * num5, SpriteEffects.FlipHorizontally, vector);
		_segments.Add(item3);
		_segments.Add(animationSegmentWithActions2);
		_segments.Add(item);
		_segments.Add(item2);
		_segments.Add(item4);
		_segments.Add(item5);
		num += 200;
		SegmentInforReport result = default(SegmentInforReport);
		result.totalTime = num - startTime;
		return result;
	}
}
