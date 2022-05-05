import { GameModel } from "../models/GameModel"
import { doc, setDoc } from "firebase/firestore"; 
import { auth, db } from "../firebase";
import { coreError } from "./error";

const NUM_PREVIEW_HISTORY_ENTRIES = 10;

function loadFile (file: File): any
{
	const reader = new FileReader();

	return new Promise((resolve, reject) => {
		reader.onload = e => {
			const content = e.target?.result?.toString();
			if (content === null) return;
	
			const result = JSON.parse(content as string);
			resolve(result);
		}
		reader.readAsText(file);
	});
}




// Check validity of data
function checkData (data: any): boolean
{
	if (data === null || data === undefined) {
		coreError("FILE_NULL");
		return false;
	}

	if (data.goals === null || data.goals === undefined) {
		coreError("GOALS_NULL");
		return false;
	}

	if (data.streak === null || data.streak === undefined) {
		coreError("STREAK_NULL");
		return false;
	}

	if (data.seasons === null || data.seasons === undefined) {
		coreError("SEASONS_NULL");
		return false;
	}

	if (data.seasons.length <= 0) {
		coreError("SEASONS_EMPTY")
		return false;
	}

	if (data.seasons[0].history === null || data.seasons[0].history === undefined) {
		coreError("LEGACY_SEASONS");
		return false;
	}

	return true;
} 

function defaultPreview (): any
{
	var ret: any = {}

	ret.season_name = "No preview available"
	ret.season_endDate = ""
	ret.season_activeLevel = 0
	ret.season_activeXP = 0
	ret.season_history = []

	return ret;
}

function parsePreview (data: any): any
{
	var ret: any = {}

	if(!checkData(data)) return defaultPreview();
	const season: any = data?.seasons[data?.seasons?.length - 1];

	ret.season_name = season.name as string;
	ret.season_endDate = season.endDate as string;
	ret.season_activeLevel = season.activeBPLevel as number;
	ret.season_activeXP = season.cXP as number;
	ret.season_history = [];
	
	for (let i = season.history.length - 1; i > season.history.length - NUM_PREVIEW_HISTORY_ENTRIES - 1; i--)
	{
		if (season.history[i] === undefined) break;

		const mode: string = season.history[i].gameMode;
		const time: number = season.history[i].time;
		const xp: number = season.history[i].amount;
		const map: string = season.history[i].map;
		const desc: string = season.history[i].description;
		const score: number = season.history[i].score;
		const enemyScore: number = season.history[i].enemyScore;
		const surrenderedWin: boolean = season.history[i].surrenderedWin;
		const surrenderedLoss: boolean = season.history[i].surrenderedLoss;

		var game: GameModel = { mode, time, xp, map, desc, score, enemyScore, surrenderedWin, surrenderedLoss }

		ret.season_history.push(game)
	}

	return ret;
}


const importData = async (data: any) => {
	const userID = auth.currentUser?.uid.toString() as string;

	var goalgroups = [];
	var streak = [];
	var seasons = [];

	if (!checkData(data)) return;

	// Load GoalGroups
	for (var i = 0; i < data.goals.length; i++) {
		var goalgroup: any = {};
		var gg = data.goals[i];

		goalgroup.uuid = gg.uuid;
		goalgroup.name = gg.name;
		goalgroup.goals = gg.goals;

		goalgroups.push(goalgroup);
	}

	// Load Streak
	streak = data.streak;

	// Load Seasons
	for (var i = 0; i < data.seasons.length; i++) {
		var season: any = {};
		var s = data.seasons[i];

		season.uuid = s.uuid;
		season.name = s.name;
		season.endDate = s.endDate;
		season.activeLevel = s.activeBPLevel;
		season.activeXP = s.cXP;
		season.history = []

		// Load Season History
		for (var j = 0; j < s.history.length; j++) {
			var entry: any = {};
			var e = s.history[j]

			entry.uuid = e.uuid;
			entry.mode = e.gameMode;
			entry.time = e.time;
			entry.xp = e.amount;
			entry.map = e.map;
			entry.desc = e.description;
			entry.score = e.score;
			entry.enemyScore = e.enemyScore;
			entry.surrenderedWin = e.surrenderedWin;
			entry.surrenderedLoss = e.surrenderedLoss;

			season.history.push(entry)
		}

		seasons.push(season);
	}

	// Push Data to Database
	await setDoc(doc(db, "users", userID), {
		userid: userID,
		goalgroups: goalgroups,
		streak: streak,
		seasons: seasons,
	}).then(() => {
		alert("Successfully imported VexTrack Data to account: " + auth.currentUser?.displayName);
	});
}

export { loadFile, defaultPreview, parsePreview,  importData }