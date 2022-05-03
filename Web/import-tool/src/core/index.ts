import { GameModel } from "../models/GameModel"

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

    if (data === null || data?.seasons === null || data?.seasons?.length <= 0)
    {
        alert("Inavlid file")
        data = null;
        return defaultPreview();
    }
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

    console.log(ret);
    return ret;
}


const importData = (data: any) => {
    console.log(JSON.stringify(data, null, 4));
}

export { loadFile, defaultPreview, parsePreview,  importData }