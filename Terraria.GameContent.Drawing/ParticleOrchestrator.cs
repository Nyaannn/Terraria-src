using System;
using Microsoft.Xna.Framework;
using Terraria.Audio;
using Terraria.GameContent.NetModules;
using Terraria.Graphics.Renderers;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.Net;

namespace Terraria.GameContent.Drawing;

public class ParticleOrchestrator
{
	private static ParticlePool<FadingParticle> _poolFading = new ParticlePool<FadingParticle>(200, GetNewFadingParticle);

	private static ParticlePool<LittleFlyingCritterParticle> _poolFlies = new ParticlePool<LittleFlyingCritterParticle>(200, GetNewPooFlyParticle);

	private static ParticlePool<ItemTransferParticle> _poolItemTransfer = new ParticlePool<ItemTransferParticle>(100, GetNewItemTransferParticle);

	private static ParticlePool<FlameParticle> _poolFlame = new ParticlePool<FlameParticle>(200, GetNewFlameParticle);

	private static ParticlePool<RandomizedFrameParticle> _poolRandomizedFrame = new ParticlePool<RandomizedFrameParticle>(200, GetNewRandomizedFrameParticle);

	private static ParticlePool<PrettySparkleParticle> _poolPrettySparkle = new ParticlePool<PrettySparkleParticle>(200, GetNewPrettySparkleParticle);

	private static ParticlePool<GasParticle> _poolGas = new ParticlePool<GasParticle>(200, GetNewGasParticle);

	public static void RequestParticleSpawn(bool clientOnly, ParticleOrchestraType type, ParticleOrchestraSettings settings, int? overrideInvokingPlayerIndex = null)
	{
		settings.IndexOfPlayerWhoInvokedThis = (byte)Main.myPlayer;
		if (overrideInvokingPlayerIndex.HasValue)
		{
			settings.IndexOfPlayerWhoInvokedThis = (byte)overrideInvokingPlayerIndex.Value;
		}
		if (clientOnly)
		{
			SpawnParticlesDirect(type, settings);
		}
		else
		{
			NetManager.Instance.SendToServerAndSelf(NetParticlesModule.Serialize(type, settings));
		}
	}

	public static void BroadcastParticleSpawn(ParticleOrchestraType type, ParticleOrchestraSettings settings)
	{
		settings.IndexOfPlayerWhoInvokedThis = (byte)Main.myPlayer;
		NetManager.Instance.BroadcastOrLoopback(NetParticlesModule.Serialize(type, settings));
	}

	public static void BroadcastOrRequestParticleSpawn(ParticleOrchestraType type, ParticleOrchestraSettings settings)
	{
		settings.IndexOfPlayerWhoInvokedThis = (byte)Main.myPlayer;
		if (Main.netMode == 1)
		{
			NetManager.Instance.SendToServerAndSelf(NetParticlesModule.Serialize(type, settings));
		}
		else
		{
			NetManager.Instance.BroadcastOrLoopback(NetParticlesModule.Serialize(type, settings));
		}
	}

	private static FadingParticle GetNewFadingParticle()
	{
		return new FadingParticle();
	}

	private static LittleFlyingCritterParticle GetNewPooFlyParticle()
	{
		return new LittleFlyingCritterParticle();
	}

	private static ItemTransferParticle GetNewItemTransferParticle()
	{
		return new ItemTransferParticle();
	}

	private static FlameParticle GetNewFlameParticle()
	{
		return new FlameParticle();
	}

	private static RandomizedFrameParticle GetNewRandomizedFrameParticle()
	{
		return new RandomizedFrameParticle();
	}

	private static PrettySparkleParticle GetNewPrettySparkleParticle()
	{
		return new PrettySparkleParticle();
	}

	private static GasParticle GetNewGasParticle()
	{
		return new GasParticle();
	}

	public static void SpawnParticlesDirect(ParticleOrchestraType type, ParticleOrchestraSettings settings)
	{
		if (Main.netMode != 2)
		{
			switch (type)
			{
			case ParticleOrchestraType.Keybrand:
				Spawn_Keybrand(settings);
				break;
			case ParticleOrchestraType.FlameWaders:
				Spawn_FlameWaders(settings);
				break;
			case ParticleOrchestraType.StellarTune:
				Spawn_StellarTune(settings);
				break;
			case ParticleOrchestraType.WallOfFleshGoatMountFlames:
				Spawn_WallOfFleshGoatMountFlames(settings);
				break;
			case ParticleOrchestraType.BlackLightningHit:
				Spawn_BlackLightningHit(settings);
				break;
			case ParticleOrchestraType.RainbowRodHit:
				Spawn_RainbowRodHit(settings);
				break;
			case ParticleOrchestraType.BlackLightningSmall:
				Spawn_BlackLightningSmall(settings);
				break;
			case ParticleOrchestraType.StardustPunch:
				Spawn_StardustPunch(settings);
				break;
			case ParticleOrchestraType.PrincessWeapon:
				Spawn_PrincessWeapon(settings);
				break;
			case ParticleOrchestraType.PaladinsHammer:
				Spawn_PaladinsHammer(settings);
				break;
			case ParticleOrchestraType.NightsEdge:
				Spawn_NightsEdge(settings);
				break;
			case ParticleOrchestraType.SilverBulletSparkle:
				Spawn_SilverBulletSparkle(settings);
				break;
			case ParticleOrchestraType.TrueNightsEdge:
				Spawn_TrueNightsEdge(settings);
				break;
			case ParticleOrchestraType.ChlorophyteLeafCrystalPassive:
				Spawn_LeafCrystalPassive(settings);
				break;
			case ParticleOrchestraType.ChlorophyteLeafCrystalShot:
				Spawn_LeafCrystalShot(settings);
				break;
			case ParticleOrchestraType.AshTreeShake:
				Spawn_AshTreeShake(settings);
				break;
			case ParticleOrchestraType.TerraBlade:
				Spawn_TerraBlade(settings);
				break;
			case ParticleOrchestraType.Excalibur:
				Spawn_Excalibur(settings);
				break;
			case ParticleOrchestraType.TrueExcalibur:
				Spawn_TrueExcalibur(settings);
				break;
			case ParticleOrchestraType.PetExchange:
				Spawn_PetExchange(settings);
				break;
			case ParticleOrchestraType.SlapHand:
				Spawn_SlapHand(settings);
				break;
			case ParticleOrchestraType.WaffleIron:
				Spawn_WaffleIron(settings);
				break;
			case ParticleOrchestraType.FlyMeal:
				Spawn_FlyMeal(settings);
				break;
			case ParticleOrchestraType.GasTrap:
				Spawn_GasTrap(settings);
				break;
			case ParticleOrchestraType.ItemTransfer:
				Spawn_ItemTransfer(settings);
				break;
			case ParticleOrchestraType.ShimmerArrow:
				Spawn_ShimmerArrow(settings);
				break;
			case ParticleOrchestraType.TownSlimeTransform:
				Spawn_TownSlimeTransform(settings);
				break;
			case ParticleOrchestraType.LoadoutChange:
				Spawn_LoadOutChange(settings);
				break;
			case ParticleOrchestraType.ShimmerBlock:
				Spawn_ShimmerBlock(settings);
				break;
			case ParticleOrchestraType.Digestion:
				Spawn_Digestion(settings);
				break;
			case ParticleOrchestraType.PooFly:
				Spawn_PooFly(settings);
				break;
			case ParticleOrchestraType.ShimmerTownNPC:
				Spawn_ShimmerTownNPC(settings);
				break;
			case ParticleOrchestraType.ShimmerTownNPCSend:
				Spawn_ShimmerTownNPCSend(settings);
				break;
			}
		}
	}

	private static void Spawn_ShimmerTownNPCSend(ParticleOrchestraSettings settings)
	{
		Rectangle rect = Utils.CenteredRectangle(settings.PositionInWorld, new Vector2(30f, 60f));
		for (float num = 0f; num < 20f; num += 1f)
		{
			PrettySparkleParticle prettySparkleParticle = _poolPrettySparkle.RequestParticle();
			int num2 = Main.rand.Next(20, 40);
			prettySparkleParticle.ColorTint = Main.hslToRgb(Main.rand.NextFloat(), 1f, 0.5f, 0);
			prettySparkleParticle.LocalPosition = Main.rand.NextVector2FromRectangle(rect);
			prettySparkleParticle.Rotation = (float)Math.PI / 2f;
			prettySparkleParticle.Scale = new Vector2(1f + Main.rand.NextFloat() * 2f, 0.7f + Main.rand.NextFloat() * 0.7f);
			prettySparkleParticle.Velocity = new Vector2(0f, -1f);
			prettySparkleParticle.FadeInNormalizedTime = 5E-06f;
			prettySparkleParticle.FadeOutNormalizedTime = 0.95f;
			prettySparkleParticle.TimeToLive = num2;
			prettySparkleParticle.FadeOutEnd = num2;
			prettySparkleParticle.FadeInEnd = num2 / 2;
			prettySparkleParticle.FadeOutStart = num2 / 2;
			prettySparkleParticle.AdditiveAmount = 0.35f;
			prettySparkleParticle.DrawVerticalAxis = false;
			Main.ParticleSystem_World_OverPlayers.Add(prettySparkleParticle);
			PrettySparkleParticle prettySparkleParticle2 = _poolPrettySparkle.RequestParticle();
			prettySparkleParticle2.ColorTint = new Color(255, 255, 255, 0);
			prettySparkleParticle2.LocalPosition = Main.rand.NextVector2FromRectangle(rect);
			prettySparkleParticle2.Rotation = (float)Math.PI / 2f;
			prettySparkleParticle2.Scale = prettySparkleParticle.Scale * 0.5f;
			prettySparkleParticle2.Velocity = new Vector2(0f, -1f);
			prettySparkleParticle2.FadeInNormalizedTime = 5E-06f;
			prettySparkleParticle2.FadeOutNormalizedTime = 0.95f;
			prettySparkleParticle2.TimeToLive = num2;
			prettySparkleParticle2.FadeOutEnd = num2;
			prettySparkleParticle2.FadeInEnd = num2 / 2;
			prettySparkleParticle2.FadeOutStart = num2 / 2;
			prettySparkleParticle2.AdditiveAmount = 1f;
			prettySparkleParticle2.DrawVerticalAxis = false;
			Main.ParticleSystem_World_OverPlayers.Add(prettySparkleParticle2);
		}
	}

	private static void Spawn_ShimmerTownNPC(ParticleOrchestraSettings settings)
	{
		Rectangle num = Utils.CenteredRectangle(settings.PositionInWorld, new Vector2(30f, 60f));
		for (float asset = 0f; asset < 20f; asset += 1f)
		{
			PrettySparkleParticle rectangle = _poolPrettySparkle.RequestParticle();
			int data = Main.rand.Next(20, 40);
			rectangle.ColorTint = Main.hslToRgb(Main.rand.NextFloat(), 1f, 0.5f, 0);
			rectangle.LocalPosition = Main.rand.NextVector2FromRectangle(num);
			rectangle.Rotation = (float)Math.PI / 2f;
			rectangle.Scale = new Vector2(1f + Main.rand.NextFloat() * 2f, 0.7f + Main.rand.NextFloat() * 0.7f);
			rectangle.Velocity = new Vector2(0f, -1f);
			rectangle.FadeInNormalizedTime = 5E-06f;
			rectangle.FadeOutNormalizedTime = 0.95f;
			rectangle.TimeToLive = data;
			rectangle.FadeOutEnd = data;
			rectangle.FadeInEnd = data / 2;
			rectangle.FadeOutStart = data / 2;
			rectangle.AdditiveAmount = 0.35f;
			rectangle.DrawVerticalAxis = false;
			Main.ParticleSystem_World_OverPlayers.Add(rectangle);
			PrettySparkleParticle aCreditsRollSegmentWithActions = _poolPrettySparkle.RequestParticle();
			aCreditsRollSegmentWithActions.ColorTint = new Color(255, 255, 255, 0);
			aCreditsRollSegmentWithActions.LocalPosition = Main.rand.NextVector2FromRectangle(num);
			aCreditsRollSegmentWithActions.Rotation = (float)Math.PI / 2f;
			aCreditsRollSegmentWithActions.Scale = rectangle.Scale * 0.5f;
			aCreditsRollSegmentWithActions.Velocity = new Vector2(0f, -1f);
			aCreditsRollSegmentWithActions.FadeInNormalizedTime = 5E-06f;
			aCreditsRollSegmentWithActions.FadeOutNormalizedTime = 0.95f;
			aCreditsRollSegmentWithActions.TimeToLive = data;
			aCreditsRollSegmentWithActions.FadeOutEnd = data;
			aCreditsRollSegmentWithActions.FadeInEnd = data / 2;
			aCreditsRollSegmentWithActions.FadeOutStart = data / 2;
			aCreditsRollSegmentWithActions.AdditiveAmount = 1f;
			aCreditsRollSegmentWithActions.DrawVerticalAxis = false;
			Main.ParticleSystem_World_OverPlayers.Add(aCreditsRollSegmentWithActions);
		}
		for (int aCreditsRollSegmentWithActions2 = 0; aCreditsRollSegmentWithActions2 < 20; aCreditsRollSegmentWithActions2++)
		{
			int asset2 = Dust.NewDust(num.TopLeft(), num.Width, num.Height, 308);
			Main.dust[asset2].velocity.Y -= 8f;
			Main.dust[asset2].velocity.X *= 0.5f;
			Main.dust[asset2].scale = 0.8f;
			Main.dust[asset2].noGravity = true;
			switch (Main.rand.Next(6))
			{
			case 0:
				Main.dust[asset2].color = new Color(255, 255, 210);
				break;
			case 1:
				Main.dust[asset2].color = new Color(190, 245, 255);
				break;
			case 2:
				Main.dust[asset2].color = new Color(255, 150, 255);
				break;
			default:
				Main.dust[asset2].color = new Color(190, 175, 255);
				break;
			}
		}
		SoundEngine.PlaySound(SoundID.Item29, settings.PositionInWorld);
	}

	private static void Spawn_PooFly(ParticleOrchestraSettings settings)
	{
		int num = _poolFlies.CountParticlesInUse();
		if (num <= 50 || !(Main.rand.NextFloat() >= Utils.Remap(num, 50f, 400f, 0.5f, 0f)))
		{
			LittleFlyingCritterParticle item2 = _poolFlies.RequestParticle();
			item2.Prepare(settings.PositionInWorld, 300);
			Main.ParticleSystem_World_OverPlayers.Add(item2);
		}
	}

	private static void Spawn_Digestion(ParticleOrchestraSettings settings)
	{
		Vector2 num = settings.PositionInWorld;
		int num2 = ((settings.MovementVector.X < 0f) ? 1 : (-1));
		int num3 = Main.rand.Next(4);
		for (int asset = 0; asset < 3 + num3; asset++)
		{
			int rectangle = Dust.NewDust(num + Vector2.UnitX * -num2 * 8f - Vector2.One * 5f + Vector2.UnitY * 8f, 3, 6, 216, -num2, 1f);
			Main.dust[rectangle].velocity /= 2f;
			Main.dust[rectangle].scale = 0.8f;
		}
		if (Main.rand.Next(30) == 0)
		{
			int aCreditsRollSegmentWithActions = Gore.NewGore(num + Vector2.UnitX * -num2 * 8f, Vector2.Zero, Main.rand.Next(580, 583));
			Main.gore[aCreditsRollSegmentWithActions].velocity /= 2f;
			Main.gore[aCreditsRollSegmentWithActions].velocity.Y = Math.Abs(Main.gore[aCreditsRollSegmentWithActions].velocity.Y);
			Main.gore[aCreditsRollSegmentWithActions].velocity.X = (0f - Math.Abs(Main.gore[aCreditsRollSegmentWithActions].velocity.X)) * (float)num2;
		}
		SoundEngine.PlaySound(SoundID.Item16, settings.PositionInWorld);
	}

	private static void Spawn_ShimmerBlock(ParticleOrchestraSettings settings)
	{
		FadingParticle num = _poolFading.RequestParticle();
		num.SetBasicInfo(TextureAssets.Star[0], null, settings.MovementVector, settings.PositionInWorld);
		float num2 = 45f;
		num.SetTypeInfo(num2);
		num.AccelerationPerFrame = settings.MovementVector / num2;
		num.ColorTint = Main.hslToRgb(Main.rand.NextFloat(), 0.75f, 0.8f);
		num.ColorTint.A = 30;
		num.FadeInNormalizedTime = 0.5f;
		num.FadeOutNormalizedTime = 0.5f;
		num.Rotation = Main.rand.NextFloat() * ((float)Math.PI * 2f);
		num.Scale = Vector2.One * (0.5f + 0.5f * Main.rand.NextFloat());
		Main.ParticleSystem_World_OverPlayers.Add(num);
	}

	private static void Spawn_LoadOutChange(ParticleOrchestraSettings settings)
	{
		Player num = Main.player[settings.IndexOfPlayerWhoInvokedThis];
		if (num.active)
		{
			Rectangle num2 = num.Hitbox;
			int num3 = 6;
			num2.Height -= num3;
			if (num.gravDir == 1f)
			{
				num2.Y += num3;
			}
			for (int asset = 0; asset < 40; asset++)
			{
				Dust dust = Dust.NewDustPerfect(Main.rand.NextVector2FromRectangle(num2), 16, null, 120, default(Color), Main.rand.NextFloat() * 0.8f + 0.8f);
				dust.velocity = new Vector2(0f, (float)(-num2.Height) * Main.rand.NextFloat() * 0.04f).RotatedBy(Main.rand.NextFloatDirection() * ((float)Math.PI * 2f) * 0.1f);
				dust.velocity += num.velocity * 2f * Main.rand.NextFloat();
				dust.noGravity = true;
				dust.noLight = (dust.noLightEmittence = true);
			}
			for (int aCreditsRollSegmentWithActions3 = 0; aCreditsRollSegmentWithActions3 < 5; aCreditsRollSegmentWithActions3++)
			{
				Dust dust2 = Dust.NewDustPerfect(Main.rand.NextVector2FromRectangle(num2), 43, null, 254, Main.hslToRgb(Main.rand.NextFloat(), 0.3f, 0.8f), Main.rand.NextFloat() * 0.8f + 0.8f);
				dust2.velocity = new Vector2(0f, (float)(-num2.Height) * Main.rand.NextFloat() * 0.04f).RotatedBy(Main.rand.NextFloatDirection() * ((float)Math.PI * 2f) * 0.1f);
				dust2.velocity += num.velocity * 2f * Main.rand.NextFloat();
				dust2.noGravity = true;
				dust2.noLight = (dust2.noLightEmittence = true);
			}
		}
	}

	private static void Spawn_TownSlimeTransform(ParticleOrchestraSettings settings)
	{
		switch (settings.UniqueInfoPiece)
		{
		case 0:
			NerdySlimeEffect(settings);
			break;
		case 1:
			CopperSlimeEffect(settings);
			break;
		case 2:
			ElderSlimeEffect(settings);
			break;
		}
	}

	private static void ElderSlimeEffect(ParticleOrchestraSettings settings)
	{
		for (int num = 0; num < 30; num++)
		{
			Dust num2 = Dust.NewDustPerfect(settings.PositionInWorld + Main.rand.NextVector2Circular(20f, 20f), 43, (settings.MovementVector * 0.75f + Main.rand.NextVector2Circular(6f, 6f)) * Main.rand.NextFloat(), 26, Color.Lerp(Main.OurFavoriteColor, Color.White, Main.rand.NextFloat()), 1f + Main.rand.NextFloat() * 1.4f);
			num2.fadeIn = 1.5f;
			if (num2.velocity.Y > 0f && Main.rand.Next(2) == 0)
			{
				num2.velocity.Y *= -1f;
			}
			num2.noGravity = true;
		}
		for (int num3 = 0; num3 < 8; num3++)
		{
			Gore.NewGoreDirect(settings.PositionInWorld + Utils.RandomVector2(Main.rand, -30f, 30f) * new Vector2(0.5f, 1f), Vector2.Zero, 61 + Main.rand.Next(3)).velocity *= 0.5f;
		}
	}

	private static void NerdySlimeEffect(ParticleOrchestraSettings settings)
	{
		Color num = new Color(0, 80, 255, 100);
		for (int num2 = 0; num2 < 60; num2++)
		{
			Dust.NewDustPerfect(settings.PositionInWorld, 4, (settings.MovementVector * 0.75f + Main.rand.NextVector2Circular(6f, 6f)) * Main.rand.NextFloat(), 175, num, 0.6f + Main.rand.NextFloat() * 1.4f);
		}
	}

	private static void CopperSlimeEffect(ParticleOrchestraSettings settings)
	{
		for (int num = 0; num < 40; num++)
		{
			Dust num2 = Dust.NewDustPerfect(settings.PositionInWorld + Main.rand.NextVector2Circular(20f, 20f), 43, (settings.MovementVector * 0.75f + Main.rand.NextVector2Circular(6f, 6f)) * Main.rand.NextFloat(), 26, Color.Lerp(new Color(183, 88, 25), Color.White, Main.rand.NextFloat() * 0.5f), 1f + Main.rand.NextFloat() * 1.4f);
			num2.fadeIn = 1.5f;
			if (num2.velocity.Y > 0f && Main.rand.Next(2) == 0)
			{
				num2.velocity.Y *= -1f;
			}
			num2.noGravity = true;
		}
	}

	private static void Spawn_ShimmerArrow(ParticleOrchestraSettings settings)
	{
		float num = 20f;
		for (int asset = 0; asset < 2; asset++)
		{
			float rectangle = (float)Math.PI * 2f * Main.rand.NextFloatDirection() * 0.05f;
			Color data = Main.hslToRgb(Main.rand.NextFloat(), 1f, 0.5f);
			data.A /= 2;
			Color aCreditsRollSegmentWithActions = data;
			aCreditsRollSegmentWithActions.A = byte.MaxValue;
			aCreditsRollSegmentWithActions = Color.Lerp(aCreditsRollSegmentWithActions, Color.White, 0.5f);
			for (float aCreditsRollSegmentWithActions2 = 0f; aCreditsRollSegmentWithActions2 < 4f; aCreditsRollSegmentWithActions2 += 1f)
			{
				PrettySparkleParticle num2 = _poolPrettySparkle.RequestParticle();
				Vector2 aCreditsRollSegmentWithActions3 = ((float)Math.PI / 2f * aCreditsRollSegmentWithActions2 + rectangle).ToRotationVector2() * 4f;
				num2.ColorTint = data;
				num2.LocalPosition = settings.PositionInWorld;
				num2.Rotation = aCreditsRollSegmentWithActions3.ToRotation();
				num2.Scale = new Vector2((aCreditsRollSegmentWithActions2 % 2f == 0f) ? 2f : 4f, 0.5f) * 1.1f;
				num2.FadeInNormalizedTime = 5E-06f;
				num2.FadeOutNormalizedTime = 0.95f;
				num2.TimeToLive = num;
				num2.FadeOutEnd = num;
				num2.FadeInEnd = num / 2f;
				num2.FadeOutStart = num / 2f;
				num2.AdditiveAmount = 0.35f;
				num2.Velocity = -aCreditsRollSegmentWithActions3 * 0.2f;
				num2.DrawVerticalAxis = false;
				if (aCreditsRollSegmentWithActions2 % 2f == 1f)
				{
					num2.Scale *= 0.9f;
					num2.Velocity *= 0.9f;
				}
				Main.ParticleSystem_World_OverPlayers.Add(num2);
			}
			for (float num3 = 0f; num3 < 4f; num3 += 1f)
			{
				PrettySparkleParticle item = _poolPrettySparkle.RequestParticle();
				Vector2 item2 = ((float)Math.PI / 2f * num3 + rectangle).ToRotationVector2() * 4f;
				item.ColorTint = aCreditsRollSegmentWithActions;
				item.LocalPosition = settings.PositionInWorld;
				item.Rotation = item2.ToRotation();
				item.Scale = new Vector2((num3 % 2f == 0f) ? 2f : 4f, 0.5f) * 0.7f;
				item.FadeInNormalizedTime = 5E-06f;
				item.FadeOutNormalizedTime = 0.95f;
				item.TimeToLive = num;
				item.FadeOutEnd = num;
				item.FadeInEnd = num / 2f;
				item.FadeOutStart = num / 2f;
				item.Velocity = item2 * 0.2f;
				item.DrawVerticalAxis = false;
				if (num3 % 2f == 1f)
				{
					item.Scale *= 1.2f;
					item.Velocity *= 1.2f;
				}
				Main.ParticleSystem_World_OverPlayers.Add(item);
				if (asset == 0)
				{
					for (int result = 0; result < 1; result++)
					{
						Dust dust = Dust.NewDustPerfect(settings.PositionInWorld, 306, item2.RotatedBy(Main.rand.NextFloatDirection() * ((float)Math.PI * 2f) * 0.025f) * Main.rand.NextFloat());
						dust.noGravity = true;
						dust.scale = 1.4f;
						dust.fadeIn = 1.2f;
						dust.color = data;
						Dust dust2 = Dust.NewDustPerfect(settings.PositionInWorld, 306, -item2.RotatedBy(Main.rand.NextFloatDirection() * ((float)Math.PI * 2f) * 0.025f) * Main.rand.NextFloat());
						dust2.noGravity = true;
						dust2.scale = 1.4f;
						dust2.fadeIn = 1.2f;
						dust2.color = data;
					}
				}
			}
		}
	}

	private static void Spawn_ItemTransfer(ParticleOrchestraSettings settings)
	{
		Vector2 vector = settings.PositionInWorld + settings.MovementVector;
		Vector2 num = Main.rand.NextVector2Circular(32f, 32f);
		Vector2 num2 = settings.PositionInWorld + num;
		Vector2 asset = vector - num2;
		int rectangle = settings.UniqueInfoPiece;
		if (ContentSamples.ItemsByType.TryGetValue(rectangle, out var data) && !data.IsAir)
		{
			rectangle = data.type;
			int aCreditsRollSegmentWithActions = Main.rand.Next(60, 80);
			Chest.AskForChestToEatItem(num2 + asset + new Vector2(-8f, -8f), aCreditsRollSegmentWithActions + 10);
			ItemTransferParticle aCreditsRollSegmentWithActions2 = _poolItemTransfer.RequestParticle();
			aCreditsRollSegmentWithActions2.Prepare(rectangle, aCreditsRollSegmentWithActions, num2, num2 + asset);
			Main.ParticleSystem_World_OverPlayers.Add(aCreditsRollSegmentWithActions2);
		}
	}

	private static void Spawn_PetExchange(ParticleOrchestraSettings settings)
	{
		Vector2 positionInWorld = settings.PositionInWorld;
		for (int i = 0; i < 13; i++)
		{
			Gore gore = Gore.NewGoreDirect(positionInWorld + new Vector2(-20f, -20f) + Main.rand.NextVector2Circular(20f, 20f), Vector2.Zero, Main.rand.Next(61, 64), 1f + Main.rand.NextFloat() * 0.3f);
			gore.alpha = 100;
			gore.velocity = ((float)Math.PI * 2f * (float)Main.rand.Next()).ToRotationVector2() * Main.rand.NextFloat() + settings.MovementVector * 0.5f;
		}
	}

	private static void Spawn_TerraBlade(ParticleOrchestraSettings settings)
	{
		float num = 30f;
		float num2 = settings.MovementVector.ToRotation() + (float)Math.PI / 2f;
		float x = 3f;
		for (float num3 = 0f; num3 < 4f; num3 += 1f)
		{
			PrettySparkleParticle prettySparkleParticle = _poolPrettySparkle.RequestParticle();
			Vector2 vector = ((float)Math.PI / 2f * num3 + num2).ToRotationVector2() * 4f;
			prettySparkleParticle.ColorTint = new Color(0.2f, 0.85f, 0.4f, 0.5f);
			prettySparkleParticle.LocalPosition = settings.PositionInWorld;
			prettySparkleParticle.Rotation = vector.ToRotation();
			prettySparkleParticle.Scale = new Vector2(x, 0.5f) * 1.1f;
			prettySparkleParticle.FadeInNormalizedTime = 5E-06f;
			prettySparkleParticle.FadeOutNormalizedTime = 0.95f;
			prettySparkleParticle.TimeToLive = num;
			prettySparkleParticle.FadeOutEnd = num;
			prettySparkleParticle.FadeInEnd = num / 2f;
			prettySparkleParticle.FadeOutStart = num / 2f;
			prettySparkleParticle.AdditiveAmount = 0.35f;
			prettySparkleParticle.Velocity = -vector * 0.2f;
			prettySparkleParticle.DrawVerticalAxis = false;
			if (num3 % 2f == 1f)
			{
				prettySparkleParticle.Scale *= 1.5f;
				prettySparkleParticle.Velocity *= 2f;
			}
			Main.ParticleSystem_World_OverPlayers.Add(prettySparkleParticle);
		}
		for (float num4 = -1f; num4 <= 1f; num4 += 2f)
		{
			PrettySparkleParticle prettySparkleParticle2 = _poolPrettySparkle.RequestParticle();
			_ = num2.ToRotationVector2() * 4f;
			Vector2 vector2 = ((float)Math.PI / 2f * num4 + num2).ToRotationVector2() * 2f;
			prettySparkleParticle2.ColorTint = new Color(0.4f, 1f, 0.4f, 0.5f);
			prettySparkleParticle2.LocalPosition = settings.PositionInWorld;
			prettySparkleParticle2.Rotation = vector2.ToRotation();
			prettySparkleParticle2.Scale = new Vector2(x, 0.5f) * 1.1f;
			prettySparkleParticle2.FadeInNormalizedTime = 5E-06f;
			prettySparkleParticle2.FadeOutNormalizedTime = 0.95f;
			prettySparkleParticle2.TimeToLive = num;
			prettySparkleParticle2.FadeOutEnd = num;
			prettySparkleParticle2.FadeInEnd = num / 2f;
			prettySparkleParticle2.FadeOutStart = num / 2f;
			prettySparkleParticle2.AdditiveAmount = 0.35f;
			prettySparkleParticle2.Velocity = vector2.RotatedBy(1.5707963705062866) * 0.5f;
			prettySparkleParticle2.DrawVerticalAxis = false;
			Main.ParticleSystem_World_OverPlayers.Add(prettySparkleParticle2);
		}
		for (float num5 = 0f; num5 < 4f; num5 += 1f)
		{
			PrettySparkleParticle prettySparkleParticle3 = _poolPrettySparkle.RequestParticle();
			Vector2 vector3 = ((float)Math.PI / 2f * num5 + num2).ToRotationVector2() * 4f;
			prettySparkleParticle3.ColorTint = new Color(0.2f, 1f, 0.2f, 1f);
			prettySparkleParticle3.LocalPosition = settings.PositionInWorld;
			prettySparkleParticle3.Rotation = vector3.ToRotation();
			prettySparkleParticle3.Scale = new Vector2(x, 0.5f) * 0.7f;
			prettySparkleParticle3.FadeInNormalizedTime = 5E-06f;
			prettySparkleParticle3.FadeOutNormalizedTime = 0.95f;
			prettySparkleParticle3.TimeToLive = num;
			prettySparkleParticle3.FadeOutEnd = num;
			prettySparkleParticle3.FadeInEnd = num / 2f;
			prettySparkleParticle3.FadeOutStart = num / 2f;
			prettySparkleParticle3.Velocity = vector3 * 0.2f;
			prettySparkleParticle3.DrawVerticalAxis = false;
			if (num5 % 2f == 1f)
			{
				prettySparkleParticle3.Scale *= 1.5f;
				prettySparkleParticle3.Velocity *= 2f;
			}
			Main.ParticleSystem_World_OverPlayers.Add(prettySparkleParticle3);
			for (int i = 0; i < 1; i++)
			{
				Dust dust = Dust.NewDustPerfect(settings.PositionInWorld, 107, vector3.RotatedBy(Main.rand.NextFloatDirection() * ((float)Math.PI * 2f) * 0.025f) * Main.rand.NextFloat());
				dust.noGravity = true;
				dust.scale = 0.8f;
				Dust dust2 = Dust.NewDustPerfect(settings.PositionInWorld, 107, -vector3.RotatedBy(Main.rand.NextFloatDirection() * ((float)Math.PI * 2f) * 0.025f) * Main.rand.NextFloat());
				dust2.noGravity = true;
				dust2.scale = 1.4f;
			}
		}
	}

	private static void Spawn_Excalibur(ParticleOrchestraSettings settings)
	{
		float num = 30f;
		float num2 = 0f;
		for (float num3 = 0f; num3 < 4f; num3 += 1f)
		{
			PrettySparkleParticle prettySparkleParticle = _poolPrettySparkle.RequestParticle();
			Vector2 vector = ((float)Math.PI / 2f * num3 + num2).ToRotationVector2() * 4f;
			prettySparkleParticle.ColorTint = new Color(0.9f, 0.85f, 0.4f, 0.5f);
			prettySparkleParticle.LocalPosition = settings.PositionInWorld;
			prettySparkleParticle.Rotation = vector.ToRotation();
			prettySparkleParticle.Scale = new Vector2((num3 % 2f == 0f) ? 2f : 4f, 0.5f) * 1.1f;
			prettySparkleParticle.FadeInNormalizedTime = 5E-06f;
			prettySparkleParticle.FadeOutNormalizedTime = 0.95f;
			prettySparkleParticle.TimeToLive = num;
			prettySparkleParticle.FadeOutEnd = num;
			prettySparkleParticle.FadeInEnd = num / 2f;
			prettySparkleParticle.FadeOutStart = num / 2f;
			prettySparkleParticle.AdditiveAmount = 0.35f;
			prettySparkleParticle.Velocity = -vector * 0.2f;
			prettySparkleParticle.DrawVerticalAxis = false;
			if (num3 % 2f == 1f)
			{
				prettySparkleParticle.Scale *= 1.5f;
				prettySparkleParticle.Velocity *= 1.5f;
			}
			Main.ParticleSystem_World_OverPlayers.Add(prettySparkleParticle);
		}
		for (float num4 = 0f; num4 < 4f; num4 += 1f)
		{
			PrettySparkleParticle prettySparkleParticle2 = _poolPrettySparkle.RequestParticle();
			Vector2 vector2 = ((float)Math.PI / 2f * num4 + num2).ToRotationVector2() * 4f;
			prettySparkleParticle2.ColorTint = new Color(1f, 1f, 0.2f, 1f);
			prettySparkleParticle2.LocalPosition = settings.PositionInWorld;
			prettySparkleParticle2.Rotation = vector2.ToRotation();
			prettySparkleParticle2.Scale = new Vector2((num4 % 2f == 0f) ? 2f : 4f, 0.5f) * 0.7f;
			prettySparkleParticle2.FadeInNormalizedTime = 5E-06f;
			prettySparkleParticle2.FadeOutNormalizedTime = 0.95f;
			prettySparkleParticle2.TimeToLive = num;
			prettySparkleParticle2.FadeOutEnd = num;
			prettySparkleParticle2.FadeInEnd = num / 2f;
			prettySparkleParticle2.FadeOutStart = num / 2f;
			prettySparkleParticle2.Velocity = vector2 * 0.2f;
			prettySparkleParticle2.DrawVerticalAxis = false;
			if (num4 % 2f == 1f)
			{
				prettySparkleParticle2.Scale *= 1.5f;
				prettySparkleParticle2.Velocity *= 1.5f;
			}
			Main.ParticleSystem_World_OverPlayers.Add(prettySparkleParticle2);
			for (int i = 0; i < 1; i++)
			{
				Dust dust = Dust.NewDustPerfect(settings.PositionInWorld, 169, vector2.RotatedBy(Main.rand.NextFloatDirection() * ((float)Math.PI * 2f) * 0.025f) * Main.rand.NextFloat());
				dust.noGravity = true;
				dust.scale = 1.4f;
				Dust dust2 = Dust.NewDustPerfect(settings.PositionInWorld, 169, -vector2.RotatedBy(Main.rand.NextFloatDirection() * ((float)Math.PI * 2f) * 0.025f) * Main.rand.NextFloat());
				dust2.noGravity = true;
				dust2.scale = 1.4f;
			}
		}
	}

	private static void Spawn_SlapHand(ParticleOrchestraSettings settings)
	{
		SoundEngine.PlaySound(SoundID.Item175, settings.PositionInWorld);
	}

	private static void Spawn_WaffleIron(ParticleOrchestraSettings settings)
	{
		SoundEngine.PlaySound(SoundID.Item178, settings.PositionInWorld);
	}

	private static void Spawn_FlyMeal(ParticleOrchestraSettings settings)
	{
		SoundEngine.PlaySound(SoundID.Item16, settings.PositionInWorld);
	}

	private static void Spawn_GasTrap(ParticleOrchestraSettings settings)
	{
		SoundEngine.PlaySound(SoundID.Item16, settings.PositionInWorld);
		Vector2 movementVector = settings.MovementVector;
		int num = 12;
		int num2 = 10;
		float num3 = 5f;
		float num4 = 2.5f;
		Color lightColorTint = new Color(0.2f, 0.4f, 0.15f);
		Vector2 positionInWorld = settings.PositionInWorld;
		float num5 = (float)Math.PI / 20f;
		float num6 = (float)Math.PI / 15f;
		for (int i = 0; i < num; i++)
		{
			Vector2 spinninpoint = movementVector + new Vector2(num3 + Main.rand.NextFloat() * 1f, 0f).RotatedBy((float)i / (float)num * ((float)Math.PI * 2f), Vector2.Zero);
			spinninpoint = spinninpoint.RotatedByRandom(num5);
			GasParticle gasParticle = _poolGas.RequestParticle();
			gasParticle.AccelerationPerFrame = Vector2.Zero;
			gasParticle.Velocity = spinninpoint;
			gasParticle.ColorTint = Color.White;
			gasParticle.LightColorTint = lightColorTint;
			gasParticle.LocalPosition = positionInWorld + spinninpoint;
			gasParticle.TimeToLive = 50 + Main.rand.Next(20);
			gasParticle.InitialScale = 1f + Main.rand.NextFloat() * 0.35f;
			Main.ParticleSystem_World_BehindPlayers.Add(gasParticle);
		}
		for (int j = 0; j < num2; j++)
		{
			Vector2 spinninpoint2 = new Vector2(num4 + Main.rand.NextFloat() * 1.45f, 0f).RotatedBy((float)j / (float)num2 * ((float)Math.PI * 2f), Vector2.Zero);
			spinninpoint2 = spinninpoint2.RotatedByRandom(num6);
			if (j % 2 == 0)
			{
				spinninpoint2 *= 0.5f;
			}
			GasParticle gasParticle2 = _poolGas.RequestParticle();
			gasParticle2.AccelerationPerFrame = Vector2.Zero;
			gasParticle2.Velocity = spinninpoint2;
			gasParticle2.ColorTint = Color.White;
			gasParticle2.LightColorTint = lightColorTint;
			gasParticle2.LocalPosition = positionInWorld;
			gasParticle2.TimeToLive = 80 + Main.rand.Next(30);
			gasParticle2.InitialScale = 1f + Main.rand.NextFloat() * 0.5f;
			Main.ParticleSystem_World_BehindPlayers.Add(gasParticle2);
		}
	}

	private static void Spawn_TrueExcalibur(ParticleOrchestraSettings settings)
	{
		float num = 36f;
		float num2 = (float)Math.PI / 4f;
		for (float num3 = 0f; num3 < 2f; num3 += 1f)
		{
			PrettySparkleParticle prettySparkleParticle = _poolPrettySparkle.RequestParticle();
			Vector2 v = ((float)Math.PI / 2f * num3 + num2).ToRotationVector2() * 4f;
			prettySparkleParticle.ColorTint = new Color(1f, 0f, 0.3f, 1f);
			prettySparkleParticle.LocalPosition = settings.PositionInWorld;
			prettySparkleParticle.Rotation = v.ToRotation();
			prettySparkleParticle.Scale = new Vector2(5f, 0.5f) * 1.1f;
			prettySparkleParticle.FadeInNormalizedTime = 5E-06f;
			prettySparkleParticle.FadeOutNormalizedTime = 0.95f;
			prettySparkleParticle.TimeToLive = num;
			prettySparkleParticle.FadeOutEnd = num;
			prettySparkleParticle.FadeInEnd = num / 2f;
			prettySparkleParticle.FadeOutStart = num / 2f;
			prettySparkleParticle.AdditiveAmount = 0.35f;
			prettySparkleParticle.DrawVerticalAxis = false;
			Main.ParticleSystem_World_OverPlayers.Add(prettySparkleParticle);
		}
		for (float num4 = 0f; num4 < 2f; num4 += 1f)
		{
			PrettySparkleParticle prettySparkleParticle2 = _poolPrettySparkle.RequestParticle();
			Vector2 vector = ((float)Math.PI / 2f * num4 + num2).ToRotationVector2() * 4f;
			prettySparkleParticle2.ColorTint = new Color(1f, 0.5f, 0.8f, 1f);
			prettySparkleParticle2.LocalPosition = settings.PositionInWorld;
			prettySparkleParticle2.Rotation = vector.ToRotation();
			prettySparkleParticle2.Scale = new Vector2(3f, 0.5f) * 0.7f;
			prettySparkleParticle2.FadeInNormalizedTime = 5E-06f;
			prettySparkleParticle2.FadeOutNormalizedTime = 0.95f;
			prettySparkleParticle2.TimeToLive = num;
			prettySparkleParticle2.FadeOutEnd = num;
			prettySparkleParticle2.FadeInEnd = num / 2f;
			prettySparkleParticle2.FadeOutStart = num / 2f;
			prettySparkleParticle2.DrawVerticalAxis = false;
			Main.ParticleSystem_World_OverPlayers.Add(prettySparkleParticle2);
			for (int i = 0; i < 1; i++)
			{
				if (Main.rand.Next(2) != 0)
				{
					Dust dust = Dust.NewDustPerfect(settings.PositionInWorld, 242, vector.RotatedBy(Main.rand.NextFloatDirection() * ((float)Math.PI * 2f) * 0.025f) * Main.rand.NextFloat());
					dust.noGravity = true;
					dust.scale = 1.4f;
					Dust dust2 = Dust.NewDustPerfect(settings.PositionInWorld, 242, -vector.RotatedBy(Main.rand.NextFloatDirection() * ((float)Math.PI * 2f) * 0.025f) * Main.rand.NextFloat());
					dust2.noGravity = true;
					dust2.scale = 1.4f;
				}
			}
		}
		num = 30f;
		num2 = 0f;
		for (float num5 = 0f; num5 < 4f; num5 += 1f)
		{
			PrettySparkleParticle prettySparkleParticle3 = _poolPrettySparkle.RequestParticle();
			Vector2 vector2 = ((float)Math.PI / 2f * num5 + num2).ToRotationVector2() * 4f;
			prettySparkleParticle3.ColorTint = new Color(0.9f, 0.85f, 0.4f, 0.5f);
			prettySparkleParticle3.LocalPosition = settings.PositionInWorld;
			prettySparkleParticle3.Rotation = vector2.ToRotation();
			prettySparkleParticle3.Scale = new Vector2((num5 % 2f == 0f) ? 2f : 4f, 0.5f) * 1.1f;
			prettySparkleParticle3.FadeInNormalizedTime = 5E-06f;
			prettySparkleParticle3.FadeOutNormalizedTime = 0.95f;
			prettySparkleParticle3.TimeToLive = num;
			prettySparkleParticle3.FadeOutEnd = num;
			prettySparkleParticle3.FadeInEnd = num / 2f;
			prettySparkleParticle3.FadeOutStart = num / 2f;
			prettySparkleParticle3.AdditiveAmount = 0.35f;
			prettySparkleParticle3.Velocity = -vector2 * 0.2f;
			prettySparkleParticle3.DrawVerticalAxis = false;
			if (num5 % 2f == 1f)
			{
				prettySparkleParticle3.Scale *= 1.5f;
				prettySparkleParticle3.Velocity *= 1.5f;
			}
			Main.ParticleSystem_World_OverPlayers.Add(prettySparkleParticle3);
		}
		for (float num6 = 0f; num6 < 4f; num6 += 1f)
		{
			PrettySparkleParticle prettySparkleParticle4 = _poolPrettySparkle.RequestParticle();
			Vector2 vector3 = ((float)Math.PI / 2f * num6 + num2).ToRotationVector2() * 4f;
			prettySparkleParticle4.ColorTint = new Color(1f, 1f, 0.2f, 1f);
			prettySparkleParticle4.LocalPosition = settings.PositionInWorld;
			prettySparkleParticle4.Rotation = vector3.ToRotation();
			prettySparkleParticle4.Scale = new Vector2((num6 % 2f == 0f) ? 2f : 4f, 0.5f) * 0.7f;
			prettySparkleParticle4.FadeInNormalizedTime = 5E-06f;
			prettySparkleParticle4.FadeOutNormalizedTime = 0.95f;
			prettySparkleParticle4.TimeToLive = num;
			prettySparkleParticle4.FadeOutEnd = num;
			prettySparkleParticle4.FadeInEnd = num / 2f;
			prettySparkleParticle4.FadeOutStart = num / 2f;
			prettySparkleParticle4.Velocity = vector3 * 0.2f;
			prettySparkleParticle4.DrawVerticalAxis = false;
			if (num6 % 2f == 1f)
			{
				prettySparkleParticle4.Scale *= 1.5f;
				prettySparkleParticle4.Velocity *= 1.5f;
			}
			Main.ParticleSystem_World_OverPlayers.Add(prettySparkleParticle4);
			for (int j = 0; j < 1; j++)
			{
				if (Main.rand.Next(2) != 0)
				{
					Dust dust3 = Dust.NewDustPerfect(settings.PositionInWorld, 169, vector3.RotatedBy(Main.rand.NextFloatDirection() * ((float)Math.PI * 2f) * 0.025f) * Main.rand.NextFloat());
					dust3.noGravity = true;
					dust3.scale = 1.4f;
					Dust dust4 = Dust.NewDustPerfect(settings.PositionInWorld, 169, -vector3.RotatedBy(Main.rand.NextFloatDirection() * ((float)Math.PI * 2f) * 0.025f) * Main.rand.NextFloat());
					dust4.noGravity = true;
					dust4.scale = 1.4f;
				}
			}
		}
	}

	private static void Spawn_AshTreeShake(ParticleOrchestraSettings settings)
	{
		float num = 10f + 20f * Main.rand.NextFloat();
		float num2 = -(float)Math.PI / 4f;
		float num3 = 0.2f + 0.4f * Main.rand.NextFloat();
		Color colorTint = Main.hslToRgb(Main.rand.NextFloat() * 0.1f + 0.06f, 1f, 0.5f);
		colorTint.A /= 2;
		colorTint *= Main.rand.NextFloat() * 0.3f + 0.7f;
		for (float num4 = 0f; num4 < 2f; num4 += 1f)
		{
			PrettySparkleParticle prettySparkleParticle = _poolPrettySparkle.RequestParticle();
			Vector2 vector = ((float)Math.PI / 4f + (float)Math.PI * num4 + num2).ToRotationVector2() * 4f;
			prettySparkleParticle.ColorTint = colorTint;
			prettySparkleParticle.LocalPosition = settings.PositionInWorld;
			prettySparkleParticle.Rotation = vector.ToRotation();
			prettySparkleParticle.Scale = new Vector2(4f, 1f) * 1.1f * num3;
			prettySparkleParticle.FadeInNormalizedTime = 5E-06f;
			prettySparkleParticle.FadeOutNormalizedTime = 0.95f;
			prettySparkleParticle.TimeToLive = num;
			prettySparkleParticle.FadeOutEnd = num;
			prettySparkleParticle.FadeInEnd = num / 2f;
			prettySparkleParticle.FadeOutStart = num / 2f;
			prettySparkleParticle.AdditiveAmount = 0.35f;
			prettySparkleParticle.LocalPosition -= vector * num * 0.25f;
			prettySparkleParticle.Velocity = vector * 0.05f;
			prettySparkleParticle.DrawVerticalAxis = false;
			if (num4 == 1f)
			{
				prettySparkleParticle.Scale *= 1.5f;
				prettySparkleParticle.Velocity *= 1.5f;
				prettySparkleParticle.LocalPosition -= prettySparkleParticle.Velocity * 4f;
			}
			Main.ParticleSystem_World_OverPlayers.Add(prettySparkleParticle);
		}
		for (float num5 = 0f; num5 < 2f; num5 += 1f)
		{
			PrettySparkleParticle prettySparkleParticle2 = _poolPrettySparkle.RequestParticle();
			Vector2 vector2 = ((float)Math.PI / 4f + (float)Math.PI * num5 + num2).ToRotationVector2() * 4f;
			prettySparkleParticle2.ColorTint = new Color(1f, 0.4f, 0.2f, 1f);
			prettySparkleParticle2.LocalPosition = settings.PositionInWorld;
			prettySparkleParticle2.Rotation = vector2.ToRotation();
			prettySparkleParticle2.Scale = new Vector2(4f, 1f) * 0.7f * num3;
			prettySparkleParticle2.FadeInNormalizedTime = 5E-06f;
			prettySparkleParticle2.FadeOutNormalizedTime = 0.95f;
			prettySparkleParticle2.TimeToLive = num;
			prettySparkleParticle2.FadeOutEnd = num;
			prettySparkleParticle2.FadeInEnd = num / 2f;
			prettySparkleParticle2.FadeOutStart = num / 2f;
			prettySparkleParticle2.LocalPosition -= vector2 * num * 0.25f;
			prettySparkleParticle2.Velocity = vector2 * 0.05f;
			prettySparkleParticle2.DrawVerticalAxis = false;
			if (num5 == 1f)
			{
				prettySparkleParticle2.Scale *= 1.5f;
				prettySparkleParticle2.Velocity *= 1.5f;
				prettySparkleParticle2.LocalPosition -= prettySparkleParticle2.Velocity * 4f;
			}
			Main.ParticleSystem_World_OverPlayers.Add(prettySparkleParticle2);
			for (int i = 0; i < 1; i++)
			{
				Dust dust = Dust.NewDustPerfect(settings.PositionInWorld, 6, vector2.RotatedBy(Main.rand.NextFloatDirection() * ((float)Math.PI * 2f) * 0.025f) * Main.rand.NextFloat());
				dust.noGravity = true;
				dust.scale = 1.4f;
				Dust dust2 = Dust.NewDustPerfect(settings.PositionInWorld, 6, -vector2.RotatedBy(Main.rand.NextFloatDirection() * ((float)Math.PI * 2f) * 0.025f) * Main.rand.NextFloat());
				dust2.noGravity = true;
				dust2.scale = 1.4f;
			}
		}
	}

	private static void Spawn_LeafCrystalPassive(ParticleOrchestraSettings settings)
	{
		float num = 90f;
		float vector = (float)Math.PI * 2f * Main.rand.NextFloat();
		float num2 = 3f;
		for (float num3 = 0f; num3 < num2; num3 += 1f)
		{
			PrettySparkleParticle prettySparkleParticle = _poolPrettySparkle.RequestParticle();
			Vector2 v = ((float)Math.PI * 2f / num2 * num3 + vector).ToRotationVector2() * 4f;
			prettySparkleParticle.ColorTint = new Color(0.3f, 0.6f, 0.3f, 0.5f);
			prettySparkleParticle.LocalPosition = settings.PositionInWorld;
			prettySparkleParticle.Rotation = v.ToRotation();
			prettySparkleParticle.Scale = new Vector2(4f, 1f) * 0.4f;
			prettySparkleParticle.FadeInNormalizedTime = 5E-06f;
			prettySparkleParticle.FadeOutNormalizedTime = 0.95f;
			prettySparkleParticle.TimeToLive = num;
			prettySparkleParticle.FadeOutEnd = num;
			prettySparkleParticle.FadeInEnd = 10f;
			prettySparkleParticle.FadeOutStart = 10f;
			prettySparkleParticle.AdditiveAmount = 0.5f;
			prettySparkleParticle.Velocity = Vector2.Zero;
			prettySparkleParticle.DrawVerticalAxis = false;
			Main.ParticleSystem_World_OverPlayers.Add(prettySparkleParticle);
		}
	}

	private static void Spawn_LeafCrystalShot(ParticleOrchestraSettings settings)
	{
		int num = 30;
		PrettySparkleParticle prettySparkleParticle = _poolPrettySparkle.RequestParticle();
		Vector2 movementVector = settings.MovementVector;
		Color value = Main.hslToRgb((float)settings.UniqueInfoPiece / 255f, 1f, 0.5f);
		value = (prettySparkleParticle.ColorTint = Color.Lerp(value, Color.Gold, (float)(int)value.R / 255f * 0.5f));
		prettySparkleParticle.LocalPosition = settings.PositionInWorld;
		prettySparkleParticle.Rotation = movementVector.ToRotation();
		prettySparkleParticle.Scale = new Vector2(4f, 1f) * 1f;
		prettySparkleParticle.FadeInNormalizedTime = 5E-06f;
		prettySparkleParticle.FadeOutNormalizedTime = 1f;
		prettySparkleParticle.TimeToLive = num;
		prettySparkleParticle.FadeOutEnd = num;
		prettySparkleParticle.FadeInEnd = num / 2;
		prettySparkleParticle.FadeOutStart = num / 2;
		prettySparkleParticle.AdditiveAmount = 0.5f;
		prettySparkleParticle.Velocity = settings.MovementVector;
		prettySparkleParticle.LocalPosition -= prettySparkleParticle.Velocity * 4f;
		prettySparkleParticle.DrawVerticalAxis = false;
		Main.ParticleSystem_World_OverPlayers.Add(prettySparkleParticle);
	}

	private static void Spawn_TrueNightsEdge(ParticleOrchestraSettings settings)
	{
		float num = 30f;
		float num2 = 0f;
		for (float num3 = 0f; num3 < 3f; num3 += 2f)
		{
			PrettySparkleParticle prettySparkleParticle = _poolPrettySparkle.RequestParticle();
			Vector2 vector = ((float)Math.PI / 4f + (float)Math.PI / 4f * num3 + num2).ToRotationVector2() * 4f;
			prettySparkleParticle.ColorTint = new Color(0.3f, 0.6f, 0.3f, 0.5f);
			prettySparkleParticle.LocalPosition = settings.PositionInWorld;
			prettySparkleParticle.Rotation = vector.ToRotation();
			prettySparkleParticle.Scale = new Vector2(4f, 1f) * 1.1f;
			prettySparkleParticle.FadeInNormalizedTime = 5E-06f;
			prettySparkleParticle.FadeOutNormalizedTime = 0.95f;
			prettySparkleParticle.TimeToLive = num;
			prettySparkleParticle.FadeOutEnd = num;
			prettySparkleParticle.FadeInEnd = num / 2f;
			prettySparkleParticle.FadeOutStart = num / 2f;
			prettySparkleParticle.AdditiveAmount = 0.35f;
			prettySparkleParticle.LocalPosition -= vector * num * 0.25f;
			prettySparkleParticle.Velocity = vector;
			prettySparkleParticle.DrawVerticalAxis = false;
			if (num3 == 1f)
			{
				prettySparkleParticle.Scale *= 1.5f;
				prettySparkleParticle.Velocity *= 1.5f;
				prettySparkleParticle.LocalPosition -= prettySparkleParticle.Velocity * 4f;
			}
			Main.ParticleSystem_World_OverPlayers.Add(prettySparkleParticle);
		}
		for (float num4 = 0f; num4 < 3f; num4 += 2f)
		{
			PrettySparkleParticle prettySparkleParticle2 = _poolPrettySparkle.RequestParticle();
			Vector2 vector2 = ((float)Math.PI / 4f + (float)Math.PI / 4f * num4 + num2).ToRotationVector2() * 4f;
			prettySparkleParticle2.ColorTint = new Color(0.6f, 1f, 0.2f, 1f);
			prettySparkleParticle2.LocalPosition = settings.PositionInWorld;
			prettySparkleParticle2.Rotation = vector2.ToRotation();
			prettySparkleParticle2.Scale = new Vector2(4f, 1f) * 0.7f;
			prettySparkleParticle2.FadeInNormalizedTime = 5E-06f;
			prettySparkleParticle2.FadeOutNormalizedTime = 0.95f;
			prettySparkleParticle2.TimeToLive = num;
			prettySparkleParticle2.FadeOutEnd = num;
			prettySparkleParticle2.FadeInEnd = num / 2f;
			prettySparkleParticle2.FadeOutStart = num / 2f;
			prettySparkleParticle2.LocalPosition -= vector2 * num * 0.25f;
			prettySparkleParticle2.Velocity = vector2;
			prettySparkleParticle2.DrawVerticalAxis = false;
			if (num4 == 1f)
			{
				prettySparkleParticle2.Scale *= 1.5f;
				prettySparkleParticle2.Velocity *= 1.5f;
				prettySparkleParticle2.LocalPosition -= prettySparkleParticle2.Velocity * 4f;
			}
			Main.ParticleSystem_World_OverPlayers.Add(prettySparkleParticle2);
			for (int i = 0; i < 2; i++)
			{
				Dust dust = Dust.NewDustPerfect(settings.PositionInWorld, 75, vector2.RotatedBy(Main.rand.NextFloatDirection() * ((float)Math.PI * 2f) * 0.025f) * Main.rand.NextFloat());
				dust.noGravity = true;
				dust.scale = 1.4f;
				Dust dust2 = Dust.NewDustPerfect(settings.PositionInWorld, 75, -vector2.RotatedBy(Main.rand.NextFloatDirection() * ((float)Math.PI * 2f) * 0.025f) * Main.rand.NextFloat());
				dust2.noGravity = true;
				dust2.scale = 1.4f;
			}
		}
	}

	private static void Spawn_NightsEdge(ParticleOrchestraSettings settings)
	{
		float num = 30f;
		float num2 = 0f;
		for (float num3 = 0f; num3 < 3f; num3 += 1f)
		{
			PrettySparkleParticle prettySparkleParticle = _poolPrettySparkle.RequestParticle();
			Vector2 vector = ((float)Math.PI / 4f + (float)Math.PI / 4f * num3 + num2).ToRotationVector2() * 4f;
			prettySparkleParticle.ColorTint = new Color(0.25f, 0.1f, 0.5f, 0.5f);
			prettySparkleParticle.LocalPosition = settings.PositionInWorld;
			prettySparkleParticle.Rotation = vector.ToRotation();
			prettySparkleParticle.Scale = new Vector2(2f, 1f) * 1.1f;
			prettySparkleParticle.FadeInNormalizedTime = 5E-06f;
			prettySparkleParticle.FadeOutNormalizedTime = 0.95f;
			prettySparkleParticle.TimeToLive = num;
			prettySparkleParticle.FadeOutEnd = num;
			prettySparkleParticle.FadeInEnd = num / 2f;
			prettySparkleParticle.FadeOutStart = num / 2f;
			prettySparkleParticle.AdditiveAmount = 0.35f;
			prettySparkleParticle.LocalPosition -= vector * num * 0.25f;
			prettySparkleParticle.Velocity = vector;
			prettySparkleParticle.DrawVerticalAxis = false;
			if (num3 == 1f)
			{
				prettySparkleParticle.Scale *= 1.5f;
				prettySparkleParticle.Velocity *= 1.5f;
				prettySparkleParticle.LocalPosition -= prettySparkleParticle.Velocity * 4f;
			}
			Main.ParticleSystem_World_OverPlayers.Add(prettySparkleParticle);
		}
		for (float num4 = 0f; num4 < 3f; num4 += 1f)
		{
			PrettySparkleParticle prettySparkleParticle2 = _poolPrettySparkle.RequestParticle();
			Vector2 vector2 = ((float)Math.PI / 4f + (float)Math.PI / 4f * num4 + num2).ToRotationVector2() * 4f;
			prettySparkleParticle2.ColorTint = new Color(0.5f, 0.25f, 1f, 1f);
			prettySparkleParticle2.LocalPosition = settings.PositionInWorld;
			prettySparkleParticle2.Rotation = vector2.ToRotation();
			prettySparkleParticle2.Scale = new Vector2(2f, 1f) * 0.7f;
			prettySparkleParticle2.FadeInNormalizedTime = 5E-06f;
			prettySparkleParticle2.FadeOutNormalizedTime = 0.95f;
			prettySparkleParticle2.TimeToLive = num;
			prettySparkleParticle2.FadeOutEnd = num;
			prettySparkleParticle2.FadeInEnd = num / 2f;
			prettySparkleParticle2.FadeOutStart = num / 2f;
			prettySparkleParticle2.LocalPosition -= vector2 * num * 0.25f;
			prettySparkleParticle2.Velocity = vector2;
			prettySparkleParticle2.DrawVerticalAxis = false;
			if (num4 == 1f)
			{
				prettySparkleParticle2.Scale *= 1.5f;
				prettySparkleParticle2.Velocity *= 1.5f;
				prettySparkleParticle2.LocalPosition -= prettySparkleParticle2.Velocity * 4f;
			}
			Main.ParticleSystem_World_OverPlayers.Add(prettySparkleParticle2);
		}
	}

	private static void Spawn_SilverBulletSparkle(ParticleOrchestraSettings settings)
	{
		_ = Main.rand.NextFloat() * ((float)Math.PI * 2f);
		Vector2 movementVector = settings.MovementVector;
		Vector2 vector = new Vector2(Main.rand.NextFloat() * 0.2f + 0.4f);
		Main.rand.NextFloat();
		float rotation = (float)Math.PI / 2f;
		Vector2 vector2 = Main.rand.NextVector2Circular(4f, 4f) * vector;
		PrettySparkleParticle prettySparkleParticle = _poolPrettySparkle.RequestParticle();
		prettySparkleParticle.AccelerationPerFrame = -movementVector * 1f / 30f;
		prettySparkleParticle.Velocity = movementVector;
		prettySparkleParticle.ColorTint = Color.White;
		prettySparkleParticle.LocalPosition = settings.PositionInWorld + vector2;
		prettySparkleParticle.Rotation = rotation;
		prettySparkleParticle.Scale = vector;
		prettySparkleParticle.FadeInNormalizedTime = 5E-06f;
		prettySparkleParticle.FadeOutNormalizedTime = 0.95f;
		prettySparkleParticle.FadeInEnd = 10f;
		prettySparkleParticle.FadeOutStart = 20f;
		prettySparkleParticle.FadeOutEnd = 30f;
		prettySparkleParticle.TimeToLive = 30f;
		Main.ParticleSystem_World_OverPlayers.Add(prettySparkleParticle);
	}

	private static void Spawn_PaladinsHammer(ParticleOrchestraSettings settings)
	{
		float moonLordIndex = Main.rand.NextFloat() * ((float)Math.PI * 2f);
		float i = 1f;
		for (float num = 0f; num < 1f; num += 1f / i)
		{
			float num2 = 0.6f + Main.rand.NextFloat() * 0.35f;
			Vector2 vector = settings.MovementVector * num2;
			Vector2 vector2 = new Vector2(Main.rand.NextFloat() * 0.4f + 0.2f);
			float f = moonLordIndex + Main.rand.NextFloat() * ((float)Math.PI * 2f);
			float rotation = (float)Math.PI / 2f;
			_ = 0.1f * vector2;
			Vector2 vector3 = Main.rand.NextVector2Circular(12f, 12f) * vector2;
			PrettySparkleParticle prettySparkleParticle = _poolPrettySparkle.RequestParticle();
			prettySparkleParticle.AccelerationPerFrame = -vector * 1f / 30f;
			prettySparkleParticle.Velocity = vector + f.ToRotationVector2() * 2f * num2;
			prettySparkleParticle.ColorTint = new Color(1f, 0.8f, 0.4f, 0f);
			prettySparkleParticle.LocalPosition = settings.PositionInWorld + vector3;
			prettySparkleParticle.Rotation = rotation;
			prettySparkleParticle.Scale = vector2;
			prettySparkleParticle.FadeInNormalizedTime = 5E-06f;
			prettySparkleParticle.FadeOutNormalizedTime = 0.95f;
			prettySparkleParticle.TimeToLive = 40f;
			Main.ParticleSystem_World_OverPlayers.Add(prettySparkleParticle);
			prettySparkleParticle = _poolPrettySparkle.RequestParticle();
			prettySparkleParticle.AccelerationPerFrame = -vector * 1f / 30f;
			prettySparkleParticle.Velocity = vector * 0.8f + f.ToRotationVector2() * 2f;
			prettySparkleParticle.ColorTint = new Color(255, 255, 255, 0);
			prettySparkleParticle.LocalPosition = settings.PositionInWorld + vector3;
			prettySparkleParticle.Rotation = rotation;
			prettySparkleParticle.Scale = vector2 * 0.6f;
			prettySparkleParticle.FadeInNormalizedTime = 0.1f;
			prettySparkleParticle.FadeOutNormalizedTime = 0.9f;
			prettySparkleParticle.TimeToLive = 60f;
			Main.ParticleSystem_World_OverPlayers.Add(prettySparkleParticle);
		}
		for (int j = 0; j < 2; j++)
		{
			int num3 = Dust.NewDust(newColor: new Color(1f, 0.7f, 0.3f, 0f), Position: settings.PositionInWorld, Width: 0, Height: 0, Type: 267);
			Main.dust[num3].velocity = Main.rand.NextVector2Circular(2f, 2f);
			Main.dust[num3].velocity += settings.MovementVector * (0.5f + 0.5f * Main.rand.NextFloat()) * 1.4f;
			Main.dust[num3].noGravity = true;
			Main.dust[num3].scale = 0.1f;
			Main.dust[num3].position += Main.rand.NextVector2Circular(16f, 16f);
			Main.dust[num3].velocity = settings.MovementVector;
			if (num3 != 6000)
			{
				Dust dust = Dust.CloneDust(num3);
				dust.scale /= 2f;
				dust.fadeIn *= 0.75f;
				dust.color = new Color(255, 255, 255, 255);
			}
		}
	}

	private static void Spawn_PrincessWeapon(ParticleOrchestraSettings settings)
	{
		float num = Main.rand.NextFloat() * ((float)Math.PI * 2f);
		float num2 = 1f;
		for (float num3 = 0f; num3 < 1f; num3 += 1f / num2)
		{
			Vector2 vector = settings.MovementVector * (0.6f + Main.rand.NextFloat() * 0.35f);
			Vector2 vector2 = new Vector2(Main.rand.NextFloat() * 0.4f + 0.2f);
			float f = num + Main.rand.NextFloat() * ((float)Math.PI * 2f);
			float rotation = (float)Math.PI / 2f;
			Vector2 vector3 = 0.1f * vector2;
			float num4 = 60f;
			Vector2 vector4 = Main.rand.NextVector2Circular(8f, 8f) * vector2;
			PrettySparkleParticle prettySparkleParticle = _poolPrettySparkle.RequestParticle();
			prettySparkleParticle.Velocity = f.ToRotationVector2() * vector3 + vector;
			prettySparkleParticle.AccelerationPerFrame = f.ToRotationVector2() * -(vector3 / num4) - vector * 1f / 30f;
			prettySparkleParticle.AccelerationPerFrame = -vector * 1f / 60f;
			prettySparkleParticle.Velocity = vector * 0.66f;
			prettySparkleParticle.ColorTint = Main.hslToRgb((0.92f + Main.rand.NextFloat() * 0.02f) % 1f, 1f, 0.4f + Main.rand.NextFloat() * 0.25f);
			prettySparkleParticle.ColorTint.A = 0;
			prettySparkleParticle.LocalPosition = settings.PositionInWorld + vector4;
			prettySparkleParticle.Rotation = rotation;
			prettySparkleParticle.Scale = vector2;
			Main.ParticleSystem_World_OverPlayers.Add(prettySparkleParticle);
			prettySparkleParticle = _poolPrettySparkle.RequestParticle();
			prettySparkleParticle.Velocity = f.ToRotationVector2() * vector3 + vector;
			prettySparkleParticle.AccelerationPerFrame = f.ToRotationVector2() * -(vector3 / num4) - vector * 1f / 15f;
			prettySparkleParticle.AccelerationPerFrame = -vector * 1f / 60f;
			prettySparkleParticle.Velocity = vector * 0.66f;
			prettySparkleParticle.ColorTint = new Color(255, 255, 255, 0);
			prettySparkleParticle.LocalPosition = settings.PositionInWorld + vector4;
			prettySparkleParticle.Rotation = rotation;
			prettySparkleParticle.Scale = vector2 * 0.6f;
			Main.ParticleSystem_World_OverPlayers.Add(prettySparkleParticle);
		}
		for (int i = 0; i < 2; i++)
		{
			Color newColor = Main.hslToRgb((0.92f + Main.rand.NextFloat() * 0.02f) % 1f, 1f, 0.4f + Main.rand.NextFloat() * 0.25f);
			int num5 = Dust.NewDust(settings.PositionInWorld, 0, 0, 267, 0f, 0f, 0, newColor);
			Main.dust[num5].velocity = Main.rand.NextVector2Circular(2f, 2f);
			Main.dust[num5].velocity += settings.MovementVector * (0.5f + 0.5f * Main.rand.NextFloat()) * 1.4f;
			Main.dust[num5].noGravity = true;
			Main.dust[num5].scale = 0.1f;
			Main.dust[num5].position += Main.rand.NextVector2Circular(16f, 16f);
			Main.dust[num5].velocity = settings.MovementVector;
			if (num5 != 6000)
			{
				Dust dust = Dust.CloneDust(num5);
				dust.scale /= 2f;
				dust.fadeIn *= 0.75f;
				dust.color = new Color(255, 255, 255, 255);
			}
		}
	}

	private static void Spawn_StardustPunch(ParticleOrchestraSettings settings)
	{
		float num = Main.rand.NextFloat() * ((float)Math.PI * 2f);
		float num2 = 1f;
		for (float num3 = 0f; num3 < 1f; num3 += 1f / num2)
		{
			Vector2 vector = settings.MovementVector * (0.3f + Main.rand.NextFloat() * 0.35f);
			Vector2 vector2 = new Vector2(Main.rand.NextFloat() * 0.4f + 0.4f);
			float f = num + Main.rand.NextFloat() * ((float)Math.PI * 2f);
			float rotation = (float)Math.PI / 2f;
			Vector2 vector3 = 0.1f * vector2;
			float num4 = 60f;
			Vector2 vector4 = Main.rand.NextVector2Circular(8f, 8f) * vector2;
			PrettySparkleParticle prettySparkleParticle = _poolPrettySparkle.RequestParticle();
			prettySparkleParticle.Velocity = f.ToRotationVector2() * vector3 + vector;
			prettySparkleParticle.AccelerationPerFrame = f.ToRotationVector2() * -(vector3 / num4) - vector * 1f / 60f;
			prettySparkleParticle.ColorTint = Main.hslToRgb((0.6f + Main.rand.NextFloat() * 0.05f) % 1f, 1f, 0.4f + Main.rand.NextFloat() * 0.25f);
			prettySparkleParticle.ColorTint.A = 0;
			prettySparkleParticle.LocalPosition = settings.PositionInWorld + vector4;
			prettySparkleParticle.Rotation = rotation;
			prettySparkleParticle.Scale = vector2;
			Main.ParticleSystem_World_OverPlayers.Add(prettySparkleParticle);
			prettySparkleParticle = _poolPrettySparkle.RequestParticle();
			prettySparkleParticle.Velocity = f.ToRotationVector2() * vector3 + vector;
			prettySparkleParticle.AccelerationPerFrame = f.ToRotationVector2() * -(vector3 / num4) - vector * 1f / 30f;
			prettySparkleParticle.ColorTint = new Color(255, 255, 255, 0);
			prettySparkleParticle.LocalPosition = settings.PositionInWorld + vector4;
			prettySparkleParticle.Rotation = rotation;
			prettySparkleParticle.Scale = vector2 * 0.6f;
			Main.ParticleSystem_World_OverPlayers.Add(prettySparkleParticle);
		}
		for (int i = 0; i < 2; i++)
		{
			Color newColor = Main.hslToRgb((0.59f + Main.rand.NextFloat() * 0.05f) % 1f, 1f, 0.4f + Main.rand.NextFloat() * 0.25f);
			int num5 = Dust.NewDust(settings.PositionInWorld, 0, 0, 267, 0f, 0f, 0, newColor);
			Main.dust[num5].velocity = Main.rand.NextVector2Circular(2f, 2f);
			Main.dust[num5].velocity += settings.MovementVector * (0.5f + 0.5f * Main.rand.NextFloat()) * 1.4f;
			Main.dust[num5].noGravity = true;
			Main.dust[num5].scale = 0.6f + Main.rand.NextFloat() * 2f;
			Main.dust[num5].position += Main.rand.NextVector2Circular(16f, 16f);
			if (num5 != 6000)
			{
				Dust dust = Dust.CloneDust(num5);
				dust.scale /= 2f;
				dust.fadeIn *= 0.75f;
				dust.color = new Color(255, 255, 255, 255);
			}
		}
	}

	private static void Spawn_RainbowRodHit(ParticleOrchestraSettings settings)
	{
		float vector = Main.rand.NextFloat() * ((float)Math.PI * 2f);
		float num = 6f;
		float num2 = Main.rand.NextFloat();
		for (float num3 = 0f; num3 < 1f; num3 += 1f / num)
		{
			Vector2 vector2 = settings.MovementVector * Main.rand.NextFloatDirection() * 0.15f;
			Vector2 vector3 = new Vector2(Main.rand.NextFloat() * 0.4f + 0.4f);
			float f = vector + Main.rand.NextFloat() * ((float)Math.PI * 2f);
			float rotation = (float)Math.PI / 2f;
			Vector2 vector4 = 1.5f * vector3;
			float num4 = 60f;
			Vector2 vector5 = Main.rand.NextVector2Circular(8f, 8f) * vector3;
			PrettySparkleParticle prettySparkleParticle = _poolPrettySparkle.RequestParticle();
			prettySparkleParticle.Velocity = f.ToRotationVector2() * vector4 + vector2;
			prettySparkleParticle.AccelerationPerFrame = f.ToRotationVector2() * -(vector4 / num4) - vector2 * 1f / 60f;
			prettySparkleParticle.ColorTint = Main.hslToRgb((num2 + Main.rand.NextFloat() * 0.33f) % 1f, 1f, 0.4f + Main.rand.NextFloat() * 0.25f);
			prettySparkleParticle.ColorTint.A = 0;
			prettySparkleParticle.LocalPosition = settings.PositionInWorld + vector5;
			prettySparkleParticle.Rotation = rotation;
			prettySparkleParticle.Scale = vector3;
			Main.ParticleSystem_World_OverPlayers.Add(prettySparkleParticle);
			prettySparkleParticle = _poolPrettySparkle.RequestParticle();
			prettySparkleParticle.Velocity = f.ToRotationVector2() * vector4 + vector2;
			prettySparkleParticle.AccelerationPerFrame = f.ToRotationVector2() * -(vector4 / num4) - vector2 * 1f / 60f;
			prettySparkleParticle.ColorTint = new Color(255, 255, 255, 0);
			prettySparkleParticle.LocalPosition = settings.PositionInWorld + vector5;
			prettySparkleParticle.Rotation = rotation;
			prettySparkleParticle.Scale = vector3 * 0.6f;
			Main.ParticleSystem_World_OverPlayers.Add(prettySparkleParticle);
		}
		for (int i = 0; i < 12; i++)
		{
			Color newColor = Main.hslToRgb((num2 + Main.rand.NextFloat() * 0.12f) % 1f, 1f, 0.4f + Main.rand.NextFloat() * 0.25f);
			int num5 = Dust.NewDust(settings.PositionInWorld, 0, 0, 267, 0f, 0f, 0, newColor);
			Main.dust[num5].velocity = Main.rand.NextVector2Circular(1f, 1f);
			Main.dust[num5].velocity += settings.MovementVector * Main.rand.NextFloatDirection() * 0.5f;
			Main.dust[num5].noGravity = true;
			Main.dust[num5].scale = 0.6f + Main.rand.NextFloat() * 0.9f;
			Main.dust[num5].fadeIn = 0.7f + Main.rand.NextFloat() * 0.8f;
			if (num5 != 6000)
			{
				Dust dust = Dust.CloneDust(num5);
				dust.scale /= 2f;
				dust.fadeIn *= 0.75f;
				dust.color = new Color(255, 255, 255, 255);
			}
		}
	}

	private static void Spawn_BlackLightningSmall(ParticleOrchestraSettings settings)
	{
		float num = Main.rand.NextFloat() * ((float)Math.PI * 2f);
		float num2 = Main.rand.Next(1, 3);
		float num3 = 0.7f;
		int num4 = 916;
		Main.instance.LoadProjectile(num4);
		Color value = new Color(255, 255, 255, 255);
		Color indigo = Color.Indigo;
		indigo.A = 0;
		for (float num5 = 0f; num5 < 1f; num5 += 1f / num2)
		{
			float f = (float)Math.PI * 2f * num5 + num + Main.rand.NextFloatDirection() * 0.25f;
			float num6 = Main.rand.NextFloat() * 4f + 0.1f;
			Vector2 vector = Main.rand.NextVector2Circular(12f, 12f) * num3;
			Color.Lerp(Color.Lerp(Color.Black, indigo, Main.rand.NextFloat() * 0.5f), value, Main.rand.NextFloat() * 0.6f);
			Color colorTint = new Color(0, 0, 0, 255);
			int num7 = Main.rand.Next(4);
			if (num7 == 1)
			{
				colorTint = Color.Lerp(new Color(106, 90, 205, 127), Color.Black, 0.1f + 0.7f * Main.rand.NextFloat());
			}
			if (num7 == 2)
			{
				colorTint = Color.Lerp(new Color(106, 90, 205, 60), Color.Black, 0.1f + 0.8f * Main.rand.NextFloat());
			}
			RandomizedFrameParticle randomizedFrameParticle = _poolRandomizedFrame.RequestParticle();
			randomizedFrameParticle.SetBasicInfo(TextureAssets.Projectile[num4], null, Vector2.Zero, vector);
			randomizedFrameParticle.SetTypeInfo(Main.projFrames[num4], 2, 24f);
			randomizedFrameParticle.Velocity = f.ToRotationVector2() * num6 * new Vector2(1f, 0.5f) * 0.2f + settings.MovementVector;
			randomizedFrameParticle.ColorTint = colorTint;
			randomizedFrameParticle.LocalPosition = settings.PositionInWorld + vector;
			randomizedFrameParticle.Rotation = randomizedFrameParticle.Velocity.ToRotation();
			randomizedFrameParticle.Scale = Vector2.One * 0.5f;
			randomizedFrameParticle.FadeInNormalizedTime = 0.01f;
			randomizedFrameParticle.FadeOutNormalizedTime = 0.5f;
			randomizedFrameParticle.ScaleVelocity = new Vector2(0.025f);
			Main.ParticleSystem_World_OverPlayers.Add(randomizedFrameParticle);
		}
	}

	private static void Spawn_BlackLightningHit(ParticleOrchestraSettings settings)
	{
		float num = Main.rand.NextFloat() * ((float)Math.PI * 2f);
		float num2 = 7f;
		float num3 = 0.7f;
		int num4 = 916;
		Main.instance.LoadProjectile(num4);
		Color value = new Color(255, 255, 255, 255);
		Color indigo = Color.Indigo;
		indigo.A = 0;
		for (float num5 = 0f; num5 < 1f; num5 += 1f / num2)
		{
			float num6 = (float)Math.PI * 2f * num5 + num + Main.rand.NextFloatDirection() * 0.25f;
			float num7 = Main.rand.NextFloat() * 4f + 0.1f;
			Vector2 vector = Main.rand.NextVector2Circular(12f, 12f) * num3;
			Color.Lerp(Color.Lerp(Color.Black, indigo, Main.rand.NextFloat() * 0.5f), value, Main.rand.NextFloat() * 0.6f);
			Color colorTint = new Color(0, 0, 0, 255);
			int num8 = Main.rand.Next(4);
			if (num8 == 1)
			{
				colorTint = Color.Lerp(new Color(106, 90, 205, 127), Color.Black, 0.1f + 0.7f * Main.rand.NextFloat());
			}
			if (num8 == 2)
			{
				colorTint = Color.Lerp(new Color(106, 90, 205, 60), Color.Black, 0.1f + 0.8f * Main.rand.NextFloat());
			}
			RandomizedFrameParticle randomizedFrameParticle = _poolRandomizedFrame.RequestParticle();
			randomizedFrameParticle.SetBasicInfo(TextureAssets.Projectile[num4], null, Vector2.Zero, vector);
			randomizedFrameParticle.SetTypeInfo(Main.projFrames[num4], 2, 24f);
			randomizedFrameParticle.Velocity = num6.ToRotationVector2() * num7 * new Vector2(1f, 0.5f);
			randomizedFrameParticle.ColorTint = colorTint;
			randomizedFrameParticle.LocalPosition = settings.PositionInWorld + vector;
			randomizedFrameParticle.Rotation = num6;
			randomizedFrameParticle.Scale = Vector2.One;
			randomizedFrameParticle.FadeInNormalizedTime = 0.01f;
			randomizedFrameParticle.FadeOutNormalizedTime = 0.5f;
			randomizedFrameParticle.ScaleVelocity = new Vector2(0.05f);
			Main.ParticleSystem_World_OverPlayers.Add(randomizedFrameParticle);
		}
	}

	private static void Spawn_StellarTune(ParticleOrchestraSettings settings)
	{
		float num = Main.rand.NextFloat() * ((float)Math.PI * 2f);
		float num2 = 5f;
		Vector2 vector = new Vector2(0.7f);
		for (float num3 = 0f; num3 < 1f; num3 += 1f / num2)
		{
			float num4 = (float)Math.PI * 2f * num3 + num + Main.rand.NextFloatDirection() * 0.25f;
			Vector2 vector2 = 1.5f * vector;
			float num5 = 60f;
			Vector2 vector3 = Main.rand.NextVector2Circular(12f, 12f) * vector;
			Color colorTint = Color.Lerp(Color.Gold, Color.HotPink, Main.rand.NextFloat());
			if (Main.rand.Next(2) == 0)
			{
				colorTint = Color.Lerp(Color.Violet, Color.HotPink, Main.rand.NextFloat());
			}
			PrettySparkleParticle prettySparkleParticle = _poolPrettySparkle.RequestParticle();
			prettySparkleParticle.Velocity = num4.ToRotationVector2() * vector2;
			prettySparkleParticle.AccelerationPerFrame = num4.ToRotationVector2() * -(vector2 / num5);
			prettySparkleParticle.ColorTint = colorTint;
			prettySparkleParticle.LocalPosition = settings.PositionInWorld + vector3;
			prettySparkleParticle.Rotation = num4;
			prettySparkleParticle.Scale = vector * (Main.rand.NextFloat() * 0.8f + 0.2f);
			Main.ParticleSystem_World_OverPlayers.Add(prettySparkleParticle);
		}
		num2 = 1f;
	}

	private static void Spawn_Keybrand(ParticleOrchestraSettings settings)
	{
		float num = Main.rand.NextFloat() * ((float)Math.PI * 2f);
		float num2 = 3f;
		Vector2 vector = new Vector2(0.7f);
		for (float num3 = 0f; num3 < 1f; num3 += 1f / num2)
		{
			float num4 = (float)Math.PI * 2f * num3 + num + Main.rand.NextFloatDirection() * 0.1f;
			Vector2 vector2 = 1.5f * vector;
			float num5 = 60f;
			Vector2 vector3 = Main.rand.NextVector2Circular(4f, 4f) * vector;
			PrettySparkleParticle prettySparkleParticle = _poolPrettySparkle.RequestParticle();
			prettySparkleParticle.Velocity = num4.ToRotationVector2() * vector2;
			prettySparkleParticle.AccelerationPerFrame = num4.ToRotationVector2() * -(vector2 / num5);
			prettySparkleParticle.ColorTint = Color.Lerp(Color.Gold, Color.OrangeRed, Main.rand.NextFloat());
			prettySparkleParticle.LocalPosition = settings.PositionInWorld + vector3;
			prettySparkleParticle.Rotation = num4;
			prettySparkleParticle.Scale = vector * 0.8f;
			Main.ParticleSystem_World_OverPlayers.Add(prettySparkleParticle);
		}
		num += 1f / num2 / 2f * ((float)Math.PI * 2f);
		num = Main.rand.NextFloat() * ((float)Math.PI * 2f);
		for (float num6 = 0f; num6 < 1f; num6 += 1f / num2)
		{
			float num7 = (float)Math.PI * 2f * num6 + num + Main.rand.NextFloatDirection() * 0.1f;
			Vector2 vector4 = 1f * vector;
			float num8 = 30f;
			Color value = Color.Lerp(Color.Gold, Color.OrangeRed, Main.rand.NextFloat());
			value = Color.Lerp(Color.White, value, 0.5f);
			value.A = 0;
			Vector2 vector5 = Main.rand.NextVector2Circular(4f, 4f) * vector;
			FadingParticle fadingParticle = _poolFading.RequestParticle();
			fadingParticle.SetBasicInfo(TextureAssets.Extra[98], null, Vector2.Zero, Vector2.Zero);
			fadingParticle.SetTypeInfo(num8);
			fadingParticle.Velocity = num7.ToRotationVector2() * vector4;
			fadingParticle.AccelerationPerFrame = num7.ToRotationVector2() * -(vector4 / num8);
			fadingParticle.ColorTint = value;
			fadingParticle.LocalPosition = settings.PositionInWorld + num7.ToRotationVector2() * vector4 * vector * num8 * 0.2f + vector5;
			fadingParticle.Rotation = num7 + (float)Math.PI / 2f;
			fadingParticle.FadeInNormalizedTime = 0.3f;
			fadingParticle.FadeOutNormalizedTime = 0.4f;
			fadingParticle.Scale = new Vector2(0.5f, 1.2f) * 0.8f * vector;
			Main.ParticleSystem_World_OverPlayers.Add(fadingParticle);
		}
		num2 = 1f;
		num = Main.rand.NextFloat() * ((float)Math.PI * 2f);
		for (float num9 = 0f; num9 < 1f; num9 += 1f / num2)
		{
			float num10 = (float)Math.PI * 2f * num9 + num;
			float typeInfo = 30f;
			Color colorTint = Color.Lerp(Color.CornflowerBlue, Color.White, Main.rand.NextFloat());
			colorTint.A = 127;
			Vector2 vector6 = Main.rand.NextVector2Circular(4f, 4f) * vector;
			Vector2 vector7 = Main.rand.NextVector2Square(0.7f, 1.3f);
			FadingParticle fadingParticle2 = _poolFading.RequestParticle();
			fadingParticle2.SetBasicInfo(TextureAssets.Extra[174], null, Vector2.Zero, Vector2.Zero);
			fadingParticle2.SetTypeInfo(typeInfo);
			fadingParticle2.ColorTint = colorTint;
			fadingParticle2.LocalPosition = settings.PositionInWorld + vector6;
			fadingParticle2.Rotation = num10 + (float)Math.PI / 2f;
			fadingParticle2.FadeInNormalizedTime = 0.1f;
			fadingParticle2.FadeOutNormalizedTime = 0.4f;
			fadingParticle2.Scale = new Vector2(0.1f, 0.1f) * vector;
			fadingParticle2.ScaleVelocity = vector7 * 1f / 60f;
			fadingParticle2.ScaleAcceleration = vector7 * (-1f / 60f) / 60f;
			Main.ParticleSystem_World_OverPlayers.Add(fadingParticle2);
		}
	}

	private static void Spawn_FlameWaders(ParticleOrchestraSettings settings)
	{
		float num = 60f;
		for (int i = -1; i <= 1; i++)
		{
			int num2 = Main.rand.NextFromList(new short[3] { 326, 327, 328 });
			Main.instance.LoadProjectile(num2);
			Player player = Main.player[settings.IndexOfPlayerWhoInvokedThis];
			float num3 = Main.rand.NextFloat() * 0.9f + 0.1f;
			Vector2 vector = settings.PositionInWorld + new Vector2((float)i * 5.33333349f, 0f);
			FlameParticle flameParticle = _poolFlame.RequestParticle();
			flameParticle.SetBasicInfo(TextureAssets.Projectile[num2], null, Vector2.Zero, vector);
			flameParticle.SetTypeInfo(num, settings.IndexOfPlayerWhoInvokedThis, player.cFlameWaker);
			flameParticle.FadeOutNormalizedTime = 0.4f;
			flameParticle.ScaleAcceleration = Vector2.One * num3 * (-1f / 60f) / num;
			flameParticle.Scale = Vector2.One * num3;
			Main.ParticleSystem_World_BehindPlayers.Add(flameParticle);
			if (Main.rand.Next(16) == 0)
			{
				Dust dust = Dust.NewDustDirect(vector, 4, 4, 6, 0f, 0f, 100);
				if (Main.rand.Next(2) == 0)
				{
					dust.noGravity = true;
					dust.fadeIn = 1.15f;
				}
				else
				{
					dust.scale = 0.6f;
				}
				dust.velocity *= 0.6f;
				dust.velocity.Y -= 1.2f;
				dust.noLight = true;
				dust.position.Y -= 4f;
				dust.shader = GameShaders.Armor.GetSecondaryShader(player.cFlameWaker, player);
			}
		}
	}

	private static void Spawn_WallOfFleshGoatMountFlames(ParticleOrchestraSettings settings)
	{
		float value = 50f;
		for (int vector = -1; vector <= 1; vector++)
		{
			int tileBatch = Main.rand.NextFromList(new short[3] { 326, 327, 328 });
			Main.instance.LoadProjectile(tileBatch);
			Player graphicsDevice = Main.player[settings.IndexOfPlayerWhoInvokedThis];
			float vector2 = Main.rand.NextFloat() * 0.9f + 0.1f;
			Vector2 distortionTarget = settings.PositionInWorld + new Vector2((float)vector * 5.33333349f, 0f);
			FlameParticle cachedDrawArea = _poolFlame.RequestParticle();
			cachedDrawArea.SetBasicInfo(TextureAssets.Projectile[tileBatch], null, Vector2.Zero, distortionTarget);
			cachedDrawArea.SetTypeInfo(value, settings.IndexOfPlayerWhoInvokedThis, graphicsDevice.cMount);
			cachedDrawArea.FadeOutNormalizedTime = 0.3f;
			cachedDrawArea.ScaleAcceleration = Vector2.One * vector2 * (-1f / 60f) / value;
			cachedDrawArea.Scale = Vector2.One * vector2;
			Main.ParticleSystem_World_BehindPlayers.Add(cachedDrawArea);
			if (Main.rand.Next(8) == 0)
			{
				Dust destination = Dust.NewDustDirect(distortionTarget, 4, 4, 6, 0f, 0f, 100);
				if (Main.rand.Next(2) == 0)
				{
					destination.noGravity = true;
					destination.fadeIn = 1.15f;
				}
				else
				{
					destination.scale = 0.6f;
				}
				destination.velocity *= 0.6f;
				destination.velocity.Y -= 1.2f;
				destination.noLight = true;
				destination.position.Y -= 4f;
				destination.shader = GameShaders.Armor.GetSecondaryShader(graphicsDevice.cMount, graphicsDevice);
			}
		}
	}
}
