import { GameModel } from "../models/GameModel"

const NUM_PREVIEW_HISTORY_ENTRIES = 10;

var data: any = null

const loadFile = (file: File) => {
    const reader = new FileReader();

    reader.onload = e => {
        const content = e.target?.result?.toString();
        if (content === null) return;

        const result = JSON.parse(content as string);
        data = result
    }
    reader.readAsText(file);
}




// TODO: Fix this

function parsePreview (): any
{
    var ret: any = {}

    console.log(data)
    if (data === null || data?.seasons === null || data?.seasons?.length <= 0)
    {
        alert("Inavlid file")
        data = null;
        return null;
    }
    console.log(data?.seasons)
    const season: any = data?.seasons[data?.seasons?.length - 1];

    ret.season_name = season.name as string;
    ret.season_endDate = season.endDate as string;
    ret.season_activeLevel = season.activeBPLevel as number;
    ret.season_activeXP = season.cXP as number;
    ret.season_history = [];
    
    for (let i = 0; i < NUM_PREVIEW_HISTORY_ENTRIES; i++)
    {
        const mode: string = season.history[i].mode;
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


const parseData = () => {
    console.log(JSON.stringify(data, null, 4));
}

export { loadFile, parsePreview, parseData }