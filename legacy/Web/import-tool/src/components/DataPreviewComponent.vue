<template>
	<div>
		<h2>Preview</h2>
		<h3>{{ season_name }}</h3>

		<h4 v-if="season_endDate || season_activeLevel || season_activeXP">Stats</h4>
		<p v-if="season_endDate">End Date: {{ season_endDate }}</p>
		<p v-if="season_activeLevel">Active Level: {{ season_activeLevel }}</p>
		<p v-if="season_activeXP">XP in Level: {{ season_activeXP }} XP</p>
		<br>

		<h4 v-if="season_history.length > 0">History</h4>
		<GameModelComponent v-if="season_history.length > 0" v-for="game in season_history" v-bind="game"/>
	</div>
</template>

<script lang="ts">
import { defaultPreview } from "../core"
import { GameModel } from "../models/GameModel"
import GameModelComponent from "./GameModelComponent.vue"

export default {
    props: {
        season_name: {
            type: String,
            required: true,
            default: defaultPreview().season_name,
        },
        season_endDate: {
            type: String,
            required: true,
            default: defaultPreview().season_endDate,
        },
        season_activeLevel: {
            type: Number,
            required: true,
            default: defaultPreview().season_activeLevel,
        },
        season_activeXP: {
            type: Number,
            required: true,
            default: defaultPreview().season_activeXP,
        },
        season_history: {
            type: [] as GameModel[],
            required: true,
            default: defaultPreview().season_history,
        }
    },
    components: { GameModelComponent }
}
</script>