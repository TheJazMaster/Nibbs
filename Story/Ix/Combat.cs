using Nanoray.PluginManager;
using System.Collections.Generic;
using TheJazMaster.Nibbs.Artifacts;
using TheJazMaster.Nibbs.Characters;
using TheJazMaster.Nibbs.Features;

namespace TheJazMaster.Nibbs.Story.Ix;

internal class IxCombatDialogue() : BaseDialogue(
	ModEntry.Instance.IxManager.IxCharacter.CharacterType,
	ModEntry.Instance.Package.PackageRoot.GetRelativeDirectory("I18n/en/Ix").GetRelativeFile("combat.json")
) {
    private static ModEntry Instance => ModEntry.Instance;
	private static IxManager IxManager => Instance.IxManager;
	internal void Inject()
	{
		var nodePresets = new Dictionary<string, StoryNode> {
			{"TookDamage", new StoryNode {
				enemyShotJustHit = true,
				minDamageDealtToPlayerThisTurn = 1,
				maxDamageDealtToPlayerThisTurn = 1
			}},
			{"TookALotOfDamage", new StoryNode {
				enemyShotJustHit = true,
				minDamageDealtToPlayerThisTurn = 3
			}},
			{"TookNonHullDamage", new StoryNode {
				enemyShotJustHit = true,
				maxDamageDealtToPlayerThisTurn = 0
			}},
			{"IxDealtDamage", new StoryNode {
				whoDidThat = IxManager.IxDeck,
				playerShotJustHit = true,
				minDamageDealtToEnemyThisAction = 1,
				oncePerCombatTags = [
					"IxShotThatGuy"
				],
			}},
			{"LaunchedPrism", new StoryNode {
				lastNamedDroneSpawned = "prism",
				anyDrones = [
					"prism"
				]
			}},
			{"LaunchedPerfectPrism", new StoryNode {
				lastNamedDroneSpawned = "perfect_prism",
				anyDrones = [
					"perfect_prism"
				]
			}},
			{"LaunchedMirror", new StoryNode {
				lastNamedDroneSpawned = "mirror",
				anyDrones = [
					"mirror"
				]
			}},
			{"DealtBigDamage", new StoryNode {
				playerShotJustHit = true,
				minDamageDealtToEnemyThisTurn = 10
			}},
			{"IxDealtBigDamage", new StoryNode {
				whoDidThat = IxManager.IxDeck,
				playerShotJustHit = true,
				minDamageDealtToEnemyThisTurn = 5
			}},
			{"Missed", new StoryNode {
				playerShotJustMissed = true,
				oncePerCombat = true,
				doesNotHaveArtifacts = [
					"Recalibrator",
					"GrazerBeam"
				]
			}},
			{"IxMissed", new StoryNode {
				playerShotJustMissed = true,
				whoDidThat = IxManager.IxDeck,
				oncePerCombat = true,
				doesNotHaveArtifacts = [
					"Recalibrator",
					"GrazerBeam"
				]
			}},
			{"AboutToDie", new StoryNode {
				enemyShotJustHit = true,
				maxHull = 2,
				oncePerCombatTags = [
					"aboutToDie"
				],
				oncePerRun = true
			}},
			{"HitArmor", new StoryNode {
				playerShotJustHit = true,
				minDamageBlockedByEnemyArmorThisTurn = 1,
				oncePerCombat = true,
				oncePerRun = true,
			}},
			{"HitArmorALot", new StoryNode {
				playerShotJustHit = true,
				minDamageBlockedByEnemyArmorThisTurn = 3,
				oncePerCombat = true,
				oncePerRun = true,
			}},
			{"UsedArmor", new StoryNode {
				enemyShotJustHit = true,
				minDamageBlockedByPlayerArmorThisTurn = 1,
				oncePerCombatTags = [
					"WowArmorISPrettyCoolHuh"
				],
				oncePerRun = true,
			}},
			{"UsedArmorALot", new StoryNode {
				enemyShotJustHit = true,
				minDamageBlockedByPlayerArmorThisTurn = 3,
				oncePerCombatTags = [
					"YowzaThatWasALOTofArmorBlock"
				],
				oncePerRun = true,
			}},
			{"EmptyHandExcessEnergy", new StoryNode {
				handEmpty = true,
				minEnergy = 1,
			}},
			{"EmptyHandInsaneTurn", new StoryNode {
				handEmpty = true,
				minCardsPlayedThisTurn = 6
			}},
			{"TrashHand", new StoryNode {
				handFullOfTrash = true,
				oncePerCombatTags = [
					"handOnlyHasTrashCards"
				],
				oncePerRun = true,
			}},
			{"UnplayableHand", new StoryNode {
				handFullOfUnplayableCards = true,
				oncePerCombatTags = [
					"handFullOfUnplayableCards"
				],
				oncePerRun = true,
			}},
			{"NoOverlap", new StoryNode {
				priority = true,
				shipsDontOverlapAtAll = true,
				oncePerCombatTags = [
					"NoOverlapBetweenShips"
				],
				oncePerRun = true,
				nonePresent = [
					"crab",
					"scrap"
				],
			}},
			{"NoOverlapButSeeker", new StoryNode {
				priority = true,
				shipsDontOverlapAtAll = true,
				oncePerCombatTags = [
					"NoOverlapBetweenShipsSeeker"
				],
				anyDronesHostile = [
					"missile_seeker"
				],
				oncePerRun = true,
				nonePresent = [
					"crab"
				],
			}},
			{"LongFight", new StoryNode {
				minTurnsThisCombat = 9,
				oncePerCombatTags = [
					"manyTurns"
				],
				oncePerRun = true,
				turnStart = true,
			}},
			{"Corroded", new StoryNode {
				lastTurnPlayerStatuses = [
					Status.corrode
				],
				oncePerRun = true,
			}},
			{"DizzyWentMissing", new StoryNode {
				lastTurnPlayerStatuses = [
					Status.missingDizzy
				],
				priority = true,
				oncePerCombatTags = [
					"dizzyWentMissing"
				],
			}},
			{"RiggsWentMissing", new StoryNode {
				lastTurnPlayerStatuses = [
					Status.missingRiggs
				],
				priority = true,
				oncePerCombatTags = [
					"riggsWentMissing"
				],
			}},
			{"PeriWentMissing", new StoryNode {
				lastTurnPlayerStatuses = [
					Status.missingPeri
				],
				priority = true,
				oncePerCombatTags = [
					"periWentMissing"
				],
			}},
			{"IsaacWentMissing", new StoryNode {
				lastTurnPlayerStatuses = [
					Status.missingIsaac
				],
				priority = true,
				oncePerCombatTags = [
					"isaacWentMissing"
				],
			}},
			{"DrakeWentMissing", new StoryNode {
				lastTurnPlayerStatuses = [
					Status.missingDrake
				],
				priority = true,
				oncePerCombatTags = [
					"drakeWentMissing"
				],
			}},
			{"MaxWentMissing", new StoryNode {
				lastTurnPlayerStatuses = [
					Status.missingMax
				],
				priority = true,
				oncePerCombatTags = [
					"maxWentMissing"
				],
			}},
			{"BooksWentMissing", new StoryNode {
				lastTurnPlayerStatuses = [
					Status.missingBooks
				],
				priority = true,
				oncePerCombatTags = [
					"booksWentMissing"
				],
			}},
			{"CATWentMissing", new StoryNode {
				lastTurnPlayerStatuses = [
					Status.missingCat
				],
				priority = true,
				oncePerCombatTags = [
					"catWentMissing"
				],
			}},
			{"JohnsonWentMissing", new StoryNode {
				lastTurnPlayerStatuses = Instance.JohnsonApi != null ? [
					StatusMeta.deckToMissingStatus[Instance.JohnsonApi.JohnsonDeck.Deck]
				] : null,
				priority = true,
				never = Instance.JohnsonApi == null,
				oncePerCombatTags = [
					"johnsonWentMissing"
				],
			}},
			{"EddieWentMissing", new StoryNode {
				lastTurnPlayerStatuses = Instance.EddieApi != null ? [
					StatusMeta.deckToMissingStatus[Instance.EddieApi.EddieDeck]
				] : null,
				priority = true,
				never = Instance.EddieApi == null,
				oncePerCombatTags = [
					"eddieWentMissing"
				],
			}},
			{"GoingToOverheat", new StoryNode {
				goingToOverheat = true,
				oncePerCombatTags = [
					"OverheatGeneric"
				],
			}},
			{"MissedWithGrazerBeam", new StoryNode {
				playerShotJustMissed = true,
				hasArtifacts = [
					"GrazerBeam"
				],
				oncePerCombat = true,
			}.ApplyModData(StoryVarsManager.JustGrazedKey, true)},
			{"MissedWithRecalibrator", new StoryNode {
				playerShotJustMissed = true,
				hasArtifacts = [
					"Recalibrator"
				],
				oncePerCombat = true,
			}},
			{"MissedWithRecalibratorGrazerBeam", new StoryNode {
				playerShotJustMissed = true,
				hasArtifacts = [
					"Recalibrator",
					"GrazerBeam"
				],
				oncePerCombat = true,
			}},
			{"PiercedArmor", new StoryNode {
				playerShotJustHit = true,
				playerJustPiercedEnemyArmor = true,
				oncePerCombatTags = [
					"EnemyArmorPierced"
				],
				oncePerRun = true,
			}},
			{"ThereIsWeakpoint", new StoryNode {
				enemyHasWeakPart = true,
				oncePerRunTags = [
					"yelledAboutWeakness"
				],
			}},
			{"ThereIsBrittlePoint", new StoryNode {
				enemyHasBrittlePart = true,
				oncePerRunTags = [
					"yelledAboutBrittle"
				],
			}},
			{"ThereIsSecretBrittlePoint", new StoryNode {
				turnStart = true,
				maxTurnsThisCombat = 1,
				hasArtifacts = [
					"FractureDetection"
				],
				oncePerCombatTags = [
					"FractureDetectionBarks"
				],
				oncePerRun = true,
			}},
			{"IonConverter", new StoryNode {
				hasArtifacts = [
					"IonConverter"
				],
				lookup = [
					"IonConverterTrigger"
				],
				oncePerRun = true,
				priority = true,
				oncePerCombatTags = [
					"IonConverterTag"
				],
			}},
			{"StartedBattleAgainstBigCrystal", new StoryNode {
				turnStart = true,
				priority = true,
				oncePerRun = true,
				maxTurnsThisCombat = 1,
				  allPresent = [
					"crystal",
					TranslateChar("Ix")
				],
			}},
			{"GainedPayback", new StoryNode {
				lastTurnPlayerStatuses = [
					Status.payback
				],
				oncePerCombat = true,
				oncePerCombatTags = [
					"GainedPayback"
				],
			}},
			{"GainedTempPayback", new StoryNode {
				lastTurnPlayerStatuses = [
					Status.tempPayback
				],
				oncePerCombatTags = [
					"GainedPayback"
				],
			}},
			{"GainedPerfectShield", new StoryNode {
				lastTurnPlayerStatuses = [
					Status.perfectShield
				],
				oncePerCombat = true
			}},

			{"PrismInMidrowSolo", new StoryNode {
				allPresent = [],
				anyDrones = [
					"prism",
					"perfect_prism"
				],
				oncePerCombatTags = [
					"PrismObservation"
				]
			}},
			{"PrismInMidrow", new StoryNode {
				anyDrones = [
					"prism",
					"perfect_prism"
				],
				oncePerCombatTags = [
					"PrismObservation"
				]
			}},
			{"EnemyPowerdriveIntoMirror", new StoryNode {
				anyDrones = [
					"mirror"
				],
				lastTurnEnemyStatuses = [
					Status.powerdrive
				],
				oncePerCombatTags = [
					"EnemyMirrorPls"
				]
			}},
			{"PrismButOverdrive", new StoryNode {
				anyDrones = [
					"prism"
				],
				lastTurnPlayerStatuses = [
					Status.overdrive
				],
				oncePerCombatTags = [
					"PrismButOverdrive"
				]
			}},
			{"PrismButPowerdrive", new StoryNode {
				anyDrones = [
					"prism"
				],
				lastTurnPlayerStatuses = [
					Status.powerdrive
				],
				oncePerCombatTags = [
					"PrismButOverdrive"
				]
			}},
			{"PerfectPrismAndOverdrive", new StoryNode {
				anyDrones = [
					"perfect_prism"
				],
				lastTurnPlayerStatuses = [
					Status.overdrive
				],
				oncePerCombatTags = [
					"PerfectPrismAndOverdrive"
				]
			}},
			{"PerfectPrismAndPowerdrive", new StoryNode {
				anyDrones = [
					"perfect_prism"
				],
				lastTurnPlayerStatuses = [
					Status.powerdrive
				],
				oncePerCombatTags = [
					"PerfectPrismAndOverdrive"
				]
			}},
			


			{"CatSummonIx", new StoryNode {
				lookup = [
					"summonIx"
				],
				oncePerCombatTags = [
					"summonIxTag"
				],
				oncePerRun = true,
			}},
			{"CatSummonIxWithoutIx", new StoryNode {
				lookup = [
					"summonIx"
				],
				allPresent = [
					TranslateChar("CAT")
				],
				nonePresent = [
					TranslateChar("Ix")
				],
				oncePerCombatTags = [
					"summonIxTag"
				],
				oncePerRun = true,
			}},

			{"SogginsEscapeIntent_2", new StoryNode {
				requiredScenes = [
					"SogginsEscapeIntent_1"
				],
				allPresent = [
					TranslateChar("Soggins"),
					TranslateChar("Ix")
				],
				enemyIntent = "sogginsEscapeIntent",
				turnStart = true,
				priority = true,
				oncePerCombat = true,
				specialFight = "sogginsMissileEvent"
			}},
			{"SogginsEscapeIntent_3", new StoryNode {
				requiredScenes = [
					"TheJazMaster.Nibbs::Ix_SogginsEscapeIntent_2"
				],
				allPresent = [
					TranslateChar("Soggins"),
					TranslateChar("Ix")
				],
				enemyIntent = "sogginsEscapeIntent",
				turnStart = true,
				priority = true,
				oncePerCombat = true,
				specialFight = "sogginsMissileEvent"
			}},
			{"SogginsEscapeIntent_4", new StoryNode {
				requiredScenes = [
					"TheJazMaster.Nibbs::Ix_SogginsEscapeIntent_3"
				],
				allPresent = [
					TranslateChar("Soggins"),
					TranslateChar("Ix")
				],
				enemyIntent = "sogginsEscapeIntent",
				turnStart = true,
				priority = true,
				oncePerCombat = true,
				specialFight = "sogginsMissileEvent"
			}},
			{"SogginsEscapeIntent_5", new StoryNode {
				requiredScenes = [
					"TheJazMaster.Nibbs::Ix_SogginsEscapeIntent_4"
				],
				allPresent = [
					TranslateChar("Soggins"),
					TranslateChar("Ix")
				],
				enemyIntent = "sogginsEscapeIntent",
				turnStart = true,
				priority = true,
				oncePerCombat = true,
				specialFight = "sogginsMissileEvent"
			}},
			{"SogginsEscapeIntent_6", new StoryNode {
				requiredScenes = [
					"TheJazMaster.Nibbs::Ix_SogginsEscapeIntent_5"
				],
				allPresent = [
					TranslateChar("Soggins"),
					TranslateChar("Ix")
				],
				enemyIntent = "sogginsEscapeIntent",
				turnStart = true,
				priority = true,
				oncePerCombat = true,
				specialFight = "sogginsMissileEvent"
			}},
		};

		InjectStory(nodePresets);
		Instance.Helper.Events.OnLoadStringsForLocale += (_, e) => InjectLocalizations(e);
	}

	internal override NodeType GetNodeType()
	{
		return NodeType.combat;
	}

	internal override bool FlipPortrait(string who)
	{
		return false;
	}
}