using Nanoray.PluginManager;
using System.Collections.Generic;
using TheJazMaster.Nibbs.Artifacts;

namespace TheJazMaster.Nibbs;

internal class CombatDialogue : BaseDialogue
{
	internal void Inject() {
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
			{"NibbsDealtDamage", new StoryNode {
				whoDidThat = ModEntry.Instance.NibbsDeck.Deck,
				playerShotJustHit = true,
				minDamageDealtToEnemyThisAction = 1,
				oncePerCombatTags = [
					"NibbsShotThatGuy"
				],
			}},
			{"DealtBigDamage", new StoryNode {
				playerShotJustHit = true,
				minDamageDealtToEnemyThisTurn = 10
			}},
			{"NibbsDealtBigDamage", new StoryNode {
				whoDidThat = ModEntry.Instance.NibbsDeck.Deck,
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
			{"ReturningFromMissing", new StoryNode {
				priority = true,
				lookup = [$"{ModEntry.Instance.Package.Manifest.UniqueName}::ReturningFromMissing"],
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
				lastTurnPlayerStatuses = (ModEntry.Instance.JohnsonApi != null) ? [
					StatusMeta.deckToMissingStatus[ModEntry.Instance.JohnsonApi.JohnsonDeck.Deck]
				] : null,
				priority = true,
				never = ModEntry.Instance.JohnsonApi == null,
				oncePerCombatTags = [
					"johnsonWentMissing"
				],
			}},
			{"EddieWentMissing", new StoryNode {
				lastTurnPlayerStatuses = (ModEntry.Instance.EddieApi != null) ? [
					StatusMeta.deckToMissingStatus[ModEntry.Instance.EddieApi.EddieDeck]
				] : null,
				priority = true,
				never = ModEntry.Instance.EddieApi == null,
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
				lookup = [
					"Grazed"
				],
				oncePerCombat = true,
			}},
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
			{"DealtDragonfireDamage", new StoryNode {
				lookup = [
					"OnDragonfireActivation"
				],
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
			{"NoWarpPrep", new StoryNode {
				turnStart = true,
				maxTurnsThisCombat = 1,
				oncePerRunTags = [
					"ShieldPrepIsGoneYouFool"
				],
				doesNotHaveArtifacts = [
					"ShieldPrep",
					"WarpMastery"
				],
			}},
			{"Hifreq", new StoryNode {
				turnStart = true,
				oncePerRunTags = [
					"ArtifactHiFreqIntercom"
				],
				hasArtifacts = [
					"HiFreqIntercom"
				],
			}},
			{"ShieldBurst", new StoryNode {
				turnStart = true,
				oncePerRunTags = [
					"ArtifactShieldBurst"
				],
				hasArtifacts = [
					"ShieldBurst"
				],
			}},
			{"QuantumEngines", new StoryNode {
				turnStart = true,
				oncePerRunTags = [
					"ArtifactQuantumEngines"
				],
				hasArtifacts = [
					new QuantumEnginesArtifact().Key()
				],
			}},
			{"SugarRush", new StoryNode {
				turnStart = true,
				oncePerRunTags = [
					"ArtifactSugarRush"
				],
				hasArtifacts = [
					new SugarRushArtifact().Key()
				],
			}},
			{"EyeOfCoba", new StoryNode {
				turnStart = true,
				oncePerRunTags = [
					"ArtifactEyeOfCoba"
				],
				hasArtifacts = [
					new EyeOfCobaArtifact().Key()
				],
			}},
			{"DragonfireCandle", new StoryNode {
				turnStart = true,
				oncePerRunTags = [
					"ArtifactDragonfireCandle"
				],
				hasArtifacts = [
					new DragonfireCandleArtifact().Key()
				],
			}},
			{"ControlRods", new StoryNode {
				turnStart = true,
      			maxTurnsThisCombat = 1,
				oncePerRunTags = [
					"AresCannon"
				],
				hasArtifacts = [
					"AresCannon"
				],
			}},
			{"GeminiCore", new StoryNode {
				turnStart = true,
      			maxTurnsThisCombat = 1,
				oncePerRunTags = [
					"GeminiCore"
				],
				hasArtifacts = [
					"GeminiCore"
				],
			}},
			{"GeminiCoreBooster", new StoryNode {
				turnStart = true,
      			maxTurnsThisCombat = 1,
				oncePerRunTags = [
					"GeminiCoreBooster"
				],
				hasArtifacts = [
					"GeminiCoreBooster"
				],
			}},
			{"Tiderunner", new StoryNode {
				turnStart = true,
      			maxTurnsThisCombat = 1,
				oncePerCombatTags = [
					"Tiderunner"
				],
				oncePerRun = true,
				hasArtifacts = [
					"Tiderunner"
				],
			}},
			{"Squadron", new StoryNode {
				turnStart = true,
      			maxTurnsThisCombat = 1,
				oncePerRunTags = [
					"Squadron"
				],
				hasArtifacts = [
					"APurpleApple.Shipyard::ArtifactSquadron"
				],
			}},
			{"StartedBattleAgainstBigCrystal", new StoryNode {
				turnStart = true,
				priority = true,
				oncePerRun = true,
				maxTurnsThisCombat = 1,
      			allPresent = [
					"crystal",
					TranslateChar("Nibbs")
				],
			}},
			{"GotAutododge", new StoryNode {
				lastTurnPlayerStatuses = [
					Status.autododgeRight
				],
				oncePerCombatTags = [
					"gotAutododge"
				],
			}},
			{"GotMagicAutododge", new StoryNode {
				lastTurnPlayerStatuses = [
					ModEntry.Instance.BacktrackAutododgeRightStatus.Status
				],
				oncePerCombatTags = [
					"gotAutododge"
				],
			}},
			{"GotAutododgeLeft", new StoryNode {
				lastTurnPlayerStatuses = [
					Status.autododgeLeft
				],
				oncePerCombatTags = [
					"gotAutododge"
				],
			}},
			{"GotMagicAutododgeLeft", new StoryNode {
				lastTurnPlayerStatuses = [
					ModEntry.Instance.BacktrackAutododgeLeftStatus.Status
				],
				oncePerCombatTags = [
					"gotAutododge"
				],
			}},
			{"GotAutododgeLeftButTheresMissiles", new StoryNode {
				lastTurnPlayerStatuses = [
					Status.autododgeLeft
				],
				anyDronesHostile = [
					"missile_normal",
					"missile_heavy",
					"missile_corrode",
					"missile_seeker",
					"missile_breacher",
					"missile_punch",
					"missile_shaker"
				],
				oncePerCombatTags = [
					"gotAutododgeButMissiles"
				],
			}},
			{"GotMagicAutododgeLeftButTheresMissiles", new StoryNode {
				lastTurnPlayerStatuses = [
					ModEntry.Instance.BacktrackAutododgeLeftStatus.Status
				],
				anyDronesHostile = [
					"missile_normal",
					"missile_heavy",
					"missile_corrode",
					"missile_seeker",
					"missile_breacher",
					"missile_punch",
					"missile_shaker"
				],
				oncePerCombatTags = [
					"gotAutododgeButMissiles"
				],
			}},
			{"UsedAutododge", new StoryNode {
				lookup = [
					"UsedAutododge"
				],
				oncePerCombatTags = [
					"usedAutododge"
				],
			}},
			{"GainedBacktrack", new StoryNode {
				lastTurnPlayerStatuses = [
					ModEntry.Instance.BacktrackRightStatus.Status
				],
				oncePerCombatTags = [
					"gotBacktrack"
				],
			}},
			{"GainedBacktrackLeft", new StoryNode {
				lastTurnPlayerStatuses = [
					ModEntry.Instance.BacktrackLeftStatus.Status
				],
				oncePerCombatTags = [
					"gotBacktrack"
				],
			}},
			{"UsedBacktrack", new StoryNode {
				lookup = [
					"UsedBacktrack"
				],
				oncePerCombatTags = [
					"usedBacktrack"
				],
			}},
			{"GainedTimestop", new StoryNode {
				lastTurnPlayerStatuses = [
					Status.timeStop
				],
				oncePerCombatTags = [
					"gotTimestop"
				],
			}},
			{"GainedTimestopWithOverdrive", new StoryNode {
				lookup = [$"{ModEntry.Instance.Package.Manifest.UniqueName}::SavedOverdriveWithTimestop"],
				whoDidThat = ModEntry.Instance.NibbsDeck.Deck,
				oncePerCombatTags = [
					"gotTimestopAndUsedItWell"
				],
				priority = true
			}},
			{"GainedTimestopWithStunCharge", new StoryNode {
				lookup = [$"{ModEntry.Instance.Package.Manifest.UniqueName}::SavedStunChargeWithTimestop"],
				whoDidThat = ModEntry.Instance.NibbsDeck.Deck,
				oncePerCombatTags = [
					"gotTimestopAndUsedItWell"
				],
				priority = true
			}},
			{"GainedTimestopWithCheapFix", new StoryNode {
				lookup = [$"{ModEntry.Instance.Package.Manifest.UniqueName}::SavedTempCheapWithTimestop"],
				whoDidThat = ModEntry.Instance.NibbsDeck.Deck,
				oncePerCombatTags = [
					"gotTimestopAndUsedItWell"
				],
				priority = true
			}},
			{"GainedTimestopWithAutopilot", new StoryNode {
				lookup = [$"{ModEntry.Instance.Package.Manifest.UniqueName}::SavedAutopilotWithTimestop"],
				whoDidThat = ModEntry.Instance.NibbsDeck.Deck,
				oncePerCombatTags = [
					"gotTimestopAndUsedItWell"
				],
				priority = true
			}},
			{"GainedTimestopWithPerfectShield", new StoryNode {
				lookup = [$"{ModEntry.Instance.Package.Manifest.UniqueName}::SavedAutopilotWithPerfectShield"],
				whoDidThat = ModEntry.Instance.NibbsDeck.Deck,
				oncePerCombatTags = [
					"gotTimestopAndUsedItWell"
				],
				priority = true
			}},
			{"PreventedOverheatWithTimestop", new StoryNode {
				lookup = [$"{ModEntry.Instance.Package.Manifest.UniqueName}::PreventedOverheatWithTimestop"],
				whoDidThat = ModEntry.Instance.NibbsDeck.Deck,
				oncePerRun = true,
				priority = true
			}},
			{"CatSummonNibbs", new StoryNode {
				lookup = [
					"summonNibbs"
				],
				oncePerCombatTags = [
					"summonNibbsTag"
				],
				oncePerRun = true,
			}},
			{"CatSummonNibbsWithoutNibbs", new StoryNode {
				lookup = [
					"summonNibbs"
				],
				nonePresent = [
					TranslateChar("Nibbs")
				],
				oncePerCombatTags = [
					"summonNibbsTag"
				],
				oncePerRun = true,
			}},

			{"Overcharger", new StoryNode {
				turnStart = true,
      			maxTurnsThisCombat = 1,
				oncePerRunTags = [
					"Overcharger"
				],
				hasArtifacts = [
					"Overcharger"
				],
			}},
			{"Wizbo", new StoryNode {
				turnStart = true,
				allPresent = [
					TranslateChar("Wizbo"),
					TranslateChar("Nibbs")
				],
				enemyIntent = "wizardMagic",
			}},
		};

		InjectStory(nodePresets);
		ModEntry.Instance.Helper.Events.OnLoadStringsForLocale += (_, e) => InjectLocalizations(e);
	}


	internal override IFileInfo GetJsonFile()
	{
		return ModEntry.Instance.Package.PackageRoot.GetRelativeDirectory("I18n/en").GetRelativeFile("combat.json");
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