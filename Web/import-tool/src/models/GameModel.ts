export interface GameModel {
	mode: string;
	time: number;
	xp: number;
	map: string;
	desc: string;
	score: number;
	enemyScore: number;
	surrenderedWin: boolean;
	surrenderedLoss: boolean;
}